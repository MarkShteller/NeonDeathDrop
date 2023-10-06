using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Tutorial Object 0", menuName = "Tutorial Object")]
public class TutorialObject : ScriptableObject
{
    public string title;
    [TextArea(10, 100)]
    public string body;
    public string voiceOverReferance;
    public Sprite image;
}
