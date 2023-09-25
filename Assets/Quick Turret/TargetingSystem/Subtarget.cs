using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.TargetingSystem
{
    [DisallowMultipleComponent]
    public partial class Subtarget : MonoBehaviour, ITaggable
    {
        [SerializeField, SerializeReference]
        private Targetable targetable;
        [SerializeField]
        private List<TargetingTag> tags = new List<TargetingTag>();
        [SerializeField]
        private Vector3 position = Vector3.zero;

        public bool IsUntagged => tags.Count == 0;

        public Vector3 Position { get => position; set => position = value; }

        public List<TargetingTag> Tags => tags;

        public TagsAsset TagsAsset => targetable.TagsAsset;

        public Targetable Targetable
        {
            get => targetable;
            set
            {
                Targetable oldTargetable = targetable;

                if (oldTargetable != value && oldTargetable != null)
                    oldTargetable.DeregisterSubtarget(this);

                targetable = value;

                if (targetable != null)
                    targetable.RegisterSubtarget(this);
            }
        }

        public Vector3 WorldPosition
        {
            get => transform.TransformPoint(position); 
            set => position = transform.InverseTransformPoint(value);
        }

        private void Awake()
        {
            ValidateTags();

            if (targetable != null)
                targetable.RegisterSubtarget(this);
        }

        private void OnValidate()
        {
            ValidateTags();

            if (targetable != null)
                targetable.RegisterSubtarget(this);
        }

        private void OnDestroy()
        {
            if (targetable != null)
                targetable.DeregisterSubtarget(this);
        }

        public void AddTag(TargetingTag tag)
        {
            if (TagsAsset == null)
                return;

            if (tags.Contains(tag) || !TagsAsset.Tags.Contains(tag))
                return;

            tags.Add(tag);
        }

        public bool HasTag(TargetingTag tag)
        {
            return tags.Contains(tag);
        }

        public void RemoveTag(TargetingTag tag)
        {
            tags.RemoveAll(t => t == tag);
        }

        public void ValidateTags()
        {
            if (TagsAsset == null)
                tags.Clear();
            else
                tags.RemoveAll(t => !TagsAsset.Tags.Contains(t));
        }
    }
}
