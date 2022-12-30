using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using GlitchedPolygons.Utilities;
using GlitchedPolygons.ExtensionMethods;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlitchedPolygons.SavegameFramework.Json
{
    /// <summary>
    /// The <see cref="JsonSavegameManager"/> is a component that should exist only exactly once in every scene.<para> </para>
    /// It provides functionality for saving/loading the game's state.<para> </para>
    /// The savegame's file format is JSON, which can be compressed and/or encrypted.<para> </para>
    /// Check out the <see cref="SavegameManager"/>'s base class documentation for more information.
    /// </summary>
    /// <seealso cref="SavegameManager"/>
    [ExecuteInEditMode]
    public sealed class JsonSavegameManager : SavegameManager
    {
        #region Inspector variables

        /// <summary>
        /// Should the generated JSON be nicely formatted and indented?
        /// </summary>
        [SerializeField]
        [Header("This is the JsonSavegameManager component.\nThere should be exactly one in every scene.\n\nThis component provides methods for saving and loading savegames.\n\nIf you encrypt and/or compress the savegames, make ABSOLUTELY sure that the \"key\", \"encrypt\", \"compress\" and \"iterationsOfPBKDF2\" settings match up in EVERY SavegameManager instance throughout the entire game, otherwise you might not be able to load one or more savegames at some point.\n\nMake sure that the key field is at least 32 characters long.\n\nSaving will result in a savegame file placed inside the savegames directory path (by default \"Applications.persistentDataPath/Savegames/\"). Check out the (well documented) source code for more information.\n")]
        private bool prettyPrint = false;

        /// <summary>
        /// Should the scenes be loaded by their name or by build index?
        /// </summary>
        [SerializeField]
        private bool loadByName = false;

        /// <summary>
        /// List of all <see cref="JsonSaveableComponent"/>s in the scene.
        /// </summary>
        [SerializeField]
        private JsonSaveableComponent[] saveableComponents = new JsonSaveableComponent[4];

        #endregion

        /// <summary>
        /// Are we currently saving or loading?
        /// </summary>
        private bool busy = false;

        /// <summary>
        /// This is the temporary <see cref="JsonSavegame"/> that survives the map transition when loading a savegame.<para> </para>
        /// </summary>
        private JsonSavegame transitorySavegame = null;

        /// <summary>
        /// Gathers all the <see cref="JsonSaveableComponent"/>s in the scene and registers them to the <see cref="JsonSavegameManager.saveableComponents"/> array.
        /// </summary>
        [ContextMenu("Gather all SaveableComponents in scene")]
        public void GatherSaveableComponentsInScene() => saveableComponents = FindObjectsOfType<JsonSaveableComponent>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            SpawnedPrefab.SPAWNED_PREFABS.Clear();
        }

        /// <inheritdoc cref="SavegameManager.Save"/>
        public override void Save(string fileName, bool saveSpawnedPrefabs = true)
        {
            if (busy)
            {
                return;
            }

            busy = true;

            StartCoroutine(SaveCoroutine(fileName, saveSpawnedPrefabs));
        }

        private IEnumerator SaveCoroutine(string fileName, bool saveSpawnedPrefabs)
        {
            CheckSavegamesDirectory();

            OnSaving();

            yield return YieldInstructions.WaitForEndOfFrame;

            JsonSavegame outputSavegame = new();

            DateTime startUtc = DateTime.UtcNow;
            TimeSpan timeout = TimeSpan.FromSeconds(savingTimeoutSeconds);

            Scene scene = SceneManager.GetActiveScene();

            outputSavegame.mapIndex = scene.buildIndex;
            outputSavegame.mapName = scene.name;

            int saveableComponentsCount = saveableComponents.Length;

            if (saveableComponentsCount != 0)
            {
                outputSavegame.components.Capacity = saveableComponentsCount;

                for (int i = 0; i < saveableComponentsCount; ++i)
                {
                    JsonSaveableComponent component = saveableComponents[i];

                    if (component == null)
                    {
                        continue;
                    }

                    component.BeforeSaving();

                    outputSavegame.components.Add(component);

                    if (i % batchSize == 0)
                    {
                        if (DateTime.UtcNow - startUtc > timeout)
                        {
                            OnSaveFailed();

                            yield return YieldInstructions.WaitForFixedUpdate;
                            busy = false;
                            OnReady();

                            yield break;
                        }

                        yield return YieldInstructions.WaitForEndOfFrame;
                    }
                }
            }

            if (SpawnedPrefab.SPAWNED_PREFABS.Count != 0 && saveSpawnedPrefabs)
            {
                for (int i = 0; i < SpawnedPrefab.SPAWNED_PREFABS.Count; ++i)
                {
                    var spawnedPrefab = SpawnedPrefab.SPAWNED_PREFABS[i];

                    if (spawnedPrefab == null)
                    {
                        continue;
                    }

                    SaveableComponent component = spawnedPrefab.GetSaveableComponent();

                    JsonSpawnedPrefabTuple spawnedPrefabTuple = new()
                    {
                        resourcePath = spawnedPrefab.GetResourcePath()
                    };

                    if (component != null)
                    {
                        component.BeforeSaving();
                        
                        spawnedPrefabTuple.json = JsonUtility.ToJson(component);
                    }

                    outputSavegame.spawnedPrefabs.Add(spawnedPrefabTuple);

                    if (i % batchSize == 0)
                    {
                        if (DateTime.UtcNow - startUtc > timeout)
                        {
                            OnSaveFailed();

                            yield return YieldInstructions.WaitForFixedUpdate;
                            busy = false;
                            OnReady();

                            yield break;
                        }

                        yield return YieldInstructions.WaitForEndOfFrame;
                    }
                }
            }

            var task = Task.Run(() =>
            {
                string filePath = Path.Combine(savegamesDirectoryPath, $"{fileName}_{DateTime.Now.ToString(TIME_FORMAT)}{savegameFileExtension}");

                string json = JsonUtility.ToJson(outputSavegame, prettyPrint);

                byte[] outputBytes = json.UTF8GetBytes();

                if (compress)
                {
                    outputBytes = GZip.Compress(outputBytes);
                }

                if (encrypt)
                {
                    byte[] salt = new byte[64];
                    RandomNumberGenerator.Fill(salt);

                    outputBytes = salt.Concat(SymmetricCryptography.Encrypt(outputBytes, key, Convert.ToBase64String(salt), iterationsOfPBKDF2)).ToArray();
                }

                File.WriteAllBytes(filePath, outputBytes);
            });

            while (!task.IsCompleted)
            {
                yield return YieldInstructions.GetWaitForSecondsRealtime(64);

                if (DateTime.UtcNow - startUtc > timeout)
                {
                    OnSaveFailed();

                    yield return YieldInstructions.WaitForFixedUpdate;
                    OnReady();

                    yield break;
                }
            }

            if (task.IsCompletedSuccessfully)
            {
                OnSave();
            }
            else
            {
                OnSaveFailed();
            }

            if (collectGarbageOnSave)
            {
                GC.Collect();
            }

            yield return YieldInstructions.WaitForFixedUpdate;
            busy = false;
            OnReady();
        }

        /// <inheritdoc cref="SavegameManager.Load"/>
        public override void Load(string fileName)
        {
            if (busy)
            {
                return;
            }

            busy = true;

            StartCoroutine(LoadCoroutine(fileName));
        }

        private IEnumerator LoadCoroutine(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(JsonSavegameManager)}: Couldn't load savegame! The specified {nameof(fileName)} string is null or empty.");
#endif
                yield break;
            }

            CheckSavegamesDirectory();

            OnLoading();

            string filePath = Path.Combine(savegamesDirectoryPath, $"{fileName}{savegameFileExtension}");

            if (!File.Exists(filePath))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(JsonSavegameManager)}: Couldn't load savegame! The specified file path does not exist or is invalid.");
