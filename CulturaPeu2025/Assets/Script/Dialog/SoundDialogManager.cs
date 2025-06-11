using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDialogManager : MonoBehaviour
{
    public static SoundDialogManager instance { get; private set; }

    private AudioSource source;
    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound)
    {
        source.PlayOneShot(sound);
    }
    public void StopSounds()
    {
        source.Stop();
    }
    public void ChangePitch(float level)
    {
        source.pitch = level;
    }
}
