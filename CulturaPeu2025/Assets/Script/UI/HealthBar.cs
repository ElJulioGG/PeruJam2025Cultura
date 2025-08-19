using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]private EnemyStats enemyStats;
    [SerializeField] private Slider slider;
    void Start()
    {
        slider.maxValue = enemyStats.health;
    }

    // Update is called once per frame
    void Update()
    {
       
        slider.value = enemyStats.health;
        if(gameObject.activeSelf && enemyStats.health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    
}
