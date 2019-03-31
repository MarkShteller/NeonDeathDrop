using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform target;
    public Vector3 targetPosition;

    public enum CameraState { Point, Interpolate, Triangulate }
    public CameraState currentState;
    public Transform targetB;

    public Animation screenGlitchAnim;
    
    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            target = go.transform;
            targetPosition = new Vector3(target.localPosition.x, transform.localPosition.y, target.localPosition.z - 17);
        }
        currentState = CameraState.Point;

        screenGlitchAnim = GetComponentInChildren<Animation>();
        if (screenGlitchAnim == null)
            Debug.LogError("Could not find glitch animation.");
    }

    // Update is called once per frame
    void FixedUpdate () 
	{
        switch (currentState)
        {
            case CameraState.Point:
                if (target != null)
                    targetPosition = new Vector3(target.localPosition.x, transform.localPosition.y, target.localPosition.z - 14.5f); //-8
                else
                    target = GameObject.FindGameObjectWithTag("Player").transform;
                break;
            case CameraState.Interpolate:
                if (target != null && targetB != null)
                {
                    Vector3 lerpedPos = Vector3.Lerp(target.localPosition, targetB.localPosition, 0.3f);
                    targetPosition = new Vector3(lerpedPos.x, transform.localPosition.y, lerpedPos.z - 20);
                }
                else if (target == null)
                    target = GameObject.FindGameObjectWithTag("Player").transform;
                else
                    Debug.LogError("Cant find camera target B.");
                break;
        }
        Vector3 lerpedCamPos = Vector3.Lerp(transform.position, targetPosition, 0.2f);
        transform.position = new Vector3(lerpedCamPos.x, transform.localPosition.y, lerpedCamPos.z);
    }

    public void SetSecondTargerAndInterpolate(Transform targetB)
    {
        this.targetB = targetB;
        currentState = CameraState.Interpolate;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 3, transform.localPosition.z);
        Debug.Log("# camera set to interpolate.");
    }

    public void GlitchScreen()
    {
        screenGlitchAnim.Play();
    }
}
