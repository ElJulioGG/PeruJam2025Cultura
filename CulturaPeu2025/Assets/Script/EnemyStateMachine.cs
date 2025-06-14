using UnityEngine;
using System.Collections;

public class EnemyStateMachine : MonoBehaviour
{
    private bool hasDied = false;
    private bool isAlive = true;
    public enum EnemyState { Wander, Idle, Chase, Attack, ReturnHome }
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool hasSpottedPlayer = false;

    private EnemyState currentState = EnemyState.Wander;
    private Vector2 homePosition;
    [SerializeField] private float maxWanderRadius = 10f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyStats stats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float wanderSpeed = 1f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float lineOfSightDistance = 8f;

    [Header("Wander")]
    [SerializeField] private float wanderTime = 2f;
    private Vector2 wanderDirection;
    private float wanderTimer;
    private float idleTimer = 1f;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;
    [SerializeField] private float restTime = 2f;
    private bool isResting = false;
    private float restTimer = 0f;
    private Vector2 lastKnownPlayerPosition;
    private string currentAnimation = "";
    [SerializeField] float lungeDuration = 1f;
    [SerializeField] float lungeSpeed = 6f;
    [SerializeField] float recoveryTime = 1f;

    [SerializeField] private float minChaseBeforeAttack = 1f;
    private float chaseTimer = 0f;

