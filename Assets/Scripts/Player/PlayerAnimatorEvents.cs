﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEvents : MonoBehaviour
{
    public PlayerBehaviour playerBehaviour;

    public void PreformPush()
    {
        playerBehaviour.PreformPush(1, 0.1f, 0.2f);
        playerBehaviour.PlayCorrespondingPushSound();
    }

    public void PreformPush2()
    {
        playerBehaviour.PreformPush(1.3f, 0.2f, 0.3f);
        playerBehaviour.PlayCorrespondingPushSound();
    }

    public void PreformSomersault()
    {
        playerBehaviour.PreformPush(3, 0.2f, 0.4f, 4);
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
        print("aaa");
        playerBehaviour.PreformRepelAttack();
    }

    public void MakeHole()
    {
        playerBehaviour.MakeHole();
    }

    public void Dead()
    {
        GameManager.Instance.GameOver();
    }
}
