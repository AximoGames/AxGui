﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class GlobalRenderContext
    {
        internal readonly List<RenderContext> RenderContextList = new List<RenderContext>();

        public void AddRenderContext(RenderContext ctx)
        {
            RenderContextList.Add(ctx);
        }

        public void Reset()
        {
            RenderContextList.Clear();
        }

        public override string ToString()
        {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            return $"RenderContext.Count: {RenderContextList.Count}";
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
        }

    }

}
