// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Xunit;

namespace AxGui.Test
{
    public class HtmlWriterTests : TestBase
    {

        [Fact]
        public void HtmlStyle()
        {
            var el = new Element();
            var m = el.ToHtml();
            Assert.Equal("<div />", m);
        }

        [Fact]
        public void HtmlStyleLeft()
        {
            var el = new Element();
            el.Style.Left = 5;
            var m = el.ToHtml();
            Assert.Equal(@"<div style=""left: 5px"" />", m);
        }

    }
}
