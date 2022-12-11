using System;
using System.Text;
using System.Security.Cryptography;

using Debug = UnityEngine.Debug;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Cryptoclass useful for easily encrypting/decrypting stuff.
    /// </summary>
    public static class SymmetricCryptography
    {
        /// <summary>
        /// Limits the maximum amount of characters in a salt string.
        /// </summary>
        private const int MAX_SALT_SIZE = 256;

        /// <summary>
        /// Cryptographic initial vector.
        /// </summary>
        private static readonly byte[] IV = new byte[16];

        /// <summary>
        /// Empty <c>byte[]</c> array for handling certain edge cases/failures (e.g. encrypting/decrypting an empty array will result in an empty array).
        /// </summary>
        private static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];

        /// <summary>
        /// Encrypt data using a specific key and salt string.
        /// </summary>
        /// <param name="data">The data you want to encrypt.</param>
        /// <param name="key">The cryptographic key with which the data should be encrypted. Make this at least 32 characters long!</param>
        /// <param name="salt">The crypto salt to use for key derivation (should also be >32 characters long).</param>
        /// <param name="rfcIterations">The amount of iterations that the <see cref="Rfc2898DeriveBytes"/> algo should perform to derive the key.</param>
        /// <returns>The encrypted bytes.</returns>
        public static byte[] Encrypt(byte[] data, string key, string salt, int rfcIterations)
        {
            if (salt.Length > MAX_SALT_SIZE)
            {
                Debug.LogError($"{nameof(SymmetricCryptography)}: {nameof(salt)} parameter too big (exceeds maximum length of {MAX_SALT_SIZE}). Procedure aborted for performance reasons; returning null...");
                return null;
            }

            if (data.Length == 0)
            {
                Debug.LogError($"{nameof(SymmetricCryptography)}: {nameof(data)} array is empty; nothing to encrypt! Returning empty array...");
                return EMPTY_BYTE_ARRAY;
            }

            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            try
            {
                using (var aes = new AesManaged())
                using (var rfc = new Rfc2898DeriveBytes(key, saltBytes, rfcIterations))
                {
                    // Assign AES key & IV.
                    aes.Key = rfc.GetBytes(32);
                    
                    for (int i = IV.Length - 1; i >= 0; i--)
                    {
                        IV[i] = saltBytes[i];
                    }

                    aes.IV = IV;

                    // Encrypt data.
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    {
                        data = encryptor.TransformFinalBlock(data, 0, data.Length);
                    }
                }

                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(SymmetricCryptography)}: data encryption failed! Returning null... Error message: {e}");
                return null;
            }
            finally
            {
                ClearIV();
            }
        }

        /// <summary>
        /// Decrypts an encrypted <c>byte[]</c> array using the specified key, salt and rfc iterations.<para> </para>
        /// Note: the decryption can only succeed if you use the EXACT same key, salt and amount of rfc iterations; so make sure to NEVER lose those values!<para> </para>
        /// If the decryption fails, the original input data is returned unmodified.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="key">The decryption key.</param>
        /// <param name="salt">The salt that was used to encrypt the data,</param>
        /// <param name="rfcIterations">The amount of rfc iterations that were used to encrypt the data.</param>
        /// <returns>The decrypted data, but if the decryption failed in some way, then the original input data is returned unmodified.</returns>
        public static byte[] Decrypt(byte[] data, string key, string salt, int rfcIterations)
        {
            if (salt.Length > MAX_SALT_SIZE)
            {
                Debug.LogError($"{nameof(SymmetricCryptography)}: {nameof(salt)} parameter too big (exceeds maximum length of {MAX_SALT_SIZE}). Procedure aborted for performance reasons; returning null.");
                return null;
            }

            if (data.Length == 0)
            {
                Debug.LogError($"{nameof(SymmetricCryptography)}: {nameof(data)} array is empty; nothing to decrypt! Returning empty array...");
                return EMPTY_BYTE_ARRAY;
            }

            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            try
            {
                using (AesManaged aes = new AesManaged())
                using (var rfc = new Rfc2898DeriveBytes(key, saltBytes, rfcIterations))
                {
                    aes.Key = rfc.GetBytes(32);

                    for (int i = IV.Length - 1; i >= 0; i--)
                    {
                        IV[i] = saltBytes[i];
                    }

                    aes.IV = IV;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        data = decryptor.TransformFinalBlock(data, 0, data.Length);
                    }
                }

                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(SymmetricCryptography)}: data decryption failed! Returning null... Error message: {e}");
                return null;
            }
            finally
            {
                ClearIV();
            }
        }

        /// <summary>
        /// Clears the <see cref="IV"/> by setting all its array elements to <c>0</c>.
        /// </summary>
        private static void ClearIV()
        {
            for (int i = IV.Length - 1; i >= 0; i--)
            {
                IV[i] = 0;
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
