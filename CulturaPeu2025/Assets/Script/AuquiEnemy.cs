using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class AuquiEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2f;
    public float wanderTime = 2f; // cada cuanto cambia de dirección
    private Vector2 wanderDirection;
    private Vector3 startPosition;

    [Header("Nombres de Bools Animator")]
    public string idleBool = "Idle";
    public string walkBool = "Walk";
    public string attack1Bool = "Attack1";
    public string attack2Bool = "Attack2";

    [Header("Ataque")]
    public float attackCooldown = 2f;
    private bool canAttack = true;
    private bool isAttacking = false;

    [Header("Detección")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;
    private Transform playerTarget;

    [Header("Sonidos")]
    public AudioClip stepSound;
    public AudioClip attack1Sound;
    public AudioClip attack2Sound;

    private Animator anim;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private float wanderTimer;


    public Collider2D attack2Collider;
    public Animator animatorps;

    [Header("Referencias externas")]
    public Movement playerMovement; // arrástralo desde el Inspector
    [SerializeField] private float disableDuration = 2f;
    private float originalPlayerSpeed;


    public Animator ataqueVenenoso;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        wanderTimer = wanderTime;
    }

    void Update()
    {
        if (isAttacking) return;

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
            SetAnimState(false, true, false, false);
            FlipSprite(wanderDirection.x);
        }
        else
        {
            SetAnimState(true, false, false, false);
        }
    }

    private void ChasePlayer()
    {
        Vector2 dir = playerTarget.position - transform.position;

        if (dir.magnitude <= 1.5f && canAttack)
        {
            StartCoroutine(DoAttack());
        }
        else
        {
            rb.MovePosition(rb.position + dir.normalized * speed * Time.deltaTime);
            SetAnimState(false, true, false, false);
            FlipSprite(dir.x);
        }
    }
    private void ReturnToStart()
    {
        Vector2 dir = startPosition - transform.position;

        if (dir.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + dir.normalized * speed * Time.deltaTime);
            SetAnimState(false, true, false, false);
            FlipSprite(dir.x);
        }
        else
        {
            SetAnimState(true, false, false, false);
        }
    }

    private IEnumerator DoAttack()
    {
        canAttack = false;
        isAttacking = true;

        int randomAttack = Random.Range(0, 2);

        if (randomAttack == 0)
        {
            SetAnimState(false, false, true, false);
            PlaySound(attack1Sound);
            ataqueVenenoso.SetTrigger("Atack");
        }
        else
        {
            SetAnimState(false, false, false, true);
            PlaySound(attack2Sound);

            if (attack2Collider != null)
                attack2Collider.enabled = true;
                animatorps.SetTrigger("Shake");
        }

        yield return new WaitForSeconds(attackCooldown);

        if (attack2Collider != null)
            attack2Collider.enabled = false;

        SetAnimState(true, false, false, false);
        isAttacking = false;
        canAttack = true;
    }


    private void SetAnimState(bool idle, bool walk, bool attack1, bool attack2)
    {
        anim.SetBool(idleBool, idle);
        anim.SetBool(walkBool, walk);
        anim.SetBool(attack1Bool, attack1);
        anim.SetBool(attack2Bool, attack2);
    }

    private void FlipSprite(float dirX)
    {
        if (dirX > 0.1f)
        {
            transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f); // derecha
        }
        else if (dirX < -0.1f)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // izquierda
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTarget = other.transform;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTarget = null;
        }
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
