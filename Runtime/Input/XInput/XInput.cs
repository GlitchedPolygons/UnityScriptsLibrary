using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using XInputDotNetPure;

namespace GlitchedPolygons.Input
{
    /// <summary>
    /// XInputDotNet wrapper behaviour for singleplayer-oriented xbox controller input sampling.<para> </para>
    ///
    /// Bind your desired actions to the various button events in here.
    /// </summary>
    public sealed class XInput : MonoBehaviour
    {
        /// <summary>
        /// The active xbox controller slot.
        /// </summary>
        public PlayerIndex activePlayerIndex = PlayerIndex.One;

        #region EVENTS

        /// <summary>
        /// This event is raised whenever the current controller (specified by the active player index) is (re)connected.
        /// </summary>
        public event Action ControllerConnected = null;

        [SerializeField]
        private UnityEvent controllerConnected;

        /// <summary>
        /// This event is raised when the currently active controller is disconnected.
        /// </summary>
        public event Action ControllerDisconnected = null;

        [SerializeField]
        private UnityEvent controllerDisconnected;

        /// <summary>
        /// This event is raised when the currently active xbox controller detects a button press.<para> </para>
        ///
        /// The pressed <see cref="XboxControllerButton"/> is passed as a function parameter.
        /// </summary>
        public event Action<XboxControllerButton> ButtonPressed = null;

        /// <summary>
        /// This event is raised when any <see cref="XboxControllerButton"/> was released (after being held down) from the currently active xbox controller.<para> </para>
        ///
        /// The released <see cref="XboxControllerButton"/> is passed as a function parameter.
        /// </summary>
        public event Action<XboxControllerButton> ButtonReleased = null;

        #endregion

        #region AXES

        /// <summary>
        /// Horizontal axis value of the left joystick. [-1; 1]
        /// </summary>
        public float LeftThumbStickX { get; private set; }

        /// <summary>
        /// Vertical axis value of the left joystick. [-1; 1]
        /// </summary>
        public float LeftThumbStickY { get; private set; }

        /// <summary>
        /// Horizontal axis value of the right joystick. [-1; 1]
        /// </summary>
        public float RightThumbStickX { get; private set; }

        /// <summary>
        /// Vertical axis value of the right joystick. [-1; 1]
        /// </summary>
        public float RightThumbStickY { get; private set; }

        /// <summary>
        /// The left trigger's axis [0; 1]
        /// </summary>
        public float LT { get; private set; }

        /// <summary>
        /// The right trigger's axis [0; 1]
        /// </summary>
        public float RT { get; private set; }

        #endregion

        #region PRIVATE VARIABLES

        private GamePadState currentState;
        private GamePadState previousState;

        private bool connected = false;
        private bool disconnected = false;

        #endregion

        private void OnDisable()
        {
            StopVibration();
        }

        private void Update()
        {
            previousState = currentState;
            currentState = GamePad.GetState(activePlayerIndex);

            if (currentState.IsConnected)
            {
                if (!connected)
                {
                    connected = true;
                    disconnected = false;

                    ControllerConnected?.Invoke();
                    controllerConnected?.Invoke();
                }

                #region THUMBSTICKS

                LeftThumbStickX = currentState.ThumbSticks.Left.X;
                LeftThumbStickY = currentState.ThumbSticks.Left.Y;

                RightThumbStickX = currentState.ThumbSticks.Right.X;
                RightThumbStickY = currentState.ThumbSticks.Right.Y;

                #endregion

                #region A-B-X-Y BUTTONS

                // A BUTTON
                if (previousState.Buttons.A == ButtonState.Released && currentState.Buttons.A == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.A);
                }

                if (previousState.Buttons.A == ButtonState.Pressed && currentState.Buttons.A == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.A);
                }

                // B BUTTON
                if (previousState.Buttons.B == ButtonState.Released && currentState.Buttons.B == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.B);
                }

