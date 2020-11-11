// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace AxGui.Test
{
    public class BoxModelChildTests
    {

        private Element CreateRootElement()
        {
            var el = new Element();
            el.Style.Position = StylePosition.Absolute;
            el.Style.Width = 500;
            el.Style.Height = 300;
            el.Style.Margin = 5;
            el.Style.BorderColor = 5;
            el.Style.Padding = 5;
            return el;
        }

        private void Layout(Element el, Box? viewport = null)
        {
            var layouter = new LayoutProcessor();
            layouter.ViewPort = viewport ?? new Box(0, 0, 640, 360);
            layouter.Process(el);
        }

        [Fact]
        public void Child()
        {
            var el = CreateRootElement();
            var child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Absolute;
            child.Style.BorderWidth = 5;
            child.Style.Width = 50;
            child.Style.Height = 30;
            Layout(el);
            Assert.Equal(new Box(15, 15, 65, 45), child.ClientRect);
        }

        [Fact]
        public void Child2()
        {
            var el = CreateRootElement();

            var child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 50;
            child.Style.Height = 30;

            child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 50;
            child.Style.Height = 30;

            Layout(el);
            Assert.Equal(new Box(75, 15, 125, 45), child.ClientRect);
        }

        [Fact]
        public void WrapChild()
        {
            var el = CreateRootElement();

            var child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 200;
            child.Style.Height = 30;

            child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 200;
            child.Style.Height = 30;

            child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 200;
            child.Style.Height = 30;

            Layout(el);
            Assert.Equal(new Box(15, 55, 215, 85), child.ClientRect);
        }

        [Fact]
        public void WrapChild_VAlign()
        {
            var el = CreateRootElement();

            Element child;

            var child1 = child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 200;
            child.Style.Height = 30;

            var child2 = child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 200;
            child.Style.Height = 50;

            var child3 = child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 200;
            child.Style.Height = 30;

            Layout(el);
            Assert.Equal(new Box(15, 35, 215, 65), child1.ClientRect);
            Assert.Equal(new Box(15, 75, 215, 105), child3.ClientRect);
        }

        [Fact]
        public void WrapChild_VAlign2()
        {
            var el = CreateRootElement();

            Element child;

            var child1 = child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 200;
            child.Style.Height = 50;

            var child2 = child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 200;
            child.Style.Height = 30;

            var child3 = child = new Element();
            el.AddChild(child);
            child.Style.Position = StylePosition.Relative;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.BorderWidth = 5;
            child.Style.Width = 200;
            child.Style.Height = 30;

            Layout(el);
            Assert.Equal(new Box(225, 35, 425, 65), child2.ClientRect);
            Assert.Equal(new Box(15, 75, 215, 105), child3.ClientRect);
        }

    }
}
