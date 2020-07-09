using System;
using System.Collections.Generic;
using Microsoft.PowerShell;
using Xunit;

namespace PSPagerTests
{
    public class BasicTests
    {
        private static readonly char vt100Escape = (char)0x1b;
        private static readonly string startAltBuffer = $"{vt100Escape}[?1049h";
        private static readonly string endAltBuffer = $"{vt100Escape}[?1049l";
        private static readonly string reverseColorStart = $"{vt100Escape}[7m";
        private static readonly string reverseColorEnd = $"{vt100Escape}[0m";
        private static readonly string pagerMessage = $"{reverseColorStart}Up:{reverseColorEnd}↑ {reverseColorStart}Down:{reverseColorEnd}↓ {reverseColorStart}Quit:{reverseColorEnd}Q :";

        private List<string> GetPadding(int count)
        {
            List<string> pad = new List<string>();

            for (int i = 0; i <= count; i++)
            {
                pad.Add(string.Empty);
            }

            return pad;
        }

        [Fact]
        public void Pressing_Q_CanExit()
        {
            TestConsole console = new TestConsole();
            console.BufferHeight = 10;
            console.WindowHeight = 10;
            console.BufferWidth = 120;
            console.WindowWidth = 120;

            List<ConsoleKeyInfo> pressedKeys = new List<ConsoleKeyInfo>
            {
                new ConsoleKeyInfo('Q', ConsoleKey.Q, shift: false, alt: false, control: false)
            };

            console.PressedKey = pressedKeys;

            Pager p = new Pager(console);
            p.Write("HelloWorld");
            var pad = GetPadding(8);
            string expected = $"HelloWorld{Environment.NewLine}{string.Join(Environment.NewLine, pad)}{pagerMessage}{endAltBuffer}";

            Assert.Equal(expected, console.ToString());
        }

        [Fact]
        public void Pressing_Down_Scrolls_One_Line()
        {
            TestConsole console = new TestConsole();
            console.BufferHeight = 3;
            console.WindowHeight = 3;
            console.BufferWidth = 120;
            console.WindowWidth = 120;

            List<ConsoleKeyInfo> pressedKeys = new List<ConsoleKeyInfo>
            {
                new ConsoleKeyInfo(char.MaxValue, ConsoleKey.DownArrow, shift: false, alt: false, control: false),
                new ConsoleKeyInfo('Q', ConsoleKey.Q, shift: false, alt: false, control: false)
            };

            console.PressedKey = pressedKeys;

            Pager p = new Pager(console);
            string testString = "1" + Environment.NewLine + "2" + Environment.NewLine + "3";
            p.Write(testString);
            string expected = $"2{Environment.NewLine}3{Environment.NewLine}{pagerMessage}{endAltBuffer}";
            Assert.Equal(expected, console.ToString());
        }

        [Fact]
        public void Large_BufferHeight_Rendering_Small_Content_No_Scroll()
        {
            TestConsole console = new TestConsole();
            console.BufferHeight = 120;
            console.WindowHeight = 120;
            console.BufferWidth = 120;
            console.WindowWidth = 120;

            List<ConsoleKeyInfo> pressedKeys = new List<ConsoleKeyInfo>
            {
                new ConsoleKeyInfo(char.MaxValue, ConsoleKey.DownArrow, shift: false, alt: false, control: false),
                new ConsoleKeyInfo('Q', ConsoleKey.Q, shift: false, alt: false, control: false)
            };

            console.PressedKey = pressedKeys;
            Pager p = new Pager(console);
            string testString = "1" + Environment.NewLine + "2" + Environment.NewLine + "3";

            var pad = GetPadding(116);

            p.Write(testString);
            string expected = $"1{Environment.NewLine}2{Environment.NewLine}3{Environment.NewLine}{string.Join(Environment.NewLine, pad)}{pagerMessage}{endAltBuffer}";
            Assert.Equal(expected, console.ToString());
        }

        [Fact]
        public void Scrolling_To_Line()
        {
            TestConsole console = new TestConsole();
            console.BufferHeight = 3;
            console.WindowHeight = 3;
            console.BufferWidth = 120;
            console.WindowWidth = 120;

            List<ConsoleKeyInfo> pressedKeys = new List<ConsoleKeyInfo>
            {
                new ConsoleKeyInfo('Q', ConsoleKey.Q, shift: false, alt: false, control: false)
            };

            console.PressedKey = pressedKeys;

            Pager p = new Pager(console);
            string testString = $"1{Environment.NewLine}2{Environment.NewLine}3";
            p.Write(testString, "3");
            string expected = $"3{Environment.NewLine}{Environment.NewLine}{pagerMessage}{endAltBuffer}";
            Assert.Equal(expected, console.ToString());
        }
    }
}
