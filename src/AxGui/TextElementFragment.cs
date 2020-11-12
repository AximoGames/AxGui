// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    internal class TextElementFragment
    {

        public TextElementFragment(string value)
        {
            //Paint = paint;
            Text = value;
        }

        //internal readonly SKPaint Paint;
        internal readonly string Text;
        public SKPoint DrawPosition;
    }

}
