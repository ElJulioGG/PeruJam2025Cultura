using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sounds[] musicSounds, sfxSounds, FootStepsSounds, UISounds, sfxLoopSounds;
    public AudioSource musicSource, sfxSource, FootStepsSource, UISource, sfxLoopSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //musica del menu
    }
    public void PlayMusic(string name)
    {
        Sounds s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    public void PlaySfx(string name)
    {
        Sounds s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip); //hay varios metodos para controlar audio
            //ver la documentacion de unity
        }
    }
    public void PlayFootSteps(string name)
    {
        Sounds s = Array.Find(FootStepsSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            FootStepsSource.PlayOneShot(s.clip);
        }
    }

    public void PlaySfxLoop1(string name)
    {
        Sounds s = Array.Find(sfxLoopSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            sfxLoopSource.clip = s.clip;
            sfxLoopSource.Play();
        }
    }

    public void StopSFXLoop()
    {
        sfxLoopSource.Stop();
    }
    public void PlayUI(string name)
    {
        Sounds s = Array.Find(UISounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            UISource.PlayOneShot(s.clip); //hay varios metodos para controlar audio
            //ver la documentacion de unity
        }
    }
    public void PitchUI(float newPitch)
    {
        UISource.pitch = newPitch;
    }
}

