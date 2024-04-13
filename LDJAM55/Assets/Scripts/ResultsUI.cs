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
        overallText.text = "Overall " + ((int)finelResult.overallRank).ToString() + "th";
        funText.text = "Fun " + ((int)finelResult.funRank).ToString() + "th";
        innovationText.text = "Innovation " + ((int)finelResult.innovationRank).ToString() + "th";
        themeText.text = "Theme " + ((int)finelResult.themeRank).ToString() + "th";
        graphicsText.text = "Graphics " + ((int)finelResult.graphicsRank).ToString() + "th";
        audioText.text = "Audio " + ((int)finelResult.audioRank).ToString() + "th";
        humorText.text = "Humor " + ((int)finelResult.humorRank).ToString() + "th";
        moodText.text = "Mood " + ((int)finelResult.moodRank).ToString() + "th";
    }
}
