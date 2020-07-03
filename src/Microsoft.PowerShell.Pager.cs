using System;
using System.Linq;
using System.Reflection;

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

        #if DEBUG
        private static PagerState state;

        private static ConsoleKeyInfo UserInputKey = new ConsoleKeyInfo('ü', ConsoleKey.NoName, shift: false, alt: false, control: false);

        private static int BufferHeight = 0;

        private static int WindowHeight = 0;
        #endif

        static Pager()
        {
            #if DEBUG
            state = new PagerState();
            #endif
        }

        public static void Write(string content, string scrollToRegexPattern, bool useAlternateScreenBuffer = true)
        {
            string[] contentAsArray = content.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);

            int startLine = 0;

            # if DEBUG
            int bufferHeight = Math.Min(WindowHeight,BufferHeight);
            #else
            int bufferHeight = Math.Min(Console.WindowHeight,Console.BufferHeight);
            #endif

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

                    #if !DEBUG
                    Console.Clear();
                    #endif

                    int offset = GetMultilineOffset(contentAsArray, startLine);
                    string displayContent = String.Join(Environment.NewLine, contentAsArray.Skip(startLine).Take(bufferHeight - offset));

                    #if DEBUG
                    state.ConsoleBufferOutput = displayContent;
                    state.Moved = moved;
                    state.MultilineOffset = offset;
                    state.StartLine = startLine;
                    #else
                    Console.WriteLine(displayContent);
                    Console.Write(pagerMessage);
                    #endif
                }

                #if DEBUG
                ConsoleKeyInfo pressed = UserInputKey;
                #else
                ConsoleKeyInfo pressed = Console.ReadKey(intercept: true);
                #endif

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
            # if DEBUG
            int bufferHeight = Math.Min(WindowHeight,BufferHeight);
            #else
            int bufferHeight = Math.Min(Console.WindowHeight,Console.BufferHeight);
            #endif

            int contentTotalLines = contentAsArray.Count();

            if (contentTotalLines <= 1)
            {
                return contentTotalLines;
            }

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

        #if DEBUG

        public static void SetTestHook(string property, object value)
        {
            var fieldInfo = typeof(Pager).GetField(property, BindingFlags.Static | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(null, value);
            }
        }

        public static object GetTestHook(string property)
        {
            var fieldInfo = typeof(Pager).GetField(property, BindingFlags.Static | BindingFlags.NonPublic);
            return fieldInfo?.GetValue(null);
        }

        public static void ResetPagerState()
        {
            state = new PagerState();
        }

        #endif
    }

    public class PagerState
    {
        public bool Moved { get; set; }
        public int StartLine { get; set; }
        public int MultilineOffset { get; set; }
        public string ConsoleBufferOutput { get; set; }
    }
}
