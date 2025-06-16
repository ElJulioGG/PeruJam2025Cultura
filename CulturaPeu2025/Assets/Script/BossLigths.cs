using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossLigths : MonoBehaviour
{
    // Array of light GameObjects to control
    [SerializeField] public EnemyStats boss;

    [SerializeField] public Light2D[] lights; // Assuming you are using Unity's 2D Light system
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public Light2D mainLight;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(boss.health < boss.baseHealth* 0.75)
        {
            mainLight.color = Color.green;
            lights[0].color = Color.green;
            lights[0].intensity = 0.2f;
            // Set the alpha to 0.2 for a dim light effect

        }
        if (boss.health < boss.baseHealth * 0.5)
        {
            mainLight.color = Color.yellow;
            lights[1].color = Color.green;
            // Set the alpha to 0.2 for a dim light effect
            lights[1].intensity = 0.2f; // Set the alpha to 0.2 for a dim light effect
        }
        if (boss.health < boss.baseHealth * 0.25)
        {
            mainLight.color = Color.red;
            lights[2].color = Color.green;
            lights[2].intensity = 0.2f;
        }
        if (boss.health <= 0)
        {

            lights[3].color = Color.green;
            lights[3].intensity = 0.2f;

        }
        print("Boss Health: " + boss.health);

        print("Boss 75%: " + boss.baseHealth * 0.75);
        print("Boss 50%: " + boss.baseHealth * 0.5);
        print("Boss 25%: " + boss.baseHealth * 0.25);
        print("Boss 0%: " + boss.baseHealth * 0.0);
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
