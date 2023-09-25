using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.DamageSystem
{
    [RequireComponent(typeof(Hurtable))]
    public class Statusable : MonoBehaviour
    {
        public static readonly float TickPeriod = 0.75f;

        private Hurtable hurtable;
        
        private void Awake()
        {
            hurtable = GetComponent<Hurtable>();
        }
        
        private void Start()
        {
            hurtable.OnHurt += HandleOnHurtEvent;
        }

        private void OnDestroy()
        {
            hurtable.OnHurt -= HandleOnHurtEvent;
        }

        private void Update()
        {
            UpdateBleedStatus();
            UpdateCorrodeStatus();
        }

        private void HandleOnHurtEvent(object sender, Hurtable.OnHurtEventArgs e)
        {

        }

        private void UpdateBleedStatus()
        {

        }

        private void UpdateCorrodeStatus()
        {

        }
    }
}