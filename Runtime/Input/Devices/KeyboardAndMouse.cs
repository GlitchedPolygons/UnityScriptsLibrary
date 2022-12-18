using System;
using UnityEngine;

namespace GlitchedPolygons.Input
{
    using Input = UnityEngine.Input;

    /// <summary>
    /// Good old keyboard and mouse.<para> </para>
    ///
    /// This <see cref="InputDevice"/> you will see primarily on standalone PC, Mac &amp; Linux.
    /// </summary>
    public class KeyboardAndMouse : InputDevice
    {
        /// <summary>
        /// A game button mapped to one or more keyboard keys.
        /// </summary>
        [Serializable]
        private struct Button
        {
            public KeyCode[] keyCodes;
        }

        /// <summary>
        /// Keyboard and mouse combo axis.<para> </para>
        ///
        /// If the Unity axis string is null or empty,
        /// the positive and negative keys will be used.<para> </para>
        ///
        /// Otherwise <c>Input.GetAxis(string)</c> will be used.
        /// </summary>
        [Serializable]
        public struct Axis
        {
            public string unityAxis;
            public KeyCode negativeKey;
            public KeyCode positiveKey;
            public float valueShiftSpeed;
            public float value;
        }

        /// <inheritdoc/>
        public override event Action<int> ButtonPressed;

        /// <inheritdoc/>
        public override event Action<int> ButtonReleased;

        /// <summary>
        /// The mapped game axes.
        /// </summary>
        [SerializeField]
        private Axis[] axes =
        {
            new Axis { negativeKey = KeyCode.A, positiveKey = KeyCode.D, valueShiftSpeed = 6.0f },
            new Axis { negativeKey = KeyCode.S, positiveKey = KeyCode.W, valueShiftSpeed = 6.0f },
            new Axis { unityAxis = "Mouse X", negativeKey = KeyCode.None, positiveKey = KeyCode.None, valueShiftSpeed = -1.0f },
            new Axis { unityAxis = "Mouse Y", negativeKey = KeyCode.None, positiveKey = KeyCode.None, valueShiftSpeed = -1.0f },
        };

        /// <summary>
        /// The game buttons and how they are mapped to the keyboard.
        /// </summary>
        [SerializeField]
        private Button[] buttons =
        {
            new Button { keyCodes = new[] { KeyCode.Space } },
            new Button { keyCodes = new[] { KeyCode.C, KeyCode.LeftCommand, KeyCode.LeftControl } },
            new Button { keyCodes = new[] { KeyCode.LeftShift, KeyCode.RightShift } },
        };

#if UNITY_EDITOR
        [SerializeField]
        private bool debugAxis = false;

        [SerializeField]
        private int debugAxisIndex = -1;
#endif

        /// <inheritdoc/>
        public override float GetAxis(int axisIndex)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            return axes[axisIndex].value;
        }

        /// <summary>
        /// Returns a button's currently assigned <xref href="KeyCode?alt=KeyCode"/>s.
        /// </summary>
        /// <param name="buttonIndex">The button whose mapped <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a>s you want to retrieve.</param>
        /// <returns>The button's assigned <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a>s.</returns>
        public KeyCode[] GetButtonKeys(int buttonIndex)
        {
            if (buttonIndex < 0 || buttonIndex >= buttons.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(buttonIndex));
            }

            return buttons[buttonIndex].keyCodes;
        }

        /// <summary>
        /// Reassign a button's <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a>s.
        /// </summary>
        /// <param name="buttonIndex">The button to modify.</param>
        /// <param name="newKeys">The new <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a>s to remap the button to.</param>
        public void ReassignButton(int buttonIndex, KeyCode[] newKeys)
        {
            if (buttonIndex < 0 || buttonIndex >= buttons.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(buttonIndex));
            }

            buttons[buttonIndex].keyCodes = newKeys;
        }

        /// <summary>
        /// Returns a copy of the specified axis mapping.
        /// </summary>
        /// <param name="axisIndex">The axis whose associated config you want to retrieve.</param>
        /// <returns>The <see cref="Axis"/> association.</returns>
        public Axis GetAxisAssociation(int axisIndex)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            return axes[axisIndex];
        }

        /// <summary>
        /// Reassign an axis.
        /// </summary>
        /// <param name="axisIndex">The axis to modify.</param>
        /// <param name="newAxisAssociation">The new axis mapping.</param>
        public void ReassignAxis(int axisIndex, Axis newAxisAssociation)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            axes[axisIndex] = newAxisAssociation;
        }

        /// <summary>
        /// Reassign <see cref="Axis.unityAxis"/>.
        /// </summary>
        /// <param name="axisIndex">The axis to modify.</param>
        /// <param name="newAxisAssociation">The new unity axis name (e.g. "Mouse X").</param>
        public void ReassignAxis(int axisIndex, string newAxisAssociation)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            axes[axisIndex].unityAxis = newAxisAssociation;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (debugAxis)
            {
                if (debugAxisIndex < 0) debugAxisIndex = 0;
                if (debugAxisIndex >= axes.Length) debugAxisIndex = axes.Length - 1;
                print($"Axis '{axes[debugAxisIndex]}' = {GetAxis(debugAxisIndex)}");
            }
