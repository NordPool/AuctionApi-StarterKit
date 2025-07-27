namespace NP.Auction.Client.Console
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using API;
    using Client.Utils;
    using Configuration;
    using Contracts;
    using Exceptions;
    using Refit;
    using Utils;

    public class Program
    {
        private static IAuctionApiClient _auctionApiClient;
        private static Auction _selectedAuction;
        private static IEnumerable<Auction> _availableAuctions;

        private static async Task Main(string[] args)
        {
            var ssoClient = InitializeSsoClient(ReadAuthorizationConfig());
            _auctionApiClient = InitializeAuctionClient(ssoClient);

            // Fetch open auctions for today
            Console.WriteLine($"Fetching auctions for today {DateTime.Today:d}...");
            _availableAuctions =
                await _auctionApiClient.GetAuctionsAsync(DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1));

            HandleAuctionsCommand();

            var command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);

            while (command != CommandType.Exit)
            {
                switch (command)
                {
                    case CommandType.Orders:
                        await HandleOrdersCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.PlaceCurve:
                        await HandlePlaceCurveCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.PlaceBlocks:
                        await HandlePlaceBlocksCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.PlaceLinkedEg:
                        await HandlePlaceLinkedEgCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.GetAllCurveOrderVersions:
                        await HandleGetAllCurveOrderVersions();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.GetAllBlockOrderVersions:
                        await HandleGetAllBlockOrderVersions();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.GetAllLinkedEgOrderVersions:
                        await HandleGetAllLinkedEgOrderVersions();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.Trades:
                        await HandleTradesCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.CancelBlock:
                        await HandleCancelBlock();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.ModifyBlock:
                        await HandleModifyBlock();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.CancelCurve:
                        await HandleCancelCurve();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.ModifyCurve:
                        await HandleModifyCurve();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.CancelLinkedEg:
                        await HandleCancelLinkedEgOrder();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.ModifyLinkedEg:
                        await HandleModifyLinkedEg();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.CancelAllOrders:
                        await HandleCancelAllOrders();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue; 
                    case CommandType.CancelAllOrdersForPortfolios:
                        await HandleCancelAllOrdersForPortfolios();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.Auctions:
                        HandleAuctionsCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.Prices:
                        await HandlePricesCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.PortfolioVolumes:
                        await HandlePortfolioVolumesCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.AuctionContracts:
                        await HandleAuctionContractsCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                    case CommandType.ReasonabilityCheckResults:
                        await HandleReasonabilityCheckResultsCommand();
                        command = ConsoleHelper.RequestSelectedAuctionCommand(_selectedAuction);
                        continue;
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        private static async Task HandleAuctionContractsCommand()
        {
            Console.WriteLine("------------");
            Console.WriteLine($"Fetching auction with multi resolution contracts for auction {_selectedAuction.Id}...");
            // Fetch contracts for selected auction
            try
            {
                var auction = await _auctionApiClient.GetAuctionContracts(_selectedAuction.Id);
                ConsoleHelper.WriteAuctionContractsInfo(auction);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }

        }

        private static async Task HandleModifyCurve()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide curve order id for modification:");

            var orderId = RequestOrderId();

            try
            {
                var existingCurveOder = await _auctionApiClient.GetCurveOrderAsync(orderId);

                Console.WriteLine("Existing curve order:");
                ConsoleHelper.WriteCurveOrder(existingCurveOder);

                Console.WriteLine("Dropping curve for first contract...");

                existingCurveOder.Curves.RemoveAt(0);

                await _auctionApiClient.ModifyCurveOrder(orderId, existingCurveOder.Curves);

                var modifiedOrder = await _auctionApiClient.GetCurveOrderAsync(orderId);
                Console.WriteLine("Modified curve order:");
                ConsoleHelper.WriteCurveOrder(modifiedOrder);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleCancelCurve()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide curve order id for cancellation:");

            var orderId = RequestOrderId();

            try
            {
                var existingCurveOrder = await _auctionApiClient.GetCurveOrderAsync(orderId);
                Console.WriteLine("Existing curve order:");
                ConsoleHelper.WriteCurveOrder(existingCurveOrder);

                Console.WriteLine("Cancelling curve order...");

                await _auctionApiClient.CancelCurveOrder(orderId);

                var cancelledOrder = await _auctionApiClient.GetCurveOrderAsync(orderId);
                Console.WriteLine("Cancelled curve order:");
                ConsoleHelper.WriteCurveOrder(cancelledOrder);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleCancelLinkedEgOrder()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide linked eg order id for cancellation:");

            var orderId = RequestOrderId();

            try
            {
                var existingLinkedEgOrder = await _auctionApiClient.GetLinkedEgOrderAsync(orderId);
                Console.WriteLine("Existing linked eg order:");
                ConsoleHelper.WriteBlockList(existingLinkedEgOrder);

                Console.WriteLine("Cancelling linked eg order...");

                await _auctionApiClient.CancelLinkedEgOrder(orderId);

                var cancelledOrder = await _auctionApiClient.GetLinkedEgOrderAsync(orderId);
                Console.WriteLine("Cancelled linked eg order:");
                ConsoleHelper.WriteBlockList(cancelledOrder);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleCancelAllOrders()
        {
            Console.WriteLine("-------------");
            Console.WriteLine($"Cancelling all orders for specified portfolios and auction {_selectedAuction.Id}");

            try
            {
                var cancelAllOrdersResponse = await _auctionApiClient.CancelAllOrdersForAuctionAsync(_selectedAuction.Id);
                if (cancelAllOrdersResponse?.CancelledOrderIds?.Count > 0)
                {
                    Console.WriteLine("Cancelled Order Ids:");
                    Console.WriteLine(string.Join(",", cancelAllOrdersResponse.CancelledOrderIds));
                }
                else
                {
                    Console.WriteLine("No orders were cancelled");
                }
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleCancelAllOrdersForPortfolios()
        {
            Console.WriteLine("-------------");
            Console.WriteLine($"Providing portfolios to which orders should be cancelled:");

            var portfolioIds = RequestPortfolioIds();

            Console.WriteLine($"Cancelling all orders for specified portfolios and auction {_selectedAuction.Id}");

            try
            {
                var cancelAllOrdersResponse = await _auctionApiClient.CancelAllOrdersForAuctionAndPortfoliosAsync(_selectedAuction.Id, portfolioIds);
                if (cancelAllOrdersResponse?.CancelledOrderIds?.Count > 0)
                {
                    Console.WriteLine("Cancelled Order Ids:");
                    Console.WriteLine(string.Join(",", cancelAllOrdersResponse.CancelledOrderIds));
                }
                else
                {
                    Console.WriteLine("No orders were cancelled");
                }
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleGetAllCurveOrderVersions()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide curve order id for getting all versions:");

            var orderId = RequestOrderId();

            try
            {
                var curveOrderVersions= await _auctionApiClient.GetCurveOrderVersionsAsync(orderId);
                Console.WriteLine("Curve order versions:");
                foreach (var curveOrderVersion in curveOrderVersions.OrderBy(c => c.Version))
                {
                    Console.WriteLine(
                        $"----------------------Version {curveOrderVersion.Version}-------------------------");
                    ConsoleHelper.WriteCurveOrder(curveOrderVersion);
                }
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleGetAllBlockOrderVersions()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide block order id for getting all versions:");

            var orderId = RequestOrderId();

            try
            {
                var blockOrderVersions = await _auctionApiClient.GetAllBlockOrderVersionsAsync(orderId);
                Console.WriteLine("Block order versions:");
                foreach (var blockOrderVersion in blockOrderVersions.OrderBy(b => b.Version))
                {
                    Console.WriteLine(
                        $"----------------------Version {blockOrderVersion.Version}-------------------------");
                    ConsoleHelper.WriteBlockList(blockOrderVersion);
                }
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleGetAllLinkedEgOrderVersions()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide linked eg order id for getting all versions:");

            var orderId = RequestOrderId();

            try
            {
                var linkedEgOrderVersions = await _auctionApiClient.GetAllLinkedEgOrderVersionsAsync(orderId);
                Console.WriteLine("Linked eg order versions:");
                foreach (var linkedEgOrderVersion in linkedEgOrderVersions.OrderBy(b => b.Version))
                {
                    Console.WriteLine(
                        $"----------------------Version {linkedEgOrderVersion.Version}-------------------------");
                    ConsoleHelper.WriteBlockList(linkedEgOrderVersion);
                }
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleModifyLinkedEg()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide linked eg order id for modification:");

            var orderId = RequestOrderId();

            try
            {
                var existingLinkedEgOrder = await _auctionApiClient.GetLinkedEgOrderAsync(orderId);
                Console.WriteLine("Existing linked eg order:");
                ConsoleHelper.WriteBlockList(existingLinkedEgOrder);
                if (!existingLinkedEgOrder.Blocks.Any() ||
                    existingLinkedEgOrder.Blocks.All(x => x.State == OrderStateType.Cancelled))
                {
                    Console.WriteLine("Existing linked eg order cancelled, modifying by adding blocks...");
                    existingLinkedEgOrder.Blocks = OrderGenerator.GenerateBlocks(BlockOrderType.LinkedExclusiveGroup, _selectedAuction)
                        .ToList();
                }
                else
                {
                    Console.WriteLine("Modifying volume for first block and first period..");
                    existingLinkedEgOrder.Blocks.First().Periods.First().Volume += 50;
                }


                await _auctionApiClient.ModifyLinkedEgOrder(orderId, existingLinkedEgOrder.Blocks);

                var modifiedOrder = await _auctionApiClient.GetLinkedEgOrderAsync(orderId);
                Console.WriteLine("Modified linked eg order:");
                ConsoleHelper.WriteBlockList(modifiedOrder);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static void HandleAuctionsCommand()
        {
            Console.WriteLine("---------");
            ConsoleHelper.WriteAuctionsInfo(_availableAuctions);

            Console.WriteLine("-------------");
            Console.WriteLine("Select an auction for further requests:");
            var selectedAuctionId = Console.ReadLine();
            _selectedAuction = _availableAuctions.First(x => x.Id == selectedAuctionId);
            ConsoleHelper.WriteDetailedAuctionInfo(_selectedAuction);
        }
        
        private static async Task HandleReasonabilityCheckResultsCommand()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide order id for reasonability check results:");
            var orderId = RequestOrderId();
            try
            {
                var reasonabilityCheckResults = await _auctionApiClient.GetReasonabilityCheckResultsAsync(_selectedAuction.Id, orderId);
                Console.WriteLine("Reasonability Check Results:");
                ConsoleHelper.WriteReasonabilityCheckResults(reasonabilityCheckResults);

                
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleModifyBlock()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide block order id for modification:");

            var orderId = RequestOrderId();

            try
            {
                var existingBlockOrder = await _auctionApiClient.GetBlockOrderAsync(orderId);
                Console.WriteLine("Existing block order:");
                ConsoleHelper.WriteBlockList(existingBlockOrder);
                if (!existingBlockOrder.Blocks.Any() ||
                    existingBlockOrder.Blocks.All(x => x.State == OrderStateType.Cancelled))
                {
                    Console.WriteLine("Existing block order cancelled, modifying by adding blocks...");
                    existingBlockOrder.Blocks = OrderGenerator.GenerateBlocks(ResolveBlockOrderType(existingBlockOrder), _selectedAuction)
                        .ToList();
                }
                else
                {
                    Console.WriteLine("Modifying volume for first block and first period..");
                    existingBlockOrder.Blocks.First().Periods.First().Volume += 50;
                }


                await _auctionApiClient.ModifyBlockOrder(orderId, existingBlockOrder.Blocks);

                var modifiedOrder = await _auctionApiClient.GetBlockOrderAsync(orderId);
                Console.WriteLine("Modified block order:");
                ConsoleHelper.WriteBlockList(modifiedOrder);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleCancelBlock()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide block order id for cancellation:");

            var orderId = RequestOrderId();

            try
            {
                var existingBlockOrder = await _auctionApiClient.GetBlockOrderAsync(orderId);
                Console.WriteLine("Existing block order:");
                ConsoleHelper.WriteBlockList(existingBlockOrder);

                Console.WriteLine("Cancelling block order...");

                await _auctionApiClient.CancelBlockOrder(orderId);

                var cancelledOrder = await _auctionApiClient.GetBlockOrderAsync(orderId);
                Console.WriteLine("Cancelled block order:");
                ConsoleHelper.WriteBlockList(cancelledOrder);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandlePlaceCurveCommand()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide portfolio name for placing curve order:");
            var portfolioName = Console.ReadLine();
            Console.WriteLine("Provide area code for placing curve order:");
            var areaCode = Console.ReadLine();

            Console.WriteLine("Provide min price limit for generation (-150 for SEM-GB, -500 for rest)");
            var minPrice = double.Parse(Console.ReadLine() ?? "-500", CultureInfo.InvariantCulture);
            Console.WriteLine("Provide max price limit for generation (1500 for SEM-GB, 3000 for rest)");
            var maxPrice = double.Parse(Console.ReadLine() ?? "3000", CultureInfo.InvariantCulture);

            Console.WriteLine($"Generating curve order for portfolio {portfolioName} with area {areaCode}");
            var curveOrderRequest =
                OrderGenerator.GenerateStaticCurveOrderRequest(portfolioName, areaCode, _selectedAuction, minPrice,
                    maxPrice);

            Console.WriteLine("Generated curve order:");
            ConsoleHelper.WriteCurveOrderRequest(curveOrderRequest);

            Console.WriteLine("Sending generated order to Auction API...");

            try
            {
                var response = await _auctionApiClient.PlaceCurveOrder(curveOrderRequest);
                Console.WriteLine("Curve order placed successfully:");
                ConsoleHelper.WriteCurveOrder(response);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandlePlaceBlocksCommand()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide portfolio name for placing block order:");
            var portfolioName = Console.ReadLine();
            Console.WriteLine("Provide area code for placing block order:");
            var areaCode = Console.ReadLine();
            Console.WriteLine("Provide block type (\"Regular\", \"Linked\", \"Profiled\", \"ExclusiveGroup\", \"Spread\"):");
            BlockOrderType blockOrderType;
            while (!Enum.TryParse(Console.ReadLine(), out blockOrderType))
                Console.WriteLine("Incorrect option specified! Try again.");

            Console.WriteLine(
                $"Generating static block order ({blockOrderType}) for portfolio {portfolioName} with area {areaCode}");
            var blockOrderRequest =
                OrderGenerator.GenerateStaticBlockOrder(portfolioName, areaCode, _selectedAuction, blockOrderType);

            Console.WriteLine("Generated block order:");
            ConsoleHelper.WriteBlockOrderRequest(blockOrderRequest);

            Console.WriteLine("Sending generated block order to Auction API...");

            try
            {
                var response = await _auctionApiClient.PlaceLinkedEgOrder(blockOrderRequest);
                Console.WriteLine("Block order placed successfully:");
                ConsoleHelper.WriteBlockList(response);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandlePlaceLinkedEgCommand()
        {
            Console.WriteLine("-------------");
            Console.WriteLine("Provide portfolio name for placing linked eg order:");
            var portfolioName = Console.ReadLine();
            Console.WriteLine("Provide area code for placing linked eg order:");
            var areaCode = Console.ReadLine();
            Console.WriteLine("Provide resolution for linked eg order (900, 1800, 3600)");

            int resolution;
            while (!int.TryParse(Console.ReadLine(), out resolution))
            {
                Console.WriteLine("Provide again valid resolution for linked eg order (900, 1800, 3600):");
            }

            Console.WriteLine(
                $"Generating static block order ({BlockOrderType.LinkedExclusiveGroup}) for portfolio {portfolioName} with area {areaCode}");
            var linkdedEgOrderRequest =
                OrderGenerator.GenerateStaticBlockOrder(portfolioName, areaCode, _selectedAuction, BlockOrderType.LinkedExclusiveGroup, resolution);

            Console.WriteLine("Generated linked eg order:");
            ConsoleHelper.WriteBlockOrderRequest(linkdedEgOrderRequest);

            Console.WriteLine("Sending generated linked eg order to Auction API...");

            try
            {
                var response = await _auctionApiClient.PlaceLinkedEgOrder(linkdedEgOrderRequest);
                Console.WriteLine("Linked eg order order placed successfully:");
                ConsoleHelper.WriteBlockList(response);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandleOrdersCommand()
        {
            Console.WriteLine("-------------");
            Console.WriteLine($"Fetching orders for auction {_selectedAuction.Id}...");
            // Fetch orders for selected auction
            var orders = await _auctionApiClient.GetOrdersAsync(_selectedAuction.Id, null, null);
            ConsoleHelper.WriteOrdersInfo(orders);
        }

        private static async Task HandleTradesCommand()
        {
            Console.WriteLine("------------");
            Console.WriteLine($"Fetching trades for auction {_selectedAuction.Id}...");
            // Fetch trades for selected auction
            var trades = await _auctionApiClient.GetTradesAsync(_selectedAuction.Id, null, null);
            ConsoleHelper.WriteTradesInfo(trades);
        }

        private static async Task HandlePricesCommand()
        {
            Console.WriteLine("------------");
            Console.WriteLine($"Fetching prices for auction {_selectedAuction.Id}...");

            try
            {
                // Fetch prices for selected auction
                var prices = await _auctionApiClient.GetPrices(_selectedAuction.Id);
                ConsoleHelper.WritePricesInfo(prices);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static async Task HandlePortfolioVolumesCommand()
        {
            Console.WriteLine("------------");
            Console.WriteLine($"Fetching portfolio volumes for auction {_selectedAuction.Id}...");

            try
            {
                // Fetch portfolio net volumes for selected auction
                var portfolioVolumes = await _auctionApiClient.GetPortfolioVolumes(_selectedAuction.Id, null, null);
                ConsoleHelper.WritePortfolioVolumesInfo(portfolioVolumes);
            }
            catch (AuctionApiException exception)
            {
                WriteException(exception);
            }
            catch (ApiException exception)
            {
                WriteException(exception);
            }
        }

        private static string[] RequestPortfolioIds()
        {
            string[] portfolioIds;
            while (true)
            {
                try
                {
                    portfolioIds = Console.ReadLine()?.Split(',').Select(p => p.Trim()).ToArray();
                    break;
                }
                catch (Exception _)
                {
                    Console.WriteLine("Portfolios should be separated by , character. Please try again: ");
                }
            }
            
            return portfolioIds;
        }

        private static Guid RequestOrderId()
        {
            Guid orderId;

            while (!Guid.TryParse(Console.ReadLine(), out orderId))
            {
                Console.WriteLine("Order Id is not a valid GUID. Please try again:");
            }

            return orderId;
        }


        private static AuthConfig ReadAuthorizationConfig()
        {
            return new AuthConfig
            {
                Username = ConfigurationManager.AppSettings["api-username"],
                Password = ConfigurationManager.AppSettings["api-password"],
                ClientSecret = ConfigurationManager.AppSettings["api-clientsecret"],
                ClientId = ConfigurationManager.AppSettings["api-clientid"]
            };
        }

        private static ICachedSsoApiClient InitializeSsoClient(AuthConfig config)
        {
            var ssoUrl = ConfigurationManager.AppSettings["sso-api-url"];
            var ssoClient = RestService.For<ISsoApiClient>(ssoUrl);

            return new CachedSsoApiClient(config, ssoClient);
        }

        private static void WriteException(AuctionApiException apiException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Request failed with following error:");
            Console.WriteLine($"HTTP STATUS: {apiException.HttpStatusCode}");
            Console.WriteLine($"{apiException.Message}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void WriteException(ApiException apiException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Request failed with following error:");
            Console.WriteLine($"HTTP STATUS: {apiException.StatusCode}");
            Console.WriteLine($"{apiException.Message}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static IAuctionApiClient InitializeAuctionClient(ICachedSsoApiClient ssoClient)
        {
            return RestService.For<IAuctionApiClient>(
                new HttpClient(new AuthenticatedHttpClientHandler(ssoClient))
                {
                    BaseAddress = new Uri(ConfigurationManager.AppSettings["auction-api-url"])
                });
        }

        private static BlockOrderType ResolveBlockOrderType(BlockList blockList)
        {
            if (blockList.Blocks.Any(x => x.IsLinkedBlock))
            {
                return BlockOrderType.Linked;
            }

            if (blockList.Blocks.Any(x => x.IsSpreadBlock))
            {
                return BlockOrderType.Spread;
            }

            if (blockList.Blocks.Any(x => x.IsExclusiveGroup))
            {
                return BlockOrderType.ExclusiveGroup;
            }

            if (blockList.Blocks.Any(x => x.IsProfiledBlock))
            {
                return BlockOrderType.Profiled;
            }

            return BlockOrderType.Regular;
        }
    }
}