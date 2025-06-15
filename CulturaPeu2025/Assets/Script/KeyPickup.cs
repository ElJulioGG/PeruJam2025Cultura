using UnityEngine;
using DG.Tweening;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private int itemIndex;

    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return; // Prevent double-trigger
        if (collision.CompareTag("Player"))
        {
            isPickedUp = true;

            // Play pickup sound
            AudioManager.instance.PlaySfx("Pickup");

            // Set the corresponding pickup variable
            switch (itemIndex)
            {
                case 0: GameManager.instance.LatigoPickup = true; break;
                case 1: GameManager.instance.MascaraPickup = true; break;
                case 2: GameManager.instance.CampanitasPickup = true; break;
                case 3: GameManager.instance.BolsaPikcup = true; break;
                case 4: GameManager.instance.ChumpiPickup = true; break;
                case 5: GameManager.instance.PututuPickup = true; break;
                case 6: GameManager.instance.ChichaPickup = true; break;
                case 7: GameManager.instance.ConopasPickup = true; break;
                case 8: GameManager.instance.CuchilloPickup = true; break;
                case 9: GameManager.instance.CocaPikcup = true; break;
                case 10: GameManager.instance.MullyPickup = true; break;
            }

            // Disable collider and start scale-down animation
            GetComponent<Collider2D>().enabled = false;
            transform.DOScale(Vector3.zero, 1.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}
