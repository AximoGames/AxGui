// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;

namespace AxGui
{

    public class StyleRule
    {
        public string Selector;
        public ElementStyle Style;

        public StyleRule(string selector, ElementStyle style)
        {
            Selector = selector;
            Style = style;
        }
    }

}
