using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int health = 3;
    public int baseHealth = 3;
    public bool isAlive = true;

    [SerializeField] private GameObject bloodSplatterPrefab;

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
    [SerializeField] private float knockbackDuration = 0.2f;
    //[SerializeField] private EnemyStateMachine enemyStateMachine;
    private float knockbackTimer;

    public bool IsKnockedBack => knockbackTimer > 0;

    public void ApplyKnockback(Vector2 origin, float force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockDirection = (transform.position - (Vector3)origin).normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(knockDirection * force, ForceMode2D.Impulse);
            knockbackTimer = knockbackDuration;
        }
    }

    private void Start()
    {
        health = baseHealth;
        isAlive = true;
    }

    private void Update()
    {
        if (knockbackTimer > 0f)
            knockbackTimer -= Time.deltaTime;
    }

    public void TakeDamage(int damageAmount)
    {
        if (!isAlive) return;

        // Sonido de daño aleatorio
        int dmgIndex = Random.Range(2, 5); // 2, 3, 4
        AudioManager.instance.PlaySfx($"UkukuDamage{dmgIndex}");

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
                shakeTween.SetEase(shakeEaseCurve);
            else
                shakeTween.SetEase(shakeEase);
        }

        if (health <= 0 && isAlive)
        {
            
            
                
            
            KillEnemy();
        }
    }

    public void KillEnemy()
    {
        isAlive = false;
        //gameObject.SetActive(false);

        if (bloodSplatterPrefab != null)
        {
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            Instantiate(bloodSplatterPrefab, transform.position, randomRotation);
        }

        //  Sonido de muerte aleatorio
        int deathIndex = Random.Range(1, 3); // 1 o 2
        AudioManager.instance.PlaySfx($"UkukuDeath{deathIndex}");
       // Destroy(gameObject, 5f); // Delay to allow sound to play
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }

    public int GetHealth()
    {
        return health;
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
        isAlive = true;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        health = baseHealth;
        isAlive = true;
    }
}
