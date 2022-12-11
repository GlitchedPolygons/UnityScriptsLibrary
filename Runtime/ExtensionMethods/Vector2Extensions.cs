using UnityEngine;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// <see cref="UnityEngine.Vector3"/> extension methods.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Returns <c>UnityEngine.Random.Range(vector.x, vector.y)</c>. 
        /// </summary>
        /// <param name="vector">The <see cref="Vector2"/> that contains the range within which a random number should be picked.</param>
        /// <returns>A random number between <see cref="Vector2.x"/> and <see cref="Vector2.y"/>.</returns>
        public static float GetRandomValue(this Vector2 vector)
        {
            return Random.Range(vector.x, vector.y);
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com
