using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using chat.abstractions;
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

            Connection.On<string>("userDisconnected", (username) =>
            {
                UserPresenceChanged?.Invoke(new UserPresenceChangeEventArgs(username, false));
            });

            Connection.On<string>("userConnected", (username) =>
            {
                UserPresenceChanged?.Invoke(new UserPresenceChangeEventArgs(username, true));
            });

            Connection.On<List<ChatUser>>("activeUserListUpdated", (users) =>
            {
                ActiveUserListChanged?.Invoke(new ActiveUserListChangedEventArgs(users));
            });

            Connection.On<PublicMessage>("publicMessageReceived", (publicMessage) =>
            {
                PublicMessageReceived?.Invoke(new PublicMessageReceivedEventArgs(publicMessage));
            });

            await Connection.StartAsync();
        }

        public async Task SignIn()
        {
            if (Connection?.State == HubConnectionState.Connected)
            {
                await Connection.InvokeAsync("signIn");
            }
        }

        public async Task Disconnect()
        {
            if (Connection?.State == HubConnectionState.Connected)
                await Connection.DisposeAsync();
        }

        public async Task SendPublicMessage(string message)
        {
            if (Connection?.State == HubConnectionState.Connected)
            {
                await Connection.InvokeAsync("sendPublicMessage", message);
            }
        }

        public async Task ChangeDisplayName(string displayName)
        {
            if (Connection?.State == HubConnectionState.Connected)
            {
                await Connection.InvokeAsync("changeDisplayName", displayName);
            }
        }

        public event Action<UserPresenceChangeEventArgs> UserPresenceChanged;

        public event Action<ActiveUserListChangedEventArgs> ActiveUserListChanged;

        public event Action<PublicMessageReceivedEventArgs> PublicMessageReceived;
    }

    public record UserPresenceChangeEventArgs(string Username, bool IsSignedIn = true);

    public record ActiveUserListChangedEventArgs(IEnumerable<ChatUser> ActiveUsers);

    public record PublicMessageReceivedEventArgs(PublicMessage PublicMessage);
}