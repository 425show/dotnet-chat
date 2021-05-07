using System;
using System.Linq;
using System.Threading.Tasks;
using chat.Commands;
using Microsoft.Identity.Client;

namespace chat
{
    class Program
    {
        static string _accessToken = string.Empty;
        static string _redirectUri = "http://localhost";
        static string _clientId = "";
        static string _scope = "";

        static async Task Main(string[] args)
        {
            IPublicClientApplication app = PublicClientApplicationBuilder
                .Create(_clientId)
                .WithRedirectUri(_redirectUri)
                .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                .Build();

            AuthenticationResult result;
            var accounts = await app.GetAccountsAsync();
            var scopes = new string[] { _scope };

            try
            { 
                result = await app.AcquireTokenSilent(scopes, 
                    accounts.FirstOrDefault()).ExecuteAsync(); 
            }
            catch (MsalUiRequiredException) 
            { 
                result = await app.AcquireTokenInteractive(scopes).ExecuteAsync(); 
            }

            if(result != null)
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
