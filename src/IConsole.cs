using System;

namespace Microsoft.PowerShell
{
    interface IConsole
    {
        int BufferHeight { get; }
        int BufferWidth { get; }
        int WindowHeight { get; }
        int WindowWidth { get; }

        void Clear();

        ConsoleKeyInfo ReadKey(bool intercept);

        void Write(string value);

        void WriteLine(string value);
    }

    public class SystemConsole : IConsole
    {
        public int BufferHeight
        {
            get => System.Console.BufferHeight;
        }

        public int BufferWidth
        {
            get => System.Console.BufferWidth;
        }

        public int WindowHeight
        {
            get => System.Console.WindowHeight;
        }

        public int WindowWidth
        {
            get => System.Console.WindowWidth;
        }

        public void Clear()
        {
            System.Console.Clear();
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return System.Console.ReadKey(intercept);
        }

        public void Write(string value)
        {
            System.Console.Write(value);
        }

        public void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }
    }
}