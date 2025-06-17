using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CircularProgress : MonoBehaviour
{
    public Slider slider;
    public float fillSpeed = 1f;
    public FadeTransition fadeTransition;
    public AudioSource audioSource;

    [Header("Escena a cargar manualmente")]
    public int sceneIndex;
    private bool transitioning = false;

    private void Update()
    {
        if (transitioning) return;

        if (Input.GetKey(KeyCode.Q))
        {
            if (!audioSource.isPlaying)
                audioSource.Play();

            slider.value += fillSpeed * Time.deltaTime;
            slider.value = Mathf.Min(slider.value, 1f);

            audioSource.volume = slider.value;

            if (slider.value >= 1f)
            {
                transitioning = true;
                fadeTransition.FadeIn();
                SceneManager.LoadScene(sceneIndex);
            }
        }
        else
        {
            slider.value -= fillSpeed * Time.deltaTime;
            slider.value = Mathf.Max(slider.value, 0f);

            audioSource.volume = slider.value;

            if (slider.value <= 0f && audioSource.isPlaying)
                audioSource.Stop();
        }


    }
}
