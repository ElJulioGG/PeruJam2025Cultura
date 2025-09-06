using UnityEngine;

public class hasweapon : MonoBehaviour
{
    [SerializeField] private string weaponName = "DefaultWeapon"; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerHasWeapon = true;

            hasweapon hw = other.GetComponent<hasweapon>();
            if (hw != null)
            {
                hw.enabled = true;
            }
            Destroy(gameObject);
            Debug.Log("Obtuvo");
        }
    }
}
