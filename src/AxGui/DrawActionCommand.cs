// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public delegate void DrawActionCommandDelegate(CommandContext ctx);

    public class DrawActionCommand : DrawCommand
    {
        private readonly DrawActionCommandDelegate _Action;
        public DrawActionCommand(DrawActionCommandDelegate action)
        {
            _Action = action;
        }

        public override void Invoke(CommandContext ctx)
        {
            _Action(ctx);
        }
    }

}
