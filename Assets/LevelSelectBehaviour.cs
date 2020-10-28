using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectBehaviour : MonoBehaviour
{
    public GameManager gameManagerPrefab;
    public InputField inputField;


    public void StartAction()
    {
        gameManagerPrefab.CurrentLevelIndex = int.Parse(inputField.text);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
