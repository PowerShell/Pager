using System;
using System.IO;

namespace Microsoft.PowerShell
{
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