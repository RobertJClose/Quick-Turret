using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.DamageSystem
{
    public class Hurtable : MonoBehaviour
    {
        private float armour;
        private float health;

        public HurtableStats Stats;

        public event System.EventHandler<OnDieEventArgs> OnDie;

        public event System.EventHandler<OnHurtEventArgs> OnHurt;

        public class OnDieEventArgs : System.EventArgs
        {

        }

        public class OnHurtEventArgs : System.EventArgs
        {
            public readonly DamageEffect OriginalDamage;
            public readonly DamageEffect FinalDamage;
            public readonly float RemainingHealth;
            public readonly GameObject Source;
            
            public OnHurtEventArgs(DamageEffect originalDamage, DamageEffect finalDamage, float remainingHealth, GameObject source)
            {
                OriginalDamage = originalDamage;
                FinalDamage = finalDamage;
                RemainingHealth = remainingHealth;
                Source = source;
            }
        }

        private void Awake()
        {
            armour = Stats.Armour;
            health = Stats.MaxHealth;
        }

        public void Hurt(GameObject source, DamageEffect damageEffect)
        {
            DamageEffect finalDamageEffect = CalculateDamage(damageEffect);
            health -= finalDamageEffect.Damage;

            OnHurtEventArgs onHurtEventArgs = new OnHurtEventArgs(damageEffect, finalDamageEffect, health, source);
            RaiseOnHurt(onHurtEventArgs);

            if (health < 0)
                Die();
        }

        private DamageEffect CalculateDamage(DamageEffect damageEffect)
        {
            float originalDamage = damageEffect.Damage;

            // First calculate damage reduction due to armour.
            // Armour piercing and bleed damage ignores armour.
            float damageReduction = armour;
            DamageEffect.DamageType testForArmourPiercing = damageEffect.Type & DamageEffect.DamageType.ArmourPiercing;

            if (testForArmourPiercing == DamageEffect.DamageType.ArmourPiercing)
                damageReduction = 0f;

            float finalDamage = originalDamage - damageReduction;

            // Ensure that the damage after reduction is at least one.
            finalDamage = Mathf.Max(finalDamage, 1);

            // If the damage is critical, we double the damage.
            DamageEffect.DamageType testForCritical = damageEffect.Type & DamageEffect.DamageType.Critical;
            if (testForCritical == DamageEffect.DamageType.Critical)
                finalDamage *= 2;

            DamageEffect finalEffect = new DamageEffect()
            {
                Damage = finalDamage,
                Type = damageEffect.Type,
            };

            // If the final damage was less than half of the original damage,
            // we add the "Blocked" type to the damage.
            if (finalDamage <= 0.5f * originalDamage)
                finalEffect.Type |= DamageEffect.DamageType.Blocked;

            return finalEffect;
        }

        private void Die()
        {
            OnDieEventArgs onDieEventArgs = new OnDieEventArgs();
            RaiseOnDie(onDieEventArgs);

            Destroy(gameObject);
        }

        protected virtual void RaiseOnDie(OnDieEventArgs e)
        {
            System.EventHandler<OnDieEventArgs> raiseEvent = OnDie;

            if (raiseEvent != null)
                raiseEvent(this, e);
        }

        protected virtual void RaiseOnHurt(OnHurtEventArgs e)
        {
            System.EventHandler<OnHurtEventArgs> raiseEvent = OnHurt;

            if (raiseEvent != null)
                raiseEvent(this, e);
        }

    }
}

