#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Collection of small and handy editor utilities for (un)flagging GameObjects in bulk.<para> </para> 
    /// These are meant to facilitate workflow inside Unity.
    /// </summary>
    static class BulkFlagger
    {
        /// <summary>
        /// Flag all <see cref="GameObject"/>s in the scene as static.
        /// </summary>
        [MenuItem("Glitched Polygons/Flag everything/Static")]
        private static void FlagStatic()
        {
            GameObject[] everything = Object.FindObjectsOfType<GameObject>();
            for (int i = everything.Length - 1; i >= 0; i--)
            {
                everything[i].isStatic = true;
            }
        }

        /// <summary>
        /// Unflag all the <see cref="GameObject"/>s from static.
        /// </summary>
        [MenuItem("Glitched Polygons/Unflag everything/Static")]
        private static void UnflagStatic()
        {
            GameObject[] everything = Object.FindObjectsOfType<GameObject>();
            for (int i = everything.Length - 1; i >= 0; i--)
            {
                everything[i].isStatic = false;
            }
        }

        /// <summary>
        /// Unflag only the static batching property on all <see cref="GameObject"/>s.
        /// </summary>
        [MenuItem("Glitched Polygons/Unflag everything/Static (batching only)")]
        private static void UnflagStaticBatching()
        {
            GameObject[] everything = Object.FindObjectsOfType<GameObject>();
            for (int i = everything.Length - 1; i >= 0; i--)
            {
                StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(everything[i]) & ~StaticEditorFlags.BatchingStatic;
                GameObjectUtility.SetStaticEditorFlags(everything[i], flags);
            }
        }
    }
}

#endif

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com
