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
        StartCoroutine(Animate(pulseAnim, pulseDuration));
    }

    public void SmallPulse()
    {
        StartCoroutine(Animate(smallPulseAnim, smallPulseDuration));
    }

    public void Popup()
    {
        StartCoroutine(Animate(popupAnim, popupDuration));
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
