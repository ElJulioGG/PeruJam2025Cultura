using UnityEngine;
using DG.Tweening;

public class ProyectilCoca : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spinParticlePrefab;

    [Header("Spin Settings")]
    [SerializeField] private float initialSpinSpeed = 180f; // degrees per second
    [SerializeField] private float maxSpinSpeed = 1080f;
    [SerializeField] private float spinAccelerationDuration = 1.5f;

    [Header("Launch Settings")]
    [SerializeField] private float launchDelay = 1f;
    [SerializeField] private float speed = 8f;

    private float currentSpinSpeed;
    private Tween spinTween;
    private Vector2 moveDirection;
    private bool hasLaunched = false;
    private float timer;

    private void OnEnable()
    {
        AudioManager.instance.PlaySfx("Leaves");
        // Spawn particle effect
        if (spinParticlePrefab != null)
            Instantiate(spinParticlePrefab, transform.position, Quaternion.identity, transform);

        currentSpinSpeed = initialSpinSpeed;

        // Kill any existing tween before creating a new one
        spinTween?.Kill();

        // Smoothly increase spin speed
        DOTween.To(() => currentSpinSpeed, x => currentSpinSpeed = x, maxSpinSpeed, spinAccelerationDuration)
            .SetEase(Ease.OutSine);

        // Get direction to player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            moveDirection = (player.transform.position - transform.position).normalized;
        else
            moveDirection = Vector2.right; // default fallback

        hasLaunched = false;
        timer = 0f;
    }

    private void Update()
    {
        // Rotate regardless of launch
        transform.Rotate(0f, 0f, currentSpinSpeed * Time.deltaTime);

        // Delay before launching
        if (!hasLaunched)
        {
            timer += Time.deltaTime;
            if (timer >= launchDelay)
            {
                hasLaunched = true;
            }
        }
        else
        {
            // Move forward
            transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        spinTween?.Kill();
    }
}
