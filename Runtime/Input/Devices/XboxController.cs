#if UNITY_STANDALONE_WIN_64 || UNITY_EDITOR_WIN || UNITY_XBOXONE

using System;
using UnityEngine;
using GlitchedPolygons.ExtensionMethods;

namespace GlitchedPolygons.Input
{
    /// <summary>
    /// The almighty xbox controller.<para> </para>
    ///
    /// This is accessible from the xbox itself 
    /// or Windows exclusively (due to XInput requiring DirectX to work).
    /// </summary>
    [RequireComponent(typeof(XInput))]
    public class XboxController : InputDevice
    {
        private XInput xinput;

        /// <inheritdoc/>
        public override event Action<int> ButtonPressed;

        /// <inheritdoc/>
        public override event Action<int> ButtonReleased;

        [SerializeField]
        private XboxControllerAxis[] axes =
        {
            XboxControllerAxis.LeftThumbStickX,
            XboxControllerAxis.LeftThumbStickY,
            XboxControllerAxis.RightThumbStickX,
            XboxControllerAxis.RightThumbStickY
        };

        [SerializeField]
        private XboxControllerButton[] buttons;

#if UNITY_EDITOR_WIN
        [SerializeField]
        private int debugAxis = -1;
#endif

        /// <inheritdoc/>
        public override float GetAxis(int axisIndex)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            switch (axes[axisIndex])
            {
                case XboxControllerAxis.RT:
                    return xinput.RT;
                case XboxControllerAxis.LT:
                    return xinput.LT;
                case XboxControllerAxis.RightThumbStickX:
                    return xinput.RightThumbStickX;
                case XboxControllerAxis.RightThumbStickY:
                    return xinput.RightThumbStickY;
                case XboxControllerAxis.LeftThumbStickX:
                    return xinput.LeftThumbStickX;
                case XboxControllerAxis.LeftThumbStickY:
                    return xinput.LeftThumbStickY;
            }

            return 0.0f;
        }

        /// <summary>
        /// Reassign an xbox controller axis.
        /// </summary>
        /// <param name="axisIndex">The axis to modify.</param>
        /// <param name="newAxisAssociation">The new controller axis to remap to.</param>
        public void ReassignAxis(int axisIndex, XboxControllerAxis newAxisAssociation)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            axes[axisIndex] = newAxisAssociation;
        }

        /// <summary>
        /// Returns the <see cref="XboxControllerAxis"/> mapped to the specified axis index.
        /// </summary>
        /// <param name="axisIndex">The axis whose associated <see cref="XboxControllerAxis"/> you want to retrieve.</param>
        /// <returns>The <see cref="XboxControllerAxis"/> associated with the passed axis index.</returns>
        public XboxControllerAxis GetAxisAssociation(int axisIndex)
        {
            if (axisIndex < 0 || axisIndex >= axes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(axisIndex));
            }

            return axes[axisIndex];
        }

        /// <summary>
        /// Reassign an xbox button.
        /// </summary>
        /// <param name="buttonIndex">The button to modify.</param>
        /// <param name="newButtonAssociation">The new controller button to remap to.</param>
        public void ReassignButton(int buttonIndex, XboxControllerButton newButtonAssociation)
        {
            if (buttonIndex < 0 || buttonIndex >= buttons.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(buttonIndex));
            }

            buttons[buttonIndex] = newButtonAssociation;
        }

        /// <summary>
        /// Returns the <see cref="XboxControllerButton"/> mapped to the specified button index.
        /// </summary>
        /// <param name="buttonIndex">The button whose associated <see cref="XboxControllerButton"/> you want to retrieve.</param>
        /// <returns>The <see cref="XboxControllerButton"/> mapped to the specified game button index.</returns>
        public XboxControllerButton GetButtonAssociation(int buttonIndex)
        {
            if (buttonIndex < 0 || buttonIndex >= buttons.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(buttonIndex));
            }

            return buttons[buttonIndex];
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            xinput = gameObject.GetOrAddComponent<XInput>();

            xinput.ButtonPressed += Xinput_ButtonPressed;
            xinput.ButtonReleased += Xinput_ButtonReleased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            xinput.ButtonPressed -= Xinput_ButtonPressed;
            xinput.ButtonReleased -= Xinput_ButtonReleased;
        }

        private void Xinput_ButtonPressed(XboxControllerButton button)
        {
            for (int i = buttons.Length - 1; i >= 0; i--)
            {
                if (buttons[i] == button)
                {
                    ButtonPressed?.Invoke(i);
                }
            }
        }

        private void Xinput_ButtonReleased(XboxControllerButton button)
        {
            for (int i = buttons.Length - 1; i >= 0; i--)
            {
                if (buttons[i] == button)
                {
                    ButtonReleased?.Invoke(i);
                }
            }
        }

#if UNITY_EDITOR_WIN
        private void Update()
        {
            if (debugAxis >= 0 && debugAxis < axes.Length) print(GetAxis(debugAxis));
        }
#endif
    }
}
#endif

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com