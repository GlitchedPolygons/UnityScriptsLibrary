using UnityEngine;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Static class that can hide/unhide the mouse cursor. A disengaged cursor should also be locked...
    /// </summary>
    public static class Cursor
    {
        /// <summary>
        /// Hides the mouse cursor.
        /// </summary>
        public static void Hide()
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Unhides the mouse cursor.
        /// </summary>
        public static void Show()
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
}

// Copyright (C) Raphael Beck, 2016 | https://glitchedpolygons.com
