using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.Mobs
{
    public class Containozoid : MonoBehaviour
    {
        float randomAngularSpeed;

        public Transform Model;

        private void Awake()
        {
            randomAngularSpeed = Random.Range(80f, 120f);
        }

        private void Update()
        {
            Model.Rotate(Model.forward, randomAngularSpeed * Time.deltaTime, Space.Self);
        }
    }
}
