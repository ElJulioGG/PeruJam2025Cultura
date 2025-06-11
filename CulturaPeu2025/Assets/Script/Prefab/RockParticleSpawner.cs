using UnityEngine;

public class RockParticleSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public int numberOfParticles = 10;
    public float forceMin = 2f;
    public float forceMax = 5f;


    public void Start()
    {
       SpawnParticles(transform.position);
    }

    public void SpawnParticles(Vector2 position)
    {
        for (int i = 0; i < numberOfParticles; i++)
        {
            GameObject rock = Instantiate(rockPrefab, position, Quaternion.identity);
            Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float force = Random.Range(forceMin, forceMax);
            rb.AddForce(randomDir * force, ForceMode2D.Impulse);
        }
    }
}
