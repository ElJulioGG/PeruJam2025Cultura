using UnityEngine;
using DG.Tweening;
using System.Collections;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private int itemIndex;
    [SerializeField] private float freezeTime = 0.8f; // Tiempo total del efecto
    [SerializeField] private float slowMotionFactor = 0.1f; // Velocidad durante slow motion
    [SerializeField] private float slowMotionDelay = 1.5f; // Tiempo antes de activar slow motion

    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return;
        if (collision.CompareTag("Player"))
        {
            isPickedUp = true;

            // Iniciar el slow motion después del delay
            StartCoroutine(StartSlowMotionAfterDelay());

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

    private IEnumerator StartSlowMotionAfterDelay()
    {
        // Esperar el delay antes de activar el slow motion
        yield return new WaitForSeconds(slowMotionDelay);

        // Iniciar el efecto de slow motion
        StartCoroutine(FreezeEffect());
    }

    private IEnumerator FreezeEffect()
    {
        // Slow motion inicial
        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Ajustar fixed delta time

        // Congelar completamente por un breve momento
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;

        // Mantener freeze
        yield return new WaitForSecondsRealtime(0.2f);

        // Slow motion de regreso
        Time.timeScale = slowMotionFactor;
        yield return new WaitForSecondsRealtime(0.1f);

        // Restaurar tiempo normal gradualmente
        float timer = 0f;
        float duration = 0.3f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(slowMotionFactor, 1f, timer / duration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        // Asegurar valores finales
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}