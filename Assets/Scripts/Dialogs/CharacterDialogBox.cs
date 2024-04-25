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
    public Image portraitImageShadow;
    public Image autoToggle;
    public Image nextBtnIcon;

    public void Populate(string speaker, string content, string image, IconManager.InteractionIcons icons)
    {
        speakerLable.text = speaker;
        contentLable.text = content;
        Sprite img = Resources.Load<Sprite>("Portraits/" + image);
        portraitImage.sprite = img;
        portraitImageShadow.sprite = img;
        nextBtnIcon.sprite = icons.buttonSouth;
    }

    public void ToggleAuto(bool b)
    {
        autoToggle.gameObject.SetActive(b);
    }
}
