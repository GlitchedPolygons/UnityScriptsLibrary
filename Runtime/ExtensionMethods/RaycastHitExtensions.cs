using UnityEngine;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// <a href="https://docs.unity3d.com/ScriptReference/RaycastHit.html">RaycastHit</a> extension methods.
    /// </summary>
    public static class RaycastHitExtensions
    {
        /// <summary>
        /// An empty (default) <a href="https://docs.unity3d.com/ScriptReference/RaycastHit.html">RaycastHit</a> dummy object.
        /// </summary>
        private static readonly RaycastHit EMPTY_RAYCASTHIT = new RaycastHit();

        /// <summary>
        /// Sort this <a href="https://docs.unity3d.com/ScriptReference/RaycastHit.html">RaycastHit</a>[] array by <a href="https://docs.unity3d.com/ScriptReference/RaycastHit-distance.html">RaycastHit.distance</a>.<para> </para>
        ///
        /// Also supports arrays from the <a href="https://docs.unity3d.com/ScriptReference/Physics.RaycastNonAlloc.html">Physics.RaycastNonAlloc</a> methods (the ones with some empty <a href="https://docs.unity3d.com/ScriptReference/RaycastHit.html">RaycastHit</a> variables in them).
        /// </summary>
        /// <param name="hits">The <a href="https://docs.unity3d.com/ScriptReference/RaycastHit.html">RaycastHit</a>s array to sort.</param>
        /// <param name="raycastNonAlloc">Does this array come from a RaycastNonAlloc method?</param>
        public static void SortByDistance(this RaycastHit[] hits, bool raycastNonAlloc)
        {
            int hitsLength = hits.Length;

            for (int i = 0; i < hitsLength; i++)
            {
                for (int ii = i + 1; ii < hitsLength; ii++)
                {
                    if (hits[i].distance > hits[ii].distance)
                    {
                        (hits[ii], hits[i]) = (hits[i], hits[ii]);
                    }
                }
            }

            if (!raycastNonAlloc)
            {
                return;
            }

            int _i = 0;
            for (int i = 0; i < hitsLength; i++)
            {
                if (hits[i].distance > 0.0f)
                {
                    if (i == 0)
                    {
                        return;
                    }

                    hits[_i] = hits[i];
                    hits[i] = EMPTY_RAYCASTHIT;
                    _i++;
                    i = _i;
                }
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com