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

        public double LowerBound { get; set; }
        public double UpperBound { get; set; }

        public Auction.Stat
    }
}