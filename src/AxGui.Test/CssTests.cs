﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Xunit;

namespace AxGui.Test
{
    public class CssTests : TestBase
    {

        [Fact]
        public void ParseFontSize()
        {
            var parser = new ExCSS.StylesheetParser();
            var sheet = parser.Parse("a {font-size:10px; }");
            var style = ((ExCSS.StyleRule)sheet!.StyleRules!.First()).Style;
            Assert.Equal("10px", style.FontSize);
        }

        [Fact(Skip = "Waiting for fix")]
        public void ParseJustifyContent()
        {
            var parser = new ExCSS.StylesheetParser();
            var sheet = parser.Parse("a {font-size:10px; display: flex; justify-content: center; }");
            var style = ((ExCSS.StyleRule)sheet!.StyleRules!.First()).Style;
            Assert.Equal("10px", style.FontSize);
            Assert.Equal("center", style.JustifyContent);
        }

    }
}