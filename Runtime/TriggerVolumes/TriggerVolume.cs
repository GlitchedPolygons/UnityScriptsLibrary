using System;
using UnityEngine;
using UnityEngine.Events;
using GlitchedPolygons.Utilities;
using GlitchedPolygons.Identification;
using GlitchedPolygons.ExtensionMethods;

namespace GlitchedPolygons.TriggerVolumes
{
    /// <summary>
    /// Represents a trigger volume that fires various events when something has entered/escaped its <a href="https://docs.unity3d.com/ScriptReference/BoxCollider.html">UnityEngine.BoxCollider</a> volume.<para> </para>
    /// You can decide what happens when specific objects enter this trigger volume by hooking up the events inside the <see cref="TriggerVolume"/>'s inspector.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(ID))]
    [RequireComponent(typeof(Collider))]
#if UNITY_EDITOR
    [RequireComponent(typeof(ColliderGizmo))]
#endif
    public class TriggerVolume : MonoBehaviour
    {
        /// <summary>
        /// Delay in seconds (e.g. the player enters the trigger volume and the OnTriggerEnter event is fired in [delay] seconds).
        /// </summary>
        [SerializeField]
        [Tooltip("Delay in seconds (e.g. the player enters the trigger volume and the OnTriggerEnter event is fired in <delay> seconds)")]
        protected float delaySeconds = 0.0f;

        /// <summary>
        /// Limits how many times this trigger is allowed to fire its events. A value of one would allow this trigger to be fired only once in its lifetime; zero means no limits at all (infinite trigger firing).
        /// </summary>
        [SerializeField]
        [Tooltip("Limits how many times this trigger is allowed to fire its events. A value of one would allow this trigger to be fired only once in its lifetime; zero means no limits at all (infinite trigger firing).")]
        protected int maxEvents = 0;

        /// <summary>
        /// Only intruders that pass this <a href="https://docs.unity3d.com/Documentation/ScriptReference/LayerMask.html">LayerMask</a> filter will be able to fire the OnTriggerEnter/Exit events.
        /// </summary>
        [SerializeField]
        [Header("Filters")]
        [Tooltip("Define which layers can cause trigger volume events.")]
        protected LayerMask layerMask = ~0;

        /// <summary>
        /// You can specify a list of tags of which each intruder has to have at least one in order to pass the filter.<para> </para>
        /// An empty array disables the tag filter (meaning that any tag is ok).
        /// </summary>
        [SerializeField]
        [Tooltip("Only colliders whose GameObject tag is on this list will fire OnTriggerEnter/Exit events. If the array is empty, all tags are accepted.")]
        protected string[] tags = { };

        /// <summary>
        /// You can filter specific <a href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</a>s to be the only ones feasible of raising the OnTriggerEnter/Exit events (after filtering by layer and tag).
        /// </summary>
        [SerializeField]
        [Tooltip("You can filter specific colliders to be the only ones feasible of raising the OnTriggerEnter/Exit events (after filtering by layer and tag).")]
        protected Collider[] customFilter;

        [SerializeField]
        private UnityEvent onTriggerEnter;

        [SerializeField]
        private UnityEvent onTriggerExit;

        /// <summary>
        /// The total amount of enter/exit events this <see cref="TriggerVolume"/> has raised so far.
        /// </summary>
        protected int firedEvents = 0;

        /// <summary>
        /// How many events this <see cref="TriggerVolume"/> already invoked.
        /// </summary>
        public int GetFiredEventsCount() => firedEvents;

        /// <summary>
        /// Sets <see cref="TriggerVolume.firedEvents"/>. Useful when loading from a savegame.
        /// </summary>
        /// <param name="firedEvents">The new amount of times the trigger has fired.</param>
        public void SetFiredEventsCount(int firedEvents) => this.firedEvents = firedEvents;

        public event Action<Collider> EnteredTrigger;

        public event Action<Collider> ExitedTrigger;

        protected ID id;
        /// <summary>
        /// Gets the trigger volume's identifier.
        /// </summary>
        /// <returns>The unique ID (int).</returns>
        public int GetID() => id.GetID();
        
        #region #if UNITY_EDITOR

#if UNITY_EDITOR

        private BoxCollider boxCollider;
        private SphereCollider sphereCollider;
        private static readonly Color TRIGGER_GIZMO_COLOR = new Color(1f, 0.5f, 0.0f, 0.3f);

        [UnityEditor.MenuItem("Glitched Polygons/Create/Trigger Volume/Box", false, 2)]
        private static void CreateBoxTriggerVolume()
        {
            int index = 0;
            beginning:
            Camera sceneViewCamera = UnityEditor.SceneView.lastActiveSceneView.camera;
            if (sceneViewCamera == null)
            {
                (UnityEditor.SceneView.sceneViews[index] as UnityEditor.EditorWindow)?.Focus();
                if (index < UnityEditor.SceneView.sceneViews.Count)
                {
                    index++;
                    goto beginning;
                }
            }

            if (sceneViewCamera == null)
            {
                return;
            }

            GameObject triggerObj = new GameObject("trigger-volume");
            triggerObj.transform.position = sceneViewCamera.transform.position + sceneViewCamera.transform.forward * 3.5f;

            BoxCollider collider = triggerObj.AddComponent<BoxCollider>();
            collider.isTrigger = true;

            triggerObj.AddComponent<TriggerVolume>();
            triggerObj.GetOrAddComponent<ColliderGizmo>().Refresh();

            UnityEditor.Selection.activeGameObject = triggerObj;
        }

