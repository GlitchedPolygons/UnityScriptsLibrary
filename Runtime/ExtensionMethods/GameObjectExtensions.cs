using UnityEngine;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// Static class containing useful <see cref="GameObject"/> extension methods.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Disables this <see cref="GameObject"/>'s <see cref="Renderer"/> component and all child <see cref="Renderer"/>s. <para> </para> 
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> whose child <see cref="Renderer"/>s you want to disable.</param>
        /// <param name="includeInactiveRenderers">Should inactive child <see cref="Renderer"/>s be considered?</param>
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
        /// Enables this <see cref="GameObject"/>'s <see cref="Renderer"/> component and all child <see cref="Renderer"/>s. <para> </para> 
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> whose child <see cref="Renderer"/>s you want to enable.</param>
        /// <param name="includeInactiveRenderers">Should inactive child <see cref="Renderer"/>s be considered?</param>
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
