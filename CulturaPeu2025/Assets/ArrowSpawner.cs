using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float arrowSpeed = 5f;
    public float cooldown = 2f; 
    private float lastShootTime = -999f;

    void Update()
    {
        if (Time.time >= lastShootTime + cooldown)
        {
            ShootArrow();
            lastShootTime = Time.time;
        }
    }

    void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, transform.position, transform.rotation);

        Vector2 direction = transform.up; 
        Arrow arrowScript = arrow.GetComponent<Arrow>();

        arrowScript.direction = direction;
        arrowScript.speed = arrowSpeed;
    }
}
