using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public Point playerPointPosition;
    public int score = 0;
    public float scoreMultiplier = 1;

    private GameObject PlayerObject;

    public PlayerBehaviour PlayerInstance;
    public CameraMovement cameraRef;

    public LevelManager levelManager;

    public AnimationCurve[] slomoCurves;

    public int CurrentLevelIndex = 0;

    private LevelScriptableObject currentLevelData;

    private float maxScoreMultiplier;
    private float timeLevelStarted;
    private float damageTaken;

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

        Application.targetFrameRate = 60;

        StartCoroutine(InitLevel());
        print("## GameManager ready");
	}

    private IEnumerator InitLevel()
    {
        //wait frame for the scene to load.
        yield return null;

        print("Init level number "+CurrentLevelIndex);
        PlayerObject = Resources.Load("Player 2.0") as GameObject;
        GameObject go = Instantiate(PlayerObject);
        PlayerInstance = go.GetComponent<PlayerBehaviour>();
        playerPointPosition = null;

        score = 0;
        scoreMultiplier = 1;
        maxScoreMultiplier = scoreMultiplier;
        timeLevelStarted = Time.time;
        damageTaken = 0;

        UIManager.Instance.SetScore(score);
        UIManager.Instance.SetScoreMultiplier(scoreMultiplier);

        cameraRef = Camera.main.transform.parent.GetComponent<CameraMovement>();
        print(cameraRef.currentState);
        currentLevelData = levelManager.Init(CurrentLevelIndex);
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
        StartCoroutine(InitLevel());
    }

    public void NextLevel()
    {
        CurrentLevelIndex++;
        cameraRef.SetLowHealth(false);
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
        UIManager.Instance.OpenGameOverScreen(score);
    }

    public void AddScoreMultiplier(float value)
    {
        scoreMultiplier += value;
        if (scoreMultiplier > maxScoreMultiplier)
            maxScoreMultiplier = scoreMultiplier;
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

    public void AddDamageCount(float damage)
    {
        damageTaken += damage;
    }

    public void LevelFinished()
    {
        Time.timeScale = 0;
        float levelTime = Time.time - timeLevelStarted;

        if (currentLevelData.showEndLevelReport)
            UIManager.Instance.OpenEndLevelDialog(score, maxScoreMultiplier, levelTime, PlayerInstance.enemyDefeatedCount, damageTaken, currentLevelData);
        else
            NextLevel();
    }

    public void DashSlomo(float duration)
    {
        StartCoroutine(SlomoCoroutine(duration, slomoCurves[0]));
    }

    public void ShockwaveSlomo(float duration)
    {
        StartCoroutine(SlomoCoroutine(duration, slomoCurves[1]));
    }

    private IEnumerator SlomoCoroutine(float duration, AnimationCurve curve)
    {
        float time = 0;
        float timeStarted = Time.time;
        while (time <= duration)
        {
            Time.timeScale = curve.Evaluate(time / duration);

            time += Time.time - timeStarted;
            yield return null;
        }
    }

}
