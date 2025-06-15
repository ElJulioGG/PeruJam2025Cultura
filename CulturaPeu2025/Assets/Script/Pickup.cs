using UnityEngine;

public class Pickup : MonoBehaviour
  
{
    [SerializeField] int pickupType = 0; // 0 = food1, 1 = food2, 2 = food3
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.instance.PlaySfx("Pickup");
            switch (pickupType)
            {
                case 0:
                    GameManager.instance.food1Cuantity++;
                    break;
                case 1:
                    GameManager.instance.food2Cuantity++;
                    break;
                case 2:
                    GameManager.instance.food3Cuantity++;
                    break;
                default:
                    Debug.LogWarning("Unknown pickup type: " + pickupType);
                    break;
            }
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
