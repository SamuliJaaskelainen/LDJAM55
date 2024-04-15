using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks;
    public AudioSource backgroundMusic;
    public AudioSource hellLoop;
    private int currentTrackIndex = 0;
    private bool isTrack1Active = true;
    [SerializeField] private Backend backend;
    
    public StateManager stateManager; // Reference to StateManager
    public static MusicManager Instance;

    public const float MUSIC_VOLUME_MAX = 0.15f;
    public const float HELL_VOLUME_MAX = 0.1f;

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

        backgroundMusic.volume = MUSIC_VOLUME_MAX;
        hellLoop.volume = HELL_VOLUME_MAX;
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
        hellLoop.enabled = false;
        backgroundMusic.enabled = false;
    }
    
    public void UnmuteAllMusic() {
        hellLoop.enabled = true;
        backgroundMusic.enabled = true;
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
            return 1;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.4f)
        {
            return 1;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.5f)
        {
            return 2;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.6f)
        {
            return 3;
        }
        else if (backend.ProductState.RelativeTimeLeft < 0.7f) {
            return 3;
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
            return 4;
        }
    }

    public void PlayResultMusic()
    {
        hellLoop.mute = true;
        backgroundMusic.mute = false;
        backgroundMusic.clip = musicTracks[5];
        backgroundMusic.Play();
    }

    public void PlayMainMenuMusic()
        {
        hellLoop.mute = true;
        backgroundMusic.mute = false;
        backgroundMusic.clip = musicTracks[8];
        backgroundMusic.Play();
    }
    
    public void UpdateBackgroundMusicIndex()
    {
        AudioClip currentTrack = musicTracks[GetTrackIndex()];
        if(backgroundMusic.clip != currentTrack)
        { 
            backgroundMusic.clip = currentTrack;
            backgroundMusic.Play();
        }
    }
} 
