using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using iep_ecommerce.Models;

namespace iep_ecommerce.Controllers
{
    public class TokensController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tokens
        public ActionResult Buy()
        {
            var userId = User.Identity.GetUserId();

            var user = db.Users.Find(userId);

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}