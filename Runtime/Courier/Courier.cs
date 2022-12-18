using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlitchedPolygons.SavegameFramework;

namespace GlitchedPolygons.Courier
{
    /// <summary>
    /// The <see cref="Courier"/> is an important entity that transports your stuff safely from one map to another.<para> </para>
    ///
    /// It works by gathering all the <see cref="SpawnedPrefab"/>s and <see cref="SaveableComponent"/>s, bundling them into a <see cref="CourierPackage"/> while you're still in the old map,
    /// and then spawning/loading those in the new map.<para> </para>
    /// 
    /// You could either manually add the <see cref="SpawnedPrefab"/>s that you want to carry over via the <see cref="Add"/> method,
    /// or you could also use a <see cref="TriggerVolumes.TriggerVolume"/> and do it from there on trigger enter
    /// (most likely when you're transitioning maps when reaching a specific location in the world).<para> </para>
    ///
    /// Make sure that there is exactly one (and only one!) <see cref="Courier"/> in every scene!<para> </para>
    ///
    /// For <see cref="SaveableComponent"/>s that are transported to another map: unlike <see cref="SpawnedPrefab"/>s, these are created at edit-time and not at runtime.
    /// That means that there needs to be a way to identify them bi-directionally! Otherwise, you would be capable of bringing an object from e.g. map 1 into map 2, and then re-carry it back to map 1 and end up having two of those objects in total... yea...<para> </para>
    ///
    /// To solve this, the <see cref="Courier"/> only carries <see cref="SaveableComponent"/>s 
    /// 
    /// Note that this internally makes use of the <see cref="SavegameFramework"/> and its <see cref="SaveableComponent"/> technology
    /// as a means of serializing state and handling map transitions. Make sure to read that documentation first if you care about how 
    /// how this all works under the hood :)<para> </para>
    /// </summary>
    /// 
    /// <remarks>
    /// Note that the <see cref="Courier"/> only transports your stuff over into maps that you load with it (either via the <see cref="LoadSceneByIndex"/> or <see cref="LoadSceneByName"/> method).<para> </para>
    /// 
    /// World transitions that are externally triggered from other scripts will not instruct the <see cref="Courier"/> to carry over objects in any way.
    /// </remarks>
    public class Courier : MonoBehaviour
    {
        [SerializeField]
        private List<SpawnedPrefab> spawnedPrefabs = new(16);

        /// <summary>
        /// Tells the <see cref="Courier"/> to carry <paramref name="spawnedPrefab"/> over to the next map it loads.
        /// </summary>
        /// <param name="spawnedPrefab"></param>
        public void Add(SpawnedPrefab spawnedPrefab)
        {
        }

        public void Remove(SpawnedPrefab spawnedPrefab)
        {
        }

        public void LoadSceneByIndex(int index)
        {
        }

        public void LoadSceneByName(string index)
        {
        }
    }
}

// Copyright (C) Raphael Beck, 2022 | https://glitchedpolygons.com