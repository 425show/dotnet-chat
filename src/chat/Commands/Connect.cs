using System;
using System.Threading.Tasks;

namespace chat.Commands
{
    public class ConnectCommand
    {
        public static async Task HandleCommand(string command)
        {
            Console.WriteLine("Starting SignalR Connection");
            await ChatClient.Instance.Connect();
        }
    }
}