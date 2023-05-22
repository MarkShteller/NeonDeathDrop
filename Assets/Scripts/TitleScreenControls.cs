using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleScreenControls : MonoBehaviour
{
    [HideInInspector] public PlayerInput playerInput;
    public GameManager gameManagerRef;
    public Button firstBtn;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        firstBtn.Select();
    }

    public void OnNewGame()
    {
        if (GameManager.Instance == null)
        {
            gameManagerRef.CurrentLevelIndex = 1;
            gameManagerRef.additiveScene = Loader.AdditiveScenes.UpperBavelle;
        }
        else
        {
            GameManager.Instance.CurrentLevelIndex = 1;
            GameManager.Instance.additiveScene = Loader.AdditiveScenes.UpperBavelle;
            GameManager.Instance.RestartLevel(false);
        }
        SceneManager.LoadScene(1);
    }

    public void OnTutorial()
    {
        if (GameManager.Instance == null)
        {
            gameManagerRef.CurrentLevelIndex = 0;
            gameManagerRef.additiveScene = Loader.AdditiveScenes.VRSpace;
        }
        else
        {
            GameManager.Instance.CurrentLevelIndex = 0;
            GameManager.Instance.additiveScene = Loader.AdditiveScenes.VRSpace;
            GameManager.Instance.RestartLevel(false);
        }
        SceneManager.LoadScene(1);
    }

    public void OnLevelSelect()
    { 
        SceneManager.LoadScene(3);

    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
