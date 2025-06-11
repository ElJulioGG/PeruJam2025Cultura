using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogAudioInfo", menuName = "ScriptableObjects/AudioInfoSO", order = 1)]
public class AudioInfoSO : ScriptableObject
{
    public string id;
    public AudioClip[] sounds;
    [Range(1, 5)]
    public int soundPerCharFrequency = 2;
    [Range(-3, 3)]
    public float minPitch = 0.5f;
    [Range(-3, 3)]
    public float maxPitch = 1.5f;
    public bool makePredictable; //choose beetween random aproach or hash aproach
}
