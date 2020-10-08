namespace NP.Auction.Client.Exceptions
{
    using System;
    using System.Net;

    public class AuctionApiException : Exception
    {
        public AuctionApiException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public AuctionApiException(string message, Exception innerException, HttpStatusCode httpStatusCode) : base(
            message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusCode HttpStatusCode { get; set; }
    }
}