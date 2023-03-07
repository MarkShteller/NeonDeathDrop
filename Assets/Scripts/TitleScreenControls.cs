using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleScreenControls : MonoBehaviour
{
    [HideInInspector] public PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnNewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnLevelSelect()
    { 
        SceneManager.LoadScene(2);

    }
}
