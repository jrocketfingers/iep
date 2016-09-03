using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iep_ecommerce.Models.ViewModels
{
    public class AuctionIndexViewModel
    {
        public List<Auction> Auctions { get; set; }
        public String SearchString { get; set; }
        public double? LowerPriceBound { get; set; } = 0;
        public double? HigherPriceBound { get; set; } = 100000;
        public Auction.State? State { get; set; } = Auction.State.OPEN;
    }
}