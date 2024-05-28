using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassUtil : MonoBehaviour
{
    public RawImage compass;
    public Transform player;
    public Transform mainCamera;

    public bool rotateWithMainCamera = false;

    void Update()
    {
        if (player)
        {
            if (!rotateWithMainCamera)
                compass.uvRect = new Rect(player.localEulerAngles.y / 360f, 0, 1, 1);
            else
                compass.uvRect = new Rect(mainCamera.localEulerAngles.y / 360f, 0, 1, 1);
        }
    }
}
