using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    [Header("Canvas Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float canvasFadeDuration = 2f;
    [SerializeField] private Ease canvasFadeEase = Ease.InOutQuad;
    [SerializeField] private string gameOverSfx = "DeathUI";

    [Header("Try Again Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashFadeDuration = 0.5f;
    [SerializeField] private Ease flashFadeEase = Ease.InOutQuad;
    [SerializeField] private string tryAgainSfx = "Lugh";

    [Header("Final Screen Fade")]
    [SerializeField] private Image screenFadeImage; // full-screen black image
    [SerializeField] private Color screenFadeColor = Color.black;
    [SerializeField] private float screenFadeDuration = 2f;
    [SerializeField] private Ease screenFadeEase = Ease.Linear;

    [Header("Eye Effect (Only for Try Again)")]
    [SerializeField] private bool enableEyesOnTryAgain = true;
    [SerializeField] private GameObject eye1;
    [SerializeField] private GameObject eye2;

    [Header("Exit Behavior")]
    [SerializeField] private bool quitOnExit = false;
    [SerializeField] private string exitSfx = "";

    private void OnEnable()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, canvasFadeDuration).SetEase(canvasFadeEase);
        }

        AudioManager.instance.PlaySfx(gameOverSfx);
    }

    public void OnTryAgainPressed()
    {
        
        if (fadeImage != null)
        {
            fadeImage.color = flashColor;
            fadeImage.DOFade(0f, flashFadeDuration).SetEase(flashFadeEase);
        }

        
        AudioManager.instance.PlaySfx(tryAgainSfx);
        AudioManager.instance.PlaySfx("Impact");

        if (enableEyesOnTryAgain)
        {
            if (eye1 != null) eye1.SetActive(true);
            if (eye2 != null) eye2.SetActive(true);
        }

        FadeToBlack(() =>
        {
            // TODO: Replace with your actual scene reload
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void OnExitPressed()
    {
        if (fadeImage != null)
        {
            fadeImage.color = flashColor;
            fadeImage.DOFade(0f, flashFadeDuration).SetEase(flashFadeEase);
        }

        
        AudioManager.instance.PlaySfx(tryAgainSfx);

        FadeToBlack(() =>
        {

        });
    }

    private void FadeToBlack(System.Action onComplete)
    {
        if (screenFadeImage != null)
        {
            screenFadeImage.color = new Color(screenFadeColor.r, screenFadeColor.g, screenFadeColor.b, 0);
            screenFadeImage.gameObject.SetActive(true);
            screenFadeImage.DOFade(1f, screenFadeDuration)
                .SetEase(screenFadeEase)
                .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            onComplete?.Invoke();
        }
    }
}
