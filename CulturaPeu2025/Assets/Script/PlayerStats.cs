using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] public int playerIndex = 0;
    [SerializeField] private int health = 6;
    public int baseHealth = 6;
    [SerializeField] private int points = 0;
    public bool playerAlive = true;
    [SerializeField] private GameObject[] bloodSplatterPrefabs;
    public static List<GameObject> allSplatters = new List<GameObject>();
    [SerializeField] public bool usingPowerUp;

    [Header("Visual Damage Shake")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float maxShakeDuration = 0.1f;
    [SerializeField] private float maxShakeStrength = 0.2f;
    [SerializeField] private float shakeRandomness = 90f;
    [SerializeField] private float shakeIntensity = 2f;

    [Header("Shake Easing")]
    [SerializeField] private Ease shakeEase = Ease.OutQuad;
    [SerializeField] private AnimationCurve shakeEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool useCurveInsteadOfEase = false;

    [Header("Invulnerability Settings")]
    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] private bool canUseAbility = true;
    [SerializeField] private float invulnerabilityDuration = 3f;
    [SerializeField] private float abilityCooldown = 15f;
    [SerializeField] private float invulnerabilityTimer = 0f;
    [SerializeField] private float cooldownTimer = 0f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color originalColor;

    [Header("Roll Settings")]
    [SerializeField] private float rollDistance = 2f;
    [SerializeField] private float rollDuration = 0.3f;
    [SerializeField] private float rollCooldown = 1f;
    private bool isRolling = false;
    private float rollCooldownTimer = 0f;
    private Vector2 lastMoveDirection = Vector2.down;

    private Rigidbody2D rb;

    void Start()
    {
        health = baseHealth;
        playerAlive = true;
        gameObject.SetActive(true);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!playerAlive) return;

        // Store last movement direction
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 inputDir = new Vector2(moveX, moveY).normalized;
        if (inputDir != Vector2.zero)
            lastMoveDirection = inputDir;

        // Manual invulnerability (shield)
        if (Input.GetMouseButtonDown(1) && canUseAbility)
        {
            ActivateInvulnerability();
        }

        if (isInvulnerable && !isRolling)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0)
                DeactivateInvulnerability();
        }

        if (!canUseAbility)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
                canUseAbility = true;
        }

        // Roll cooldown
        if (rollCooldownTimer > 0)
            rollCooldownTimer -= Time.deltaTime;

        // Roll logic
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && rollCooldownTimer <= 0 && inputDir != Vector2.zero)
        {
            StartCoroutine(PerformRoll(lastMoveDirection));
        }
    }

    private IEnumerator PerformRoll(Vector2 direction)
    {
        isRolling = true;
        isInvulnerable = true;
        rollCooldownTimer = rollCooldown;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.yellow;

        // Apply velocity for rolling using physics
        rb.linearVelocity = direction.normalized * (rollDistance / rollDuration);

        yield return new WaitForSeconds(rollDuration);

        // Stop rolling movement
        rb.linearVelocity = Vector2.zero;
        isInvulnerable = false;
        isRolling = false;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    public void SetPlayerIndex(int index)
    {
        playerIndex = index;
    }

    public IEnumerator AddPointsAfterDelay(int pointsToAdd)
    {
        yield return new WaitForSeconds(1.5f);
        if (!playerAlive) yield break;
        points += pointsToAdd;
    }

    public void TakeDamage(int damageAmount)
    {
        if (!playerAlive) return;
        if (isInvulnerable)
        {
            Debug.Log("Hit absorbed by invulnerability!");
            if (!isRolling) DeactivateInvulnerability();
            return;
        }

        health -= damageAmount;

        if (spriteTransform != null)
        {
            spriteTransform.DOComplete();
            var shakeTween = spriteTransform.DOShakePosition(
                duration: maxShakeDuration * shakeIntensity,
                strength: maxShakeStrength * shakeIntensity,
                vibrato: (int)(5 + 15 * shakeIntensity),
                randomness: shakeRandomness,
                snapping: false,
                fadeOut: true
            );

            if (useCurveInsteadOfEase)
            {
                shakeTween.SetEase(shakeEaseCurve);
            }
            else
            {
                shakeTween.SetEase(shakeEase);
            }
        }

        if (health <= 0 && playerAlive)
        {
            KillPlayer();
        }
    }

    public void SetPlayerHealth(int newHealth) => health = newHealth;
    public int GetPlayerHealth() => health;
    public int GetPlayerIndex() => playerIndex;

    public void UsePowerUp()
    {
        if (playerAlive && !usingPowerUp)
            usingPowerUp = true;
    }

    public void KillPlayer()
    {
        playerAlive = false;
        gameObject.SetActive(false);

        if (playerIndex >= 0 && playerIndex < bloodSplatterPrefabs.Length)
        {
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            GameObject splatter = Instantiate(bloodSplatterPrefabs[playerIndex], transform.position, randomRotation);
            allSplatters.Add(splatter);
        }
    }

    public void ApplyKnockback(Vector2 origin, float force)
    {
        if (rb != null)
        {
            Vector2 knockDirection = (transform.position - (Vector3)origin).normalized;
            rb.AddForce(knockDirection * force, ForceMode2D.Impulse);
        }
    }

    private void ResetShake()
    {
        if (spriteTransform != null)
        {
            spriteTransform.DOComplete();
            spriteTransform.localPosition = Vector3.zero;
        }
    }

    public void Respawn()
    {
        ResetShake();
        health = baseHealth;
        playerAlive = true;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        health = baseHealth;
        playerAlive = true;
    }

    private void ActivateInvulnerability()
    {
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
        canUseAbility = false;
        cooldownTimer = abilityCooldown;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.cyan;
    }

    private void DeactivateInvulnerability()
    {
        isInvulnerable = false;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}
