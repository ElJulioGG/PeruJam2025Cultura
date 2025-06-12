using UnityEngine;

public class MeleeDefault : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float knockbackForce = 10f;
    public int Damage => damage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Or check layer
        {
            EnemyStats stats = other.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
                stats.ApplyKnockback(transform.position, knockbackForce);
                gameObject.SetActive(false);
                //Destroy(gameObject); // Or use pooling
            }
        }
    }

}
