﻿namespace NP.Auction.Client.Contracts
{
    public enum OrderResultType
    {
        Curve = 0,
        Block = 1
    }

    public enum TradeSide
    {
        Buy = 0,
        Sell = 1
    }

    public enum OrderStateType
    {
        New = 0,
        Accepted = 1,
        Cancelled = 2,
        UserAccepted = 3,
        ResultsPublished = 4,
        None = 5
    }

    public enum AuctionStateType
    {
        Open = 0,
        Closed = 1,
        ResultsPublished = 2,
        Cancelled = 3
    }

    public enum PortfolioPermission
    {
        None = 0,
        READ = 1,
        WRITE = 2
    }

    public enum AuctionResultState
    {
        NotAvailable,
        PreliminaryResults,
        Final,
        InitialPrice
    }
}