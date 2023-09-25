using QuickTurret.DamageSystem;
using QuickTurret.TargetingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkezoid : MonoBehaviour
{
    Targetable targetable;
    Subtarget subtarget;

    public Hurtable FinalHalo;

    private void Awake()
    {
        targetable = GetComponent<Targetable>();
        subtarget = GetComponent<Subtarget>();
    }

    private void Start()
    {
        FinalHalo.OnDie += HandleFinalHaloOnDieEvent;
    }

    private void OnDestroy()
    {
        FinalHalo.OnDie -= HandleFinalHaloOnDieEvent;
    }

    private void HandleFinalHaloOnDieEvent(object sender, Hurtable.OnDieEventArgs e)
    {

    }
}
