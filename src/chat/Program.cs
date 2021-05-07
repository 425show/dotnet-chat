using System;
using chat.Commands;

namespace chat
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Console.WriteLine("Enter Command:");
                
                var input  = Console.ReadLine();
                HandleInput(input);
            }
        }


        static void HandleInput(string input)
        {
            if(input.StartsWith("connect", StringComparison.OrdinalIgnoreCase))
            {
                ConnectCommand.HandleCommand(input);
            }
            if(input.StartsWith("receive", StringComparison.OrdinalIgnoreCase))
            {
                ReceiveCommand.HandleCommand(input);
            }
            if(input.StartsWith("say", StringComparison.OrdinalIgnoreCase))
            {
                SayCommand.HandleCommand(input);
            }
        }
    }
}
