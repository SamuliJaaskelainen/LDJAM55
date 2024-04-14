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

    void Update()
    {
        const float rankClimbDurationSeconds = 3f;
        float rankClimbProgress = (Time.time - finalResultCaptureTimeSeconds) / rankClimbDurationSeconds;
        float currentRelativeMaxRank = Math.Clamp(rankClimbProgress, 0f, 1f);
        int contestantCount = finalResult.otherContestantCount + 1;
        int currentMaxRank = contestantCount - (int)(contestantCount * currentRelativeMaxRank);

        overallText.text = "Overall " + rankToString(finalResult.overallRank, currentMaxRank);
        funText.text = "Fun " + rankToString(finalResult.funRank, currentMaxRank);
        innovationText.text = "Innovation " + rankToString(finalResult.innovationRank, currentMaxRank);
        themeText.text = "Theme " + rankToString(finalResult.themeRank, currentMaxRank);
        graphicsText.text = "Graphics " + rankToString(finalResult.graphicsRank, currentMaxRank);
        audioText.text = "Audio " + rankToString(finalResult.audioRank, currentMaxRank);
        humorText.text = "Humor " + rankToString(finalResult.humorRank, currentMaxRank);
        moodText.text = "Mood " + rankToString(finalResult.moodRank, currentMaxRank);
    }

    private string rankToString(int rank, int currentMaxRank)
    {
        int limitedRank = Math.Max(rank, currentMaxRank);
        if (limitedRank == 1)
        {
            return limitedRank.ToString() + "st";
        }
        else if (limitedRank == 2)
        {
            return limitedRank.ToString() + "nd";
        }
        else if (limitedRank == 3)
        {
            return limitedRank.ToString() + "rd";
        }
        return limitedRank.ToString() + "th";
    }
}
