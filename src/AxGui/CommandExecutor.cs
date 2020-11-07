// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class CommandExecutor
    {

        private CommandContext? _CommandContext;

        public void Execute(CommandRecorder recorder, SKCanvas canvas)
        {
            var ctx = _CommandContext;
            if (ctx == null)
            {
                ctx = new CommandContext(canvas);
                _CommandContext = ctx;
            }
            else
            {
                ctx.Canvas = canvas;
            }

            var rootContext = recorder._RenderContext;
            if (rootContext == null)
                throw new Exception("rootContext == null");

            var cmds = rootContext.Commands;
            for (var i = 0; i < cmds.Count; i++)
            {
                cmds[i].Invoke(ctx);
            }
        }
    }

}
