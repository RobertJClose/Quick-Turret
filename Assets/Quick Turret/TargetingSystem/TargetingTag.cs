using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.TargetingSystem
{
    [System.Serializable]
    public class TargetingTag : System.IEquatable<TargetingTag>
    {
        [SerializeField]
        private SerializableGuid guid;

        public string Name;
        public TagType Type;

        public System.Guid GUID
        {
            get
            {
                if (guid.ToString() == null)
                    guid = System.Guid.NewGuid();

                return guid;
            }
        }

        public enum TagType
        {
            // Changing the integer value associated with each TagType requires a 
            // change to the TagsAssetEditor, where the int values are used to query
            // the type of each tag.
            Other,
            Target,
            Subtarget,
        }

        public TargetingTag() { guid = System.Guid.NewGuid(); }

        public static bool operator ==(TargetingTag lhs, TargetingTag rhs)
        {
            // Handling if the left operand is null.
            if (lhs is null)
                return rhs is null;

            // Our Equals() method knows how to handle a passed null value.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(TargetingTag lhs, TargetingTag rhs) => !(lhs == rhs);

        public bool Equals(TargetingTag other)
        {
            // Check for null argument.
            if (other is null)
                return false;

            // Optimisation for a common success case.
            if (ReferenceEquals(this,other))
                return true;

            // Test the run-time types.
            if (GetType() != other.GetType()) 
                return false;

            // Two tags are equal if they have the same GUID.
            return guid.Equals(other.guid);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            TargetingTag tag = obj as TargetingTag;

            if (tag is null)
                return false;
            else
                return Equals(tag);
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }
    }
}
