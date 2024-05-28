using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PokemonMotor : NPCMotor
{
    public Transform battleCameraRoot;
    public Pokemon pokemon;
    public Wild wild;
    [Header("Battle Settings")]
    public float maxTrainerBattleDistance;
    public bool inBattle;
    public float rotationSpeed;
    [Header("Movement Info")]
    int randomMovementWaitTime = 1;//rolled
    [Range(1, 10)] public int minMovementWaitTime = 1;
    [Range(10, 30)] public int maxMovementWaitTime = 30;
    [Range(0f, 60f)] public float movementTimer = 1f;//count this number down

    [Header("Bored Info")]
    int randomBoredWaitTime = 1;//rolled
    [Range(1, 60)] public int minBoredWaitTime = 1;
    [Range(60, 120)] public int maxBoredWaitTime = 60;
    [Range(0f, 120f)] public float boredTimer = 1f;//count this number down

    [Header("Emote Info")]
    [Range(0, 10)] public int emotes = 1;
    [Range(0, 10)] public int boredEmotes = 1;
    int chosenEmote = 0;

    Vector3 targetLocation;
    Transform targetPokemonLocation;

    [HideInInspector]
    public bool inBall = false;

    int _animIDInBattle;
    int _animIDEmote;
    int _animIDEmoteState;
    int _animIDBored;
    int _animIDBoredState;

    public PokemonSpawner Spawner { private get ; set; }
    //[HideInInspector]
    public int mySpawnerIndex = -1;

    public List<ColliderSettings> colliderSettings;

    Transform encounteredTrainer;

    [Range(1f, 300f)]
    public float despawnTimer = 30f;

    private void OnValidate()
    {
        if (colliderSettings.Count > 0)
            foreach (ColliderSettings c in colliderSettings)
                c.colliderName = c.agentType.ToString();
    }

    private void Awake()
    {
        SetBoredWaitTimer();

        anim = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();

        AssignBoxColliderSettings(2);
    }

    void AssignBoxColliderSettings(int index)
    {
        GetComponent<CapsuleCollider>().center = colliderSettings[index].center;
        GetComponent<CapsuleCollider>().height = colliderSettings[index].size.y;
        // GetComponent<CapsuleCollider>().isTrigger = colliderSettings[index].isTrigger;
    }

    private void Start()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDInBattle = Animator.StringToHash("In Battle");
        _animIDEmote = Animator.StringToHash("Emote");
        _animIDEmoteState = Animator.StringToHash("Emote State");
        _animIDBored = Animator.StringToHash("Bored");
        _animIDBoredState = Animator.StringToHash("Bored State");

        ToNewLocation();
    }

    private void Update()
    {
        anim.SetFloat(_animIDSpeed, nma.velocity.magnitude);

        CheckBored();

        if (inBattle && encounteredTrainer)
        {
            FaceTarget();
            float distanceToTrainer = Vector3.Distance(encounteredTrainer.position, transform.position);
            if (distanceToTrainer > maxTrainerBattleDistance)
            {
                ToNewLocation();
                encounteredTrainer.GetComponent<PlayerControllerInputs>().playerInputState = PlayerState.Playing;
                encounteredTrainer.GetComponent<PlayerControllerInputs>().FightEntered = false;
                inBattle = false;
                anim.SetBool(_animIDInBattle, false);
                encounteredTrainer = null;
                GetComponent<Collider>().isTrigger = true;
                UIManager.instance.EndEncounter();
                EncounterManager.instance.DestroyCam();
            }
            return;
        }
        CheckMovementTimer();

        if (!inBattle && !inBall)
            despawnTimer -= Time.deltaTime;

        if (despawnTimer < 0)
        {
            Spawner.RestorePoint(mySpawnerIndex);
            Destroy(gameObject);
        }
    }

    public void StopAgent()
    {
        nma.isStopped = true;
        inBall = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trainer") && other.GetComponent<PlayerControllerInputs>().playerInputState != PlayerState.InBattle)
        {
            boredTimer = 60f; //reset the bored timer :)
            inBattle = true;
            anim.SetBool(_animIDInBattle, true);
            other.GetComponent<PlayerControllerInputs>().playerInputState = PlayerState.InBattle;
            UIManager.instance.StartEncounter();
            EncounterManager.instance.BeginEncounter(this);

            nma.isStopped = true;

            encounteredTrainer = other.transform;
            GetComponent<Collider>().isTrigger = false;// Should this be done? - Probably. Helpful for Pokemon encounters
        }
    }

    void FaceTarget()
    {
        if (targetPokemonLocation)
        {
            Vector3 pokemonDirection = (targetPokemonLocation.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(pokemonDirection.x, 0, pokemonDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        Vector3 trainerDirection = (encounteredTrainer.position - transform.position).normalized;
        Quaternion lookRotationInversed = Quaternion.LookRotation(new Vector3(-trainerDirection.x, 0, -trainerDirection.z));
        battleCameraRoot.rotation = Quaternion.Slerp(battleCameraRoot.rotation, lookRotationInversed, Time.deltaTime * (rotationSpeed / 5f));
    }

    #region Movement and Animations
    private void ToNewLocation()
    {
        nma.isStopped = false;

        targetLocation = Spawner.NewMovementLocation();
        nma.SetDestination(targetLocation);

        SetNextMovementTimer();
    }

    void CheckMovementTimer()
    {
        if (inBall)
            return;
        if (movementTimer <= 0f)
        {
            ToNewLocation();

            return;
        }

        movementTimer -= Time.deltaTime;
    }

    void CheckBored()
    {
        if (boredTimer <= 0f)
        {
            nma.isStopped = true;
            SetBoredState();
            SetBoredWaitTimer();

            return;
        }
        boredTimer -= Time.deltaTime;
        ResetBoredState();
    }

    void SetNextMovementTimer()
    {
        randomMovementWaitTime = Random.Range(minMovementWaitTime, maxMovementWaitTime);
        movementTimer = randomMovementWaitTime;
    }

    void SetBoredWaitTimer()
    {
        randomBoredWaitTime = Random.Range(minBoredWaitTime, maxBoredWaitTime + 1);
        boredTimer = randomBoredWaitTime;
    }

    void SetBoredState()
    {
        chosenEmote = Random.Range(0, boredEmotes + 1);
        anim.SetBool(_animIDBored, true);
        anim.SetInteger(_animIDBoredState, chosenEmote);
    }

    void ResetBoredState()
    {
        anim.SetBool(_animIDBored, false);
        anim.SetInteger(_animIDBoredState, 0);
    }

    void SetEmoteState(int chosenState)
    {
        anim.SetBool(_animIDEmote, true);
        anim.SetInteger(_animIDEmoteState, chosenState);
    }

    void ResetEmoteState()
    {
        anim.SetBool(_animIDEmote, false);
        anim.SetInteger(_animIDEmoteState, 0);
    }
    #endregion

    public void BreakOut()
    {
        nma.isStopped = false;
        inBall = false;
    }

    public void Caught()
    {
        Spawner.RestorePoint(mySpawnerIndex);
        Destroy(gameObject);
    }
}

[System.Serializable]
public class ColliderSettings
{
    [HideInInspector]
    public string colliderName;
    public AgentSize agentType;
    public bool isTrigger;

    public Vector3 center;
    public Vector3 size;
}

public enum AgentSize { Human, ExtraSmall, Small, Medium, Large, ExtraLarge }