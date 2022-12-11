using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// <see cref="EventTrigger"/> extension methods.
    /// </summary>
    public static class EventTriggerExtensions
    {
        /// <summary>
        /// Adds an event trigger listener to an <see cref="EventTrigger"/>. Use this for dynamically subscribing to Unity UI event trigger events at runtime.
        /// </summary>
        /// <param name="et">The <see cref="EventTrigger"/> to subscribe to.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>The added <see cref="EventTrigger.Entry"/>.</returns>
        public static EventTrigger.Entry AddEventTriggerListener(this EventTrigger et, EventTriggerType eventType, System.Action<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = eventType,
                callback = new EventTrigger.TriggerEvent()
            };
            entry.callback.AddListener(new UnityAction<BaseEventData>(callback));
            et.triggers.Add(entry);
            return entry;
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com
