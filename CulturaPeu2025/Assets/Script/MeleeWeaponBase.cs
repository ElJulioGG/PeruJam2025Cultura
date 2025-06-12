using UnityEngine;

public class MeleeWeaponBase : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;
    [SerializeField] private float activeTime = 0.1f;
    [SerializeField] private float recoveryTime = 0.3f;

    private bool isAttacking = false;
    private Vector2 aimDirection = Vector2.right;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (hitbox != null)
            hitbox.SetActive(false);
    }

    void Update()
    {
        if (mainCamera == null) return;

        // Get mouse position in world space (2D-safe)
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Get direction from weapon to mouse
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        SetAimDirection(direction);

        // Left mouse button attack
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        // Debug line to visualize direction
        Debug.DrawLine(transform.position, transform.position + (Vector3)aimDirection * 2f, Color.red);
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    public void SetAimDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.1f)
        {
            aimDirection = dir.normalized;

            // Rotate the weapon to face the direction
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Optional: Flip the hitbox vertically if aiming left
            Vector3 scale = transform.localScale;
            scale.y = dir.x < 0 ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            transform.localScale = scale;
        }
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (hitbox != null)
            hitbox.SetActive(true);

        yield return new WaitForSeconds(activeTime);

        if (hitbox != null)
            hitbox.SetActive(false);

        yield return new WaitForSeconds(recoveryTime);

        isAttacking = false;
    }
}
