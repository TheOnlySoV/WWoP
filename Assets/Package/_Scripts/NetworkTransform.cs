using UnityEngine;
using UnityEngine.UI;

public class NetworkedTransform : MonoBehaviour
{
    public ObjectType objType = ObjectType.SceneObject;
    public string username;
    public bool active;
    public int tID;
    [SerializeField]
    private int serverID;
    public bool client;

    //[HideInInspector]
    public int modelSelection;

    [Space(10)]
    public Transform namePlateParent;
    public float fadeMax = 10f;
    public float fadeMin = 5f;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDCrouch;
    private int _animIDThrowBall;
    private int _animIDMotionSpeed;
    private int _animIDEmote;
    private int _animIDEmoteState;

    //cached components
    PlayerController controller;
    PlayerControllerInputs inputs;
    Animator animator;
    Camera cam;
    Transform player;
    CanvasGroup namePlateCanvasGroup;

    bool awaitingID = false;
    bool requestSent = false;

    //Object Type State Managed
    private void Awake()
    {
        switch(objType)
        {
            case ObjectType.ClientPlayer:
                awaitingID = false;
                requestSent = false;
                controller = GetComponent<PlayerController>();
                inputs = GetComponent<PlayerControllerInputs>();
                break;
            case ObjectType.ServerPlayer:
                cam = Camera.main;
                player = transform;
                //namePlateCanvasGroup = namePlateParent.GetComponent<CanvasGroup>();
                break;
            case ObjectType.SceneObject:

                break;
        }

        animator = GetComponent<Animator>();

        AssignAnimationIDs();
    }

    private void Start()
    {
        switch (objType)
        {
            case ObjectType.ClientPlayer:

                break;
            case ObjectType.ServerPlayer:
                //AssignNamePlate();
                break;
            case ObjectType.SceneObject:

                break;
        }
        if (serverID > 0)
            active = true;
    }

    private void Update()
    {
        if (serverID > 0)
        {
            active = true;
            awaitingID = false;
        }
        if (active == false) { return; }
        //if (!client)
            //print("Made it here - 97");
        TrainerData data;

        switch(objType)
        {
            case ObjectType.ClientPlayer:
                if (controller)
                {
                    if (!awaitingID)
                    {
                        if (!requestSent)
                        {
                            requestSent = true;
                            awaitingID = true;
                            return;
                        }
                        data = new TrainerData(username, serverID, tID, transform.position, transform.eulerAngles, transform.localScale,
                                               animator.GetFloat(_animIDSpeed), animator.GetBool(_animIDJump), animator.GetBool(_animIDGrounded), 
                                               animator.GetBool(_animIDFreeFall), animator.GetBool(_animIDCrouch), animator.GetBool(_animIDThrowBall), 
                                               animator.GetBool(_animIDEmote), animator.GetInteger(_animIDEmoteState), modelSelection);
                        NetworkManager.instance.UpdateClientObjectData(data);
                    }
                }
                break;
            case ObjectType.ServerPlayer:
                data = NetworkManager.instance.GetServerObjectData(serverID, client);
                if (data != null)
                {
                    //print("Made it here - 122");

                    transform.position = data.position;
                    transform.eulerAngles = data.rotation;
                    transform.localScale = data.scale;

                    SetPlayerAnimations(data);
                }
                //NamePlateFade();
                break;
            case ObjectType.SceneObject:

                break;
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDCrouch = Animator.StringToHash("Crouch");
        _animIDThrowBall = Animator.StringToHash("ThrowBall");
        _animIDEmote = Animator.StringToHash("Emote");
        _animIDEmoteState = Animator.StringToHash("EmoteState");

        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animator.SetFloat(_animIDMotionSpeed, 1);
    }

    private void SetPlayerAnimations(TrainerData data)
    {
        animator.SetFloat(_animIDSpeed, data.speed);
        animator.SetBool(_animIDJump, data.jump);
        animator.SetBool(_animIDGrounded, data.grounded);
        animator.SetBool(_animIDFreeFall, data.freefall);
        animator.SetBool(_animIDCrouch, data.crouch);
        animator.SetBool(_animIDThrowBall, data.throwball);
        animator.SetBool(_animIDEmote, data.emote);
        animator.SetInteger(_animIDEmoteState, data.emoteState);
    }

    public void AssignNamePlate()
    {
        namePlateParent.GetComponent<Canvas>().worldCamera = cam;
        namePlateParent.GetChild(0).GetComponent<Text>().text = username;
    }

    public void NamePlateFade()
    {
        if (player == null)
            return;
        float dist = Vector3.Distance(transform.position, player.position);
        if (cam)
        {
            if (dist <= fadeMin) //player is within max alpha
            {
                namePlateCanvasGroup.alpha = 1;
            } else if (dist >= fadeMin && dist <= fadeMax) //manage alpha
            {
                float multi = fadeMax / fadeMin;
                float temp = dist / fadeMax;
                namePlateCanvasGroup.alpha = (1 - temp) * multi;
            } else //nameplate disappear
            {
                namePlateCanvasGroup.alpha = 0;
            }
            namePlateParent.LookAt(cam.transform);
        }
    }

    public int ServerID
    {
        get { return serverID; }
        set { serverID = value; }
    }

    public void SetServerID(int newID)
    {
        serverID = newID;
    }
}

public enum ObjectType { ClientPlayer, ServerPlayer, SceneObject }