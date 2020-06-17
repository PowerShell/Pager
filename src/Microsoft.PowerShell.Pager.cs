using System;
using System.Linq;

namespace Microsoft.PowerShell
{
    public class Pager
    {
        private static readonly char vt100Escape = (char) 0x1b;
        private static readonly string startAltBuffer = $"{vt100Escape}[?1049h]";
        private static readonly string endAltBuffer = $"{vt100Escape}[?1049l";
        private static readonly string reverseColorStart = $"{vt100Escape}[7m";
        private static readonly string reverseColorEnd = $"{vt100Escape}[0m";

        private static readonly string pagerMessage = $"{reverseColorStart}Up:{reverseColorEnd}↑ {reverseColorStart}Down:{reverseColorEnd}↓ {reverseColorStart}Quit:{reverseColorEnd}Q :";

        public static void Write(string content, string scrollToRegexPattern, bool useAlternateScreenBuffer = true)
        {
            string[] contentAsArray = content.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);

            int startLine = 0;
            int bufferHeight = Math.Min(Console.WindowHeight,Console.BufferHeight);
            bool moved = true;

            if (!String.IsNullOrEmpty(scrollToRegexPattern))
            {
                for(int i = 0; i < contentAsArray.Length; i++)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(contentAsArray[i], scrollToRegexPattern))
                    {
                        startLine = i;
                    }
                }
            }

            Console.Write(startAltBuffer);

            do
            {
                if (moved) {
                    Console.Clear();
                    int offset = GetMultilineOffset(contentAsArray, startLine);
                    string displayContent = String.Join(Environment.NewLine, contentAsArray.Skip(startLine).Take(bufferHeight - offset));
                    Console.WriteLine(displayContent);
                    Console.Write(pagerMessage);
                }

                var pressed = Console.ReadKey(intercept: true);

                if (pressed.Key == ConsoleKey.UpArrow) {
                    if (startLine > 0) {
                        startLine--;
                        moved = true;
                    }
                }
                else if (pressed.Key == ConsoleKey.DownArrow) {
                    if ((startLine + bufferHeight) < contentAsArray.Count()) {
                        startLine++;
                        moved = true;
                    }
                }
                else if (pressed.Key == ConsoleKey.Q) {
                    break;
                }
                else {
                    moved = false;
                }

            } while (true);

            Console.Write(endAltBuffer);
        }

        private static int GetMultilineOffset(string[] contentAsArray, int startLine)
        {
            int bufferHeight = Math.Min(Console.WindowHeight,Console.BufferHeight);
            int contentTotalLines = contentAsArray.Count();
            int endLine = startLine + bufferHeight;
            int bufferWidth = Console.BufferWidth;

            int offset = 0;

            for(int i = startLine; i < endLine; i++)
            {
                int lineLength = contentAsArray[i].Length;
                if (lineLength > bufferWidth)
                {
                    offset += lineLength / bufferWidth;
                }
            }

            return offset;
        }
    }
}
