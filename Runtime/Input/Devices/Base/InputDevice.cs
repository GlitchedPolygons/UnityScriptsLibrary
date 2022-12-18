using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlitchedPolygons.Input
{
    /// <summary>
    /// Abstract input device class.<para> </para>
    ///
    /// To add new devices (e.g. xbox controller, steam controller, etc...),
    /// derive from this class and implement everything carefully.<para> </para>
    ///
    /// If you want to use the <c>MonoBehaviour.OnEnable</c> or <c>MonoBehaviour.OnDisable</c> messages, 
    /// please don't forget to override with a call to <c>base.OnEnable();</c> 
    /// or respectively <c>base.OnDisable();</c>.
    /// </summary>
    public abstract class InputDevice : MonoBehaviour
    {
        /// <summary>
        /// This event is raised in the frame where the player presses a button.<para> </para>
        ///
        /// Which button was pressed is passed as int argument.
        /// </summary>
        public abstract event Action<int> ButtonPressed;

        /// <summary>
        /// This event is invoked the frame when the user lets go of a button.<para> </para>
        ///
        /// Which button was released is passed as int argument.
        /// </summary>
        public abstract event Action<int> ButtonReleased;

        /// <summary>
        /// This event is raised when a button has been pressed and held down for at least
        /// <see cref="LONG_BUTTON_PRESS_THRESHOLD"/> seconds.
        /// </summary>
        public event Action<int> ButtonPressedLong;

        /// <summary>
        /// Gets the current value of a game axis.
        /// </summary>
        /// <param name="axisIndex">The axis to query (its integer index).</param>
        /// <returns>The current value of the game axis (usually this number is [-1;1] )</returns>
        public abstract float GetAxis(int axisIndex);

        /// <summary>
        /// The minimum amount of seconds a button has to be held down for the <see cref="ButtonPressedLong"/> event to fire.
        /// </summary>
        public const float LONG_BUTTON_PRESS_THRESHOLD = 1.45f;

        /// <summary>
        /// Current state of the buttons.
        /// </summary>
        private readonly Dictionary<int, bool> buttonStates = new Dictionary<int, bool>(20);

#if UNITY_EDITOR
        [SerializeField]
        private bool debugButtons = false;
#endif

        [SerializeField]
        [Tooltip("Only the game buttons whose index return true in this array will be checked against long button pressed events (for performance's sake).")]
        private bool[] allowLongButtonPressed;

        /// <summary>
        /// Is a specific button currently held down?
        /// </summary>
        /// <param name="button">Which button?</param>
        /// <returns>True if the specified button is held down; false if otherwise.</returns>
        public bool IsButtonDown(int button)
        {
            bool buttonState;
            buttonStates.TryGetValue(button, out buttonState);

            return buttonState;
        }

        protected virtual void OnEnable()
        {
            ButtonPressed += InputController_ButtonPressed;
            ButtonReleased += InputController_ButtonReleased;
        }

        protected virtual void OnDisable()
        {
            ButtonPressed -= InputController_ButtonPressed;
            ButtonReleased -= InputController_ButtonReleased;
        }

        private void InputController_ButtonPressed(int buttonIndex)
        {
#if UNITY_EDITOR
            if (debugButtons) print($"Pressed Button {buttonIndex}");
#endif
            if (buttonStates.ContainsKey(buttonIndex))
            {
                buttonStates[buttonIndex] = true;
            }
            else
            {
                buttonStates.Add(buttonIndex, true);
            }

            if (buttonIndex >= 0 && buttonIndex < allowLongButtonPressed.Length && allowLongButtonPressed[buttonIndex])
            {
                StartCoroutine(LongButtonPressCoroutine(buttonIndex));
            }
        }

        private void InputController_ButtonReleased(int buttonIndex)
        {
#if UNITY_EDITOR
            if (debugButtons) print($"Released Button {buttonIndex}");
#endif
            if (buttonStates.ContainsKey(buttonIndex))
            {
                buttonStates[buttonIndex] = false;
            }
            else
            {
                // Even though this else block doesn't seem like it can ever be reached,
                // you'd be surprised of what would happen if the user starts the game with a button already held down ;)
                buttonStates.Add(buttonIndex, false);
            }
        }

        private IEnumerator LongButtonPressCoroutine(int buttonIndex)
        {
            float endTime = Time.time + LONG_BUTTON_PRESS_THRESHOLD;
            while (Time.time < endTime)
            {
                if (!IsButtonDown(buttonIndex))
                {
                    yield break;
                }

                yield return null;
            }

#if UNITY_EDITOR
            if (debugButtons) print($"Long Pressed Button {buttonIndex}");
#endif

            ButtonPressedLong?.Invoke(buttonIndex);
        }

        /// <summary>
        /// Gets the device's configuration as a portable json string.
        /// </summary>
        /// <param name="prettyPrint">Should the json look good?</param>
        /// <returns>A portable json string representation of this <see cref="InputDevice"/> instance.</returns>
        public string ToJson(bool prettyPrint = false)
        {
            return JsonUtility.ToJson(this, prettyPrint);
        }

        /// <summary>
        /// Loads a json config that has been obtained with <see cref="ToJson"/> into this <see cref="InputDevice"/> instance.
        /// </summary>
        /// <param name="json">The json config to load.</param>
        public void LoadFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentException($"{nameof(InputDevice)}::{nameof(LoadFromJson)}: The passed json string is null or empty! Couldn't load config...");
            }

            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com