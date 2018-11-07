using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform target;

	// Use this for initialization
	/*void Start () {
	
	}*/
	
	// Update is called once per frame
	void Update () 
	{
        //float oSize = Camera.main.orthographicSize;
        //transform.localPosition = new Vector3(target.localPosition.x - oSize, transform.localPosition.y, target.localPosition.z - oSize);
        transform.localPosition = new Vector3(target.localPosition.x, transform.localPosition.y, target.localPosition.z -15);
	}
}
