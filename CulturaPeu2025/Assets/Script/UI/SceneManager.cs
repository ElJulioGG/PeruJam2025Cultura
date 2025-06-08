using UnityEngine;
using UnityEngine.SceneManagement;

public class SScenControler : MonoBehaviour
{
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame() funciona o wa ");
        Application.Quit();
    }
}