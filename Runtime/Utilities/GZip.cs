using System;
using System.Text;

using System.IO;
using System.IO.Compression;

using Debug = UnityEngine.Debug;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// This class provides functionality for compressing and decompressing strings and raw <c>byte[]</c> arrays using the <see cref="GZipStream"/>.
    /// </summary>
    public static class GZip
    {
        /// <summary>
        /// The default <see cref="Encoding"/> to use for compressing/decompressing strings.
        /// </summary>
        private static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;

        /// <summary>
        /// Empty byte array for handling certain edge cases/failures (e.g. compressing an empty array will result in an empty array).
        /// </summary>
        private static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];

        /// <summary>
        /// Compresses the specified bytes using <see cref="GZipStream"/>.
        /// </summary>
        /// <param name="bytes">The <c>byte[]</c> array to compress.</param>
        /// <param name="bufferSize">The size of the underlying stream buffer. Try to give a rough estimate of how many bytes you'll need...</param>
        /// <param name="compressionLevel">Choose the desired <see cref="CompressionLevel"/>. The default value favors speed over efficiency (<see cref="CompressionLevel.Fastest"/>).</param>
        /// <returns>The compressed <c>byte[]</c> array.</returns>
        public static byte[] Compress(byte[] bytes, int bufferSize = 4096, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            #region _OLD

            // Old, pre .NET 4.6 legacy code
            // Only use this if you are forcefully bound to a prior version of the .NET framework!

            //using (var memoryStream = new MemoryStream())
            //{
            //    using (var gzStream = new GZipStream(memoryStream, CompressionMode.Compress))
            //    {
            //        gzStream.Write(bytes, 0, bytes.Length);
            //    }
            //    return memoryStream.ToArray();
            //}

            #endregion

            if (ReferenceEquals(bytes, null))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(GZip)}: You tried to compress a null array; returning null...");
#endif
                return null;
            }

            if (bytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(GZip)}: You tried to compress an empty array; the resulting array will also be empty!");
#endif
                return EMPTY_BYTE_ARRAY;
            }

            byte[] compressedBytes;

            using (var compressedStream = new MemoryStream())
            {
                using (var originalStream = new MemoryStream(bytes))
                {
                    using (var gzip = new GZipStream(compressedStream, compressionLevel))
                    {
                        originalStream.CopyTo(gzip, bufferSize);
                    }
                }
                compressedBytes = compressedStream.ToArray();
            }

            return compressedBytes;
        }

        /// <summary>
        /// Compresses the specified string with the <see cref="GZipStream"/> and the <see cref="DEFAULT_ENCODING"/>.
        /// </summary>
        /// <param name="text">The string to compress.</param>
        /// <returns>The compressed (gzipped) string.</returns>
        public static string Compress(string text)
        {
            return Convert.ToBase64String(Compress(DEFAULT_ENCODING.GetBytes(text), text.Length));
        }

        /// <summary>
        /// Compresses the specified string with <see cref="GZipStream"/> using a specific <see cref="Encoding"/>. 
        /// </summary>
        /// <returns>The compressed string.</returns>
        /// <param name="text">String to compress.</param>
        /// <param name="encoding">Encoding.</param>
        public static string Compress(string text, Encoding encoding)
        {
            return Convert.ToBase64String(Compress(encoding.GetBytes(text), text.Length));
        }

        /// <summary>
        /// Decompresses the specified bytes.
        /// </summary>
        /// <returns>The decompressed bytes.</returns>
        /// <param name="gzippedBytes">The gzipped bytes to decompress.</param>
        /// <param name="bufferSize">The size of the underlying stream buffer. Try to give a rough estimate of how many bytes you'll need...</param>
        public static byte[] Decompress(byte[] gzippedBytes, int bufferSize = 4096)
        {
            #region _OLD

            // Old, pre .NET 4.6 legacy code. 
            // Only use this if you are forcefully bound to a prior version of the .NET framework!

            //using (var outputStream = new MemoryStream())
            //using (var inputStream = new MemoryStream(gzippedBytes))
            //using (var gzStream = new GZipStream(inputStream, CompressionMode.Decompress))
            //{
            //    const int SIZE = 4096;
            //    byte[] buffer = new byte[SIZE];

            //    int count = 0;
            //    do
            //    {
            //        count = gzStream.Read(buffer, 0, SIZE);
            //        if (count > 0)
            //        {
            //            outputStream.Write(buffer, 0, count);
            //        }
            //    }
            //    while (count > 0);

            //    return outputStream.ToArray();
            //}

            #endregion

            if (ReferenceEquals(gzippedBytes, null))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(GZip)}: You tried to decompress a null array; returning null...");
#endif
                return null;
            }

            if (gzippedBytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(GZip)}: You tried to decompress an empty array; the resulting array will also be empty!");
#endif
                return EMPTY_BYTE_ARRAY;
            }

            using (MemoryStream decompressedStream = new MemoryStream())
            {
                using (MemoryStream compressedStream = new MemoryStream(gzippedBytes))
                {
                    using (GZipStream gzip = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        gzip.CopyTo(decompressedStream, bufferSize);
                    }
                }

                return decompressedStream.ToArray();
            }
        }

        /// <summary>
        /// Decompresses the specified gzipped string using the <see cref="DEFAULT_ENCODING"/>.
        /// </summary>
        /// <param name="gzippedString">The gzipped string to decompress.</param>
        /// <returns>The decompressed string.</returns>
        public static string Decompress(string gzippedString)
        {
            return DEFAULT_ENCODING.GetString(Decompress(Convert.FromBase64String(gzippedString)));
        }

        /// <summary>
        /// Decompresses the specified gzipped string using a specific <see cref="Encoding"/>.
        /// </summary>
        /// <returns>The decompressed string.</returns>
        /// <param name="gzippedString">The gzipped string to decompress.</param>
        /// <param name="encoding">Encoding.</param>
        public static string Decompress(string gzippedString, Encoding encoding)
        {
            return encoding.GetString(Decompress(Convert.FromBase64String(gzippedString)));
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
