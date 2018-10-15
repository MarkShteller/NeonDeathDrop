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

    // Update is called once per frame
    /*void Update ()
    {
        if (playerPointPosition != null)
        {
            //print("player pos on grid: " + playerPointPosition.x + "," + playerPointPosition.y);

        }
	}*/
}
