using UnityEngine;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// <a href="https://docs.unity3d.com/ScriptReference/Vector3">UnityEngine.Vector3</a> extension methods.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Returns <c>UnityEngine.Random.Range(vector.x, vector.y)</c>. 
        /// </summary>
        /// <param name="vector">The <a href="https://docs.unity3d.com/ScriptReference/Vector2">UnityEngine.Vector2</a> that contains the range within which a random number should be picked.</param>
        /// <returns>A random number between <a href="https://docs.unity3d.com/ScriptReference/Vector2-x">Vector2.x</a> and <a href="https://docs.unity3d.com/ScriptReference/Vector2-y">Vector2.y</a>.</returns>
        public static float GetRandomValue(this Vector2 vector)
        {
            return Random.Range(vector.x, vector.y);
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com
