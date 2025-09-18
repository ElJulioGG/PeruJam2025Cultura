using UnityEngine;
using System.Collections;

public class AuquiMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float acceleration = 8f;
    public float deceleration = 12f;

    [Header("Player Detection Settings")]
    public float detectionRadius = 8f;
    public float chaseSpeed = 4f;
    public float attackRange = 1.5f;

    [Header("Random Movement Settings")]
    public float minMoveTime = 2f;
    public float maxMoveTime = 5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    public float movementRadius = 5f;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;

    [Header("References")]
    public Rigidbody2D rb;
    public Transform playerTransform;
    public Animator animator;

    private Vector2 randomDirection;
    private bool isWaiting = false;
    private bool isChasingPlayer = false;
    private bool isAttacking = false;
    private bool canAttack = true;
    private Vector2 currentVelocity;
    private Vector2 initialPosition;
    private Vector3 localScale;
    private float distanceToPlayer;

    // Hashes de parámetros del Animator
    private readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    private readonly int attackTypeHash = Animator.StringToHash("AttackType");

    void Start()
    {
        initialPosition = transform.position;
        localScale = transform.localScale;

        // Buscar el jugador por tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        // Buscar el Animator si no está asignado
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        StartCoroutine(RandomMovementRoutine());
    }

    void FixedUpdate()
    {
        CheckForPlayer();

        if (!isChasingPlayer && !isWaiting && !isAttacking)
        {
            HandleRandomMovement();
        }
        else if (isChasingPlayer && !isAttacking)
        {
            ChasePlayer();
        }

        UpdateAnimations();
    }

    private void LateUpdate()
    {
        if (!isAttacking) // Solo flip si no está atacando
        {
            HandleSpriteFlip();
        }
    }

    private void CheckForPlayer()
    {
        if (playerTransform != null)
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            isChasingPlayer = distanceToPlayer <= detectionRadius;

            // Verificar si puede atacar
            if (isChasingPlayer && distanceToPlayer <= attackRange && canAttack && !isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        canAttack = false;

        // Detener movimiento durante el ataque
        rb.linearVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;

        // Elegir ataque aleatorio (1 o 2)
        int randomAttack = Random.Range(1, 3);
        animator.SetInteger(attackTypeHash, randomAttack);
        animator.SetBool(isAttackingHash, true);

        // Esperar a que la animación de ataque comience
        yield return new WaitForSeconds(0.1f);

        // Esperar mientras la animación de ataque está en progreso
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
               animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            yield return null;
        }

        // Finalizar ataque
        animator.SetBool(isAttackingHash, false);
        isAttacking = false;

        // Cooldown del ataque
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void ChasePlayer()
    {
        if (playerTransform != null && !isAttacking)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Vector2 targetVelocity = directionToPlayer * chaseSpeed;
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, 1f / acceleration);
        }
    }

    private IEnumerator RandomMovementRoutine()
    {
        while (true)
        {
            if (!isChasingPlayer && !isAttacking)
            {
                // Generar dirección aleatoria
                randomDirection = Random.insideUnitCircle.normalized;
                float moveTime = Random.Range(minMoveTime, maxMoveTime);

                // Mover por un tiempo
                float timer = 0f;
                while (timer < moveTime && !isChasingPlayer && !isAttacking)
                {
                    // Verificar que no se aleje demasiado de la posición inicial
                    if (Vector2.Distance(transform.position, initialPosition) > movementRadius)
                    {
                        randomDirection = (initialPosition - (Vector2)transform.position).normalized;
                    }

                    timer += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }

                if (!isChasingPlayer && !isAttacking)
                {
                    // Esperar aleatoriamente
                    isWaiting = true;
                    rb.linearVelocity = Vector2.zero;
                    currentVelocity = Vector2.zero;
                    yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
                    isWaiting = false;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void HandleRandomMovement()
    {
        if (!isAttacking)
        {
            Vector2 targetVelocity = randomDirection * moveSpeed;
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, 1f / acceleration);
        }
    }

    private void HandleSpriteFlip()
    {
        if (rb.linearVelocity.x > 0)
        {
            transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
        }
        else if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
        }
    }

    private void UpdateAnimations()
    {
        // Actualizar velocidad para animación de caminar/correr
        float currentSpeed = rb.linearVelocity.magnitude;
        animator.SetFloat(moveSpeedHash, currentSpeed);

        // Si está atacando, asegurarse de que no se mueva
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Para debug: dibujar los radios
    void OnDrawGizmosSelected()
    {
        // Radio de detección del jugador (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Application.isPlaying ? initialPosition : transform.position, detectionRadius);

        // Radio de ataque (verde)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? initialPosition : transform.position, attackRange);

        // Radio de movimiento aleatorio (amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? initialPosition : transform.position, movementRadius);
    }

    // Método público para detener el movimiento
    public void StopMovement()
    {
        StopAllCoroutines();
        isWaiting = true;
        isChasingPlayer = false;
        isAttacking = false;
        rb.linearVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
        animator.SetBool(isAttackingHash, false);
    }

    // Método público para reanudar el movimiento
    public void ResumeMovement()
    {
        isWaiting = false;
        isChasingPlayer = false;
        isAttacking = false;
        canAttack = true;
        StartCoroutine(RandomMovementRoutine());
    }
}