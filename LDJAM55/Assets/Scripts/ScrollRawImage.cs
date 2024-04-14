using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRawImage : MonoBehaviour
{
    public float speedX = 1.0f;
    public float speedY = 1.0f;

    RawImage rawImage;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position + new Vector2(speedX, speedY) * Time.unscaledDeltaTime, rawImage.uvRect.size);
    }
}
