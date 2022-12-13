using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using GlitchedPolygons.ExtensionMethods;
using UnityEngine;
using UnityEngine.SceneManagement;
using GlitchedPolygons.Utilities;

namespace GlitchedPolygons.SavegameFramework.Xml
{
    /// <summary>
    /// The <see cref="XmlSavegameManager"/> is a component that should only ever exist once per scene.<para> </para>
    /// It provides functionality for saving/loading the game's state.<para> </para>
    /// The savegame's file format is xml that can be optionally compressed and/or encrypted.
    /// <remarks>When working in the editor with domain reloading disabled, you MUST ensure that the script execution order handles <see cref="XmlSavegameManager"/> BEFORE <see cref="SpawnedPrefab"/>!</remarks>
    /// </summary>
    [ExecuteInEditMode]
    public sealed class XmlSavegameManager : SavegameManager
    {
        #region Constants

        /// <summary>
        /// Pre-allocated savegame <see cref="XDocument"/>.
        /// </summary>
        private readonly XDocument savegame = new
        (
            new XElement("savegame",
                new XElement("map", new XAttribute("index", -2), new XAttribute("name", "map_name_here"),
                    new XElement("components", null),
                    new XElement("prefabs", null)))
        );

        #endregion

        #region Inspector variables
        
        /// <summary>
        /// Should the generated xml be nicely formatted and indented with whitespaces and line breaks?<para> </para>
        /// This only makes sense if you're not encrypting and not compressing the savegame, since it's a feature intended to enhance human readability of the xml (which falls away when encrypting/compressing).
        /// </summary>
        [SerializeField]
        [Header("This is the XmlSavegameManager component.\nThere should be exactly one in every scene.\n\nThis component provides methods for saving and loading savegames.\n\nIf you encrypt and/or compress the savegames, make ABSOLUTELY sure that the \"key\", \"encrypt\", \"compress\" and \"iterationsOfPBKDF2\" settings match up in EVERY XmlSavegameManager instance throughout the entire game, otherwise you might not be able to load one or more savegames at some point.\n\nMake sure that the key field is at least 32 characters long.\n\nSaving will result in a savegame file placed inside the savegames directory path (by default \"Applications.persistentDataPath/Savegames\"). Check out the (well documented) source code for more information.\n\nLoading a savegame will cause a chain reaction of events:\n\n1.\nReading savegame file (eventually decompress/decrypt).\n\n2.\nPassing it to a transitory savegame variable that survives the map transition.\n\n3.\nDetermining which scene to load, trigger the map transition and pass the transitory savegame variable to the new scene's XmlSavegameManager component.\n\n4.\nActually load the savegame's data into the scene; spawn any saved prefabs that weren't there at edit-time.\n\n5.\nErase traces.\n\nYou can gather all the SaveableComponents in the scene at edit-time automatically: there is a component context menu entry that does that.\n\nNote that only max. 1 SaveableComponent should ever be in a GameObject's hierarchy (e.g. a saveable Prop.cs component shouldn't have any child GameObjects with other SaveableComponents under it!).\nIdeally you would have one SaveableComponent at the root GameObject of your prefabs.\n")]
        private bool indent = false;

        /// <summary>
        /// Should the scenes be loaded by their name or by build index?
        /// </summary>
        [SerializeField]
        private bool loadByName = false;

        /// <summary>
        /// List of all <see cref="XmlSaveableComponent"/>s in the scene.
        /// </summary>
        [SerializeField]
        private XmlSaveableComponent[] saveableComponents = new XmlSaveableComponent[4];

        #endregion

        #region Cached XElements

        // The xe_ and XE_ prefix is used here to distinguish the actual class
        // variables from their linked XElement variable (xe_) and getter (XE_).
        // This obviously isn't mandatory, but it's very nice and a lot more readable.

        private XElement xe_root = null;
        private XElement XE_Root => xe_root ??= savegame.Root;

        private XElement xe_map = null;
        private XElement XE_Map => xe_map ??= XE_Root?.Element("map");

        private XAttribute xe_index = null;
        private XAttribute XE_Index => xe_index ??= XE_Map?.FirstAttribute;

        private XAttribute xe_name = null;
        private XAttribute XE_Name => xe_name ??= XE_Map?.LastAttribute;

        private XElement xe_components = null;
        private XElement XE_Components => xe_components ??= XE_Map?.Element("components");

        private XElement xe_prefabs = null;
        private XElement XE_Prefabs => xe_prefabs ??= XE_Map?.Element("prefabs");

        #endregion

