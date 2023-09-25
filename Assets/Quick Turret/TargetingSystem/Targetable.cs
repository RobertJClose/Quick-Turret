using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace QuickTurret.TargetingSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Subtarget))]
    public partial class Targetable : MonoBehaviour, ITaggable
    {
        [SerializeField, SerializeReference]
        private List<Subtarget> subtargets = new List<Subtarget>();

        [SerializeField]
        private List<TargetingTag> tags = new List<TargetingTag>();
        [SerializeField]
        private TagsAsset tagsAsset;

        public List<Subtarget> SubtargetsCopy => new List<Subtarget>(subtargets);

        public List<TargetingTag> Tags => tags;

        public TagsAsset TagsAsset
        {
            get => tagsAsset;
            set
            {
                if (value == null)
                {
                    tagsAsset = null;
                    Tags.Clear();
                }
                else if (tagsAsset != null)
                {
                    tagsAsset = value;
                    Tags.Clear();
                }
            }
        }

        public bool IsUntagged => Tags.Count == 0;
        
        private void OnValidate()
        {
            ValidateTags();
            SortSubtargets();
        }

        /// <summary>
        /// This method removes the given Subtarget from its list of Subtargets. It 
        /// does not remove the Subtarget's reference to this Targetable.
        /// </summary>
        /// <param name="subtarget">The Subtarget to be removed.</param>
        public void DeregisterSubtarget(Subtarget subtarget)
        {
            subtargets.Remove(subtarget);
            SortSubtargets();
        }

        /// <summary>
        /// This method checks that the given Subtarget is pointing to this Targetable
        /// before adding it to its list of Subtargets.
        /// </summary>
        /// <param name="subtarget">
        /// The Subtarget to be added.
        /// </param>
        public void RegisterSubtarget(Subtarget subtarget)
        {
            if (subtarget.Targetable != this)
                return;

            if (subtargets.Contains(subtarget))
                return;
            
            subtargets.Add(subtarget);
            SortSubtargets();
        }

        private void SortSubtargets()
        {
            int mySubtargetFirstSort(Subtarget a, Subtarget b)
            {
                if (a == GetComponent<Subtarget>())
                    return -1;
                else if (b == GetComponent<Subtarget>())
                    return 1;
                else
                    return 0;
            }

            subtargets.Sort(mySubtargetFirstSort);
        }

        private void ValidateTags()
        {
            if (tagsAsset == null)
                Tags.Clear();
            else
                Tags.RemoveAll(t => !tagsAsset.Tags.Contains(t));
        }
    }
}
