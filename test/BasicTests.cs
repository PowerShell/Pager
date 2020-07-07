using System;
using System.IO;
using Microsoft.PowerShell;
using Xunit;

namespace PSPagerTests
{
    public class BasicTests
    {
        private static readonly char vt100Escape = (char)0x1b;
        private static readonly string startAltBuffer = $"{vt100Escape}[?1049h]";
        private static readonly string endAltBuffer = $"{vt100Escape}[?1049l";
        private static readonly string reverseColorStart = $"{vt100Escape}[7m";
        private static readonly string reverseColorEnd = $"{vt100Escape}[0m";
        private static readonly string pagerMessage = $"{reverseColorStart}Up:{reverseColorEnd}↑ {reverseColorStart}Down:{reverseColorEnd}↓ {reverseColorStart}Quit:{reverseColorEnd}Q :";


        [Fact]
        public void Pressing_Q_CanExit()
        {
            TestConsole console = new TestConsole();
            console.BufferHeight = 50;
            console.WindowHeight = 50;
            console.BufferWidth = 120;
            console.WindowWidth = 120;
            console.PressedKey = new ConsoleKeyInfo('Q', ConsoleKey.Q, shift: false, alt: false, control: false);
            Pager p = new Pager(console);

            p.Write("HelloWorld", scrollToRegexPattern: null, useAlternateScreenBuffer: false);
            string expected = $"HelloWorld{Environment.NewLine}{pagerMessage}{endAltBuffer}";
            Assert.Equal(expected, console.ToString());
        }
    }
}