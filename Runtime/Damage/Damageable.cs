using System;
using UnityEngine;
using UnityEngine.Events;

namespace GlitchedPolygons.Damage
{
    /// <summary>
    /// Damage distribution hub.<para> </para>
    ///
    /// This component distributes damage that's been applied via the <see cref="ApplyDamage"/> method to all interested subscribers of the <see cref="Damaged"/> event.<para> </para>
    ///
    /// Remember that for each event subscription you should always have an equivalent unsubscription (e.g. subscribe in OnEnable and unsubscribe in OnDisable) to prevent leaving events packed with "orphan" listeners after the object's destruction.
    /// </summary>
    public class Damageable : MonoBehaviour
    {
        /// <summary>
        /// This event is raised when <see cref="ApplyDamage"/> is called.<para> </para>
        ///
        /// Subscribe any interested damage receivers to this event. Damage amount, type and eventual bullet <a href="https://docs.unity3d.com/ScriptReference/RaycastHit.html">RaycastHit</a> are passed as event arguments (<see cref="Damage"/>).<para> </para>
        /// </summary>
        public event Action<Damage> Damaged;

        [SerializeField]
        private UnityEvent onDamaged;

        /// <summary>
        /// Invokes the <see cref="Damaged"/> event with the specified <see cref="Damage"/>.
        /// </summary>
        /// <param name="damageEventArgs">The damage related event arguments.</param>
        protected void OnDamaged(Damage damageEventArgs)
        {
            Damaged?.Invoke(damageEventArgs);
        }

        /// <summary>
        /// Apply damage to this <see cref="Damageable"/> object.<para> </para>
        /// 
        /// All damage receiving entities who are subscribed to the <see cref="Damaged"/> event will perceive it.  
        /// </summary>
        /// <param name="damageAmount">The amount of damage that should be applied.</param>
        /// <param name="damageType">The type of damage that should be applied.</param>
        /// <param name="bulletHit">If the <see cref="DamageType"/> is <see cref="DamageType.Bullet"/>, then pass in the bullet's <a href="https://docs.unity3d.com/ScriptReference/RaycastHit.html">RaycastHit</a>.<para> </para>
        ///If a <a href="https://docs.unity3d.com/ScriptReference/RaycastHit.html">RaycastHit</a> doesn't make sense with the specified <see cref="DamageType"/>, then pass <c>default(RaycastHit)</c> as a parameter.</param>
        public void ApplyDamage(float damageAmount, DamageType damageType, RaycastHit bulletHit)
        {
            // Invoke the public event.
            OnDamaged(new Damage(damageAmount, damageType, bulletHit));

            // Invoke the UnityEvent (for inspector hookups).
            onDamaged?.Invoke();
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com