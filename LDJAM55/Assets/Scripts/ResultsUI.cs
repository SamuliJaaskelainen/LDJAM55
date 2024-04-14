using System;
using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    // TODO: Implement animated rank revearl (grow from 1000 up to final rank)

    [SerializeField] Backend backend;

    [SerializeField] TextMeshProUGUI overallText;
    [SerializeField] TextMeshProUGUI funText;
    [SerializeField] TextMeshProUGUI innovationText;
    [SerializeField] TextMeshProUGUI themeText;
    [SerializeField] TextMeshProUGUI graphicsText;
    [SerializeField] TextMeshProUGUI audioText;
    [SerializeField] TextMeshProUGUI humorText;
    [SerializeField] TextMeshProUGUI moodText;

    DataTypes.FinalResult finalResult;
    float finalResultCaptureTimeSeconds;

    void OnEnable()
    {
        finalResult = backend.ProductState.ComputeFinalResult();
        finalResultCaptureTimeSeconds = Time.time;
    }

    private void Update()
    {
        const float rankClimbDurationSeconds = 3f;
        float rankClimbProgress = (Time.time - finalResultCaptureTimeSeconds) / rankClimbDurationSeconds;
        float currentRelativeMaxRank = Math.Clamp(rankClimbProgress, 0f, 1f);
        int contestantCount = finalResult.otherContestantCount + 1;
        int currentMaxRank = contestantCount - (int)(contestantCount * currentRelativeMaxRank);

        // TODO: Add special cases for 1st and 2nd and 3rd
        overallText.text = "Overall " + Math.Max(finalResult.overallRank, currentMaxRank).ToString() + "th";
        funText.text = "Fun " + Math.Max(finalResult.funRank, currentMaxRank).ToString() + "th";
        innovationText.text = "Innovation " + Math.Max(finalResult.innovationRank, currentMaxRank).ToString() + "th";
        themeText.text = "Theme " + Math.Max(finalResult.themeRank, currentMaxRank).ToString() + "th";
        graphicsText.text = "Graphics " + Math.Max(finalResult.graphicsRank, currentMaxRank).ToString() + "th";
        audioText.text = "Audio " + Math.Max(finalResult.audioRank, currentMaxRank).ToString() + "th";
        humorText.text = "Humor " + Math.Max(finalResult.humorRank, currentMaxRank).ToString() + "th";
        moodText.text = "Mood " + Math.Max(finalResult.moodRank, currentMaxRank).ToString() + "th";
    }
}
