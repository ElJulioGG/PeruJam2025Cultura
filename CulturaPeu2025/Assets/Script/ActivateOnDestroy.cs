using UnityEngine;

public class ActivateOnDestroy : MonoBehaviour
{
    public GameObject objectToWatch;    
    public GameObject objectToActivate;  

    void Update()
    {
        if (objectToWatch == null && objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Destroy(this); 
        }
    }
}