        /// <summary>
        /// Are we currently saving or loading?
        /// </summary>
        private bool busy = false;

        /// <summary>
        /// This is the temporary <see cref="XDocument"/> that survives the map transition when loading a savegame.<para> </para>
        /// </summary>
        private XDocument transitorySavegame = null;

        /// <summary>
        /// Gathers all the <see cref="XmlSaveableComponent"/>s in the scene and registers them to the <see cref="XmlSavegameManager.saveableComponents"/> array.
        /// </summary>
        [ContextMenu("Gather all SaveableComponents in scene")]
        public void GatherSaveableComponentsInScene() => saveableComponents = FindObjectsOfType<XmlSaveableComponent>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            SpawnedPrefab.SPAWNED_PREFABS.Clear();
        }

        /// <summary>
        /// Saves the game out to a time-stamped savegame file inside the <see cref="SavegameManager.savegamesDirectoryPath"/> folder.
        /// </summary>
        /// <param name="fileName">The savegame's file name (WITHOUT EXTENSION!)</param>
        public override void Save(string fileName)
        {
            if (busy)
            {
                return;
            }

            busy = true;

            StartCoroutine(SaveCoroutine(fileName));
        }

        private IEnumerator SaveCoroutine(string fileName)
        {
            CheckSavegamesDirectory();

            OnSaving();

            yield return YieldInstructions.WaitForEndOfFrame;

            DateTime startUtc = DateTime.UtcNow;
            TimeSpan timeout = TimeSpan.FromSeconds(savingTimeoutSeconds);

            Scene scene = SceneManager.GetActiveScene();

            XE_Index.Value = scene.buildIndex.ToString();
            XE_Name.Value = scene.name;

            XE_Components.RemoveAll();

            ICollection<(int, XElement)> saveableComponentsXmlBuffer = new List<(int, XElement)>(saveableComponents.Length);
            ICollection<(string, XElement)> spawnedPrefabsXmlBuffer = new List<(string, XElement)>(batchSize);

            if (saveableComponents.Length != 0)
            {
                for (int i = 0; i < saveableComponents.Length; ++i)
                {
                    XmlSaveableComponent component = saveableComponents[i];

                    if (component == null)
                    {
                        continue;
                    }
                    
                    component.BeforeSaving();

                    saveableComponentsXmlBuffer.Add((component.ID, component.GetXml()));

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

            XE_Prefabs.RemoveAll();

            if (SpawnedPrefab.SPAWNED_PREFABS.Count != 0)
            {
                for (int i = 0; i < SpawnedPrefab.SPAWNED_PREFABS.Count; ++i)
                {
                    SpawnedPrefab prefab = SpawnedPrefab.SPAWNED_PREFABS[i];

                    if (prefab == null)
                    {
                        continue;
                    }

                    XElement xml = null;
                    
                    XmlSaveableComponent component = prefab.GetSaveableComponent() as XmlSaveableComponent;

                    if (component != null)
                    {
                        component.BeforeSaving();
                        
                        xml = component.GetXml();
                    }
                    
                    spawnedPrefabsXmlBuffer.Add((prefab.GetResourcePath(), xml));

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
                if (saveableComponentsXmlBuffer.Count != 0)
                {
                    XE_Components.Add(saveableComponentsXmlBuffer.Select(idXmlTuple => new XElement("component", new XAttribute("id", idXmlTuple.Item1), idXmlTuple.Item2)));
                }

                if (spawnedPrefabsXmlBuffer.Count != 0)
                {
                    XE_Prefabs.Add(spawnedPrefabsXmlBuffer.Select(resourcePathXmlTuple => new XElement("prefab",
                        new XElement("path", resourcePathXmlTuple.Item1),
                        new XElement("component", resourcePathXmlTuple.Item2))));
                }

                string filePath = Path.Combine(savegamesDirectoryPath, $"{fileName}_{DateTime.Now.ToString(TIME_FORMAT)}{savegameFileExtension}");

                if (!encrypt && !compress)
                {
                    savegame?.Save(filePath, indent ? SaveOptions.None : SaveOptions.DisableFormatting);
                }
                else
                {
                    using var memoryStream = new MemoryStream();

                    // Write the xml document to the memory stream.
                    savegame.Save(memoryStream, SaveOptions.DisableFormatting);

                    // Extract the bytes from the memory stream.
                    byte[] xmlBytes = memoryStream.ToArray();

                    if (compress)
                    {
                        xmlBytes = GZip.Compress(xmlBytes);
                    }

                    if (encrypt)
                    {
                        byte[] salt = new byte[64];
                        RandomNumberGenerator.Fill(salt);

                        xmlBytes = salt.Concat(SymmetricCryptography.Encrypt(xmlBytes, key, Convert.ToBase64String(salt), iterationsOfPBKDF2)).ToArray();
                    }

                    File.WriteAllBytes(filePath, xmlBytes);
                }
            });

            while (!task.IsCompleted)
            {
                yield return YieldInstructions.GetWaitForSecondsRealtime(64);

                if (DateTime.UtcNow - startUtc > timeout)
                {
                    OnSaveFailed();

                    yield return YieldInstructions.WaitForFixedUpdate;
                    busy = false;
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

        /// <summary>
        /// Loads a savegame file from inside the <see cref="SavegameManager.savegamesDirectoryPath"/>. <para> </para>
        /// The <paramref name="fileName"/> parameter is just the savegame's file name (local to the <see cref="SavegameManager.savegamesDirectoryPath"/> and WITHOUT THE FILE EXTENSION).<para> </para>
        /// This will cause a chain reaction of method calls and procedures in order to make the loading procedure
        /// and map transition as smooth as possible.<para> </para>
        /// The method registers <see cref="XmlSavegameManager.OnNewSceneLoaded(Scene, LoadSceneMode)"/>
        /// to the <see cref="SceneManager.sceneLoaded"/> event and then loads the savegame's stored scene. A scene transition is triggered.<para> </para>
        /// What happens next is that the <see cref="OnNewSceneLoaded(Scene, LoadSceneMode)"/> method is fired in the NEW scene on the OLD <see cref="XmlSavegameManager"/> instance.<para> </para>
        /// That method then passes its <see cref="XmlSavegameManager.transitorySavegame"/> document over to the new scene's <see cref="XmlSavegameManager"/>, and calls <see cref="Reconstruct"/> on it. The old scene's <see cref="XmlSavegameManager"/> can now die safely. 
        /// </summary>
        /// <param name="fileName">The savegame's file name (WITHOUT ITS EXTENSION!). Path is local to the <see cref="SavegameManager.savegamesDirectoryPath"/>.</param>
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
            if (fileName.NullOrEmpty())
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(XmlSavegameManager)}: Couldn't load savegame! The specified {nameof(fileName)} string is null or empty.");
#endif
                yield break;
            }

            CheckSavegamesDirectory();

            OnLoading();

            string filePath = Path.Combine(savegamesDirectoryPath, $"{fileName}{savegameFileExtension}");

            if (!File.Exists(filePath))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(XmlSavegameManager)}: Couldn't load savegame! The specified file path does not exist or is invalid.");
#endif
                yield break;
            }

            DateTime startUtc = DateTime.UtcNow;
            TimeSpan timeout = TimeSpan.FromSeconds(loadingTimeoutSeconds);

            var task = Task.Run(() =>
            {
                if (!encrypt && !compress)
                {
                    transitorySavegame = XDocument.Load(filePath);
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

                    using var memoryStream = new MemoryStream(data ?? Array.Empty<byte>());

                    transitorySavegame = XDocument.Load(memoryStream);
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
                    string sceneName = transitorySavegame.Root?.Element("map")?.Attribute("name")?.Value;

                    if (sceneName.NotNullNotEmpty())
                    {
#if UNITY_EDITOR
                        Debug.Log($"{nameof(XmlSavegameManager)}: Load savegame by scene name: {sceneName}");
#endif
                        SceneManager.sceneLoaded += OnNewSceneLoaded;
                        loadOperation = SceneManager.LoadSceneAsync(sceneName);
                    }
                }
                else
                {
                    if (int.TryParse(transitorySavegame.Root?.Element("map")?.FirstAttribute.Value, out int index))
                    {
#if UNITY_EDITOR
                        Debug.Log($"{nameof(XmlSavegameManager)}: Load savegame by scene build index: {index}");
#endif
                        SceneManager.sceneLoaded += OnNewSceneLoaded;
                        loadOperation = SceneManager.LoadSceneAsync(index);
                    }
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
                Debug.LogError($"{nameof(XmlSavegameManager)}: Failed to load savegame {filePath}");
#endif
            }
        }


        /// <summary>
        /// When a new map is loaded, this method is called on the old <see cref="XmlSavegameManager"/> and will
        /// pass its savegame <see cref="XDocument"/> to the newly loaded map's <see cref="XmlSavegameManager"/>.<para> </para>
        /// This way the new <see cref="XmlSavegameManager"/> is aware of the data it has to reconstruct. Once that's done,
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

            if (!int.TryParse(transitorySavegame?.Root?.Element("map")?.FirstAttribute.Value, out int index))
            {
                return;
            }

            if (index != scene.buildIndex)
            {
                return;
            }

            XmlSavegameManager newSavegameManager = FindObjectOfType<XmlSavegameManager>();

            if (newSavegameManager == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(XmlSavegameManager)}: In the newly loaded scene there is no {nameof(XmlSavegameManager)}; nothing to load the data with!");
#endif
                return;
            }

            // The savegame variable has to survive the map transition.
            // Therefore, we assign it to the new scene's XmlSavegameManager and load it up there. 
            // This manager here has served its purpose very well, and can now die in peace... RIP!
            newSavegameManager.transitorySavegame = this.transitorySavegame;
            newSavegameManager.Reconstruct();

            // Unregister and goodbye!
            SceneManager.sceneLoaded -= OnNewSceneLoaded;
        }

