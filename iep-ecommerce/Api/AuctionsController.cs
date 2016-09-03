using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using iep_ecommerce.Models;

namespace iep_ecommerce.Api
{
    public class AuctionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Auctions
        public IQueryable<Auction> GetAuctions()
        {
            return db.Auctions;
        }

        // GET: api/Auctions/5
        [ResponseType(typeof(Auction))]
        public IHttpActionResult GetAuction(Guid id)
        {
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return NotFound();
            }

            return Ok(auction);
        }

        // PUT: api/Auctions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAuction(Guid id, Auction auction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != auction.Id)
            {
                return BadRequest();
            }

            db.Entry(auction).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuctionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Auctions
        [ResponseType(typeof(Auction))]
        public IHttpActionResult PostAuction(Auction auction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Auctions.Add(auction);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = auction.Id }, auction);
        }

        // DELETE: api/Auctions/5
        [ResponseType(typeof(Auction))]
        public IHttpActionResult DeleteAuction(Guid id)
        {
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return NotFound();
            }

            db.Auctions.Remove(auction);
            db.SaveChanges();

            return Ok(auction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AuctionExists(Guid id)
        {
            return db.Auctions.Count(e => e.Id == id) > 0;
        }
    }
}