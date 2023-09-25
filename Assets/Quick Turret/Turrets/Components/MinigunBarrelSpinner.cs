using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.TurretComponents
{
    public class MinigunBarrelSpinner : MonoBehaviour
    {
        public Transform Barrels;
        public float Speed = 6f;

        private void Update()
        {
            Barrels.Rotate(Barrels.forward, Speed * Time.deltaTime, Space.World);
        }
    }
}