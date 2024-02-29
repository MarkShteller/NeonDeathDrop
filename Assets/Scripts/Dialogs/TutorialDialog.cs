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
    public Image imageBg;

    public void Populate(TutorialObject tutorialObjRef)
    {
        tutObj = tutorialObjRef;
        title.text = tutorialObjRef.title;
        body.text = tutorialObjRef.body;

        if (tutorialObjRef.image != null)
        {
            image.gameObject.SetActive(true);
            image.sprite = tutorialObjRef.image;
            imageBg.gameObject.SetActive(true);
        }
        else
        {
            image.gameObject.SetActive(false);
            imageBg.gameObject.SetActive(false);

        }
    }

    public override void CloseDialog()
    {
        
    }

    public override void SelectFirst()
    {
        
    }
}
