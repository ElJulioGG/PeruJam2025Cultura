using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupWatcher : MonoBehaviour
{
    [Header("Enemigos a monitorear")]
    [SerializeField] private List<EnemyStats> enemyList = new List<EnemyStats>();

    [Header("Objeto a desactivar cuando todos mueran")]
    [SerializeField] private GameObject objectToDisable;

    [Header("Objeto a activar cuando todos mueran")]
    [SerializeField] private GameObject objectToActivate;

    [SerializeField] public AudioSource muaha;
    [SerializeField] public AudioSource Falling;

    private BoxCollider2D boxCollider;
    private bool hasTriggered = false;
    private bool hasFinished = false;

    public AudioSource principalMusic;
    public AudioSource fightmusic;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        objectToDisable.SetActive(false);
    }

    private void Update()
    {
        if (!hasTriggered || hasFinished) return;

        bool allDead = true;

        foreach (EnemyStats enemy in enemyList)
        {
            if (enemy != null && enemy.isAlive)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            hasFinished = true;

            if (objectToDisable != null)
                objectToDisable.SetActive(false);

            if (objectToActivate != null)
                objectToActivate.SetActive(true);
            Falling.Play();
            principalMusic.Play();
            fightmusic.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (collision.CompareTag("Player"))
        {
            objectToDisable.SetActive(true);
            hasTriggered = true;

            if (boxCollider != null)
                boxCollider.enabled = false;

            Falling?.Play();
            muaha?.Play();
            principalMusic.Stop();
            fightmusic.Play();
        }
    }
}
