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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentLevelIndex = gameManagerPrefab.CurrentLevelIndex;
            //GameManager.Instance.additiveScene = Loader.AdditiveScenes.UpperBavelle_daytime;
            GameManager.Instance.RestartLevel(false);
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
