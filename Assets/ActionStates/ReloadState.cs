using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState : ActionBaseState
{
    public override void EnterState(ActionStateManager actions)
    {
       
        actions.RHandAim.weight = 0;
        actions.LHandIK.weight = 0;

        
        actions.anim.SetTrigger("Reload");

        
        actions.anim.GetComponent<MonoBehaviour>().StartCoroutine(ReturnToDefault(actions));
    }

    IEnumerator ReturnToDefault(ActionStateManager actions)
    {
        
        AnimatorClipInfo[] clips = actions.anim.GetCurrentAnimatorClipInfo(0);
        float clipLength = 1f; // default

        if(clips.Length > 0) clipLength = clips[0].clip.length;

        yield return new WaitForSeconds(clipLength);

        
        actions.SwitchState(actions.Default);
    }

    public override void UpdateState(ActionStateManager actions)
    {
        
    }
}
