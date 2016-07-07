using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iep_ecommerce.Models.Auction
{
    public class AuctionSearchListViewModel
    {
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Lower Price Bound")]
        public double LowerBound { get; set; }

        [Display(Name = "Upper Price Bound")]
        public double UpperBound { get; set; }

        [Display(Name = "Auction State")]
        public Auction.State State { get; set; }
    }
}