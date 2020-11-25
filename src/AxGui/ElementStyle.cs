// This file is part of AxGUI. Web: https://github.com/AximoGames
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
        Flex,
        Grid,
        Inline,
        InlineBlock,
        InlineFlex,
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

    /// <summary>
    /// Horizontal align
    /// </summary>
    public enum StyleJustifyContenet
    {
        Unset,
        Initial,
        Inherit,

        Center,
    }

    public enum StyleFlexDirection
    {
        Unset,
        Initial,
        Inherit,

        Row,
        Column,
    }

    /// <summary>
    /// Vertical align
    /// </summary>
    public enum StyleAlignItems
    {
        Unset,
        Initial,
        Inherit,

        Center,
    }

    public enum StylePosition
    {
        Unset,
        Initial,
        Inherit,

        Static,
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

        public void CopyTo(ElementStyle target)
        {
            target._BoundingChanged = _BoundingChanged;

            target._Anchors = _Anchors;
            target._Margin = _Margin;
            target._BorderColor = _BorderColor;
            target._BorderStyle = _BorderStyle;
            target._BorderWidth = _BorderWidth;
            target._BorderRadius = _BorderRadius;
            target._Padding = _Padding;

            target._MaxSize = _MaxSize;
            target._MinSize = _MinSize;
            target._Size = _Size;

            target._BackgroundColor = _BackgroundColor;
            target._Color = _Color;
            target._FontSize = _FontSize;
            target._FontFamily = _FontFamily;

            target.Display = Display;
            target.Position = Position;
            target.Visibility = Visibility;

            target.JustifyContent = JustifyContent;
            target.AlignItems = AlignItems;
            target.FlexDirection = FlexDirection;
            target._FlexGrow = _FlexGrow;
        }

        public static ElementStyle Combine(ElementStyle parent, ElementStyle child)
        {
            var target = new ElementStyle();
            Combine(target, parent, child);
            return target;
        }

        public static void Combine(ElementStyle target, ElementStyle parent, ElementStyle child)
        {
            target._BoundingChanged = true;

            target._Anchors = StyleRect.Combine(parent._Anchors, child._Anchors);
            target._Margin = StyleRect.Combine(parent._Margin, child._Margin);
            target._BorderColor = StyleRect.Combine(parent._BorderColor, child._BorderColor);
            target._BorderWidth = StyleRect.Combine(parent._BorderWidth, child._BorderWidth);
            target._BorderStyle = StyleRect.Combine(parent._BorderStyle, child._BorderStyle);
            target._BorderRadius = StyleCorners.Combine(parent._BorderRadius, child._BorderRadius);
            target._Padding = StyleRect.Combine(parent._Padding, child._Padding);

            target._MaxSize = StyleSize.Combine(parent._MaxSize, child._MaxSize);
            target._MinSize = StyleSize.Combine(parent._MinSize, child._MinSize);
            target._Size = StyleSize.Combine(parent._Size, child._Size);

            target._BackgroundColor = StyleValue.Combine(parent._BackgroundColor, child._BackgroundColor);
            target._Color = StyleValue.Combine(parent._Color, child._Color);
            target._FontSize = StyleValue.Combine(parent._FontSize, child._FontSize);
            target._FontFamily = StyleValue.Combine(parent._FontFamily, child._FontFamily);

            target.Display = StyleHelper.CombineEnum(parent.Display, child.Display);
            target.Position = StyleHelper.CombineEnum(parent.Position, child.Position);
            target.Visibility = StyleHelper.CombineEnum(parent.Visibility, child.Visibility);

            target.JustifyContent = StyleHelper.CombineEnum(parent.JustifyContent, child.JustifyContent);
            target.AlignItems = StyleHelper.CombineEnum(parent.AlignItems, child.AlignItems);
            target.FlexDirection = StyleHelper.CombineEnum(parent.FlexDirection, child.FlexDirection);
            target._FlexGrow = StyleValue.Combine(parent._FlexGrow, child._FlexGrow);
        }

        internal static void SetRule(ElementStyle target, ExCSS.StyleDeclaration parent)
        {
            target._BoundingChanged = true;

            target._Anchors = new StyleRect(parent.Left, parent.Top, parent.Right, parent.Bottom);
            target._Margin = StyleRect.Combine(parent.Margin, new StyleRect(parent.MarginLeft, parent.MarginTop, parent.MarginRight, parent.MarginBottom));
            target._BorderColor = StyleRect.Combine(parent.BorderColor, new StyleRect(parent.BorderLeftColor, parent.BorderTopColor, parent.BorderRightColor, parent.BorderBottomColor));
            target._BorderWidth = StyleRect.Combine(parent.BorderWidth, new StyleRect(parent.BorderLeftWidth, parent.BorderTopWidth, parent.BorderRightWidth, parent.BorderBottomWidth));
            target._BorderStyle = StyleRect.Combine(parent.BorderStyle, new StyleRect(parent.BorderLeftStyle, parent.BorderTopStyle, parent.BorderRightStyle, parent.BorderBottomStyle));
            target._BorderRadius = StyleCorners.Combine(parent.BorderRadius, new StyleCorners(parent.BorderTopLeftRadius, parent.BorderTopRightRadius, parent.BorderBottomLeftRadius, parent.BorderBottomRightRadius));
            target._Padding = StyleRect.Combine(parent.Padding, new StyleRect(parent.PaddingLeft, parent.PaddingTop, parent.PaddingRight, parent.PaddingBottom));

            target._MaxSize = new StyleSize(parent.MaxWidth, parent.MaxHeight);
            target._MinSize = new StyleSize(parent.MinWidth, parent.MinHeight);
            target._Size = new StyleSize(parent.Width, parent.Height);

            target._BackgroundColor = parent.BackgroundColor;
            target._Color = parent.Color;
            target._FontSize = parent.FontSize;
            target._FontFamily = parent.FontFamily;

            target.Display = StyleHelper.ParseEnum<StyleDisplay>(parent.Display);
            target.Position = StyleHelper.ParseEnum<StylePosition>(parent.Position);
            target.Visibility = StyleHelper.ParseEnum<StyleVisibility>(parent.Visibility);

            target.JustifyContent = StyleHelper.ParseEnum<StyleJustifyContenet>(parent.JustifyContent);
            target.AlignItems = StyleHelper.ParseEnum<StyleAlignItems>(parent.AlignItems);
            target.FlexDirection = StyleHelper.ParseEnum<StyleFlexDirection>(parent.FlexDirection);
            target._FlexGrow = parent.FlexGrow;

            // temporary fix
            if (parent.TextAlign == "center")
                target.JustifyContent = StyleJustifyContenet.Center;
            if (parent.VerticalAlign == "middle")
                target.AlignItems = StyleAlignItems.Center;
        }

        internal static void WriteRule(ExCSS.StyleDeclaration target, ElementStyle parent)
        {
            target.Left = parent.Left.ToCss();
            target.Top = parent.Top.ToCss();
            target.Right = parent.Right.ToCss();
            target.Bottom = parent.Bottom.ToCss();

            target.MarginLeft = parent.MarginLeft.ToCss();
            target.MarginTop = parent.MarginTop.ToCss();
            target.MarginRight = parent.MarginRight.ToCss();
            target.MarginBottom = parent.MarginBottom.ToCss();

            //target._BorderColor = StyleRect.Combine(parent.BorderColor, new StyleRect(parent.BorderLeftColor, parent.BorderTopColor, parent.BorderRightColor, parent.BorderBottomColor));

            target.BorderLeftWidth = parent.BorderWidth.Left.ToCss();
            target.BorderTopWidth = parent.BorderWidth.Top.ToCss();
            target.BorderRightWidth = parent.BorderWidth.Right.ToCss();
            target.BorderBottomWidth = parent.BorderWidth.Bottom.ToCss();

            //target._BorderStyle = StyleRect.Combine(parent.BorderStyle, new StyleRect(parent.BorderLeftStyle, parent.BorderTopStyle, parent.BorderRightStyle, parent.BorderBottomStyle));
            //target._BorderRadius = StyleCorners.Combine(parent.BorderRadius, new StyleCorners(parent.BorderTopLeftRadius, parent.BorderTopRightRadius, parent.BorderBottomLeftRadius, parent.BorderBottomRightRadius));

            target.PaddingLeft = parent.PaddingLeft.ToCss();
            target.PaddingTop = parent.PaddingTop.ToCss();
            target.PaddingRight = parent.PaddingRight.ToCss();
            target.PaddingBottom = parent.PaddingBottom.ToCss();

            target.MinWidth = parent.MinWidth.ToCss();
            target.MinHeight = parent.MinHeight.ToCss();

            target.MaxWidth = parent.MaxWidth.ToCss();
            target.MaxHeight = parent.MaxHeight.ToCss();

            target.Width = parent.Width.ToCss();
            target.Height = parent.Height.ToCss();

            target.BackgroundColor = parent.BackgroundColor.ToCss();
            target.Color = parent.Color.ToCss();
            target.FontSize = parent.FontSize.ToCss();
            target.FontFamily = parent.FontFamily.ToCss();

            target.Display = StyleHelper.ToCss(parent.Display);
            target.Position = StyleHelper.ToCss(parent.Position);
            target.Visibility = StyleHelper.ToCss(parent.Visibility);

            target.JustifyContent = StyleHelper.ToCss(parent.JustifyContent);
            target.AlignItems = StyleHelper.ToCss(parent.AlignItems);
            target.FlexDirection = StyleHelper.ToCss(parent.FlexDirection);
            target.FlexGrow = parent.FlexGrow.ToCss();
        }

        public static ElementStyle FromString(string styleBlock)
        {
            if (!styleBlock.Contains("{"))
                styleBlock = "a {" + styleBlock + "}";

            var parser = new ExCSS.StylesheetParser(includeUnknownDeclarations: true);
            var sheet = parser.Parse(styleBlock);

            var style = new ElementStyle();
            foreach (ExCSS.StyleRule rule in sheet.StyleRules)
            {
                SetRule(style, rule.Style);
                break;
            }

            return style;
        }

        internal bool _BoundingChanged;
        private void BoundingChanged()
        {
            _BoundingChanged = true;
        }

        private Action BoundingChangedDelegate;

        public StylePosition Position;
        public StyleDisplay Display;
        public StyleVisibility Visibility;

        public StyleJustifyContenet JustifyContent;
        public StyleAlignItems AlignItems;
        public StyleFlexDirection FlexDirection;

        internal StyleValue _FlexGrow;
        internal StyleValue FlexGrow
        {
            get => _FlexGrow;
            set => _FlexGrow = value;
        }

        internal StyleValue _FontFamily;
        internal StyleValue FontFamily
        {
            get => _FontFamily;
            set => _FontFamily = value;
        }

        internal StyleValue _FontSize;
        internal StyleValue FontSize
        {
            get => _FontSize;
            set => _FontSize = value;
        }

        internal StyleValue _Color;
        internal StyleValue Color
        {
            get => _Color;
            set => _Color = value;
        }

        internal StyleValue _BackgroundColor;
        internal StyleValue BackgroundColor
        {
            get => _BackgroundColor;
            set => _BackgroundColor = value;
        }

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

        internal StyleSize _MaxSize;
        public StyleSize MaxSize
        {
            get => _MaxSize;
            set
            {
                if (_MaxSize == value)
                    return;
                _MaxSize = value;
                BoundingChanged();
            }
        }

        public StyleValue MaxWidth
        {
            get => _MaxSize.Width;
            set => MaxSize = new StyleSize(value, _MaxSize.Height);
        }

        public StyleValue MaxHeight
        {
            get => _MaxSize.Height;
            set => MaxSize = new StyleSize(_MaxSize.Width, value);
        }

        #region Anchors

        internal StyleRect _Anchors;
        public StyleRect Anchors
        {
            get => _Anchors;
            set
            {
                if (_Anchors == value)
                    return;
                _Anchors = value;
                _Anchors.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        public StyleValue Left
        {
            get => _Anchors._Left;
            set
            {
                if (_Anchors._Left == value)
                    return;
                _Anchors._Left = value;
                BoundingChanged();
            }
        }

        public StyleValue Top
        {
            get => _Anchors._Top;
            set
            {
                if (_Anchors._Top == value)
                    return;
                _Anchors._Top = value;
                BoundingChanged();
            }
        }

        public StyleValue Right
        {
            get => _Anchors._Right;
            set
            {
                if (_Anchors._Right == value)
                    return;
                _Anchors._Right = value;
                BoundingChanged();
            }
        }

        public StyleValue Bottom
        {
            get => _Anchors._Bottom;
            set
            {
                if (_Anchors._Bottom == value)
                    return;
                _Anchors._Bottom = value;
                BoundingChanged();
            }
        }

        #endregion

        #region Margin

        internal StyleRect _Margin;
        public StyleRect Margin
        {
            get => _Margin;
            set
            {
                if (_Margin == value)
                    return;
                _Margin = value;
                _Margin.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        public StyleValue MarginLeft
        {
            get => _Margin._Left;
            set
            {
                if (_Margin._Left == value)
                    return;
                _Margin._Left = value;
                BoundingChanged();
            }
        }

        public StyleValue MarginTop
        {
            get => _Margin._Top;
            set
            {
                if (_Margin._Top == value)
                    return;
                _Margin._Top = value;
                BoundingChanged();
            }
        }

        public StyleValue MarginRight
        {
            get => _Margin._Right;
            set
            {
                if (_Margin._Right == value)
                    return;
                _Margin._Right = value;
                BoundingChanged();
            }
        }

        public StyleValue MarginBottom
        {
            get => _Margin._Bottom;
            set
            {
                if (_Margin._Bottom == value)
                    return;
                _Margin._Bottom = value;
                BoundingChanged();
            }
        }

        #endregion

        #region Padding

        internal StyleRect _Padding;
        public StyleRect Padding
        {
            get => _Padding;
            set
            {
                if (_Padding == value)
                    return;
                _Padding = value;
                _Padding.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        public StyleValue PaddingLeft
        {
            get => _Padding._Left;
            set
            {
                if (_Padding._Left == value)
                    return;
                _Padding._Left = value;
                BoundingChanged();
            }
        }

        public StyleValue PaddingTop
        {
            get => _Padding._Top;
            set
            {
                if (_Padding._Top == value)
                    return;
                _Padding._Top = value;
                BoundingChanged();
            }
        }

        public StyleValue PaddingRight
        {
            get => _Padding._Right;
            set
            {
                if (_Padding._Right == value)
                    return;
                _Padding._Right = value;
                BoundingChanged();
            }
        }

        public StyleValue PaddingBottom
        {
            get => _Padding._Bottom;
            set
            {
                if (_Padding._Bottom == value)
                    return;
                _Padding._Bottom = value;
                BoundingChanged();
            }
        }

        #endregion

        internal StyleRect _BorderWidth;
        public StyleRect BorderWidth
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

        internal StyleRect _BorderColor;
        public StyleRect BorderColor
        {
            get => _BorderColor;
            set
            {
                if (_BorderColor == value)
                    return;
                _BorderColor = value;
            }
        }

        internal StyleRect _BorderStyle;
        public StyleRect BorderStyle
        {
            get => _BorderColor;
            set
            {
                if (_BorderColor == value)
                    return;
                _BorderColor = value;
            }
        }

        internal StyleCorners _BorderRadius;
        public StyleCorners BorderRadius
        {
            get => _BorderRadius;
            set
            {
                if (_BorderRadius == value)
                    return;
                _BorderRadius = value;
            }
        }

    }

}
