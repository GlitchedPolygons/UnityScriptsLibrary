using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GlitchedPolygons.Logic
{
    /// <summary>
    /// Logic relay that can be fired with or without a delay.<para> </para>
    /// When fired, a relay will invoke all of its registered actions at once.<para> </para>
    /// You can decide what happens on fire by subscribing/unsubscribing
    /// listeners to the <see cref="Fired"/> event (via <see cref="AddListener"/> and <see cref="RemoveListener"/>) or
    /// by hooking the events up directly in the <see cref="Relay"/>'s inspector.
    /// </summary>
    public sealed class Relay : MonoBehaviour
    {
        /// <summary>
        /// This event is raised whenever the <see cref="Relay"/> has been fired with the <see cref="Fire"/> method (after an optional delay).
        /// </summary>
        public event Action Fired;

        /// <summary>
        /// Optional delay (in seconds) between the call to <see cref="Fire"/> and actual event invocation.
        /// </summary>
        [SerializeField]
        [Tooltip("How many seconds should pass between the call to Fire() and actual relay execution.")]
        private float delay = 0.0f;

        /// <summary>
        /// <see cref="UnityEvent"/> for inspector hookups.
        /// </summary>
        [SerializeField]
        private UnityEvent onFire;

        /// <summary>
        /// The list of all <see cref="Action"/>s registered to this <see cref="Relay"/>.
        /// </summary>
        private readonly List<Action> registeredActions = new List<Action>(10);

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Glitched Polygons/Create/Logic/Relay", false, 2)]
        private static void CreateRelay()
        {
            Camera sceneViewCamera = UnityEditor.SceneView.lastActiveSceneView.camera;
            if (sceneViewCamera == null)
            {
                Debug.LogError($"{nameof(Relay)}: No active scene view detected. Focus the scene view into which you want to instantiate the {nameof(Relay)} and then try again!");
                return;
            }

            GameObject relay = new GameObject("relay")
            {
                isStatic = true,
                layer = LayerMask.NameToLayer("Ignore Raycast")
            };
            relay.transform.position = sceneViewCamera.transform.position + sceneViewCamera.transform.forward * 3.5f;
            relay.AddComponent<Relay>();

            UnityEditor.Selection.activeGameObject = relay;
        }
#endif

        private void OnEnable()
        {
            delay = Mathf.Abs(delay);
        }

        private void OnDisable()
        {
            for (int i = registeredActions.Count - 1; i >= 0; i--)
            {
                var registeredAction = registeredActions[i];
                if (registeredAction != null)
                {
                    Fired -= registeredAction;
                }
            }
            registeredActions.Clear();
        }

        /// <summary>
        /// Invokes the <see cref="onFire"/> and <see cref="Fired"/> events.
        /// </summary>
        private void OnFire()
        {
            onFire?.Invoke();
            Fired?.Invoke();
        }

        /// <summary>
        /// Fires the relay, invoking all registered method calls.<para> </para>
        /// This waits for <see cref="delay"/> seconds and then calls <see cref="OnFire"/>, thus triggering the <see cref="Fired"/> and <see cref="onFire"/> events.
        /// </summary>
        public void Fire()
        {
            if (delay > 0.0f)
            {
                Invoke(nameof(OnFire), delay);
            }
            else
            {
                OnFire();
            }
        }

        /// <summary>
        /// Cancels all pending invocations on this <see cref="Relay"/>.
        /// </summary>
        public void CancelPendingInvocations()
        {
            CancelInvoke();
        }

        /// <summary>
        /// Gets this <see cref="Relay"/>'s current delay value.
        /// </summary>
        /// <returns>The delay in seconds between this <see cref="Relay"/>'s fire and the actual invocation of its registered method calls.</returns>
        public float GetDelay() => delay;

        /// <summary>
        /// Sets this <see cref="Relay"/>'s delay value.<para> </para>
        /// Pending invocations (triggered with the old delay value) won't be affected by this.
        /// </summary>
        /// <param name="newDelay">The new delay value in seconds.</param>
        public void SetDelay(float newDelay) => delay = newDelay;

        /// <summary>
        /// Gets the amount of registered listeners.
        /// </summary>
        /// <returns>The amount of currently registered listeners.</returns>
        public int GetListenersCount()
        {
            return registeredActions.Count;
        }

        /// <summary>
        /// Checks whether the specified <see cref="Action"/> is a registered listener or not.
        /// </summary>
        /// <param name="action">The action to check.</param>
        /// <returns>True if the <see cref="Relay"/> contains the specified listener; false if not.</returns>
        public bool ContainsListener(Action action)
        {
            if (ReferenceEquals(action, null))
            {
                throw new NullReferenceException($"{nameof(Relay)}: The specified action is null!");
            }

            return registeredActions.Contains(action);
        }

        /// <summary>
        /// Adds a listener to this <see cref="Relay"/> that will be invoked every time the <see cref="Fired"/> event is raised.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to add to the <see cref="Relay"/>.</param>
        /// <returns>Whether the <see cref="Action"/> could be added successfully to the <see cref="Relay"/> or not.</returns>
        public bool AddListener(Action action)
        {
            if (ReferenceEquals(action, null))
            {
                Debug.LogWarning($"{nameof(Relay)}: The specified action is null; couldn't add it!");
                return false;
            }

            Fired += action;
            registeredActions.Add(action);
            return true;
        }

        /// <summary>
        /// Removes a specific listener from this <see cref="Relay"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to remove from the <see cref="Relay"/>.</param>
        /// <returns>Whether the <see cref="Action"/> could be removed successfully from the <see cref="Relay"/> or not.</returns>
        public bool RemoveListener(Action action)
        {
            if (ReferenceEquals(action, null))
            {
                Debug.LogWarning($"{nameof(Relay)}: The specified action is null; nothing to remove!");
                return false;
            }

            if (!registeredActions.Contains(action))
            {
                Debug.LogWarning($"{nameof(Relay)}: The specified action is not subscribed as a listener to this relay; nothing to remove!");
                return false;
            }

            Fired -= action;
            registeredActions.Remove(action);
            return true;
        }

        /// <summary>
        /// Clears all listeners by unsubscribing them from the <see cref="Relay.Fired"/> event.
        /// </summary>
        public void ClearAllListeners()
        {
            OnDisable();
        }
    }
}

// Copyright (C) Raphael Beck, 2016 - 2018 | https://glitchedpolygons.com
