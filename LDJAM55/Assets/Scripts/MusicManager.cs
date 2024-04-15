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
        AudioClip currentTrack = musicTracks[currentTrackIndex];

        if (isTrack1Active)
        {
            backgroundMusic.clip = currentTrack;
            backgroundMusic.Play();
        } 
        else
        {
            hellLoop.clip = currentTrack;
            hellLoop.Play();
        }

        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length; // Cycle tracks
    }
    
    public void SwitchMusicTracks()
    {
        isTrack1Active = !isTrack1Active;

        if (isTrack1Active) 
        {
            backgroundMusic.mute = true; // Mute the inactive track
            backgroundMusic.mute = false;

        }
        else
        {
            hellLoop.mute = true;
            hellLoop.mute = false;
        }
    }
    
    public void ToggleMusicMute()
    {
        backgroundMusic.mute = !backgroundMusic.mute;
        hellLoop.mute = !hellLoop.mute;
    }
}
