// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing.Imaging;
using System.Globalization;

namespace AxGui
{

    public struct StyleValue : IEquatable<StyleValue>
    {
        public float Number;
        //public string String;
        public StyleUnit Unit;

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Unit);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StyleValue m))
                return false;

            return this == m;
        }

        public bool Equals(StyleValue other)
        {
            return this == other;
        }

        public static bool operator ==(StyleValue x, StyleValue y)
        {
            return x.Unit == y.Unit && x.Number == y.Number;
        }

        public static bool operator !=(StyleValue x, StyleValue y)
        {
            return !(x == y);
        }

        public bool HasValue()
        {
            return Unit > StyleUnit.Inherit && Unit != StyleUnit.Auto;
        }

        public static implicit operator StyleValue(float value)
        {
            return new StyleValue
            {
                Number = value,
                Unit = StyleUnit.Pixel,
            };
        }

        public static implicit operator StyleValue(string value)
        {
            var v = new StyleValue();
            if (string.IsNullOrEmpty(value))
                return v;

            if (value.EndsWith("%"))
            {
                v.Number = float.Parse(value.AsSpan(0, value.Length - 1), provider: CultureInfo.InvariantCulture.NumberFormat);
                v.Unit = StyleUnit.Percentage;
            }
            else if (value.EndsWith("px"))
            {
                v.Number = float.Parse(value.AsSpan(0, value.Length - 1), provider: CultureInfo.InvariantCulture.NumberFormat);
                v.Unit = StyleUnit.Number;
            }
            else
            {
                v.Number = float.Parse(value, provider: CultureInfo.InvariantCulture.NumberFormat);
                v.Unit = StyleUnit.Number;
            }

            return v;
        }

        internal StyleValue Normalize(ProcessLayoutContext ctx, Axis axis)
        {
            var value = this;
            switch (Unit)
            {
                case StyleUnit.Percentage:
                    value.Number = (ctx.LocalViewPort.Size(axis) / 100) * Number;
                    value.Unit = StyleUnit.Pixel;
                    break;
            }
            return value;
        }

    }

}
