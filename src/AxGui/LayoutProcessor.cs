﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class LayoutProcessor
    {
        private ProcessLayoutContext ctx = new ProcessLayoutContext();
        public Box ViewPort;
        public void Process(Element root)
        {
            ctx.GlobalViewPort = ViewPort;
            ctx.LocalViewPort = ViewPort;
            root.ComputeStyle(ctx);
            root.ComputeChildBoundsOffers();
            root.ComputeBounds(ctx);
        }
    }

}