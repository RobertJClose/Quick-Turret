using QuickTurret.TargetingSystem;
using QuickTurret.TurretComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperLaserSight : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    public TurretController controller;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (controller.HasTarget)
            lineRenderer.SetPosition(1, transform.InverseTransformPoint(controller.Subtarget.WorldPosition));
        else
            lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));
    }
}
