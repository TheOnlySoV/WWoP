using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CameraPlayerCollision : MonoBehaviour
{
    public PlayerControllerInputs _input;

    public Transform FPSMeshParent;
    public Transform TPSMeshParent;

    public GameObject FPS_HUD;
    public GameObject TPS_HUD;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trainer"))
        {
            //for (int i = 0; i <= TPSMeshParent.childCount - 1; i++)
            //{
            //    TPSMeshParent.GetChild(i).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            //}
            //FPSMeshParent.gameObject.SetActive(true);
            //FPS_HUD.SetActive(true);
            //TPS_HUD.SetActive(false);

            //_input.viewState = ViewState.FirstPersonView;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trainer"))
        {
            //for (int i = 0; i <= TPSMeshParent.childCount - 1; i++)
            //{
            //    TPSMeshParent.GetChild(i).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            //}
            //FPSMeshParent.gameObject.SetActive(false);
            //TPS_HUD.SetActive(true);
            //FPS_HUD.SetActive(false);

            //_input.viewState = ViewState.ThirdPersonView;
        }
    }
}
