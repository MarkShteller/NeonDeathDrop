using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Point playerPointPosition;

	// Use this for initialization
	void Awake ()
    {
        Instance = this;
        
	}

    /*
    void Start()
    {
        // Switch to 640 x 480 fullscreen at 60 hz
        Screen.SetResolution(640, 480, true, 60);
    }*/

    // Update is called once per frame
    /*void Update ()
    {
        if (playerPointPosition != null)
        {
            //print("player pos on grid: " + playerPointPosition.x + "," + playerPointPosition.y);

        }
	}*/

    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 50, 200, 100), "Change resolution"))
        {
            print("### changed resolution");
            //Screen.SetResolution(1280, 720, true);
            //Camera.main.rect = new Rect((Screen.width - 512.0f) / Screen.width, (Screen.height - 256.0f) / Screen.height, 384.0f / Screen.width, 96.0f / Screen.height);
            Camera.main.rect = new Rect(0f, 0f, 384.0f / Screen.width, 96.0f / Screen.height);
        }
    }
}
