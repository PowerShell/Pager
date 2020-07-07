using System;
using System.Linq;
using System.Reflection;

[assembly:System.Runtime.CompilerServices.InternalsVisibleTo(assemblyName: "test")]
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

        private static IConsole defaultConsole;

        static Pager()
        {
            defaultConsole = new SystemConsole();
        }

        internal Pager(IConsole testConsole)
        {
            defaultConsole = testConsole;
        }

        public void Write(string content, string scrollToRegexPattern, bool useAlternateScreenBuffer = true)
        {
            string[] contentAsArray = content.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
            int startLine = 0;
            int bufferHeight = Math.Min(defaultConsole.WindowHeight, defaultConsole.BufferHeight);
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

            defaultConsole.Write(startAltBuffer);

            do
            {
                if (moved) {

                    defaultConsole.Clear();

                    int offset = GetMultilineOffset(contentAsArray, startLine);//, bufferHeight);
                    string displayContent = String.Join(Environment.NewLine, contentAsArray.Skip(startLine).Take(bufferHeight - offset - 1));

                    defaultConsole.WriteLine(displayContent);
                    defaultConsole.Write(pagerMessage);
                }

                ConsoleKeyInfo pressed = defaultConsole.ReadKey(intercept: true);

                if (pressed.Key == ConsoleKey.UpArrow) {
                    if (startLine > 0) {
                        startLine--;
                        moved = true;
                    }
                }
                else if (pressed.Key == ConsoleKey.DownArrow) {
                    if ((bufferHeight - 1) < contentAsArray.Count()) {
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

            defaultConsole.Write(endAltBuffer);
        }

        private int GetMultilineOffset(string[] contentAsArray, int startLine)//, int bufferHeight)
        {
            int contentTotalLines = contentAsArray.Count();

            if (contentTotalLines <= 1)
            {
                return contentTotalLines;
            }

            //int endLine = startLine + bufferHeight;
            int bufferWidth = defaultConsole.BufferWidth;

            int offset = 0;

            for(int i = startLine; i < contentTotalLines; i++)
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
