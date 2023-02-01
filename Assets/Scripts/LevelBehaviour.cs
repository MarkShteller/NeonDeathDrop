using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour
{
    public GameObject CMCameras;

    public void SwitchCMCamera(int camIndex)
    {
        for (int i = 0; i < CMCameras.transform.childCount; i++)
        {
            CMCameras.transform.GetChild(i).gameObject.SetActive(false);
        }

        if(camIndex != -1)
            CMCameras.transform.GetChild(camIndex).gameObject.SetActive(true);
    }

    public void DisableALLCMCameras()
    {
        for (int i = 0; i < CMCameras.transform.childCount; i++)
        {
            CMCameras.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
