using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateManager : MonoBehaviour
{
    public enum State
    {
        MainMenu,
        Gameplay,
        Results
    }
    State currentState;

    [SerializeField] Backend backend;
    [SerializeField] DeveloperSpawner developerSpawner;
    [SerializeField] GameObject mainMenuParent;
    [SerializeField] GameObject gameplayParent;
    [SerializeField] GameObject resultsParent;

    [SerializeField] RectTransform pointer;
    [SerializeField] TextMeshProUGUI startText;
    [SerializeField] TextMeshProUGUI muteText;
    [SerializeField] TextMeshProUGUI volumeText;
    [SerializeField] TextMeshProUGUI quitText;

    [SerializeField] List<DialogueManager.Dialogue> storyStart = new List<DialogueManager.Dialogue>();

    public static bool PressedUp()
    {
        return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    }

    public static bool PressedDown()
    {
        return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
    }

    public static bool PressedRight()
    {
        return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
    }

    public static bool PressedLeft()
    {
        return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
    }

    public static bool PressedUse()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z);
    }

    public static bool PressedQuit()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    void Start()
    {
        ChangeState(State.MainMenu);
    }

    int menuIndex = 0; 
    int audioVolume = 10; // Keith TODO: Implement volume to audio systems
    bool isMusicMuted = false; // Keith TODO: Implement mute to music
    float menuStateChangedTime;

    List<GameObject> spawnedDevelopers = new List<GameObject>();
    float developerSpawnRate;
    float developerSpawnTimer;

    void Update()
    {
        switch(currentState)
        {
            case State.MainMenu:
                UpdateMainMenu();
                break;

            case State.Gameplay:
                UpdateGameplay();
                break;

            case State.Results:
                UpdateResults();
                break;
        }
    }

    public void ChangeState(State newState)
    {
        currentState = newState;
        Debug.Log("Change state to " + currentState);
        mainMenuParent.SetActive(currentState == State.MainMenu);
        gameplayParent.SetActive(currentState == State.Gameplay);
        resultsParent.SetActive(currentState == State.Results);
        menuStateChangedTime = Time.time;

        // Init new sate
        switch(currentState)
        {
            case State.MainMenu:
                developerSpawnRate = 0.1f;
                menuIndex = 0;
                UpdateMainMenuGraphics();
                for(int i = 0; i < spawnedDevelopers.Count; ++i)
                {
                    if(spawnedDevelopers[i] != null)
                    {
                        Destroy(spawnedDevelopers[i]);
                    }
                }
                spawnedDevelopers.Clear();
                break;

            case State.Gameplay:
                DialogueManager.Instance.ShowStoryConversation(storyStart);
                // Keith TODO: Add game start audio (check with Start game menu audio)
                break;

            case State.Results:
                // Keith TODO: Add results shown audio
                break;
        }
    }

    void UpdateMainMenu()
    {
        if (PressedUp())
        {
            menuIndex--;
            // Keith TODO: Add menu change audio
        }
        if (PressedDown())
        {
            menuIndex++;
            // Keith TODO: Add menu change audio
        }
        menuIndex = Mathf.Clamp(menuIndex, 0, 3);

        switch(menuIndex)
        {
            case 0: // Start game
                if (PressedUse())
                {
                    ChangeState(State.Gameplay);
                    // Keith TODO: Add menu confirm / game starts audio
                }
                break;

            case 1: // Set mute
                if(PressedUse() || PressedRight() || PressedLeft()) 
                {
                    isMusicMuted = !isMusicMuted;
                    // Keith TODO: Add menu confirm
                }
                break;

            case 2: // Set volume
                if (PressedRight() || PressedUse()) 
                {
                    audioVolume++;
                    // Keith TODO: Add menu change audio
                }
                if (PressedLeft())
                {
                    audioVolume--;
                    // Keith TODO: Add menu change audio
                }
                audioVolume = Mathf.Clamp(audioVolume, 0, 10);
                break;

            case 3: // Quit game
                if (PressedUse()) Application.Quit();
                break;
        }

        if (PressedQuit())
        {
            Application.Quit();
        }

        if (Input.anyKeyDown)
        {
            UpdateMainMenuGraphics();
        }
    }

    void UpdateMainMenuGraphics()
    {
        muteText.text = isMusicMuted ? "Music muted [X]" : "Music muted [  ]";
        volumeText.text = "Audio volume " + audioVolume.ToString();
        switch (menuIndex)
        {
            case 0: // Start game
                pointer.transform.position = startText.transform.position;
                break;

            case 1: // Set mute
                pointer.transform.position = muteText.transform.position;
                break;

            case 2: // Set volume
                pointer.transform.position = volumeText.transform.position;
                break;

            case 3: // Quit game
                pointer.transform.position = quitText.transform.position;
                break;
        }
        pointer.anchoredPosition += new Vector2(-288.0f, 0.0f);
    }

    void UpdateGameplay()
    {
        if(backend.ProductState.TimeLeft <= 0.0f)
        {
            ChangeState(State.Results);
        }

        if (Time.time > developerSpawnTimer)
        {
            spawnedDevelopers.RemoveAll(item => item == null);

            if(spawnedDevelopers.Count < 5)
            {
                developerSpawnRate = 0.25f;
            }
            else if (spawnedDevelopers.Count < 10)
            {
                developerSpawnRate = 0.5f;
            }
            else if (spawnedDevelopers.Count < 15)
            {
                developerSpawnRate = 1.0f;
            }
            else
            {
                developerSpawnRate = 2.0f;
            }

            developerSpawnTimer = developerSpawnRate + Time.time;
            GameObject newDev = developerSpawner.TrySpawnDeveloper(backend.FetchDeveloperFromPool());
            if(newDev != null)
            {
                spawnedDevelopers.Add(newDev);
            }
        }    

        backend.ProgressTick();

        if (PressedQuit())
        {
            ChangeState(State.MainMenu);
            // Keith TODO: Add menu confirm audio
        }

        // Debug key to show results
        if (Input.GetKeyDown(KeyCode.End))
        {
            ChangeState(State.Results);
        }
    }

    void UpdateResults()
    {
        if (menuStateChangedTime > (Time.time + 3.0f))
            return;
        
        if (PressedQuit())
        {
            ChangeState(State.MainMenu);
            // Keith TODO: Add menu confirm audio
        }
    }
}
