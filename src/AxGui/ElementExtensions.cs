// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SkiaSharp;

namespace AxGui
{

    public static class ElementExtensions
    {

        public static void SetStyle(this Element element, ElementStyle style)
        {
            style.CopyTo(element.Style);
        }

    }

}
