using QuickTurret.DamageSystem;
using QuickTurret.Waypoints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace QuickTurret.Mobs
{
    public class MonkezoidHalo : MonoBehaviour
    {
        Hurtable myHurtable;
        float randomAngularSpeed;
        Vector3 randomAxis;

        public GameObject Monkezoid;

        private void Awake()
        {
            myHurtable = GetComponent<Hurtable>();
            randomAngularSpeed = Random.Range(180f, 240f);
            randomAxis = new Vector3(Random.value, Random.value, Random.value).normalized;
        }

        private void Start()
        {
            myHurtable.OnHurt += HandleOnHurtEvent;
            myHurtable.OnDie += HandleOnDieEvent;
        }

        private void Update()
        {
            transform.Rotate(randomAxis, randomAngularSpeed * Time.deltaTime, Space.Self);
        }

        private void OnDestroy()
        {
            myHurtable.OnHurt -= HandleOnHurtEvent;
            myHurtable.OnDie -= HandleOnDieEvent;
        }

        public void HandleOnHurtEvent(object sender, Hurtable.OnHurtEventArgs e)
        {
            if (Monkezoid != null)
            {
                Monkezoid.GetComponent<DamageTextFactory>().CreateDamageText(e.FinalDamage);
            }
        }

        public void HandleOnDieEvent(object sender, Hurtable.OnDieEventArgs e)
        {
            if (Monkezoid != null)
            {
                Monkezoid.GetComponent<NavMeshAgent>().speed *= 0.8f;
            }
        }
    }
}