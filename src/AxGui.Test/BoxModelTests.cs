﻿using System;
using Xunit;

namespace AxGui.Test
{
    public class BoxModelTests
    {

        private Element CreateElement()
        {
            var el = new Element();
            el.Style.Position = StylePosition.Absolute;
            el.Style.Height = 100;
            return el;
        }

        private void Layout(Element el)
        {
            var layouter = new LayoutProcessor();
            layouter.ViewPort = new Box(0, 0, 640, 360);
            layouter.Process(el);
        }

        [Fact]
        public void MarginCheck()
        {
            var el = CreateElement();
            el.Style.Anchors = new BoxModelRect { Top = 5 };
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
            el.Style.Anchors = new BoxModelRect { Top = 5 };

            Layout(el);
            Assert.Equal(100, el.ClientRect.Height);
            Assert.Equal(el.MarginRect.Top, el.ClientRect.Top);
            Assert.Equal(el.MarginRect.Bottom, el.ClientRect.Bottom);
        }

        [Fact]
        public void MarginClientSet()
        {
            var el = CreateElement();
            el.Style.Anchors = new BoxModelRect { Top = 5 };
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
            el.Style.Anchors = new BoxModelRect { Top = 5 };
            el.Style.Margin = 5;

            Layout(el);
            Assert.Equal(el.BorderRect, el.ClientRect);
            Assert.Equal(el.PaddingRect, el.ClientRect);
        }

    }
}