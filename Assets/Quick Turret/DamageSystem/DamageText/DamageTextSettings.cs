using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace QuickTurret.DamageSystem
{
    [CreateAssetMenu(fileName = "Damage Text Settings", menuName = "Damage System/Damage Text Settings")]
    public class DamageTextSettings : ScriptableObject
    {
        public static readonly int NumberOfSettings = 7;

        public DamageText DamageTextPrefab;

        public Setting BaseSettings = new Setting();

        public OverrideSetting CriticalSettings = new OverrideSetting() { DamageType = DamageEffect.DamageType.Critical };
        public OverrideSetting ArmourPiercingSettings = new OverrideSetting() { DamageType = DamageEffect.DamageType.ArmourPiercing };
        public OverrideSetting BlockedSetting = new OverrideSetting() { DamageType = DamageEffect.DamageType.Blocked };
        
        List<OverrideSetting> Overrides => new List<OverrideSetting>() 
        { 
            CriticalSettings, 
            ArmourPiercingSettings, 
            BlockedSetting,
        };

        [System.Serializable]
        public class Setting
        {
            public TMP_FontAsset FontAsset;
            public float FontSize = 36f;
            public Color FontColor = Color.white;
            public Color FontOutlineColor = Color.black;
            public float TweenClimbAmount;
            [Min(0f)]
            public float TweenLifetime = 1.0f;
            [Range(0f, 1f)]
            public float TweenRandomness = 0.5f;
        }

        [System.Serializable]
        public class OverrideSetting : Setting
        {
            [ReadOnly]
            public DamageEffect.DamageType DamageType;
            public int Priority;

            public List<bool> DoesOverride = new List<bool>(NumberOfSettings) { false, false, false, false, false, false, false};
        }

        private void OnValidate()
        {
            CriticalSettings.DamageType = DamageEffect.DamageType.Critical;
            ArmourPiercingSettings.DamageType = DamageEffect.DamageType.ArmourPiercing;
            BlockedSetting.DamageType = DamageEffect.DamageType.Blocked;
        }

        public TMP_FontAsset GetFontAsset(DamageEffect damageEffect)
        {
            TMP_FontAsset output = BaseSettings.FontAsset;
            int currentPriority = int.MaxValue;

            foreach (var setting in Overrides)
            {
                if ((setting.DamageType & damageEffect.Type) == DamageEffect.DamageType.Normal)
                    continue;

                if (setting.DoesOverride[0] && setting.Priority < currentPriority)
                {
                    output = setting.FontAsset;
                    currentPriority = setting.Priority;
                }
            }

            return output;
        }

        public float GetFontSize(DamageEffect damageEffect)
        {
            float output = BaseSettings.FontSize;
            int currentPriority = int.MaxValue;

            foreach (var setting in Overrides)
            {
                if ((setting.DamageType & damageEffect.Type) == DamageEffect.DamageType.Normal)
                    continue;

                if (setting.DoesOverride[1] && setting.Priority < currentPriority)
                {
                    output = setting.FontSize;
                    currentPriority = setting.Priority;
                }
            }

            return output;
        }

        public Color GetFontColor(DamageEffect damageEffect)
        {
            Color output = BaseSettings.FontColor;
            int currentPriority = int.MaxValue;

            foreach (var setting in Overrides)
            {
                if ((setting.DamageType & damageEffect.Type) == DamageEffect.DamageType.Normal)
                    continue;

                if (setting.DoesOverride[2] && setting.Priority < currentPriority)
                {
                    output = setting.FontColor;
                    currentPriority = setting.Priority;
                }
            }

            return output;
        }

        public Color GetFontOutlineColor(DamageEffect damageEffect)
        {
            Color output = BaseSettings.FontOutlineColor;
            int currentPriority = int.MaxValue;

            foreach (var setting in Overrides)
            {
                if ((setting.DamageType & damageEffect.Type) == DamageEffect.DamageType.Normal)
                    continue;

                if (setting.DoesOverride[3] && setting.Priority < currentPriority)
                {
                    output = setting.FontOutlineColor;
                    currentPriority = setting.Priority;
                }
            }

            return output;
        }

        public float GetTweenClimbAmount(DamageEffect damageEffect)
        {
            float output = BaseSettings.TweenClimbAmount;
            int currentPriority = int.MaxValue;

            foreach (var setting in Overrides)
            {
                if ((setting.DamageType & damageEffect.Type) == DamageEffect.DamageType.Normal)
                    continue;

                if (setting.DoesOverride[4] && setting.Priority < currentPriority)
                {
                    output = setting.TweenClimbAmount;
                    currentPriority = setting.Priority;
                }
            }

            return output;
        }

        public float GetTweenLifetime(DamageEffect damageEffect)
        {
            float output = BaseSettings.TweenLifetime;
            int currentPriority = int.MaxValue;

            foreach (var setting in Overrides)
            {
                if ((setting.DamageType & damageEffect.Type) == DamageEffect.DamageType.Normal)
                    continue;

                if (setting.DoesOverride[5] && setting.Priority < currentPriority)
                {
                    output = setting.TweenLifetime;
                    currentPriority = setting.Priority;
                }
            }

            return output;
        }

        public float GetTweenRandomness(DamageEffect damageEffect)
        {
            float output = BaseSettings.TweenRandomness;
            int currentPriority = int.MaxValue;

            foreach (var setting in Overrides)
            {
                if ((setting.DamageType & damageEffect.Type) == DamageEffect.DamageType.Normal)
                    continue;

                if (setting.DoesOverride[6] && setting.Priority < currentPriority)
                {
                    output = setting.TweenRandomness;
                    currentPriority = setting.Priority;
                }
            }

            return output;
        }
    }
}