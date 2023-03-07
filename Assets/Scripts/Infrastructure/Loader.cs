using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    //public GameObject soundManager;
    public enum AdditiveScenes { Title, UpperBavelle, LowerBavelle }

    public AdditiveScenes additiveScene;

    void Awake()
    {
        //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
        if (GameManager.Instance == null)
            //Instantiate gameManager prefab
            Instantiate(gameManager);

        StartCoroutine(LoadAdditive());
    }


    private IEnumerator LoadAdditive()
    {
        SceneManager.LoadScene(additiveScene.ToString(), LoadSceneMode.Additive);
        yield return 0; // wait a frame, so it can finish loading
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(additiveScene.ToString()));
    }
}