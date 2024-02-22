using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverDialog : AbstractDialog {

    public Text totalScore;
    public Button restartButton;

	public void Init(int score)
    {
        totalScore.text = "TOTAL SCORE:\n" + score.ToString("N0");
        SelectFirst();
	}

    public void RestartButton()
    {
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.UIRestart, transform.position);
        GameManager.Instance.RestartLevel(true);
    }

    public void QuitButton()
    { 
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.UIRestart, transform.position);
        SceneManager.LoadScene(0);
    }

    public override void SelectFirst()
    {
        restartButton.Select();
    }

    public override void CloseDialog()
    {
    }

}
