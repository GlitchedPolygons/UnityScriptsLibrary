using UnityEngine;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// This class holds the GetOrAddComponent extension method variants.
    /// </summary>
    public static class GetOrAddComponentExtension
    {
        /// <summary>
        /// Tries to get the component on this <a href="https://docs.unity3d.com/Documentation/ScriptReference/GameObject.html">GameObject</a> and adds one if there wasn't one attached already.
        /// </summary>
        /// <typeparam name="T">The type of <a href="https://docs.unity3d.com/Documentation/ScriptReference/Component.html">Component</a> to get or add.</typeparam>
        /// <param name="gameObject">The <a href="https://docs.unity3d.com/Documentation/ScriptReference/GameObject.html">GameObject</a> whose component you want to get or add.</param>
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
        /// Tries to get the component on this <a href="https://docs.unity3d.com/Documentation/ScriptReference/Component.html">Component</a> and adds one if there wasn't one attached already.
        /// </summary>
        /// <typeparam name="T">The type of <a href="https://docs.unity3d.com/Documentation/ScriptReference/Component.html">Component</a> to get or add.</typeparam>
        /// <param name="component">The <a href="https://docs.unity3d.com/Documentation/ScriptReference/Component.html">Component</a> whose component you want to get or add.</param>
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
