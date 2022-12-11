using UnityEngine;
using UnityEngine.Events;

namespace GlitchedPolygons
{
    /// <summary>
    /// Simple component that exposes the <c>MonoBehaviour.Start</c>, <c>MonoBehaviour.Awake</c> and <c>MonoBehaviour.OnEnable</c> messages to the inspector.<para> </para>
    /// Useful if you want to trigger basic stuff happening in the scene on load without writing any code (just hook up stuff in the inspector <a href="https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html">UnityEvent</a>).
    /// </summary>
    public class Lifetime : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onAwake;

        [SerializeField]
        private UnityEvent onStart;

        [SerializeField]
        private UnityEvent onEnable;

        [SerializeField]
        private UnityEvent onDisable;

        [SerializeField]
        private UnityEvent onDestroy;

        private void Awake()
        {
            onAwake?.Invoke();
        }

        private void Start()
        {
            onStart?.Invoke();
        }

        private void OnEnable()
        {
            onEnable?.Invoke();
        }

        private void OnDisable()
        {
            onDisable?.Invoke();
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }
    }
}

// Copyright (C) Raphael Beck, 2016 | https://glitchedpolygons.com
