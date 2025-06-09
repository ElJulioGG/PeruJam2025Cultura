using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageFadeOut : MonoBehaviour
{
    [Header("Images to Fade")]
    public Image image1;
    public Image image2;

    [Header("Fade Settings")]
    public float fadeDuration = 0.3f;

    void Start()
    {
    }

    public void FadeOutImages()
    {
        if (image1 != null)
        {
            image1.DOFade(0f, fadeDuration).SetEase(Ease.OutExpo);
        }

        if (image2 != null)
        {
            image2.DOFade(0f, fadeDuration).SetEase(Ease.OutExpo);
        }
    }
}
