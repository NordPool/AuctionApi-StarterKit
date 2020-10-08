# AuctionApi-StarterKit
Rapid onboarding starter kit for .NET (Core and Framework > 4.7.0) the Nord Pool Auctions API

## Content
This solution contains the following projects:

### NP.Auction.Client
- A .NET Standard 2.0 library containing a client for NP Auction REST API with all contracts and handling of authentication/authorization through a separate SSO API Client
- This library can be used within both, .NET Core (version > 2.0) or .NET Framework (version > 4.7.0). For .NET Core use, check the NP.Auction.Client.Core.Extensions project for simple DI registeration using .NET Core built-in DI
- REST API clients have been constructed utilizing 3rd party library [Refit](https://github.com/reactiveui/refit)

### NP.Auction.Client.Console
- A .NET Framework 4.7.2 Console application that demonstrates all key functionalities of the Auction REST API by utilizing the NP.Auction.Client library.
  - Before usage, please specify proper username/password combination in the App.Config

### NP.Auction.Client.Core.Extensions
- A .NET Core 3.1 library that contains a small extension method for easier registration of the Auction REST API Client
  - In case loading of this particular project fails, please make sure that .NET Core 3.1 SDK is installed. You can install the SDK from [here](https://dotnet.microsoft.com/download/dotnet-core/3.1)



