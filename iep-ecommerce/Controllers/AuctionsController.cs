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
        public ActionResult Index(String searchString = null, double? lowerPriceBound = null, double? higherPriceBound = null, Auction.State? status = null)
        {
            var auctions = from m in db.Auctions
                           select m;

            if(!String.IsNullOrEmpty(searchString))
            {
                auctions = auctions.Where(s => s.Title.Contains(searchString));
            }

            if(lowerPriceBound != null)
            {
                auctions = auctions.Where(s => s.getValue() >= lowerPriceBound);
            }

            if(higherPriceBound != null)
            {
                auctions = auctions.Where(s => s.getValue() <= higherPriceBound);
            }

            if(status != null)
            {
                auctions = auctions.Where(s => s.Status == status);
            }

            var userId = User.Identity.GetUserId();

            if(userId != null) {
                var user = db.Users.FirstOrDefault(x => x.Id == userId);
                ViewBag.Tokens = user.Tokens;
            }

            return View(auctions.ToList());
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
        public ActionResult Create([Bind(Include = "Id,Title,StartingPrice,Duration,CreatedAt,OpenedAt,ClosedAt,Status")] Auction auction)
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
        public ActionResult Edit([Bind(Include = "Id,Name,StartingPrice,Duration,CreatedAt,OpenedAt,ClosedAt,Status")] Auction auction)
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
            var jobid = BackgroundJob.Schedule(() => auction.finish(), TimeSpan.FromSeconds(auction.Duration));
            db.SaveChanges();

            FlashMessage.Confirmation("Successfully started " + auction.Title);

            return RedirectToAction("Index");
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
