// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic.CompilerServices;
using SkiaSharp;

namespace AxGui
{

    public struct StyleCorners : IEquatable<StyleCorners>
    {

        public StyleCorners(StyleValue topLeft, StyleValue topRight, StyleValue bottomLeft, StyleValue bottomRight)
        {
            _TopLeft = topLeft;
            _TopRight = topRight;
            _BottomLeft = bottomLeft;
            _BottomRight = bottomRight;

            ParentChanged = null;
        }

        public StyleCorners(StyleValue topLeft, StyleValue bottomRight)
        {
            _TopLeft = topLeft;
            _TopRight = StyleValue.Unset;
            _BottomLeft = StyleValue.Unset;
            _BottomRight = bottomRight;

            ParentChanged = null;
        }

        public StyleCorners(StyleValue value)
        {
            _TopLeft = value;
            _TopRight = value;
            _BottomLeft = value;
            _BottomRight = value;

            ParentChanged = null;
        }

        internal Action? ParentChanged;

        private void Changed()
        {
            ParentChanged?.Invoke();
        }

        internal StyleValue _TopLeft;
        public StyleValue TopLeft
        {
            get => _TopLeft;
            set
            {
                if (_TopLeft == value)
                    return;
                _TopLeft = value;
                Changed();
            }
        }

        internal StyleValue _TopRight;
        public StyleValue TopRight
        {
            get => _TopRight;
            set
            {
                if (_TopRight == value)
                    return;
                _TopRight = value;
                Changed();
            }
        }

        internal StyleValue _BottomLeft;
        public StyleValue BottomLeft
        {
            get => _BottomLeft;
            set
            {
                if (_BottomLeft == value)
                    return;
                _BottomLeft = value;
                Changed();
            }
        }

        internal StyleValue _BottomRight;
        public StyleValue BottomRight
        {
            get => _BottomRight;
            set
            {
                if (_BottomRight == value)
                    return;
                _BottomRight = value;
                Changed();
            }
        }

        public StyleCorners Clone()
        {
            return new StyleCorners
            {
                TopLeft = TopLeft,
                TopRight = TopRight,
                BottomLeft = BottomLeft,
                BottomRight = BottomRight,
            };
        }

        internal StyleCorners Normalize(ProcessLayoutContext ctx)
        {
            return new StyleCorners
            {
                TopLeft = TopLeft.Normalize(ctx, Axis.X),
                TopRight = TopRight.Normalize(ctx, Axis.Y),
                BottomLeft = BottomLeft.Normalize(ctx, Axis.X),
                BottomRight = BottomRight.Normalize(ctx, Axis.Y),
            };
        }

        public static bool operator ==(StyleCorners x, StyleCorners y)
        {
            return x.TopLeft == y.TopLeft && x.TopRight == y.TopRight && x.BottomLeft == y.BottomLeft && x.BottomRight == y.BottomRight;
        }

        public static bool operator !=(StyleCorners x, StyleCorners y)
        {
            return !(x == y);
        }

        public static bool Equals(StyleCorners left, StyleCorners right)
        {
            return left == right;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is StyleCorners m))
                return false;

            return this == m;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TopLeft, TopRight, BottomLeft, BottomRight);
        }

        public bool Equals(StyleCorners other)
        {
            return this == other;
        }

        public Box ToBox()
        {
            return new Box(TopLeft.Number, TopRight.Number, BottomLeft.Number, BottomRight.Number);
        }

        public static implicit operator StyleCorners(float value)
        {
            return new StyleCorners
            {
                TopLeft = value,
                TopRight = value,
                BottomLeft = value,
                BottomRight = value,
            };
        }

        public static implicit operator StyleCorners(string value)
        {
            if (value.Contains(","))
                return new StyleCorners(value);

            var parts = value.Split(" ");
            if (parts.Length == 1)
                return new StyleCorners(parts[0]);

            if (parts.Length == 2)
                return new StyleCorners(parts[0], parts[1]);

            if (parts.Length == 4)
                return new StyleCorners(parts[0], parts[1], parts[2], parts[3]);

            return new StyleCorners();
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
                    0 => TopLeft,
                    1 => TopRight,
                    2 => BottomLeft,
                    3 => BottomRight,
                    _ => throw new NotSupportedException(),
                };
            }
            set
            {
                _ = index switch
                {
                    0 => TopLeft = value,
                    1 => TopRight = value,
                    2 => BottomLeft = value,
                    3 => BottomRight = value,
                    _ => throw new NotSupportedException(),
                };
            }
        }

        public static StyleCorners Combine(StyleCorners parent, StyleCorners child)
        {
            StyleCorners target = default;
            target.TopLeft = StyleValue.Combine(parent.TopLeft, child.TopLeft);
            target.TopRight = StyleValue.Combine(parent.TopRight, child.TopRight);
            target.BottomLeft = StyleValue.Combine(parent.BottomLeft, child.BottomLeft);
            target.BottomRight = StyleValue.Combine(parent.BottomRight, child.BottomRight);
            return target;
        }

    }

}
