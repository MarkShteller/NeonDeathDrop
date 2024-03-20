using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndDemoScreen : AbstractDialog
{
    public Button backBtn;

    public override void CloseDialog()
    {
    }

    public override void SelectFirst()
    {
        backBtn.Select();
    }
    public void OnQuit()
    {
        UIManager.Instance.CloseAllDialogs();
        AudioManager.Instance.StopAllSounds();
        GameManager.Instance.ClearLevel();
        SceneManager.LoadScene(0);
    }
}
