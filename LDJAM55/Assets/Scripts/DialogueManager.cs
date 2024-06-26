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
    [SerializeField] Sprite bossPortrait;
    [SerializeField] Image trait1;
    [SerializeField] Image trait2;
    [SerializeField] Image trait3;
    [SerializeField] Sprite[] traitSprites;

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
        if(StateManager.SLOW_MODE_ON) Time.timeScale = 0.0f;
        isConversationActive = true;
        isInHiring = false;
        hiringOption = 0;
        isOpenedOnThisFrame = true;
        isPortraitVisible = false;
        developerReference = developerInHell;
        DataTypes.Developer developer = developerInHell.GetComponent<DeveloperInHell>().developer;

        if(developer.Traits.Count >= 1)
        {
            trait1.sprite = GetTraitIcon(developer.Traits[0]);
        }
        else
        {
            trait1.sprite = emptyPortrait;
        }

        if (developer.Traits.Count >= 2)
        {
            trait2.sprite = GetTraitIcon(developer.Traits[1]);
        }
        else
        {
            trait2.sprite = emptyPortrait;
        }

        if (developer.Traits.Count >= 3)
        {
            trait3.sprite = GetTraitIcon(developer.Traits[2]);
        }
        else
        {
            trait3.sprite = emptyPortrait;
        }
        currentDilogueList = new List<Dialogue>(developer.Dialogue);
        Debug.Log("Dialogue loaded, size of " + currentDilogueList.Count);
        ShowNextDialogue();
    }

    Sprite GetTraitIcon(DataTypes.Developer.Trait trait)
    {
        return traitSprites[(int)trait];
    }

    public void ShowStoryConversation(List<Dialogue> storyDialogue)
    {
        isConversationActive = true;
        isInHiring = false;
        hiringOption = 0;
        isOpenedOnThisFrame = true;
        isPortraitVisible = false;
        developerReference = null;
        currentDilogueList = new List<Dialogue>(storyDialogue);
        trait1.sprite = emptyPortrait;
        trait2.sprite = emptyPortrait;
        trait3.sprite = emptyPortrait;
        Time.timeScale = 0.0f;
        Debug.Log("Dialogue loaded, size of " + currentDilogueList.Count);
        ShowNextDialogue();
    }

    void ShowNextDialogue() {
        PlayNextDialogueAudio();
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
            if(developerReference == null)
            {
                portraitImage.sprite = bossPortrait;
            }
            else
            {
                portraitImage.sprite = currentDilogueList.Count % 2 == 0 ? spiritPortrait1 : spiritPortrait2;
            }
        }
    }

    void StopConversation()
    {
        if(developerReference != null)
        {
            // Destory reference immediately to avoid bugs with MoveHell.cs
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
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        // Darken scene if conversation is active
        dimmer.color = Color.Lerp(dimmer.color, IsConversationActive() ? dimmerColor : Color.clear, Time.unscaledDeltaTime * 3.0f);

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
                    PlaySummonSelectionAudio();
                    StopConversation();
                }
            }
        }
        else if(!isOpenedOnThisFrame)
        {
            if(Time.unscaledTime > textAnimTimer)
            {
                textAnimTimer = Time.unscaledTime + textAnimRate;
                wordsShown++;
                if(wordsShown%5==0 && (wordsShown<currentDialogueWords.Length)) PlaySpiritDialogue();
                
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
            dialogueBox.reverse = true;
            dialogueBox.ResetToStart();
            dialogueBox.Play();
            portraitAnim.reverse = true;
            portraitAnim.ResetToStart();
            portraitAnim.Play();

            isOpenedOnThisFrame = false;
        }
    }
    
    void PlaySpiritDialogue()
    {
        if(developerReference is null) AudioManager.Instance.PlaySound(Random.Range(3,14), transform.position);
        else AudioManager.Instance.PlaySound(Random.Range(14,22), transform.position);
    }
    
    void PlayNextDialogueAudio()
    {
        AudioManager.Instance.PlaySound(Random.Range(23,27), transform.position);
    }

    void PlaySummonSelectionAudio() {
        AudioManager.Instance.PlaySound(27, transform.position);
    }
}
