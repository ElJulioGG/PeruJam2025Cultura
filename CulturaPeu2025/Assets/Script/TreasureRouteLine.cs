using UnityEngine;
using System.Collections;

public class TreasureRouteLine : MonoBehaviour
{
    public Transform startPoint; 
    public Transform endPoint;   
    public float interval = 0.5f; 

    public LineRenderer line;

    void Start()
    {
        StartCoroutine(UpdateLineRoutine());
    }

    IEnumerator UpdateLineRoutine()
    {
        while (true)
        {
            // Mostrar la l�nea
            line.positionCount = 2;
            line.SetPosition(0, startPoint.position);
            line.SetPosition(1, endPoint.position);

            yield return new WaitForSeconds(interval);

            // Ocultar la l�nea (resetea)
            line.positionCount = 0;

            yield return new WaitForSeconds(interval);
        }
    }
}
