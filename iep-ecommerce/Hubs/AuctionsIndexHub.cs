using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using iep_ecommerce.Models;
using System.Threading.Tasks;

namespace iep_ecommerce.Hubs
{
    public class AuctionsIndexHub : Hub
    {
        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            Groups.Add(Context.ConnectionId, name);

            return base.OnConnected();
        }

        public void Close(Guid id, String winner)
        {
            Clients.All.closeAuction(id, winner);
        }

        [Authorize]
        public void Bid(Guid id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            String userId = Context.User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(userId);

            Auction auction = db.Auctions.FirstOrDefault(x => x.Id == id);

            if(auction.getLastBidder() == user)
            {
                Clients.Group(user.Email).Toast("You already hold the last bid on this auction.");

                return;
            }

            try
            {
                var outbiddenUser = auction.getLastBidder();
                auction.bid(user, db);
                Clients.All.Update(new { id = auction.Id, status = auction.Status, leader = user.UserName, timestamp = auction.ClosesAt });

                Clients.Group(user.Email).Toast("You have successfully made a bid on " + auction.Title);

                if(outbiddenUser != null && user != outbiddenUser)
                    Clients.Group(outbiddenUser.Email).Toast("You have been outbidden on " + auction.Title + " by " + user.UserName);
            }
            catch (NotEnoughTokensException)
            {
                Clients.User(Context.User.Identity.Name).NotEnoughTokens();
            }
        }
    }
}