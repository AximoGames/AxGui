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
        internal static SKTypeface DefaultSymbolTypeFace { get; }

        internal static SKTypeface FindTypeface(string? fontFamily)
        {
            if (fontFamily == null)
                return DefaultTypeFace;

            switch (fontFamily.ToLower())
            {
                case "symbol":
                case "dejavu":
                    return DefaultSymbolTypeFace;
                default:
                    return DefaultTypeFace;
                    return DefaultTypeFace;
            }
        }

        static FontManager()
        {
            DefaultTypeFace = LoadEmbeddedTypeFace("Roboto-Regular.ttf.gz");
            DefaultSymbolTypeFace = LoadEmbeddedTypeFace("DejaVuSans.ttf.gz");

            //DefaultSymbolTypeFace = DefaultTypeFace;
            //DefaultTypeFace = DefaultSymbolTypeFace;
        }

        private static SKTypeface LoadEmbeddedTypeFace(string fontPath)
        {
            var fileProvider = new EmbeddedFileProvider(typeof(TextElement).Assembly);
            var fileInfo = fileProvider.GetFileInfo(fontPath);
            using var stream = fileInfo.CreateReadStream();
            using GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress);
            return SKTypeface.FromStream(decompressionStream);
        }

    }

}
