using System;
using System.IO;
using System.Text;

namespace GlitchedPolygons.ExtensionMethods
{
    /// <summary>
    /// <see cref="StringBuilder"/> extension methods.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Writes the <see cref="StringBuilder"/>'s characters out to a <see cref="FileStream"/>, <c>byte</c> by <c>byte</c> (in a loop).
        /// Slightly more efficient than <see cref="File.WriteAllText"/> and allocates less garbage.<para> </para>
        /// Uses <see cref="FileMode.Create"/> and <see cref="FileAccess.Write"/> for the <see cref="FileStream"/>.
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/> to write into a file on disk.</param>
        /// <param name="filePath">The destination file path. Make sure that the target directory exists before calling this!</param>
        public static void WriteToFile(this StringBuilder stringBuilder, string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            
            for (int i = 0; i < stringBuilder.Length; ++i)
            {
                fs.WriteByte(Convert.ToByte(stringBuilder[i]));
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
