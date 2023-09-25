using QuickTurret.DamageSystem;
using QuickTurret.TargetingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.Mobs
{
    public class ContainozoidSphere : MonoBehaviour
    {
        Hurtable myHurtable;
        GameObject lastDamageSource;

        public GameObject Containozoid;

        DamageEffect OnDeathDamage = new DamageEffect()
        {
            Damage = 50,
            Type = DamageEffect.DamageType.Critical | DamageEffect.DamageType.ArmourPiercing,
        };

        private void Awake()
        {
            myHurtable = GetComponent<Hurtable>();
        }

        private void Start()
        {
            myHurtable.OnDie += HandleOnDieEvent;
            myHurtable.OnHurt += HandleOnHurtEvent;
        }

        private void OnDestroy()
        {
            myHurtable.OnDie -= HandleOnDieEvent;
            myHurtable.OnHurt -= HandleOnHurtEvent;
        }

        public void HandleOnDieEvent(object sender, Hurtable.OnDieEventArgs onDieEventArgs)
        {
            if (Containozoid != null)
            {
                Containozoid.GetComponent<Hurtable>().Hurt(lastDamageSource, OnDeathDamage);
            }
        }

        public void HandleOnHurtEvent(object sender, Hurtable.OnHurtEventArgs onHurtEventArgs)
        {
            lastDamageSource = onHurtEventArgs.Source;
        }
    }
}
