using QuickTurret.DamageSystem;
using QuickTurret.TurretComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurretController))]
public class BulletTrailFactory : MonoBehaviour
{
    private static Transform BulletTrailParent;

    private TurretController controller;

    public BulletTrail BulletTrailPrefab;

    private void Awake()
    {
        if (BulletTrailParent == null)
        {
            GameObject parent = new GameObject("Bullet Trails");
            BulletTrailParent = parent.transform;
        }

        controller = GetComponent<TurretController>();
    }

    private void Start()
    {
        controller.OnFire += HandleOnFireEvent;
    }

    private void OnDestroy()
    {
        controller.OnFire -= HandleOnFireEvent;
    }

    private void CreateBulletTrail(Vector3 origin, Vector3 destination, Color color)
    {
        Vector3 toDestination = destination - origin;
        BulletTrail bulletTrail = Instantiate(BulletTrailPrefab, origin, Quaternion.LookRotation(toDestination), BulletTrailParent);

        bulletTrail.SetupBulletTrail(destination, color);
    }

    private void HandleOnFireEvent(object sender, TurretController.OnFireEventArgs e)
    {
        CreateBulletTrail(e.BulletOrigin, e.BulletDestination, e.AmmoType.Colour);
    }
}
