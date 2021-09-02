namespace NP.Auction.Client.Contracts
{
    using System.Collections.Generic;

    public class CurveOrderPatchRequest
    {
        public List<Curve> Curves { get; set; }
        public string Comment { get; set; }
    }
}