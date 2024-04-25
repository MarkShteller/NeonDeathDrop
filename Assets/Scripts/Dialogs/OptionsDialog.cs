using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsDialog : AbstractDialog
{
    public InputActionAsset actions;
    public Toggle fulscreenToggle;
    public Toggle hudToggle;
    public TMP_Dropdown graphicsPresetDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Image confirmIcon;
    public Image closeIcon;


    private Resolution[] resolutions;

    private void OnEnable()
    {
        print("# Populating settings menu");

        if(UIManager.Instance != null)
            hudToggle.isOn = !UIManager.Instance.recordingMode;
        fulscreenToggle.isOn = Screen.fullScreen;

        //actions.Disable();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            print("found saved rebind settings");
            actions.LoadBindingOverridesFromJson(rebinds);
        }

        graphicsPresetDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
        graphicsPresetDropdown.RefreshShownValue();

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        int currentResIndex = 0;
        List<string> resOptions = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        { 
            resOptions.Add(resolutions[i].ToString());
            if (resolutions[i].Equals(Screen.currentResolution))
                currentResIndex = i;
        }
        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.value = currentResIndex;
        
        SelectFirst();
    }

    public void SetGraphicsPreset(int index)
    {
        print("# Changing graphics setting to index "+ index);
        QualitySettings.SetQualityLevel(index);
    }

    public void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen, resolutions[index].refreshRate);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetHUDVisible(bool isHUD)
    {
        if (UIManager.Instance != null)
            UIManager.Instance.SetHUDVisible(isHUD);
    }

    public override void SelectFirst()
    {
        hudToggle.Select();
    }

    public override void CloseDialog()
    {
        print("closing options dialog and updating key bindings");
        //actions.Enable();

        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);

        try
        {
            AnalyticsService.Instance.RecordEvent(new SettingsDialogEvent()
            {
                currentLevelIndex = GameManager.Instance.CurrentLevelIndex,
                inputRebinds = rebinds,
                qualityLevelIndex = QualitySettings.GetQualityLevel()
            });
        }
        catch (Exception e) { }
    }

    private void OnDisable()
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    public override void SetIcons(IconManager.InteractionIcons icons)
    {
        confirmIcon.sprite = icons.buttonSouth;
        closeIcon.sprite = icons.buttonEast;
    }
}
