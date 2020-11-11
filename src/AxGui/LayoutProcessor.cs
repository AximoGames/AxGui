// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class LayoutProcessor
    {
        private readonly GlobalProcessLayoutContext ctx = new GlobalProcessLayoutContext();
        public Box ViewPort;
        public void Process(Element root)
        {
            ctx.GlobalViewPort = ViewPort;
            root.ProcessLayoutContext.LocalViewPort = ViewPort;
            root.CallComputeStyle(ctx);
            root.CallComputeChildBoundsOffers(ctx);
            root.CallComputeBounds(ctx);
        }
    }

}
