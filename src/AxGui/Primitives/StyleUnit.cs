// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using EnumsNET;

namespace AxGui
{

    internal static class StyleHelper
    {
        public static T CombineEnum<T>(T parent, T child)
            where T : Enum
        {
            var parentUnit = CastTo<StyleUnit>.From(parent);
            var childUnit = CastTo<StyleUnit>.From(child);

            if (childUnit is StyleUnit.Unset or StyleUnit.Inherit)
                return parent;

            return child;
        }

        public static T ParseEnum<T>(string value)
            where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value))
                return default;

            return Enums.Parse<T>(value.Replace("-", ""), true);
        }
    }

    public enum StyleUnit
    {
        Unset,
        Initial,
        Inherit,

        Number,
        Pixel,
        Percentage,
        //String,
        Auto,
        Color,
    }

}
