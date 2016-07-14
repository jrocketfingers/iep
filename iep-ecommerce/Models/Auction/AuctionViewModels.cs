using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iep_ecommerce.Models {
    public class AuctionIndexViewModel
    {
        public IEnumerable<Auction> Auctions { get; set; }

        public ApplicationUser User { get; set; }
    }
}