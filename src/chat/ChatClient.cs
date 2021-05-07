using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace chat
{
    public class ChatClient
    {
        private HubConnection _connection;

        public static ChatClient Instance { get; private set; }

        static ChatClient()
        {
            Instance = new ChatClient();
        }
        
        public async Task Connect()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/hubs/chat")
                .Build();

            await _connection.StartAsync();
        }

        public async Task Disconnect()
        {
            if(_connection != null && _connection.State == HubConnectionState.Connected)
            await _connection.DisposeAsync();
        }
    }
}