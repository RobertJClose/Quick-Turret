using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikazoid : MonoBehaviour
{
    bool isGrowing;
    float scale;
    float t;

    public float MaxScale = 1.25f;
    public float MinScale = 0.8f;
    public float ScaleSpeed = 0.5f;

    public Transform Model;

    private void Awake()
    {
        isGrowing = true;
        scale = 1.0f;
        t = 1.0f;
    }

    private void Update()
    {
        scale = Mathf.Lerp(MinScale, MaxScale, t);

        Model.localScale = scale * Vector3.one;

        if (isGrowing)
            t += ScaleSpeed * Time.deltaTime;
        else
            t -= ScaleSpeed * Time.deltaTime;

        if (isGrowing && t > 1)
            isGrowing = false;
        
        if (!isGrowing && t < 0)
            isGrowing = true;
    }
}
