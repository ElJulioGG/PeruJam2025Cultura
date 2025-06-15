using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    private NPCController npc;
    public bool showInteract;
    public GameObject Texto;
    private void Update()
    {
        if (GameManager.instance.playerCanAction && npc != null)
        {
            if (Input.GetKeyDown(KeyCode.E) && GameManager.instance.playerCanDialog)
            {
                showInteract = false;
                Debug.Log("Player in dialog");

                GameManager.instance.playerIsInDialog = true;
                GameManager.instance.playerCanMove = false;
                GameManager.instance.playerCanDialog = false;
                npc.ActiveDialog();

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC") && !GameManager.instance.playerIsInDialog)
        {
            npc = collision.GetComponent<NPCController>();
            if (npc != null)
            {
                showInteract = true;
                GameManager.instance.playerCanDialog = true;
                Texto.SetActive(true);

                Debug.Log("Player Can Dialog");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            showInteract = false;
            npc = null;
            GameManager.instance.playerCanDialog = false;
        }
    }
}
