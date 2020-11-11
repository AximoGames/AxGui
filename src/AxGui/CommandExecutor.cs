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
            var commandContext = _CommandContext;
            if (commandContext == null)
            {
                commandContext = new CommandContext(canvas);
                _CommandContext = commandContext;
            }
            else
            {
                commandContext.Canvas = canvas;
            }

            var rootContext = recorder.RenderContext;
            if (rootContext == null)
                throw new Exception("rootContext == null");

            var renderContextList = rootContext.RenderContextList;
            for (var rci = 0; rci < renderContextList.Count; rci++)
            {
                var renderContext = renderContextList[rci];
                var commands = renderContext.Commands;
                for (var i = 0; i < commands.Count; i++)
                {
                    commands[i].Invoke(commandContext);
                }
            }

        }
    }

}
