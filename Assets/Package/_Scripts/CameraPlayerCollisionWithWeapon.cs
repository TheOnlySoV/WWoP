using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CameraPlayerCollisionWithWeapon : MonoBehaviour
{
    public PlayerControllerInputs _input;

    //Enable these if the HUD changes when switching between first and third person views
    //public GameObject FPS_HUD;
    //public GameObject TPS_HUD;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetChild(1).GetChild(0).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            other.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            //FPS_HUD.SetActive(true);
            //TPS_HUD.SetActive(false);

            _input.viewState = ViewState.FirstPersonView;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetChild(1).GetChild(0).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            other.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

            //TPS_HUD.SetActive(true);
            //FPS_HUD.SetActive(false);

            _input.viewState = ViewState.ThirdPersonView;
        }
    }
}
