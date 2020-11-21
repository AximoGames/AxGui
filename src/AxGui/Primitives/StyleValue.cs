// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace AxGui
{

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public unsafe struct StyleValue : IEquatable<StyleValue>
    {
        [FieldOffset(0)]
        public float Number; // 4 bytes
        [FieldOffset(4)]
        public StyleUnit Unit; // 4 bytes
        [FieldOffset(8)]
        public string Value; // 8 bytes

        [FieldOffset(0)]
        public SKColor Color; // 4 bytes

        //public SKColor Color
        //{
        //    get
        //    {
        //        if (Unit != StyleUnit.Color)
        //            return SKColor.Empty;

        //        fixed (float* storagePtr = &Number)
        //        {
        //            SKColor* colorPtr = (SKColor*)storagePtr;
        //            return *colorPtr;
        //        }
        //    }
        //    set
        //    {
        //        Unit = StyleUnit.Color;
        //        fixed (float* storagePtr = &Number)
        //        {
        //            SKColor* colorPtr = (SKColor*)storagePtr;
        //            *colorPtr = value;
        //        }
        //    }
        //}

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Unit);
        }

        public override bool Equals(object? obj)
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

            if (value.EndsWith("%", StringComparison.InvariantCulture))
            {
                v.Number = float.Parse(value.AsSpan(0, value.Length - 1), provider: CultureInfo.InvariantCulture.NumberFormat);
                v.Unit = StyleUnit.Percentage;
            }
            else if (value.EndsWith("px", StringComparison.InvariantCulture))
            {
                v.Number = float.Parse(value.AsSpan(0, value.Length - 2), provider: CultureInfo.InvariantCulture.NumberFormat);
                v.Unit = StyleUnit.Number;
            }
            else if (value == "auto")
            {
                v.Unit = StyleUnit.Auto;
            }
            else if (value == "initial")
            {
                v.Unit = StyleUnit.Initial;
            }
            else if (value == "inherit")
            {
                v.Unit = StyleUnit.Inherit;
            }
            else if (value == "unset")
            {
                v.Unit = StyleUnit.Unset;
            }
            else if (value.StartsWith("#"))
            {
                SKColor.TryParse(value, out v.Color);
            }
            else if (value.StartsWith("rgb("))
            {
                v.Unit = StyleUnit.Color;
                var parts = value.Substring(4, value.Length - 5).Replace(" ", "").Split(",");
                if (parts.Length == 3)
                    v.Color = new SKColor(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]));
            }
            else if (value.StartsWith("rgba("))
            {
                v.Unit = StyleUnit.Color;
                var parts = value.Substring(5, value.Length - 6).Replace(" ", "").Split(",");
                if (parts.Length == 4)
                    v.Color = new SKColor(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]), (byte)Math.Round(float.Parse(parts[3], CultureInfo.InvariantCulture) * 255));
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

        public static StyleValue Combine(StyleValue parent, StyleValue child)
        {
            if (child.Unit is StyleUnit.Unset or StyleUnit.Inherit)
                return parent;

            return child;
        }

        public override string ToString()
        {
            return Unit switch
            {
                StyleUnit.Number => Number.ToString("F1", CultureInfo.InvariantCulture),
                StyleUnit.Pixel => Number.ToString("F1", CultureInfo.InvariantCulture) + "px",
                StyleUnit.Percentage => Number.ToString("F1", CultureInfo.InvariantCulture) + " %",
                StyleUnit.Color => Color.ToString(),
#pragma warning disable HAA0102 // Non-overridden virtual method call on value type
                _ => Unit.ToString(),
#pragma warning restore HAA0102 // Non-overridden virtual method call on value type
            };
        }

    }

}
