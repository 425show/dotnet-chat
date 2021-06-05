using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;
using System.Collections.Generic;
using static Terminal.Gui.View;

namespace chat
{
    public class ChatAppUi
    {
        private StatusBar statusBar;
        private FrameView bottomPane;
        private TextField messageTextBox;
        private FrameView leftPane;
        private List<string> userList;
        private ListView userListView;
        private FrameView rightPane;
        private readonly AccessTokenFactory accessTokenFactory;
        private readonly Toplevel applicationTop;
        private ChatHubClient chatHubClient;

        public ChatAppUi(AccessTokenFactory accessTokenFactory,
            ChatHubClient chatHubClient,
            Toplevel applicationTop)
        {
            this.accessTokenFactory = accessTokenFactory;
            this.chatHubClient = chatHubClient;
            this.applicationTop = applicationTop;
        }

        public void Paint()
        {
            SetupFrames();
            SetupStatusBar();
        }

        private void SetupFrames()
        {
            userList = new List<string>();
            userListView = new ListView(userList)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(0),
                Height = Dim.Fill(0),
                AllowsMarking = false,
                CanFocus = true
            };

            leftPane = new FrameView("Users")
            {
                X = 0,
                Y = 0,
                Width = 40,
                Height = Dim.Percent(82),
                Title = "Users online"
            };
            leftPane.Add(userListView);

            rightPane = new FrameView("Chat")
            {
                X = 40,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Percent(82),
                Title = "Chat messages"
            };

            bottomPane = new FrameView("Bottom")
            {
                X = 0,
                Y = Pos.Bottom(leftPane),
                Title = "Type your message and hit <Enter> to send:",
                Width = Dim.Fill(),
                Height = Dim.Percent(18),
            };

            messageTextBox = new TextField("Hello world!")
            {
                X = 1,
                Y = 1,
                Width = Dim.Percent(98)
            };
            messageTextBox.KeyPress += OnMessageBoxKeyPress;
            bottomPane.Add(messageTextBox);

            applicationTop.Add(leftPane);
            applicationTop.Add(rightPane);
            applicationTop.Add(bottomPane);

            messageTextBox.SetFocus();
        }

        protected void OnMessageBoxKeyPress(KeyEventEventArgs args)
        {
            if (args.KeyEvent.Key == Key.Enter)
            {
                var messageToSend = messageTextBox.Text;
                messageTextBox.Text = "";
            }
        }

        private void SetupStatusBar()
        {
            statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Help", () => {}),
                new StatusItem(Key.F2, "~F2~ Authenticate", async () => await Authenticate()),
                new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Quit ())
            });

            statusBar.KeyPress += StatusBar_KeyPress;

            applicationTop.Add(statusBar);
        }

        private void StatusBar_KeyPress(View.KeyEventEventArgs e)
        {
            switch (ShortcutHelper.GetModifiersKey(e.KeyEvent))
            {
                case Key.CtrlMask | Key.T:
                    e.Handled = true;
                    break;
            }
        }

        private bool ConfirmQuit()
        {
            var n = MessageBox.Query("Quit Demo", "Are you sure you want to quit?", "Yes", "No");
            return n == 0;
        }

        private void Quit()
        {
            if (ConfirmQuit())
            {
                Program.UiThread = null; // this is a hack, open to suggestions
                applicationTop.Running = false;
            }
        }

        private async Task Authenticate()
        {
            await accessTokenFactory.Authenticate();
            if (!string.IsNullOrEmpty(accessTokenFactory.GetAccessToken()))
            {
                AddConnectStatusBarCommand();
            }
        }

        private void AddConnectStatusBarCommand()
        {
            var currentItems = statusBar.Items.ToList();
            currentItems.RemoveAt(1);
            currentItems.Insert(1, new StatusItem(Key.F3, "~F3~ Connect", async () => await ConnectToChatHub()));
            statusBar.Items = currentItems.ToArray();
            statusBar.SetNeedsDisplay();
        }

        protected void OnActiveUserListChanged(ActiveUserListChangedEventArgs args)
        {
            userList.Clear();
            args.ActiveUsers.OrderBy(x => x).ToList().ForEach(_ => userList.Add(_));
            userListView.SetNeedsDisplay();
        }

        protected void OnUserPresenceChanged(UserPresenceChangeEventArgs args)
        {
            if (args.IsSignedIn && !userList.Contains(args.Username))
            {
                // these were replaced with ActiveUserListChanged but there's gotta be value here
            }

            if (!args.IsSignedIn && userList.Contains(args.Username))
            {
                // these were replaced with ActiveUserListChanged but there's gotta be value here
            }
        }

        private async Task ConnectToChatHub()
        {
            chatHubClient.ActiveUserListChanged += OnActiveUserListChanged;

            var token = accessTokenFactory.GetAccessToken();

            await chatHubClient.Connect(token);

            if (chatHubClient.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
            {
                var currentItems = statusBar.Items.ToList();
                currentItems.RemoveAt(1);
                currentItems.Insert(1, new StatusItem(Key.F3, "~F3~ Disconnect", async () =>
                {
                    await chatHubClient.Disconnect();
                    AddConnectStatusBarCommand();
                }));
                statusBar.Items = currentItems.ToArray();
                statusBar.SetNeedsDisplay();

                await chatHubClient.SignIn();
            }
        }
    }
}