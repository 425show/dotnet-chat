using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using chat;
using System.Threading.Tasks;
using Terminal.Gui;
using System.Diagnostics;
using System.Globalization;
using chat.Commands;
using System.Linq;

public class Program
{
    internal static class HostRunner
    {
        internal static IHost Host;
        internal static void Run()
        {
            IConfigurationSection optionsSection = null;

            HostRunner.Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    optionsSection = configurationBuilder.Build().GetSection(nameof(DotNetChatOptions));
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<CommandHandler>();
                    services.AddSingleton<AccessTokenFactory>();
                    services.Configure<DotNetChatOptions>(optionsSection);
                })
                .Build();
        }
    }

    public static Action running = MainApp;

    static bool Quit()
    {
        var n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit?", "Yes", "No");
        return n == 0;
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

        statusBar = new StatusBar(new StatusItem[] {
            new StatusItem(Key.F1, "~F1~ Help", () => {}),
            new StatusItem(Key.F2, "~F2~ Authenticate", async () => await Authenticate()),
            new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => { if (Quit ()) { running = null; top.Running = false; } })
        });

        top.Add(win, statusBar);
        Application.Run(top);
    }

    static StatusBar statusBar;

    static async Task Authenticate()
    {
        var factory = HostRunner.Host.Services.GetService<AccessTokenFactory>();
        await factory.Authenticate();
        if (!string.IsNullOrEmpty(factory.GetAccessToken()))
        {
            AddNewStatusBarItemOnTheFly();
        }
    }

    static void AddNewStatusBarItemOnTheFly()
    {
        var currentItems = statusBar.Items.ToList();
        currentItems.Insert(2, new StatusItem(Key.F3, "~F3~ Connect", () => { }));
        statusBar.Items = currentItems.ToArray();
        Application.Refresh();
    }

    static void Main()
    {
        HostRunner.Run();
        Console.OutputEncoding = System.Text.Encoding.Default;

        while (running != null)
        {
            running.Invoke();
        }
        Application.Shutdown();
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