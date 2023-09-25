using QuickTurret.DamageSystem;
using QuickTurret.TargetingSystem;
using QuickTurret.Mobs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.TurretComponents
{
    [RequireComponent(typeof(TargetScanner))]
    [RequireComponent(typeof(AttitudeControlSystem))]
    [RequireComponent(typeof(Autoloader))]
    [RequireComponent(typeof(RaycastGun))]
    public class TurretController : MonoBehaviour
    {
        AttitudeControlSystem attitudeControlSystem;
        Autoloader autoloader;
        RaycastGun raycastGun;
        Subtarget subtarget;
        TargetScanner scanner;
        Targetable targetable;
        List<Targetable> targetables;

        public bool HasTarget => subtarget != null;
        public bool IsFireReady => HasTarget && IsLoaded;
        public bool IsLoaded => autoloader.IsLoaded;
        public Subtarget Subtarget => subtarget;

        public event System.EventHandler<OnFireEventArgs> OnFire;

        public class OnFireEventArgs : System.EventArgs
        {
            public readonly AmmoType AmmoType;
            public readonly DamageEffect DamageEffect;
            public readonly Vector3 BulletOrigin;
            public readonly Vector3 BulletDestination;

            public OnFireEventArgs(AmmoType ammoType, DamageEffect damageEffect, Vector3 bulletOrigin, Vector3 bulletDestination)
            {
                AmmoType = ammoType;
                DamageEffect = damageEffect;
                BulletOrigin = bulletOrigin;
                BulletDestination = bulletDestination;
            }
        }

        private void Awake()
        {
            attitudeControlSystem = GetComponent<AttitudeControlSystem>();
            autoloader = GetComponent<Autoloader>();
            raycastGun = GetComponent<RaycastGun>();
            scanner = GetComponent<TargetScanner>();
        }

        private void Start()
        {
            scanner.OnScan += HandleOnScanEvent;
        }

        private void Update()
        {
            if (IsFireReady)
                FireTurret();
        }

        private void OnDestroy()
        {
            scanner.OnScan -= HandleOnScanEvent;
        }

        public void ResetTarget()
        {
            targetable = null;
            subtarget = null;
        }

        private void FireTurret()
        {
            AmmoType round = autoloader.DrawChamberedRound();
            DamageEffect damageEffect = round.CreateDamageEffect();
            (Vector3 bulletOrigin, Vector3 bulletDestination) = raycastGun.Fire(damageEffect);

            OnFireEventArgs e = new OnFireEventArgs(round, damageEffect, bulletOrigin, bulletDestination);
            RaiseOnFire(e);
        }

        private void HandleOnScanEvent(object sender, TargetScanner.OnScanEventArgs e)
        {
            targetables = e.Targetables;

            if (targetables.Count == 0)
            {
                targetable = null;
                subtarget = null;
                attitudeControlSystem.TrackSubtarget(null);
                return;
            }

            if (!targetables.Contains(targetable))
                targetable = targetables[0];

            List<Subtarget> subtargets = targetable.SubtargetsCopy;
            
            if (scanner.TargetingPriorities != null)
                scanner.TargetingPriorities.SortByTags(subtargets);
            
            subtarget = subtargets[0];
            attitudeControlSystem.TrackSubtarget(subtarget);
        }

        private void RaiseOnFire(OnFireEventArgs onFireEventArgs)
        {
            System.EventHandler<OnFireEventArgs> raiseEvent = OnFire;

            if (raiseEvent != null)
                raiseEvent(this, onFireEventArgs);
        }
    }
}
