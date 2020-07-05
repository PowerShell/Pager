using System;
using System.IO;

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

    public class TestConsole : IConsole
    {
        public int BufferHeight
        { get; set; }

        public int BufferWidth
        { get; set; }

        public ConsoleKeyInfo PressedKey
        { get; set; }

        public int WindowHeight
        { get; set; }

        public int WindowWidth
        { get; set; }

        private StringWriter consoleContent = new StringWriter();

        public void Clear()
        {
            consoleContent = new StringWriter();
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return PressedKey;
        }

        public override string ToString()
        {
            return consoleContent.ToString();
        }

        public void Write(string value)
        {
            consoleContent.Write(value);
        }

        public void WriteLine(string value)
        {
            consoleContent.WriteLine(value);
        }
    }
}