using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GlitchedPolygons.ExtensionMethods;
using GlitchedPolygons.Utilities;
using UnityEngine;

namespace GlitchedPolygons.SavegameFramework
{
    /// <summary>
    /// Abstract base class for all savegame manager implementations.<para> </para> 
    /// There can be many, such as XML based ones, others that use JSON, or even ProtoBuf, YAML, msgpack, well... the possibilities are endless.<para> </para>
    /// They all share the same raw and basic functionalities defined in here though.
    /// </summary>
    public abstract class SavegameManager : MonoBehaviour
    {
        #region Constants

        /// <summary>
        /// The <see cref="DateTime"/> format used for the savegame timestamps.
        /// </summary>
        protected const string TIME_FORMAT = "yyyyMMddHHmmssff";

        #endregion

        #region Events

        /// <summary>
        /// This event is raised whenever the user initiated a saving procedure
        /// (started but not yet done; use this to show some UI label like a spinning disc or something to notify the user that saving is in progress).
        /// </summary>
        public event Action Saving;

        /// <summary>
        /// This event is raised whenever the user saved the game (provided that the procedure was successful).
        /// </summary>
        public event Action Saved;

        /// <summary>
        /// Raised if a saving procedure failed.
        /// </summary>
        public event Action SaveFailed;

        /// <summary>
        /// Raised when the player initiated a load procedure (started but not yet done; use this to show some UI label like a spinning disc or something to notify the user that loading is in progress).
        /// </summary>
        public event Action Loading;

        /// <summary>
        /// This event is raised when a savegame has been loaded (successfully!).
        /// </summary>
        public event Action Loaded;

        /// <summary>
        /// Raised if a loading procedure failed.
        /// </summary>
        public event Action LoadFailed;

        /// <summary>
        /// Raised whenever the <see cref="SavegameManager"/> instance is ready again after a saving or loading procedure finished successfully.<para> </para>
        /// Use this to e.g. reenable the UI buttons for saving/loading.
        /// </summary>
        public event Action Ready;

        #endregion

        #region Inspector values

        /// <summary>
        /// Savegame encryption key.
        /// </summary>
        [SerializeField]
        [Header("SavegameManager base class fields:\n")]
        [Tooltip("The key used to decrypt the savegames.\n\nPlease never leave this empty and FOR HEAVEN'S SAKE NEVER LOSE IT!")]
        protected string key = null;

        /// <summary>
        /// Encrypt the savegame before writing it out to disk.
        /// </summary>
        [SerializeField]
        [Tooltip("Should the savegame be encrypted using the above specified key?")]
        protected bool encrypt = false;

        /// <summary>
        /// Compress the savegame using <see cref="GZip"/>.
        /// </summary>
        [SerializeField]
        [Tooltip("Should the savegame be gzipped before being written to/loaded from disk?")]
        protected bool compress = true;

        /// <summary>
        /// How many rounds of PBKDF2 should be applied to <see cref="key"/> in order to derive the AES key from it?<para> </para>
        /// Once decided, DO NOT CHANGE THIS value or you'll be unable to decrypt your data unless you save the used iteration count along with the data you're decrypting!
        /// </summary>
        [SerializeField]
        [Tooltip("How many rounds of PBKDF2 should be applied to the AES key for derivation. Once decided, DO NOT CHANGE THIS value or you'll be unable to decrypt your data unless you save the used iteration count along with the data you're decrypting!")]
        protected int iterationsOfPBKDF2 = 32;

        /// <summary>
        /// Collect generated garbage after saving?
        /// </summary>
        [SerializeField]
        [Tooltip("Should an instant garbage collection be triggered after saving the game?\nThis can be of advantage in scenarios where big savegames are generated, since those allocate huge amounts of memory (unavoidable due to the way serialization works; strings are always created and needed).\nThis is particularly relevant if you follow the \"collect often, collect small chunks\" convention of keeping a small heap.\nContinuously letting Unity expand the heap WILL trigger a collection sometime during gameplay and cause a stutter; therefore, collecting frequently and maintaining that heap as small as possible can make or break your game's performance.")]
        protected bool collectGarbageOnSave = true;

        /// <summary>
        /// <see cref="GC.Collect()"/> after loading a savegame?
        /// </summary>
        [SerializeField]
        [Tooltip("Should an instant garbage collection be triggered after loading a savegame? For more information, check out the \"collectGarbageOnSave\" field's tooltip above.")]
        protected bool collectGarbageOnLoad = true;

        /// <summary>
        /// This will be the name of the folder inside <c>Application.persistentDataPath</c> where all savegames will be saved to and loaded from.
        /// </summary>
        [SerializeField]
        protected string savegamesDirectoryName = "Savegames";

        /// <summary>
        /// File extension to use for savegames.
        /// </summary>
        [SerializeField]
        protected string savegameFileExtension = ".sav";

        /// <summary>
        /// After how many seconds of saving should the procedure be aborted and considered a failure?
        /// </summary>
        [SerializeField]
        [Range(1, 300)]
        protected int savingTimeoutSeconds = 20;

        /// <summary>
        /// After how many seconds of loading attempt should the procedure be aborted and considered a failure?
        /// </summary>
        [SerializeField]
        [Range(1, 300)]
        protected int loadingTimeoutSeconds = 20;

        /// <summary>
        /// How many objects should be batched during saving / loading per frame.
        /// </summary>
        /// <remarks>
        /// E.g. a value of 256 in a big scene would spread the serialization of all the objects over multiple frames: every 256 processed objects,
        /// <c>yield return YieldInstructions.WaitForEndOfFrame;</c> is called! Put a lot of thought into what value to use here,
        /// because you don't want to brick the game during saving but also not spread the saving procedure over multiple seconds, causing state inconsistencies!
        /// </remarks>
        [SerializeField]
        [Range(16, 4096)]
        protected int batchSize = 256;

