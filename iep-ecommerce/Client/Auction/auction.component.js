var State;
(function (State) {
    State[State["Open"] = 2] = "Open";
    State[State["Sold"] = 3] = "Sold";
    State[State["Expired"] = 4] = "Expired";
    State[State["Awaiting"] = 5] = "Awaiting";
})(State || (State = {}));
;
var ANIMATION_END = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
var Auction = (function () {
    function Auction($el) {
        this.$el = $el;
        this.$bidButton = $el.querySelector(".bid-btn");
        if (this.$bidButton != null)
            this.$bidButton.addEventListener("click", this.bid.bind(this));
        this.$title = $el.querySelector(".auction-title");
        this.$winner = $el.querySelector(".auction-winner");
        this.$timeLeft = $el.querySelector(".time-left");
        this.$timeLeftLabel = $el.querySelector(".time-left-label");
        this.title = this.$title.textContent;
        this.id = this.$el.id;
        this.status = parseInt(this.$el.dataset["status"]);
        this.timerId = setInterval(this.updateTimeLeft.bind(this), 1000, this);
    }
    Object.defineProperty(Auction.prototype, "TimeLeft", {
        get: function () {
            return this.timeLeft;
        },
        set: function (value) {
            this.$timeLeft.textContent = value;
            this.timeLeft = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Auction.prototype, "Winner", {
        get: function () {
            return this.winner;
        },
        set: function (value) {
            //$(this.$winner).addClass('animated fadeOutDown');
            //$(this.$winner).one(ANIMATION_END, () => {
            this.$winner.textContent = value;
            //    $(this.$winner).removeClass('animated fadeOutDown').addClass('animated fadeInDown');
            //    $(this.$winner).one(ANIMATION_END, () => {
            //        $(this.$winner).removeClass('animated fadeOutDown');
            //    });
            //});
            this.winner = value;
        },
        enumerable: true,
        configurable: true
    });
    Auction.prototype.updateTimeLeft = function () {
        var closesAt = moment(this.$el.dataset["closesAt"]);
        var durationString = closesAt.diff(moment());
        var duration = moment.duration(durationString);
        if (duration.asSeconds() <= 0) {
            if (this.status === State.Sold || this.status === State.Expired) {
                this.TimeLeft = closesAt.fromNow();
                this.$timeLeftLabel.textContent = "Finished ";
                this.$bidButton.style.visibility = "hidden";
            }
            else {
                this.$timeLeftLabel.textContent = "Last chance to bid...";
                this.status = State.Awaiting;
                this.TimeLeft = "";
            }
        }
        else {
            this.$timeLeftLabel.textContent = "Time left: ";
            this.TimeLeft = Math.floor(duration.asHours()) + moment.utc(durationString).format(":mm:ss");
        }
    };
    Auction.prototype.bid = function (ev) {
        window.indexHub.server.bid(this.id);
    };
    Auction.prototype.update = function (msg) {
        this.status = msg.status;
        this.Winner = msg.leader;
        if (this.status != undefined) {
            if (this.status != State.Open)
                this.$bidButton.hidden = true;
            else if (this.status == State.Open)
                this.$bidButton.hidden = false;
        }
        if (msg.timestamp !== undefined) {
            this.$el.dataset["closesAt"] = msg.timestamp;
            this.updateTimeLeft();
        }
    };
    return Auction;
}());
//# sourceMappingURL=auction.component.js.map