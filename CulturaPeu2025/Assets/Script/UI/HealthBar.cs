using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField]private EnemyStats enemyStats;
    [SerializeField] private Slider slider;
    public GameObject hielo;
    void Start()
    {
        slider.maxValue = enemyStats.health;
    }

    void Update()
    {
       
        slider.value = enemyStats.health;
        if(gameObject.activeSelf && enemyStats.health <= 0)
        {
            gameObject.SetActive(false);
            hielo.SetActive(true);
        }


    }

    
}
