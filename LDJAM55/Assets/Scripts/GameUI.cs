using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    // TODO: Implement bars for backend and game stats

    [SerializeField] Backend backend;

    [SerializeField] GameObject portal;
    [SerializeField] Image portalEdge;
    [SerializeField] Sprite[] portalFrames;

    [SerializeField] ImageAnimator bossAnim;
    [SerializeField] Sprite[] bossSummonFrames;
    [SerializeField] Sprite[] bossIdleFrames;

    [SerializeField] TextMeshProUGUI timer;

    [SerializeField] TextMeshProUGUI features;
    [SerializeField] TextMeshProUGUI bugs;

    [SerializeField] TextMeshProUGUI mechanics;
    [SerializeField] TextMeshProUGUI visuals;
    [SerializeField] TextMeshProUGUI polish;

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
        // TODO: Implement TimeSpan to show hours and minutes from 72h to 0h
        timer.text = ((int)(backend.ProductState.TimeLeft)).ToString();

        features.text = " Features " + backend.Backlog();
        bugs.text = " Found bugs " + backend.FoundBugs();

        mechanics.text = " Mechanics " + (int)(backend.ProductState.MechanicsFeature * 100.0f);

        if (backend.ProductState.TimeLeft < StateManager.AUDIO_STORY_TIME)
        {
            visuals.text = " Audiovisuals " + (int)(backend.ProductState.AudioVisualsFeature * 100.0f);
        }
        else
        {
            visuals.text = " Visuals " + (int)(backend.ProductState.VisualsFeature * 100.0f);
        }
        polish.text = " Polish " + (int)(backend.ProductState.PolishFeature * 100.0f);

        int developersAlive = 0;
        for(int i = 0; i < backend.ActiveDevelopers.Length; ++i)
        {
            if (backend.ActiveDevelopers[i] != null)
            {
                devDurabilities[i].text = ((int)(backend.ActiveDevelopers[i].Durability)).ToString();

                if(backend.ActiveDevelopers[i].IsAlive)
                {
                    if(!devAlive[i])
                    { 
                        devAlive[i] = true;
                        devAnims[i].SetNewFrames(new Sprite[] { backend.ActiveDevelopers[i].portrait });
                    }
                    developersAlive++;
                }
                else
                {
                    if(devAlive[i])
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
                devAnims[i].SetNewFrames(emptySprite);
            }
        }

        bool isPortalOpen = developersAlive != backend.ActiveDevelopers.Length;

        if(isPortalOpen)
        {
            bossAnim.SetNewFrames(bossSummonFrames);
            portal.SetActive(true);
            if(Time.time > portalAnimTimer)
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
                if(portalFrame == 0)
                {
                    portal.SetActive(false);
                }
            }
        }
        
    }
}