                if (previousState.Buttons.B == ButtonState.Pressed && currentState.Buttons.B == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.B);
                }

                // X BUTTON
                if (previousState.Buttons.X == ButtonState.Released && currentState.Buttons.X == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.X);
                }

                if (previousState.Buttons.X == ButtonState.Pressed && currentState.Buttons.X == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.X);
                }

                // Y BUTTON
                if (previousState.Buttons.Y == ButtonState.Released && currentState.Buttons.Y == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.Y);
                }

                if (previousState.Buttons.Y == ButtonState.Pressed && currentState.Buttons.Y == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.Y);
                }

                #endregion

                #region BUMPERS

                // LB
                if (previousState.Buttons.LeftShoulder == ButtonState.Released && currentState.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.LB);
                }

                if (previousState.Buttons.LeftShoulder == ButtonState.Pressed && currentState.Buttons.LeftShoulder == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.LB);
                }

                // RB
                if (previousState.Buttons.RightShoulder == ButtonState.Released && currentState.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.RB);
                }

                if (previousState.Buttons.RightShoulder == ButtonState.Pressed && currentState.Buttons.RightShoulder == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.RB);
                }

                #endregion

                #region TRIGGERS

                // SET TRIGGER AXES
                LT = previousState.Triggers.Left;
                RT = previousState.Triggers.Right;

                // LT PRESSED
                if (previousState.Triggers.Left < .5f && currentState.Triggers.Left > .5f)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.LT);
                }

                // LT RELEASED
                if (previousState.Triggers.Left > .5f && currentState.Triggers.Left < .5f)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.LT);
                }

                // RT PRESSED
                if (previousState.Triggers.Right < .5f && currentState.Triggers.Right > .5f)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.RT);
                }

                // RT RELEASED
                if (previousState.Triggers.Right > .5f && currentState.Triggers.Right < .5f)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.RT);
                }

                #endregion

                #region START & BACK BUTTONS

                // START BUTTON
                if (previousState.Buttons.Start == ButtonState.Released && currentState.Buttons.Start == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.Start);
                }

                if (previousState.Buttons.Start == ButtonState.Pressed && currentState.Buttons.Start == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.Start);
                }

                // BACK BUTTON
                if (previousState.Buttons.Back == ButtonState.Released && currentState.Buttons.Back == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.Back);
                }

                if (previousState.Buttons.Back == ButtonState.Pressed && currentState.Buttons.Back == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.Back);
                }

                #endregion

                #region THUMBSTICK BUTTONS

                // LEFT THUMBSTICK BUTTON
                if (previousState.Buttons.LeftStick == ButtonState.Released && currentState.Buttons.LeftStick == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.LeftThumbStick);
                }

                if (previousState.Buttons.LeftStick == ButtonState.Pressed && currentState.Buttons.LeftStick == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.LeftThumbStick);
                }

                // RIGHT THUMBSTICK BUTTON
                if (previousState.Buttons.RightStick == ButtonState.Released && currentState.Buttons.RightStick == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.RightThumbStick);
                }

                if (previousState.Buttons.RightStick == ButtonState.Pressed && currentState.Buttons.RightStick == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.RightThumbStick);
                }

                #endregion

                #region D-PAD

                // D-PAD UP BUTTON
                if (previousState.DPad.Up == ButtonState.Released && currentState.DPad.Up == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.DPad_Up);
                }

                if (previousState.DPad.Up == ButtonState.Pressed && currentState.DPad.Up == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.DPad_Up);
                }

                // D-PAD RIGHT BUTTON
                if (previousState.DPad.Right == ButtonState.Released && currentState.DPad.Right == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.DPad_Right);
                }

                if (previousState.DPad.Right == ButtonState.Pressed && currentState.DPad.Right == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.DPad_Right);
                }

                // D-PAD DOWN BUTTON
                if (previousState.DPad.Down == ButtonState.Released && currentState.DPad.Down == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.DPad_Down);
                }

                if (previousState.DPad.Down == ButtonState.Pressed && currentState.DPad.Down == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.DPad_Down);
                }

                // D-PAD LEFT BUTTON
                if (previousState.DPad.Left == ButtonState.Released && currentState.DPad.Left == ButtonState.Pressed)
                {
                    ButtonPressed?.Invoke(XboxControllerButton.DPad_Left);
                }

                if (previousState.DPad.Left == ButtonState.Pressed && currentState.DPad.Left == ButtonState.Released)
                {
                    ButtonReleased?.Invoke(XboxControllerButton.DPad_Left);
                }

                #endregion
            }
            else
            {
                if (!disconnected)
                {
                    disconnected = true;
                    connected = false;

                    ControllerDisconnected?.Invoke();
                    controllerDisconnected?.Invoke();
                }
            }
        }

        /// <summary>
        /// Returns whether the specified controller is currently connected or not.
        /// </summary>
        /// <param name="playerIndex">The controller's player index (One = top left quadrant on the xbox controller's ring of light, Two = top right, etc...)</param>
        public bool IsControllerConnected(PlayerIndex playerIndex)
        {
            return playerIndex == activePlayerIndex ? currentState.IsConnected : GamePad.GetState(playerIndex).IsConnected;
        }

        /// <summary>
        /// Make the xbox controller vibrate over a given amount of seconds.
        /// </summary>
        public void Vibrate(float duration, float leftMotor = 1.0f, float rightMotor = 1.0f)
        {
            if (duration < 0) duration *= -1.0f;
            GamePad.SetVibration(activePlayerIndex, leftMotor, rightMotor);
            Invoke(nameof(StopVibration), duration);
        }

        /// <summary>
        /// Immediately stop the xbox controller's vibration.
        /// </summary>
        public void StopVibration()
        {
            GamePad.SetVibration(activePlayerIndex, 0.0f, 0.0f);
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com