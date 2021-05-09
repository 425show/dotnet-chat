using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using chat;

IConfigurationSection optionsSection = null;

var host = Host.CreateDefaultBuilder(args)
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

await host.Services.GetService<AccessTokenFactory>().Authenticate();

while (true)
{
    Console.WriteLine("Enter Command:");
    var input = Console.ReadLine();
    host.Services.GetService<CommandHandler>().HandleInput(input);
}