$(function () {
    var Auctions = {};
    var wonAuctions = 0;
    var $wonAuctions = document.querySelector("#won-auctions");
    $(".auction").each(function (index, el) {
        var auction = new Auction(el);
        Auctions[auction.id] = auction;
    });
    $(".button-collapse").sideNav();
    window.indexHub = $.connection.auctionsIndexHub;
    window.indexHub.client.update = function (msg) {
        Auctions[msg.id].update(msg);
    };
    window.indexHub.client.notEnoughTokens = function () {
        Materialize.toast("Not enough tokens!", 3000);
    };
    window.indexHub.client.notifyOfVictory = function (auctionId) {
        wonAuctions++;
        var badge = null;
        if ((badge = $wonAuctions.querySelector('.new.badge')) === null) {
            badge = document.createElement("span");
            badge.className = "new badge blue";
            $wonAuctions.appendChild(badge);
        }
        badge.textContent = wonAuctions.toString();
    };
    window.indexHub.client.toast = function (msg, duration) {
        Materialize.toast(msg, duration);
    };
    $.connection.hub.start();
});
//# sourceMappingURL=auctionsIndex.js.map