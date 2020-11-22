// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable CS8602 // Dereference of a possibly null reference.

using System;
using System.Linq;
using Xunit;

namespace AxGui.Test
{
    public class BoxModelSpanTests : TestBase
    {

        private Element CreateRootElement()
        {
            var el = new Element();
            el.Style.Display = StyleDisplay.Block;
            el.Style.Position = StylePosition.Absolute;
            el.Style.Height = 320;
            el.Style.Width = 220;
            el.Style.BorderWidth = 5;
            el.Style.Padding = 5;
            return el;
        }

        [Fact]
        public void Child()
        {
            var el = CreateRootElement();

            Element child;

            var child1 = child = new TextElement();
            child.Data = "child";
            child.Style.Height = 30;
            child.Style.Display = StyleDisplay.Inline;
            child.Style.Position = StylePosition.Static;
            (child as TextElement).Content = "Testduck Testduck2 Testduck3";
            (child as TextElement).TextSize = 20;
            el.AddChild(child);

            Layout(el);

            var txt = child.Children[1];

            Assert.Equal(new Point(10, 70), txt.ClientRect.LeftBottom);
        }

        [Fact]
        public void InlineBlock()
        {
            var el = CreateRootElement();

            Element child;
            Element box;

            var box1 = box = new Element();
            box.Data = "b1";
            box.Style.Display = StyleDisplay.Block;
            box.Style.Position = StylePosition.Static;
            box.Style.Width = 20;
            box.Style.Height = 20;
            el.AddChild(box);

            var child1 = child = new TextElement();
            child.Data = "child";
            child.Style.Height = 30;
            child.Style.Display = StyleDisplay.InlineBlock;
            child.Style.Position = StylePosition.Static;
            (child as TextElement).Content = "Testduck Testduck2 Testduck3";
            (child as TextElement).TextSize = 20;
            el.AddChild(child);

            Layout(el);

            var txt = child.ChildrenInternal[0];

            Assert.Equal(1, child.ChildrenInternal.Count);
            Assert.Equal(new Point(10, 60), txt.ClientRect.LeftBottom);
        }

        [Fact]
        public void Flow()
        {
            var el = CreateRootElement();

            Element child;
            Element box;

            var box1 = box = new Element();
            box.Data = "b1";
            box.Style.Display = StyleDisplay.InlineBlock;
            box.Style.Position = StylePosition.Static;
            box.Style.Width = 20;
            box.Style.Height = 20;
            el.AddChild(box);

            var child1 = child = new TextElement();
            child.Data = "child";
            child.Style.Width = 5;
            child.Style.Height = 30;
            child.Style.Display = StyleDisplay.Inline;
            child.Style.Position = StylePosition.Static;
            (child as TextElement).Content = "Testduck Testduck2 Testduck3";
            (child as TextElement).TextSize = 20;
            el.AddChild(child);

            var box2 = box = new Element();
            box.Data = "b1";
            box.Style.Display = StyleDisplay.InlineBlock;
            box.Style.Position = StylePosition.Static;
            box.Style.Width = 20;
            box.Style.Height = 20;
            el.AddChild(box);

            var child2 = child = new TextElement();
            child.Data = "child2";
            child.Style.Width = 5;
            child.Style.Height = 30;
            child.Style.Display = StyleDisplay.Inline;
            child.Style.Position = StylePosition.Static;
            (child as TextElement).Content = "Testduck4 Testduck5 Testduck6";
            (child as TextElement).TextSize = 20;
            el.AddChild(child);

            var box3 = box = new Element();
            box.Data = "b3";
            box.Style.Display = StyleDisplay.InlineBlock;
            box.Style.Position = StylePosition.Static;
            box.Style.Width = 30;
            box.Style.Height = 20;
            el.AddChild(box);

            var child3 = child = new TextElement();
            child.Data = "child3";
            child.Style.Height = 30;
            child.Style.Width = 5;
            child.Style.Display = StyleDisplay.Inline;
            child.Style.Position = StylePosition.Static;
            (child as TextElement).Content = "Testduck4 Testduck5 Testxk|²³|ₚ|ganz6";
            (child as TextElement).TextSize = 20;
            el.AddChild(child);

            Layout(el);

            var txt1 = child1.ChildrenInternal.Last();
            var txt2 = child2.ChildrenInternal.Last();
            var txt3 = child3.ChildrenInternal.Last();

            Assert.Equal(new Point(10, 70), txt1.ClientRect.LeftBottom);
            Assert.Equal(new Point(10, 100), txt2.ClientRect.LeftBottom);
            Assert.Equal(new Point(10, 160), txt3.ClientRect.LeftBottom);

            Assert.Equal(new Point(10, 40), box1.ClientRect.LeftBottom);
            Assert.Equal(70, box2.ClientRect.Bottom);
            Assert.Equal(new Point(10, 130), box3.ClientRect.LeftBottom);
        }

    }
}
