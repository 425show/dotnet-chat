using System;
using System.Collections.Generic;
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

            Connection.On<string>("userDisconnected", (username) =>
            {
                UserPresenceChanged?.Invoke(new UserPresenceChangeEventArgs(username, false));
            });

            Connection.On<string>("userConnected", (username) =>
            {
                UserPresenceChanged?.Invoke(new UserPresenceChangeEventArgs(username, true));
            });

            Connection.On<List<string>>("activeUserListUpdated", (usernames) =>
            {
                ActiveUserListChanged?.Invoke(new ActiveUserListChangedEventArgs(usernames));
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

        public event Action<UserPresenceChangeEventArgs> UserPresenceChanged;

        public event Action<ActiveUserListChangedEventArgs> ActiveUserListChanged;
    }

    public class UserPresenceChangeEventArgs : EventArgs
    {
        public UserPresenceChangeEventArgs(string username, bool isSignedIn = true)
        {
            this.Username = username;
            this.IsSignedIn = isSignedIn;
        }

        public string Username { get; set; }

        public bool IsSignedIn { get; set; } = true;
    }

    public class ActiveUserListChangedEventArgs : EventArgs
    {
        public ActiveUserListChangedEventArgs(IEnumerable<string> activeUsers)
        {
            ActiveUsers = activeUsers;
        }

        public IEnumerable<string> ActiveUsers { get; set; }
    }
}