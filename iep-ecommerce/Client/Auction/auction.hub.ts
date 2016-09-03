interface IUpdateMessage {
    id: string,
    status: State;
    leader?: string;
    timestamp?: string;
}

interface SignalR {
    auctionsIndexHub: AuctionsHubProxy
}

interface AuctionsHubProxy {
    client: AuctionsClient,
    server: AuctionsServer
}

interface AuctionsClient {
    update: (msg: IUpdateMessage) => void;
    notifyOfVictory: (auctionId: string) => void;
    notEnoughTokens: () => void;
    toast: (msg: string, duration: number) => void;
}

interface AuctionsServer {
    bid: (id: string) => void;
}