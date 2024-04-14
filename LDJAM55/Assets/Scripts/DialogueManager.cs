using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] Backend backend;
    [SerializeField] Image dimmer;
    [SerializeField] AnimateLocaPosition dialogueBox;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Image portraitImage;
    [SerializeField] AnimateLocaPosition portraitAnim;
    [SerializeField] Sprite emptyPortrait;
    [SerializeField] Sprite spiritPortrait1;
    [SerializeField] Sprite spiritPortrait2;

    List<Dialogue> currentDilogueList = new List<Dialogue>();
    GameObject developerReference;
    bool isInHiring = false;
    int hiringOption = 0;
    bool isOpenedOnThisFrame = false;
    bool isPortraitVisible = false;
    bool isConversationActive = false;
    Color dimmerColor;
    int wordsShown = 0;
    string[] currentDialogueWords;
    float textAnimRate = 0.04f;
    float textAnimTimer;

    [System.Serializable]
    public struct Dialogue
    {
        [Multiline]
        public string text;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dimmer.gameObject.SetActive(true);
        dimmerColor = dimmer.color;
        dimmer.color = Color.clear;
        dialogueBox.gameObject.SetActive(true);
        dialogueBox.reverse = false;
        dialogueBox.ClamToEnd();
        portraitImage.sprite = emptyPortrait;
        portraitAnim.gameObject.SetActive(true);
        portraitAnim.reverse = false;
        portraitAnim.ClamToEnd();
    }

    public bool IsConversationActive()
    {
        return isConversationActive;
    }

    public void ShowConversation(GameObject developerInHell)
    {
        // TODO: Show trait icons
        dialogueBox.reverse = true;
        dialogueBox.ResetToStart();
        dialogueBox.Play();
        portraitAnim.reverse = true;
        portraitAnim.ResetToStart();
        portraitAnim.Play();
        isConversationActive = true;
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
        // Keith TODO: Add show dialogue audio
        if (currentDilogueList.Count > 0)
        { 
            if(currentDilogueList[0].text == "SUMMON")
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
            StopConversation();
        }
    }

    void ShowDialogue(Dialogue dialogue)
    {
        wordsShown = 1;
        currentDialogueWords = dialogue.text.Split(" ");
        if(isPortraitVisible)
        {
            portraitImage.sprite = developerReference.GetComponent<DeveloperInHell>().developer.portrait;
        }
        else
        {
            portraitImage.sprite = currentDilogueList.Count % 2 == 0 ? spiritPortrait1 : spiritPortrait2;
        }
    }

    void StopConversation()
    {
        if(developerReference != null)
        { 
            DestroyImmediate(developerReference);
        }
        dialogueBox.reverse = false;
        dialogueBox.ResetToStart();
        dialogueBox.Play();
        portraitAnim.reverse = false;
        portraitAnim.ResetToStart();
        portraitAnim.Play();
        currentDilogueList.Clear();
        isConversationActive = false;
    }

    void Update()
    {
        // Darken scene if conversation is active
        dimmer.color = Color.Lerp(dimmer.color, IsConversationActive() ? dimmerColor : Color.clear, Time.deltaTime * 3.0f);

        // Only update when we have dialogue
        if (!IsConversationActive())
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
                        // Keith TODO: Add menu confirm audio
                        isInHiring = false;
                        isPortraitVisible = true;
                        backend.AddActiveDeveloper(developerReference.GetComponent<DeveloperInHell>().developer);
                        ShowNextDialogue();
                    }
                    else
                    {
                        StopConversation();
                    }
                }
            }
            else
            {
                dialogueText.text = "Summon?\n     Yes\n    >No";

                if (StateManager.PressedUse())
                {
                    // Keith TODO: Add menu confirm audio
                    StopConversation();
                }
            }
        }
        else if(!isOpenedOnThisFrame)
        {
            if(Time.time > textAnimTimer)
            {
                textAnimTimer = Time.time + textAnimRate;
                wordsShown++;
                wordsShown = Mathf.Clamp(wordsShown, 0, currentDialogueWords.Length);
                dialogueText.text = "";
                for (int i = 0; i < wordsShown; ++i)
                {
                    dialogueText.text += currentDialogueWords[i];
                    dialogueText.text += " ";
                }
                
            }

            if (StateManager.PressedUse()) ShowNextDialogue();
        }
        else
        {
            isOpenedOnThisFrame = false;
        }
    }
}
