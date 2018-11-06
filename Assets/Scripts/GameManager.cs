using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Point playerPointPosition;
    public int score;
    public GameObject PlayerObject;

	// Use this for initialization
	void Awake ()
    {
        Instance = this;
        score = 0;
        print("game manager ready");
	}

    public void SetPlayerPosition(Vector3 position)
    {
        PlayerObject.transform.position = position;
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
