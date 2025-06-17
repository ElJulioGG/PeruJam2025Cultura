using UnityEngine;
using System.Collections;
using DG.Tweening; // Required for DOTween

public class EnemyStateMachine4 : MonoBehaviour
{
    [SerializeField] private GameObject attackProjectilePrefab;
    [SerializeField] private GameObject defensiveProjectilePrefab;
   // [SerializeField] private float defensiveTriggerRange = 1.5f;

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
    [SerializeField] private float stopDistance = 4f;

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
        hasDied = true;

        int deathIndex = Random.Range(1, 3);
        AudioManager.instance.PlaySfx($"UkukuDeath{deathIndex}");

        rb.linearVelocity = Vector2.zero;
        rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 2f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        PlayAnimation("MisayocIdle");

        float randomTorque = Random.Range(-10f, 10f);
        rb.AddTorque(randomTorque, ForceMode2D.Impulse);

        float upwardForce = Random.Range(400f, 500f);
        rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Force);
    }

    private void Update()
    {
        if (!stats.isAlive)
        {
            if (isAlive) Die();
            return;
        }

        if (rb.linearVelocity.magnitude > 0.1f)
            PlayAnimation("MisayocMove");
        else
            PlayAnimation("MisayocIdle");

        if (!isAttacking)
        {
            FlipSprite(rb.linearVelocity);
            PlayAnimation("MisayocAtack");
        }
            

        if (player == null) return;

        cooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Wander:
                if (CanSeePlayer()) ChangeState(EnemyState.Chase);
                Wander();
                break;

            case EnemyState.ReturnHome:
                if (CanSeePlayer()) ChangeState(EnemyState.Chase);
                Vector2 toLastPosition = lastKnownPlayerPosition - rb.position;
                if (toLastPosition.magnitude <= 0.1f)
                {
                    homePosition = lastKnownPlayerPosition;
                    ChangeState(EnemyState.Wander);
                }
                else rb.linearVelocity = toLastPosition.normalized * wanderSpeed;
                break;

            case EnemyState.Idle:
                rb.linearVelocity = Vector2.zero;
                idleTimer -= Time.deltaTime;
                if (CanSeePlayer()) ChangeState(EnemyState.Chase);
                else if (idleTimer <= 0f) ChangeState(EnemyState.Wander);
                break;

            case EnemyState.Chase:
                if (!CanSeePlayer()) ChangeState(EnemyState.ReturnHome);
                else
                {
                    chaseTimer += Time.deltaTime;
                    MaintainDistanceFromPlayer();

                    if (Vector2.Distance(transform.position, player.position) <= attackRange &&
                        cooldownTimer <= 0 && !isAttacking &&
                        chaseTimer >= minChaseBeforeAttack)
                    {
                        ChangeState(EnemyState.Attack);
                    }
                }
                break;

            case EnemyState.Attack:
                if (!isAttacking) StartCoroutine(RangedAttack());
                break;
        }
    }

    private void MaintainDistanceFromPlayer()
    {
        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance > stopDistance + 0.5f)
            rb.linearVelocity = toPlayer.normalized * chaseSpeed;
        else if (distance < stopDistance - 0.5f)
            rb.linearVelocity = -toPlayer.normalized * chaseSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;

        if (newState != EnemyState.Wander) isResting = false;
        if (newState == EnemyState.Chase) chaseTimer = 0f;
        if (newState == EnemyState.Wander && !isResting) PickNewWanderDirection();
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
                hasSpottedPlayer = false;
        }
    }

    private IEnumerator RangedAttack()
    {
        
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        Vector2 direction = (player.position - transform.position).normalized;
        FlipSprite(direction);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;

        float stepBackTime = 0.5f;
        float stepBackSpeed = 2f;
        float stepTimer = 0f;

        while (stepTimer < stepBackTime)
        {
            rb.linearVelocity = -direction * stepBackSpeed;
            stepTimer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        int dashIndex = Random.Range(1, 4);
        AudioManager.instance.PlaySfx($"UkukuDash{dashIndex}");

        yield return new WaitForSeconds(0.2f);
        PlayAnimation("MisayocAtack2");
        if (attackProjectilePrefab != null)
        {
            GameObject projectile = Instantiate(attackProjectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            if (projRb != null)
                projRb.linearVelocity = direction * lungeSpeed;
            
            // Scale up using DOTween
            transform.DOScale(1.8f, 1f).SetEase(Ease.OutBack);
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        int tiredIndex = Random.Range(1, 3);
        AudioManager.instance.PlaySfx($"UkukuTired{tiredIndex}");

        yield return new WaitForSeconds(recoveryTime);
        yield return new WaitForSeconds(1f);

        if (defensiveProjectilePrefab != null)
        {
            Instantiate(defensiveProjectilePrefab, transform.position, Quaternion.identity);

            // Reset scale using DOTween
            transform.DOScale(1.5f, 0.3f).SetEase(Ease.InOutQuad);
        }

        cooldownTimer = attackCooldown;
        isAttacking = false;
        ChangeState(EnemyState.Chase);
    }

    private void Wander()
    {
        if (isResting)
        {
            rb.linearVelocity = Vector2.zero;
            restTimer -= Time.deltaTime;
            if (restTimer <= 0f)
            {
                isResting = false;
                PickNewWanderDirection();
            }
            return;
        }

        wanderTimer -= Time.deltaTime;
        rb.linearVelocity = wanderDirection * wanderSpeed;

        if (wanderTimer <= 0f)
            StartResting();
    }

    private void StartResting()
    {
        isResting = true;
        restTimer = restTime;
        rb.linearVelocity = Vector2.zero;
    }

    private void FlipSprite(Vector2 velocity)
    {
        if (velocity.x > 0.1f)
            transform.localScale = new Vector3(1.5f, transform.localScale.y, transform.localScale.z);
        else if (velocity.x < -0.1f)
            transform.localScale = new Vector3(-1.5f, transform.localScale.y, transform.localScale.z);
    }

    private void PickNewWanderDirection()
    {
        wanderTimer = wanderTime;
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector2 potentialTarget = rb.position + randomDir * wanderSpeed * wanderTime;

        if (Vector2.Distance(potentialTarget, homePosition) <= maxWanderRadius)
            wanderDirection = randomDir;
        else
            wanderDirection = (homePosition - rb.position).normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? homePosition : transform.position, maxWanderRadius);
    }

    public void PlayDamageSound()
    {
        int dmgIndex = Random.Range(2, 5);
        AudioManager.instance.PlaySfx($"UkukuDamage{dmgIndex}");
    }
}
