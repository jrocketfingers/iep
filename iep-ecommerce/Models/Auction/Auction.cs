using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iep_ecommerce.Models
{
    public class Bid
    {
        [Key]
        public Guid Id { get; set; }

        public double value = 1;
    }

    public class Auction
    {
        public enum State { DRAFT, READY, OPEN, SOLD, EXPIRED };

        private string name;
        private long duration;
        private double startingPrice;

        public class AuctionNotReadyException: Exception { }

        public Auction() { }

        public Auction(string name, double startingPrice)
        {
            Status = Auction.State.DRAFT;
            this.name = name;
            this.startingPrice = startingPrice;

            Bids = new List<Bid>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name {
            get { return name; }
            set
            {
                if (Status != State.DRAFT)
                    throw new AuctionNotReadyException();

                name = value;
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

        public DateTime CreatedAt { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public State Status { get; set; }

        public bool startAuction()
        {
            if (Status != Auction.State.DRAFT)
                return false;

            Status = Auction.State.OPEN;
            OpenedAt = DateTime.Now;

            return true;
        }

        public ICollection<Bid> Bids { get; set; }

        public void bid(Bid bid)
        {
            Bids.Add(bid);
        }

        public double getValue()
        {
            double value = startingPrice;

            foreach(Bid b in Bids)
            {
                value += b.value;
            }

            return value;
        }
    }
}