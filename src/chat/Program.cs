using System;
using System.Threading.Tasks;
using Terminal.Gui;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace chat
{
    public class Program
    {
        public static Action ChatAppUiRunning = ChatAppUi.Run;

        static void Main()
        {
            GenericHostRunner.Run();
            Console.OutputEncoding = System.Text.Encoding.Default;

            while (ChatAppUiRunning != null)
            {
                ChatAppUiRunning.Invoke();
            }

            Application.Shutdown();
        }
    }
}