using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace chat.Commands
{
    [Command("--set-display-name")]
    public class SetDisplayName : ICommandHandler
    {
        private readonly ILogger<SetDisplayName> logger;
        private readonly ChatHubClient chatHubClient;

        public SetDisplayName(ILogger<SetDisplayName> logger,
            ChatHubClient chatHubClient)
        {
            this.logger = logger;
            this.chatHubClient = chatHubClient;
        }

        public async Task HandleInput(string input)
        {
            logger.LogInformation($"SetDisplayName received: {input}");
            await chatHubClient.ChangeDisplayName(input);
        }
    }
}