namespace NP.Auction.Client.API
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Configuration;
    using Contracts.SSO;

    public class CachedSsoApiClient : ICachedSsoApiClient
    {
        private const string AuctionScope = "auction_api";
        private const string GrantType = "password";
        private readonly ISsoApiClient _apiClient;
        private readonly AuthConfig _config;

        private string _authToken;

        private DateTime _tokenExpirationTime;

        public CachedSsoApiClient(AuthConfig config, ISsoApiClient apiClient)
        {
            _config = config;
            _apiClient = apiClient;
        }

        public async Task<string> GetAuthTokenAsync()
        {
            if (!ValidTokenExists())
            {
                var token = await _apiClient.PostTokenAsync(ConstructBasicAuthHeader(), ConstructTokenRequest())
                    .ConfigureAwait(false);

                _authToken = token.AccessToken;
                _tokenExpirationTime = DateTime.UtcNow.Add(new TimeSpan(0, 0, token.ExpiresIn));
            }

            return _authToken;
        }

        private bool ValidTokenExists()
        {
            if (string.IsNullOrEmpty(_authToken)) return false;

            return DateTime.UtcNow <= _tokenExpirationTime;
        }

        private string ConstructBasicAuthHeader()
        {
            var authHeader = $"{_config.ClientId}:{_config.ClientSecret}";
            var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(authHeader));

            return $"Basic {base64String}";
        }

        private TokenRequest ConstructTokenRequest()
        {
            return new TokenRequest
            {
                GrantType = GrantType,
                Password = _config.Password,
                Username = _config.Username,
                Scope = AuctionScope
            };
        }
    }
}