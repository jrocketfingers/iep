using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iep_ecommerce.Models;

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

            auction.bid(new Bid());

            Assert.AreEqual(11, auction.getValue());
        }
    }
}
