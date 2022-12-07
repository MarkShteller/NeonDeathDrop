using FMOD.Studio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

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

    public CompanionBehaviour companion;

    public LevelManager levelManager;

    [SerializeField] private VisualEffect HoleFinisherVFX;

    public AnimationCurve[] slomoCurves;
    public Volume slomoVolume;
    public Volume takingDanmageVolume;
    public int CurrentLevelIndex = 0;

    private LevelScriptableObject currentLevelData;

    private float maxScoreMultiplier;
    private float timeLevelStarted;
    private float damageTaken;

    private Coroutine lerpSlomoCoroutine;
    private Coroutine scoreUpdaterCoroutine;
    private float fixedDeltaTime;

    private EventInstance SlomoSnapshot;
    private EventInstance SlomoSFX;
    private EventInstance DeathSnapshot;
    private float slomoIntensity;

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
        this.fixedDeltaTime = Time.fixedDeltaTime;

        
        StartCoroutine(InitLevel());
        print("## GameManager ready");
	}

    private void Start()
    {
        //init music after once awakes are done
        StartCoroutine(AudioManager.Instance.StartMusic());

        SlomoSnapshot = FMODUnity.RuntimeManager.CreateInstance("snapshot:/SlowMo_Effect");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(SlomoSnapshot, PlayerInstance.transform, PlayerInstance.GetComponent<Rigidbody>());
        SlomoSnapshot.start();

        SlomoSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Environment/Slowmo");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(SlomoSFX, PlayerInstance.transform, PlayerInstance.GetComponent<Rigidbody>());

        DeathSnapshot = FMODUnity.RuntimeManager.CreateInstance("snapshot:/Death_MusicDuck");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(DeathSnapshot, PlayerInstance.transform, PlayerInstance.GetComponent<Rigidbody>());
        DeathSnapshot.start();
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

        GameObject companionObject = Resources.Load("Alex") as GameObject;
        companion = Instantiate(companionObject).GetComponent<CompanionBehaviour>();

        score = 0;
        scoreMultiplier = 1;
        maxScoreMultiplier = scoreMultiplier;
        killPoints = 0;
        timeLevelStarted = Time.time;
        damageTaken = 0;
        scoreUpdaterCoroutine = StartCoroutine(KillPointsUpdater());

        UIManager.Instance.SetScore(score);
        UIManager.Instance.SetScoreMultiplier(scoreMultiplier);

        yield return null;

        cameraRef = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        print(cameraRef.currentState);
        currentLevelData = levelManager.Init(CurrentLevelIndex);

        /////////////
        
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

        companion.transform.position = new Vector3(position.x - companion.stopDistance, position.y, position.z);
        companion.SetPlayerRef(PlayerInstance);
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
        DeathSnapshot.setParameterByName("Death_MusicDuck_Intensity", 0);

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
        DeathSnapshot.setParameterByName("Death_MusicDuck_Intensity", 1f);
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

    public IEnumerator TriggerFinisher(Transform target, float time)
    {
        companion.FinisherMove(PlayerInstance.finisherSpline);
        cameraRef.SetFinisher(target, time);
        HoleFinisherVFX.Play();
        HoleFinisherVFX.transform.position = target.position;
        yield return new WaitForSeconds(time);
        HoleFinisherVFX.Stop();
    }

    public void SetSlomo(float timeScale)
    {
        SlomoSFX.start();
        SlomoSnapshot.setParameterByName("SlowMo_Effect_Intensity", 1);
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        lerpSlomoCoroutine = StartCoroutine(LerpSlomoVolumeWeight(0.5f, 1));
    }

    public void EndSlomo()
    {
        SlomoSnapshot.setParameterByName("SlowMo_Effect_Intensity", 0);
        SlomoSFX.stop(STOP_MODE.ALLOWFADEOUT);
        Time.timeScale = 1;
        Time.fixedDeltaTime = this.fixedDeltaTime;
        if (lerpSlomoCoroutine != null)
            StopCoroutine(lerpSlomoCoroutine);
        StartCoroutine(LerpSlomoVolumeWeight(0.5f, 0));
    }

    public void TakeDamage()
    {
        StopCoroutine("LerpDamageVolumeWeight");
        StartCoroutine(LerpDamageVolumeWeight(0.5f));
    }

    private IEnumerator LerpDamageVolumeWeight(float speed)
    {
        while (takingDanmageVolume.weight < 0.95f)
        {
            takingDanmageVolume.weight = Mathf.Lerp(takingDanmageVolume.weight, 1, speed);
            yield return null;
        }
        while (takingDanmageVolume.weight > 0.05f)
        {
            takingDanmageVolume.weight = Mathf.Lerp(takingDanmageVolume.weight, 0, speed);
            yield return null;
        }
        takingDanmageVolume.weight = 0;
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
