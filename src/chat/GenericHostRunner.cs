using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace chat
{
    public class GenericHostRunner
    {
        internal static IHost Host;
        internal static void Run()
        {
            IConfigurationSection optionsSection = null;

            GenericHostRunner.Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    optionsSection = configurationBuilder.Build().GetSection(nameof(DotNetChatOptions));
                })
                .ConfigureServices(services =>
                {
                    services.Configure<DotNetChatOptions>(optionsSection);
                    services.AddSingleton<ChatHubClient>();
                    services.AddSingleton<CommandHandler>();
                    services.AddSingleton<AccessTokenFactory>();
                })
                .Build();
        }
    }
}