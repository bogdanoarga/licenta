
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public float soundMultiplier;
    void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume * soundMultiplier;
            sound.source.pitch = sound.pitch;
        }
    }

    public void Play(string name)
    {
       Sound sound= Array.Find(sounds, sound => sound.name == name);
        sound.source.Play();
    }
}
