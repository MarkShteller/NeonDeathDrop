using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.Diagnostics;

public class TitleScreenControls : MonoBehaviour
{
    [HideInInspector] public PlayerInput playerInput;
    public GameManager gameManagerRef;
    public Button firstBtn;
    public OptionsDialog optionsDialog;
    public PlayableDirector creditsTimeline;
    public PlayableDirector introTimeline;
    public AnalyticsBehavoiur analyticsBehavoiur;

    private bool enableControls = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");
        

        print("Unity services state: "+UnityServices.State);
        //print("TitleScreen event analyticsResult: " + analyticsResult);
    }

    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K))
            Utils.ForceCrash(ForcedCrashCategory.FatalError);
    }

    public void OnNewGame()
    {
        if (enableControls)
        {
            FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.UIRestart);
            AudioManager.Instance.StopAllSounds();

            if (GameManager.Instance == null)
            {
                gameManagerRef.CurrentLevelIndex = 0;
                gameManagerRef.additiveScene = Loader.AdditiveScenes.UpperBavelle_daytime;
                SceneManager.LoadScene(1);
            }
            else
            {
                GameManager.Instance.CurrentLevelIndex = 0;
                GameManager.Instance.additiveScene = Loader.AdditiveScenes.UpperBavelle_daytime;
                GameManager.Instance.RestartLevel(false);
            }
        }
        AnalyticsService.Instance.RecordEvent("NewGame");
        print("## analytics init with session id: " + AnalyticsService.Instance.SessionID);
        //AnalyticsResult analyticsResult = Analytics.CustomEvent("NewGame");
        //print("NewGame event analyticsResult: " + analyticsResult);
    }

    public void OnTutorial()
    {
        if (enableControls)
        {
            if (GameManager.Instance == null)
            {
                gameManagerRef.CurrentLevelIndex = 1;
                gameManagerRef.additiveScene = Loader.AdditiveScenes.VRSpace;
            }
            else
            {
                GameManager.Instance.CurrentLevelIndex = 1;
                GameManager.Instance.additiveScene = Loader.AdditiveScenes.VRSpace;
                GameManager.Instance.RestartLevel(false);
            }
            SceneManager.LoadScene(1);
        }
    }

    public void OnLevelSelect()
    { 
        if(enableControls)
            SceneManager.LoadScene(7);
    }

    public void OnCredits()
    {
        if (enableControls)
        { 
            creditsTimeline.Play();
            AnalyticsService.Instance.RecordEvent("Credits");
        }
    }

    public void OnSettings()
    {
        optionsDialog.gameObject.SetActive(true);
        //optionsDialog.SelectFirst();
    }

    public void OnCancel()
    {
        creditsTimeline.time = 0;
        creditsTimeline.Evaluate();
        creditsTimeline.Stop();

        introTimeline.time = 5.5f;
        introTimeline.Evaluate();

        if (optionsDialog.gameObject.active)
        {
            optionsDialog.CloseDialog();
            optionsDialog.gameObject.SetActive(false);
            firstBtn.Select();
        }
    }

    public void IntroFinished()
    { 
        enableControls = true;
    }

    public void PrivacyMessage()
    {
        playerInput.SwitchCurrentActionMap("Privacy");
        introTimeline.Stop();
        print("privacy message");
    }

    public void OnAccept()
    {
        analyticsBehavoiur.ConsentGiven();
        introTimeline.time = 6.4f;
        introTimeline.Evaluate();
        introTimeline.Play();
        playerInput.SwitchCurrentActionMap("UI");
        AnalyticsService.Instance.RecordEvent("TitleScreen");
        firstBtn.Select();
        AudioManager.Instance.StartAmbiance(AudioManager.Instance.EnvAmbienceHighway); 
    }

    public void OnDecline()
    {
        //AnalyticsService.Instance.StopDataCollection();
        introTimeline.time = 6.4f;
        introTimeline.Evaluate();
        introTimeline.Play();
        playerInput.SwitchCurrentActionMap("UI");
        firstBtn.Select();
        AudioManager.Instance.StartAmbiance(AudioManager.Instance.EnvAmbienceHighway);
        //enableControls = true;
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
