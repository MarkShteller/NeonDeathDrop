using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTileBehaviour : MonoBehaviour {

    public GameObject MainPillar;
    public GameObject SelectedPillar;

    public AnimationCurve dropAnim;
    public float dropDuration;
    public AnimationCurve riseAnim;
    public float riseDuration;
    public AnimationCurve slideDownAnim;
    public float slideDownDuration;
    public AnimationCurve pulseAnim;
    public float pulseDuration;
    public AnimationCurve smallPulseAnim;
    public float smallPulseDuration;
    public AnimationCurve popupAnim;
    public float popupDuration;

    public void Drop()
    {
        StartCoroutine(Animate(dropAnim, dropDuration));
    }

    public IEnumerator WeakDrop(float targetHeight, Action callback = null)
    {
        yield return StartCoroutine(Animate(dropAnim, dropDuration, targetHeight));
        if(callback !=  null) callback();
    }

    public void Rise()
    {
        StartCoroutine(Animate(riseAnim, riseDuration));
    }

    public void SelectPillar()
    {
        if (MainPillar && SelectedPillar)
        {
            MainPillar.SetActive(false);
            SelectedPillar.SetActive(true);
        }
    }

    public void DeselectPillar()
    {
        if (MainPillar && SelectedPillar)
        {
            MainPillar.SetActive(true);
            SelectedPillar.SetActive(false);
        }
    }

    public void SlideDown()
    {
        StartCoroutine(Animate(slideDownAnim, slideDownDuration));
    }

    public void Pulse()
    {
        StartCoroutine(AnimateVisuals(pulseAnim, pulseDuration));
    }

    public void SmallPulse()
    {
        StartCoroutine(AnimateVisuals(smallPulseAnim, smallPulseDuration));
    }

    public void Popup()
    {
        StartCoroutine(AnimateVisuals(popupAnim, popupDuration));
    }

    private IEnumerator AnimateVisuals(AnimationCurve curve, float duration, float targetHeight =0)
    {
        float time = 0;
        while (time < duration)
        {
            float yPos = curve.Evaluate(time/duration);
            //print("time: "+time +" duration: "+duration +" ypos: "+yPos);
            MainPillar.transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
            
            if(targetHeight != 0 && yPos <= -targetHeight)
                break;

            //print(transform.position);
            yield return null;
            time += Time.deltaTime;
        }
    }

    private IEnumerator Animate(AnimationCurve curve, float duration, float targetHeight = 0)
    {
        float time = 0;
        while (time < duration)
        {
            float yPos = curve.Evaluate(time / duration);
            //print("time: "+time +" duration: "+duration +" ypos: "+yPos);
            transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);

            if (targetHeight != 0 && yPos <= -targetHeight)
                break;

            //print(transform.position);
            yield return null;
            time += Time.deltaTime;
        }
    }

}
