using System;
using System.Collections.Generic;
using System.Linq;

[assembly:System.Runtime.CompilerServices.InternalsVisibleTo(assemblyName: "test")]
namespace Microsoft.PowerShell
{
    public class Pager
    {
        private static readonly char vt100Escape = (char) 0x1b;
        private static readonly string startAltBuffer = $"{vt100Escape}[?1049h";
        private static readonly string endAltBuffer = $"{vt100Escape}[?1049l";
        private static readonly string reverseColorStart = $"{vt100Escape}[7m";
        private static readonly string reverseColorEnd = $"{vt100Escape}[0m";

        private static readonly string pagerMessage = $"{reverseColorStart}Up:{reverseColorEnd}↑ {reverseColorStart}Down:{reverseColorEnd}↓ {reverseColorStart}Quit:{reverseColorEnd}Q :";

        private static IConsole defaultConsole;

        public Pager()
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

                    int offset = GetMultilineOffset(contentAsArray, startLine, bufferHeight);

                    var selectedContent = contentAsArray.Skip(startLine).Take(bufferHeight - offset - 1);

                    // add padding so the pager message is always the last line.
                    int paddingLines = bufferHeight - (selectedContent.Count() + offset + 1);

                    List<string> pad = new List<string>();

                    for (int i = 0; i <= paddingLines; i++)
                    {
                        pad.Add(string.Empty);
                    }

                    string[] toDisplay = selectedContent.Concat(pad).ToArray();

                    string displayContent = String.Join(Environment.NewLine, toDisplay);

                    defaultConsole.Write(displayContent);
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

        private int GetMultilineOffset(string[] contentAsArray, int startLine, int bufferHeight)
        {
            int contentTotalLines = contentAsArray.Count();
            int endLine = Math.Min(startLine + bufferHeight, contentTotalLines - 1);
            int bufferWidth = defaultConsole.BufferWidth;

            int offset = 0;

            for(int i = startLine; i <= endLine; i++)
            {
                int lineLength = contentAsArray[i].Length;
                if (lineLength > bufferWidth)
                {
                    double val = Math.Ceiling((double) (lineLength / bufferWidth));
                    offset += Convert.ToInt32(val);
                }
            }

            return offset;
        }
    }
}
