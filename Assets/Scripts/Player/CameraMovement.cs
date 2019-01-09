using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	private Transform target;

    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if(go != null)
            target = go.transform;
    }

    // Update is called once per frame
    void Update () 
	{
        if(target != null)
            transform.localPosition = new Vector3(target.localPosition.x, transform.localPosition.y, target.localPosition.z -17);
        else
            target = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
