using System.Collections;
using UnityEngine;

public class escenasCambio : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReloadScene()
    {
        StartCoroutine(LoadSceneWithDelay(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));
        AudioManager.instance.musicSource.Stop();

    }

    public void LoadSceneMenu()
    {
        StartCoroutine(LoadSceneWithDelay("Menu"));

        AudioManager.instance.musicSource.Stop();

    }
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(4f);
        AudioManager.instance.sfxSource.Stop();
        AudioManager.instance.UISource.Stop();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

    }
}
