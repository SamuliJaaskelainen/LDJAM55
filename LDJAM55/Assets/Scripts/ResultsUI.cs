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

        overallText.text = "Overall " + RankToString(finalResult.overallRank, currentMaxRank);
        funText.text = "Fun " + RankToString(finalResult.funRank, currentMaxRank);
        innovationText.text = "Innovation " + RankToString(finalResult.innovationRank, currentMaxRank);
        themeText.text = "Theme " + RankToString(finalResult.themeRank, currentMaxRank);
        graphicsText.text = "Graphics " + RankToString(finalResult.graphicsRank, currentMaxRank);
        audioText.text = "Audio " + RankToString(finalResult.audioRank, currentMaxRank);
        humorText.text = "Humor " + RankToString(finalResult.humorRank, currentMaxRank);
        moodText.text = "Mood " + RankToString(finalResult.moodRank, currentMaxRank);
    }

    private static string RankToString(int rank, int currentMaxRank)
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
