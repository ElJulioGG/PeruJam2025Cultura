using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class AuquiEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2f;
    public float wanderTime = 2f;
    private Vector2 wanderDirection;
    private Vector3 startPosition;
    private float wanderTimer;

    [Header("Ataque")]
    public int damageAmount = 1;
    public float damageInterval = 3f;
    public float attackCooldown = 7f;
    private bool canAttack = true;
    private bool isPlayerInside = false;
    private Transform playerTarget;

    [Header("Detección")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;

    [Header("Sonidos")]
    public AudioClip stepSound;
    public AudioClip attack1Sound;
    public AudioClip attack2Sound;

    [Header("Colliders de ataque (asignar en Inspector)")]
    public Collider2D attackCollider1; // Para venenoso
    public Collider2D attackCollider2; // Para onda

    [Header("Animators de ataques (asignar en Inspector)")]
    public Animator venenosoAnimator; // Animator para el ataque venenoso
    public Animator ondaAnimator;     // Animator para el ataque onda

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Animator animator; // Animator principal de movimiento
    private Coroutine attackRoutine;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        wanderTimer = wanderTime;

        if (attackCollider1 != null) attackCollider1.enabled = false;
        if (attackCollider2 != null) attackCollider2.enabled = false;
    }

    void Update()
    {
        if (playerTarget != null && Vector2.Distance(transform.position, playerTarget.position) <= detectionRadius)
        {
            ChasePlayer();
        }
        else
        {
            if (Vector2.Distance(transform.position, startPosition) > 7f && playerTarget == null)
            {
                ReturnToStart();
            }
            else
            {
                Wander();
            }
        }
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            Vector2 newDir;
            do
            {
                newDir = Random.insideUnitCircle.normalized;
            }
            while (Mathf.Abs(newDir.x) < 0.2f && Mathf.Abs(newDir.y) < 0.2f);

            wanderDirection = newDir;
            wanderTimer = wanderTime;
        }

        if (wanderDirection != Vector2.zero)
        {
            rb.MovePosition(rb.position + wanderDirection * speed * Time.deltaTime);
            animator.SetBool("isWalking", true);
            SmoothFlip(wanderDirection.x);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void ChasePlayer()
    {
        Vector2 dir = playerTarget.position - transform.position;
        rb.MovePosition(rb.position + dir.normalized * speed * Time.deltaTime);
        animator.SetBool("isWalking", true);
        SmoothFlip(dir.x);
    }

    private void ReturnToStart()
    {
        Vector2 dir = startPosition - transform.position;

        if (dir.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + dir.normalized * speed * Time.deltaTime);
            animator.SetBool("isWalking", true);
            SmoothFlip(dir.x);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private IEnumerator DamageLoop(PlayerStats player)
    {
        while (isPlayerInside || playerTarget != null)
        {
            if (canAttack)
            {
                player.TakeDamage(damageAmount);

                // Decidir aleatoriamente qué ataque usar
                bool useAttack1 = Random.value > 0.5f;

                if (useAttack1)
                {
                    animator.SetInteger("attackType", 1);
                    PlaySound(attack1Sound);

                    if (venenosoAnimator != null)
                        venenosoAnimator.SetTrigger("Attack");

                    if (attackCollider1 != null) attackCollider1.enabled = true;
                }
                else
                {
                    animator.SetInteger("attackType", 2);
                    PlaySound(attack2Sound);

                    if (ondaAnimator != null)
                        ondaAnimator.SetTrigger("Attack");

                    if (attackCollider2 != null) attackCollider2.enabled = true;
                }

                yield return new WaitForSeconds(damageInterval);

                // Desactivar colliders después del golpe
                if (attackCollider1 != null) attackCollider1.enabled = false;
                if (attackCollider2 != null) attackCollider2.enabled = false;

                animator.SetInteger("attackType", 0);

                canAttack = false;
                yield return new WaitForSeconds(attackCooldown);
                canAttack = true;
            }
            else
            {
                yield return null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTarget = other.transform;
            isPlayerInside = true;

            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null && attackRoutine == null)
                attackRoutine = StartCoroutine(DamageLoop(stats));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerTarget = null;

            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }

            if (attackCollider1 != null) attackCollider1.enabled = false;
            if (attackCollider2 != null) attackCollider2.enabled = false;

            animator.SetInteger("attackType", 0);
            animator.SetBool("isWalking", false);
        }
    }

    private void SmoothFlip(float dirX)
    {
        if (Mathf.Abs(dirX) > 0.1f)
        {
            float targetScaleX = dirX > 0 ? -1.5f : 1.5f;
            Vector3 targetScale = new Vector3(targetScaleX, 1.5f, 1.5f);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 5f);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void PlayStepSound()
    {
        PlaySound(stepSound);
    }
}
