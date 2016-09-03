using Hangfire;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using iep_ecommerce.Hubs;
using System.Globalization;

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

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[Column(TypeName = "datetime2")]
        public DateTime? CreatedAt { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Auction Auction { get; set; }
    }

    public class Auction
    {
        public enum State { DRAFT, READY, OPEN, SOLD, EXPIRED };

        private IAuctionTime auctionTime;

        private string title;
        private long duration;

        private Hangfire.BackgroundJobClient schedulerClient;

        public Auction() : this(null) { }

        public Auction(IAuctionTime injectedAuctionTime = null, BackgroundJobClient injectedScheduler = null) {
            Bids = new List<Bid>();

            auctionTime = injectedAuctionTime ?? new AuctionTime();
            schedulerClient = injectedScheduler ?? new BackgroundJobClient();
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
        public ApplicationUser Winner { get; set; }

        public double Value
        {
            get
            {
                return getValue();
            }
        }

        public String FormattedValue
        {
            get
            {
                return getValue().ToString("N", CultureInfo.CreateSpecificCulture("Lt-sr-SP"));
            }
        }

        public bool start(ApplicationDbContext context)
        {
            if (Status != Auction.State.READY)
                return false;

            Status = Auction.State.OPEN;
            FinishAuctionJob = schedulerClient.Schedule(() => Auction.finish(this.Id), TimeSpan.FromSeconds(this.Duration));

            OpenedAt = auctionTime.Now;
            ClosesAt = auctionTime.Now + TimeSpan.FromSeconds(Duration);

            return true;
        }

        public bool stop(ApplicationDbContext context)
        {
            if(FinishAuctionJob != null)
            {
                schedulerClient.Delete(this.FinishAuctionJob);
                FinishAuctionJob = null;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Finishes a job.
        /// Called from the Hangfire scheduler. The first attempt at
        /// referencing the model method itself failed, as it just
        /// creates an empty Auction object. This way we're fetching
        /// the proper object from the database.
        /// </summary>
        /// <param name="id"></param>
        public static void finish(Guid id)
        {
            var db = new ApplicationDbContext();

            var auction = db.Auctions.Find(id);

            if(auction.Status != Auction.State.OPEN)
            {
                throw new IncorrectAuctionStateException();
            }

            if (auction.Bids.Count != 0)
            {
                auction.Status = Auction.State.SOLD;
                var lastBidder = auction.getLastBidder();
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<AuctionsIndexHub>();
                hubContext.Clients.Group(lastBidder.Email).notifyOfVictory(auction.Id);
            }
            else
                auction.Status = Auction.State.EXPIRED;

            db.SaveChanges();
        }

        public virtual ICollection<Bid> Bids { get; set; }

        public void bid(ApplicationUser user, ApplicationDbContext context = null)
        {
            var bid = new Bid(user, this);

            bid.CreatedAt = DateTime.UtcNow;

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
                        select b.User).FirstOrDefault();

            return user;
        }

        public String getLastBidderName
        {
            get
            {
                if (getLastBidder() != null)
                    return getLastBidder().UserName;
                else
                    return "No one has yet placed a bid";
            }
        }
    }
}