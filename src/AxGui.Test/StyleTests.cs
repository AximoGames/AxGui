﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        public void Combine()
        {
            var parent = new ElementStyle
            {
                Anchors = new BoxModelRect
                {
                    Left = 10,
                    Top = 20,
                },
            };

            var child = new ElementStyle
            {
                Anchors = new BoxModelRect
                {
                    Right = 30,
                    Bottom = 40,
                },
            };

            var combined = ElementStyle.Combine(parent, child);
            Assert.Equal(new BoxModelRect(10, 20, 30, 40), combined.Anchors);
        }

    }
}
