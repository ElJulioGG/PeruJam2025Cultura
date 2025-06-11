using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem
{
    public class DialogHolder : MonoBehaviour
    {
        private bool dialogFinished = true;
        private void OnEnable()
        {
            print("DialogHolderEnables");
            StartCoroutine(dialogSequence());
        }
        private IEnumerator dialogSequence()
        {
            if (!dialogFinished)
            {
                for (int i = 0; i < transform.childCount - 1; i++)
                {
                    Deactivate();
                    transform.GetChild(i).gameObject.SetActive(true);
                    yield return new WaitUntil(() => transform.GetChild(i).GetComponent<DialogLine>().finished);
                }
            }
            else
            {
                int index = transform.childCount - 1;
                Deactivate();
                transform.GetChild(index).gameObject.SetActive(true);
                yield return new WaitUntil(() => transform.GetChild(index).GetComponent<DialogLine>().finished);
            }

            gameObject.SetActive(false);
            dialogFinished = true;
            GameManager.instance.playerIsInDialog = false;
            GameManager.instance.playerCanMove = true;
        }
        private void Deactivate()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            print("lmao");
        }
    }


}