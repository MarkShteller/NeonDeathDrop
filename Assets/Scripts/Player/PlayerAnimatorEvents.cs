using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEvents : MonoBehaviour
{
    public PlayerBehaviour playerBehaviour;

    public void PreformPush()
    {
        playerBehaviour.PreformPush(1, 0.1f, 0.2f, ForcePushFloorTrigger.PulseType.Light);
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerPush, transform.position);
    }

    public void PreformPush2()
    {
        playerBehaviour.PreformPush(1.5f, 0.2f, 0.4f, ForcePushFloorTrigger.PulseType.Light);
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerPushCombo, transform.position);
    }

    public void PreformPull()
    {
        playerBehaviour.PreformPush(1.5f, 0.2f, 0.4f, ForcePushFloorTrigger.PulseType.Light);
        playerBehaviour.PreformPullAttack();
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerPush, transform.position);
    }

    public void PreformSomersault()
    {
        playerBehaviour.PreformPush(2f, 0.2f, 0.4f, ForcePushFloorTrigger.PulseType.Heavy, 4);
        playerBehaviour.PreformSomersault();
    }

    public void FinishSomersault()
    {
        playerBehaviour.FinishSomersault();
    }


    public void PreformShockwave()
    {
        playerBehaviour.PreformShockwave();
    }

    public void PreformRepelAttack()
    {
        print("aaa1");
        playerBehaviour.PreformRepelAttack();
    }

    public void FinishRepelAttack()
    {
        playerBehaviour.FinishRepelAttack();
    }

    public void MakeHole()
    {
        playerBehaviour.MakeHole();
    }

    public void Launch()
    {
        playerBehaviour.PreformLaunch();
    }

    public void FinishFinisher()
    {
        playerBehaviour.FinishFinisher();
    }

    public void Dead()
    {
        //GameManager.Instance.GameOver();
    }

    public void DeathDrop()
    {
        //GameManager.Instance.GameOver();

        //shockwave
        playerBehaviour.PreformShockwaveTrailer();

        //paticles 
    }

}
