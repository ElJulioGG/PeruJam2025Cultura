using UnityEngine;
using System.Collections;

public class AuquiAttack : MonoBehaviour
{
    public AuquiEnemy enemy;
    public float disableDuration = 2f;
    public string playerHitAnimation = "Hit";
    public Collider2D collider2D;


    public Movement playerMovement; // arrástralo desde el Inspector
    private float originalPlayerSpeed=7f;

    public ParticleSystem PS1;
    public ParticleSystem PS2;

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Player") && enemy != null)
        {
            PS1.Play();
            PS2.Play();
            bool isAttack2 = enemy.GetComponent<Animator>().GetInteger("attackType") == 2;

            if (isAttack2)
            {
                GameManager.instance.playerHasWeapon = false;
                GameManager.instance.playerCanMove=false;

                StartCoroutine(DisablePlayerMovement());

            }
        }
    }

    private IEnumerator DisablePlayerMovement()
    {
        originalPlayerSpeed = playerMovement.maxSpeed;
        playerMovement.maxSpeed = 0f;
        GameManager.instance.playerCanMove = false;
        yield return new WaitForSeconds(disableDuration);
        PS1.Stop();
        PS2.Stop();
        GameManager.instance.playerCanMove = true;
        GameManager.instance.playerHasWeapon = true;
        playerMovement.maxSpeed = originalPlayerSpeed;
        

    }
}
