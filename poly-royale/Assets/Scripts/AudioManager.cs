using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioClip music;
    private AudioSource musicSource;
    public Sound[] sounds;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        DontDestroyOnLoad(transform.gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
        }
        
        musicSource = this.gameObject.AddComponent<AudioSource>();
        musicSource.clip = music;
        musicSource.loop = true;
        // musicSource.playOnAwake = true;
        musicSource.Play();
    }

    public void Play(string name)
    {
        var s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
}