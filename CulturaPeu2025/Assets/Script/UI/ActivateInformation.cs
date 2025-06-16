using UnityEngine;

public class ActivateInformation : MonoBehaviour
{
    public GameObject itemInformation;
    private void Start()
    {
        itemInformation.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            itemInformation.SetActive(true);
            Destroy(itemInformation, 12f);
        }
    }
}
