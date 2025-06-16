using UnityEngine;

public class PressOnce : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            Destroy(gameObject);
        }
    }
}
