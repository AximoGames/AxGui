// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic.CompilerServices;
using SkiaSharp;

namespace AxGui
{

    public struct StyleRect : IEquatable<StyleRect>
    {

        public StyleRect(StyleValue left, StyleValue top, StyleValue right, StyleValue bottom)
        {
            _Left = left;
            _Top = top;
            _Right = right;
            _Bottom = bottom;

            ParentChanged = null;
        }

        public StyleRect(StyleValue leftRight, StyleValue topBottom)
        {
            _Left = leftRight;
            _Top = topBottom;
            _Right = leftRight;
            _Bottom = topBottom;

            ParentChanged = null;
        }

        public StyleRect(StyleValue value)
        {
            _Left = value;
            _Top = value;
            _Right = value;
            _Bottom = value;

            ParentChanged = null;
        }

        internal Action? ParentChanged;

        private void Changed()
        {
            ParentChanged?.Invoke();
        }

        internal StyleValue _Left;
        public StyleValue Left
        {
            get => _Left;
            set
            {
                if (_Left == value)
                    return;
                _Left = value;
                Changed();
            }
        }

        internal StyleValue _Top;
        public StyleValue Top
        {
            get => _Top;
            set
            {
                if (_Top == value)
                    return;
                _Top = value;
                Changed();
            }
        }

        internal StyleValue _Right;
        public StyleValue Right
        {
            get => _Right;
            set
            {
                if (_Right == value)
                    return;
                _Right = value;
                Changed();
            }
        }

        internal StyleValue _Bottom;
        public StyleValue Bottom
        {
            get => _Bottom;
            set
            {
                if (_Bottom == value)
                    return;
                _Bottom = value;
                Changed();
            }
        }

        public StyleRect Clone()
        {
            return new StyleRect
            {
                Left = Left,
                Top = Top,
                Right = Right,
                Bottom = Bottom,
            };
        }

        internal StyleRect Normalize(ProcessLayoutContext ctx)
        {
            return new StyleRect
            {
                Left = Left.Normalize(ctx, Axis.X),
                Top = Top.Normalize(ctx, Axis.Y),
                Right = Right.Normalize(ctx, Axis.X),
                Bottom = Bottom.Normalize(ctx, Axis.Y),
            };
        }

        public static bool operator ==(StyleRect x, StyleRect y)
        {
            return x.Left == y.Left && x.Top == y.Top && x.Right == y.Right && x.Bottom == y.Bottom;
        }

        public static bool operator !=(StyleRect x, StyleRect y)
        {
            return !(x == y);
        }

        public static bool Equals(StyleRect left, StyleRect right)
        {
            return left == right;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is StyleRect m))
                return false;

            return this == m;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Top, Right, Bottom);
        }

        public bool Equals(StyleRect other)
        {
            return this == other;
        }

        public Box ToBox()
        {
            return new Box(Left.Number, Top.Number, Right.Number, Bottom.Number);
        }

        public static implicit operator StyleRect(float value)
        {
            return new StyleRect
            {
                Left = value,
                Top = value,
                Right = value,
                Bottom = value,
            };
        }

        public static implicit operator StyleRect(string value)
        {
            if (value.Contains(","))
                return new StyleRect(value);

            var parts = value.Split(" ");
            if (parts.Length == 1)
                return new StyleRect(parts[0]);

            if (parts.Length == 2)
                return new StyleRect(parts[0], parts[1]);

            if (parts.Length == 4)
                return new StyleRect(parts[0], parts[1], parts[2], parts[3]);

            return new StyleRect();
        }

        //public StyleValue GetMin(Axis axis)
        //    => axis == Axis.X ? Left : Top;

        //public void SetMin(Axis axis, StyleValue value)
        //{
        //    if (axis == Axis.X)
        //        Left = value;
        //    else
        //        Top = value;
        //}

        //public StyleValue GetMax(Axis axis)
        //    => axis == Axis.X ? Right : Bottom;

        //public void SetMax(Axis axis, StyleValue value)
        //{
        //    if (axis == Axis.X)
        //        Right = value;
        //    else
        //        Bottom = value;
        //}

        public StyleValue this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Left,
                    1 => Top,
                    2 => Right,
                    3 => Bottom,
                    _ => throw new NotSupportedException(),
                };
            }
            set
            {
                _ = index switch
                {
                    0 => Left = value,
                    1 => Top = value,
                    2 => Right = value,
                    3 => Bottom = value,
                    _ => throw new NotSupportedException(),
                };
            }
        }

        public static StyleRect Combine(StyleRect parent, StyleRect child)
        {
            StyleRect target = default;
            target.Left = StyleValue.Combine(parent.Left, child.Left);
            target.Top = StyleValue.Combine(parent.Top, child.Top);
            target.Right = StyleValue.Combine(parent.Right, child.Right);
            target.Bottom = StyleValue.Combine(parent.Bottom, child.Bottom);
            return target;
        }

    }

}
