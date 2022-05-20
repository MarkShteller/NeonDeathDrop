using System;
using UnityEngine;

public static class ControllerInputDevice
{
    private static bool isLBPressed = false;
    private static bool isRBPressed = false;
    private static bool isDashPressed = false;
    private static bool isSpecialPressed = false;
    private static bool isConfirmPressed = false;
    private static bool isChargePressed = false;
    private static bool isHeavyPressed = false;
    private static bool isCompanionPressed = false;
    private static bool isLTPressed = false;

    public static bool GetLeftButtonDown()
    {
        if (Input.GetAxis("LeftButton") == 1)// && !isLeftTriggerPressed)
        {
            isLBPressed = true;
            return true;
        }

        if (Input.GetAxis("LeftButton") == 0)
            isLBPressed = false;

        return false;
    }

    public static bool GetLeftButtonUp()
    {
        if (Input.GetKeyUp(KeyCode.Joystick1Button4))
            return true;
        /*if (isLBPressed && Input.GetAxis("LeftButton") == 0)
        {
            isLBPressed = false;
            return true;
        }*/
        return false;
    }

    public static bool GetRightButtonDown()
    {
        if (Input.GetAxis("RightButton") == 1 && !isRBPressed)
        {
            isRBPressed = true;
            return true;
        }

        if (Input.GetAxis("RightButton") == 0)
            isRBPressed = false;

        return false;
    }

    public static bool GetConfirmButtonDown()
    {
        if (Input.GetButtonDown("Submit") && !isConfirmPressed)
        {
            //Debug.Log("ConfirmButtonDown");
            isConfirmPressed = true;
            return true;
        }

        if (Input.GetButtonDown("Submit"))
            isConfirmPressed = false;

        return false;
    }

    public static bool GetDashButtonDown()
    {
        if (Input.GetAxis("Dash") == 1 && !isDashPressed)
        {
            isDashPressed = true;
            return true;
        }

        if (Input.GetAxis("Dash") == 0)
            isDashPressed = false;

        return false;
    }

    public static bool GetCompanionButtonDown()
    {
        if (Input.GetAxis("Companion") == 1 && !isCompanionPressed)
        {
            isCompanionPressed = true;
            return true;
        }

        if (Input.GetAxis("Companion") == 0)
            isCompanionPressed = false;

        return false;
    }

    public static bool GetSpecialButtonDown()
    {
        if (Input.GetAxis("Special") == 1 && !isSpecialPressed)
        {
            isSpecialPressed = true;
            return true;
        }

        if (Input.GetAxis("Special") == 0)
            isSpecialPressed = false;

        return false;
    }

    public static bool GetChargeButtonDown()
    {
        if (Input.GetAxis("Charge") == 1)
        {
            isChargePressed = true;
            return true;
        }

        if (Input.GetAxis("Charge") == 0)
            isChargePressed = false;

        return false;
    }

    public static bool GetHeavyButtonDown()
    {
        if (Input.GetAxis("Heavy") == 1 && !isHeavyPressed)
        {
            isHeavyPressed = true;
            return true;
        }

        if (Input.GetAxis("Heavy") == 0)
            isHeavyPressed = false;

        return false;
    }

    public static bool GetLeftTriggerDown()
    {
        if (Input.GetAxis("LeftTrigger") == 1 && !isLTPressed)
        {
            isLTPressed = true;
            return true;
        }

        if (Input.GetAxis("LeftTrigger") == 0)
            isLTPressed = false;

        return false;
    }
}
