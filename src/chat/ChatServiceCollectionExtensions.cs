using System;
using System.Linq;
using System.Reflection;
using chat.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace chat
{
    public static class ChatServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandHandling(this IServiceCollection services)
        {
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<SendPublicMessage>();
            services.AddSingleton<CommandHandlerLocator>();

            Assembly
                .GetExecutingAssembly()
                .GetTypes()
                    .Where(_ => _.GetCustomAttribute<CommandAttribute>() != null)
                    .ToList()
                        .ForEach(_ => services.AddSingleton(_));

            return services;
        }
    }
}