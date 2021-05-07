using System;
using System.Threading.Tasks;

namespace chat.Commands
{
    public class ConnectCommand
    {
        public static async Task HandleCommand(string command, string accessToken)
        {
            Console.WriteLine("Starting SignalR Connection");
            await ChatClient.Instance.Connect(accessToken);
        }
    }
}