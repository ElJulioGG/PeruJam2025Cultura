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
    [SerializeField] private GameObject weapon;

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


    
    [SerializeField] private int shieldCount = 0;

    [SerializeField] private int maxBaseHealth = 6;
    [SerializeField] private int maxTotalHealth = 10;

    [SerializeField] private GameObject shieldHeartPrefab; // Para mostrar el sprite del escudo (si tienes uno)
    [SerializeField] private Transform healthBarParent; // Donde instanciar escudos visuales


    private bool isRolling = false;
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
        GameManager.instance.playerHealth = health;
    }
    private void TryUseHealingItem()
    {
        if (GameManager.instance.food1Cuantity > 0 && health < maxBaseHealth)
        {
            GameManager.instance.food1Cuantity--;
            health = Mathf.Min(health + 2, maxBaseHealth);
            GameManager.instance.playerHealth = health;
            Debug.Log("Used healing item. Current HP: " + health);
        }
        else
        {
            Debug.Log("Can't use healing item. Either none left or HP full.");
        }
    }

    private void TryUseShieldItem()
    {
        if (GameManager.instance.food2Cuantity > 0 && maxBaseHealth + shieldCount < maxTotalHealth)
        {
            GameManager.instance.food2Cuantity--;
            shieldCount++;
            Debug.Log("Used shield item. Shields: " + shieldCount);
            UpdateShieldVisuals();
        }
        else
        {
            Debug.Log("Can't use shield. Either none left or already at max.");
        }
    }

    void Update()
    {
        if (!playerAlive) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            TryUseHealingItem();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            TryUseShieldItem();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            TryUseInvulnerabilityItem();


        // Manual invulnerability (shield)
        if (Input.GetMouseButtonDown(1) && canUseAbility)
        {
            ActivateInvulnerability();
        }

        if (!isRolling && isInvulnerable)
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
        if (!GameManager.instance.playerHasWeapon)
        {
                weapon.SetActive(false);
        }
        else
        {
            
                weapon.SetActive(true);
        }

    }

    public void SetRollingState(bool rolling)
    {
        isRolling = rolling;
        isInvulnerable = rolling || invulnerabilityTimer > 0;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = rolling ? Color.yellow :
                (invulnerabilityTimer > 0 ? Color.cyan : originalColor);
        }
    }
    private void TryUseInvulnerabilityItem()
    {
        if (GameManager.instance.food3Cuantity > 0 && canUseAbility)
        {
            GameManager.instance.food3Cuantity--;
            ActivateInvulnerability();
            Debug.Log("Used invincibility item. Activated invulnerability!");
        }
        else
        {
            Debug.Log("Can't use invincibility item. Either none left or ability on cooldown.");
        }
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
    private void UpdateShieldVisuals()
    {
        if (healthBarParent == null || shieldHeartPrefab == null)
            return;

        // Limpia los antiguos
        foreach (Transform child in healthBarParent)
        {
            if (child.CompareTag("ShieldHeart"))
                Destroy(child.gameObject);
        }

        // Instancia nuevos escudos
        for (int i = 0; i < shieldCount; i++)
        {
            GameObject heart = Instantiate(shieldHeartPrefab, healthBarParent);
            heart.tag = "ShieldHeart";
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (!playerAlive) return;
        if (isInvulnerable)
        {
            Debug.Log("Hit absorbed by invulnerability!");
            return;
        }

        // Use shield first
        if (shieldCount > 0)
        {
            shieldCount--;
            Debug.Log("Shield absorbed damage!");
            AudioManager.instance.PlaySfx("DamageShield");
            UpdateShieldVisuals();
            return;
        }

        health -= damageAmount;
        AudioManager.instance.PlaySfx("Damage");
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
        GameManager.instance.playerHealth = health;
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
        if (!isRolling && spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}
