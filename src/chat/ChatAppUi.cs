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

        internal static void Run()
        {
            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Application.UseSystemConsole = true;

            Application.Init();
            Application.HeightAsBuffer = true;
            ApplicationTop = Application.Top;

            int margin = 3;

            var win = new Window("welcome to dotnet-chat")
            {
                X = 1,
                Y = 1,

                Width = Dim.Fill() - margin,
                Height = Dim.Fill() - margin
            };

            win.KeyPress += Win_KeyPress;

            StatusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Help", () => {}),
                new StatusItem(Key.F2, "~F2~ Authenticate", async () => await Authenticate()),
                new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Quit ())
            });

            ApplicationTop.Add(win, StatusBar);
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