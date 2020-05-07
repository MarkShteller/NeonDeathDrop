using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEvents : MonoBehaviour
{
    public PlayerBehaviour playerBehaviour;

    public void PreformPush()
    {
        playerBehaviour.PreformPush();
    }

    public void PreformShockwave()
    {
        playerBehaviour.PreformShockwave();
    }

    public void MakeHole()
    {
        playerBehaviour.MakeHole();
    }
}
