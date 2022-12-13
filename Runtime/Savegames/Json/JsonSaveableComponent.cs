using UnityEngine;
using GlitchedPolygons.Identification;

namespace GlitchedPolygons.SavegameFramework.Json
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ID))]
    public abstract class JsonSaveableComponent : SaveableComponent
    {
        /// <summary>
        /// The <see cref="ID"/> component used for <see cref="JsonSaveableComponent"/> identification and distinction on save/load.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private ID id = null;

        /// <summary>
        /// The <see cref="JsonSaveableComponent"/>'s ID. 
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
                if (GetComponent<JsonSavegameManager>() != null)
                {
                    UnityEditor.EditorUtility.DisplayDialog("ERROR!", "There is already a SavegameManager component attached to this GameObject. There may only be one or the other on a single GameObject!", "K, sorry :(");
                    DestroyImmediate(this);
                    return;
                }

                if (GetComponents<JsonSaveableComponent>().Length > 1)
                {
                    UnityEditor.EditorUtility.DisplayDialog("ERROR! Stop it right there, you...", "There is already a SaveableComponent attached to this GameObject. There may only be one per GameObject!", "K, sorry :(");
                    DestroyImmediate(this);
                    return;
                }
                
                FindObjectOfType<JsonSavegameManager>()?.GatherSaveableComponentsInScene();
            }
#endif
        }
        
        protected virtual void OnDestroy()
        {
#if UNITY_EDITOR
            FindObjectOfType<JsonSavegameManager>()?.GatherSaveableComponentsInScene();
#endif
        }

        /// <summary>
        /// Converts a <see cref="JsonSaveableComponent"/> instance to a <see cref="JsonSaveableComponentTuple"/>, ready to be serialized out into a <see cref="JsonSavegame"/>.
        /// </summary>
        /// <param name="component"><see cref="JsonSaveableComponent"/> to convert.</param>
        /// <returns><c>new JsonSaveableComponentTuple { id = component.ID, json = JsonUtility.ToJson(component) }</c></returns>
        public static implicit operator JsonSaveableComponentTuple(JsonSaveableComponent component)
        {
            return new JsonSaveableComponentTuple
            {
                id = component.ID,
                json = JsonUtility.ToJson(component)
            };
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com