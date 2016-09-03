using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using iep_ecommerce.Models;
using iep_ecommerce.Models.Tokens;

namespace iep_ecommerce.Controllers
{
    public class TokensController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult BuyCallback(string userId, Guid orderId)
        {
            var user = db.Users.Find(userId);
            var order = db.Orders.Find(orderId);

            if (order.Status == Models.Tokens.OrderState.SUCCESSFUL)
            {
                user.Tokens += order.NumberOfTokens;
                db.SaveChanges();
            }

            return new HttpStatusCodeResult(200);
        }

        // GET: Tokens
        public ActionResult Buy()
        {
            var userId = User.Identity.GetUserId();

            var user = db.Users.Find(userId);

            var order = new Order { User = user, NumberOfTokens = 1, Status = OrderState.SUCCESSFUL };

            db.Orders.Add(order);
            db.SaveChanges();

            BuyCallback(user.Id, order.Id);

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}