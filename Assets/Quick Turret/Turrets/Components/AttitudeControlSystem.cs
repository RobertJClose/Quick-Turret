using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickTurret.TargetingSystem;

namespace QuickTurret.TurretComponents
{
    public class AttitudeControlSystem : MonoBehaviour
    {
        private Subtarget subtarget;

        [Min(0f)]
        public float TrackingRange = 0f;
        public Transform Base;
        public Transform Gun;

        private void Update()
        {
            RotateBase();
            RotateGun();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, TrackingRange);
        }

        public void TrackSubtarget(Subtarget subtarget)
        {
            this.subtarget = subtarget;
        }

        private void RotateBase()
        {
            if (subtarget == null || subtarget.Targetable == null)
                return;

            Vector3 targetXZPosition = subtarget.WorldPosition;
            targetXZPosition.y = Base.position.y;

            Base.LookAt(targetXZPosition);
        }

        private void RotateGun()
        {
            if (subtarget == null || subtarget.Targetable == null)
                return;

            Vector3 targetPosition = subtarget.WorldPosition;
            Vector3 gunPivot = Gun.transform.position;

            Vector3 toTarget = targetPosition - gunPivot;

            Gun.rotation = Quaternion.LookRotation(toTarget);
        }
    }
}
