using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace chat.Commands
{
    // if all other commands fail, we'll assume this is a public message
    public class SendPublicMessage : ICommandHandler
    {
        private readonly ILogger<SendPublicMessage> logger;
        private readonly ChatHubClient chatHubClient;

        public SendPublicMessage(ILogger<SendPublicMessage> logger,
            ChatHubClient chatHubClient)
        {
            this.chatHubClient = chatHubClient;
            this.logger = logger;
        }

        public async Task HandleInput(string input)
        {
            logger.LogInformation($"Sending public message {input}");
            await this.chatHubClient.SendPublicMessage(input);
        }
    }
}