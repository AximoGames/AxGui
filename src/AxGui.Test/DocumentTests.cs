// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace AxGui.Test
{
    public class DocumentTests
    {

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

            Assert.Empty(doc.Body.Children[0].Children);
            Assert.Equal("div", doc.Body.Children[0].TagName);
        }

        [Fact]
        public void ParseFragment()
        {
            var html = @"
<div></div>
<div></div>
";

            var doc = Document.FromString(html);

            Assert.Empty(doc.Body.Children[0].Children);
            Assert.Equal(2, doc.Body.Children.Count);
            Assert.Equal("div", doc.Body.Children[1].TagName);
        }

        [Fact]
        public void ParseStyleAttribute()
        {
            var html = @"<div style='width:10px'></div>";
            var doc = Document.FromString(html);
            Assert.Equal("10px", doc.Body.Children[0].Style.Width);
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
            Assert.Equal("div", doc.Body.Children[0].TagName);
        }

        [Fact]
        public void ParseText()
        {
            var html = @"<span>
abc
 def</span>";

            var doc = Document.FromString(html);

            Assert.IsType<TextElement>(doc.Body.Children[0]);
            Assert.Empty(doc.Body.Children[0].Children);
            Assert.Equal(@"
abc
 def", (doc.Body.Children[0] as TextElement)?.Content?.Replace("\n", "\r\n"));
        }

    }
}
