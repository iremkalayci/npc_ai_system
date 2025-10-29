using UnityEngine;

public class HipFireState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        aim.anim.SetBool("Aiming", false);
        aim.currentFov = aim.hipFov;
    }

    public override void UpdateState(AimStateManager aim)
    {
        // Eski Input System ile sağ tık
        if (Input.GetMouseButton(1)) // 1 = sağ tık
            aim.SwitchState(aim.Aim);
    }
}
