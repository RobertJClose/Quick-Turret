using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.DamageSystem
{
    public class RaycastGun : MonoBehaviour
    {
        public Transform BarrelEnd;

        public float MaximumRange = 1000f;

        private void OnDrawGizmosSelected()
        {
            if (BarrelEnd != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(BarrelEnd.position, 0.3f);
            }
        }

        public (Vector3 origin, Vector3 destination) Fire(DamageEffect damageEffect)
        {
            Ray ray = new Ray(BarrelEnd.position, BarrelEnd.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, MaximumRange))
            {
                if (hitInfo.transform.TryGetComponent(out Hurtable hurtable))
                {
                    hurtable.Hurt(gameObject, damageEffect);
                }

                return (ray.origin, hitInfo.point);
            }

            return (ray.origin, ray.GetPoint(MaximumRange));
        }
    }
}