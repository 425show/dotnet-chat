using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace chat
{
    public class AccessTokenFactory
    {
        private DotNetChatOptions _options;

        public AccessTokenFactory(IOptions<DotNetChatOptions> options)
        {
            _options = options.Value;
        }

        private string AccessToken { get; set; }

        public string GetAccessToken() => AccessToken;

        public async Task Authenticate()
        {
            IPublicClientApplication app = PublicClientApplicationBuilder
                .Create(_options.ClientId)
                .WithRedirectUri(_options.RedirectUrl)
                .WithB2CAuthority(_options.Authority)
                .Build();

            AuthenticationResult result;
            var accounts = await app.GetAccountsAsync();
            var scopes = new string[] { _options.ChatScope };

            try
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            }

            if (result != null)
            {
                AccessToken = result.AccessToken;
            }
        }
    }
}