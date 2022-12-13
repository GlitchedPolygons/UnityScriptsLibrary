using System;
using System.Collections.Generic;

namespace GlitchedPolygons.SavegameFramework.Json
{
    /// <summary>
    /// Data holder class for the <see cref="JsonSavegameManager"/> implementation of the <see cref="GlitchedPolygons.SavegameFramework"/>.<para> </para>
    /// This is what is serialized out on save using Unity's <c>JsonUtility.ToJson(object)</c>.
    /// </summary>
    [Serializable]
    public class JsonSavegame
    {
        /// <summary>
        /// Scene build index.
        /// </summary>
        public int mapIndex = -2;
        
        /// <summary>
        /// Scene name.
        /// </summary>
        public string mapName = "map_name_here";
        
        /// <summary>
        /// The saved <see cref="JsonSaveableComponent"/>s.
        /// </summary>
        public List<JsonSaveableComponentTuple> components = new();
        
        /// <summary>
        /// The saved <see cref="SpawnedPrefab"/>s.
        /// </summary>
        public List<JsonSpawnedPrefabTuple> spawnedPrefabs = new();
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com