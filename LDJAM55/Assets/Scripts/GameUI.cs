using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    // TODO: Implement bars for backend and game stats

    [SerializeField] Backend backend;

    [SerializeField] GameObject portal;
    [SerializeField] Image portalEdge;
    [SerializeField] RawImage portalMask;
    [SerializeField] Sprite[] portalFrames;
    [SerializeField] Texture[] portalMaskFrames;

    [SerializeField] ImageAnimator bossAnim;
    [SerializeField] Sprite[] bossSummonFrames;
    [SerializeField] Sprite[] bossIdleFrames;

    [SerializeField] TextMeshProUGUI timer;

    [SerializeField] Image features;
    [SerializeField] Image bugs;

    [SerializeField] Image mechanics;
    [SerializeField] Image visuals;
    [SerializeField] Image polish;
    [SerializeField] Gradient fillGradient;

    [SerializeField] ImageAnimator[] devAnims;
    [SerializeField] Sprite[] emptySprite;
    [SerializeField] Sprite[] devDeathSprites;
    [SerializeField] TextMeshProUGUI[] devDurabilities;

    int portalFrame;
    float portalAnimSpeed = 0.2f;
    float portalAnimTimer;
    bool[] devAlive = new bool[Backend.ACTIVE_DEVELOPERS];

    void Update()
    {
        const float threeDaysInSeconds = 24f * 3f * 60f * 60f;

        TimeSpan timeLeft = TimeSpan.FromSeconds(threeDaysInSeconds * backend.ProductState.RelativeTimeLeft);
        timer.text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeLeft.TotalHours, timeLeft.Minutes, timeLeft.Seconds);

        float backlog = backend.Backlog();
        features.fillAmount = Mathf.Clamp01(backlog);
        features.color = fillGradient.Evaluate(backlog);
        float foundBugs = backend.FoundBugs();
        bugs.fillAmount = Mathf.Clamp01(foundBugs);
        bugs.color = fillGradient.Evaluate(foundBugs);

        mechanics.fillAmount = Mathf.Clamp01(backend.ProductState.MechanicsFeature);
        mechanics.color = fillGradient.Evaluate(backend.ProductState.MechanicsFeature);

        if (backend.ProductState.TimeLeft < StateManager.AUDIO_STORY_TIME)
        {
            visuals.fillAmount = Mathf.Clamp01(backend.ProductState.AudioVisualsFeature);
            visuals.color = fillGradient.Evaluate(backend.ProductState.AudioVisualsFeature);
        }
        else
        {
            visuals.fillAmount = Mathf.Clamp01(backend.ProductState.VisualsFeature);
            visuals.color = fillGradient.Evaluate(backend.ProductState.VisualsFeature);
        }
        polish.fillAmount = Mathf.Clamp01(backend.ProductState.PolishFeature);
        polish.color = fillGradient.Evaluate(backend.ProductState.PolishFeature);

        int developersAlive = 0;
        for (int i = 0; i < backend.ActiveDevelopers.Length; ++i)
        {
            if (backend.ActiveDevelopers[i] != null)
            {
                devDurabilities[i].text = ((int)(backend.ActiveDevelopers[i].Durability)).ToString();

                if (backend.ActiveDevelopers[i].IsAlive)
                {
                    if (!devAlive[i])
                    {
                        devAlive[i] = true;
                        devAnims[i].SetNewFrames(new Sprite[] { backend.ActiveDevelopers[i].portrait });
                    }
                    developersAlive++;
                }
                else
                {
                    if (devAlive[i])
                    {
                        devAnims[i].SetNewFrames(devDeathSprites);
                        devAnims[i].Play();
                        devAlive[i] = false;
                        // Keith TODO: Developer combustion audio
                    }
                }
            }
            else
            {
                devDurabilities[i].text = "";
                devAnims[i].SetNewFrames(emptySprite);
            }
        }

        bool isPortalOpen = developersAlive != backend.ActiveDevelopers.Length;

        if (isPortalOpen)
        {
            bossAnim.SetNewFrames(bossSummonFrames);
            portal.SetActive(true);
            if (Time.time > portalAnimTimer)
            {
                portalAnimTimer = Time.time + portalAnimSpeed;
                portalFrame++;
                if (portalFrame >= portalFrames.Length)
                {
                    // Keep looping last two frames
                    portalFrame -= 2;
                }
                portalFrame = Mathf.Clamp(portalFrame, 0, portalFrames.Length - 1);
                portalEdge.sprite = portalFrames[portalFrame];
                portalMask.texture = portalMaskFrames[portalFrame];
            }
        }
        else
        {
            bossAnim.SetNewFrames(bossIdleFrames);
            if (Time.time > portalAnimTimer)
            {
                portalAnimTimer = Time.time + portalAnimSpeed;
                portalFrame--;
                portalFrame = Mathf.Clamp(portalFrame, 0, portalFrames.Length);
                portalEdge.sprite = portalFrames[portalFrame];
                portalMask.texture = portalMaskFrames[portalFrame];
                if (portalFrame == 0)
                {
                    portal.SetActive(false);
                }
            }
        }

    }
}