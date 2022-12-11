using UnityEngine;

namespace GlitchedPolygons.Identification
{
    /// <summary>
    /// Unique ID for a GameObject.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class ID : MonoBehaviour
    {
        [SerializeField]
        private int id;

        /// <summary>
        /// Gets the GameObject's identifier.
        /// </summary>
        /// <returns>The GameObject's integer identifier.</returns>
        public int GetID() => id;
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
