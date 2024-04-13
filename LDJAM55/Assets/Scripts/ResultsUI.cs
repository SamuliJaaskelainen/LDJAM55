using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        overallText.text = "Overall " + ((int)finelResult.overallScore).ToString() + "th";
        funText.text = "Fun " + ((int)finelResult.funScore).ToString() + "th";
        innovationText.text = "Innovation " + ((int)finelResult.innovationScore).ToString() + "th";
        themeText.text = "Theme " + ((int)finelResult.themeScore).ToString() + "th";
        graphicsText.text = "Graphics " + ((int)finelResult.graphicsScore).ToString() + "th";
        audioText.text = "Audio " + ((int)finelResult.audioScore).ToString() + "th";
        humorText.text = "Humor " + ((int)finelResult.humorScore).ToString() + "th";
        moodText.text = "Mood " + ((int)finelResult.moodScore).ToString() + "th";
    }
}
