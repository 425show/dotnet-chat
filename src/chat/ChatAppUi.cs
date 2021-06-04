using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace chat
{
    public class ChatAppUi
    {
        private static StatusBar StatusBar;
        private static Toplevel ApplicationTop;
        private static FrameView BottomPane;
        private static FrameView LeftPane;
        private static List<string> UserList;
        private static ListView UserListView;
        private static FrameView RightPane;

        public static ChatHubClient ChatClient { get; private set; }

        internal static void Run()
        {
            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Application.UseSystemConsole = true;

            Application.Init();
            Application.HeightAsBuffer = true;
            ApplicationTop = Application.Top;

            StatusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Help", () => {}),
                new StatusItem(Key.F2, "~F2~ Authenticate", async () => await Authenticate()),
                new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Quit ())
            });

            StatusBar.KeyPress += StatusBar_KeyPress;

            DrawFrameViews();

            ApplicationTop.Add(StatusBar);
            Application.Run(ApplicationTop);
        }
        private static void StatusBar_KeyPress(View.KeyEventEventArgs e)
        {
            switch (ShortcutHelper.GetModifiersKey(e.KeyEvent))
            {
                case Key.CtrlMask | Key.T:
                    e.Handled = true;
                    break;
            }
        }

        private static bool ConfirmQuit()
        {
            var n = MessageBox.Query("Quit Demo", "Are you sure you want to quit?", "Yes", "No");
            return n == 0;
        }

        private static void Quit()
        {
            if (ConfirmQuit())
            {
                Program.ChatAppUiRunning = null; // this is a hack, open to suggestions
                ApplicationTop.Running = false;
            }
        }

        private static async Task Authenticate()
        {
            var factory = GenericHostRunner.Host.Services.GetService<AccessTokenFactory>();
            await factory.Authenticate();
            if (!string.IsNullOrEmpty(factory.GetAccessToken()))
            {
                AddConnectStatusBarCommand();
            }
        }

        private static void AddConnectStatusBarCommand()
        {
            var currentItems = StatusBar.Items.ToList();
            currentItems.RemoveAt(1);
            currentItems.Insert(1, new StatusItem(Key.F3, "~F3~ Connect", async () => await ConnectToChatHub()));
            StatusBar.Items = currentItems.ToArray();
            StatusBar.SetNeedsDisplay();
        }

        protected static void OnActiveUserListChanged(ActiveUserListChangedEventArgs args)
        {
            UserList.Clear();
            args.ActiveUsers.OrderBy(x => x).ToList().ForEach(_ => UserList.Add(_));
            UserListView.SetNeedsDisplay();
        }

        private static async Task ConnectToChatHub()
        {
            ChatClient = GenericHostRunner.Host.Services.GetService<ChatHubClient>();

            ChatClient.ActiveUserListChanged += OnActiveUserListChanged;

            ChatClient.UserPresenceChanged += (args) =>
            {
                if (args.IsSignedIn && !UserList.Contains(args.Username))
                {
                }

                if (!args.IsSignedIn && UserList.Contains(args.Username))
                {
                }
            };

            var token = GenericHostRunner.Host.Services.GetService<AccessTokenFactory>().GetAccessToken();

            await ChatClient.Connect(token);

            if (ChatClient.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
            {
                var currentItems = StatusBar.Items.ToList();
                currentItems.RemoveAt(1);
                currentItems.Insert(1, new StatusItem(Key.F3, "~F3~ Disconnect", async () =>
                {
                    await ChatClient.Disconnect();
                    AddConnectStatusBarCommand();
                }));
                StatusBar.Items = currentItems.ToArray();
                StatusBar.SetNeedsDisplay();

                await ChatClient.SignIn();
            }
        }

        private static void DrawFrameViews()
        {
            UserList = new List<string>();
            UserListView = new ListView(UserList)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(0),
                Height = Dim.Fill(0),
                AllowsMarking = false,
                CanFocus = true
            };

            LeftPane = new FrameView("Users")
            {
                X = 0,
                Y = 0,
                Width = 40,
                Height = Dim.Percent(82),
                Title = "Users online"
            };

            RightPane = new FrameView("Chat")
            {
                X = 40,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Percent(82),
                Title = "Chat messages"
            };

            BottomPane = new FrameView("Bottom")
            {
                X = 0,
                Y = Pos.Bottom(LeftPane),
                Title = "Type your message and hit <Enter> to send:",
                Width = Dim.Fill(),
                Height = Dim.Percent(18),
            };

            var textField = new TextField("Hello world!")
            {
                X = 1,
                Y = 1,
                Width = Dim.Percent(98),
            };

            textField.KeyPress += (args) =>
            {
                if (args.KeyEvent.Key == Key.Enter)
                {
                    var messageToSend = textField.Text;
                    textField.Text = "";
                }
            };

            BottomPane.Add(textField);

            LeftPane.Add(UserListView);
            ApplicationTop.Add(LeftPane);
            ApplicationTop.Add(RightPane);
            ApplicationTop.Add(BottomPane);

            textField.SetFocus();
        }
    }
}