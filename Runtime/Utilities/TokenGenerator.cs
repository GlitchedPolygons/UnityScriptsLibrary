using System.Text;
using System.Security.Cryptography;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Random string token generator.
    /// </summary>
    public static class TokenGenerator
    {
        private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        
        private static readonly StringBuilder STRING_BUILDER = new (8);

        /**
         * Generates a new string token string.
         * <param name="length">[OPTIONAL] Amount of characters in the token. Increase this if you're experiencing token collisions often.</param>
         * <param name="alphabet">[OPTIONAL] Custom alphabet of characters to use for generating the token.</param>
         */
        public static string GenerateToken(int length = 4, string alphabet = null)
        {
            if (length <= 0 || (alphabet is not null && alphabet.Length == 0))
            {
                return string.Empty;
            }
            
            STRING_BUILDER.Clear();

            for (int i = length - 1; i >= 0; --i)
            {
                char randomCharacter = (alphabet ?? ALPHABET)[RandomNumberGenerator.GetInt32(0, alphabet?.Length ?? ALPHABET.Length)];
                
                STRING_BUILDER.Append(randomCharacter);
            }

            return STRING_BUILDER.ToString();
        }
    }
}

// Copyright (C) Raphael Beck, 2022 | https://glitchedpolygons.com