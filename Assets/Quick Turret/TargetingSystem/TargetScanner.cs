using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuickTurret.TargetingSystem
{
    public class TargetScanner : MonoBehaviour
    {
        public bool IsScanning = true;
        [Min(0f)]
        public float ScanRange = 10f;
        [Min(0f)]
        [Delayed]
        public float ScanPeriod = 0.5f;

        public TagPriorities TargetingPriorities;

        public event System.EventHandler<OnScanEventArgs> OnScan;

        public class OnScanEventArgs : System.EventArgs
        {
            public List<Targetable> Targetables;

            public OnScanEventArgs(List<Targetable> targetables)
            {
                Targetables = targetables;
            }
        }

        private void Start()
        {
            StartCoroutine(ScanForTargets(ScanPeriod));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ScanRange);
        }

        private List<Targetable> FindTargets(float range)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            List<Collider> collidersAsList = new List<Collider>(colliders);
            
            List<Targetable> targetables = collidersAsList.ConvertAll(c => c.GetComponentInParent<Targetable>());
            targetables.RemoveAll(t => t == null);

            // Remove all duplicated entries via a HashSet.
            targetables = new HashSet<Targetable>(targetables).ToList();

            targetables.Sort(TargetsDistanceComparison);

            if (TargetingPriorities != null)
                TargetingPriorities.SortByTags(targetables);

            for (int i = 0; i < targetables.Count; i++)
            {
                if (TargetingPriorities != null)
                {
                    Targetable item = targetables[i];
                }
            }

            return targetables;
        }

        private void RaiseOnScan(OnScanEventArgs e)
        {
            System.EventHandler<OnScanEventArgs> raise = OnScan;

            if (raise != null)
                raise(this, e);
        }

        private IEnumerator ScanForTargets(float scanPeriod)
        {
            while (true)
            {
                if (IsScanning)
                {
                    List<Targetable> targetables = FindTargets(ScanRange);
                    OnScanEventArgs e = new OnScanEventArgs(targetables);
                    RaiseOnScan(e);
                }

                yield return new WaitForSeconds(scanPeriod);
            }
        }

        private int TargetsDistanceComparison(Targetable a, Targetable b)
        {
            if (a == null || b == null)
                return 0;

            float aSqrDistance = (a.transform.position - transform.position).sqrMagnitude;
            float bSqrDistance = (b.transform.position - transform.position).sqrMagnitude;

            return aSqrDistance.CompareTo(bSqrDistance);
        }
    }
}
