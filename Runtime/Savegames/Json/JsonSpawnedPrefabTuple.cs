using System;

namespace GlitchedPolygons.SavegameFramework.Json
{
    /// <summary>
    /// Necessary for allowing Unity's <c>JsonUtility</c> to correctly serialize all <see cref="SpawnedPrefab"/>s and their <see cref="JsonSaveableComponent"/> contents.
    /// </summary>
    [Serializable]
    public class JsonSpawnedPrefabTuple
    {
        /// <summary>
        /// The <a href="https://docs.unity3d.com/ScriptReference/Resources.html">Resources</a> path to use for reconstructing the <see cref="SpawnedPrefab"/> on load.
        /// </summary>
        public string resourcePath = string.Empty;

        /// <summary>
        /// Json content of the <see cref="JsonSaveableComponent"/>'s data.
        /// </summary>
        public string json = string.Empty;
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com