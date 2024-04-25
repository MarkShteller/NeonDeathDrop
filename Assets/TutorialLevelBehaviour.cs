using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialLevelBehaviour : MonoBehaviour
{
    public GameObject xboxTexts;
    public GameObject playstationTexts;
    public GameObject pcTexts;

    void Start()
    {
        GameManager.Instance.PlayerInstance.changedInput += SwitchControls;
        GameManager.Instance.PlayerInstance.playerInput.SwitchCurrentActionMap("UI");
        SwitchControls(GameManager.Instance.PlayerInstance.GetPlayerInput());
    }

    private void OnDisable()
    {
        GameManager.Instance.PlayerInstance.changedInput -= SwitchControls;
    }

    private void SwitchControls(PlayerInput input)
    {
        if (input.devices[0].name.Contains("DualSenseGamepadHID") || input.devices[0].name.Contains("DualShockGamepadHID"))
        {
            pcTexts.SetActive(false);
            xboxTexts.SetActive(false);
            playstationTexts.SetActive(true);
        }
        else if (input.devices[0].name.Contains("Keyboard"))
        {
            pcTexts.SetActive(true);
            xboxTexts.SetActive(false);
            playstationTexts.SetActive(false);
        }
        else
        {
            pcTexts.SetActive(false);
            xboxTexts.SetActive(true);
            playstationTexts.SetActive(false);
        }
    }

}
