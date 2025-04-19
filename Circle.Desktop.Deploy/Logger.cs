using System.Diagnostics;

namespace Circle.Desktop.Deploy
{
    public class Logger
    {
        private static readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"FATAL ERROR: {message}");
            Console.ResetColor();

            Program.PauseIfInteractive();
            Environment.Exit(-1);
        }

        public static void Write(string message, ConsoleColor col = ConsoleColor.Gray)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(stopwatch.ElapsedMilliseconds.ToString().PadRight(8));

            Console.ForegroundColor = col;
            Console.WriteLine(message);
        }
    }
}
