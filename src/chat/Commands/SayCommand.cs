using System;
namespace chat.Commands
{
    public class SayCommand
    {
        public static void HandleCommand(string command)
        {
            Console.WriteLine(command);
        }
    }
}