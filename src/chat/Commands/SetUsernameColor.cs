using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace chat.Commands
{
    [Command("--set-username-color")]
    public class SetUsernameColor : ICommandHandler
    {
        private readonly ILogger<SetUsernameColor> logger;

        public SetUsernameColor(ILogger<SetUsernameColor> logger)
        {
            this.logger = logger;
        }

        public Task HandleInput(string input)
        {
            logger.LogInformation($"SetUsernameColor received: {input}");
            return Task.CompletedTask;
        }
    }
}