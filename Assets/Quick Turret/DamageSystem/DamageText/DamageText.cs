using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using QuickTurret.DamageSystem;

[RequireComponent(typeof(TextMeshPro))]
public class DamageText : MonoBehaviour
{
    Camera cam;
    Vector3 initialPosition;
    TextMeshPro text;

    private void Awake()
    {
        cam = Camera.main;
        initialPosition = transform.position;
        text = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        Quaternion rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        transform.rotation = rotation;
    }

    public void SetTextAppearance(string text, TMP_FontAsset fontAsset, float fontSize, Color fontColor, Color outlineColor)
    {
        this.text.text = text;
        this.text.font = fontAsset;
        this.text.fontSize = fontSize;
        this.text.color = fontColor;
        this.text.outlineColor = outlineColor;
    }

    public void SetTextTweening(float climbAmount, float lifetime, float randomness)
    {
        Vector3 climbVector = climbAmount * Vector3.up;
        
        Vector3 randomVector = new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(0f, 2f),
            Random.Range(-2f, 2f)).normalized;
        
        Vector3 finalPosition = initialPosition + climbVector + 0.5f * randomness * randomVector;

        Sequence shakeSequence = DOTween.Sequence();
        shakeSequence.Append(transform.DOMove(finalPosition, lifetime))
            .Join(transform.DOShakeScale(lifetime, randomness))
            .AppendCallback(() => Destroy(gameObject));
    }
}
