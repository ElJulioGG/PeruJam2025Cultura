using UnityEngine;
using System.Collections;

public class AuquiOnda : MonoBehaviour
{
    [Header("Paralysis Settings")]
    public float paralysisDuration = 2f;
    public AudioClip paralysisSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Header("Player References - Assign in Inspector")]
    public Movement playerMovement;
    public PlayerStats playerStats;
    public GameManager gameManager;

    [Header("Visual Feedback")]
    public GameObject paralysisEffect;
    public Color paralysisColor = Color.blue;

    private bool canParalyze = true;
    private float cooldownTimer = 0f;
    public float paralysisCooldown = 5f;

    private SpriteRenderer playerSprite;
    private Color originalColor;
    private float originalMaxSpeed;
    private bool originalWeaponState;
    private bool originalCanMove;
    private bool originalCanAction;

    private bool isParalyzing = false;
    private Coroutine paralysisCoroutine;

    public ParticleSystem ps1;
    public ParticleSystem ps2;
    public AudioSource audioSource;

    private void Start()
    {
        InitializeReferences();
        if (ps1 != null) ps1.Stop();
        if (ps2 != null) ps2.Stop();
        if (audioSource != null) audioSource.playOnAwake = false;
    }

    private void InitializeReferences()
    {
        if (playerStats != null && playerSprite == null)
        {
            playerSprite = playerStats.GetComponentInChildren<SpriteRenderer>();
            if (playerSprite != null)
            {
                originalColor = playerSprite.color;
            }
        }

        if (playerMovement != null)
        {
            originalMaxSpeed = playerMovement.maxSpeed;
        }
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canParalyze && cooldownTimer <= 0 && !isParalyzing)
        {
            if (playerMovement != null && playerStats != null && gameManager != null)
            {
                if (paralysisCoroutine != null)
                {
                    StopCoroutine(paralysisCoroutine);
                }
                paralysisCoroutine = StartCoroutine(ParalyzePlayer());
            }
        }
    }

    private IEnumerator ParalyzePlayer()
    {
        isParalyzing = true;
        canParalyze = false;

        SaveOriginalStates();

        if (paralysisSound != null)
        {
            AudioSource.PlayClipAtPoint(paralysisSound, transform.position, soundVolume);
        }

        ApplyParalysis();

        Debug.Log("Jugador paralizado - No puede moverse ni actuar por " + paralysisDuration + " segundos");

        float timer = 0f;
        while (timer < paralysisDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        RestorePlayerState();

        isParalyzing = false;
        cooldownTimer = paralysisCooldown;
        canParalyze = true;

        Debug.Log("Parálisis terminada - Jugador recuperó movimiento y acciones");
    }

    private void SaveOriginalStates()
    {
        originalWeaponState = gameManager.playerHasWeapon;
        originalCanMove = gameManager.playerCanMove;
        originalCanAction = gameManager.playerCanAction;
        originalMaxSpeed = playerMovement.maxSpeed;

        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
        }
    }

    private void ApplyParalysis()
    {
        Rigidbody2D playerRb = playerMovement.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        playerMovement.maxSpeed = 0f;
        playerMovement.SetInputVector(Vector2.zero);

        gameManager.playerHasWeapon = false;
        gameManager.playerCanMove = false;
        gameManager.playerCanAction = false;

        if (playerStats.weapon != null)
        {
            playerStats.weapon.SetActive(false);
        }

        if (playerSprite != null)
        {
            playerSprite.color = paralysisColor;
        }

        if (paralysisEffect != null)
        {
            GameObject effect = Instantiate(paralysisEffect, playerStats.transform.position, Quaternion.identity);
            effect.transform.SetParent(playerStats.transform);
            Destroy(effect, paralysisDuration);
        }
        audioSource.Play();

        ps1.Play();
        ps2.Play();
    }

    private void RestorePlayerState()
    {
        playerMovement.maxSpeed = originalMaxSpeed;
        audioSource.Stop();

        gameManager.playerHasWeapon = originalWeaponState;
        gameManager.playerCanMove = true; // FORZAR a true
        gameManager.playerCanAction = true; // FORZAR a true

        if (playerStats.weapon != null)
        {
            playerStats.weapon.SetActive(originalWeaponState);
        }

        if (playerSprite != null)
        {
            playerSprite.color = originalColor;
        }

        if (gameManager != null)
        {
            gameManager.setPlayerCanMove(true);
        }

        Debug.Log($"Restaurado - Movimiento: {gameManager.playerCanMove}, Acciones: {gameManager.playerCanAction}, Arma: {gameManager.playerHasWeapon}");
        ps1.Stop();
        ps2.Stop();
    }

    public void ForceRestorePlayerState()
    {
        if (isParalyzing)
        {
            if (paralysisCoroutine != null)
            {
                StopCoroutine(paralysisCoroutine);
            }
            RestorePlayerState();
            isParalyzing = false;
            canParalyze = true;
        }
        ps1.Stop();
        ps2.Stop();
        audioSource.Stop();

    }

    private void OnDisable()
    {
        if (isParalyzing)
        {
            ForceRestorePlayerState();
        }
    }

    private void OnDestroy()
    {
        if (isParalyzing)
        {
            ForceRestorePlayerState();
        }
    }

    public void SetPlayerReferences(Movement movement, PlayerStats stats, GameManager manager)
    {
        playerMovement = movement;
        playerStats = stats;
        gameManager = manager;
        InitializeReferences();
    }

    private void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(0.5f, 0f, 1f, 0.3f);

            if (collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
            }
        }
    }
}