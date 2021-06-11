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
            while (true)
            {
                Console.WriteLine("Enter Command:");

                var input = Console.ReadLine();
                await HandleInput(input);
            }
        }

        static async Task HandleInput(string input)
        {
            if (input.StartsWith("connect", StringComparison.OrdinalIgnoreCase))
            {
                await ConnectCommand.HandleCommand(input);
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
