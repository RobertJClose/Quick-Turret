using QuickTurret.DamageSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.TurretComponents
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Ammo Type", menuName = "Ammunition/AmmoType")]
    public partial class AmmoType : ScriptableObject, ISerializationCallbackReceiver
    {
        // This list contains the probability that a round of this ammo will cause
        // a particular type of damage. Each type of damage defined by
        // DamageEffect.DamageType other than "Normal" damage should have exactly one
        // entry in this list at all times.
        // Furthermore, we keep this list sorted so that the order of the damage types
        // matches the order of their definition.
        private List<(DamageEffect.DamageType Type, float Chance)> damageTypeChances = new List<(DamageEffect.DamageType Type, float chance)>();

        public string AmmoName;
        public Color Colour;
        [Min(0f)]
        public float Damage;

        private void Awake()
        {
            ValidateDamageEffectChancesList();
        }

        private void OnValidate()
        {
            ValidateDamageEffectChancesList(); 
        }

        public DamageEffect CreateDamageEffect()
        {
            DamageEffect output = new DamageEffect();

            output.Damage = Damage;

            foreach (var (Type, Chance) in damageTypeChances)
            {
                float roll = Random.Range(0f, 1f);

                if (roll < Chance)
                    output.Type |= Type;
            }

            return output;
        }

        // This method ensures that the "damageTypeChances" list has exactly one entry
        // for each type of non-normal damage defined in DamageEffect.DamageType.
        // We also sort the list so that its entries are in the same order as the 
        // damage types are defined in DamageEffect.DamageType.
        private void ValidateDamageEffectChancesList()
        {
            foreach (var type in System.Enum.GetValues(typeof(DamageEffect.DamageType)))
            {
                if ((int)type == (int)DamageEffect.DamageType.Normal)
                    continue;

                // Ensure that every type of non-normal damage has at least one entry.
                if (!damageTypeChances.Exists(c => (int)c.Type == (int)type))
                {
                    damageTypeChances.Add(((DamageEffect.DamageType)type, 0f));
                    serializedChances.Add(0f);
                }

                // Remove any duplicate entries for each type. The surviving entry
                // is the first one in the list.
                var entries = damageTypeChances.FindAll(c => (int)c.Type == (int)type);
                for (int i = 1; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    damageTypeChances.Remove(entry);
                }
            }

            // Sort the list to match the order of the damage types as defined in
            // DamageEffect.DamageType.
            int comparer((DamageEffect.DamageType, float) a, (DamageEffect.DamageType, float) b)
            {
                int aTypeAsInt = (int)a.Item1;
                int bTypeAsInt = (int)b.Item1;

                return aTypeAsInt.CompareTo(bTypeAsInt);
            }

            damageTypeChances.Sort(comparer);
        }
    }

    // This section of the partial class handles serialisation. There are the 
    // required methods for the ISerializationCallbacReceiver interface, as well as
    // a List<float> object to enable serialization of "damageTypeChances".
    public partial class AmmoType : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private List<float> serializedChances = new List<float>();

        public void OnBeforeSerialize()
        {
            ValidateDamageEffectChancesList();

            serializedChances.Clear();
            foreach (var item in damageTypeChances)
            {
                serializedChances.Add(item.Chance);
            }
        }

        public void OnAfterDeserialize()
        {
            ValidateDamageEffectChancesList();

            // In this loop we do one less iteration than the number of damage types
            // because our list has no entry for "Normal" type damage.
            int numOfDamageTypes = System.Enum.GetValues(typeof(DamageEffect.DamageType)).Length;
            for (int i = 0; i < numOfDamageTypes - 1; i++)
            {
                DamageEffect.DamageType type = damageTypeChances[i].Type;
                float newChance = Mathf.Clamp01(serializedChances[i]);
                damageTypeChances[i] = (type, newChance);
            }
        }
    }
}
