using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public static float BulletSpeed = 1000f;

    Vector3 destination;
    TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public IEnumerator Start()
    {
        yield return null;

        float sqrDistanceLeft = (destination - transform.position).sqrMagnitude;
        while (sqrDistanceLeft > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, BulletSpeed * Time.deltaTime);
            sqrDistanceLeft = (destination - transform.position).sqrMagnitude;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void SetupBulletTrail(Vector3 destination, Color bulletColor)
    {
        this.destination = destination;

        GradientColorKey[] colorKeys = trailRenderer.colorGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = trailRenderer.colorGradient.alphaKeys;

        colorKeys[0].color = bulletColor;
        colorKeys[1].color = bulletColor;

        Gradient gradient = new Gradient();
        gradient.SetKeys(colorKeys, alphaKeys);

        trailRenderer.colorGradient = gradient;
    }
}
