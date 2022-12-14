using UnityEngine;
using System.Collections.Generic;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Utility class that keeps cached instances of various <a href="https://docs.unity3d.com/ScriptReference/YieldInstruction.html">YieldInstruction</a>s
    /// (no need to create new ones in each coroutine).
    /// </summary>
    public static class YieldInstructions
    {
        /// <summary>
        /// Cached <a href="https://docs.unity3d.com/ScriptReference/WaitForSeconds.html">WaitForSeconds</a> instances.
        /// </summary>
        private static readonly Dictionary<int, WaitForSeconds> WAIT_FOR_SECONDS = new Dictionary<int, WaitForSeconds>(32);

        /// <summary>
        /// Cached <a href="https://docs.unity3d.com/ScriptReference/WaitForSecondsRealtime.html">WaitForSecondsRealtime</a> instances.
        /// </summary>
        private static readonly Dictionary<int, WaitForSecondsRealtime> WAIT_FOR_SECONDS_REALTIME = new Dictionary<int, WaitForSecondsRealtime>(32);

        /// <summary>
        /// Gets a cached <a href="https://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html">WaitForEndOfFrame</a> instance.
        /// </summary>
        public static WaitForEndOfFrame WaitForEndOfFrame { get; } = new WaitForEndOfFrame();

        /// <summary>
        /// Gets a cached <a href="https://docs.unity3d.com/ScriptReference/WaitForFixedUpdate.html">WaitForFixedUpdate</a> instance.
        /// </summary>
        public static WaitForFixedUpdate WaitForFixedUpdate { get; } = new WaitForFixedUpdate();

        /// <summary>
        /// Checks if the specified amount of milliseconds has already been waited for once.<para> </para>
        /// If that's the case, retrieve the corresponding <a href="https://docs.unity3d.com/ScriptReference/WaitForSeconds.html">WaitForSeconds</a> instance, 
        /// otherwise create a new one and cache it into the <see cref="WAIT_FOR_SECONDS"/> dictionary.
        /// </summary>
        /// <param name="milliseconds">The amount of MILLISECONDS (ms) to wait in the coroutine.</param>
        /// <returns>The cached <a href="https://docs.unity3d.com/ScriptReference/WaitForSeconds.html">WaitForSeconds</a> instance.</returns>
        public static WaitForSeconds GetWaitForSeconds(int milliseconds)
        {
            if (milliseconds < 0)
            {
                milliseconds *= -1;
            }

            if (!WAIT_FOR_SECONDS.ContainsKey(milliseconds))
            {
                WAIT_FOR_SECONDS.Add(milliseconds, new WaitForSeconds(milliseconds * 0.001f));
            }

            return WAIT_FOR_SECONDS[milliseconds];
        }

        /// <summary>
        /// Checks if the specified amount of milliseconds has already been waited for once.<para> </para>
        /// If that's the case, retrieve the corresponding <a href="https://docs.unity3d.com/ScriptReference/WaitForSecondsRealtime.html">WaitForSecondsRealtime</a> instance, 
        /// otherwise create a new one and cache it into the <see cref="WAIT_FOR_SECONDS_REALTIME"/> dictionary.
        /// </summary>
        /// <param name="milliseconds">The amount of MILLISECONDS (ms) to stop the coroutine.</param>
        /// <returns>The cached <a href="https://docs.unity3d.com/ScriptReference/WaitForSecondsRealtime.html">WaitForSecondsRealtime</a> instance.</returns>
        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(int milliseconds)
        {
            if (milliseconds < 0)
            {
                milliseconds *= -1;
            }

            if (!WAIT_FOR_SECONDS_REALTIME.ContainsKey(milliseconds))
            {
                WAIT_FOR_SECONDS_REALTIME.Add(milliseconds, new WaitForSecondsRealtime(milliseconds * 0.001f));
            }

            return WAIT_FOR_SECONDS_REALTIME[milliseconds];
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com
