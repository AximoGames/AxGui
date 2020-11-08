// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography.X509Certificates;

namespace AxGui
{

    public readonly struct StyleSize : IEquatable<StyleSize>
    {
        public readonly StyleValue Width;
        public readonly StyleValue Height;

        public StyleSize(StyleValue width, StyleValue height)
        {
            Width = width;
            Height = height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StyleSize m))
                return false;

            return this == m;
        }

        public bool Equals(StyleSize other)
        {
            return this == other;
        }

        public static bool operator ==(StyleSize x, StyleSize y)
        {
            return x.Width == y.Width && x.Height == y.Height;
        }

        public static bool operator !=(StyleSize x, StyleSize y)
        {
            return !(x == y);
        }

        public Size ToSize() => new Size(Width.Number, Height.Number);

    }

}
