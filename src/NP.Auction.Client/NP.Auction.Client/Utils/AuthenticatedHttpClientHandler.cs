namespace NP.Auction.Client.Utils
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using API;

    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly ICachedSsoApiClient _ssoApiClient;

        public AuthenticatedHttpClientHandler(ICachedSsoApiClient ssoApiClient, HttpClientHandler innerHandler = null) :
            base(innerHandler ?? new HttpClientHandler())
        {
            _ssoApiClient = ssoApiClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var auth = request.Headers.Authorization;
            if (auth != null)
            {
                var token = await _ssoApiClient.GetAuthTokenAsync().ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}