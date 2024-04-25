using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseDialog : AbstractDialog
{
    public Button continueBtn;
    public Button restartBtn;
    public Button optionsBtn;
    public Button quitBtn;
    public TMPro.TMP_Text lvlDetailsText;

    // Start is called before the first frame update
    void Start()
    {
        SelectFirst();
    }

    public void Populate()
    { 
        lvlDetailsText.text = "[debug] Level name: "+GameManager.Instance.GetCurrentLevelName();
    }

    public void OnContinue()
    {
        UIManager.Instance.CloseAllDialogs();
    }

    public void OnRestart()
    { 
        UIManager.Instance.CloseAllDialogs();
        GameManager.Instance.RestartLevel(true);
    }

    public void OnOptions()
    {
        //UIManager.Instance.ClosePauseDialog();
        UIManager.Instance.OpenOptionsDialog();
    }

    public void OnQuit()
    {
        UIManager.Instance.CloseAllDialogs();
        AudioManager.Instance.StopAllSounds();
        GameManager.Instance.ClearLevel();
        SceneManager.LoadScene(0);
    }

    public override void SelectFirst()
    {
        continueBtn.Select();
    }

    public override void CloseDialog()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetIcons(IconManager.InteractionIcons icons)
    {
    }
}
