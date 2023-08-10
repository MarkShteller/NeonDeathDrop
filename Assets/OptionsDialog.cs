using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsDialog : AbstractDialog
{
    public Toggle fulscreenToggle;
    public TMP_Dropdown graphicsPresetDropdown;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private void OnEnable()
    {
        print("# Populating settings menu");
        
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

    public override void SelectFirst()
    {
        fulscreenToggle.Select();
    }
}