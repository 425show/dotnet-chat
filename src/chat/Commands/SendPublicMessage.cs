using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace chat.Commands
{
    // if all other commands fail, we'll assume this is a public message
    public class SendPublicMessage : ICommandHandler
    {
        private readonly ILogger<SendPublicMessage> logger;

        public SendPublicMessage(ILogger<SendPublicMessage> logger)
        {
            this.logger = logger;
        }

        public Task HandleInput(string input)
        {
            logger.LogInformation(input);
            return Task.CompletedTask;
        }
    }
}