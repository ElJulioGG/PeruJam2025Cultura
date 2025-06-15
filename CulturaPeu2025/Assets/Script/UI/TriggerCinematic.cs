using UnityEngine;
using UnityEngine.Playables;

public class TriggerCinematic : MonoBehaviour
{
    public PlayableDirector director;

    public GameObject player;
    public Transform playerTargetPosition;

    public GameObject cameraObj;
    public GameObject petObject; 
    public GameObject petBody;   
    public AudioSource audioSource;
    public AudioSource musicCinematic;

    private bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;
            GameManager.instance.playerCanMove = false;
            GameManager.instance.playerIsInDialog = false;
            director.Play();
            director.stopped += OnTimelineFinished;
            gameObject.SetActive(false);
            audioSource.Stop();
        }
    }

    private void OnTimelineFinished(PlayableDirector obj)
    {
        if (player != null && playerTargetPosition != null)
        {
            player.transform.position = playerTargetPosition.position;
            player.SetActive(true);
        }

        if (cameraObj != null)
            cameraObj.SetActive(true);

        if (petBody != null)
            petBody.SetActive(true);

        if (petObject != null)
        {
            petObject.SetActive(true);

            PetFollower followerScript = petObject.GetComponent<PetFollower>();
            if (followerScript != null)
                followerScript.enabled = true;
        }

        director.stopped -= OnTimelineFinished;
        GameManager.instance.playerCanMove = true;
        audioSource.Play();
        musicCinematic.Stop();
    }
}
