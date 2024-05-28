using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.UI;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
*/

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
[RequireComponent(typeof(PlayerControllerInputs))]
#endif
public class PlayerController : MonoBehaviour
{
    //NetworkManager networkManager;
    public Transform throwingHand;

    [Range(1f, 50f)] public float throwingStrength = 10f;
    [Range(0, 1f)] public float throwingArcMod = .2f;
    bool callingFollower = false;

    public float throwballCooldown = 2f;
    bool throwingBall = false;
    public bool inBattle;

    public float standingHeight = 1.8f;
    public float standingCenter = 0.93f;
    public float crouchHeight = 0.9f;
    public float crouchCenter = 0.465f;

    public float interactRange = 5f;
    public LayerMask interactMask;

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("Rotation speed of the character")]
    public float RotationSpeed = 1.0f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    public GameObject FPSCamera;
    public GameObject TPSCamera;

    // player
    private float _speed;
    [HideInInspector]
    public float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    [HideInInspector]
    public bool sprintToggled = false;
    private bool crouchToggled = false;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDThrowBall;
    private int _animIDMotionSpeed;
    private int _animIDCrouching;
    private int _animIDEmote;
    private int _animIDEmoteState;

#if ENABLE_INPUT_SYSTEM 
    private PlayerInput _playerInput;
#endif
    private Animator _animator;
    private CharacterController _controller;
    private PlayerControllerInputs _input;
    private GameObject _mainCamera;

    private InventoryManager im;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;

    public bool changeLock = false;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
			return false;
#endif
        }
    }

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        im = InventoryManager.instance;
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerControllerInputs>();
        im._input = _input;
        im._controller = this;

        UIManager.instance.UpdateCompassAndMiniMap(transform);
        _input.FightEntered = false;
#if ENABLE_INPUT_SYSTEM 
        _playerInput = GetComponent<PlayerInput>();
