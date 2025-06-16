using UnityEngine;
using System.Collections.Generic;

public class TreasurePathCurved : MonoBehaviour
{
    public Transform player;
    public List<Transform> waypoints;
    public float moveSpeed = 5f;
    public float reachThreshold = 1f;
    public Material trailMaterial;

    private int currentIndex = 0;
    private Transform trailFollower;

    void Start()
    {
        trailFollower = new GameObject("TrailFollower").transform;
        trailFollower.position = player.position;

        var trail = trailFollower.gameObject.AddComponent<TrailRenderer>();

        trail.material = trailMaterial;

        trail.startWidth = 0.25f;
        trail.endWidth = 0.25f;
        trail.minVertexDistance = 0.05f;
        trail.numCornerVertices = 10;
        trail.numCapVertices = 10;
    }

    void Update()
    {
        if (currentIndex >= waypoints.Count)
            return;

        Vector3 currentTarget = waypoints[currentIndex].position;
        Vector3 followTarget = Vector3.Lerp(player.position, currentTarget, 0.5f);

        trailFollower.position = Vector3.Lerp(trailFollower.position, followTarget, Time.deltaTime * moveSpeed);

        if (Vector3.Distance(player.position, currentTarget) < reachThreshold)
        {
            Destroy(waypoints[currentIndex].gameObject);
            currentIndex++;
        }
    }
}
