using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public Point playerPointPosition;
    public int score = 0;
    public float scoreMultiplier = 1;

    public float killPoints = 0;
    private float maxKillPoints =1;
    public float[] multiplierTiers;
    public string[] praiseTiers;
    private int currentMultiplierTierIndex = 0;

    private GameObject PlayerObject;

    public PlayerBehaviour PlayerInstance;
    public CameraMovement cameraRef;

    public LevelManager levelManager;

    public AnimationCurve[] slomoCurves;
    public Volume slomoVolume;
    public int CurrentLevelIndex = 0;

    private LevelScriptableObject currentLevelData;

    private float maxScoreMultiplier;
    private float timeLevelStarted;
    private float damageTaken;

    private Coroutine lerpSlomoCoroutine;
    private Coroutine scoreUpdaterCoroutine;

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
        killPoints = 0;
        timeLevelStarted = Time.time;
        damageTaken = 0;
        scoreUpdaterCoroutine = StartCoroutine(KillPointsUpdater());

        UIManager.Instance.SetScore(score);
        UIManager.Instance.SetScoreMultiplier(scoreMultiplier);

        cameraRef = Camera.main.transform.parent.GetComponent<CameraMovement>();
        print(cameraRef.currentState);
        currentLevelData = levelManager.Init(CurrentLevelIndex);
    }

    private IEnumerator KillPointsUpdater()
    {
        while (true)
        {
            if(killPoints > 0)
                killPoints -= Time.fixedDeltaTime * maxKillPoints / 10;
            if (killPoints < 0)
                killPoints = 0;
            if (killPoints == 0 && scoreMultiplier != 1)
            {
                SetScoreMultiplier(1);
                maxKillPoints = 1;
            }
            UIManager.Instance.SetKillPoints(killPoints, multiplierTiers[currentMultiplierTierIndex+1]);
            yield return new WaitForFixedUpdate();
        }
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

    public void AddKillPoints(float value)
    {
        killPoints += value;
        if (killPoints > maxKillPoints)
            maxKillPoints = killPoints;
        for (int i = multiplierTiers.Length - 1; i > 0; i--)
        {
            if (killPoints >= multiplierTiers[i])
            {
                currentMultiplierTierIndex = i;
                SetScoreMultiplier(i + 1);
                break;
            }
        }
    }

    public void ResetKillPointsAndMultiplier()
    {
        killPoints = 0;
        maxKillPoints = 1;
        currentMultiplierTierIndex = 0;
        SetScoreMultiplier(1);
    }

    /*public void AddScoreMultiplier(float value)
    {
        scoreMultiplier += value;
        if (scoreMultiplier > maxScoreMultiplier)
            maxScoreMultiplier = scoreMultiplier;
        UIManager.Instance.SetScoreMultiplier(scoreMultiplier);
    }*/

    public void SetScoreMultiplier(float value)
    {
        if (scoreMultiplier != value)
        {
            scoreMultiplier = value;
            UIManager.Instance.SetScoreMultiplier(scoreMultiplier, praiseTiers[currentMultiplierTierIndex + 1]);
        }
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

        StopCoroutine(scoreUpdaterCoroutine);

        if (currentLevelData.showEndLevelReport)
            UIManager.Instance.OpenEndLevelDialog(score, maxScoreMultiplier, levelTime, PlayerInstance.enemyDefeatedCount, damageTaken, currentLevelData);
        else
            NextLevel();
    }

    public void SetSlomo(float timeScale)
    {
        Time.timeScale = timeScale;
        lerpSlomoCoroutine = StartCoroutine(LerpSlomoVolumeWeight(0.5f, 1));
    }

    public void EndSlomo()
    {
        Time.timeScale = 1;
        if(lerpSlomoCoroutine != null)
            StopCoroutine(lerpSlomoCoroutine);
        StartCoroutine(LerpSlomoVolumeWeight(0.5f, 0));
    }

    private IEnumerator LerpSlomoVolumeWeight(float speed, float target)
    {
        while (slomoVolume.weight < target - 0.01f)
        {
            slomoVolume.weight = Mathf.Lerp(slomoVolume.weight, target, speed);
            yield return null;
        }
        slomoVolume.weight = target;
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
