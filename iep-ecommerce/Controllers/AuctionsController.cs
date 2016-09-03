using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using iep_ecommerce.Models;
using iep_ecommerce.Models.ViewModels;
using Vereyon.Web;
using Hangfire;

namespace iep_ecommerce.Controllers
{
    [Authorize]
    public class AuctionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Auctions
        [AllowAnonymous]
        public ActionResult Index(String SearchString = null, double? LowerPriceBound = null, double? HigherPriceBound = null, Auction.State? Status = null, String Winner = null)
        {
            var AuctionsQuery = from m in db.Auctions
                                select m;

            if(Winner != null)
            {
                ViewBag.Title = "Won auctions";
            }
            else
            {
                ViewBag.Title = "Available auctions";
            }

            IEnumerable<Auction.State> values = Enum.GetValues(typeof(Auction.State)).Cast<Auction.State>();

            if(!User.IsInRole("admin"))
            {

                List<Auction.State> admin_visible_values = new List<Auction.State>();
                admin_visible_values.Add(Auction.State.READY);
                admin_visible_values.Add(Auction.State.DRAFT);

                values = values.Except(admin_visible_values);
            }

            IEnumerable<SelectListItem> status_items = from value in values
                                                       select new SelectListItem
                                                       {
                                                           Text = value.ToString(),
                                                           Value = value.ToString(),
                                                           Selected = value == Status
                                                       };

            ViewBag.Status = status_items;

            if(!User.IsInRole("admin"))
            {
                AuctionsQuery = AuctionsQuery.Where(s => s.Status != Auction.State.DRAFT && s.Status != Auction.State.READY);
            }

            var Auctions = AuctionsQuery.ToList();

            if(!String.IsNullOrEmpty(SearchString))
            {
                Auctions = Auctions.FindAll(s => s.Title.Contains(SearchString));
            }

            if(LowerPriceBound != null)
            {
                Auctions = Auctions.FindAll(s => s.getValue() >= LowerPriceBound);
            }

            if(HigherPriceBound != null)
            {
                Auctions = Auctions.FindAll(s => s.getValue() <= HigherPriceBound);
            }

            if(Status != null)
            {
                Auctions = Auctions.FindAll(s => s.Status == Status);
            }
            else if(!User.IsInRole("admin"))
            {
                Auctions = Auctions.FindAll(s => s.Status == Auction.State.OPEN);
            }

            if(Winner != null)
            {
                Auctions = Auctions.FindAll(s => s.getLastBidder().Id == Winner);
            }

            var userId = User.Identity.GetUserId();

            if(userId != null) {
                var user = db.Users.FirstOrDefault(x => x.Id == userId);
                ViewBag.Tokens = user.Tokens;
            }

            var model = new AuctionIndexViewModel();
            model.Auctions = Auctions;
            model.LowerPriceBound = LowerPriceBound;
            model.HigherPriceBound = HigherPriceBound;
            model.State = Status;

            return View(model);
        }

        // GET: Auctions/Details/5
        [AllowAnonymous]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // GET: Auctions/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,Title,StartingPrice,Duration,CreatedAt,OpenedAt,ClosesAt,Status")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                auction.Id = Guid.NewGuid();
                db.Auctions.Add(auction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(auction);
        }

        // GET: Auctions/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,Title,StartingPrice,Duration,CreatedAt,OpenedAt,ClosedAt,Status")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auction);
        }

        // GET: Auctions/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Auction auction = db.Auctions.Find(id);
            db.Auctions.Remove(auction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Bid(Guid id)
        {
            Auction auction = db.Auctions.FirstOrDefault(x => x.Id == id);

            try
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.FirstOrDefault(x => x.Id == userId);

                auction.bid(user, db);
            }
            catch (NotEnoughTokensException)
            {
                FlashMessage.Queue("<a href=\"" + Url.Action("Buy", "Tokens", null, Request.Url.Scheme) + "\">Buy some.</a>", "Not enough tokens to bid.", FlashMessageType.Danger, true);

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        public ActionResult Start(Guid id)
        {
            var auction = db.Auctions.Find(id);

            auction.start(db);
            db.SaveChanges();

            FlashMessage.Confirmation("Successfully started " + auction.Title);

            return Redirect(Request.UrlReferrer.ToString()); 
        }

        [Authorize(Roles = "admin")]
        public ActionResult Stop(Guid id)
        {
            var auction = db.Auctions.Find(id);

            auction.stop(db);
            db.SaveChanges();

            FlashMessage.Confirmation("Successfully stopped " + auction.Title);

            return Redirect(Request.UrlReferrer.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
