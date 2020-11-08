﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace AxGui
{

    public enum StyleDisplay
    {
        Unset,
        Initial,
        Inherit,

        Block,
        Inline,
        None,
    }

    public enum StyleVisibility
    {
        Unset,
        Initial,
        Inherit,

        Visible,
        Hidden,
    }

    public enum StylePosition
    {
        Unset,
        Initial,
        Inherit,

        Relative,
        Absolute,
        Fixed,
    }

    public enum BoxSizing
    {
        Unset,
        Initial,
        Inherit,

        ContentBox,
        BorderBox,
    }

    public class ElementStyle
    {

        public ElementStyle()
        {
            BoundingChangedDelegate = BoundingChanged;
            _Margin.ParentChanged = BoundingChangedDelegate;
            _Padding.ParentChanged = BoundingChangedDelegate;
            _BorderWidth.ParentChanged = BoundingChangedDelegate;
            _Anchors.ParentChanged = BoundingChangedDelegate;
        }

        internal bool _BoundingChanged = false;
        private void BoundingChanged()
        {
            _BoundingChanged = true;
        }

        private Action BoundingChangedDelegate;

        public StylePosition Position;
        public StyleDisplay Display;
        public StyleVisibility Visibility;

        internal StyleSize _Size;
        public StyleSize Size
        {
            get => _Size;
            set
            {
                if (_Size == value)
                    return;
                _Size = value;
                BoundingChanged();
            }
        }

        public StyleValue Width
        {
            get => _Size.Width;
            set => Size = new StyleSize(value, _Size.Height);
        }

        public StyleValue Height
        {
            get => _Size.Height;
            set => Size = new StyleSize(_Size.Width, value);
        }

        internal StyleSize _MinSize;
        public StyleSize MinSize
        {
            get => _MinSize;
            set
            {
                if (_MinSize == value)
                    return;
                _MinSize = value;
                BoundingChanged();
            }
        }

        public StyleValue MinWidth
        {
            get => _MinSize.Width;
            set => MinSize = new StyleSize(value, _MinSize.Height);
        }

        public StyleValue MinHeight
        {
            get => _MinSize.Height;
            set => MinSize = new StyleSize(_MinSize.Width, value);
        }

        internal BoxModelRect _Anchors = new BoxModelRect();
        public BoxModelRect Anchors
        {
            get => _Anchors;
            set
            {
                if (_Anchors == value)
                    return;
                _Anchors = value.Clone();
                _Anchors.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        internal BoxModelRect _Margin = new BoxModelRect();
        public BoxModelRect Margin
        {
            get => _Margin;
            set
            {
                if (_Margin == value)
                    return;
                _Margin = value.Clone();
                _Margin.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        internal BoxModelRect _Padding = new BoxModelRect();
        public BoxModelRect Padding
        {
            get => _Padding;
            set
            {
                if (_Padding == value)
                    return;
                _Padding = value.Clone();
                _Padding.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        internal BoxModelRect _BorderWidth = new BoxModelRect();
        public BoxModelRect BorderWidth
        {
            get => _BorderWidth;
            set
            {
                if (_BorderWidth == value)
                    return;
                _BorderWidth = value.Clone();
                _BorderWidth.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        internal BoxModelRect _BorderColor = new BoxModelRect();
        public BoxModelRect BorderColor
        {
            get => _BorderColor;
            set
            {
                if (_BorderColor == value)
                    return;
                _BorderColor = value.Clone();
            }
        }

        internal BoxModelRect _BorderStyle = new BoxModelRect();
        public BoxModelRect BorderStyle
        {
            get => _BorderColor;
            set
            {
                if (_BorderColor == value)
                    return;
                _BorderColor = value.Clone();
            }
        }

        internal BoxModelRect _BorderRadius = new BoxModelRect();
        public BoxModelRect BorderRadius
        {
            get => _BorderRadius;
            set
            {
                if (_BorderRadius == value)
                    return;
                _BorderRadius = value.Clone();
            }
        }

    }

}
