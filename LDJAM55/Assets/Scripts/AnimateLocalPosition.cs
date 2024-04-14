using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLocaPosition : MonoBehaviour
{
    public bool autoStart = false;
    public float duration = 1.0f;
    public bool loop = false;
    public bool reverse = false;
    public AnimationCurve X = new AnimationCurve();
    public AnimationCurve Y = new AnimationCurve();
    public AnimationCurve Z = new AnimationCurve();
    public float curveMultiplier = 1.0f;

    float timer = 0.0f;
    Vector3 startValue = new Vector3();

    void Awake()
    {
        startValue = transform.localPosition;
        if (X.length == 0) { X.AddKey(0, 0); }
        if (Y.length == 0) { Y.AddKey(0, 0); }
        if (Z.length == 0) { Z.AddKey(0, 0); }
        ResetToStart();

        if (autoStart)
        {
            Play();
        }
    }

    void Update()
    {
        if(reverse)
        {
            timer -= Time.unscaledDeltaTime / duration;
            if (timer > 0.0f)
            {
                transform.localPosition = new Vector3(startValue.x + X.Evaluate(timer) * curveMultiplier, startValue.y + Y.Evaluate(timer) * curveMultiplier, startValue.z + Z.Evaluate(timer) * curveMultiplier);
            }
            else
            {
                ClamToEnd();
                if (loop)
                {
                    ResetToStart();
                    enabled = true;
                }
            }
        }
        else
        {
            timer += Time.unscaledDeltaTime / duration;
            if (timer < 1.0f)
            {
                transform.localPosition = new Vector3(startValue.x + X.Evaluate(timer) * curveMultiplier, startValue.y + Y.Evaluate(timer) * curveMultiplier, startValue.z + Z.Evaluate(timer) * curveMultiplier);
            }
            else
            {
                ClamToEnd();
                if (loop)
                {
                    ResetToStart();
                    enabled = true;
                }
            }
        }    
    }

    public void Play()
    {
        enabled = true;
    }

    public void Pause()
    {
        enabled = false;
    }

    public void Restart()
    {
        if (reverse)
        {
            timer = 1.0f;
        }
        else
        {
            timer = 0.0f;
        }
        enabled = true;
    }

    public void ClamToEnd()
    {
        if (!reverse)
        {
            timer = 1.0f;
            transform.localPosition = new Vector3(startValue.x + X.Evaluate(timer) * curveMultiplier, startValue.y + Y.Evaluate(timer) * curveMultiplier, startValue.z + Z.Evaluate(timer) * curveMultiplier);
        }
        else
        {
            timer = 0.0f;
            transform.localPosition = startValue;
        }
        enabled = false;
    }

    public void ResetToStart()
    {
        if(reverse)
        {
            timer = 1.0f;
            transform.localPosition = new Vector3(startValue.x + X.Evaluate(timer) * curveMultiplier, startValue.y + Y.Evaluate(timer) * curveMultiplier, startValue.z + Z.Evaluate(timer) * curveMultiplier);
        }
        else
        {
            timer = 0.0f;
            transform.localPosition = startValue;
        }
        enabled = false;
    }
}
