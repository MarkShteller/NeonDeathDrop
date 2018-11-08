using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public Point playerPointPosition;
    public int score = 0;
    public GameObject PlayerObject;

    public PlayerBehaviour PlayerInstance;

    public LevelManager levelManager;
    public int CurrentLevelIndex = 0;

	// Use this for initialization
	void Awake ()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        InitLevel();
        print("## GameManager ready");
	}

    private void InitLevel()
    {
        print("Init level number "+CurrentLevelIndex);
        PlayerObject = Resources.Load("Player 2.0") as GameObject;
        GameObject go = Instantiate(PlayerObject);
        PlayerInstance = go.GetComponent<PlayerBehaviour>();
        levelManager.Init(CurrentLevelIndex);
    }

    public void SetPlayerPosition(Vector3 position)
    {
        if (PlayerInstance == null)
        {
            GameObject go = Instantiate(PlayerObject);
            PlayerInstance = go.GetComponent<PlayerBehaviour>();
        }
        PlayerInstance.transform.position = position;
    }

    public void RestartLevel(bool isDead)
    {
        Time.timeScale = 1;
        if (isDead)
        {
            score = 0;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        InitLevel();
    }

    public void NextLevel()
    {
        CurrentLevelIndex++;
        RestartLevel(false);
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
