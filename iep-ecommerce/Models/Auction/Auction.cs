﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iep_ecommerce.Models
{
    public class AuctionNotReadyException : Exception { }
    public class NotEnoughTokensException : Exception { }

    public class Bid
    {
        public Bid(ApplicationUser user, Auction auction)
        {
            this.User = user;
            this.Auction = auction;
        }

        [Key]
        public Guid Id { get; set; }

        public double value = 1;

        public ApplicationUser User { get; set; }

        public Auction Auction { get; set; }
    }

    public class Auction
    {
        public enum State { DRAFT, READY, OPEN, SOLD, EXPIRED };

        private string title;
        private long duration;
        private double startingPrice;

        public Auction() { }

        public Auction(string title, double startingPrice)
        {
            Status = Auction.State.DRAFT;
            this.title = title;
            this.startingPrice = startingPrice;

            Bids = new List<Bid>();
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
        public DateTime? ClosedAt { get; set; }
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