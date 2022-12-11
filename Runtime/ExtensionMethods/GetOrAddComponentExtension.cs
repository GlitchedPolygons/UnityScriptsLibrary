using UnityEngine;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// This class holds the GetOrAddComponent extension method variants.
    /// </summary>
    public static class GetOrAddComponentExtension
    {
        /// <summary>
        /// Tries to get the component on this <see cref="GameObject"/> and adds one if there wasn't one attached already.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Component"/> to get or add.</typeparam>
        /// <param name="gameObject">The <see cref="GameObject"/> whose component you want to get or add.</param>
        /// <returns>The component.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T result = gameObject.GetComponent<T>();
            if (result == null)
            {
                result = gameObject.AddComponent<T>();
            }

            return result;
        }

        /// <summary>
        /// Tries to get the component on this <see cref="Component"/> and adds one if there wasn't one attached already.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Component"/> to get or add.</typeparam>
        /// <param name="component">The <see cref="Component"/> whose component you want to get or add.</param>
        /// <returns>The component.</returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            T result = component.GetComponent<T>();
            if (result == null)
            {
                result = component.gameObject.AddComponent<T>();
            }

            return result;
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
