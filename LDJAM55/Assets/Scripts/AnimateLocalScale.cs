using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLocalScale : MonoBehaviour
{
    public bool autoStart = false;
    public float startOffset = 0.0f;
    public float duration = 1.0f;
    public bool loop = false;
    public bool ignoreStartValue = false;
    public AnimationCurve X = new AnimationCurve();
    public AnimationCurve Y = new AnimationCurve();
    public AnimationCurve Z = new AnimationCurve();

    float timer = 0.0f;
    Vector3 startValue = new Vector3();
    Vector3 originalScale;

    void Start()
    {
        if (ignoreStartValue)
        {
            startValue = Vector3.one;
        }
        else
        {
            startValue = transform.localScale;
        }
        originalScale = transform.localScale;
        if (X.length == 0) { X.AddKey(0, 1); }
        if (Y.length == 0) { Y.AddKey(0, 1); }
        if (Z.length == 0) { Z.AddKey(0, 1); }
        ResetValues();
        timer = startOffset;

        if (autoStart)
        {
            Play();
        }
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime / duration;
        if (timer < 1.0f)
        {
            transform.localScale = new Vector3(startValue.x * X.Evaluate(timer), startValue.y * Y.Evaluate(timer), startValue.z * Z.Evaluate(timer));
        }
        else
        {
            if (loop)
            {
                timer -= 1.0f;
                enabled = true;
            }
            else
            {
                ResetValues();
            }
        }
    }

    public void Play()
    {
        transform.localScale = originalScale;
        startValue = transform.localScale;
        originalScale = startValue;

        if (enabled)
        {
            Restart();
        }

        enabled = true;

        Update();
    }
    public void Pause()
    {
        enabled = false;
    }

    public void Restart()
    {
        timer = startOffset;
        enabled = true;
    }

    public void ResetValues()
    {
        timer = 0.0f;
        transform.localScale = new Vector3(startValue.x * X.Evaluate(timer), startValue.y * Y.Evaluate(timer), startValue.z * Z.Evaluate(timer));
        enabled = false;
    }
}
