using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
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
        // TODO: Implement TimeSpan
        timer.text = ((int)(backend.ProductState.TimeLeft)).ToString();

        features.text = " Features " + backend.Backlog();
        bugs.text = " Found bugs " + backend.FoundBugs();

        mechanics.text = " Mechanics " + (int)(backend.ProductState.MechanicsFeature * 100.0f);
        visuals.text = " Visuals " + (int)(backend.ProductState.VisualsFeature * 100.0f); // TODO: Change to audiovisual in late game
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