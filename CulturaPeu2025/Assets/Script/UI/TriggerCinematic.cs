using UnityEngine;
using UnityEngine.Playables;

public class TriggerCinematic : MonoBehaviour
{
    public PlayableDirector director;

    public GameObject player;
    public Transform playerTargetPosition;

    public GameObject cameraObj;
    public GameObject petObject; // El GameObject que tiene el script PetFollower
    public GameObject petBody;   // El cuerpo visual de la mascota, si quieres activarlo por separado

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
    }
}
