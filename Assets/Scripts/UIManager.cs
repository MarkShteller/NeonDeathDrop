using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

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

    public GameObject hudObject;

    public GameObject interactableLableObject;

    private Animation praiseTextEnterAnim;
    private Coroutine TriggerPraiseCoroutine;

    [SerializeField] private GameOverDialog gameOverDialog;
    [SerializeField] private EndLevelReportDialog endLevelReportDialog;

    public ProjectionCanvasController projectionCanvas;

    void Awake () {
        Instance = this;
	}

    private void Start()
    {
        praiseTextEnterAnim = PraiseText.GetComponent<Animation>();
        PraiseText.gameObject.SetActive(false);
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

    public void OpenGameOverScreen(int score)
    {
        GameOverDialog dialog = Instantiate(gameOverDialog, this.transform);
        dialog.Init(score);
        hudObject.SetActive(false);
    }

    public void SetScoreMultiplier(float scoreMultiplier, string praiseText)
    {
        projectionCanvas.SetTrigger("RingOut");

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

    public void OpenEndLevelDialog(int score, float maxMultiplier, float time, int enemyCount, float damage, LevelScriptableObject level)
    {
        EndLevelReportDialog dialog = Instantiate(endLevelReportDialog, this.transform);
        dialog.Init(score, maxMultiplier, time, enemyCount, damage, level);
       // GameManager.Instance.PlayerInstance.playerInput.SwitchCurrentActionMap("UI");
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
}
