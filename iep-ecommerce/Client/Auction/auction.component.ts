enum State { Open = 2, Sold = 3, Expired = 4, Awaiting = 5 };

const ANIMATION_END = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';

class Auction {
    private $bidButton: HTMLElement;
    private $title: Element;
    private $winner: Element;
    private $timeLeft: Element;
    private $timeLeftLabel: Element;

    public id: string;
    private title: string;
    private timeLeft: string;
    private timerId: number;
    private status: State;
    private winner: string;

    public get TimeLeft(): string {
        return this.timeLeft;
    }

    public set TimeLeft(value: string) {
        this.$timeLeft.textContent = value;
        this.timeLeft = value;
    }

    public get Winner(): string {
        return this.winner;
    }

    public set Winner(value: string) {
        //$(this.$winner).addClass('animated fadeOutDown');
        //$(this.$winner).one(ANIMATION_END, () => {
            this.$winner.textContent = value;
        //    $(this.$winner).removeClass('animated fadeOutDown').addClass('animated fadeInDown');

        //    $(this.$winner).one(ANIMATION_END, () => {
        //        $(this.$winner).removeClass('animated fadeOutDown');
        //    });
        //});

        this.winner = value;
    }

    private updateTimeLeft(): void {
        let closesAt = moment(this.$el.dataset["closesAt"]);
        let durationString = closesAt.diff(moment());
        let duration = moment.duration(durationString);
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
        } else {
            this.$timeLeftLabel.textContent = "Time left: ";
            this.TimeLeft = Math.floor(duration.asHours()) + moment.utc(durationString).format(":mm:ss");
        }
    }

    constructor(private $el: HTMLElement) {
        this.$bidButton = <HTMLElement>$el.querySelector(".bid-btn");
        if(this.$bidButton != null)
            this.$bidButton.addEventListener("click", this.bid.bind(this));

        this.$title = $el.querySelector(".auction-title");
        this.$winner = $el.querySelector(".auction-winner");
        this.$timeLeft = $el.querySelector(".time-left");
        this.$timeLeftLabel = $el.querySelector(".time-left-label");

        this.title = this.$title.textContent;
        this.id = this.$el.id;
        this.status = <State>parseInt(this.$el.dataset["status"]);

        this.timerId = setInterval(this.updateTimeLeft.bind(this), 1000, this);
    }

    public bid(ev: MSGesture) {
        window.indexHub.server.bid(this.id);
    }

    public update(msg: IUpdateMessage) {
        this.status = msg.status;
        
        this.Winner = msg.leader;

        if (this.status != undefined) {
            if (this.status != State.Open)
                (<HTMLElement>this.$bidButton).hidden = true;
            else if (this.status == State.Open)
                (<HTMLElement>this.$bidButton).hidden = false;
        }

        if (msg.timestamp !== undefined) {
            this.$el.dataset["closesAt"] = msg.timestamp;
            this.updateTimeLeft();
        }
    }
}
