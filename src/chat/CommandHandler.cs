using System;
using chat.Commands;

namespace chat
{
    public class CommandHandler
    {
        private readonly AccessTokenFactory _accessTokenFactory;

        public CommandHandler(AccessTokenFactory accessTokenFactory)
        {
            _accessTokenFactory = accessTokenFactory;
        }

        public void HandleInput(string input)
        {
            if (input.StartsWith("connect", StringComparison.OrdinalIgnoreCase))
            {
                ConnectCommand.HandleCommand(input, _accessTokenFactory.GetAccessToken()).Wait();
            }
            if (input.StartsWith("receive", StringComparison.OrdinalIgnoreCase))
            {
                ReceiveCommand.HandleCommand(input);
            }
            if (input.StartsWith("say", StringComparison.OrdinalIgnoreCase))
            {
                SayCommand.HandleCommand(input);
            }
        }
    }
}