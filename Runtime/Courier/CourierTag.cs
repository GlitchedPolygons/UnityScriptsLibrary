using System;
using System.Collections;
using System.Collections.Generic;
using GlitchedPolygons.Identification;
using GlitchedPolygons.SavegameFramework;
using UnityEngine;

namespace GlitchedPolygons.Courier
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(GUID))]
    public class CourierTag : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private GUID guid;

        [SerializeField]
        [HideInInspector]
        private SaveableComponent saveableComponent;
        
        private void Awake()
        {
            if (guid == null)
            {
                guid = GetComponent<GUID>();   
            }

            if (saveableComponent == null)
            {
                saveableComponent = GetComponent<SaveableComponent>();
            }
        }

        private void OnEnable()
        {
            Awake();
        }

        public string GetGUID() => guid.GetGUID();
    }
}

// Copyright (C) Raphael Beck, 2022 | https://glitchedpolygons.com