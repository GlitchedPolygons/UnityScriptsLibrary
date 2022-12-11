#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Layer quick-selection wizard.
    /// </summary>
    sealed class SelectByLayer : ScriptableWizard
    {
        /// <summary>
        /// Name of the layer.
        /// </summary>
        [SerializeField]
        private string layerName = "< layer name >";

        /// <summary>
        /// Should a progress bar be shown whilst selecting.
        /// </summary>
        [SerializeField]
        private bool showProgressBar = true;

        /// <summary>
        /// The selection progress. [0; 1]
        /// </summary>
        private static float progress = 0.0f;

        /// <summary>
        /// The current selection of <see cref="GameObject"/>s.
        /// </summary>
        private static readonly List<GameObject> SELECTION = new List<GameObject>(300);

        [MenuItem("Glitched Polygons/Quick-select/Select by layer")]
        private static void SelectAllOfTagWizard()
        {
            DisplayWizard<SelectByLayer>("Select all objects on a specific layer...", "Make selection");
        }

        private void OnWizardCreate()
        {
            SELECTION.Clear();
            GameObject[] gameObjects = FindObjectsOfType<GameObject>();
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                if (showProgressBar)
                {
                    progress = 1.0f - (float)i / gameObjects.Length;
                    EditorUtility.DisplayProgressBar("Quick-select progress", "Gathering selection... please wait!", progress);
                }

                if (gameObjects[i].layer == LayerMask.NameToLayer(layerName))
                {
                    SELECTION.Add(gameObjects[i]);
                }
            }
            Selection.objects = SELECTION.ToArray();
            System.GC.Collect();
        }
    }
}

#endif

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com
