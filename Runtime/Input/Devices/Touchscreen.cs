using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GlitchedPolygons.ExtensionMethods;

namespace GlitchedPolygons.Input
{
    /// <summary>
    /// A touchscreen as you know it from smartphones and tablets.
    /// </summary>
    public class Touchscreen : InputDevice
    {
        [Serializable]
        private struct UIButton
        {
            [Tooltip("The game button index.")]
            public int index;
            [Tooltip("The associated UI button element that when clicked or tapped on will trigger the event.")]
            public Button button;
        }

        /// <inheritdoc/>
        public override event Action<int> ButtonPressed;
        /// <inheritdoc/>
        public override event Action<int> ButtonReleased;

        [SerializeField]
        private float[] axes = new[] { 0.0f, 0.0f, 0.0f, 0.0f };

        [SerializeField]
        private UIButton[] buttons;

        private List<EventTrigger.Entry>[] entries = null;

        private void Awake()
        {
            entries = new List<EventTrigger.Entry>[buttons.Length];
            for (int i = entries.Length - 1; i >= 0; i--)
            {
                entries[i] = new List<EventTrigger.Entry>(2);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            for (int i = buttons.Length - 1; i >= 0; i--)
            {
                var button = buttons[i];
                var eventTrigger = button.button.GetOrAddComponent<EventTrigger>();

                int indexClosure = i;
                entries[i].Add(eventTrigger.AddEventTriggerListener(EventTriggerType.PointerDown, data => { ButtonPressed?.Invoke(indexClosure); }));
                entries[i].Add(eventTrigger.AddEventTriggerListener(EventTriggerType.PointerUp, data => { ButtonReleased?.Invoke(indexClosure); }));
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            for (int i = buttons.Length - 1; i >= 0; i--)
            {
                var button = buttons[i];
                if (button.button == null)
                {
                    continue;
                }

                var eventTrigger = button.button.GetOrAddComponent<EventTrigger>();
                if (eventTrigger == null)
                {
                    continue;
                }

                for (int ii = entries[i].Count - 1; ii >= 0; ii--)
                {
                    eventTrigger.triggers.Remove(entries[i][ii]);
                }

                entries[i].Clear();
            }
        }

        /// <inheritdoc/>
        public override float GetAxis(int axisIndex)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            return axes[axisIndex];
        }

        /// <summary>
        /// Sets an axis value directly.<para> </para>
        /// WARNING: This is not clamped! You have to clamp the axis value manually if you want range restrictions!
        /// </summary>
        /// <param name="axisIndex">Axis to set.</param>
        /// <param name="value">The value.</param>
        public void SetAxis(int axisIndex, float value)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            axes[axisIndex] = value;
        }

        /// <summary>
        /// Sets an axis value directly after being clamped between clampRange.x and clampRange.y
        /// </summary>
        /// <param name="axisIndex">Axis to set.</param>
        /// <param name="value">The value.</param>
        /// <param name="clampRange">The allowed value range (e.g. [-1;1] )</param>
        public void SetAxis(int axisIndex, float value, Vector2 clampRange)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            axes[axisIndex] = Mathf.Clamp(value, clampRange.x, clampRange.y);
        }

        /// <summary>
        /// Immediately invokes the <see cref="ButtonPressed"/> event (UI).
        /// </summary>
        /// <param name="button">The button to press.</param>
        public void PressButton(int button)
        {
            ButtonPressed?.Invoke(button);
        }

        /// <summary>
        /// Immediately invokes the <see cref="ButtonReleased"/> event (UI).
        /// </summary>
        /// <param name="button">The button that's been released.</param>
        public void ReleaseButton(int button)
        {
            ButtonReleased?.Invoke(button);
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
