﻿namespace NP.Auction.Client.Contracts
{
    using System;
    using System.Collections.Generic;

    public class TradesSummary
    {
        public string AreaCode { get; set; }
        public string AuctionId { get; set; }
        public string OrderId { get; set; }
        public string CompanyName { get; set; }
        public string Portfolio { get; set; }
        public string CurrencyCode { get; set; }
        public string UserId { get; set; }
        public OrderResultType OrderResultType { get; set; }
        public string Name { get; set; }
        public string ExclusiveGroup { get; set; }
        public string LinkedTo { get; set; }
        public bool IsSpreadBlock { get; set; }
        public List<Trade> Trades { get; set; }
    }

    public class Trade
    {
        public string TradeId { get; set; }
        public string ContractId { get; set; }
        public DateTime DeliveryStart { get; set; }
        public DateTime DeliveryEnd { get; set; }
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public TradeSide Side { get; set; }
        public AuctionResultState Status { get; set; }
    }
}