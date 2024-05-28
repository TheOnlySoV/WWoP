using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float detectionRange = 5f;
    public Vector3 spawnRange = new Vector3(1f, 0f, 1f);
    public bool debug = false;

    public LayerMask raycastLayer;

    Ray ray;
    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = new Color(0f, 0f, 1f, 0.4f);
            Gizmos.DrawCube(transform.position, spawnRange);

            ray = new Ray(transform.position, -Vector3.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer))
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
                Gizmos.DrawSphere(hit.point, detectionRange);
            } else
                debug = false;
        }
    }
}

public enum SpawnState { Idle, Full, Spawning }
public enum SpawnDetectionType { Sphere, Square }