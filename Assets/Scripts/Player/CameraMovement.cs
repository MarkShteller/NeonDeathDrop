using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraMovement : MonoBehaviour {

	public Transform target;
    public Vector3 targetPosition;
    public float smoothStep;

    public enum CameraState { Point, FollowVertically, Interpolate, Triangulate }
    public CameraState currentState;
    public Transform targetB;

    public CinemachineVirtualCamera VCamDashImpact;
    public CinemachineVirtualCamera finisherVCam;
    public CinemachineVirtualCamera heavyAttackVCam;
    public CinemachineVirtualCamera sublevelUpVCam;
    public CinemachineVirtualCamera shockwaveVCam;

    public Animation screenGlitchAnim;
    public Animator animator;

    public CinemachineTargetGroup targetGroup;

    private void Start()
    {
        VCamDashImpact.gameObject.SetActive(false);

        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            target = go.transform;
            targetPosition = new Vector3(target.localPosition.x, transform.localPosition.y, target.localPosition.z - 17);
            targetGroup.AddMember(go.transform, 1, 0);
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
                {
                    target = GameObject.FindGameObjectWithTag("Player").transform;
                    if(target != null && targetGroup != null) targetGroup.AddMember(target, 1, 0);
                }
                break;
            case CameraState.FollowVertically:
                if (target != null)
                    targetPosition = new Vector3(target.localPosition.x, target.localPosition.y +15f, target.localPosition.z - 14.5f);
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
        Vector3 lerpedCamPos = Vector3.Lerp(transform.position, targetPosition, smoothStep * Time.deltaTime);
        transform.position = new Vector3(lerpedCamPos.x, lerpedCamPos.y, lerpedCamPos.z);
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
        //screenGlitchAnim.Play();
        animator.SetTrigger("Glitch");
    }


    public void FastZoom(Transform additionalTarget)
    {
        //animator.SetTrigger("FastZoom");
        StartCoroutine(SwitchVCam(VCamDashImpact,additionalTarget, 0.3f));
    }

    public void FrameHeavyAttack(Transform additionalTarget)
    { 
        StartCoroutine(SwitchVCam(heavyAttackVCam, additionalTarget, 0.3f));
    }

    public void FrameSublevelUpJump(float time)
    { 
        StartCoroutine(SwitchVCam(sublevelUpVCam, null, time));
    }

    public void FrameShockwave(float time, Transform lookAt)
    { 
        StartCoroutine(SwitchVCam(shockwaveVCam, null, time, lookAt));
    }

    public void SetLowHealth(bool b)
    {
        animator.SetBool("LowHealth", b);
    }

    private IEnumerator SwitchVCam(CinemachineVirtualCamera vCamera, Transform additionalTarget, float time, Transform lookAtTarget = null)
    {
        //targetGroup.AddMember(additionalTarget, 1, 0);
        if(additionalTarget != null)
            vCamera.Follow = additionalTarget;
        if(lookAtTarget != null)
            vCamera.LookAt = lookAtTarget;
        vCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        //targetGroup.RemoveMember(additionalTarget);
        vCamera.gameObject.SetActive(false);
    }

    public void SetFinisher(Transform target, float time)
    {
        StartCoroutine(SwitchFinisherCam(target, time));
    }

    private IEnumerator SwitchFinisherCam(Transform target, float time)
    {
        finisherVCam.Follow = GameManager.Instance.PlayerInstance.transform;
        finisherVCam.LookAt = target;
        finisherVCam.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        finisherVCam.gameObject.SetActive(false);
    }

    public void RecenterCameraHeight(float y)
    {
        transform.position = new Vector3(transform.position.x, y+15, transform.position.z);
    }
}
