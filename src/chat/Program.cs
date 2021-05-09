using System;
using System.Linq;
using System.Threading.Tasks;
using chat.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace chat
{
    class Program
    {
        static string _accessToken = string.Empty;
        static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args);
        static async Task Main(string[] args)
        {
            DotNetChatOptions options = new();

            CreateHostBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    options = configuration
                                .Build()
                                    .GetSection(nameof(DotNetChatOptions))
                                    .Get<DotNetChatOptions>();
                })
                .Build();

            IPublicClientApplication app = PublicClientApplicationBuilder
                .Create(options.ClientId)
                .WithRedirectUri(options.RedirectUrl)
                .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                .Build();

            AuthenticationResult result;
            var accounts = await app.GetAccountsAsync();
            var scopes = new string[] { options.ChatScope };

            try
            {
                result = await app.AcquireTokenSilent(scopes,
                    accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            }

            if (result != null)
            {
                _accessToken = result.AccessToken;
            }

            while (true)
            {
                Console.WriteLine("Enter Command:");

                var input = Console.ReadLine();
                HandleInput(input);
            }
        }

        static void HandleInput(string input)
        {
            if (input.StartsWith("connect", StringComparison.OrdinalIgnoreCase))
            {
                ConnectCommand.HandleCommand(input, _accessToken).Wait();
            }
            if (input.StartsWith("receive", StringComparison.OrdinalIgnoreCase))
            {
                ReceiveCommand.HandleCommand(input);
            }
            if (input.StartsWith("say", StringComparison.OrdinalIgnoreCase))
            {
                SayCommand.HandleCommand(input);
            }
        }
    }
}
