using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public LevelGenerator levelGenerator;

    public int overrideLevelIndex = -1;
    //public GameObject soundManager;
    public enum AdditiveScenes { TitleScreen, UpperBavelle, LowerBavelle, VRSpace }


    void Awake()
    {
        /*if (overrideLevelIndex != -1)
            gameManager.GetComponent<GameManager>().CurrentLevelIndex = overrideLevelIndex;
        */
        //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
        if (GameManager.Instance == null)
            //Instantiate gameManager prefab
            Instantiate(gameManager);

        StartCoroutine(LoadAdditive());
    }


    private IEnumerator LoadAdditive()
    {
        if (GameManager.Instance.additiveScene == AdditiveScenes.VRSpace)
            levelGenerator.isVRSpace = true;

        SceneManager.LoadScene(GameManager.Instance.additiveScene.ToString(), LoadSceneMode.Additive);
        yield return null; // wait a frame, so it can finish loading
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameManager.Instance.additiveScene.ToString()));
    }
}