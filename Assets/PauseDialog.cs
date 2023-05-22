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

    // Start is called before the first frame update
    void Start()
    {
        continueBtn.Select();
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
        SceneManager.LoadScene(0);
    }
}
