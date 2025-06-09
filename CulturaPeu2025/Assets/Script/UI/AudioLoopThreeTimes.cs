using UnityEngine;

public class AudioLoopThreeTimes : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource; 

    [Tooltip("Número de repeticiones (por defecto: 3)")]
    public int repeatCount = 3;

    private int currentCount = 0;
    private bool isPlaying = false;

    void Start()
    {
        if (audioSource != null)
        {
            PlayNext();
        }
        else
        {
            Debug.LogWarning("AudioSource no asignado.");
        }
    }

    void Update()
    {
        if (isPlaying && !audioSource.isPlaying)
        {
            currentCount++;
            if (currentCount < repeatCount)
            {
                PlayNext();
            }
            else
            {
                isPlaying = false;
                Debug.Log("Audio finalizado después de 3 repeticiones.");
            }
        }
    }

    void PlayNext()
    {
        audioSource.Play();
        isPlaying = true;
    }
}
