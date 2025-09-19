using UnityEngine;
using System.Collections;

public class ActivatorTrigger : MonoBehaviour
{
    [Header("Activation Settings")]
    public bool activateOnTrigger = true;
    public float activationDelay = 0f;
    public bool destroyAfterActivation = true;

    [Header("Properties to Activate")]
    public bool setCanAction = true;
    public bool setCanMove = true;
    public bool setWeapon = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (activateOnTrigger)
            {
                StartCoroutine(ActivatePropertiesWithDelay());
            }
        }
    }

    private IEnumerator ActivatePropertiesWithDelay()
    {
        if (activationDelay > 0)
        {
            yield return new WaitForSeconds(activationDelay);
        }

        // Activar las propiedades en el GameManager
        if (GameManager.instance != null)
        {
            if (setCanAction)
            {
                GameManager.instance.playerCanAction = true;
                Debug.Log("playerCanAction activado: true");
            }

            if (setCanMove)
            {
                GameManager.instance.playerCanMove = true;
                Debug.Log("playerCanMove activado: true");
            }

            if (setWeapon)
            {
                GameManager.instance.playerHasWeapon = true;
                Debug.Log("playerHasWeapon activado: true");
            }
        }

        // Opcional: activar también en el PlayerStats si es necesario
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null && setWeapon)
        {
            if (playerStats.weapon != null)
            {
                playerStats.weapon.SetActive(true);
                Debug.Log("Arma visual activada");
            }
        }

        // Destruir el objeto después de activar si está configurado
        if (destroyAfterActivation)
        {
            Destroy(gameObject);
        }
        else
        {
            // Desactivar el collider para que no se active múltiples veces
            GetComponent<Collider2D>().enabled = false;
        }
    }

    // Método público para activar manualmente desde otros scripts
    public void ManualActivate()
    {
        StartCoroutine(ActivatePropertiesWithDelay());
    }

    // Método para activar inmediatamente sin corrutina
    public void ImmediateActivate()
    {
        if (GameManager.instance != null)
        {
            if (setCanAction) GameManager.instance.playerCanAction = true;
            if (setCanMove) GameManager.instance.playerCanMove = true;
            if (setWeapon) GameManager.instance.playerHasWeapon = true;
        }

        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null && setWeapon && playerStats.weapon != null)
        {
            playerStats.weapon.SetActive(true);
        }

        if (destroyAfterActivation)
        {
            Destroy(gameObject);
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }

    // Para debug: visualizar el área del trigger
    private void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Verde transparente

            if (collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
            }
        }
    }
}