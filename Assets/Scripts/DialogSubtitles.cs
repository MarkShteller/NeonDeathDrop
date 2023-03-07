using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogSubtitles : MonoBehaviour
{
    public TMP_Text speakerLable;
    public TMP_Text contentLable;

    public void Populate(string speaker, string content)
    {
        speakerLable.text = speaker;
        contentLable.text = content;
    }
}
