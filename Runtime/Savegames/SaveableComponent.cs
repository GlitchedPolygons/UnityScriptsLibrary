using UnityEngine;

namespace GlitchedPolygons.SavegameFramework
{
    /// <summary>
    /// All components that you wish to be saved and loaded in some way between runtimes should inherit ultimately from this.<para> </para>
    /// Find out more in the documentation of the implementing child classes, such as <see cref="GlitchedPolygons.SavegameFramework.Xml.XmlSaveableComponent"/>.
    /// </summary>
    /// <seealso cref="GlitchedPolygons.SavegameFramework.Xml.XmlSaveableComponent"/>
    public abstract class SaveableComponent : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="SaveableComponent"/>'s ID. 
        /// This is a unique identifier that should be immutable and persistent.
        /// </summary>
        public abstract int ID { get; }

        /// <summary>
        /// This is called by the <see cref="SavegameManager"/> implementation before saving this instance's data out to the savegame.<para> </para>
        /// Use this to sync up any data if you need (e.g. loading Unity component data - which is NOT serialized alongside automatically; keep that in mind! - into some field on the savegame like for example a "Vector3 positionCoordinates;" or something like that, yea..)
        /// </summary>
        public abstract void BeforeSaving();
        
        /// <summary>
        /// This is called after a successful <see cref="SavegameManager.Load"/>.<para> </para>
        /// Child classes should implement this and define what happens after the <see cref="SavegameManager.Load"/> fed the stored data into this class (e.g. if a <see cref="SaveableComponent"/>'s <c>Transform.position</c> is stored in the savegame, this would be the place to then take that value and load it back into the <c>Transform</c>). 
        /// </summary>
        public abstract void AfterLoading();
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com