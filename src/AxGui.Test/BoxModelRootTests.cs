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
    }

    public class BoxModelRootTests : TestBase
    {

        private Element CreateElement()
        {
            var el = new Element();
            el.Style.Display = StyleDisplay.Block;
            el.Style.Position = StylePosition.Absolute;
            el.Style.Width = 200;
            el.Style.Height = 100;
            return el;
        }

        [Fact]
        public void MarginCheck()
        {
            var el = CreateElement();
            el.Style.Anchors = new StyleRect { Top = 5 };
            el.Style.Margin = 5;
            //el.Style.BorderWidth = 5;
            //el.Style.Anchors.Bottom = 20;

            Layout(el);
            Assert.Equal(5, el.MarginRect.Top);
            Assert.Equal(115, el.MarginRect.Bottom);
            //Assert.Equal(el.MarginRect, el.ClientRect);
        }

        [Fact]
        public void MarginClientTopBottomEquals()
        {
            var el = CreateElement();
            el.Style.Anchors = new StyleRect { Top = 5 };

            Layout(el);
            Assert.Equal(100, el.ClientRect.Height);
            Assert.Equal(el.MarginRect.Top, el.ClientRect.Top);
            Assert.Equal(el.MarginRect.Bottom, el.ClientRect.Bottom);
        }

        [Fact]
        public void MarginClientSet()
        {
            var el = CreateElement();
            el.Style.Anchors = new StyleRect { Top = 5 };
            el.Style.Margin = 5;

            Layout(el);
            Assert.Equal(100, el.ClientRect.Height);
            Assert.Equal(10, el.ClientRect.Top);
            Assert.Equal(110, el.ClientRect.Bottom);
        }

        [Fact]
        public void BorderPaddingClientEquals()
        {
            var el = CreateElement();
            el.Style.Anchors = new StyleRect { Top = 5 };
            el.Style.Margin = 5;

            Layout(el);
            Assert.Equal(el.BorderRect, el.ClientRect);
            Assert.Equal(el.PaddingRect, el.ClientRect);
        }

        [Fact]
        public void MinHeight()
        {
            var el = CreateElement();
            el.Style.Anchors = new StyleRect { Top = 5 };
            el.Style.Margin = 5;
            el.Style.BorderWidth = 5;
            el.Style.Padding = 5;
            el.Style.MinHeight = 50;

            Layout(el, viewport: new Box(0, 0, 640, 20));
            Assert.Equal(new Box(15, 20, 215, 120), el.ClientRect);
        }

        [Fact]
        public void AnchorTopBottom()
        {
            var el = CreateElement();
            el.Style.Anchors = new StyleRect { Top = 5, Bottom = 5 };
            el.Style.Margin = 5;
            el.Style.BorderWidth = 5;
            el.Style.Padding = 5;

            Layout(el);
            Assert.Equal(new Box(15, 20, 215, 340), el.ClientRect);
        }

        [Fact]
        public void PercentHeight()
        {
            var el = CreateElement();
            el.Style.Anchors = new StyleRect { Top = 5 };
            el.Style.Margin = 5;
            el.Style.BorderWidth = 5;
            el.Style.Padding = 5;
            el.Style.Height = "50%";

            Layout(el);
            Assert.Equal(new Box(15, 20, 215, 200), el.ClientRect);
        }

        [Fact]
        public void CenterY()
        {
            var el = CreateElement();
            el.Style.Margin = "auto";
            el.Style.BorderWidth = 5;
            el.Style.Padding = 5;
            el.Style.Height = 100;
            el.Style.Width = 150;

            Layout(el);
            Assert.Equal(new Box(245, 130, 395, 230), el.ClientRect);
        }

    }
}
