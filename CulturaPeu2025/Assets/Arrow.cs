using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction;
    public int damage = 1;
    public string playerTag = "Player";

    private void Start()
    {
        // Destruye la flecha automáticamente a los 2 segundos
        Destroy(gameObject, 6f);
    }

    private void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            GameManager.instance.playerHealth -= damage;
            GameManager.instance.playerIsHit = true;

            if (GameManager.instance.playerHealth <= 0)
            {
                GameManager.instance.playerDied = true;
                GameManager.instance.playerCanMove = false;
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
