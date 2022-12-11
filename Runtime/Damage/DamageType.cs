namespace GlitchedPolygons.Damage
{
    /// <summary>
    /// Various types of damage that can be applied.<para> </para>
    /// <see cref="DamageType.Silent"/> can be used for saving/loading the game
    /// (it's supposed to modify an object's health without any special effects, sounds or anything).
    /// </summary>
    public enum DamageType
    {
        /// <summary>
        /// Use for silently applying damage (e.g. when loading/reconstructing a game session).
        /// </summary>
        Silent,

        /// <summary>
        /// Nothing special, just damage.
        /// </summary>
        Generic,

        /// <summary>
        /// Bullet.
        /// </summary>
        Bullet,

        /// <summary>
        /// Ellehu ekber? ;D
        /// </summary>
        Explosion,

        /// <summary>
        /// Fire damage.
        /// </summary>
        Fire,

        /// <summary>
        /// Freezing frost damage.
        /// </summary>
        Frost,

        /// <summary>
        /// Drown damage.
        /// </summary>
        Drown,

        /// <summary>
        /// Water induced damage (most probably due to physical impact, such as a violent fall into 
        /// the water from a dangerous height, tsunami, etc...).
        /// </summary>
        Water,

        /// <summary>
        /// Neurotoxin of any kind (e.g. nervegas, lethal injection, etc...)
        /// </summary>
        Neurotoxin,

        /// <summary>
        /// Any sort of poison.
        /// </summary>
        Poison,

        /// <summary>
        /// Radioactivity related damage.
        /// </summary>
        Radiation,

        /// <summary>
        /// Corrosion damage.
        /// </summary>
        Acid,

        /// <summary>
        /// Plasma damage (laser rifles?).
        /// </summary>
        Plasma,

        /// <summary>
        /// Gravity-related damage.
        /// </summary>
        Falldamage,

        /// <summary>
        /// Electrical shock damage.
        /// </summary>
        Electricity,

        /// <summary>
        /// Physical impact (e.g. being hit by props, heavy objects, etc...).
        /// </summary>
        Physics,

        /// <summary>
        /// Blunt weapon damage (sticks, clubs, baseball bats, hammers, etc...).
        /// </summary>
        Blunt
    }
}

// Copyright (C) Raphael Beck, 2016 | https://glitchedpolygons.com
