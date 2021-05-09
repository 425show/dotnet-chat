using System;
using System.Linq;
using System.Threading.Tasks;
using chat.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace chat
{
    class Program
    {
        static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args);
        static async Task Main(string[] args)
        {
            DotNetChatOptions options = new();
            IConfigurationSection optionsSection = null;

            var host = CreateHostBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    optionsSection = configurationBuilder.Build().GetSection(nameof(DotNetChatOptions));
                    options = optionsSection.Get<DotNetChatOptions>();
                })
                .ConfigureServices(services => 
                {
                    services.AddSingleton<CommandHandler>();
                    services.AddSingleton<AccessTokenFactory>();
                    services.Configure<DotNetChatOptions>(optionsSection);
                })
                .Build();

            await host.Services.GetService<AccessTokenFactory>().Authenticate();

            while (true)
            {
                Console.WriteLine("Enter Command:");
                var input = Console.ReadLine();
                host.Services.GetService<CommandHandler>().HandleInput(input);
            }
        }
    }
}
