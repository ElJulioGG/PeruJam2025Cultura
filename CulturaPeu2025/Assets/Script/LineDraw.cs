using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathTrailFollower : MonoBehaviour
{
    public List<Transform> pathPoints;
    public float moveSpeed = 2f;
    public float waitTime = 1f;

    public GameObject objectToActivate;
    public GameObject objectToWatch;
    public ParticleSystem trailParticles;

    private bool isMoving = false;
    private int currentIndex = 0;

    void Update()
    {
        // Si el objeto observado fue destruido, este también se destruye
        if (objectToWatch == null)
        {
            Destroy(gameObject);
            return;
        }

        // Presiona V para iniciar el recorrido
        if (Input.GetKeyDown(KeyCode.V) && !isMoving && pathPoints.Count > 0)
        {
            StopAllCoroutines(); // Por si acaso
            currentIndex = 0;

            if (trailParticles != null)
                trailParticles.Play();

            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        isMoving = true;

        while (currentIndex < pathPoints.Count)
        {
            Transform target = pathPoints[currentIndex];

            while (Vector3.Distance(transform.position, target.position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
            currentIndex++;
        }

        // Al finalizar el recorrido
        if (trailParticles != null)
            trailParticles.Stop();

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        isMoving = false;
    }

    void OnDrawGizmos()
    {
        if (pathPoints == null) return;

        Gizmos.color = Color.yellow;
        foreach (var point in pathPoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.3f);
        }

        Gizmos.color = Color.cyan;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
        }
    }
}
