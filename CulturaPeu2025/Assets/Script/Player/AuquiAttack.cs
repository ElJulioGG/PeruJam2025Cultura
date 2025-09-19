using UnityEngine;
using System.Collections;

public class AuquiAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 8f;
    public int attackDamage = 1;
    public float attackDuration = 2f;

    [Header("External Attack Objects")]
    public GameObject attack1Object; // Objeto completo del ataque 1
    public GameObject attack2Object; // Objeto completo del ataque 2

    [Header("Animation Settings")]
    public string attack1Animation = "Attack1";
    public string attack2Animation = "Attack2";

    [Header("Sound Settings")]
    public AudioClip attack1Sound1;
    public AudioClip attack1Sound2;
    public AudioClip attack2Sound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Header("Audio Source")]
    public AudioSource audioSource; // AudioSource asignado desde el Inspector

    private Transform playerTransform;
    private bool canAttack = true;
    private bool isAttacking = false;
    private float distanceToPlayer;

    // Componentes caché
    private Animator attack1Animator;
    private Animator attack2Animator;
    private Collider2D attack1Collider;
    private Collider2D attack2Collider;

    void Start()
    {
        // Buscar el jugador por tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        // Obtener AudioSource si no está asignado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f; // Sonido 3D
            }
        }

        // Obtener componentes de los objetos de ataque
        InitializeAttackComponents();

        // Desactivar objetos de ataque inicialmente
        if (attack1Object != null) attack1Object.SetActive(false);
        if (attack2Object != null) attack2Object.SetActive(false);
    }

    private void InitializeAttackComponents()
    {
        // Obtener Animators
        if (attack1Object != null)
        {
            attack1Animator = attack1Object.GetComponent<Animator>();
            attack1Collider = attack1Object.GetComponent<Collider2D>();
        }

        if (attack2Object != null)
        {
            attack2Animator = attack2Object.GetComponent<Animator>();
            attack2Collider = attack2Object.GetComponent<Collider2D>();
        }

        // Desactivar colliders inicialmente
        if (attack1Collider != null) attack1Collider.enabled = false;
        if (attack2Collider != null) attack2Collider.enabled = false;
    }

    void Update()
    {
        CheckForAttackOpportunity();
    }

    private void CheckForAttackOpportunity()
    {
        if (playerTransform != null && canAttack && !isAttacking)
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        canAttack = false;

        // Elegir ataque aleatorio (1 o 2)
        int randomAttack = Random.Range(1, 3);
        GameObject currentAttackObject = randomAttack == 1 ? attack1Object : attack2Object;
        Animator currentAnimator = randomAttack == 1 ? attack1Animator : attack2Animator;
        Collider2D currentCollider = randomAttack == 1 ? attack1Collider : attack2Collider;

        // Reproducir sonido según el ataque
        PlayAttackSound(randomAttack);

        // Activar el objeto de ataque
        if (currentAttackObject != null)
        {
            currentAttackObject.SetActive(true);

            // Posicionar el ataque en el enemigo
            currentAttackObject.transform.position = transform.position;
            currentAttackObject.transform.rotation = transform.rotation;
        }

        // Activar animación
        if (currentAnimator != null)
        {
            currentAnimator.SetInteger("AttackType", randomAttack);
            currentAnimator.SetBool("IsAttacking", true);
        }

        // Activar collider de daño después de un pequeño delay
        yield return new WaitForSeconds(0.2f);

        if (currentCollider != null)
        {
            currentCollider.enabled = true;
        }

        // Esperar la duración del ataque
        yield return new WaitForSeconds(attackDuration);

        // Desactivar todo
        DeactivateAttack(currentAttackObject, currentAnimator, currentCollider);

        isAttacking = false;

        // Cooldown del ataque
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void PlayAttackSound(int attackType)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = null;

        switch (attackType)
        {
            case 1: // Attack 1 - Reproducir uno de los dos sonidos aleatoriamente
                if (attack1Sound1 != null && attack1Sound2 != null)
                {
                    clipToPlay = Random.Range(0, 2) == 0 ? attack1Sound1 : attack1Sound2;
                }
                else if (attack1Sound1 != null)
                {
                    clipToPlay = attack1Sound1;
                }
                else if (attack1Sound2 != null)
                {
                    clipToPlay = attack1Sound2;
                }
                break;

            case 2: // Attack 2 - Reproducir el sonido específico
                if (attack2Sound != null)
                {
                    clipToPlay = attack2Sound;
                }
                break;
        }

        if (clipToPlay != null)
        {
            audioSource.volume = soundVolume;
            audioSource.PlayOneShot(clipToPlay);
        }
    }

    private void DeactivateAttack(GameObject attackObject, Animator animator, Collider2D collider)
    {
        // Desactivar animación
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
            animator.Rebind();
            animator.Update(0f);
        }

        // Desactivar collider
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Desactivar objeto
        if (attackObject != null)
        {
            attackObject.SetActive(false);
        }
    }

    // Método para que los colliders llamen cuando golpean al jugador
    public void OnAttackHit(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // Intentar quitar escudo primero
                if (playerStats.RemoveShield())
                {
                    Debug.Log("Escudo removido del jugador");
                }
                else
                {
                    playerStats.TakeDamage(attackDamage);
                    Debug.Log("Daño aplicado al jugador: -" + attackDamage + " vida");
                }
            }
        }
    }

    // Métodos públicos para control externo
    public void StopAllAttacks()
    {
        StopAllCoroutines();
        canAttack = false;
        isAttacking = false;

        // Desactivar todos los ataques
        DeactivateAttack(attack1Object, attack1Animator, attack1Collider);
        DeactivateAttack(attack2Object, attack2Animator, attack2Collider);

        // Detener sonidos
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    // Método para forzar la reproducción de un sonido específico (útil para testing)
    public void PlayTestSound(int attackType)
    {
        PlayAttackSound(attackType);
    }
}