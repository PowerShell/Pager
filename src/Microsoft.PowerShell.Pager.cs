// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("test")]
namespace Microsoft.PowerShell
{
    /// <summary>
    ///  This class implements the Pager by writting content to the alternate screen buffer.
    /// </summary>
    public class Pager
    {
        private static readonly char vt100Escape = (char) 0x1b;
        private static readonly string startAltBuffer = $"{vt100Escape}[?1049h";
        private static readonly string endAltBuffer = $"{vt100Escape}[?1049l";
        private static readonly string reverseColorStart = $"{vt100Escape}[7m";
        private static readonly string reverseColorEnd = $"{vt100Escape}[0m";
        private static readonly string eraseScreenBuffer = $"{vt100Escape}[2J";
        private static readonly string repositionCursor = $"{vt100Escape}[1;1H";

        private static readonly string pagerMessage = 
            $"{reverseColorStart}Up:{reverseColorEnd}↑ " + 
            $"{reverseColorStart}Down:{reverseColorEnd}↓ " +
            $"{reverseColorStart}Quit:{reverseColorEnd}Q :";

        private static IConsole defaultConsole;

        /// <summary>
        /// Initialzes the default console to write the input string to.
        /// </summary>
        public Pager()
        {
            defaultConsole = new SystemConsole();
        }

        /// <summary>
        /// Override the default console with an implementation of IConsole.
        /// </summary>
        internal Pager(IConsole testConsole)
        {
            defaultConsole = testConsole;
        }

        /// <summary>
        /// Write input to alternate screen buffer and display pager prompt on the last line.
        /// </summary>
        /// <param name="content">Content to display on alternate screen buffer.</param>
        /// <param name="scrollToRegexPattern">Regular expression pattern to scroll to.null</param>
        public void Write(string content, string scrollToRegexPattern = null)
        {
            string[] contentAsArray = content.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
            int startLine = 0;
            int bufferHeight = Math.Min(defaultConsole.BufferHeight, defaultConsole.WindowHeight);
            bool moved = true;

            if (!String.IsNullOrEmpty(scrollToRegexPattern))
            {
                for(int i = 0; i < contentAsArray.Length; i++)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(contentAsArray[i], scrollToRegexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        startLine = i;
                    }
                }
            }

            defaultConsole.Write(startAltBuffer);

            while(true)
            {
                if (moved) {
                    defaultConsole.Write(eraseScreenBuffer);
                    defaultConsole.Write(repositionCursor);

                    var paddingNeeded = WriteContentToConsole(contentAsArray, startLine);

                    for (int i = 0; i < paddingNeeded; i++)
                    {
                        defaultConsole.WriteLine(string.Empty);
                    }

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
            }

            defaultConsole.Write(endAltBuffer);
        }

        /// <summary>
        /// Write input to alternate screen buffer and return number of lines that are unused.
        /// </summary>
        /// <param name="content">Content to display on alternate screen buffer.</param>
        /// <param name="scrollToRegexPattern">Index of the starting line to display.</param>
        /// <returns>Number of lines not used to write the content.</returns>
        private double WriteContentToConsole(string[] content, int startLine)
        {
            double physicalLinesAvailable = Math.Min(defaultConsole.BufferHeight, defaultConsole.WindowHeight) - 1;
            double physicalWidth = Math.Min(defaultConsole.BufferWidth, defaultConsole.WindowWidth);
            double physicalLinesNeeded = 0;

            for(int i = startLine; i < content.Length; i++)
            {
                double lineLength = content[i].Length;
                if (lineLength == 0) {
                    physicalLinesNeeded = 1;
                }
                else {
                    physicalLinesNeeded = Math.Ceiling(lineLength / physicalWidth);
                }

                if (physicalLinesAvailable - physicalLinesNeeded < 0)
                {
                    break;
                }

                defaultConsole.WriteLine(content[i]);

                physicalLinesAvailable -= physicalLinesNeeded;
            }

            return physicalLinesAvailable;
        }
    }
}
