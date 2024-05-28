using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapUtil : MonoBehaviour
{
    public Camera mmCam;
    public Transform player;

    public bool rotateWithPlayer = false;

    public float height;
    public Perspectives perspective = Perspectives.Orthographic;

    private void Update()
    {
        if (player)
        {
            UpdateCamera();
        }
    }

    void UpdateCamera()
    {
        mmCam.transform.position = new Vector3 (player.position.x, player.position.y + height, player.position.z);

        if (rotateWithPlayer)
            mmCam.transform.rotation = Quaternion.Euler(new Vector3(0, player.transform.rotation.y, 0));
    }
}

public enum Perspectives { Orthographic, Perspective }