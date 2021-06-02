using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Terminal.Gui;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace chat
{
    public class Program
    {
        internal static StatusBar StatusBar;
        public static Action running = MainApp;

        static void Main()
        {
            GenericHostRunner.Run();
            Console.OutputEncoding = System.Text.Encoding.Default;

            while (running != null)
            {
                running.Invoke();
            }
            Application.Shutdown();
        }

        static void MainApp()
        {
            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Application.UseSystemConsole = true;

            Application.Init();
            Application.HeightAsBuffer = true;

            var top = Application.Top;
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
                new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => { if (Quit ()) { running = null; top.Running = false; } })
            });

            top.Add(win, StatusBar);
            Application.Run(top);
        }

        static bool Quit()
        {
            var n = MessageBox.Query("Quit Demo", "Are you sure you want to quit?", "Yes", "No");
            return n == 0;
        }

        static async Task Authenticate()
        {
            var factory = GenericHostRunner.Host.Services.GetService<AccessTokenFactory>();
            await factory.Authenticate();
            if (!string.IsNullOrEmpty(factory.GetAccessToken()))
            {
                AddConnectStatusBarCommand();
            }
        }

        static void AddConnectStatusBarCommand()
        {
            var currentItems = StatusBar.Items.ToList();
            currentItems.RemoveAt(1);
            currentItems.Insert(1, new StatusItem(Key.F3, "~F3~ Connect", async () => await ConnectToChatHub()));
            StatusBar.Items = currentItems.ToArray();
            StatusBar.SetNeedsDisplay();
        }

        static async Task ConnectToChatHub()
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