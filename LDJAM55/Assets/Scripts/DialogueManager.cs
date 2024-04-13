using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] Backend backend;
    [SerializeField] GameObject dimmer;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Image portraitImage;
    [SerializeField] Sprite emptyPortrait;
    [SerializeField] Sprite spiritPortrait1;
    [SerializeField] Sprite spiritPortrait2;

    List<Dialogue> currentDilogueList = new List<Dialogue>();
    GameObject developerReference;
    bool isInHiring = false;
    int hiringOption = 0;
    bool isOpenedOnThisFrame = false;
    bool isPortraitVisible = false;

    [System.Serializable]
    public struct Dialogue
    {
        [Multiline]
        public string text;
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

    public void ShowConversation(GameObject developerInHell)
    {
        // TODO: Show trait icons
        isInHiring = false;
        hiringOption = 0;
        isOpenedOnThisFrame = true;
        isPortraitVisible = false;
        developerReference = developerInHell;
        currentDilogueList = new List<Dialogue>(developerInHell.GetComponent<DeveloperInHell>().developer.Dialogue);
        Debug.Log("Dialogue loaded, size of " + currentDilogueList.Count);
        ShowNextDialogue();
    }

    void ShowNextDialogue()
    {
        if(currentDilogueList.Count > 0)
        { 
            if(currentDilogueList[0].text == "HIRE")
            {
                isInHiring = true;
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
        if(isPortraitVisible)
        {
            portraitImage.sprite = developerReference.GetComponent<DeveloperInHell>().developer.portrait;
        }
        else
        {
            portraitImage.sprite = currentDilogueList.Count % 2 == 0 ? spiritPortrait1 : spiritPortrait2;
        }
    }

    void HideDialogue()
    {
        if(developerReference != null)
        { 
            DestroyImmediate(developerReference);
        }
        dialogueBox.SetActive(false);
        dimmer.SetActive(false);
        portraitImage.sprite = emptyPortrait;
        currentDilogueList.Clear();
    }

    void Update()
    {
        // Only update when we have dialogue
        if(!IsConversationActive())
            return;

        if(isInHiring)
        {
            if (StateManager.PressedUp()) hiringOption = 0;
            if (StateManager.PressedDown()) hiringOption = 1;

            if (hiringOption == 0)
            {
                dialogueText.text = "Summon?\n    >Yes\n     No";

                if (StateManager.PressedUse())
                {
                    if(developerReference != null)
                    { 
                        isInHiring = false;
                        isPortraitVisible = true;
                        backend.AddActiveDeveloper(developerReference.GetComponent<DeveloperInHell>().developer);
                        ShowNextDialogue();
                    }
                    else
                    {
                        HideDialogue();
                    }
                }
            }
            else
            {
                dialogueText.text = "Summon?\n     Yes\n    >No";

                if (StateManager.PressedUse())
                {
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
