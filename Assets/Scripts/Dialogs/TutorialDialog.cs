using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialDialog : AbstractDialog
{
    private TutorialObject tutObj;

    public TMP_Text title;
    public TMP_Text body;
    public Image image;

    public void Populate(TutorialObject tutorialObjRef)
    {
        tutObj = tutorialObjRef;
        title.text = tutorialObjRef.title;
        body.text = tutorialObjRef.body;

        if (tutorialObjRef.image != null)
        {
            image.gameObject.SetActive(true);
            image.sprite = tutorialObjRef.image;
        }
        else
            image.gameObject.SetActive(false);
    }

    public override void CloseDialog()
    {
        
    }

    public override void SelectFirst()
    {
        
    }
}
