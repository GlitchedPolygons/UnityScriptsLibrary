using System;
using UnityEngine;

namespace GlitchedPolygons.Input
{
    /// <summary>
    /// A centralized input manager that gathers input from multiple <see cref="InputDevice"/>s and mixes their button events and axes together.<para> </para>
    ///
    /// Useful if you want to accept input from e.g. an <see cref="XboxController"/> and <see cref="KeyboardAndMouse"/> combo simultaneously.
    /// </summary>
    public sealed class InputMixer : MonoBehaviour
    {
        /// <summary>
        /// This event is raised in the frame where the player presses a button.<para> </para>
        ///
        /// Which button was pressed is passed as int argument.
        /// </summary>
        public event Action<int> ButtonPressed;

        /// <summary>
        /// This event is invoked the frame when the user lets go of a button.<para> </para>
        ///
        /// Which button was released is passed as int argument.
        /// </summary>
        public event Action<int> ButtonReleased;

        /// <summary>
        /// This event is raised when a button has been pressed and held down for at least
        /// <see cref="InputDevice.LONG_BUTTON_PRESS_THRESHOLD"/> seconds.
        /// </summary>
        public event Action<int> ButtonPressedLong;

        [SerializeField]
        private InputDevice[] inputDevices;

#if UNITY_EDITOR
        [SerializeField]
        private bool debugButtons = false;

        [SerializeField]
        private bool debugAxis = false;

        [SerializeField]
        private int debugAxisIndex;
#endif

        /// <summary>
        /// Gets the current value of a game axis (UNCLAMPED).
        /// </summary>
        /// <param name="axisIndex">The axis to query (its integer index).</param>
        /// <returns>The current value of the game axis.</returns>
        public float GetAxis(int axisIndex)
        {
            float axis = 0.0f;
            for (int i = inputDevices.Length - 1; i >= 0; i--)
            {
                axis += inputDevices[i].GetAxis(axisIndex);
            }

            return axis;
        }

        /// <summary>
        /// Gets the current value of a game axis (CLAMPED).
        /// </summary>
        /// <param name="axisIndex">The axis to query (its integer index).</param>
        /// <param name="clampRange">Range within which the axis value will be clamped (e.g. [-1;1] makes sense for directional locomotion axes).</param>
        /// <returns>The current value of the game axis (usually this number is [-1;1] )</returns>
        public float GetAxis(int axisIndex, Vector2 clampRange)
        {
            return Mathf.Clamp(GetAxis(axisIndex), clampRange.x, clampRange.y);
        }

        private void OnEnable()
        {
            for (int i = inputDevices.Length - 1; i >= 0; i--)
            {
                var inputDevice = inputDevices[i];
                if (inputDevice == null)
                {
                    continue;
                }

                inputDevice.ButtonPressed += InputDevice_ButtonPressed;
                inputDevice.ButtonPressedLong += InputDevice_ButtonPressedLong;
                inputDevice.ButtonReleased += InputDevice_ButtonReleased;
            }
        }

        private void OnDisable()
        {
            for (int i = inputDevices.Length - 1; i >= 0; i--)
            {
                var inputDevice = inputDevices[i];
                if (inputDevice == null)
                {
                    continue;
                }

                inputDevice.ButtonPressed -= InputDevice_ButtonPressed;
                inputDevice.ButtonPressedLong -= InputDevice_ButtonPressedLong;
                inputDevice.ButtonReleased -= InputDevice_ButtonReleased;
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (debugAxis)
            {
                print($"InputManager Axis '{debugAxisIndex}' = {GetAxis(debugAxisIndex)}");
            }
        }
#endif

        private void InputDevice_ButtonPressed(int buttonIndex)
        {
#if UNITY_EDITOR
            if (debugButtons) print($"InputManager: Pressed Button {buttonIndex}");
#endif
            ButtonPressed?.Invoke(buttonIndex);
        }

        private void InputDevice_ButtonPressedLong(int buttonIndex)
        {
#if UNITY_EDITOR
            if (debugButtons) print($"InputManager: Long Pressed Button {buttonIndex}");
#endif
            ButtonPressedLong?.Invoke(buttonIndex);
        }

        private void InputDevice_ButtonReleased(int buttonIndex)
        {
#if UNITY_EDITOR
            if (debugButtons) print($"InputManager: Released Button {buttonIndex}");
#endif
            ButtonReleased?.Invoke(buttonIndex);
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com