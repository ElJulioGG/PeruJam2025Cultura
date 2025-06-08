using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FadeTransition : MonoBehaviour
{
    [Header("Referencia a la imagen UI")]
    public Image uiImage;

    [Header("Duración del fade")]
    public float fadeDuration = 1f;

    [Header("GameObject a desactivar")]
    public GameObject image;

    void Start()
    {
        FadeOut();

    }

    public void FadeOut()
    {
        uiImage.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            image.SetActive(false);
        });
    }

    public void FadeIn()
    {
        image.SetActive(true);
        uiImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            image.SetActive(false); 
        });
    }
 
}
