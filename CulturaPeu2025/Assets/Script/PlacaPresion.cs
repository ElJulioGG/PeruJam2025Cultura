using System.Collections.Generic;
using UnityEngine;

public class PlacaPresion : MonoBehaviour
{
    public Sprite spritePressed;
    public Sprite spriteNormal;
    public AudioSource soundPressed;
    public List<GameObject> objectsToDeactivate;
    public CameraShake cameraShake;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteNormal != null)
        {
            spriteRenderer.sprite = spriteNormal;
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            soundPressed.Play();

            if (spriteRenderer != null && spritePressed != null)
            {
                spriteRenderer.sprite = spritePressed;
            }
            cameraShake.Shake(4);
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
