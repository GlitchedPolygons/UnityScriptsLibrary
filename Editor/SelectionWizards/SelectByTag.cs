#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Wizard for selecting all objects that have one specific tag.
    /// </summary>
    sealed class SelectByTag : ScriptableWizard
    {
        /// <summary>
        /// The tag to search.
        /// </summary>
        [SerializeField]
        private string searchTag = "< search tag >";

        [MenuItem("Glitched Polygons/Quick-select/Select by tag")]
        private static void SelectAllOfTagWizard()
        {
            DisplayWizard<SelectByTag>("Select all objects with a specific tag...", "Make selection");
        }

        private void OnWizardCreate()
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(searchTag);
            Selection.objects = gameObjects;
        }
    }
}
#endif

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com
