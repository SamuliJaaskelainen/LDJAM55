using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] Backend backend;

    [SerializeField] TextMeshProUGUI overallText;
    [SerializeField] TextMeshProUGUI funText;
    [SerializeField] TextMeshProUGUI innovationText;
    [SerializeField] TextMeshProUGUI themeText;
    [SerializeField] TextMeshProUGUI graphicsText;
    [SerializeField] TextMeshProUGUI audioText;
    [SerializeField] TextMeshProUGUI humorText;
    [SerializeField] TextMeshProUGUI moodText;

    void OnEnable()
    {
        DataTypes.FinalResult finelResult = backend.ProductState.ComputeFinalResult();

        // TODO: Add special cases for 1st and 2nd and 3rd
        overallText.text = "Overall " + (finelResult.overallRank).ToString() + "th";
        funText.text = "Fun " + (finelResult.funRank).ToString() + "th";
        innovationText.text = "Innovation " + (finelResult.innovationRank).ToString() + "th";
        themeText.text = "Theme " + (finelResult.themeRank).ToString() + "th";
        graphicsText.text = "Graphics " + (finelResult.graphicsRank).ToString() + "th";
        audioText.text = "Audio " + (finelResult.audioRank).ToString() + "th";
        humorText.text = "Humor " + (finelResult.humorRank).ToString() + "th";
        moodText.text = "Mood " + (finelResult.moodRank).ToString() + "th";
    }
}
