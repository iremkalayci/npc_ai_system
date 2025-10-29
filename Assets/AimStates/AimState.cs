using UnityEngine;

public class AimState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        aim.anim.SetBool("Aiming", true);
        aim.currentFov=aim.adsFov;

    }

    public override void UpdateState(AimStateManager aim)
    {
        // Sağ tık bırakıldıysa HipFire geç
        if (!Input.GetMouseButton(1))
            aim.SwitchState(aim.Hip);
    }
}
