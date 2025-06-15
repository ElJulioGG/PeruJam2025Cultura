using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject destroyParticles;
    [SerializeField] private ParticleSystem trailParticles;
    [SerializeField] private GameObject projectilePrefab; // Needed to clone
    [SerializeField] private Transform spriteChild; // Reference to the visual sprite

    public bool slash = false;

    private Vector2 direction;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;

        if (spriteChild != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            spriteChild.rotation = Quaternion.Euler(0f, 0f, angle);

            // Mirror the sprite on the Y axis if facing left
            Vector3 localScale = spriteChild.localScale;
            localScale.y = Mathf.Abs(localScale.y) * (direction.x < 0 ? -1f : 1f);
            spriteChild.localScale = localScale;
        }
    }

    private void Start()
    {
        if (slash)
        {
            int randomIndex = Random.Range(1, 4);
            switch (randomIndex)
            {
                case 1:
                    AudioManager.instance.PlaySfx("Slash1");
                    break;
                case 2:
                    AudioManager.instance.PlaySfx("Slash2");
                    break;
                case 3:
                    AudioManager.instance.PlaySfx("Slash3");
                    break;
            }
        }
        else
        {
            AudioManager.instance.PlaySfx("SpawnAnimal");
        }

        Invoke(nameof(DestroySelf), lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void DestroySelf()
    {
        if (destroyParticles != null)
            Instantiate(destroyParticles, transform.position, Quaternion.identity);

        if (trailParticles != null)
        {
            trailParticles.transform.parent = null;
            trailParticles.Stop();
            Destroy(trailParticles.gameObject, trailParticles.main.duration + trailParticles.main.startLifetime.constantMax);
        }

        // Spawn X-pattern projectiles if slash is enabled
        if (slash && projectilePrefab != null)
        {
            Vector2[] directions = {
                new Vector2(1, 1).normalized,
                new Vector2(-1, 1).normalized,
                new Vector2(1, -1).normalized,
                new Vector2(-1, -1).normalized
            };

            foreach (Vector2 dir in directions)
            {
                GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
                if (ep != null)
                {
                    ep.slash = false; // prevent infinite spawning
                    ep.Initialize(dir);
                }
            }
        }

        AudioManager.instance.PlaySfx("Leaves");
        Destroy(gameObject);
    }
}
