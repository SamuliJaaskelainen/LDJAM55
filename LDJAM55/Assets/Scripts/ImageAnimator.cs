using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour
{
    [SerializeField] Sprite[] frames;
    public bool autoPlay = true;
    public float rate;
    public bool loop;
    float timer;
    int frame = 0;
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
        SetFrame();

        if (!autoPlay)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if(Time.time > timer)
        {
            timer = Time.time + rate;
            frame++;
            SetFrame();
        }
    }

    void SetFrame()
    {
        if (frame == frames.Length)
        {
            if(loop)
            {
                frame = 0;
            }
            else
            {
                enabled = false;
            }
        }
        frame = Mathf.Clamp(frame, 0, frames.Length - 1);
        image.sprite = frames[frame];
    }

    public void Play()
    {
        frame = 0;
        SetFrame();
        enabled = true;
    }

    public void SetNewFrames(Sprite[] newFrames)
    {
        frames = newFrames;
        enabled = true;
    }
}
