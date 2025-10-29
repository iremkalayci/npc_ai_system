using UnityEngine;

public class RunState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Running", true);
        movement.currentMoveSpeed = movement.runSpeed;
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (!Input.GetKey(KeyCode.LeftShift))
            ExitState(movement, movement.Walk);
        if (movement.dir.magnitude < 0.1f)
            ExitState(movement, movement.Idle);
            if (movement.vInput < 0) movement.currentMoveSpeed = movement.runBackSpeed;
        else movement.currentMoveSpeed = movement.runSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Running", false);
        movement.currentMoveSpeed = 3f;
        movement.SwitchState(state);
    }
}
