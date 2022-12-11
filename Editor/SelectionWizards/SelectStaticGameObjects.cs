using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Extension method class for quickly selecting all <see cref="GameObject"/>s marked as static in the scene.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour"/>
    public static class SelectStaticGameObjects
    {
        /// <summary>
        /// Selects all <see cref="GameObject"/>s that are static.
        /// </summary>
        [MenuItem("Glitched Polygons/Quick-select/Select all static GameObjects")]
        private static void SelectAllStaticGameObjects()
        {
            var selection = new List<GameObject>(50);
            GameObject[] gameObjects = Object.FindObjectsOfType<GameObject>();

            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                float progress = 1.0f - (float)i / gameObjects.Length;
                EditorUtility.DisplayProgressBar("Quick-select progress", "Gathering selection... please wait!", progress);

                if (gameObjects[i].isStatic)
                {
                    selection.Add(gameObjects[i]);
                }
            }

            EditorUtility.ClearProgressBar();
            Selection.objects = selection.ToArray();
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com
