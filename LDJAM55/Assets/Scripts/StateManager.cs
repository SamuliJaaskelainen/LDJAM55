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

    float developerSpawnRate = 3.0f;
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
                menuIndex = 0;
                UpdateMainMenuGraphics();
                break;

            case State.Gameplay:
                // TODO: Init Gameplay
                break;

            case State.Results:
                // TODO: Show Ludum Dare stats
                break;
        }
    }

    void UpdateMainMenu()
    {
        if (PressedUp()) menuIndex--;
        if (PressedDown()) menuIndex++;
        menuIndex = Mathf.Clamp(menuIndex, 0, 3);

        switch(menuIndex)
        {
            case 0: // Start game
                if (PressedUse()) ChangeState(State.Gameplay);
                break;

            case 1: // Set mute
                if(PressedUse() || PressedRight() || PressedLeft()) isMusicMuted = !isMusicMuted;
                break;

            case 2: // Set volume
                if (PressedRight() || PressedUse()) audioVolume++;
                if (PressedLeft()) audioVolume--;
                audioVolume = Mathf.Clamp(audioVolume, 0, 10);
                break;

            case 3: // Quit game
                if (Input.GetKeyDown(KeyCode.Space)) Application.Quit();
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
        if(Time.time > developerSpawnTimer)
        {
            developerSpawnTimer = developerSpawnRate + Time.time;
            developerSpawner.TrySpawnDeveloper(backend.FetchDeveloperFromPool());
        }    

        backend.ProgressTick();

        if (PressedQuit())
        {
            ChangeState(State.MainMenu);
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
        
        if (PressedUse() || PressedQuit())
        {
            ChangeState(State.MainMenu);
        }   
    }
}
