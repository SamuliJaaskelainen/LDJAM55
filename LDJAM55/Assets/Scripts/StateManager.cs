using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    [SerializeField] AnimateLocalRotation pointerAnim;
    [SerializeField] TextMeshProUGUI startText;
    [SerializeField] TextMeshProUGUI muteText;
    [SerializeField] TextMeshProUGUI volumeText;
    [SerializeField] TextMeshProUGUI slowModeText;
    [SerializeField] GameObject slowModeInfo;
    [SerializeField] TextMeshProUGUI quitText;
    [SerializeField] GameObject quitInfo;

    [SerializeField] List<DialogueManager.Dialogue> storyStart = new List<DialogueManager.Dialogue>();
    [SerializeField] List<DialogueManager.Dialogue> storyAudio = new List<DialogueManager.Dialogue>();

    public static bool SLOW_MODE_ON = false;

    public const float AUDIO_STORY_TIME = 0.4f;
    bool audioStoryShown = false;

    Vector2 pointerOffset = new Vector2(-313.25f, 0.0f);

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
        backend.clearHell = ClearHell;
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
        switch (currentState)
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
        switch (currentState)
        {
            case State.MainMenu:
                ClearHell();
                backend.Reset();
                developerSpawnRate = 0.1f;
                menuIndex = 0;
                audioStoryShown = false;
                UpdateMainMenuGraphics();
                break;

            case State.Gameplay:
                DialogueManager.Instance.ShowStoryConversation(storyStart);
                // Keith TODO: Add game start audio (check with Start game menu audio)
                break;

            case State.Results:
                ClearHell();
                // Keith TODO: Add results shown audio
                break;
        }
    }

    public void ClearHell()
    {
        for (int i = 0; i < spawnedDevelopers.Count; ++i)
        {
            if (spawnedDevelopers[i] != null)
            {
                Destroy(spawnedDevelopers[i]);
            }
        }
        spawnedDevelopers.Clear();
    }

    void UpdateMainMenu()
    {
        if (PressedUp())
        {
            menuIndex--;
            pointerAnim.Play();
            // Keith TODO: Add menu change audio
        }
        if (PressedDown())
        {
            menuIndex++;
            pointerAnim.Play();
            // Keith TODO: Add menu change audio
        }
        menuIndex = Mathf.Clamp(menuIndex, 0, 4);

        switch (menuIndex)
        {
            case 0: // Start game
                if (PressedUse())
                {
                    ChangeState(State.Gameplay);
                    // Keith TODO: Add menu confirm / game starts audio
                }
                break;

            case 1: // Set mute
                if (PressedUse() || PressedRight() || PressedLeft())
                {
                    isMusicMuted = !isMusicMuted;
                    pointerAnim.Play();
                    // Keith TODO: Add menu confirm
                }
                break;

            case 2: // Set volume
                if (PressedRight() || PressedUse())
                {
                    audioVolume++;
                    pointerAnim.Play();
                    // Keith TODO: Add menu change audio
                }
                if (PressedLeft())
                {
                    audioVolume--;
                    pointerAnim.Play();
                    // Keith TODO: Add menu change audio
                }
                audioVolume = Mathf.Clamp(audioVolume, 0, 10);
                break;

            case 3: // Set slow mode
                if (PressedUse() || PressedRight() || PressedLeft())
                {
                    SLOW_MODE_ON = !SLOW_MODE_ON;
                    pointerAnim.Play();
                    // Keith TODO: Add menu confirm
                }
                break;

            case 4: // Quit game
                if (PressedUse()) Application.Quit();
                break;
        }

        if (PressedQuit())
        {
            Application.Quit();
        }

        UpdateMainMenuGraphics();
    }

    void UpdateMainMenuGraphics()
    {
        muteText.text = isMusicMuted ? "Music muted [X]" : "Music muted [  ]";
        volumeText.text = "Audio volume " + audioVolume.ToString();
        slowModeText.text = SLOW_MODE_ON ? "Slow reader mode [X]" : "Slow reader mode [  ]";
        slowModeInfo.SetActive(menuIndex == 3);
        quitInfo.SetActive(menuIndex == 4);
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

            case 3: // Slow mode
                pointer.transform.position = slowModeText.transform.position;
                break;

            case 4: // Quit game
                pointer.transform.position = quitText.transform.position;
                break;
        }
        pointer.anchoredPosition += pointerOffset;
    }

    void UpdateGameplay()
    {
        if (backend.ProductState.TimeLeft <= 0.0f)
        {
            ChangeState(State.Results);
        }
        else if (!audioStoryShown && backend.ProductState.RelativeTimeLeft < AUDIO_STORY_TIME)
        {
            ClearHell();
            DialogueManager.Instance.ShowStoryConversation(storyAudio);
            backend.AllowAudioSpawn = true;
            audioStoryShown = true;
        }

        if (Time.time > developerSpawnTimer)
        {
            spawnedDevelopers.RemoveAll(item => item == null);

            if (spawnedDevelopers.Count < 5)
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
            if (newDev != null)
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

        // Debug key to fast forward time
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            Time.timeScale += 10.0f;
        }
        else if (Input.GetKeyDown(KeyCode.PageDown))
        {
            Time.timeScale = 1.0f;
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
