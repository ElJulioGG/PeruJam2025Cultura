using UnityEngine;

public class ChangeFloorTypeTrigger : MonoBehaviour
{
    [SerializeField] public int newFloorType = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.floorType = newFloorType;
            }


        }
    }
}
