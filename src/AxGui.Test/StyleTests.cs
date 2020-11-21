// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;
using Xunit;

namespace AxGui.Test
{
    public class StyleTests
    {

        [Fact]
        public void ParseWidth()
        {
            var style = ElementStyle.FromString("width:10px");
            Assert.Equal("10px", style.Width);
        }

        [Fact]
        public void ParseDisplay()
        {
            var style = ElementStyle.FromString("display:inline-block");
            Assert.Equal(StyleDisplay.InlineBlock, style.Display);
        }


        [Fact]
        public void ParseColor()
        {
            var style = ElementStyle.FromString("border-top-color:#010203");
            Assert.Equal(new SKColor(1, 2, 3), style.BorderColor.Top.Color);
        }

        [Fact]
        public void Combine()
        {
            var parent = new ElementStyle
            {
                Anchors = new StyleRect
                {
                    Left = 10,
                    Top = 20,
                },
            };

            var child = new ElementStyle
            {
                Anchors = new StyleRect
                {
                    Right = 30,
                    Bottom = 40,
                },
            };

            var combined = ElementStyle.Combine(parent, child);
            Assert.Equal(new StyleRect(10, 20, 30, 40), combined.Anchors);
        }

    }
}
