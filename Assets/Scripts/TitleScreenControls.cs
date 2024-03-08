using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class TitleScreenControls : MonoBehaviour
{
    [HideInInspector] public PlayerInput playerInput;
    public GameManager gameManagerRef;
    public Button firstBtn;
    public PlayableDirector creditsTimeline;
    public PlayableDirector introTimeline;

    private bool enableControls = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");
        firstBtn.Select();
    }

    public void OnNewGame()
    {
        if (enableControls)
        {
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
        if(enableControls)
            creditsTimeline.Play();
    }

    public void OnCancel()
    {
        creditsTimeline.time = 0;
        creditsTimeline.Evaluate();
        creditsTimeline.Stop();

        introTimeline.time = 4.4f;
        introTimeline.Evaluate();
        //introTimeline.Stop();

        IntroFinished();
    }

    public void IntroFinished()
    {
        enableControls = true;
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
