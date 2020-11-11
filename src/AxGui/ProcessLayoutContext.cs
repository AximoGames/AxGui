// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class ProcessLayoutContext
    {
        public Box LocalViewPort;
        internal GlobalProcessLayoutContext? GlobalContext;
        internal readonly List<Element> RowElements = new List<Element>();
        internal Point RowPosition;
        internal float RowHeight;
    }

}
