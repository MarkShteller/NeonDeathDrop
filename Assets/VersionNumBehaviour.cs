using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionNumBehaviour : MonoBehaviour
{
    TMP_Text versionTxt;

    void Start()
    {
        versionTxt = GetComponent<TMP_Text>();
        versionTxt.text = "Demo v" + Application.version;
    }

}
