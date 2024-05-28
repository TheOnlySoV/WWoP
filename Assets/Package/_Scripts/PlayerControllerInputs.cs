using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerControllerInputs : MonoBehaviour
{
    public ViewState viewState;
    public PlayerState playerInputState = PlayerState.Playing;

    PlayerInput playerInputLink;
    public bool FightEntered { get; set; }

    public void SetPlayerState(int index)
    {
        playerInputState = (PlayerState)index;
        InputCheck();
    }
    public void SetPlayerState(PlayerState index)
    {
        playerInputState = index;
        InputCheck();
    }

    public bool freezeOnPause = false;

    [Header("Character Input Values")]
    #region Input Booleans
    public Vector2 move;
	public Vector2 look;
	public bool jump;
    public bool crouch;
    public bool sprint;
    public bool interact;
    public bool throwBall;
    public bool callFollower;
    public bool viewSwitch;
    public bool start;
    public bool enterFight;
    public float scrollValue;
    #endregion

    [Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

    public bool InputCheck()
    {
        bool returnBoolean = true;

        switch (playerInputState)
        {
            case PlayerState.Playing:
                Cursor.lockState = CursorLockMode.Locked;
                returnBoolean = true;
                break;

            case PlayerState.Paused:
                if (playerInputLink.currentControlScheme == "Gamepad")
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
                returnBoolean = false;
                break;

            case PlayerState.InBattle:
                if (playerInputLink.currentControlScheme == "Gamepad")
                    Cursor.lockState = CursorLockMode.Locked;
                else if (!FightEntered)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
                returnBoolean = true;
                break;

            case PlayerState.Emote:
                if (playerInputLink.currentControlScheme == "Gamepad")
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
                returnBoolean = false;
                break;
        }

        return returnBoolean;
    }

    bool PauseInputCheck()
    {
        bool returnBoolean = true;

        switch (playerInputState)
        {
            case PlayerState.Playing:
                Cursor.lockState = CursorLockMode.Locked;
                returnBoolean = true;
                break;

            case PlayerState.Paused:
                if (playerInputLink.currentControlScheme == "Gamepad")
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
                returnBoolean = true;
                break;

            case PlayerState.Emote:
                if (playerInputLink.currentControlScheme == "Gamepad")
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
                returnBoolean = false;
                break;
        }

        return returnBoolean;
    }

    #region Inputs
#if ENABLE_INPUT_SYSTEM

    private void Awake()
    {
        playerInputLink = GetComponent<PlayerInput>();
    }
    public void OnMove(InputValue value)
    {
        if (!InputCheck())
            return;
        MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
    {
        if (!InputCheck())
            return;
        if (cursorInputForLook)
		{
			LookInput(value.Get<Vector2>());
		}
    }

    public void OnJump(InputValue value)
    {
        if (!InputCheck())
            return;
        JumpInput(value.isPressed);
    }

    public void OnInteract(InputValue value)
    {
        if (!InputCheck())
            return;
        InteractInput(value.isPressed);
    }

    public void OnViewSwitch(InputValue value)
    {
        if (!InputCheck())
            return;
        ViewSwitchInput(value.isPressed);
    }

    public void OnStart(InputValue value)
    {
        if (!PauseInputCheck())
            return;
        StartInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        if (!InputCheck())
            return;
        SprintInput(value.isPressed);
    }

    public void OnCrouch(InputValue value)
    {
        if (!InputCheck())
            return;
        CrouchInput(value.isPressed);
    }

    public void OnThrowBall(InputValue value)
    {
        if (!InputCheck())
            return;
        ThrowBallInput(value.isPressed);
    }

    public void OnCallFollower(InputValue value)
    {
        if (!InputCheck())
            return;
        CallFollowerInput(value.isPressed);
    }

    public void OnEnterFight(InputValue value)
    {
        if (playerInputState != PlayerState.InBattle)
            return;
        EnterFightInput(value.isPressed);
    }

    public void OnBagSlotChange(InputValue value)
    {
        if (!InputCheck())
            return;
        BagSlotChangeInput(value.Get<float>());
    }
#endif
    #endregion

    #region Responses
    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void InteractInput(bool newInteractState)
    {
        interact = newInteractState;
    }

    public void ViewSwitchInput(bool newViewSwitchState)
    {
        viewSwitch = newViewSwitchState;
    }

    public void StartInput(bool newQuickMenuState)
    {
        start = newQuickMenuState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    public void CrouchInput(bool newCrouchState)
    {
        crouch = newCrouchState;
    }

    public void ThrowBallInput(bool newThrowState)
    {
        throwBall = newThrowState;
    }

    public void CallFollowerInput(bool newCallState)
    {
        callFollower = newCallState;
    }

    public void EnterFightInput(bool newFightState)
    {
        enterFight = newFightState;
    }

    public void BagSlotChangeInput(float newBagSlotChangeState)
    {
        scrollValue = newBagSlotChangeState;
    }
    #endregion

    private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

public enum ViewState { FirstPersonView, ThirdPersonView }