        [UnityEditor.MenuItem("Glitched Polygons/Create/Trigger Volume/Sphere", false, 2)]
        private static void CreateSphereTriggerVolume()
        {
            int index = 0;
            beginning:
            Camera sceneViewCamera = UnityEditor.SceneView.lastActiveSceneView.camera;
            if (sceneViewCamera == null)
            {
                (UnityEditor.SceneView.sceneViews[index] as UnityEditor.EditorWindow)?.Focus();
                if (index < UnityEditor.SceneView.sceneViews.Count)
                {
                    index++;
                    goto beginning;
                }
            }

            if (sceneViewCamera == null)
            {
                return;
            }

            GameObject triggerObj = new GameObject("trigger-volume");
            triggerObj.transform.position = sceneViewCamera.transform.position + sceneViewCamera.transform.forward * 3.5f;

            SphereCollider collider = triggerObj.AddComponent<SphereCollider>();
            collider.isTrigger = true;

            triggerObj.AddComponent<TriggerVolume>();
            triggerObj.GetOrAddComponent<ColliderGizmo>().Refresh();

            UnityEditor.Selection.activeGameObject = triggerObj;
        }

        private void Awake()
        {
            id = GetComponent<ID>();

            if (GetComponents<TriggerVolume>().Length > 1)
            {
                UnityEditor.EditorUtility.DisplayDialog("ERROR!", $"There is already a {nameof(TriggerVolume)} attached to this GameObject. There may only be one per GameObject!", "K, sorry :(");
                DestroyImmediate(this);
            }
        }

#endif

        #endregion

        private void OnEnable()
        {
            if (id == null)
                id = GetComponent<ID>();
        }

        /// <summary>
        /// Invokes the <see cref="onTriggerEnter"/> event and increments <see cref="firedEvents"/> by 1.
        /// </summary>
        private void InvokeOnTriggerEnter()
        {
            onTriggerEnter?.Invoke();
            firedEvents++;
        }

        /// <summary>
        /// Invokes the <see cref="onTriggerExit"/> event and increments <see cref="firedEvents"/> by 1.
        /// </summary>
        private void InvokeOnTriggerExit()
        {
            onTriggerExit?.Invoke();
            firedEvents++;
        }

        /// <summary>
        /// Verifies that a <a href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</a> is a valid intruder based on the filters set up on this <see cref="TriggerVolume"/>.<para> </para>
        /// Only valid intruders may raise the <see cref="onTriggerEnter"/> and <see cref="onTriggerExit"/> events!
        /// </summary>
        /// <param name="intruder">The <a href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</a> to check.</param>
        /// <returns>Whether the intruder <a href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</a> passed the specified filters or not. True means that the <a href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</a> is feasible of raising the <see cref="onTriggerEnter"/> and <see cref="onTriggerExit"/> events.</returns>
        private bool IsValidIntruder(Collider intruder)
        {
            // First check if the maximum amount of event fires has been reached (if there is one).
            // If that's the case, immediately get rid of this trigger volume; we don't need it anymore.
            if (maxEvents > 0 && firedEvents >= maxEvents)
            {
                Destroy(gameObject);
                return false;
            }

            // Then the intruder's layer has to pass the filter.
            if (layerMask != ~0 && !layerMask.Contains(intruder.gameObject.layer))
            {
                if (layerMask == 0)
                {
                    Debug.LogWarning($"{nameof(TriggerVolume)}: The {nameof(LayerMask)} you specified in this {nameof(TriggerVolume)} is empty (equals to \"Nothing\" in the inspector)... No layer should pass the filter at all? Are you sure that this is what you want?");
                }
                return false;
            }

            // At this point if there is a tag filter set up, 
            // the intruder's tag has to pass it before continuing.
            if (tags.Length > 0)
            {
                bool hasTag = false;
                for (int i = tags.Length - 1; i >= 0; i--)
                {
                    if (intruder.CompareTag(tags[i]))
                    {
                        hasTag = true;
                        break;
                    }
                }

                if (!hasTag)
                {
                    return false;
                }
            }

            // Finally, check for any specific collider matches 
            // if there are any custom intruders defined in the filter.
            if (customFilter.Length > 0)
            {
                bool match = false;
                for (int i = customFilter.Length - 1; i >= 0; i--)
                {
                    if (intruder == customFilter[i])
                    {
                        match = true;
                        break;
                    }
                }

                if (!match)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This unity message is executed when something enters our trigger <a href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</a>.
        /// </summary>
        /// <param name="intruder">The <a href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</a> that entered our <see cref="TriggerVolume"/>.</param>
        private void OnTriggerEnter(Collider intruder)
        {
            if (onTriggerEnter == null)
            {
                return;
            }

            if (IsValidIntruder(intruder))
            {
                Invoke(nameof(InvokeOnTriggerEnter), Mathf.Abs(delaySeconds));
            }
        }

        /// <summary>
        /// Fires when a <a href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</a> exits the <see cref="TriggerVolume"/>
        /// </summary>
        /// <param name="intruder">The collider that escaped from the trigger volume.</param>
        private void OnTriggerExit(Collider intruder)
        {
            if (onTriggerExit == null)
            {
                return;
            }

            if (IsValidIntruder(intruder))
            {
                Invoke(nameof(InvokeOnTriggerExit), Mathf.Abs(delaySeconds));
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2018 | https://glitchedpolygons.com
