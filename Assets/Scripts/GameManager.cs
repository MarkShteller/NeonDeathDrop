using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using UnityEngine.Video;
using static Loader;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public bool isTutorial;

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
    public VideoPlayer videoPlayer;
    public int CurrentLevelIndex = 0;

    [HideInInspector] public AdditiveScenes additiveScene;

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

    public Action GameManagerReady;

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

        additiveScene = levelManager.GetLevelData(CurrentLevelIndex).additiveScene;
        StartCoroutine(InitLevel());
	}

    private void InitSFX()
    {
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

        
        PlayerObject = Resources.Load("Player 2.0") as GameObject;
        GameObject go = Instantiate(PlayerObject);
        PlayerInstance = go.GetComponent<PlayerBehaviour>();
        playerPointPosition = null;

        GameObject companionObject = Resources.Load("Alex") as GameObject;
        companion = Instantiate(companionObject).GetComponent<CompanionBehaviour>();

        print("Loading level number "+CurrentLevelIndex);
        currentLevelData = levelManager.Init(CurrentLevelIndex);

        DialogManager.Instance.InitLevelDialogManager(currentLevelData.dialogSheetName);

        if (currentLevelData.isVRSpace)
        {
            PlayerInstance.SwitchToVRSpaceOutfit();
        }
        /*if (currentLevelData.includeCompanion)
        {
            GameObject companionObject = Resources.Load("Alex") as GameObject;
            companion = Instantiate(companionObject).GetComponent<CompanionBehaviour>();
        }*/

        if (currentLevelData.isPlayVideoOnStart)
        {
            UIManager.Instance.SetBlackBG(true);
            videoPlayer.gameObject.SetActive(true);
            //videoPlayer.targetCamera = Camera.main;
            videoPlayer.clip = currentLevelData.videoClip;
            videoPlayer.loopPointReached += VideoEnd;
           // videoPlayer.audioOutputMode = VideoAudioOutputMode.APIOnly;
        }

        if (currentLevelData.startPlayerFalling)
        {
            PlayerInstance.isStartWithFalling = true;
        }

        score = 0;
        scoreMultiplier = 1;
        maxScoreMultiplier = scoreMultiplier;
        killPoints = 0;
        timeLevelStarted = Time.time;
        damageTaken = 0;

        yield return null;

        scoreUpdaterCoroutine = StartCoroutine(KillPointsUpdater());
        UIManager.Instance.SetScore(score);
        UIManager.Instance.SetScoreMultiplier(scoreMultiplier);

        cameraRef = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        //print(cameraRef.currentState);

        //init music after once awakes are done
        if(!currentLevelData.startMusicWithEvent)
            StartCoroutine(AudioManager.Instance.StartMusic(currentLevelData.levelMusicTrack));
        AudioManager.Instance.StartAmbiance(AudioManager.Instance.EnvAmbienceHighway);

        InitSFX();

        print("## GameManager ready");
        GameManagerReady();
    }

    private void VideoEnd(VideoPlayer vp)
    {
        UIManager.Instance.SetBlackBG(false);
        videoPlayer.loopPointReached -= VideoEnd;
        videoPlayer.gameObject.SetActive(false);
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

    public void SetPlayerPosition(Vector3 position, LevelScriptableObject levelData)
    {
        if (PlayerInstance == null)
        {
            GameObject go = Instantiate(PlayerObject);
            PlayerInstance = go.GetComponent<PlayerBehaviour>();
        }
        PlayerInstance.transform.position = position;
        PlayerInstance.visualsHolder.transform.eulerAngles = levelData.PlayerSpawnRotation;

        if (levelData.includeCompanion)
        {
            companion.transform.position = new Vector3(position.x - companion.stopDistance, position.y, position.z);
            companion.SetPlayerRef(PlayerInstance);
        }
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
        DeathSnapshot.setParameterByName("Death_MusicDuck_Intensity", 0);
        if(UIManager.Instance != null)
            UIManager.Instance.hudObject.SetActive(true);

        score = 0;
        this.StopAllCoroutines();
        levelManager.ClearActiveLevels();
        AudioManager.Instance.StopAllSounds();
        additiveScene = levelManager.GetLevelData(CurrentLevelIndex).additiveScene;
        SceneManager.LoadScene(1); // Load MainCore scene which will load the rest of the scenes

        StartCoroutine(InitLevel());
    }

    public void NextLevel()
    {
        CurrentLevelIndex++;
        additiveScene = AdditiveScenes.UpperBavelle_sunset;
        cameraRef.SetLowHealth(false);
        RestartLevel(false);
    }

    public LevelGenerator GetCurrentSublevel()
    {
        //print("getting sublevel");
        return levelManager.GetCurrentSublevel();
    }

    public void AddScore(int points)
    {
        score += (int) Math.Round(points * scoreMultiplier);
        UIManager.Instance.SetScore(score);
    }

    public void GameOver()
    {
        DeathSnapshot.setParameterByName("Death_MusicDuck_Intensity", 1f);
        //Time.timeScale = 0;
        EnemyManager.Instance.SetUpdateEnemies(false);
        UIManager.Instance.OpenGameOverScreen(score);

        try
        {
            AnalyticsService.Instance.RecordEvent(new GameOverEvent()
            {
                currentLevelIndex = CurrentLevelIndex,
                lastPlayerPosition = playerPointPosition.x.ToString() + "," + playerPointPosition.y.ToString()
            });
        } catch (Exception e) { Debug.LogWarning(e.Message); }
    }

    public void SetDuckMusicIntensity(float value)
    { 
        //DeathSnapshot.setParameterByName("Death_MusicDuck_Intensity", value);
        SlomoSnapshot.setParameterByName("SlowMo_Effect_Intensity", value);
    }

    public void SetDeathSnapshotIntensity(float value)
    {
        DeathSnapshot.setParameterByName("Death_MusicDuck_Intensity", value);
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
            if (maxScoreMultiplier < scoreMultiplier)
                maxScoreMultiplier = scoreMultiplier;
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
        try
        {
            AnalyticsService.Instance.RecordEvent(new LevelFinishedEvent()
            {
                currentLevelIndex = CurrentLevelIndex,
                damageTaken = (int)damageTaken,
                enemyCount = PlayerInstance.enemyDefeatedCount,
                maxMultiplier = (int)maxScoreMultiplier,
                totalTime = levelTime,
                score = score
            });
        }
        catch (Exception e) { }
    }

    internal void MoveLevelDown()
    {
        levelManager.NextSublevel(true);
    }

    internal void MoveLevelUp()
    {
        levelManager.NextSublevel(false);
    }

    public IEnumerator TriggerFinisher(Transform target, float time)
    {
        companion.FinisherMove(PlayerInstance.finisherSpline);
        //cameraRef.SetFinisher(target, time);
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

    public void LandingShockwaveSlomo(float duration)
    {
        StartCoroutine(SlomoCoroutine(duration, slomoCurves[2]));
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
            float result = curve.Evaluate(time / duration);
            Time.timeScale = result < 0 ? 0 : result;

            time += Time.time - timeStarted;
            yield return null;
        }
    }


    public string GetCurrentLevelName()
    {
        return currentLevelData.name;
    }

    internal void ClearLevel()
    {
        levelManager.ClearActiveLevels();
    }

    
}
