$(function () {
    let Auctions = {};
    let wonAuctions: number = 0;
    let $wonAuctions: Element = document.querySelector("#won-auctions");

    $(".auction").each(function (index: number, el: Element) {
        let auction = new Auction(<HTMLElement>el);
        Auctions[auction.id] = auction;
    });

    $(".button-collapse").sideNav();

    window.indexHub = $.connection.auctionsIndexHub;

    window.indexHub.client.update = function (msg: IUpdateMessage) {
        Auctions[msg.id].update(msg);
    };

    window.indexHub.client.notEnoughTokens = function () {
        Materialize.toast("Not enough tokens!", 3000);
    };

    window.indexHub.client.notifyOfVictory = function (auctionId: string) {
        wonAuctions++;

        let badge = null;

        if ((badge = $wonAuctions.querySelector('.new.badge')) === null) {
            badge = document.createElement("span");
            badge.className = "new badge blue";

            $wonAuctions.appendChild(badge);
        }

        badge.textContent = wonAuctions.toString();
    };

    window.indexHub.client.toast = function(msg: string, duration: number) {
        Materialize.toast(msg, duration);
    };

    $.connection.hub.start();
});