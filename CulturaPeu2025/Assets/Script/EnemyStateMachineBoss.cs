using UnityEngine;
using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;


public class EnemyStateMachineBoss : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        JumpAttack,
        ProjectileAttack,
        ProjectileAttack2,
        ProjectileAttack3,
        ProjectileAttack4,
        ProjectileAttack5
    }
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeAmplitude= 1f;
    [SerializeField] private float shakeFrequency = 2f;
    [SerializeField] private CameraShake2 cameraShake; // Reference to the camera shake script
    //[SerializeField] private CinemachineImpulseSource impulseSource;
    [Header("VFX")]
    [SerializeField] private ParticleSystem attackParticles;
    [SerializeField] private GameObject teleportEffectPrefab;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyStats stats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject attackPrefab2;
    [SerializeField] private GameObject attackPrefab3;
    [SerializeField] private GameObject attackPrefab4;
    [SerializeField] private GameObject iceAtack;

    [Header("Special Projectile Chance")]
    [SerializeField] private GameObject rareAttackPrefab;
    [SerializeField][Range(0f, 1f)] private float rareProjectileChance = 0.2f;

    [Header("Idle Settings")]
    [SerializeField] private float idleTimeBeforeAttack = 5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private Vector3 attackOffset = new Vector3(0, 1f, 0);
    [SerializeField] private Vector3 attackOffset2 = new Vector3(0, 1f, 0);
    [SerializeField] private string jumpStartAnim = "BossJump1";
    [SerializeField] private string jumpEndAnim = "BossJump2";
    [SerializeField] private string burstAttackAnim = "BossAttack";

    private EnemyState currentState = EnemyState.Idle;
    private bool isAttacking = false;
    private bool isAlive = true;
    private string currentAnimation = "";
    private float idleTimer = 0f;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        ChangeState(EnemyState.Idle);
    }
   

    private void Update()
    {
        if (!stats.isAlive)
        {
            if (isAlive)
                Die();
            return;
        }

        if (currentState == EnemyState.Idle && !isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeBeforeAttack)
            {
                idleTimer = 0f;
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    ChangeState(EnemyState.JumpAttack);
                }
                else
                {
                    int projAttackType = Random.Range(0, 5);
                    switch (projAttackType)
                    {
                        case 0: ChangeState(EnemyState.ProjectileAttack); break;
                        case 1: ChangeState(EnemyState.ProjectileAttack2); break;
                        case 2: ChangeState(EnemyState.ProjectileAttack3); break;
                        case 3: ChangeState(EnemyState.ProjectileAttack4); break;
                        case 4: ChangeState(EnemyState.ProjectileAttack5); break;
                    }
                }
            }
        }

        UpdateAnimation();
        FlipSprite(rb.linearVelocity);
    }

    private void ChangeState(EnemyState newState)
    {
        if (isAttacking) return;

        currentState = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                idleTimer = 0f;
                break;
            case EnemyState.JumpAttack:
                StartCoroutine(PerformJumpAttack());
                break;
            case EnemyState.ProjectileAttack:
                StartCoroutine(PerformProjectileAttack());
                break;
            case EnemyState.ProjectileAttack2:
                StartCoroutine(PerformAttack());
                break;
            case EnemyState.ProjectileAttack3:
                StartCoroutine(PerformProjectileAttack3());
                break;
            case EnemyState.ProjectileAttack4:
                StartCoroutine(PerformProjectileAttack4());
                break;
            case EnemyState.ProjectileAttack5:
                StartCoroutine(PerformProjectileAttack5());
                break;
        }
    }

    private GameObject GetProjectile(GameObject defaultPrefab)
    {
        if (defaultPrefab == iceAtack || rareAttackPrefab == null)
            return defaultPrefab;

        return Random.value < rareProjectileChance ? rareAttackPrefab : defaultPrefab;
    }

    private IEnumerator PerformProjectileAttack5()
    {
        isAttacking = true;
        PlayAnimation(burstAttackAnim);

        GameObject[] attackOptions = { attackPrefab, attackPrefab2, attackPrefab3, attackPrefab4, iceAtack };
        int totalProjectiles = 6;
        float timeBetweenShots = 0.25f;

        for (int i = 0; i < totalProjectiles; i++)
        {
            if (player != null && stats.isAlive)
            {
                GameObject chosenAttack = attackOptions[Random.Range(0, attackOptions.Length)];
                Vector3 spawnPos;
                GameObject projectile;

                if (chosenAttack == iceAtack)
                {
                    spawnPos = player.position + attackOffset2;
                    projectile = Instantiate(chosenAttack, spawnPos, Quaternion.identity);
                }
                else
                {
                    spawnPos = transform.position + attackOffset;
                    projectile = Instantiate(GetProjectile(chosenAttack), spawnPos, Quaternion.identity);

                    EnemyProjectile ep = projectile.GetComponent<EnemyProjectile>();
                    if (ep != null)
                    {
                        Vector2 dirToPlayer = (player.position - spawnPos).normalized;
                        ep.Initialize(dirToPlayer);
                    }
                }

                AudioManager.instance.PlaySfx("CastAnimal");
                yield return new WaitForSeconds(timeBetweenShots);
            }
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        PlayAnimation("BossIdle");
        ChangeState(EnemyState.Idle);
    }

    private IEnumerator PerformProjectileAttack4()
    {
        isAttacking = true;
        PlayAnimation(burstAttackAnim);

        for (int i = 0; i < 6; i++)
        {
            if (player != null && stats.isAlive)
            {
                GameObject[] prefabs = { attackPrefab3, attackPrefab4 };
                GameObject chosenPrefab = prefabs[i % 2];
                Vector3 spawnPos = transform.position + attackOffset;
                GameObject proj = Instantiate(GetProjectile(chosenPrefab), spawnPos, Quaternion.identity);

                EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
                if (ep != null)
                {
                    Vector2 dir = (player.position - spawnPos).normalized;
                    ep.Initialize(dir);
                }

                AudioManager.instance.PlaySfx("CastAnimal");
            }
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        PlayAnimation("BossIdle");
        ChangeState(EnemyState.Idle);
    }

    private IEnumerator PerformProjectileAttack3()
    {
        isAttacking = true;
        PlayAnimation(burstAttackAnim);

        for (int i = 0; i < 8; i++)
        {
            if (player != null && stats.isAlive)
            {
                GameObject chosenPrefab = (Random.value < 0.5f) ? attackPrefab : attackPrefab2;
                Vector3 spawnPos = transform.position + attackOffset;
                GameObject proj = Instantiate(GetProjectile(chosenPrefab), spawnPos, Quaternion.identity);

                EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
                if (ep != null)
                {
                    Vector2 dir = (player.position - spawnPos).normalized;
                    ep.Initialize(dir);
                }

                AudioManager.instance.PlaySfx("CastAnimal");
            }
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        PlayAnimation("BossIdle");
        ChangeState(EnemyState.Idle);
    }

    private IEnumerator PerformJumpAttack()
    {
        isAttacking = true;
        PlayAnimation(jumpStartAnim);

        int jumpIndex = Random.Range(1, 4);
        AudioManager.instance.PlayUI($"BossDash{jumpIndex}");
        AudioManager.instance.PlaySfx("BossJumpStart");
        AudioManager.instance.PlayUI("Up");
        //  Shake suave al iniciar salto
        cameraShake.ShakeFromScript(shakeAmplitude, shakeFrequency, shakeDuration);

        yield return transform.DOMoveY(transform.position.y + 15f, 0.5f).SetEase(Ease.OutQuad).WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        if (teleportEffectPrefab != null)
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);

        if (player != null)
        {
            Vector3 teleportTarget = player.position + new Vector3(0, 15f, 0);
            transform.position = teleportTarget;
            rb.position = teleportTarget;
        }

        if (teleportEffectPrefab != null)
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);

        AudioManager.instance.PlaySfx("UkukuTeleport");
        AudioManager.instance.PlayUI("Fall");
        yield return transform.DOMoveY(transform.position.y - 15f, 0.5f).SetEase(Ease.InQuad).WaitForCompletion();

        PlayAnimation(jumpEndAnim);
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = true;

        // Shake fuerte al aterrizar
        AudioManager.instance.PlaySfx("BossLandImpact");
        // impulseSource.GenerateImpulse(1f); // impulso más fuerte
        cameraShake.ShakeFromScript(shakeAmplitude*2, shakeFrequency*2, shakeDuration*2);
        AudioManager.instance.PlayUI("Floor");

        yield return new WaitForSeconds(1f);

        isAttacking = false;
        PlayAnimation("BossIdle");
        ChangeState(EnemyState.Idle);
    }


    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        PlayAnimation("BossAtack2");

        if (attackParticles != null) attackParticles.Play();

        float delay = attackCooldown / Mathf.Max(1, 10);
        for (int i = 0; i < 10; i++)
        {
            if (player != null && stats.isAlive)
            {
                Vector3 spawnPos = player.position + attackOffset2;
                Instantiate(iceAtack, spawnPos, Quaternion.identity);

                AudioManager.instance.PlaySfx("CastAnimal");
            }
            yield return new WaitForSeconds(delay);
        }

        if (attackParticles != null) attackParticles.Stop();

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        PlayAnimation("BossIdle");
        ChangeState(EnemyState.Idle);
    }

    private IEnumerator PerformProjectileAttack()
    {
        isAttacking = true;
        PlayAnimation(burstAttackAnim);

        int totalBursts = 2;
        int projectilesPerBurst = 3;
        float timeBetweenShots = 0.3f;

        for (int b = 0; b < totalBursts; b++)
        {
            for (int i = 0; i < projectilesPerBurst; i++)
            {
                if (player != null && stats.isAlive)
                {
                    Vector3 spawnPos = transform.position + attackOffset;
                    GameObject projectile = Instantiate(GetProjectile(attackPrefab), spawnPos, Quaternion.identity);

                    EnemyProjectile ep = projectile.GetComponent<EnemyProjectile>();
                    if (ep != null)
                    {
                        Vector2 dirToPlayer = (player.position - spawnPos).normalized;
                        ep.Initialize(dirToPlayer);
                    }

                    AudioManager.instance.PlaySfx("CastAnimal");
                }
                yield return new WaitForSeconds(timeBetweenShots);
            }
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        PlayAnimation("BossIdle");
        ChangeState(EnemyState.Idle);
    }

    private void Die()
    {
        isAlive = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = true;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            if (col.isTrigger)
            {
                col.enabled = false;
            }
            
        }

        PlayAnimation("BossDeath");
        LoadSceneCinematic();
    }
    public void LoadSceneCinematic()
    {
        StartCoroutine(LoadSceneWithDelay("CinematicaFinal"));

        AudioManager.instance.musicSource.Stop();

    }
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(5f);
        AudioManager.instance.sfxSource.Stop();
        AudioManager.instance.UISource.Stop();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

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

    private void UpdateAnimation()
    {
        if (isAttacking || !isAlive)
            return;

        if (rb.linearVelocity.magnitude > 0.1f)
            PlayAnimation("Ukuku2Move");
        else
            PlayAnimation("Ukuku2Idle");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
