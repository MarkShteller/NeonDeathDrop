using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Point playerPointPosition;
    public int score;

	// Use this for initialization
	void Awake ()
    {
        Instance = this;
        score = 0;
	}

    internal void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int points)
    {
        score += points;
        UIManager.Instance.SetScore(score);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        UIManager.Instance.ShowGameOverScreen(score);
    }

}
