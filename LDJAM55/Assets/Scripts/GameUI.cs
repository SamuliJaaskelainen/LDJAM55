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

    [SerializeField] TextMeshProUGUI timer;

    [SerializeField] TextMeshProUGUI features;
    [SerializeField] TextMeshProUGUI bugs;

    [SerializeField] TextMeshProUGUI mechanics;
    [SerializeField] TextMeshProUGUI visuals;
    [SerializeField] TextMeshProUGUI polish;

    [SerializeField] Image[] devImgs;
    [SerializeField] TextMeshProUGUI[] devDurabilities;

    void Update()
    {
        // TODO: Implement TimeSpan to show hours and minutes from 72h to 0h
        timer.text = ((int)(backend.ProductState.TimeLeft)).ToString();

        features.text = " Features " + backend.Backlog();
        bugs.text = " Found bugs " + backend.FoundBugs();

        mechanics.text = " Mechanics " + (int)(backend.ProductState.MechanicsFeature * 100.0f);
        // TODO: Dialogue popup for forgotten audio
        if (backend.ProductState.TimeLeft < 60.0f)
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
            if(backend.ActiveDevelopers[i] != null)
            {     
                devImgs[i].gameObject.SetActive(backend.ActiveDevelopers[i].IsAlive);
                devImgs[i].sprite = backend.ActiveDevelopers[i].portrait;
                devDurabilities[i].text = ((int)(backend.ActiveDevelopers[i].Durability)).ToString();

                if(backend.ActiveDevelopers[i].IsAlive)
                { 
                    developersAlive++;
                }
            }
            else
            {
                devImgs[i].gameObject.SetActive(false);
            }
        }

        portal.SetActive(developersAlive != backend.ActiveDevelopers.Length);
    }
}