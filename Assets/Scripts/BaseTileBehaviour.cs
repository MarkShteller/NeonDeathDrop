using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTileBehaviour : MonoBehaviour {

    public AnimationCurve dropAnim;
    public float dropDuration;
    public AnimationCurve riseAnim;
    public float riseDuration;
    public AnimationCurve slideDownAnim;
    public float slideDownDuration;
    public AnimationCurve pulseAnim;
    public float pulseDuration;

    public void Drop()
    {
        StartCoroutine(Animate(dropAnim, dropDuration));
    }

    public void Rise()
    {
        StartCoroutine(Animate(riseAnim, riseDuration));
    }

    public void SlideDown()
    {
        StartCoroutine(Animate(slideDownAnim, slideDownDuration));
    }

    public void Pulse()
    {
        StartCoroutine(Animate(pulseAnim, pulseDuration));
    }

    private IEnumerator Animate(AnimationCurve curve, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float yPos = curve.Evaluate(time/duration);
            //print("time: "+time +" duration: "+duration +" ypos: "+yPos);
            transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
            //print(transform.position);
            yield return null;
            time += Time.deltaTime;
        }
    }

}
