using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.DamageSystem
{
    [System.Serializable]
    public struct DamageEffect
    {
        [Min(0f)]
        public float Damage;
        public DamageType Type;

        [System.Flags]
        public enum DamageType
        {
            Normal = 0,
            Critical = 2,
            ArmourPiercing = 4,
            Blocked = 8,
        }
    }
}
