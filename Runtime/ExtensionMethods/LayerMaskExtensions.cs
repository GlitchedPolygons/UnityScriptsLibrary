using UnityEngine;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// <see cref="LayerMask"/> extension methods.
    /// </summary>
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Checks whether a <see cref="LayerMask"/> contains a specific layer or not.
        /// </summary>
        /// <param name="layerMask">The <see cref="LayerMask"/> to check against.</param>
        /// <param name="layer">The layer to check.</param>
        /// <returns>Whether the <see cref="LayerMask"/> contains the layer or not.</returns>
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
