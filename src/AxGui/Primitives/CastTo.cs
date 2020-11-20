// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace AxGui
{
    // See: https://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int

    /// <summary>
    /// Class to cast to type <see cref="T"/>
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    public static class CastTo<T>
    {
        /// <summary>
        /// Casts <see cref="S"/> to <see cref="T"/> without arithmetic overflow check
        /// This does not cause boxing for value types.
        /// Useful in generic methods.
        /// </summary>
        /// <typeparam name="S">Source type to cast from. Usually a generic type.</typeparam>
        public static T From<S>(S s)
        {
            return Cache<S>.Caster(s);
        }

        /// <summary>
        /// Casts <see cref="S"/> to <see cref="T"/> with arithmetic overflow check
        /// This does not cause boxing for value types.
        /// Useful in generic methods.
        /// </summary>
        /// <typeparam name="S">Source type to cast from. Usually a generic type.</typeparam>
        public static T FromChecked<S>(S s)
        {
            return Cache<S>.CasterChecked(s);
        }

        private static class Cache<S>
        {
            public static readonly Func<S, T> Caster = Get();

            private static Func<S, T> Get()
            {
                var p = Expression.Parameter(typeof(S));
                var c = Expression.Convert(p, typeof(T));
                return Expression.Lambda<Func<S, T>>(c, p).Compile();
            }

            public static readonly Func<S, T> CasterChecked = GetChecked();

            private static Func<S, T> GetChecked()
            {
                var p = Expression.Parameter(typeof(S));
                var c = Expression.ConvertChecked(p, typeof(T));
                return Expression.Lambda<Func<S, T>>(c, p).Compile();
            }

        }
    }

}
