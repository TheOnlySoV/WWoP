using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PokeballAnimatorController : MonoBehaviour
{
    public Animator anim;
    public ThrownPokeball thrownBall;

    int open;
    [HideInInspector]
    public int shakeNumber;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        InitAnimator();
    }

    void InitAnimator()
    {
        open = Animator.StringToHash("Open");
        shakeNumber = Animator.StringToHash("ShakeNumber");
    }

    public void ClosePokeball()
    {
        anim.SetBool(open, false);

        thrownBall.anim.enabled = true;
        thrownBall.InitialCheck();
    }

    public void CatchFailed()
    {
        anim.SetBool(open, true);
    }
}
