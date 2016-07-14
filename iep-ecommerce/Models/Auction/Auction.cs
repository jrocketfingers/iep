using Hangfire;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iep_ecommerce.Models
{
    public class AuctionNotReadyException : Exception { }
    public class IncorrectAuctionStateException : Exception { }

    public interface IAuctionTime
    {
        DateTime Now { get; }
    }

    class AuctionTime : IAuctionTime
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }

    public class Bid
    {
        public Bid(ApplicationUser user, Auction auction)
        {
            this.Id = Guid.NewGuid();
            this.User = user;
            this.Auction = auction;
        }

        public Bid() { }

        [Key]
        public Guid Id { get; set; }

        public double value = 1;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Auction Auction { get; set; }
    }

    public class Auction
    {
        public enum State { DRAFT, READY, OPEN, SOLD, EXPIRED };

        private IAuctionTime auctionTime;

        private string title;
        private long duration;

        public Auction(IAuctionTime injectedAuctionTime = null) {
            Bids = new List<Bid>();

            auctionTime = injectedAuctionTime ?? new AuctionTime();
        }

        public Auction(string title, double startingPrice, long duration = 300, IAuctionTime injectedAuctionTime = null) : this(injectedAuctionTime)
        {
            Status = Auction.State.DRAFT;
            this.title = title;
            this.StartingPrice = startingPrice;

            Duration = duration;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Title {
            get { return title; }
            set
            {
                if (Status != State.DRAFT)
                    throw new AuctionNotReadyException();

                title = value;
            }
        }

        public double StartingPrice { get; set; }
        public long Duration
        {
            get { return duration; }
            set
            {
                if (Status != State.DRAFT)
                    throw new AuctionNotReadyException();

                duration = value;
            }
        }

        public DateTime? CreatedAt { get; set; }
        public DateTime? OpenedAt { get; set; }
        public DateTime? ClosesAt { get; set; }
        public State Status { get; set; }
        public string FinishAuctionJob { get; set; }

        public double Value
        {
            get
            {
                return getValue();
            }
        }

        public bool start(ApplicationDbContext context)
        {
            if (Status != Auction.State.DRAFT)
                return false;

            Status = Auction.State.OPEN;
            OpenedAt = auctionTime.Now;
            ClosesAt = auctionTime.Now + TimeSpan.FromSeconds(Duration);

            return true;
        }

        public void finish()
        {
            if(Status != Auction.State.OPEN)
            {
                throw new IncorrectAuctionStateException();
            }
        }

        public virtual ICollection<Bid> Bids { get; set; }

        public void bid(ApplicationUser user, ApplicationDbContext context = null)
        {
            var bid = new Bid(user, this);

            user.useToken(context);

            // Reset to 10 seconds, if the bid time is under 10 seconds
            if((ClosesAt - auctionTime.Now) < TimeSpan.FromSeconds(10))
            {
                ClosesAt = auctionTime.Now + TimeSpan.FromSeconds(10);
            }

            this.Bids.Add(bid);

            if (context != null) context.SaveChanges();
        }

        public double getValue()
        {
            double value = StartingPrice;

            foreach(Bid b in Bids)
            {
                value += b.value;
            }

            return value;
        }

        public ApplicationUser getLastBidder()
        {
            var user = (from b in Bids
                        orderby b.CreatedAt descending
                        select b.User).First();

            return user;
        }
    }
}