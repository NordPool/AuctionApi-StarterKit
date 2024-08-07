namespace NP.Auction.Client.Contracts
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class ReasonabilityResultsInfo
    {
        public string Portfolio { get; set; }
        public string Area { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderApprovalState OrderApprovalState { get; set; }
        public IEnumerable<ValidatedCurve> Curves { get; set; }
        public string ReferenceDay { get; set; }
        public string AuctionId { get; set; }
        public Guid OrderId { get; set; }
        public string ApprovalModifier { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ApprovalSource ApprovalSource { get; set; }
    }

    public class ValidatedCurve
    {
        public Guid Id { get; set; }
        public int TimeStep { get; set; }
        public string ContractId { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
    }
    
    public enum OrderApprovalState
    {
        Undefined,
        Approved,
        NotApproved
    }
    
    public enum ApprovalSource
    {
        Automatic,
        Operator,
        Member
    }
}