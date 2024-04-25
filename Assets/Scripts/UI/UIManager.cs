using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    public bool recordingMode;
    public IconManager iconManager;

    public Slider manaSlider;
    public Slider healthSlider;
    public Text scoreText;
    public Text scoreMultiplierText;
    public Text killPointsText;
    public ProgressBar killPointsBar;
    public Text PraiseText;
    public Text coreCount;

    public Transform powerupsGrid;
    public PowerupUIBehaviour powerupUIPrefab;

    public GameObject SuperBurst;
    public GameObject hudObject;
    public GameObject blackBG;

    public GameObject interactableLableObject;

    public GameObject buttonsHUDObject;
    public GameObject poleInstructionObject;

    private Animation praiseTextEnterAnim;
    private Coroutine TriggerPraiseCoroutine;

    [SerializeField] private GameOverDialog gameOverDialog;
    [SerializeField] private EndLevelReportDialog endLevelReportDialog;

    [SerializeField] private PauseDialog pauseDialog;
    [SerializeField] private OptionsDialog optionsDialog;
    [SerializeField] private TutorialDialog tutorialDialog;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private EndDemoScreen endDemoScreen;
    [SerializeField] private GameObject bossSlainScreen;

    private Stack dialogStack;

    public ProjectionCanvasController projectionCanvas;

    private RebindSaveLoad rebindSaveLoad;

    void Awake () {
        Instance = this;
        dialogStack = new Stack();
        rebindSaveLoad = GetComponent<RebindSaveLoad>();

    }

    private void Start()
    {
        praiseTextEnterAnim = PraiseText.GetComponent<Animation>();
        PraiseText.gameObject.SetActive(false);
        SetRecordingMode(recordingMode);

        rebindSaveLoad.OnLoad();
        GameManager.Instance.GameManagerReady += GameManagerReady;
    }

    private void GameManagerReady()
    {
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            praiseTextEnterAnim.Rewind();
            praiseTextEnterAnim.Play();
        }
    }*/

    public void SetScore(int value)
    {
        scoreText.text = value.ToString("N0");
    }

    public void SetMana(float value)
    {
        manaSlider.value = value;
    }

    public void SetHealth(float value)
    {
        healthSlider.value = value;
    }

    public void SetCoreCount(int value)
    {
        coreCount.text = value.ToString("N0");
    }

    public void SetBlackBG(bool b)
    {
        blackBG.SetActive(b);
    }

    public void OpenGameOverScreen(int score)
    {
        GameOverDialog dialog = Instantiate(gameOverDialog, this.transform);
        dialog.Init(score);
        hudObject.SetActive(false);

    }

    public void SetScoreMultiplier(float scoreMultiplier, string praiseText)
    {
        //projectionCanvas.SetTrigger("RingOut");

        scoreMultiplierText.text = "x"+scoreMultiplier.ToString("N0"); //used to be N1

        /*PraiseText.text = praiseText;
        praiseTextEnterAnim.Rewind();
        praiseTextEnterAnim.Play();*/

        if (TriggerPraiseCoroutine != null)
            StopCoroutine(TriggerPraiseCoroutine);
        TriggerPraiseCoroutine = StartCoroutine(TriggerPraiseAnim(praiseText));
    }

    public void SetScoreMultiplier(float scoreMultiplier)
    {
        scoreMultiplierText.text = "x" + scoreMultiplier.ToString("N0"); //used to be N1
    }

    private IEnumerator TriggerPraiseAnim(string text)
    {
        yield return new WaitForSeconds(0.3f);
        PraiseText.gameObject.SetActive(true);

        PraiseText.text = text;
        praiseTextEnterAnim.Rewind();
        praiseTextEnterAnim.Play();
    }

    public void SetKillPoints(float value, float maxValue)
    {
        if (value == 0)
        {
            killPointsBar.gameObject.SetActive(false);
            PraiseText.gameObject.SetActive(false);
        }
        else
        {
            killPointsBar.gameObject.SetActive(true);
        }
        killPointsText.text = value.ToString();
        killPointsBar.min = 0;
        killPointsBar.max = maxValue;
        killPointsBar.current = value;
    }

    public IEnumerator PlaySuperBurst()
    {
        SuperBurst.SetActive(true);
        SuperBurst.GetComponent<Animator>().SetTrigger("Play");
        yield return new WaitForSeconds(1);
        SuperBurst.SetActive(false);
    }

    public void OpenPauseDialog()
    {
        GameManager.Instance.SetDeathSnapshotIntensity(0.5f);
        OpenDialogGeneric(pauseDialog.gameObject);
    }

    public void OpenOptionsDialog()
    {
        OpenDialogGeneric(optionsDialog.gameObject);
    }

    public void OpenEndDemoDialog()
    {
        OpenDialogGeneric(endDemoScreen.gameObject);
    }

    public void OpenTutorialDialog(int tutorialIndex)
    {
        OpenDialogGeneric(tutorialDialog.gameObject);
        tutorialDialog.Populate(tutorialManager.tutorialObjects[tutorialIndex]);
    }

    public void OpenPowerupTutorialDialog(PowerUpType type)
    {
        int indexOffset = 5;
        OpenDialogGeneric(tutorialDialog.gameObject);
        tutorialDialog.Populate(tutorialManager.tutorialObjects[indexOffset + (int)type]);
    }

    private void OpenDialogGeneric(GameObject dialog)
    {
        //hide the dialog thats on top of the stack for future recovery
        if (dialogStack.Count > 0)
        {
            GameObject topDialog = dialogStack.Peek() as GameObject;
            topDialog.SetActive(false);
        }

        dialogStack.Push(dialog);
        GameManager.Instance.SetDuckMusicIntensity(0.5f);
        GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");

        Time.timeScale = 0;
        dialog.SetActive(true);
        dialog.GetComponent<AbstractDialog>().SelectFirst();
        dialog.GetComponent<AbstractDialog>().SetIcons(iconManager.currentIcons);
    }

    public void CloseTopDialog()
    {
        if (dialogStack.Count > 0)
        {
            GameObject topDialog = dialogStack.Pop() as GameObject;
            topDialog.GetComponent<AbstractDialog>().CloseDialog();
            topDialog.SetActive(false);
        }

        if (dialogStack.Count == 0)
        {
            Time.timeScale = 1;
            if (!GameManager.Instance.isTutorial)
                GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            else
                GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("PlayerTutorial");
            GameManager.Instance.SetDeathSnapshotIntensity(0f);
            GameManager.Instance.SetDuckMusicIntensity(0f);
            rebindSaveLoad.OnSave();
        }
        else
        {
            GameObject topDialog = dialogStack.Peek() as GameObject;
            topDialog.SetActive(true);
            topDialog.GetComponent<AbstractDialog>().SelectFirst();
        }
    }

    public void CloseAllDialogs()
    {
        GameObject topDialog = dialogStack.Peek() as GameObject;
        topDialog.SetActive(false);
        dialogStack.Clear();
        rebindSaveLoad.OnSave();

        Time.timeScale = 1;
        if (!GameManager.Instance.isTutorial)
            GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        else
            GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("PlayerTutorial");
        GameManager.Instance.SetDuckMusicIntensity(0f);
        GameManager.Instance.SetDeathSnapshotIntensity(0f);
    }

    public void OpenEndLevelDialog(int score, float maxMultiplier, float time, int enemyCount, float damage, LevelScriptableObject level)
    {
        EndLevelReportDialog dialog = Instantiate(endLevelReportDialog, this.transform);
        dialog.Init(score, maxMultiplier, time, enemyCount, damage, level);
        dialog.SelectFirst();
        GameManager.Instance.PlayerInstance.playerInput.SwitchCurrentActionMap("UI");

        Analytics.CustomEvent("EndLevelReport",
            new Dictionary<string, object> {
                { "currentLevelName", level.displayName },
                { "score", score },
                { "maxMultiplier", maxMultiplier},
                { "time", time},
                { "enemyCount", enemyCount},
                { "damage", damage}
            }
        );
    }

    public void BossSlain()
    {
        print("show slay UI");
        bossSlainScreen.SetActive(true);
        bossSlainScreen.GetComponent<Animator>().SetTrigger("Slay");

        Analytics.CustomEvent("BossSlain");
    }

    public void AddPowerup(PowerUpObject powerupData)
    {
        PowerupUIBehaviour pub = Instantiate(powerupUIPrefab, powerupsGrid);
        pub.Init(powerupData);
    }

    public void UpdatePowerupTimer(PowerUpObject powerupData, float newTime)
    {
        foreach (PowerupUIBehaviour pub in powerupsGrid.GetComponentsInChildren<PowerupUIBehaviour>())
        {
            if (pub.powerUpData.Equals(powerupData))
            {
                pub.UpdateTimer(newTime);
                break;
            }
        }
    }

    public void SetInteractableVisible(bool b)
    {
        interactableLableObject.SetActive(b);
    }

    public void SetPoleInstructionVisible(bool b)
    {
        poleInstructionObject.SetActive(b);
    }

    public void SetHUDVisible(bool b)
    {
        if(!recordingMode)
            hudObject.SetActive(b);
        else
            hudObject.SetActive(false);
    }

    public void SetRecordingMode(bool b)
    {
        recordingMode = b;
        SetHUDVisible(!recordingMode);
    }
}
