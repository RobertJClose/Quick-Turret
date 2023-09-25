using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.TargetingSystem
{
    [CreateAssetMenu(fileName = "TargetingTags", menuName = "Targeting System/Tags Asset")]
    public class TagsAsset : ScriptableObject
    {
        public List<TargetingTag> Tags = new List<TargetingTag>();

        private void OnValidate()
        {
            Tags.Sort((a, b) => a.Type.CompareTo(b.Type));
        }
    }
}
