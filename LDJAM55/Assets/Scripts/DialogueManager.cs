using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] GameObject dimmer;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Image portraitImage;
    [SerializeField] Sprite emptyPortrait;

    List<Dialogue> currentDilogueList = new List<Dialogue>();
    GameObject developerReference;
    bool isInHiring = false;
    int hiringOption = 0;
    bool isOpenedOnThisFrame = false;

    public enum Traits
    {
        Fun,
        Innovation,
        Theme,
        Graphics,
        Audio,
        Humor,
        Mood
    }

    [System.Serializable]
    public struct Dialogue
    {
        public string text;
        public Sprite portrait;
    }

    void Awake()
    {
        Instance = this;
        HideDialogue();
    }

    public bool IsConversationActive()
    {
        return dialogueBox.activeSelf;
    }

    public void ShowConversation(GameObject developer, List<Dialogue> dialogueList, List<Traits> traits)
    {
        // TODO: Show trait icons
        isOpenedOnThisFrame = true;
        developerReference = developer;
        currentDilogueList = dialogueList;
        ShowNextDialogue();
    }

    void ShowNextDialogue()
    {
        if(currentDilogueList.Count > 0)
        { 
            if(currentDilogueList[0].text == "HIRE")
            {
                ShowHiring();
            }
            else
            {
                ShowDialogue(currentDilogueList[0]);
            }
            currentDilogueList.RemoveAt(0);
        }

        else
        {
            HideDialogue();
        }
    }

    void ShowDialogue(Dialogue dialogue)
    {
        dimmer.SetActive(true);
        dialogueBox.SetActive(true);
        dialogueText.text = dialogue.text;
        if(dialogue.portrait == null)
        {
            portraitImage.sprite = emptyPortrait;
        }
        else
        {
            portraitImage.sprite = dialogue.portrait;
        }
        
    }

    void HideDialogue()
    {
        if(developerReference != null)
        { 
            Destroy(developerReference);
        }
        dialogueBox.SetActive(false);
        dimmer.SetActive(false);
        portraitImage.sprite = emptyPortrait;
    }

    void ShowHiring()
    {
        hiringOption = 0;
        isInHiring = true;
    }

    void Update()
    {
        if(isInHiring)
        {
            if (StateManager.PressedUp()) hiringOption = 0;
            if (StateManager.PressedDown()) hiringOption = 1;

            if (hiringOption == 0)
            {
                dialogueText.text = "Summon?\n    >Yes\n     No";

                if (StateManager.PressedUse())
                {
                    isInHiring = false;
                    // TOOD: Add character to party
                    ShowNextDialogue();
                }
            }
            else
            {
                dialogueText.text = "Summon?\n     Yes\n    >No";

                if (StateManager.PressedUse())
                {
                    isInHiring = false;
                    HideDialogue();
                }
            }
        }
        else if(!isOpenedOnThisFrame)
        {
            if (StateManager.PressedUse()) ShowNextDialogue();
        }
        else
        {
            isOpenedOnThisFrame = false;
        }
    }
}
