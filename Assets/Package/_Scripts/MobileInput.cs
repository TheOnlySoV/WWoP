using UnityEngine;

public class MobileInput : MonoBehaviour
{
    [Header("Output")]
    public PlayerControllerInputs playerControllerInputs;

    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        playerControllerInputs.MoveInput(virtualMoveDirection);
    }

    public void VirtualLookInput(Vector2 virtualLookDirection)
    {
        playerControllerInputs.LookInput(virtualLookDirection);
    }

    public void VirtualJumpInput(bool virtualJumpState)
    {
        playerControllerInputs.JumpInput(virtualJumpState);
    }

    public void VirtualSprintInput(bool virtualSprintState)
    {
        playerControllerInputs.SprintInput(virtualSprintState);
    }
        
}
