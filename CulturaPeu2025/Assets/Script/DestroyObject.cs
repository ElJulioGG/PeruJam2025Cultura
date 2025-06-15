using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 1f;
    [SerializeField] private GameObject destroyParticles;

    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    void OnDestroy()
    {
        if (destroyParticles != null)
        {
            AudioManager.instance.PlaySfx("PotBreak");
            Instantiate(destroyParticles, transform.position, Quaternion.identity);
        }
    }
}
