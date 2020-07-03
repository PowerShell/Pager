using System;
using Microsoft.PowerShell;
using Xunit;

namespace PSPagerTests
{
    public class BasicTests
    {
        [Fact]
        public void CanExit()
        {
            try
            {
                Pager.SetTestHook("UserInputKey", new ConsoleKeyInfo('Q', ConsoleKey.Q, shift: false, alt: false, control: false));
                Pager.SetTestHook("WindowHeight", 50);
                Pager.SetTestHook("BufferHeight", 50);
                var state = Pager.GetTestHook("state") as PagerState;
                Pager.Write("HelloWorld", scrollToRegexPattern: null, useAlternateScreenBuffer: false);

                Assert.Equal("HelloWorld", state.ConsoleBufferOutput);
                Assert.True(state.Moved);
                Assert.Equal(1, state.MultilineOffset);
                Assert.Equal(0, state.StartLine);

            }
            finally
            {
                Pager.ResetPagerState();
            }
        }
    }
}