#endif
                yield break;
            }

            DateTime startUtc = DateTime.UtcNow;
            TimeSpan timeout = TimeSpan.FromSeconds(loadingTimeoutSeconds);

            var task = Task.Run(() =>
            {
                if (!encrypt && !compress)
                {
                    transitorySavegame = JsonUtility.FromJson<JsonSavegame>(File.ReadAllText(filePath));
                }
                else
                {
                    byte[] data = null;

                    if (encrypt)
                    {
                        using var dataStream = new MemoryStream();
                        using var fileStream = new FileStream(filePath, FileMode.Open);

                        byte[] salt = new byte[64];
                        fileStream.Read(salt, 0, salt.Length);

                        fileStream.CopyTo(dataStream);

                        data = SymmetricCryptography.Decrypt(dataStream.ToArray(), key, Convert.ToBase64String(salt), iterationsOfPBKDF2);
                    }

                    if (compress)
                    {
                        data = GZip.Decompress(data ?? File.ReadAllBytes(filePath));
                    }

                    transitorySavegame = JsonUtility.FromJson<JsonSavegame>(data.UTF8GetString());
                }
            });

            while (!task.IsCompleted)
            {
                yield return YieldInstructions.GetWaitForSecondsRealtime(64);

                if (DateTime.UtcNow - startUtc > timeout)
                {
                    OnLoadFailed();

                    yield return YieldInstructions.WaitForFixedUpdate;
                    OnReady();

                    yield break;
                }
            }

            if (task.IsCompletedSuccessfully && transitorySavegame is not null)
            {
                AsyncOperation loadOperation = null;
                
                if (loadByName)
                {
                    string sceneName = transitorySavegame.mapName;

                    if (sceneName.NotNullNotEmpty())
                    {
#if UNITY_EDITOR
                        Debug.Log($"{nameof(JsonSavegameManager)}: Load savegame by scene name: {sceneName}");
#endif
                        SceneManager.sceneLoaded += OnNewSceneLoaded;
                        loadOperation = SceneManager.LoadSceneAsync(sceneName);
                    }
                }
                else
                {
                    SceneManager.sceneLoaded += OnNewSceneLoaded;
                    loadOperation = SceneManager.LoadSceneAsync(transitorySavegame.mapIndex);
                }
                
                if (loadOperation != null)
                {
                    while (!loadOperation.isDone)
                    {
                        yield return YieldInstructions.GetWaitForSecondsRealtime(64);
                    }

                    loadOperation.allowSceneActivation = true;
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(JsonSavegameManager)}: Failed to load savegame {filePath}");
#endif
            }
        }

        /// <summary>
        /// When a new map is loaded, this method is called on the old <see cref="JsonSavegameManager"/> and will
        /// pass its <see cref="JsonSavegame"/> to the newly loaded map's <see cref="JsonSavegameManager"/>.<para> </para>
        /// This way the new <see cref="JsonSavegameManager"/> is aware of the data it has to reconstruct. Once that's done,
        /// the <see cref="Reconstruct"/> method is called on the new manager.
        /// </summary>
        /// <param name="scene">The newly loaded <see cref="Scene"/>.</param>
        /// <param name="loadSceneMode">The used <see cref="LoadSceneMode"/>.</param>
        private void OnNewSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (transitorySavegame == null)
            {
                return;
            }

            if (transitorySavegame.mapIndex != scene.buildIndex)
            {
                return;
            }

            JsonSavegameManager newSavegameManager = FindObjectOfType<JsonSavegameManager>();

            if (newSavegameManager == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(JsonSavegameManager)}: In the newly loaded scene there is no {nameof(JsonSavegameManager)}; nothing to load the data with!");
