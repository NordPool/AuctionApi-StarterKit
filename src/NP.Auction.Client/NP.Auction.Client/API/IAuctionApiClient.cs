namespace NP.Auction.Client.API
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Contracts;
    using Exceptions;
    using Newtonsoft.Json;
    using Refit;

    [Headers("Authorization: Bearer")]
    public interface IAuctionApiClient
    {
        const string BasePath = "/api/v1";
        /// <summary>
        ///     Lists all auctions that have "closeForBiddingTime" within provided timespan
        /// </summary>
        /// <param name="closeBiddingFrom">CloseForBidding start period</param>
        /// <param name="closeBiddingTo">CloseForBidding end period</param>
        /// <returns>Collection of auctions <see cref="IEnumerable{Auction}" /></returns>
        [Get("/auctions")]
        Task<IEnumerable<Auction>> GetAuctionsAsync([Query] DateTime closeBiddingFrom,
            [Query] DateTime closeBiddingTo);

        /// <summary>
        ///     Get all trades for selected auction. Auction needs to be in "ResultsPublished" state in order to have trades.
        /// </summary>
        /// <param name="auctionId">Auction id for which the trades should be requested</param>
        /// <param name="portfolios">Collection of portfolios for additional filtering (optional). Provide null for no filtering</param>
        /// <param name="areas">Collection of areas for additional filtering (optional). Provide null for no filtering</param>
        /// <returns>Collection of trades <see cref="IEnumerable{TradeSummary}" /></returns>
        [Get("/auctions/{auctionId}/trades")]
        Task<IEnumerable<TradesSummary>> GetTradesAsync(string auctionId,
            [Query(CollectionFormat.Multi)] string[] portfolios, [Query(CollectionFormat.Multi)] string[] areas);

        /// <summary>
        ///     Get all orders for selected auction.
        /// </summary>
        /// <param name="auctionId">Auction id for which the orders should be requested</param>
        /// <param name="portfolios">Collection of portfolios for additional filtering (optional). Provide null for no filtering</param>
        /// <param name="areas">Collection of areas for additional filtering (optional). Provide null for no filtering</param>
        /// <returns>Collection of different type orders <see cref="OrdersResponse" /></returns>
        [Get("/auctions/{auctionId}/orders")]
        Task<OrdersResponse> GetOrdersAsync(string auctionId,
            [Query(CollectionFormat.Multi)] string[] portfolios, [Query(CollectionFormat.Multi)] string[] areas);

        /// <summary>
        ///     Get prices for selected auction.
        /// </summary>
        /// <param name="auctionId">Auction id for which the prices should be requested</param> 
        /// <returns>Collection of prices <see cref="PricesResponse" /></returns>
        [Get("/auctions/{auctionId}/prices")]
        Task<PricesResponse> GetPricesAsync(string auctionId);
        
        /// <summary>
        ///     Get Reasonability check results for selected auction and order id.
        /// </summary>
        /// <param name="externalAuctionId">Auction id for which the reasonability check results should be requested</param> 
        /// <param name="orderId">Order id for which the reasonability check results should be requested</param> 
        /// <returns>Collection of prices <see cref="ReasonabilityResultsInfo" /></returns>
        [Get("/auctions/{externalAuctionId}/orders/{orderId}/results")]
        Task<ReasonabilityResultsInfo> GetReasonabilityCheckResultsAsync(string externalAuctionId, Guid orderId);

        /// <summary>
        ///     Get portfolio net volumes for selected auction.
        /// </summary>
        /// <param name="auctionId">Auction id for which the portfolio volumes should be requested</param> 
        /// <returns>Collection of portfolio volumes <see cref="PortfolioVolumesResponse" /></returns>
        [Get("/auctions/{auctionId}/portfoliovolumes")]
        Task<PortfolioVolumesResponse> GetPortfolioVolumesAsync(string auctionId,
            [Query(CollectionFormat.Multi)] string[] portfolios, [Query(CollectionFormat.Multi)] string[] areas);

        /// <summary>
        ///     Get contracts with multi resolutions for selected auction.
        /// </summary>
        /// <param name="auctionId">Auction id for which contracts should be requested</param> 
        /// <returns>Auction contracts with multi resolution <see cref="AuctionMultiResolutionResponse" /></returns>
        [Get("/auctions/{auctionId}")]
        Task<AuctionMultiResolutionResponse> GetAuctionAsync(string auctionId);

        /// <summary>
        ///     Cancel all orders for all portfolios for specified auction that user has access to.
        /// </summary>
        /// <param name="auctionId">Auction id for which orders should be cancelled</param> 
        /// <returns>Collection of cancelled order ids <see cref="CancelAllOrdersResponse" /></returns>
        [Delete("/auctions/{auctionId}/orders/cancelall")]
        Task<CancelAllOrdersResponse> CancelAllOrdersForAuctionAsync(string auctionId);

        /// <summary>
        ///     Cancel all orders for specified portfolios and auction that user has access to.
        /// </summary>
        /// <param name="auctionId">Auction id for which orders should be cancelled</param>
        /// <param name="portfolios">Portfolios for which orders should be cancelled</param>  
        /// <returns>Collection of cancelled order ids <see cref="CancelAllOrdersResponse" /></returns>
        [Delete("/auctions/{auctionId}/orders/cancel")]
        Task<CancelAllOrdersResponse> CancelAllOrdersForAuctionAndPortfoliosAsync(string auctionId, [Query(CollectionFormat.Multi)] string[] portfolios);

        /// <summary>
        ///     Post a new curve order through Auction API
        /// </summary>
        /// <param name="curveOrderRequest">Curve order to be placed<see cref="CurveOrderRequest" /></param>
        /// <returns>Placed curve order<see cref="CurveOrder" /></returns>
        [Post("/curveorders")]
        Task<CurveOrder> PostCurveOrderAsync([Body] CurveOrderRequest curveOrderRequest);

        /// <summary>
        ///     Post a new block order through Auction API
        /// </summary>
        /// <param name="curveOrderRequest">Curve order to be placed<see cref="BlockOrderRequest" /></param>
        /// <returns>Placed block order<see cref="BlockList" /></returns>
        [Post("/blockorders")]
        Task<BlockList> PostBlockOrderAsync([Body] BlockOrderRequest blockOrderRequest);

        /// <summary>
        ///     Gets a block order based on provided order id
        /// </summary>
        /// <param name="orderId">Order Id of block order</param>
        /// <returns>A block order for the specified order id <see cref="BlockList" /></returns>
        [Get("/blockorders/{orderId}")]
        Task<BlockList> GetBlockOrderAsync(Guid orderId);

        /// <summary>
        ///     Get all block order versions based on provided order id
        /// </summary>
        /// <param name="orderId">Order id of the block order</param>
        /// <returns>All block order versions for the specified order id <see cref="IEnumerable{BlockList}" /></returns>
        [Get("/blockorders/{orderId}/versions")]
        Task<IEnumerable<BlockList>> GetAllBlockOrderVersionsAsync(Guid orderId);

        /// <summary>
        ///     Gets a curve order based on provided order id
        /// </summary>
        /// <param name="orderId">Order id of the curve order</param>
        /// <returns>A curve order for the specified order id <see cref="CurveOrder" /></returns>
        [Get("/curveorders/{orderId}")]
        Task<CurveOrder> GetCurveOrderAsync(Guid orderId);

        /// <summary>
        ///     Get all curve order versions based on provided order id
        /// </summary>
        /// <param name="orderId">Order id of the curve order</param>
        /// <returns>All curve order versions for the specified order id <see cref="IEnumerable{CurveOrder}" /></returns>
        [Get("/curveorders/{orderId}/versions")]
        Task<IEnumerable<CurveOrder>> GetCurveOrderVersionsAsync(Guid orderId);

        /// <summary>
        ///     Modify existing curve order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="curves">
        ///     Curves to modify (new curves will be added, if empty list is provided then curve order is
        ///     cancelled
        /// </param>
        /// <returns></returns>
        [Patch("/curveorders/{orderId}")]
        Task PatchCurveOrderAsync(Guid orderId, [Body] CurveOrderPatchRequest curveOrderPatchRequest);

        /// <summary>
        ///     Modify existing block order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="blocks">
        ///     Blocks to modify (new blocks will be added, if empty list is provided then block order is
        ///     cancelled
        /// </param>
        /// <returns></returns>
        [Patch("/blockorders/{orderId}")]
        Task PatchBlockOrderAsync(Guid orderId, [Body] BlockOrderPatchRequest blockOrderPatchRequest);

        /// <summary>
        ///     Get all exclusive group order versions based on provided order id.
        ///     Used for both exclusive group and linked exclusive group orders.
        /// </summary>
        /// <param name="orderId">Order id of the exclusive group order</param>
        /// <returns>All versions for the specified order id <see cref="IEnumerable{BlockList}" /></returns>
        [Get("/exclusivegrouporders/{orderId}/versions")]
        Task<IEnumerable<BlockList>> GetAllExclusiveGroupOrderVersionsAsync(Guid orderId);

        /// <summary>
        ///     Gets an exclusive group order based on provided order id.
        ///     Used for both exclusive group and linked exclusive group orders.
        /// </summary>
        /// <param name="orderId">Order id of the exclusive group order</param>
        /// <returns>The exclusive group order for the specified order id <see cref="BlockList" /></returns>
        [Get("/exclusivegrouporders/{orderId}")]
        Task<BlockList> GetExclusiveGroupOrderAsync(Guid orderId);

        /// <summary>
        ///     Post a new exclusive group order through Auction API.
        ///     Used for both exclusive group and linked exclusive group blocks.
        /// </summary>
        /// <param name="blockOrderRequest">Exclusive group order to be placed <see cref="BlockOrderRequest" /></param>
        /// <returns>Placed exclusive group order <see cref="BlockList" /></returns>
        [Post("/exclusivegrouporders")]
        Task<BlockList> PostExclusiveGroupOrderAsync([Body] BlockOrderRequest blockOrderRequest);

        /// <summary>
        ///     Modify an existing exclusive group order.
        ///     Used for both exclusive group and linked exclusive group orders.
        ///     Providing an empty blocks list cancels the order.
        /// </summary>
        /// <param name="orderId">Order id to modify</param>
        /// <param name="blockOrderPatchRequest">Blocks to modify (empty list cancels the order)</param>
        [Patch("/exclusivegrouporders/{orderId}")]
        Task PatchExclusiveGroupOrderAsync(Guid orderId, [Body] BlockOrderPatchRequest blockOrderPatchRequest);
    }

    /// <summary>
    ///     Extending client for better error handling and support of cancelling blocks/curves and not
    ///     yet published prices.
    /// </summary>
    public static class AuctionApiClientExtensions
    {
        public static async Task<CurveOrder> PlaceCurveOrder(this IAuctionApiClient apiClient,
            CurveOrderRequest curveOrder)
        {
            try
            {
                return await apiClient.PostCurveOrderAsync(curveOrder);
            }
            catch (ApiException exception)
            {
                throw ConstructApiException(exception);
            }
        }

        public static async Task ModifyCurveOrder(this IAuctionApiClient apiClient, Guid orderId,
            List<Curve> curves)
        {
            try
            {
                await apiClient.PatchCurveOrderAsync(orderId, new CurveOrderPatchRequest {Curves = curves});
            }
            catch (ApiException exception)
            {
                throw ConstructApiException(exception);
            }
        }

        public static async Task<BlockList> PlaceBlockOrder(this IAuctionApiClient apiClient,
            BlockOrderRequest blockOrder)
        {
            try
            {
                return await apiClient.PostBlockOrderAsync(blockOrder);
            }
            catch (ApiException exception)
            {
                throw ConstructApiException(exception);
            }
        }

        public static async Task<BlockList> PlaceExclusiveGroupOrder(this IAuctionApiClient apiClient,
            BlockOrderRequest blockOrder)
        {
            try
            {
                return await apiClient.PostExclusiveGroupOrderAsync(blockOrder);
            }
            catch (ApiException exception)
            {
                throw ConstructApiException(exception);
            }
        }

        public static async Task CancelBlockOrder(this IAuctionApiClient apiClient, Guid orderId)
        {
            await apiClient.ModifyBlockOrder(orderId, new List<Block>());
        }

        public static async Task CancelExclusiveGroupOrder(this IAuctionApiClient apiClient, Guid orderId)
        {
            await apiClient.ModifyExclusiveGroupOrder(orderId, new List<Block>());
        }

        public static async Task CancelCurveOrder(this IAuctionApiClient apiClient, Guid orderId)
        {
            await apiClient.ModifyCurveOrder(orderId, new List<Curve>());
        }

        public static async Task ModifyBlockOrder(this IAuctionApiClient apiClient, Guid orderId, List<Block> blocks)
        {
            try
            {
                await apiClient.PatchBlockOrderAsync(orderId, new BlockOrderPatchRequest {Blocks = blocks});
            }
            catch (ApiException exception)
            {
                throw ConstructApiException(exception);
            }
        }

        public static async Task ModifyExclusiveGroupOrder(this IAuctionApiClient apiClient, Guid orderId, List<Block> blocks)
        {
            try
            {
                await apiClient.PatchExclusiveGroupOrderAsync(orderId, new BlockOrderPatchRequest { Blocks = blocks });
            }
            catch (ApiException exception)
            {
                throw ConstructApiException(exception);
            }
        }
        public static async Task<PricesResponse> GetPrices(this IAuctionApiClient apiClient,
            string auctionId)
        {
            try
            {
                return await apiClient.GetPricesAsync(auctionId);
            }
            catch (ApiException exception)
            {
                throw ConstructApiException(exception);
            }
        }

        public static async Task<PortfolioVolumesResponse> GetPortfolioVolumes(this IAuctionApiClient apiClient,
            string auctionId, string[] portfolios, string[] areas)
        {
            try
            {
                return await apiClient.GetPortfolioVolumesAsync(auctionId, portfolios, areas);
            }
            catch (ApiException exception)
            {
                throw ConstructApiException(exception);
            }
        }

        public static async Task<AuctionMultiResolutionResponse> GetAuctionContracts(this IAuctionApiClient apiClient, string auctionId)
        {
            try
            {
                return await apiClient.GetAuctionAsync(auctionId);
            }
            catch (ApiException exception)
            {
                throw ConstructApiException(exception);
            }
        }

        private static AuctionApiException ConstructApiException(ApiException exception)
        {
            var additionalErrorMessage = "";
            if (exception.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = JsonConvert.DeserializeObject<dynamic>(exception.Content);
                additionalErrorMessage = (string) content.Message;
            }

            return new AuctionApiException($"Request failed: {additionalErrorMessage}", exception,
                exception.StatusCode);
        }
    }
}