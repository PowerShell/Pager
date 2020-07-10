// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.PowerShell
{
    /// <summary>
    /// Interface for console APIs used by Pager.
    /// </summary>
    interface IConsole
    {
        /// <summary>
        /// Height of the buffer.
        /// </summary>
        int BufferHeight { get; }

        /// <summary>
        /// Width of the buffer.
        /// </summary>
        int BufferWidth { get; }

        /// <summary>
        /// Height of the window.
        /// </summary>
        int WindowHeight { get; }

        /// <summary>
        /// Width of the window.
        /// </summary>
        int WindowWidth { get; }

        /// <summary>
        /// Clear the contents of the console.
        /// </summary>
        void Clear();

        /// <summary>
        /// Read key press from the user.
        /// </summary>
        /// <param name="intercept">Intercept the key press and not show on console.</param>
        /// <returns>ConsoleKeyInfo object for the key press.</returns>
        ConsoleKeyInfo ReadKey(bool intercept);

        /// <summary>
        /// Write the value to console.
        /// </summary>
        /// <param name="value">Value to be written to console.</param>
        void Write(string value);

        /// <summary>
        /// Write the value to console and append end of line.
        /// </summary>
        /// <param name="value">Value to be written to console.</param>
        void WriteLine(string value);
    }

    /// <summary>
    /// Implementation on IConsole which redirects APIs to System.Console APIs.
    /// </summary>
    internal class SystemConsole : IConsole
    {
        /// <summary>
        /// Height of the buffer.
        /// </summary>
        public int BufferHeight
        {
            get => System.Console.BufferHeight;
        }

        /// <summary>
        /// Width of the buffer.
        /// </summary>
        public int BufferWidth
        {
            get => System.Console.BufferWidth;
        }

        /// <summary>
        /// Height of the window.
        /// </summary>
        public int WindowHeight
        {
            get => System.Console.WindowHeight;
        }

        /// <summary>
        /// Width of the window.
        /// </summary>
        public int WindowWidth
        {
            get => System.Console.WindowWidth;
        }

        /// <summary>
        /// Clear the contents of the console.
        /// </summary>
        public void Clear()
        {
            System.Console.Clear();
        }

        /// <summary>
        /// Read key press from the user.
        /// </summary>
        /// <param name="intercept">Intercept the key press and not show on console.</param>
        /// <returns>ConsoleKeyInfo object for the key press.</returns>
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return System.Console.ReadKey(intercept);
        }

        /// <summary>
        /// Write the value to console.
        /// </summary>
        /// <param name="value">Value to be written to console.</param>
        public void Write(string value)
        {
            System.Console.Write(value);
        }

        /// <summary>
        /// Write the value to console and append end of line.
        /// </summary>
        /// <param name="value">Value to be written to console.</param>
        public void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }
    }
}
