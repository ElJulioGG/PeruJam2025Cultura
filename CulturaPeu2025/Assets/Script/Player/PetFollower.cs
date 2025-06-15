using UnityEngine;

public class PetFollower : MonoBehaviour
{
    public Transform player;
    public float horizontalOffset = 10f;
    public float baseVerticalOffset = 1.5f;
    public float floatAmplitude = 0.5f;
    public float floatSpeed = 2f;
    public float followSpeed = 5f;

    private float lastPlayerX;
    private float floatTimer = 0f;

    void Start()
    {
        if (player != null)
            lastPlayerX = player.position.x;
    }

    void Update()
    {
        if (player == null) return;

        float direction = player.position.x - lastPlayerX;
        float offsetX = direction >= 0 ? -horizontalOffset : horizontalOffset;

        floatTimer += Time.deltaTime * floatSpeed;
        float floatY = Mathf.Sin(floatTimer) * floatAmplitude;

        Vector3 targetPosition = new Vector3(
            player.position.x + offsetX,
            player.position.y + baseVerticalOffset + floatY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        transform.localRotation = direction >= 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

        lastPlayerX = player.position.x;
    }
}
