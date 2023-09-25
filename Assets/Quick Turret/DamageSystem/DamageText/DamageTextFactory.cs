using QuickTurret.DamageSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(Hurtable))]
public class DamageTextFactory : MonoBehaviour
{
    private static Transform damageTextParent;

    Hurtable hurtable;

    public DamageTextSettings settings;
    public Vector3 SpawnPosition;

    private void Awake()
    {
        if (damageTextParent == null)
        {
            GameObject parent = new GameObject("Damage Text");
            damageTextParent = parent.transform;
        }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(SpawnPosition, 0.25f);
    }

    public void CreateDamageText(DamageEffect damageEffect)
    {
        float damageTaken = damageEffect.Damage;

        DamageText damageText = Instantiate(settings.DamageTextPrefab, transform.TransformPoint(SpawnPosition), Quaternion.identity, damageTextParent);

        string text = damageTaken.ToString("####");
        TMP_FontAsset fontAsset = settings.GetFontAsset(damageEffect);
        float fontSize = settings.GetFontSize(damageEffect);
        Color fontColor = settings.GetFontColor(damageEffect);
        Color outlineColor = settings.GetFontOutlineColor(damageEffect);

        float climbAmount = settings.GetTweenClimbAmount(damageEffect);
        float lifetime = settings.GetTweenLifetime(damageEffect);
        float randomness = settings.GetTweenRandomness(damageEffect);

        damageText.SetTextAppearance(text, fontAsset, fontSize, fontColor, outlineColor);
        damageText.SetTextTweening(climbAmount, lifetime, randomness);
    }

    private void HandleOnHurtEvent(object sender, Hurtable.OnHurtEventArgs e)
    {
        CreateDamageText(e.FinalDamage);
    }

}
