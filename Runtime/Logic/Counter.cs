using System;
using UnityEngine;
using UnityEngine.Events;
using GlitchedPolygons.Identification;

namespace GlitchedPolygons.Logic
{
    /// <summary>
    /// An integer based mathematical counter.
    /// You can add, subtract, multiply and divide
    /// with it and make it do things when it reaches certain values.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(ID))]
    public class Counter : MonoBehaviour
    {
        /// <summary>
        /// Raised when the <see cref="count"/> reaches the <see cref="targetCount"/>.
        /// </summary>
        public event Action ReachedTargetCount;

        /// <summary>
        /// The current count.
        /// </summary>
        [SerializeField]
        [Tooltip("The current count.")]
        private int count = 0;

        /// <summary>
        /// Once the <see cref="Counter"/> reaches this value through the various operations (<see cref="Add"/>, <see cref="Multiply"/>, ...) the <see cref="onReachedTargetCount"/> event is raised.
        /// </summary>
        [SerializeField]
        [Tooltip("Once the counter reaches this number by adding, subtracting, multiplying or dividing the reached target count event is triggered.")]
        private int targetCount = 0;

        /// <summary>
        /// <a href="https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html">UnityEvent</a> for inspector event hookups that are invoked when the <see cref="count"/> reaches <see cref="targetCount"/>.
        /// </summary>
        [SerializeField]
        private UnityEvent onReachedTargetCount;

        protected ID id;
        /// <summary>
        /// Gets the counter's associated identifier (<see cref="ID"/>).
        /// </summary>
        /// <returns>The counter's unique ID.</returns>
        public int GetID() => id.GetID();

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Glitched Polygons/Create/Logic/Counter", false, 2)]
        private static void CreateCounter()
        {
            Camera sceneViewCamera = UnityEditor.SceneView.lastActiveSceneView.camera;
            if (sceneViewCamera == null)
            {
                Debug.LogError($"{nameof(Relay)}: No active scene view detected. Focus the scene view into which you want to instantiate the {nameof(Relay)} and then try again!");
                return;
            }

            GameObject counter = new GameObject("counter")
            {
                isStatic = true,
                layer = LayerMask.NameToLayer("Ignore Raycast")
            };
            counter.transform.position = sceneViewCamera.transform.position + sceneViewCamera.transform.forward * 3.5f;
            counter.AddComponent<Counter>();

            UnityEditor.Selection.activeGameObject = counter;
        }
#endif

        private void Awake()
        {
            id = GetComponent<ID>();
        }

        /// <summary>
        /// Checks if the <see cref="targetCount"/> has been reached yet and invokes the <see cref="onReachedTargetCount"/> event if so.
        /// </summary>
        private void CheckCount()
        {
            if (count != targetCount)
            {
                return;
            }

            ReachedTargetCount?.Invoke();
            onReachedTargetCount?.Invoke();
        }

        /// <summary>
        /// Add to the counter.
        /// </summary>
        /// <param name="addendum">Add this amount to the count.</param>
        public void Add(int addendum)
        {
            count += addendum;
            CheckCount();
        }

        /// <summary>
        /// Divides the <see cref="count"/> by an integer divisor (remainders are ignored).
        /// </summary>
        /// <param name="divisor">Divide by this.</param>
        public void Divide(int divisor)
        {
            count /= divisor;
            CheckCount();
        }

        /// <summary>
        /// Multiplies <see cref="count"/> by an integer factor.
        /// </summary>
        /// <param name="factor">The factor.</param>
        public void Multiply(int factor)
        {
            count *= factor;
            CheckCount();
        }

        /// <summary>
        /// Gets this counter's current value.
        /// </summary>
        /// <returns>The current count.</returns>
        public int GetCount() => count;

        /// <summary>
        /// Sets the current count value.
        /// </summary>
        /// <param name="newCount">The new count.</param>
        /// <param name="stealthy">Stealthily setting the count won't trigger the counter events (even if the target value was reached).</param>
        public void SetCount(int newCount, bool stealthy = false)
        {
            count = newCount;

            if (!stealthy)
            {
                CheckCount();
            }
        }

        /// <summary>
        /// Gets the current target count.
        /// </summary>
        /// <returns>The current target count.</returns>
        public int GetTargetCount() => targetCount;

        /// <summary>
        /// Sets the <see cref="Counter"/>'s target count value.
        /// </summary>
        /// <param name="newTargetCount">The new target count.</param>
        /// <param name="stealthy">Stealthily setting the target count won't trigger the counter events (even if the target value was reached).</param>
        public void SetTargetCount(int newTargetCount, bool stealthy = false)
        {
            targetCount = newTargetCount;

            if (!stealthy)
            {
                CheckCount();
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
