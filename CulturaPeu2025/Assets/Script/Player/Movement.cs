using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float rollForce = 10f;
    [SerializeField] private float rollDuration = 0.3f;
    [SerializeField] private float rollCooldown = 1f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform spriteHolder;

    [Header("Stats Reference")]
    [SerializeField] private PlayerStats playerStats;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    private bool isRolling = false;
    private float rollCooldownTimer = 0f;
    private Vector2 lastMoveDirection = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isRolling)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(horizontal, vertical).normalized;

            if (moveInput != Vector2.zero)
                lastMoveDirection = moveInput;

            if (Input.GetKeyDown(KeyCode.Space) && rollCooldownTimer <= 0 && moveInput != Vector2.zero)
            {
                StartCoroutine(PerformRoll(lastMoveDirection));
                return;
            }

            HandleAnimation(moveInput);
        }
        else
        {
            // Reapply the proper animation after roll ends
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("TuristaRoll") &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isRolling = false;

                if (playerStats != null)
                    playerStats.SetRollingState(false);

                // Play correct post-roll animation
                HandleAnimation(moveInput);
            }
        }

        if (rollCooldownTimer > 0)
            rollCooldownTimer -= Time.deltaTime;
    }


    void FixedUpdate()
    {
        if (isRolling) return;

        Vector2 desiredVelocity = moveInput * maxSpeed;
        Vector2 velocityDiff = desiredVelocity - rb.linearVelocity;
        rb.AddForce(velocityDiff * 10f, ForceMode2D.Force);
    }

    private void HandleAnimation(Vector2 direction)
    {
        bool hasWeapon = GameManager.instance != null && GameManager.instance.playerHasWeapon;

        if (direction == Vector2.zero)
        {
            string idleAnim = hasWeapon ? "TuristaIdleNoArms" : "TuristaIdle";
            animator.Play(idleAnim);
            return;
        }

        if (hasWeapon)
        {
            // Only one walk anim when no arms
            animator.Play("TuristaWalkNoArms");

            if (spriteHolder != null && direction.x != 0)
                spriteHolder.localScale = new Vector3(direction.x < 0 ? -1 : 1, 1, 1);
        }
        else
        {
            string prefix = "TuristaWalk";

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                animator.Play(prefix + "Rigth");

                if (spriteHolder != null)
                    spriteHolder.localScale = new Vector3(direction.x < 0 ? -1 : 1, 1, 1);
            }
            else
            {
                if (direction.y > 0)
                    animator.Play(prefix + "Up");
                else
                    animator.Play(prefix + "Down");
            }
        }
    }



    private IEnumerator PerformRoll(Vector2 direction)
    {
        isRolling = true;
        rollCooldownTimer = rollCooldown;

        if (animator != null)
        {
            bool hasWeapon = GameManager.instance != null && GameManager.instance.playerHasWeapon;
            string rollAnim = hasWeapon ? "TuristaRoll" : "TuristaRoll";
            animator.Play(rollAnim);
        }

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * rollForce, ForceMode2D.Impulse);

        if (playerStats != null)
            playerStats.SetRollingState(true);

        yield return new WaitForSeconds(rollDuration);

        isRolling = false;

        if (playerStats != null)
            playerStats.SetRollingState(false);

        HandleAnimation(moveInput); // ensure animation updates right after roll ends
    }


    public void SetInputVector(Vector2 direction)
    {
        moveInput = direction;
    }
}
