using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private GameObject dialog;
    public void ActiveDialog()
    {
        dialog.SetActive(true);
        print("Dialog Activated");
    }
    public bool DialogActive()
    {
        return dialog.activeInHierarchy;
    }
}
