using UnityEngine;

public class AuquiVenenosoAtack : MonoBehaviour
{
    public AudioClip hitSound;
    public string playerTag = "Player";
    public float soundVolume = 1f;
    public float damageCooldown = 5f;

    private AudioSource audioSource;
    private Animator animator;
    private bool canDamage = true;
    private Coroutine damageRoutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && canDamage)
        {
            ApplyDamage();
            damageRoutine = StartCoroutine(DamageCooldown());
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && canDamage)
        {
            ApplyDamage();
            damageRoutine = StartCoroutine(DamageCooldown());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (damageRoutine != null)
            {
                StopCoroutine(damageRoutine);
                damageRoutine = null;
            }
            canDamage = true;
        }
    }

    private void ApplyDamage()
    {
        animator.SetTrigger("Atack");

        if (GameManager.instance != null)
        {
            GameManager.instance.playerHealth -= 1;
            Debug.Log("Jugador golpeado. Vida restante: " + GameManager.instance.playerHealth);
        }

        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound, soundVolume);
        }

        canDamage = false;
    }

    private System.Collections.IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }
}
