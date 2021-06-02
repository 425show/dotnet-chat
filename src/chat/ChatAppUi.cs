using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace chat
{
    public class ChatAppUi
    {
        private static StatusBar StatusBar;
        private static Toplevel ApplicationTop;
        private static FrameView BottomPane;
        private static FrameView LeftPane;
        private static FrameView RightPane;
        private static Window Window;

        internal static void Run()
        {
            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Application.UseSystemConsole = true;

            Application.Init();
            Application.HeightAsBuffer = true;
            ApplicationTop = Application.Top;

            int margin = 1;

            Window = new Window("welcome to dotnet-chat")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill() - margin,
                Height = Dim.Fill() - margin
            };

            Window.KeyPress += Win_KeyPress;

            StatusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Help", () => {}),
                new StatusItem(Key.F2, "~F2~ Authenticate", async () => await Authenticate()),
                new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Quit ())
            });

            // add all the ui components to the display
            ApplicationTop.Add(Window);

            DrawFrameViews();

            ApplicationTop.Add(StatusBar);
            Application.Run(ApplicationTop);
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

        private static async Task ConnectToChatHub()
        {
            var chatClient = GenericHostRunner.Host.Services.GetService<ChatHubClient>();
            var token = GenericHostRunner.Host.Services.GetService<AccessTokenFactory>().GetAccessToken();
            await chatClient.Connect(token);

            if (chatClient.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
            {
                var currentItems = StatusBar.Items.ToList();
                currentItems.RemoveAt(1);
                currentItems.Insert(1, new StatusItem(Key.F3, "~F3~ Disconnect", async () =>
                {
                    await chatClient.Disconnect();
                    AddConnectStatusBarCommand();
                }));
                StatusBar.Items = currentItems.ToArray();
                StatusBar.SetNeedsDisplay();
            }
        }

        private static void DrawFrameViews()
        {
            LeftPane = new FrameView("Users")
            {
                X = 0,
                Y = 0,
                Width = 40,
                Height = Dim.Fill(5),
                CanFocus = true,
                Title = "Users online"
            };

            RightPane = new FrameView("Chat")
            {
                X = 40,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(5),
                CanFocus = false,
            };

            BottomPane = new FrameView("Bottom")
            {
                X = 0,
                Y = 29,
                Title = "Type your message and hit <Enter> to send:",
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                CanFocus = true
            };

            var s = "Hello world!";

            var textField = new TextField(s)
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

            Window.Add(LeftPane);
            Window.Add(RightPane);
            Window.Add(BottomPane);
        }

        private static void Win_KeyPress(View.KeyEventEventArgs e)
        {
            switch (ShortcutHelper.GetModifiersKey(e.KeyEvent))
            {
                case Key.CtrlMask | Key.T:
                    e.Handled = true;
                    break;
            }
        }
    }
}