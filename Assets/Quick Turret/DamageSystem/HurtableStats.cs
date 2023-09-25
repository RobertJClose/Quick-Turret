using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.DamageSystem
{
    [CreateAssetMenu(fileName = "HurtableStats", menuName = "Damage System/HurtableStats")]
    public class HurtableStats : ScriptableObject
    {
        public float MaxHealth;
        public float Armour;
    }
}