#endif
            for (int i = buttons.Length - 1; i >= 0; i--)
            {
                CheckGameButton(i, buttons[i].keyCodes);
            }

            for (int i = axes.Length - 1; i >= 0; i--)
            {
                var axis = axes[i];
                if (string.IsNullOrEmpty(axis.unityAxis))
                {
                    float delta = axis.valueShiftSpeed * Time.deltaTime;
                    if (Input.GetKey(axis.positiveKey) && !Input.GetKey(axis.negativeKey))
                        axis.value += delta;
                    else if (Input.GetKey(axis.negativeKey) && !Input.GetKey(axis.positiveKey))
                        axis.value -= delta;
                    else
                        axis.value = Mathf.MoveTowards(axis.value, 0.0f, delta);
                }
                else
                {
                    axis.value = Input.GetAxis(axis.unityAxis);
                }

                axis.value = Mathf.Clamp(axis.value, -1.0f, 1.0f);
                axes[i] = axis;
            }
        }

        /// <summary>
        /// Checks if a game button has been pressed or released and invokes the corresponding events.
        /// </summary>
        /// <param name="buttonIndex">The button to check.</param>
        /// <param name="keyCode">The <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a> mapped to the button.</param>
        private void CheckGameButton(int buttonIndex, KeyCode keyCode)
        {
            if (IsMouseButton(keyCode))
            {
                if (Input.GetMouseButtonDown(GetMouseKeyIndex(keyCode)))
                {
                    ButtonPressed?.Invoke(buttonIndex);
                }

                if (Input.GetMouseButtonUp(GetMouseKeyIndex(keyCode)))
                {
                    ButtonReleased?.Invoke(buttonIndex);
                }
            }
            else
            {
                if (Input.GetKeyDown(keyCode))
                {
                    ButtonPressed?.Invoke(buttonIndex);
                }

                if (Input.GetKeyUp(keyCode))
                {
                    ButtonReleased?.Invoke(buttonIndex);
                }
            }
        }

        /// <summary>
        /// Checks if a button has been pressed or released and invokes the corresponding events.
        /// </summary>
        /// <param name="buttonIndex">The button to check.</param>
        /// <param name="keyCodes">The <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a>s mapped to the button.</param>
        private void CheckGameButton(int buttonIndex, KeyCode[] keyCodes)
        {
            for (int i = keyCodes.Length - 1; i >= 0; i--)
            {
                CheckGameButton(buttonIndex, keyCodes[i]);
            }
        }

        /// <summary>
        /// Is the specified <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a> a mouse button?
        /// </summary>
        /// <param name="keyCode">The <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a> to test against being a mouse button or keyboard key.</param>
        /// <returns>Whether the passed <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a> was a mouse button or not.</returns>
        private bool IsMouseButton(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Mouse0:
                case KeyCode.Mouse1:
                case KeyCode.Mouse2:
                case KeyCode.Mouse3:
                case KeyCode.Mouse4:
                case KeyCode.Mouse5:
                case KeyCode.Mouse6:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the numeric index of a mouse button's <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a>.<para> </para>
        ///
        /// E.g. <see cref="KeyCode.Mouse0"/> = 0, <see cref="KeyCode.Mouse1"/> = 1, etc...
        /// </summary>
        /// <param name="keyCode">The mouse button's <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a>.</param>
        /// <returns>The numeric index of a mouse button's <a href="https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</a>.</returns>
        private int GetMouseKeyIndex(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Mouse0:
                    return 0;
                case KeyCode.Mouse1:
                    return 1;
                case KeyCode.Mouse2:
                    return 2;
                case KeyCode.Mouse3:
                    return 3;
                case KeyCode.Mouse4:
                    return 4;
                case KeyCode.Mouse5:
                    return 5;
                case KeyCode.Mouse6:
                    return 6;
            }

            return -1;
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com