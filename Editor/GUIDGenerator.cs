using System;
using UnityEditor;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Editor utility for generating a fresh <see cref="System.Guid"/> and copying its <c>string</c> content to the system clipboard.
    /// </summary>
    public static class GUIDGenerator
    {
        /// <summary>
        /// Generate a new <see cref="System.Guid"/> and copy its string to the clipboard.
        /// </summary>
        [MenuItem("Glitched Polygons/Copy a fresh, new GUID to the clipboard")]
        private static void GenerateGUID()
        {
            var guid = Guid.NewGuid().ToString();
            EditorGUIUtility.systemCopyBuffer = guid;
            EditorUtility.DisplayDialog("New GUID copied to clipboard; ready to paste!", "A fresh System.Guid has been copied to the clipboard:\n\n" + guid, "KTHXBYE");
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com
