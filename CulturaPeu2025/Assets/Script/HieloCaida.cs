using UnityEngine;

public class HieloCaida : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.instance.PlaySfx("HieloCaida");
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 10f); // Destroy the object after 1 second
    }
}
