using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseDialog : MonoBehaviour
{
    public Button continueBtn;
    public Button restartBtn;
    public Button optionsBtn;
    public Button quitBtn;
    public TMPro.TMP_Text lvlDetailsText;

    // Start is called before the first frame update
    void Start()
    {
        continueBtn.Select();
    }

    public void Populate()
    { 
        lvlDetailsText.text = "[debug] Level name: "+GameManager.Instance.GetCurrentLevelName();
    }

    public void OnContinue()
    {
        UIManager.Instance.ClosePauseDialog();
    }

    public void OnRestart()
    { 
        UIManager.Instance.ClosePauseDialog();
        GameManager.Instance.RestartLevel(true);
    }

    public void OnOptions()
    { 
        UIManager.Instance.ClosePauseDialog();
    }

    public void OnQuit()
    {
        UIManager.Instance.ClosePauseDialog();
        AudioManager.Instance.StopAllSounds();
        GameManager.Instance.ClearLevel();
        SceneManager.LoadScene(0);
    }
}
