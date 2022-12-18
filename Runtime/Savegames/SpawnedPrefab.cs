using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GlitchedPolygons.SavegameFramework
{
    /// <summary>
    /// This component should be on all saveable prefab objects (at root level, NOT on a nested child <c>GameObject</c>!) that
    /// weren't in the scene at edit-time (ergo spawned at runtime, like grenades, dropped items, etc...).<para> </para>
    ///
    /// It can have a linked <see cref="SaveableComponent"/> to store/reconstruct the important data
    /// (use <see cref="SpawnedPrefab.GetSaveableComponent"/> to access it).<para> </para>
    ///
    /// Make sure it doesn't appear in the scene at edit-time but only and EXCLUSIVELY AT RUNTIME (via instantiation!).<para> </para>
    ///
    /// The <see cref="SpawnedPrefab.resourcePath"/> is the local <a href="https://docs.unity3d.com/ScriptReference/Resources.html">Resources</a> path used to reconstruct (instantiate) this prefab object on load.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class SpawnedPrefab : MonoBehaviour
    {
        /// <summary>
        /// List of prefabs spawned at runtime that shall persist between game sessions.
        /// </summary>
        public static readonly List<SpawnedPrefab> SPAWNED_PREFABS = new(256);

        /// <summary>
        /// The <see cref="Resources"/> path used to reconstruct the prefab on load.
        /// </summary>
        [SerializeField]
        private string resourcePath = null;

        /// <summary>
        /// Get the <see cref="resourcePath"/>.
        /// </summary>
        /// <returns>The <see cref="Resources"/> path used to reconstruct the prefab on load.</returns>
        public string GetResourcePath() => resourcePath;

        /// <summary>
        /// The linked <see cref="SaveableComponent"/>.<para> </para>
        ///
        /// This is optional, but nice to have if you want to not only respawn your saved prefabs on load but also restore any data on them.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private SaveableComponent saveableComponent = null;

        /// <summary>
        /// Gets the linked <see cref="saveableComponent"/>.
        /// </summary>
        /// <returns>Optional <see cref="SaveableComponent"/> that can contain prefab data that you want to persist between game sessions.</returns>
        public SaveableComponent GetSaveableComponent() => saveableComponent;

        [ContextMenu("Get linked SaveableComponent")]
        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Perform some safety checks...

                if (GetComponent<SavegameManager>() != null)
                {
                    UnityEditor.EditorUtility.DisplayDialog("ERROR!", "There is already a SavegameManager component attached to this GameObject. There may only be one or the other on a single GameObject!", "gosh darn");
                    DestroyImmediate(this);
                    return;
                }

                if (GetComponents<SpawnedPrefab>().Length > 1)
                {
                    UnityEditor.EditorUtility.DisplayDialog("ERROR!", "There is already one of these components attached to this GameObject. There may only be one SpawnedPrefab component per GameObject!", "K, sorry :(");
                    DestroyImmediate(this);
                    return;
                }
            }
#endif
            if (saveableComponent == null)
            {
                // Automatically assign the saveable  
                // component reference for maximum comfort.
                saveableComponent = GetComponent<SaveableComponent>();
            }
        }

        private void OnEnable()
        {
            SPAWNED_PREFABS.Add(this);
        }

        private void OnDisable()
        {
            SPAWNED_PREFABS.Remove(this);
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com