#else
		Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;

        //networkManager = NetworkManager.Instance;

        //gameObject.name = MainMenuManager.Instance.username;
        //GetComponent<NetworkedTransform>().username = gameObject.name;
        //GetComponent<NetworkedTransform>().objectID = MainMenuManager.Instance.ID;

    }

    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        JumpAndGravity();
        GroundedCheck();
        Move();

        ViewSwitchCheck();
        MenuCheck();
        SprintCheck();
        ThrowCheck();
        InteractCheck();

        #region KBM Specific

        #endregion

        #region Controller Specific

        #endregion
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    public void ThrowBall()
    {
        if (!callingFollower)
        {
            GameObject newThrownBall = Instantiate(im.bag.pokeballs[im.bag.ballIndex].ball.throwable, throwingHand.position, transform.rotation);

            Rigidbody rb = newThrownBall.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce((_mainCamera.transform.forward * throwingStrength) + (Vector3.up * (throwingStrength * throwingArcMod)) + GetComponent<CharacterController>().velocity, ForceMode.Impulse);
            rb.AddTorque((_mainCamera.transform.right * -throwingStrength) + _mainCamera.transform.forward, ForceMode.Impulse);

            Destroy(newThrownBall, 30f);
        } else
        {
            GameObject newThrownBall = Instantiate(im.party[0].ball, throwingHand.position, transform.rotation);

            newThrownBall.GetComponent<ThrownPokeball>().callingFollower = true;

            Rigidbody rb = newThrownBall.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce((_mainCamera.transform.forward * throwingStrength) + (Vector3.up * (throwingStrength * throwingArcMod)) + GetComponent<CharacterController>().velocity, ForceMode.Impulse);
            rb.AddTorque((_mainCamera.transform.right * -throwingStrength) + _mainCamera.transform.forward, ForceMode.Impulse);
        }

        changeLock = false;
    }

    IEnumerator CallFollower()
    {
        //Set the pokemon that is being called
        throwingBall = false;
        callingFollower = true;
        yield return new WaitForSecondsRealtime(throwballCooldown);

        throwingBall = false;
    }

    public void FollowerCalled()
    {
        callingFollower = false;
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDCrouching = Animator.StringToHash("Crouch");
        _animIDThrowBall = Animator.StringToHash("ThrowBall");
        _animIDEmote = Animator.StringToHash("Emote");
        _animIDEmoteState = Animator.StringToHash("EmoteState");
    }

    private void ViewSwitchCheck()
    {
        if (_input.viewSwitch)
        {
            //print("View Switch Input Accepted");

            switch(_input.viewState)
            {
                case ViewState.FirstPersonView:
                    TPSCamera.SetActive(true);
                    FPSCamera.SetActive(false);
                    break;

                case ViewState.ThirdPersonView:
                    FPSCamera.SetActive(true);
                    TPSCamera.SetActive(false);
                    break;
            }

            _input.viewSwitch = false;
        }
    }

    Ray ray;
    RaycastHit hit;
    private void InteractCheck()
    {
        ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        if (!active)
            active = true;
        if (_input.interact && Physics.Raycast(ray, out hit, interactRange, interactMask))
        {
            if (hit.transform.GetComponent<Interactable>())
                hit.transform.GetComponent<Interactable>().Interact();
        }
        _input.interact = false;
    }

    private void MenuCheck()
    {
        if (_input.start)
        {
            //print("Menu Input Accepted");

            //deactivate input
            switch(_input.playerInputState)
            {
                case PlayerState.Playing:
                    _input.SetPlayerState((int)PlayerState.Paused);
                    GameManager.instance.pauseMenu.SetActive(true);
                    break;
                case PlayerState.Paused:
                    _input.SetPlayerState((int)PlayerState.Playing);
                    GameManager.instance.pauseMenu.SetActive(false);
                    UIManager.instance.CloseAllPauseWindows();
                    break;

                default: break;
            }

            _input.start = false;
        }
    }

    public void TryEmote(int index)
    {
        _input.playerInputState = PlayerState.Emote;
        _animator.SetBool(_animIDEmote, true);
        _animator.SetInteger(_animIDEmoteState, index);
    }

    private void SprintCheck()
    {
        if (!Grounded)
            return;
        if (_input.move.y < 0 && _input.viewState == ViewState.FirstPersonView)
        {
            sprintToggled = false;
            return;
        }
        if (_input.sprint)
        {
            if (crouchToggled)
            {
                crouchToggled = false;
                //ChangeCrouch();
                _input.sprint = false;
                return;
            }
            //print("Sprint Input Accepted");
            if (_input.move.y >= 0 && _input.viewState == ViewState.FirstPersonView)
                sprintToggled = !sprintToggled;
            else
                sprintToggled = !sprintToggled;

            _input.sprint = false;
        }
    }

    private void CrouchCheck()
    {
        if (_input.crouch)
        {
            //print("Crouch Input Accepted");
            crouchToggled = !crouchToggled;
            //sprintToggled = false;
            //ChangeCrouch();

            _input.crouch = false;
        }
    }

    private void ThrowCheck()
    {
        _animator.SetBool(_animIDThrowBall, false);
        if (_input.throwBall)
        {
            if (!throwingBall)
            {
                changeLock = true;
                StartCoroutine(ThrowBallCooldown());
                _animator.SetBool(_animIDThrowBall, true);
            }
            _input.throwBall = false;
        } else if (_input.callFollower)
        {
            if (!throwingBall)
            {
                StartCoroutine(CallFollower());
                _animator.SetBool(_animIDThrowBall, true);
            }
            _input.callFollower = false;
        }
        else
        {
            _input.callFollower = false;
            _input.throwBall = false;
        }
    }

    IEnumerator ThrowBallCooldown()
    {
        throwingBall = true;
        yield return new WaitForSecondsRealtime(throwballCooldown);
        throwingBall = false;
    }

    public void ChangeCrouch()
    {
        SetAnimatorController(_animIDCrouching, crouchToggled);

        switch (crouchToggled)
        {
            case false:
                _controller.center = new Vector3(0f, standingCenter, 0f);
                _controller.height = standingHeight;
                break;

            case true:
                _controller.center = new Vector3(0f, crouchCenter, 0f);
                _controller.height = crouchHeight;
                break;
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        SetAnimatorController(_animIDGrounded, Grounded);
    }

    public void SetAnimatorController(int animID, bool state)
    {
        if (_hasAnimator)
        {
            _animator.SetBool(animID, state);
        }
    }

    private void CameraRotation()
    {
        if (!_input.InputCheck())
        {
            _input.look = new Vector2(0f, 0f);
            return;
        }
        switch (_input.viewState)
        {
            case ViewState.FirstPersonView:
                // if there is an input
                if (_input.look.sqrMagnitude >= _threshold)
                {
                    //Don't multiply mouse input by Time.deltaTime
                    float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                    _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                    _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                    // clamp our pitch rotation
                    _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                    // Update Cinemachine camera target pitch
                    CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                    // rotate the player left and right
                    transform.Rotate(Vector3.up * _rotationVelocity);
                }

                break;
            case ViewState.ThirdPersonView:
                // if there is an input and camera position is not fixed
                if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
                {
                    //Don't multiply mouse input by Time.deltaTime;
                    float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                    _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                    _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
                }

                // clamp our rotations so our values are limited 360 degrees
                _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Cinemachine will follow this target
                CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                    _cinemachineTargetYaw, 0.0f);

                break;
        }
    }

    private void Move()
    {
        if (!_input.InputCheck())
            _input.move = new Vector2(0f, 0f);

        if (_input.playerInputState != PlayerState.Emote)
        {
            _animator.SetBool("Emote", false);
            _animator.SetInteger("EmoteState", 0);
        }
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = sprintToggled ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

            if (_input.viewState == ViewState.ThirdPersonView)
            {
                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    private void JumpAndGravity()
    {//TODO If the player is crouched, uncrouch the player and do nothing else
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                    crouchToggled = false;
                    //ChangeCrouch();
                }
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
    bool active = false;
    private void OnDrawGizmos()
    {
        if (active)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(ray);
        }
    }
}

public enum PlayerState { Playing, Paused, InBattle, Emote }