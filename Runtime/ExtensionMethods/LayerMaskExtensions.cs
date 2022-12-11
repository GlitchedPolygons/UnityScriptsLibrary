using UnityEngine;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// <a href="https://docs.unity3d.com/Documentation/ScriptReference/LayerMask.html">LayerMask</a> extension methods.
    /// </summary>
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Checks whether a <a href="https://docs.unity3d.com/Documentation/ScriptReference/LayerMask.html">LayerMask</a> contains a specific layer or not.
        /// </summary>
        /// <param name="layerMask">The <a href="https://docs.unity3d.com/Documentation/ScriptReference/LayerMask.html">LayerMask</a> to check against.</param>
        /// <param name="layer">The layer to check.</param>
        /// <returns>Whether the <a href="https://docs.unity3d.com/Documentation/ScriptReference/LayerMask.html">LayerMask</a> contains the layer or not.</returns>
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
