using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private int playerIndex;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPlayerIndex(int index)
    {
        playerIndex = index;
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    void Update()
    {
        // Use old input system for keyboard control
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down arrows
        moveInput = new Vector2(horizontal, vertical).normalized;
    }

    void FixedUpdate()
    {
        Vector2 desiredVelocity = moveInput * maxSpeed;
        Vector2 velocityDiff = desiredVelocity - rb.linearVelocity;

        // Apply force to approach desired velocity
        rb.AddForce(velocityDiff * 10f, ForceMode2D.Force);
    }

    // Optional manual override for moveInput from code
    public void SetInputVector(Vector2 direction)
    {
        moveInput = direction;
    }
}
