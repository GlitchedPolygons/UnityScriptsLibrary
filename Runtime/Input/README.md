# Glitched Polygons Input System

This is the modular input system. So far there's 3 devices implemented and working correctly: the xbox one controller, a keyboard-mouse combo and a touchscreen (UI driven).

* To use it attach an InputMixer component to a GameObject in your scene: every scene that wants to make use of this system needs one.
* Then attach the devices you need to some other object (typically a child of the InputManager GameObject is a nice choice, keeping it all close together).
* Add the devices to the InputMixer inspector.
* Activate debugging to quickly check if everything works as expected (e.g. press a few buttons and see if messages are printed to the console)

### Subscribing to the input events

In order to use the input system you have to subscribe to its events: either via 
* `InputMixer.ButtonPressed`/`InputMixer.ButtonPressedLong`/`InputMixer.ButtonReleased`
* or directly (raw events) from an input device like XboxController.cs
* * The events are called the same here, except they come directly from the input device and offer no mixing capability with other devices.

### Implementing a new input device

Adding more input devices to this input system is not hard: you just have to create a class that inherits from InputDevice and implement it carefully. Take a look at that abstract base class in order to find out what your device is gonna need to implement/handle. 

Note that the focus should always be on performance! **That's also why raw ints are used as button and axis indices.** Therefore: write down your input mapping and keep it consistent! E.g. have your jump action always be button index nr. 3

Raphael Beck, 2018

