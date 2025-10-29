using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState : ActionBaseState
{
    public override void EnterState(ActionStateManager actions)
    {
        // IK weight'leri animasyon ile uyumlu olacak şekilde 1 bırak
        actions.RHandAim.weight = 0;
        actions.LHandIK.weight = 0;

        // Reload trigger
        actions.anim.SetTrigger("Reload");

        // Opsiyonel: Reload animasyonu süresince bekleyip DefaultState'e dön
        actions.anim.GetComponent<MonoBehaviour>().StartCoroutine(ReturnToDefault(actions));
    }

    IEnumerator ReturnToDefault(ActionStateManager actions)
    {
        // Animasyon uzunluğunu al
        AnimatorClipInfo[] clips = actions.anim.GetCurrentAnimatorClipInfo(0);
        float clipLength = 1f; // default

        if(clips.Length > 0) clipLength = clips[0].clip.length;

        yield return new WaitForSeconds(clipLength);

        // Reload bittiğinde DefaultState'e dön
        actions.SwitchState(actions.Default);
    }

    public override void UpdateState(ActionStateManager actions)
    {
        // Gerekirse buraya reload esnasında input ekleyebilirsin
    }
}
