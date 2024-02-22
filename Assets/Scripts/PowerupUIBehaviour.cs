using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupUIBehaviour : MonoBehaviour
{
    [HideInInspector]
    public PowerUpObject powerUpData;

    public Image powerupImage;
    public Image timerMask;
    public Text timer;
    public Text name;

    private float totalTime;
    private float currentTimer;

    public void Init(PowerUpObject powerUpData)
    {
        this.powerUpData = powerUpData;
        totalTime = currentTimer = powerUpData.effectTime;
        powerupImage.sprite = powerUpData.powerupImage;
        name.text = powerUpData.powerUpName;
    }

    public void UpdateTimer(float newTime)
    {
        totalTime = currentTimer = newTime;
        timerMask.rectTransform.sizeDelta = new Vector2(timerMask.rectTransform.sizeDelta.x, 110);
    }

    private void Update()
    {
        currentTimer -= Time.deltaTime;
        timerMask.rectTransform.sizeDelta = new Vector2(timerMask.rectTransform.sizeDelta.x, currentTimer / totalTime * 110);

        if (currentTimer < 10)
            timer.text = currentTimer.ToString("F1");
        else
            timer.text = currentTimer.ToString("N0");

        if (currentTimer <= 0)
            Destroy(gameObject);
    }
}
