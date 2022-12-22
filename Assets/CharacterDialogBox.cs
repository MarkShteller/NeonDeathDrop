using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogBox : MonoBehaviour
{
    public TMP_Text speakerLable;
    public TMP_Text contentLable;
    public Image portraitImage;

    public void Populate(string speaker, string content, string image)
    {
        speakerLable.text = speaker;
        contentLable.text = content;
        //apply image too
    }
}
