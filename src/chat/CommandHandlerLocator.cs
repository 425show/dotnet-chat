using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using chat.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace chat
{
    public class CommandHandlerLocator
    {
        private readonly IServiceProvider serviceProvider;

        public CommandHandlerLocator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            Assembly
                .GetExecutingAssembly()
                .GetTypes()
                    .Where(_ => _.GetCustomAttribute<CommandAttribute>() != null)
                    .ToList()
                        .ForEach(_ =>
                        {
                            Register(_.GetCustomAttribute<CommandAttribute>().Name, _);
                        });
        }

        public Dictionary<string, Type> CommandHandlers { get; set; } = new Dictionary<string, Type>();

        private void Register(string name, Type t)
        {
            CommandHandlers.Add(name, t);
        }

        public ICommandHandler GetCommandHandler(string command = "sendPublicMessage")
        {
            if (!CommandHandlers.ContainsKey(command)) return serviceProvider.GetService<SendPublicMessage>();
            return (ICommandHandler)serviceProvider.GetService(CommandHandlers[command]);
        }
    }
}