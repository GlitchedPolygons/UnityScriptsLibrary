using UnityEngine;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// Static class containing useful <a href="https://docs.unity3d.com/Documentation/ScriptReference/GameObject.html">GameObject</a> extension methods.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Disables this <a href="https://docs.unity3d.com/Documentation/ScriptReference/GameObject.html">GameObject</a>'s
        /// <a href="https://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">Renderer</a> component and all child
        /// <a href="https://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">Renderer</a>s. <para> </para> 
        /// </summary>
        /// <param name="gameObject">The <a href="https://docs.unity3d.com/Documentation/ScriptReference/GameObject.html">GameObject</a> whose child <a href="https://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">Renderer</a>s you want to disable.</param>
        /// <param name="includeInactiveRenderers">Should inactive child <a href="https://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">Renderer</a>s be considered?</param>
        public static void DisableAllChildRenderers(this GameObject gameObject, bool includeInactiveRenderers = false)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(includeInactiveRenderers);

            if (renderers == null || renderers.Length <= 0)
            {
                return;
            }

            for (int i = renderers.Length - 1; i >= 0; i--)
            {
                renderers[i].enabled = false;
            }
        }

        /// <summary>
        /// Enables this <a href="https://docs.unity3d.com/Documentation/ScriptReference/GameObject.html">GameObject</a>'s <a href="https://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">Renderer</a> component and all child <a href="https://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">Renderer</a>s. <para> </para> 
        /// </summary>
        /// <param name="gameObject">The <a href="https://docs.unity3d.com/Documentation/ScriptReference/GameObject.html">GameObject</a> whose child <a href="https://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">Renderer</a>s you want to enable.</param>
        /// <param name="includeInactiveRenderers">Should inactive child <a href="https://docs.unity3d.com/Documentation/ScriptReference/Renderer.html">Renderer</a>s be considered?</param>
        public static void EnableAllChildRenderers(this GameObject gameObject, bool includeInactiveRenderers = false)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(includeInactiveRenderers);

            if (renderers == null || renderers.Length <= 0)
            {
                return;
            }

            for (int i = renderers.Length - 1; i >= 0; i--)
            {
                renderers[i].enabled = true;
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2016 | https://glitchedpolygons.com
