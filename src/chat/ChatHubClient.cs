using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace chat
{
    public class ChatHubClient
    {
        string _hubUrl = "https://localhost:5001/hubs/chat";
        internal HubConnection Connection;
        public async Task Connect(string accessToken)
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(accessToken);
                })
                .Build();

            await Connection.StartAsync();
        }

        public async Task Disconnect()
        {
            if (Connection != null && Connection.State == HubConnectionState.Connected)
                await Connection.DisposeAsync();
        }
    }
}