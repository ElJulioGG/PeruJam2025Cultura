using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public float followSpeed = 10f;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Vector2.Lerp(transform.position, mousePos, followSpeed * Time.deltaTime);
    }
}
