using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNet.Identity;
using iep_ecommerce.Models;
using iep_ecommerce.Controllers;
using Moq;

/*
 * 
 **/
namespace iep_ecommerce.Tests.Features
{
    [TestClass]
    public class AuctionTests
    {
        [TestMethod]
        public void TestNoBidsAuctionValue()
        {
            var auction = new Auction("Test Auction", 10);

            Assert.AreEqual(10, auction.getValue());
        }

        [TestMethod]
        public void TestBidsAuctionValue()
        {
            var auction = new Auction("Test Auction", 10);
            var user = new ApplicationUser();
            user.Tokens++;

            auction.Bids.Add(new Bid(user, auction));

            Assert.AreEqual(11, auction.getValue());
        }

        [TestMethod]
        [ExpectedException(typeof(NotEnoughTokensException))]
        public void TestBidWithoutTokens()
        {
            var auction = new Auction("Test Auction", 10);
            var user = new ApplicationUser();

            var context = new ApplicationDbContext();

            auction.bid(user, context);
        }

        [TestMethod]
        public void TestBidResetsClosesAt()
        {
            var time = DateTime.Now;
            var auctionTimeMock = new Mock<IAuctionTime>();

            auctionTimeMock.Setup(x => x.Now).Returns(time);
            
            var auction = new Auction("Test Auction", 10, 5, auctionTimeMock.Object);
            var user = new ApplicationUser();
            user.Tokens++;

            var context = new ApplicationDbContext();

            auction.bid(user, context);

            Assert.AreEqual(TimeSpan.FromSeconds(10), auction.ClosesAt - time);
        }
    }
}
