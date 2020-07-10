// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.PowerShell
{
    /// <summary>
    /// Implementation of IConsole to be used for unit-testing of Microsoft.PowerShell.Pager.
    /// </summary>
    public class TestConsole : IConsole
    {
        /// <summary>
        /// Height of the buffer.
        /// </summary>
        public int BufferHeight
        { get; set; }

        /// <summary>
        /// Width of the buffer.
        /// </summary>
        public int BufferWidth
        { get; set; }

        /// <summary>
        /// List of key presses to simulate.
        /// </summary>
        public List<ConsoleKeyInfo> PressedKey
        { get; set; }

        /// <summary>
        /// Height of the window.
        /// </summary>
        public int WindowHeight
        { get; set; }

        /// <summary>
        /// Width of the window.
        /// </summary>
        public int WindowWidth
        { get; set; }

        /// <summary>
        /// Simulate the console content
        /// </summary>
        private StringWriter consoleContent = new StringWriter();

        /// <summary>
        /// Index for the next key to be read for simulating user key press.
        /// </summary>
        private int currentKeyIndex = 0;

        /// <summary>
        /// Clear the contents of the simulated console.
        /// </summary>
        public void Clear()
        {
            consoleContent = new StringWriter();
        }

        /// <summary>
        /// Simulate the ReadKey console API to simualte user input.
        /// </summary>
        /// <param name="intercept">Whether the key stoke should be intercepted. Also true for tests.</param>
        /// <returns>ConsoleKeyInfo object for the simulated key press.</returns>
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            if(currentKeyIndex < PressedKey.Count)
            {
                return PressedKey[currentKeyIndex++];
            }

            throw new IndexOutOfRangeException("No more keys to read");
        }

        /// <summary>
        /// Read the contents of the simulated console
        /// </summary>
        /// <returns>Contents of the simulated console as string.</returns>
        public override string ToString()
        {
            return consoleContent.ToString();
        }

        /// <summary>
        /// Write the value to simulated console.
        /// </summary>
        /// <param name="value">Value to be written to simulated console.</param>
        public void Write(string value)
        {
            consoleContent.Write(value);
        }

        /// <summary>
        /// Write the value to simulated console and append end of line.
        /// </summary>
        /// <param name="value">Value to be written to simulated console.</param>
        public void WriteLine(string value)
        {
            consoleContent.WriteLine(value);
        }
    }
}