        #endregion

        /// <summary>
        /// Saves the game out to a time-stamped savegame file inside the <see cref="SavegameManager.savegamesDirectoryPath"/> folder.
        /// </summary>
        /// <param name="fileName">The savegame's file name (WITHOUT EXTENSION!)</param>
        public abstract void Save(string fileName);

        /// <summary>
        /// Saves the game out to a <paramref name="destinationStream"/>.
        /// </summary>
        /// <remarks>The target output <see cref="Stream"/> will be closed and disposed by this method, so do NOT use it afterwards. A <c>using</c> statement is therefore not needed!</remarks>
        /// <seealso cref="Stream"/>
        /// <param name="destinationStream">The <see cref="Stream"/> to write the savegame into. This will be closed and disposed by this method, so make sure not to use it anymore afterwards!</param>
        public abstract void Save(Stream destinationStream);

        /// <summary>
        /// Loads a savegame file from inside the <see cref="SavegameManager.savegamesDirectoryPath"/>. <para> </para>
        /// The <paramref name="fileName"/> parameter is just the savegame's file name (local to the <see cref="SavegameManager.savegamesDirectoryPath"/> and WITHOUT THE FILE EXTENSION).<para> </para>
        /// This will cause a chain reaction of method calls and procedures in order to make the loading procedure
        /// and map transition as smooth as possible.<para> </para>
        /// Check out the documentation of the implementing child classes to find out more about how it works (e.g. <see cref="GlitchedPolygons.SavegameFramework.Xml.XmlSavegameManager"/>).
        /// </summary>
        /// <param name="fileName">The savegame's file name (WITHOUT ITS EXTENSION!). Path is local to the <see cref="SavegameManager.savegamesDirectoryPath"/>.</param>
        /// <seealso cref="GlitchedPolygons.SavegameFramework.Xml.XmlSavegameManager"/>
        /// <seealso cref="GlitchedPolygons.SavegameFramework.Json.JsonSavegameManager"/>
        public abstract void Load(string fileName);

        /// <summary>
        /// Loads a savegame from a <see cref="Stream"/>.<para> </para>
        /// This will cause a chain reaction of method calls and procedures in order to make the loading procedure
        /// and map transition as smooth as possible.<para> </para>
        /// Check out the documentation of the implementing child classes to find out more about how it works (e.g. <see cref="GlitchedPolygons.SavegameFramework.Xml.XmlSavegameManager"/>).
        /// </summary>
        /// <remarks>The source input <see cref="Stream"/> will be closed and disposed by this method, so do NOT use it afterwards. A <c>using</c> statement is therefore not needed!</remarks>
        /// <param name="sourceStream">The <see cref="Stream"/> to read the savegame data from.</param>
        public abstract void Load(Stream sourceStream);

        /// <summary>
        /// Loads the most recent savegame file automatically.
        /// </summary>
        public void LoadNewestSavegame()
        {
            Load(GetNewestSavegame());
        }
        
        /// <summary>
        /// This is the full path to the directory that contains the savegames.
        /// </summary>
        protected string savegamesDirectoryPath;

        /// <summary>
        /// Invokes the <see cref="Saved"/> event.
        /// </summary>
        protected void OnSave()
        {
            Saved?.Invoke();
        }

        /// <summary>
        /// Invokes the <see cref="Loaded"/> event.
        /// </summary>
        protected void OnLoad()
        {
            Loaded?.Invoke();
        }

        /// <summary>
        /// Invokes the <see cref="Saving"/> event.
        /// </summary>
        protected void OnSaving()
        {
            Saving?.Invoke();
        }

        /// <summary>
        /// Invokes the <see cref="Loading"/> event.
        /// </summary>
        protected void OnLoading()
        {
            Loading?.Invoke();
        }

        /// <summary>
        /// Invokes the <see cref="SaveFailed"/> event.
        /// </summary>
        protected void OnSaveFailed()
        {
            SaveFailed?.Invoke();
        }

        /// <summary>
        /// Invokes the <see cref="LoadFailed"/> event.
        /// </summary>
        protected void OnLoadFailed()
        {
            LoadFailed?.Invoke();
        }

        /// <summary>
        /// Invokes the <see cref="Ready"/> event.
        /// </summary>
        protected void OnReady()
        {
            Ready?.Invoke();
        }

        /// <summary>
        /// Checks if the savegames directory exists and creates one if not.
        /// </summary>
        /// <returns>True if the savegame directory existed; false if it had to be created.</returns>
        protected bool CheckSavegamesDirectory()
        {
            if (string.IsNullOrEmpty(savegamesDirectoryPath))
            {
                savegamesDirectoryPath = Path.Combine(Application.persistentDataPath, savegamesDirectoryName);
            }

            if (Directory.Exists(savegamesDirectoryPath))
            {
                return true;
            }

            Directory.CreateDirectory(savegamesDirectoryPath);
            return false;
        }

        /// <summary>
        /// Gets the newest, most recent savegame file name; without its extension and local to the <see cref="savegamesDirectoryPath"/>,
        /// such that it can be loaded seamlessly into a savegame manager's load function.
        /// </summary>
        protected string GetNewestSavegame()
        {
            CheckSavegamesDirectory();

            FileInfo newestSavegameFile = new DirectoryInfo(savegamesDirectoryPath).GetNewestFile($"*{savegameFileExtension}");
            
            return newestSavegameFile != null
                ? Path.GetFileNameWithoutExtension(newestSavegameFile.Name)
                : string.Empty;
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com