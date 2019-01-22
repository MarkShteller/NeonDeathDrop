using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	private Transform target;

    public enum CameraState { Point, Interpolate, Triangulate }
    public CameraState currentState;
    public Transform targetB;
    
    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if(go != null)
            target = go.transform;

        currentState = CameraState.Point;
    }

    // Update is called once per frame
    void Update () 
	{
        switch (currentState)
        {
            case CameraState.Point:
                if (target != null)
                    transform.localPosition = new Vector3(target.localPosition.x, transform.localPosition.y, target.localPosition.z - 17);
                else
                    target = GameObject.FindGameObjectWithTag("Player").transform;
                break;
            case CameraState.Interpolate:
                if (target != null && targetB != null)
                {
                    Vector3 lerpedPos = Vector3.Lerp(target.localPosition, targetB.localPosition, 0.3f);
                    transform.localPosition = new Vector3(lerpedPos.x, transform.localPosition.y, lerpedPos.z - 20);
                }
                break;
        }
    }

    public void SetSecondTargerAndInterpolate(Transform targetB)
    {
        this.targetB = targetB;
        currentState = CameraState.Interpolate;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 3, transform.localPosition.z);
        Debug.Log("# camera set to interpolate.");
    }
}
