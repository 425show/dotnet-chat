using System;
using chat.Commands;

namespace chat
{
    public class CommandHandler
    {
        public void HandleInput(string input)
        {
            // todo: implement an attribute method of handling custom commands
            // todo: handle any command before the last line of this method
            // todo: the last line of this method is essentially "the user said something publicly"
            SayCommand.HandleCommand(input);
        }
    }
}