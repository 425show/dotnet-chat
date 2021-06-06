using System;
using System.Threading.Tasks;
using chat.Commands;
using Microsoft.Extensions.Logging;

namespace chat
{
    public class CommandHandler
    {
        private readonly ILogger<CommandHandler> logger;
        private readonly CommandHandlerLocator commandHandlerLocator;

        public CommandHandler(ILogger<CommandHandler> logger,
            CommandHandlerLocator commandHandlerLocator)
        {
            this.logger = logger;
            this.commandHandlerLocator = commandHandlerLocator;
        }

        public Task HandleInput(string input)
        {
            input = $"{input} ";

            if (input.StartsWith("--"))
            {
                var command = input.Substring(0, input.IndexOf(' '));
                var commandArgument = input.Substring(command.Length).Trim();
                commandHandlerLocator.GetCommandHandler(command).HandleInput(commandArgument);
            }
            else
            {
                commandHandlerLocator.GetCommandHandler().HandleInput(input);
            }

            return Task.CompletedTask;
        }
    }
}