        /// <summary>
        /// This method loads the <see cref="XmlSavegameManager.transitorySavegame"/> variable that survived the map transition.<para> </para>
        /// It's the method that actually applies the reconstruction to the various <see cref="SaveableComponent"/>s in the newly loaded scene (and spawns any objects that were runtime-created on save).
        /// </summary>
        private void Reconstruct()
        {
            // Get the loaded savegame's root XElement.
            XElement root = transitorySavegame?.Root;

            if (root == null)
            {
                Debug.LogError($"{nameof(XmlSavegameManager)}: There was an error transitioning the transitory savegame document over to the new scene... Couldn't reconstruct scene successfully!");
                return;
            }

            try
            {
                /*  Here's a quick reminder of how the savegame xml tree looks:
                 
                    <savegame>
                        <map index="1" name="scene_name_here">
                            <components>
                                <component id="7">
                                    <!-- SaveableComponent data here -->
                                </component>
                                <!-- etc... -->
                            </components>
                            <prefabs>
                                <prefab>
                                    <path>resource_prefab_path_here</path>
                                    <component>
                                        <!-- SaveableComponent data here -->
                                    </component>
                                </prefab>
                                <!-- etc... -->
                            </prefabs>
                        </map>
                    </savegame>
                 */

                XElement map = root.Element("map");

                IDictionary<int, XElement> components = map
                    ?.Element("components")
                    ?.Elements("component")
                    ?.ToDictionary(c => int.TryParse(c?.FirstAttribute?.Value ?? "-1", out int i) ? i : -1) ?? new Dictionary<int, XElement>();

                IEnumerable<XElement> prefabs = map
                    ?.Element("prefabs")
                    ?.Elements("prefab");

                for (int i = saveableComponents.Length - 1; i >= 0; i--)
                {
                    try
                    {
                        XmlSaveableComponent component = saveableComponents[i];

                        if (component == null)
                        {
                            continue;
                        }

                        if (components.TryGetValue(component.ID, out XElement xml))
                        {
                            component.LoadXml(xml);
                            
                            component.AfterLoading();
                        }
                        else
                        {
                            Destroy(component.gameObject);
                        }
                    }
                    catch (Exception e)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"{nameof(XmlSavegameManager)}: Failed to deserialize {nameof(XmlSaveableComponent)} with ID = {saveableComponents[i].ID} from savegame while loading and reconstructing the scene. Thrown exception: {e}");
#endif
                    }
                }

