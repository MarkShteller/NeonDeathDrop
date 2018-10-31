using System;
using UnityEngine;

public static class ControllerInputDevice
{
    private static bool isLeftTriggerPressed = false;
    private static bool isRightTriggerPressed = false;

    public static bool GetLeftTriggerDown()
    {
        if (Input.GetAxis("LeftTrigger") == 1 && !isLeftTriggerPressed)
        {
            isLeftTriggerPressed = true;
            return true;
        }

        if (Input.GetAxis("LeftTrigger") == 0)
            isLeftTriggerPressed = false;

        return false;
    }

    public static bool GetRightTriggerDown()
    {
        if (Input.GetAxis("RightTrigger") == 1 && !isRightTriggerPressed)
        {
            isRightTriggerPressed = true;
            return true;
        }

        if (Input.GetAxis("RightTrigger") == 0)
            isRightTriggerPressed = false;

        return false;
    }
}