    private void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName) return;

        animator.Play(animationName);
        currentAnimation = animationName;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (animator == null) animator = GetComponent<Animator>();

        homePosition = transform.position;

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (player == null && GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player").transform;

        PickNewWanderDirection();
    }

    public void Die()
    {
        isAlive = false;
        ChangeState(EnemyState.Idle);
        //   if (hasDied) return;
        hasDied = true;


        // Play random death sound
        int deathIndex = Random.Range(1, 3); // 1 or 2
        AudioManager.instance.PlaySfx($"UkukuDeath{deathIndex}");

        // Stop movement
        rb.velocity = Vector2.zero;

        // Unlock only rotation
        rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;

        // Increase gravity so enemy falls
        rb.gravityScale = 2f;

        // Ensure Rigidbody is dynamic and not kinematic
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        // Disable all hitboxes / colliders
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        // Change to a non-interacting layer (optional, make sure DeadEnemy exists in your Layer settings)
       // gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        // Play idle animation (or death animation if available)
        PlayAnimation("UKUKUIdle");

        // Apply spin
        float randomTorque = Random.Range(-10f, 10f);
        rb.AddTorque(randomTorque, ForceMode2D.Impulse);
       
        // Apply upward force
        float upwardForce = Random.Range(400f, 500f);
        rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Force);
        Debug.Log($"Applying upward force: {upwardForce}");
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
           

        if (rb.velocity.magnitude > 0.1f)
            PlayAnimation("UkukuMove");
        else
            PlayAnimation("UKUKUIdle");

        if (!isAttacking)
            FlipSprite(rb.velocity);

        if (player == null) return;

        cooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Wander:
                if (CanSeePlayer())
                {
                    ChangeState(EnemyState.Chase);
                    return;
                }
                Wander();
                break;

            case EnemyState.ReturnHome:
                if (CanSeePlayer())
                {
                    ChangeState(EnemyState.Chase);
                    return;
                }

                Vector2 toLastPosition = (lastKnownPlayerPosition - rb.position);
                if (toLastPosition.magnitude <= 0.1f)
                {
                    homePosition = lastKnownPlayerPosition;
                    ChangeState(EnemyState.Wander);
                }
                else
                {
                    rb.velocity = toLastPosition.normalized * wanderSpeed;
                }
                break;

            case EnemyState.Idle:
                rb.velocity = Vector2.zero;
                idleTimer -= Time.deltaTime;
                if (CanSeePlayer())
                {
                    ChangeState(EnemyState.Chase);
                    return;
                }
                else if (idleTimer <= 0f)
                {
                    ChangeState(EnemyState.Wander);
                }
                break;

            case EnemyState.Chase:
                if (!CanSeePlayer())
                {
                    ChangeState(EnemyState.ReturnHome);
                }
                else
                {
                    chaseTimer += Time.deltaTime;

                    ChasePlayer();

                    if (Vector2.Distance(transform.position, player.position) <= attackRange &&
                        cooldownTimer <= 0 &&
                        !isAttacking &&
                        chaseTimer >= minChaseBeforeAttack)
                    {
                        ChangeState(EnemyState.Attack);
                    }
                }
                break;

            case EnemyState.Attack:
                if (!isAttacking)
                    StartCoroutine(AttackLunge());
                break;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;

        if (newState != EnemyState.Wander)
            isResting = false;

        if (newState == EnemyState.Chase)
            chaseTimer = 0f;

        if (newState == EnemyState.Wander && !isResting)
            PickNewWanderDirection();
    }

    private float lostSightTimer = 0f;
    private float lostSightThreshold = 0.5f;

    private bool CanSeePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > lineOfSightDistance)
        {
            HandleLostSight();
            return false;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleMask);
        Debug.DrawRay(transform.position, direction * distance, Color.red);
        if (hit.collider == null)
        {
            lastKnownPlayerPosition = player.position;

            if (!hasSpottedPlayer)
            {
                hasSpottedPlayer = true;
                AudioManager.instance.PlaySfx("UkukuSeePlayer");
            }

            lostSightTimer = 0f;
            return true;
        }

        HandleLostSight();
        return false;
    }

    private void HandleLostSight()
    {
        if (hasSpottedPlayer)
        {
            lostSightTimer += Time.deltaTime;
            if (lostSightTimer >= lostSightThreshold)
            {
                hasSpottedPlayer = false;
            }
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * chaseSpeed;
    }

    private IEnumerator AttackLunge()
    {
        isAttacking = true;

        rb.velocity = Vector2.zero;
        Vector2 direction = (player.position - transform.position).normalized;

        FlipSprite(direction);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;

        float stepBackTime = 0.5f;
        float stepBackSpeed = 2f;
        float stepTimer = 0f;
        while (stepTimer < stepBackTime)
        {
            rb.velocity = -direction * stepBackSpeed;
            stepTimer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;

        int dashIndex = Random.Range(1, 4);
        AudioManager.instance.PlaySfx($"UkukuDash{dashIndex}");

        float timer = 0f;
        while (timer < lungeDuration)
        {
            rb.velocity = direction * lungeSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        int tiredIndex = Random.Range(1, 3);
        AudioManager.instance.PlaySfx($"UkukuTired{tiredIndex}");

        yield return new WaitForSeconds(recoveryTime);

        cooldownTimer = attackCooldown;
        isAttacking = false;

        ChangeState(EnemyState.Chase);
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

        wanderTimer -= Time.deltaTime;
        rb.velocity = wanderDirection * wanderSpeed;

        if (wanderTimer <= 0f)
        {
            StartResting();
        }
    }

    private void StartResting()
    {
        isResting = true;
        restTimer = restTime;
        rb.velocity = Vector2.zero;
    }

    private void FlipSprite(Vector2 velocity)
    {
        if (velocity.x > 0.1f)
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
        else if (velocity.x < -0.1f)
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
    }

    private void PickNewWanderDirection()
    {
        wanderTimer = wanderTime;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector2 potentialTarget = rb.position + randomDir * wanderSpeed * wanderTime;

        if (Vector2.Distance(potentialTarget, homePosition) <= maxWanderRadius)
        {
            wanderDirection = randomDir;
        }
        else
        {
            wanderDirection = (homePosition - rb.position).normalized;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? homePosition : transform.position, maxWanderRadius);
    }

    public void PlayDamageSound()
    {
        int dmgIndex = Random.Range(2, 5); // 2, 3, 4
        AudioManager.instance.PlaySfx($"UkukuDamage{dmgIndex}");
    }
}
