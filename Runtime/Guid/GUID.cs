using System;
using UnityEngine;

namespace GlitchedPolygons.Identification
{
    /// <summary>
    /// The <see cref="GUID"/> component holds a unique <see cref="System.Guid"/> string
    /// that can be used for identifying objects throughout sessions (like when loading a savegame for instance).
    /// </summary>
    [ExecuteInEditMode]
    public sealed class GUID : MonoBehaviour
    {
        /// <summary>
        /// The actual string containing the guid.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private string guid = null;

        /// <summary>
        /// Gets the current guid string.
        /// </summary>
        /// <returns>The current guid string.</returns>
        public string GetGUID() => guid;

        /// <summary>
        /// Assigns a new random <see cref="Guid"/>.
        /// </summary>
        public void ChangeGUID() => guid = Guid.NewGuid().ToString();

        /// <summary>
        /// Assigns a specific <see cref="Guid"/>.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> to assign.</param>
        public void ChangeGUID(Guid guid) => this.guid = guid.ToString();
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
