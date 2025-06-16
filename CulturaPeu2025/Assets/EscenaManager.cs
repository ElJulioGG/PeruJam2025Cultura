using UnityEngine;

public class EscenaManager : MonoBehaviour
{
    [SerializeField] public GameObject boss;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boss.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void playStartMusic()
    {
        AudioManager.instance.PlayMusic("BossMusic");
    }
    public void playTemblor()
    {
        AudioManager.instance.PlayUI("Floor");
    }
    public void activateBoss()
    {
        boss.SetActive(true);
    }
}
