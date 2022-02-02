using System.Collections.Generic;

namespace NP.Auction.Client.Contracts
{
    public class AreaPrice
    {
        public string AreaCode { get; set; }
        public List<CurrencyPrice> Prices { get; set; }
    }
}