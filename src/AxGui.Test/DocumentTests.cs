// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace AxGui.Test
{
    public class DocumentTests : TestBase
    {

        [Fact]
        public void ParseEmptyDocument()
        {
            var doc = Document.FromString("");
            Assert.Empty(doc._Body.Children);
        }

        [Fact]
        public void ParseDocument()
        {
            var html = @"
<html><head></head>
<body>
<div>
</div>
</body>
</html>";

            var doc = Document.FromString(html);

            Assert.Empty(doc._Body.Children[0].Children);
            Assert.Equal("div", doc._Body.Children[0].TagName);
        }

        [Fact]
        public void ParseFragment()
        {
            var html = @"
<div></div>
<div></div>
";

            var doc = Document.FromString(html);

            Assert.Empty(doc._Body.Children[0].Children);
            Assert.Equal(2, doc._Body.Children.Count);
            Assert.Equal("div", doc._Body.Children[1].TagName);
        }

        [Fact]
        public void ParseStyleAttribute()
        {
            var html = @"<div style='width:10px'></div>";
            var doc = Document.FromString(html);
            Assert.Equal("10px", doc._Body.Children[0].Style.Width);
        }

        [Fact]
        public void ParseStyleElement()
        {
            var html = @"<html>
<head><style>
.testClass { width: 10px }
</style></head>
<body><div></div></body>
</html>";
            var doc = Document.FromString(html);
            Assert.Equal(".testClass", doc.Styles.GetRuleByClass("testClass")?.Selector);
            Assert.Equal("div", doc._Body.Children[0].TagName);
            Assert.Equal(1, doc._Body.Children.Count);
        }

        //        [Fact]
        //        public void ParseText()
        //        {
        //            var html = @"<span>
        //abc
        // def</span>";

        //            var doc = Document.FromString(html);

        //            Assert.IsType<TextElement>(doc._Body.Children[0]);
        //            Assert.Empty(doc._Body.Children[0].Children);
        //            AssertEqualsNormalizeLineBreaks(@"
        //abc
        // def", (doc._Body.Children[0] as TextElement)?.Content);
        //        }

    }
}
