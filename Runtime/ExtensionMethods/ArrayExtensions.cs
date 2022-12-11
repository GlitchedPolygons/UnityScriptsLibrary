using System;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// Extension methods for <see cref="System.Array"/>s.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Determines whether the array is <c>null</c> or empty.
        /// </summary>
        /// <param name="array">The array to check.</param>
        /// <returns><c>true</c> if the array is either <c>null</c> or empty; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this Array array)
        {
            return array is null || array.Length == 0;
        }
    }
}

// Copyright (C) Raphael Beck, 2019 | https://glitchedpolygons.com
