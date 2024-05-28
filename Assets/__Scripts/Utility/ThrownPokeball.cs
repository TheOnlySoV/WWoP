using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrownPokeball : MonoBehaviour
{
    public Pokeball ball;

    public LayerMask captureLayer;
    Rigidbody rb;

    Transform target;
    Wild wildTarget;

    public int spawnIndex = 0;
    public bool callingFollower = false;

    //animator
    public Animator anim;
    public Animator ballAnim;
    int open;
    int shakeNumber;

    bool catching = false;
    float myA;
    int currentShake = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        anim.enabled = false;

        InitAnimator();
    }

    void InitAnimator()
    {
        open = Animator.StringToHash("Open");
        shakeNumber = Animator.StringToHash("Shake Number");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (catching)
            return;
        if (!callingFollower && collision.collider.CompareTag("Pokemon"))
        {
            PokemonMotor pm = collision.gameObject.GetComponent<PokemonMotor>();
            if (!pm.inBall)
            {
                target = collision.transform;
                wildTarget = collision.gameObject.GetComponent<PokemonMotor>().wild;
                collision.gameObject.GetComponent<PokemonMotor>().StopAgent();
                wildTarget.ball = ball.throwable;

                StartCoroutine("StartCatch");
            }
        }

        if (callingFollower && collision.collider.CompareTag("CallableSurface"))
        {
            GameManager.instance.SpawnFollower(0, collision.GetContact(0).point);
            rb.useGravity = false;
            rb.isKinematic = true;

            Destroy(gameObject, 1f);
        }
    }

    IEnumerator StartCatch()
    {
        catching = true;

        yield return new WaitForSecondsRealtime(.1f);

        GetComponent<Collider>().isTrigger = true;

        rb.useGravity = false;
        rb.isKinematic = true;

        if (target)
        {
            yield return new WaitForSecondsRealtime(.5f);

            ballAnim.enabled = true;
            ballAnim.SetBool(open, true);
        }
    }

    private void Update()
    {
        if (target)
            transform.LookAt(target);
    }

    public void InitialCheck()
    {
        Stats stats = wildTarget.stats;
        float statusModifier = ball.EvaluateStatusCondition(wildTarget);
        myA = (Mathf.Floor((((3 * stats.HP) - (2 * wildTarget.currentHealth)) * 4096 * wildTarget.pokemon.info.catchRate * ball.modifier) / (3 * stats.HP))) * statusModifier;
        bool cc = ball.CalculateCritical(myA);

        if (!cc)
        {
            bool firstPass = ball.PerformShakeCheck(myA);
            if (firstPass)
                anim.SetInteger(shakeNumber, ++currentShake);
            else
                CatchFailure();
        } else
            anim.SetInteger(shakeNumber, 4);
    }

    public void SecondCheck()
    {
        bool secondPass = ball.PerformShakeCheck(myA);
        if (secondPass)
            anim.SetInteger(shakeNumber, ++currentShake);
        else
            CatchFailure();
    }

    public void ThirdCheck()
    {
        bool thirdPass = ball.PerformShakeCheck(myA);
        if (thirdPass)
            anim.SetInteger(shakeNumber, ++currentShake);
        else
            CatchFailure();
    }

    public void FinalCheck()
    {
        bool finalPass = ball.PerformShakeCheck(myA);
        if (finalPass)
            anim.SetInteger(shakeNumber, ++currentShake);
        else
            CatchFailure();
    }

    void CatchFailure()
    {
        anim.SetInteger(shakeNumber, -1);
        ballAnim.SetInteger(ballAnim.GetComponent<PokeballAnimatorController>().shakeNumber, -1);

        target.GetComponent<PokemonMotor>().BreakOut();
    }

    public void CatchSuccess()
    {
        anim.SetInteger(shakeNumber, 4);

        InventoryManager.instance.CatchPokemon(InventoryManager.instance.ConvertToOwned(wildTarget));

        //Destroy(target.gameObject);
        target.GetComponent<PokemonMotor>().Caught();
    }

    public void DestroyBall()
    {
        Destroy(gameObject);
    }
}
