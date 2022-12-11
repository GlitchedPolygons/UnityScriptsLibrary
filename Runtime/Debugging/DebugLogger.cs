using UnityEngine;

namespace GlitchedPolygons.Debugging
{
    /// <summary>
    /// Debugging component useful for calling Debug.Log &amp; Co. from UnityEvent/UI event hookups in game.
    /// </summary>
    public class DebugLogger : MonoBehaviour
    {
        /// <summary>
        /// Calls <a href="https://docs.unity3d.com/ScriptReference/Debug.Log.html">Debug.Log</a>.
        /// </summary>
        /// <param name="message">The message to print to the console.</param>
        public void Log(string message)
        {
            Debug.Log(message);
        }

        /// <summary>
        /// Calls <a href="https://docs.unity3d.com/ScriptReference/Debug.LogWarning.html">Debug.LogWarning</a>.
        /// </summary>
        /// <param name="message">The warning message to print to the console.</param>
        public void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// Calls <a href="https://docs.unity3d.com/ScriptReference/Debug.LogError.html">Debug.LogError</a>.
        /// </summary>
        /// <param name="message">The error message to print to the console.</param>
        public void LogError(string message)
        {
            Debug.LogError(message);
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
