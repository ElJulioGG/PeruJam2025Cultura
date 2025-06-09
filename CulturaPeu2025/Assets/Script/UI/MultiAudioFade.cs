using UnityEngine;
using DG.Tweening;

public class MultiAudioFade : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource audio1; // baja rápido
    public AudioSource audio2; // baja lento
    public AudioSource audio3; // baja lento

    [Header("Fade Settings")]
    public float fastFadeDuration = 0.3f;     // duración del fade para audio1
    public float slowFadeDuration = 1.0f;     // duración del fade para audio2 y audio3

    [Header("GameObject to Activate")]
    public GameObject objectToActivate;

    public void FadeOutAndActivate()
    {
        if (audio1 != null)
        {
            DOTween.To(() => audio1.volume, x => audio1.volume = x, 0f, fastFadeDuration).SetEase(Ease.Linear);
        }

        if (audio2 != null)
        {
            DOTween.To(() => audio2.volume, x => audio2.volume = x, 0f, slowFadeDuration).SetEase(Ease.Linear);
        }

        if (audio3 != null)
        {
            DOTween.To(() => audio3.volume, x => audio3.volume = x, 0f, slowFadeDuration).SetEase(Ease.Linear);
        }

        // Activar el objeto después del fade más largo
        float waitTime = Mathf.Max(fastFadeDuration, slowFadeDuration);
        DOVirtual.DelayedCall(waitTime, () =>
        {
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }
        });
    }
}
