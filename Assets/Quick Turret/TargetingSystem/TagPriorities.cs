using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.TargetingSystem
{
    [CreateAssetMenu(fileName = "TagPriorities", menuName = "Targeting System/Tag Priorities")]
    public class TagPriorities : ScriptableObject, ISerializationCallbackReceiver
    {
        // The 'noTagPriorityValue' is used to allow editing of the priority value in
        // the custom inspector for this ScriptableObject.
        [SerializeField]
        private int serializedNoTagPriorityValue;
        private readonly Priority noTagPriority = new Priority(null, 0);

        // [SerializeReference] so we can keep a null value in 'noTagPriority'.
        [SerializeReference]
        [SerializeField]
        private List<Priority> priorityList = new List<Priority>();

        [SerializeField]
        private TagsAsset tagsAsset;

        public TagsAsset TagsAsset
        {
            get => tagsAsset;
            set
            {
                if (tagsAsset != value)
                {
                    tagsAsset = value;
                    priorityList.RemoveAll(p => p != noTagPriority);
                    noTagPriority.PriorityValue = 0;

                    if (tagsAsset != null)
                    {
                        foreach (var tag in tagsAsset.Tags)
                        {
                            Priority priority = new Priority(tag, 0);
                            priorityList.Add(priority);
                        }
                    }
                }
            }
        }

        [System.Serializable]
        public class Priority : System.IEquatable<Priority>, System.IComparable<Priority>
        {
            public TargetingTag Tag;
            public int PriorityValue;

            public Priority(TargetingTag Tag, int PriorityValue)
            {
                this.Tag = Tag;
                this.PriorityValue = PriorityValue;
            }

            public static bool operator==(Priority lhs, Priority rhs)
            {
                // Handle the case that the lhs is null.
                if (lhs is null)
                    return rhs is null;

                // Our Equals() method can handle if the rhs is null.
                return lhs.Equals(rhs);
            }

            public static bool operator!=(Priority lhs, Priority rhs) => !(lhs == rhs);

            public static bool operator<(Priority lhs, Priority rhs)
            {
                if (lhs is null)
                    return rhs is not null;
                else
                    return lhs.CompareTo(rhs) < 0;
            }

            public static bool operator>(Priority lhs, Priority rhs)
            {
                if (lhs is null)
                    return false;
                else
                    return lhs.CompareTo(rhs) > 0;
            }

            public static bool operator<=(Priority lhs, Priority rhs)
            {
                if (lhs == rhs)
                    return true;
                else
                    return lhs < rhs;
            }

            public static bool operator>=(Priority lhs, Priority rhs)
            {
                if (lhs == rhs)
                    return true;
                else
                    return lhs > rhs;
            }

            public int CompareTo(Priority other)
            {
                if (other is null)
                    return -1;

                return PriorityValue.CompareTo(other.PriorityValue);
            }

            public override bool Equals(object obj)
            {
                if (obj is null)
                    return false;

                Priority priority = obj as Priority;

                if (priority is null)
                    return false;
                else 
                    return Equals(priority);
            }

            public bool Equals(Priority other)
            {
                if (other is null)
                    return false;

                // Optimisation for a common success case.
                if (ReferenceEquals(this, other))
                    return true;

                // Check run-time types.
                if (GetType() != other.GetType()) 
                    return false;

                // Check for equality.
                return Tag == other.Tag && PriorityValue == other.PriorityValue;
            }

            public override int GetHashCode()
            {
                return Tag.GetHashCode() / PriorityValue.GetHashCode();
            }
        }

        private void OnEnable()
        {
            if (!priorityList.Contains(noTagPriority))
                priorityList.Add(noTagPriority);
        }

        private void OnValidate()
        {
            // Ensure that 'noTagPriority' has no tag.
            noTagPriority.Tag = null;

            // Add any Priority values to the list that are missing.
            if (!priorityList.Contains(noTagPriority))
                priorityList.Add(noTagPriority);

            if (tagsAsset != null)
            {
                foreach (var tag in tagsAsset.Tags)
                {
                    if (priorityList.Find(p => p.Tag == tag) == null)
                        priorityList.Add(new Priority(tag, 0));
                }
            }

            // Remove any Priority values that don't belong.
            if (tagsAsset == null)
                priorityList.RemoveAll(p => p != noTagPriority);
            else
                priorityList.RemoveAll(p => p != noTagPriority && !tagsAsset.Tags.Contains(p.Tag));
        }

        public Priority FindHighestPriorityTag(ITaggable taggable)
        {
            if (taggable == null)
                throw new System.ArgumentNullException(nameof(taggable));

            if (taggable.TagsAsset != tagsAsset)
                throw new System.ArgumentException("TagsAsset mismatch.", nameof(taggable));

            if (taggable.Tags.Count == 0)
                return noTagPriority;

            Priority priority = null;
            foreach (var tag in taggable.Tags)
            {
                Priority tagPriority = GetPriority(tag);

                if (tagPriority < priority)
                    priority = tagPriority;
            }

            return priority;
        }

        public Priority GetPriority(TargetingTag tag)
        {
            if (tag == null)
                return noTagPriority;

            if (!tagsAsset.Tags.Contains(tag))
                throw new System.ArgumentException($"TagsAsset mismatch for tag \"{tag.Name}\".", nameof(tag));

            return priorityList.Find(t => t.Tag == tag);
        }

        public void OnBeforeSerialize()
        {
            serializedNoTagPriorityValue = noTagPriority.PriorityValue;
        }

        public void OnAfterDeserialize()
        {
            noTagPriority.PriorityValue = serializedNoTagPriorityValue;
        }

        public void SortByTags<T>(List<T> taggables) where T : ITaggable
        {
            int comparer(T a, T b)
            {
                return DefaultITaggableComparer(a, b);
            }

            taggables.Sort(comparer);
        }

        private int DefaultITaggableComparer(ITaggable a, ITaggable b)
        {
            if (a == null && b == null)
                return 0;
            else if (a == null && b != null)
                return 1;
            else if (a != null && b == null)
                return -1;

            Priority aPriority = FindHighestPriorityTag(a);
            Priority bPriority = FindHighestPriorityTag(b);

            int output = aPriority.CompareTo(bPriority);
            return output;
        }
    }
}
