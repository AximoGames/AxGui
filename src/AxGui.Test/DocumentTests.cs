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
<html>
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
            Assert.Equal("div", doc.Body.Children[0].TagName);
        }

    }
}
