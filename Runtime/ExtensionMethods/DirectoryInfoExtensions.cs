using System;
using System.IO;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// <see cref="DirectoryInfo"/> extension methods.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Deletes the specified directory recursively,
        /// including all of its sub-directories and files.<para> </para>
        /// Careful with this, ffs!
        /// </summary>
        /// <param name="dir">The directory to delete.</param>
        public static void DeleteRecursively(this DirectoryInfo dir)
        {
            if (dir is null || !dir.Exists)
            {
                return;
            }

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                DeleteRecursively(subDir);
                subDir.Delete();
            }
        }

        /// <summary>
        /// Gets the most recent file from a <see cref="DirectoryInfo"/>.
        /// The file's last write time is used for comparison.
        /// </summary>
        /// <param name="dir">The directory to scan.</param>
        /// <param name="searchPattern">Search pattern (e.g. <c>*.sav</c>)</param>
        /// <returns><c>null</c> if the <paramref name="dir"/> does not exist on disk or is empty; the newest <see cref="FileInfo"/> otherwise.</returns>
        public static FileInfo GetNewestFile(this DirectoryInfo dir, string searchPattern)
        {
            if (dir is null || !dir.Exists)
            {
                return null;
            }
            
            FileInfo[] files = dir.GetFiles(searchPattern);
            if (files.Length == 0)
            {
                return null;
            }

            DateTime lastWriteTime = DateTime.MinValue;
            FileInfo lastWrittenFile = null;

            for (int i = files.Length - 1; i >= 0; --i)
            {
                FileInfo file = files[i];
                if (file.LastWriteTime <= lastWriteTime)
                {
                    continue;
                }

                lastWrittenFile = file;
                lastWriteTime = file.LastWriteTime;
            }

            return lastWrittenFile;
        }
    }
}

// Copyright (C) Raphael Beck, 2022 | https://glitchedpolygons.com