#endif
                return;
            }

            // The savegame variable has to survive the map transition.
            // Therefore, we assign it to the new scene's JsonSavegameManager and load it up there. 
            // This manager here has served its purpose very well, and can now die in peace... RIP!
            newSavegameManager.transitorySavegame = this.transitorySavegame;
            newSavegameManager.Reconstruct();

            // Unregister and goodbye!
            SceneManager.sceneLoaded -= OnNewSceneLoaded;
        }

        /// <summary>
        /// This method loads the <see cref="JsonSavegameManager.transitorySavegame"/> variable that survived the map transition.<para> </para>
        /// It's the method that actually applies the reconstruction to the various <see cref="SaveableComponent"/>s in the newly loaded scene (and spawns any objects that were runtime-created on save).
        /// </summary>
        private void Reconstruct()
        {
            if (transitorySavegame is null)
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(JsonSavegameManager)}: There was an error transitioning the transitory savegame instance over to the new scene... Couldn't reconstruct scene successfully!");
#endif
                return;
            }

            IDictionary<int, JsonSaveableComponentTuple> components = transitorySavegame.components.ToDictionary(c => c.id);

            for (int i = saveableComponents.Length - 1; i >= 0; --i)
            {
                try
                {
                    JsonSaveableComponent jsonSaveableComponent = saveableComponents[i];

                    if (jsonSaveableComponent == null)
                    {
                        continue;
                    }

                    if (components.TryGetValue(jsonSaveableComponent.ID, out var component))
                    {
                        JsonUtility.FromJsonOverwrite(component.json, jsonSaveableComponent);
                        jsonSaveableComponent.AfterLoading();
                    }
                    else
                    {
                        Destroy(jsonSaveableComponent.gameObject);
                    }
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError($"{nameof(JsonSavegameManager)}: Failed to deserialize {nameof(JsonSaveableComponent)} with ID = {saveableComponents[i].ID} from savegame while loading and reconstructing the scene. Thrown exception: {e}");
#endif
                }
            }

            for (int i = transitorySavegame.spawnedPrefabs.Count - 1; i >= 0; --i)
            {
                JsonSpawnedPrefabTuple prefab = transitorySavegame.spawnedPrefabs[i];

                if (prefab is null)
                {
                    continue;
                }

                try
                {
                    GameObject obj = Resources.Load<GameObject>(prefab.resourcePath);

                    if (obj != null)
                    {
                        obj = Instantiate(obj);

                        JsonSaveableComponent component = obj.GetComponent<JsonSaveableComponent>();

                        if (component != null)
                        {
                            JsonUtility.FromJsonOverwrite(prefab.json, component);
                            component.AfterLoading();
                        }
                    }
#if UNITY_EDITOR
                    else
                    {
                        Debug.LogError($"{nameof(JsonSavegameManager)}: the Resources path \"{prefab.resourcePath}\" doesn't exist and thus couldn't be instantiated on load.");
                    }
#endif
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError($"{nameof(JsonSavegameManager)}: Failed to spawn prefab with resource path = {prefab.resourcePath} from savegame while loading and reconstructing the scene. Thrown exception: {e}");
#endif
                }
            }

            OnLoad();
            OnReady();

            busy = false;

            if (collectGarbageOnLoad)
            {
                GC.Collect();
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com