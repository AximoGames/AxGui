// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace AxGui.Test
{

    public class TestBase
    {
        public Element? RootElement;

        protected void Layout(Element el, Box? viewport = null)
        {
            RootElement = el;
            var layouter = new LayoutProcessor();
            layouter.ViewPort = viewport ?? new Box(0, 0, 640, 360);
            layouter.Process(el);
        }

        protected static void AssertEqualsNormalizeLineBreaks(string expected, string actual)
        {
            Assert.Equal(NormalizeLineBreaks(expected), NormalizeLineBreaks(actual));
        }

        protected static string NormalizeLineBreaks(string text)
        {
            return text.Replace("\r", "");
        }

    }
}
