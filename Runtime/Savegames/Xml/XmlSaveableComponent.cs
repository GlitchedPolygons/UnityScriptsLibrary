using UnityEngine;
using System.Xml.Linq;
using GlitchedPolygons.Identification;

namespace GlitchedPolygons.SavegameFramework.Xml
{
    /// <summary>
    /// All components that you wish to be saved and loaded in some way between runtimes should inherit from <see cref="XmlSaveableComponent"/> in order to provide functionality for object persistance/reconstruction.<para> </para>
    /// If you want to use the Awake and OnDestroy methods in <see cref="XmlSaveableComponent"/>s, please use the <c>override</c> keyword and call <c>base.Awake();</c> and/or <c>base.OnDestroy();</c>.<para> </para>
    /// All <see cref="XmlSaveableComponent"/>s have the <c>[ExecuteInEditMode]</c> attribute; therefore if you encounter unwanted behaviour due to MonoBehaviour methods being called at edit time, consider adding: <c>if (!Application.isPlaying) return; </c>
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(ID))]
    public abstract class XmlSaveableComponent : SaveableComponent
    {
        /// <summary>
        /// The <see cref="ID"/> component used for <see cref="XmlSaveableComponent"/> identification and distinction on save/load.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private ID id = null;

        /// <summary>
        /// The <see cref="XmlSaveableComponent"/>'s ID. 
        /// This is a unique identifier that should be immutable and persistent.
        /// </summary>
        public override int ID
        {
            get
            {
                if (id == null)
                {
                    id = GetComponent<ID>();
                }

                return id.GetID();
            }
        }
        
        /// <inheritdoc cref="SaveableComponent.BeforeSaving"/>
        public abstract override void BeforeSaving();

        /// <inheritdoc cref="SaveableComponent.AfterLoading"/>
        public abstract override void AfterLoading();

        [ContextMenu("Print ID to console")]
        protected void PrintID()
        {
            Debug.Log($"InstanceID: {GetInstanceID()}\nID: {ID}\nGameObject's Name: {gameObject.name}");
        }

        protected virtual void Awake()
        {
            id = GetComponent<ID>();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (GetComponent<XmlSavegameManager>() != null)
                {
                    UnityEditor.EditorUtility.DisplayDialog("ERROR!", "There is already a SavegameManager component attached to this GameObject. There may only be one or the other on a single GameObject!", "K, sorry :(");
                    DestroyImmediate(this);
                    return;
                }

                if (GetComponents<XmlSaveableComponent>().Length > 1)
                {
                    UnityEditor.EditorUtility.DisplayDialog("ERROR! Stop it right there, you...", "There is already a SaveableComponent attached to this GameObject. There may only be one per GameObject!", "K, sorry :(");
                    DestroyImmediate(this);
                    return;
                }
                
                FindObjectOfType<XmlSavegameManager>()?.GatherSaveableComponentsInScene();
            }
#endif
        }

        protected virtual void OnDestroy()
        {
#if UNITY_EDITOR
            FindObjectOfType<XmlSavegameManager>()?.GatherSaveableComponentsInScene();
#endif
        }

        /// <summary>
        /// Gets the nicely formatted xml <c><see cref="XElement"/></c> containing all the data that is needed to reconstruct the saveable object through <see cref="XmlSaveableComponent.LoadXml"/>.
        /// </summary>
        /// <returns>The <c><see cref="XElement"/></c> containing all data needed to reconstruct the object on <see cref="XmlSaveableComponent.LoadXml"/>.</returns>
        public abstract XElement GetXml();

        /// <summary>
        /// Loads an <c><see cref="XElement"/></c> (hopefully the one that was originally produced by  the <see cref="GetXml"/> method) into the class.<para> </para>
        /// This method should reconstruct the object's state on load, and then return <c>true</c> if the procedure was successful (<c>false</c> if it failed in some way).<para> </para>
        /// </summary>
        /// <remarks>This should NOT be used to make the <see cref="SaveableComponent"/> use the loaded data in any way: THIS IS JUST FOR LOADING DATA INTO THE CLASS FIELDS! Instead, use the <see cref="SaveableComponent.AfterLoading"/> method for applying the loaded data (e.g. setting <c>Transform</c> positions and rotations and whatnot)!</remarks>
        /// <param name="xml">The <c><see cref="XElement"/></c> that was obtained via <see cref="GetXml"/>.</param>
        /// <returns>Should return <c>true</c> if the loading procedure was successful; <c>false</c> if it failed in some way.</returns>
        public abstract bool LoadXml(XElement xml);
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
