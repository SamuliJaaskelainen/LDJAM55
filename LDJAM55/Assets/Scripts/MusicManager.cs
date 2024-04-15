using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource hellLoop;
    private int currentTrackIndex = 0;
    private bool isTrack1Active = true;
    [SerializeField] private Backend backend;
    
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.25f; 
    
    [Range(0f, 1f)]
    [SerializeField] private float hellVolume = 0.25f; 
    
    public StateManager stateManager; // Reference to StateManager
    public static MusicManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        backgroundMusic.volume = musicVolume;
        hellLoop.volume = hellVolume;
        
        PlayMusic(); // Start playing music on scene load

    }
    
    private void PlayMusic()
    {
        AudioClip currentTrack = musicTracks[GetTrackIndex()];
        backgroundMusic.clip = currentTrack;
        backgroundMusic.Play();
        hellLoop.clip = musicTracks[7];
        hellLoop.Play();
        hellLoop.mute = true;


    }

    public void PlayHellMusic() {
        hellLoop.mute = false;
        backgroundMusic.mute = true;
    }
    
    public void PlayBackgroundMusic() {
        hellLoop.mute = true;
        backgroundMusic.mute = false;
    }

    public void MuteAllMusic() {
        hellLoop.volume = 0f;
        backgroundMusic.volume = 0f;
    }
    
    public void UnmuteAllMusic() {
        hellLoop.volume = hellVolume;
        backgroundMusic.volume = musicVolume;
    }

    
    public int GetTrackIndex()
    {
        // if (backend.ProductState.RelativeTimeLeft < StateManager.AUDIO_STORY_TIME)
        if (backend.ProductState.RelativeTimeLeft < 0.1f)
        {
            return 0;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.2f)
        {
            return 0;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.3f)
        {
            return 2;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.4f)
        {
            return 2;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.5f)
        {
            return 3;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.6f)
        {
            return 4;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.7f)
        {
            return 4;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.8f)
        {
            return 4;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.9f)
        {
            return 4;
        }
        else if (backend.ProductState.RelativeTimeLeft < 1.0f)
        {
            return 4;
        }
        else
        {
            return 0;
        }
    }
}
