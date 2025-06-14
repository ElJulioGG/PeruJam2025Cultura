using UnityEngine;
using System.Collections;
using UnityEngine.Splines;

public class RangedEnemyStateMachine : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem attackParticles;

    private enum EnemyState { Wander, Idle, Chase, Attack, ReturnHome }

    private EnemyState currentState = EnemyState.Wander;
    private Vector2 homePosition;
    private bool isAttacking = false;
    private bool isResting = false;
    private string currentAnimation = "";

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyStats stats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject attackPrefab;

    [Header("Wander Settings")]
    [SerializeField] private float wanderSpeed = 1f;
    [SerializeField] private float maxWanderRadius = 10f;
    [SerializeField] private float wanderTime = 2f;
    [SerializeField] private float restTime = 2f;

    [Header("Chase Settings")]
    [SerializeField] private float stopDistance = 4f;
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float lineOfSightDistance = 8f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float attackDuration = 2.5f;
    [SerializeField] private int attackCount = 3;
    [SerializeField] private float timeBetweenAttacks = 0.5f;
    [SerializeField] private Vector3 attackOffset = new Vector3(0, 1f, 0);
    [SerializeField] private string attackAnimationName = "Ukuku2Atack";
    [SerializeField] private string moveAnimationName = "Ukuku2Move";
    [SerializeField] private string idleAnimationName = "Ukuku2Idle";
    [SerializeField] private string onSeePlayerSfx = "UkukuSeePlayer";
    [SerializeField] private string onCastSfx = "UkukuCast";

    private float cooldownTimer = 0f;
    private Vector2 wanderDirection;
    private float wanderTimer = 0f;
    private float restTimer = 0f;
    private bool hasSpottedPlayer = false;

    private void Start()
    {
        homePosition = transform.position;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        PickNewWanderDirection();
    }
    private bool isAlive = true;
    public void Die()
    {
        isAlive = false;
        ChangeState(EnemyState.Idle);
     


        // Play random death sound
        int deathIndex = Random.Range(1, 3); // 1 or 2
        AudioManager.instance.PlaySfx($"UkukuDeath{deathIndex}");

        // Stop movement
        rb.velocity = Vector2.zero;

      
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = true;

        // Disable all hitboxes / colliders
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

       
        PlayAnimation("Ukuku2Death");

      
    }
    private void Update()
    {
        if (!stats.isAlive)
        {
            if (isAlive)
            {
                Die();

            }

            return;
        }

        cooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Wander:
                if (CanSeePlayer())
                    ChangeState(EnemyState.Chase);
                else
                    Wander();
                break;

            case EnemyState.Idle:
                rb.velocity = Vector2.zero;
                if (CanSeePlayer())
                    ChangeState(EnemyState.Chase);
                else if ((restTimer -= Time.deltaTime) <= 0f)
                    ChangeState(EnemyState.Wander);
                break;

            case EnemyState.Chase:
                if (!CanSeePlayer())
                {
                    ChangeState(EnemyState.ReturnHome);
                }
                else
                {
                    MaintainDistanceFromPlayer();
                    if (cooldownTimer <= 0 && !isAttacking)
                        ChangeState(EnemyState.Attack);
                }
                break;

            case EnemyState.Attack:
                if (!isAttacking)
                    StartCoroutine(PerformAttack());
                break;

            case EnemyState.ReturnHome:
                if (CanSeePlayer())
                {
                    ChangeState(EnemyState.Chase);
                    return;
                }

                Vector2 toHome = homePosition - rb.position;
                if (toHome.magnitude < 0.1f)
                    ChangeState(EnemyState.Wander);
                else
                    rb.velocity = toHome.normalized * wanderSpeed;
                break;
        }

        if (rb.velocity.magnitude > 0.1f)
            PlayAnimation(moveAnimationName);
        else
            PlayAnimation(idleAnimationName);

        FlipSprite(rb.velocity);
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        if (newState == EnemyState.Wander)
        {
            PickNewWanderDirection();
            isResting = false;
        }
        if (newState == EnemyState.Idle)
        {
            restTimer = restTime;
        }
        if (newState == EnemyState.Attack)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void Wander()
    {
        if (isResting)
        {
            rb.velocity = Vector2.zero;
            restTimer -= Time.deltaTime;
            if (restTimer <= 0f)
            {
                isResting = false;
                PickNewWanderDirection();
            }
            return;
        }

        rb.velocity = wanderDirection * wanderSpeed;
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            isResting = true;
            restTimer = restTime;
        }
    }

    private void PickNewWanderDirection()
    {
        wanderTimer = wanderTime;
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector2 target = rb.position + randomDir * wanderSpeed * wanderTime;

        if (Vector2.Distance(target, homePosition) <= maxWanderRadius)
            wanderDirection = randomDir;
        else
            wanderDirection = (homePosition - rb.position).normalized;
    }

    private void MaintainDistanceFromPlayer()
    {
        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance > stopDistance + 0.5f)
            rb.velocity = toPlayer.normalized * chaseSpeed;
        else if (distance < stopDistance - 0.5f)
            rb.velocity = -toPlayer.normalized * chaseSpeed;
        else
            rb.velocity = Vector2.zero;
    }

    private bool CanSeePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > lineOfSightDistance) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleMask);
        Debug.DrawRay(transform.position, direction * distance, Color.red);

        if (hit.collider == null)
        {
            if (!hasSpottedPlayer)
            {
                hasSpottedPlayer = true;
                if (!string.IsNullOrEmpty(onSeePlayerSfx))
                    AudioManager.instance.PlaySfx(onSeePlayerSfx);
            }
            return true;
        }
        return false;
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        PlayAnimation(attackAnimationName);

        if (attackParticles != null) attackParticles.Play();

        float delay = attackDuration / Mathf.Max(1, attackCount);
        for (int i = 0; i < attackCount; i++)
        {
            if (player != null && stats.isAlive)
            {
                Vector3 spawnPos = player.position + attackOffset;
                Instantiate(attackPrefab, spawnPos, Quaternion.identity);

                if (!string.IsNullOrEmpty(onCastSfx))
                    AudioManager.instance.PlaySfx(onCastSfx);
            }
            yield return new WaitForSeconds(delay);
        }

        cooldownTimer = attackCooldown;

        // etiene partículas al terminar
        if (attackParticles != null) attackParticles.Stop();

        isAttacking = false;
        ChangeState(EnemyState.Chase);
    }


    private void FlipSprite(Vector2 velocity)
    {
        if (velocity.x > 0.1f)
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
        else if (velocity.x < -0.1f)
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
    }

    private void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName) return;
        animator.Play(animationName);
        currentAnimation = animationName;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? homePosition : transform.position, maxWanderRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