                if (prefabs != null)
                {
                    foreach (XElement prefab in prefabs)
                    {
                        try
                        {
                            GameObject obj = Resources.Load<GameObject>(prefab.Element("path")?.Value);

                            if (obj != null)
                            {
                                obj = Instantiate(obj);

                                XmlSaveableComponent component = obj.GetComponent<XmlSaveableComponent>();

                                if (component != null)
                                {
                                    component.LoadXml(prefab.Element("component"));
                                    
                                    component.AfterLoading();
                                }
                            }
#if UNITY_EDITOR
                            else
                            {
                                Debug.LogError($"{nameof(XmlSavegameManager)}: the Resources path \"{prefab.Element("path")?.Value}\" doesn't exist and thus couldn't be instantiated on load.");
                            }
#endif
                        }
                        catch (Exception e)
                        {
#if UNITY_EDITOR
                            Debug.LogError($"{nameof(XmlSavegameManager)}: Failed to spawn prefab with resource path = {prefab.Element("path")?.Value} from savegame while loading and reconstructing the scene. Thrown exception: {e}");
#endif
                        }
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
            catch (Exception e)
            {
                Debug.LogError($"{nameof(XmlSavegameManager)}: Savegame loading procedure failed; error message: {e}");
            }

            transitorySavegame = null;
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com