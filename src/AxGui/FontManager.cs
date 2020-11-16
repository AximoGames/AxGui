// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using SkiaSharp;

namespace AxGui
{

    public static class FontManager
    {

        internal static SKTypeface DefaultTypeFace { get; }

        static FontManager()
        {
            DefaultTypeFace = LoadEmbeddedTypeFace();
        }

        private static SKTypeface LoadEmbeddedTypeFace()
        {
            var fileProvider = new EmbeddedFileProvider(typeof(TextElement).Assembly);
            var fileInfo = fileProvider.GetFileInfo("Roboto-Regular.ttf.gz");
            using var stream = fileInfo.CreateReadStream();
            using GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress);
            return SKTypeface.FromStream(decompressionStream);
        }

    }

}
