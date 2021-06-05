﻿using System;
using System.Threading.Tasks;
using Terminal.Gui;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace chat
{
    public class Program
    {
        public static Action UiThread = StartUi;
        public static IHost Host;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.Default;

            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Application.UseSystemConsole = true;
            Application.Init();
            Application.HeightAsBuffer = true;

            IConfigurationSection optionsSection = null;

            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    optionsSection = configurationBuilder.Build().GetSection(nameof(DotNetChatOptions));
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<Toplevel>(Application.Top);
                    services.Configure<DotNetChatOptions>(optionsSection);
                    services.AddSingleton<ChatHubClient>();
                    services.AddSingleton<CommandHandler>();
                    services.AddSingleton<AccessTokenFactory>();
                    services.AddSingleton<ChatAppUi>();
                })
                .Build();

            // stasrt the app
            while (UiThread != null) UiThread.Invoke();

            Application.Shutdown();
        }

        internal static void StartUi()
        {
            var ui = Host.Services.GetService<ChatAppUi>();
            ui.Paint();

            Application.Run(Application.Top);
        }
    }
}