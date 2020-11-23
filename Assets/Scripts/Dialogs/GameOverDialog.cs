using UnityEngine;
using UnityEngine.UI;

public class GameOverDialog : MonoBehaviour {

    public Text totalScore;

	public void Init(int score)
    {
        totalScore.text = "TOTAL SCORE:\n" + score.ToString("N0");
	}

    public void RestartButton()
    {
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.UIRestart, transform.position);
        GameManager.Instance.RestartLevel(true);
    }


    private void Update()
    {
        if (ControllerInputDevice.GetConfirmButtonDown())
        {
            RestartButton();
        }
    }
}
