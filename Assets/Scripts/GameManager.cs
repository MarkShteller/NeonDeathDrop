using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public Point playerPointPosition;
    public int score = 0;
    public float scoreMultiplier = 1;

    private GameObject PlayerObject;

    public PlayerBehaviour PlayerInstance;

    public LevelManager levelManager;
    public int CurrentLevelIndex = 0;

	// Use this for initialization
	void Awake ()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

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
        playerPointPosition = null;

        score = 0;
        scoreMultiplier = 1;
        UIManager.Instance.SetScore(score);
        UIManager.Instance.SetScoreMultiplier(scoreMultiplier);

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

    public void SetPlayerSpawnPosition(Vector3 position)
    {
        if (PlayerInstance == null)
        {
            GameObject go = Instantiate(PlayerObject);
            PlayerInstance = go.GetComponent<PlayerBehaviour>();
        }
        PlayerInstance.spawnPosition = position;
    }

    public void RestartLevel(bool isDead)
    {
        print("## restarting level");
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
        score += (int) Math.Round(points * scoreMultiplier);
        UIManager.Instance.SetScore(score);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        UIManager.Instance.ShowGameOverScreen(score);
    }

    public void AddScoreMultiplier(float value)
    {
        scoreMultiplier += value;
        UIManager.Instance.SetScoreMultiplier(scoreMultiplier);
    }

    public void SetScoreMultiplier(int value)
    {
        scoreMultiplier = value;
        UIManager.Instance.SetScoreMultiplier(scoreMultiplier);
    }

    public void IncrementEnemyKillCount()
    {
        PlayerInstance.enemyDefeatedCount++;
        Debug.Log("# enemy kill count: "+ PlayerInstance.enemyDefeatedCount);
    }
}
