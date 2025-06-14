using UnityEngine;
using UnityEngine.Playables;

public class TriggerCinematic : MonoBehaviour
{
    public PlayableDirector director;
    public string playerTag = "Player";

    public bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasPlayed && other.CompareTag(playerTag))
        {
            director.Play();
            hasPlayed = true;
        }
    }
}
