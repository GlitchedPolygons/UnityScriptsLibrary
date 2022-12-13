using System;

namespace GlitchedPolygons.SavegameFramework.Json
{
    /// <summary>
    /// Necessary for allowing Unity's <c>JsonUtility</c> to correctly serialize all <see cref="JsonSaveableComponent"/>s alongside their IDs.
    /// </summary>
    [Serializable]
    public class JsonSaveableComponentTuple
    {
        /// <summary>
        /// The <see cref="JsonSaveableComponent"/>'s <see cref="JsonSaveableComponent.ID"/>.
        /// </summary>
        public int id = -1;
        
        /// <summary>
        /// Json content of the <see cref="JsonSaveableComponent"/>'s data.
        /// </summary>
        public string json = string.Empty;
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com