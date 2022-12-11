using UnityEngine;

namespace GlitchedPolygons.Damage
{
    /// <summary>
    /// Event arguments for the <see cref="Damageable.Damaged"/> event.
    /// </summary>
    public struct Damage
    {
        /// <summary>
        /// The amount of damage that was applied.
        /// </summary>
        public float damageAmount;

        /// <summary>
        /// The type of damage that was applied.
        /// </summary>
        public DamageType damageType;

        /// <summary>
        /// If the damage was due to a bullet impact, this is its RaycastHit.
        /// </summary>
        public RaycastHit damageBulletRaycastHit;

        public Damage(float damage, DamageType damageType, RaycastHit hit)
        {
            damageAmount = damage;
            damageBulletRaycastHit = hit;
            this.damageType = damageType;
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com

