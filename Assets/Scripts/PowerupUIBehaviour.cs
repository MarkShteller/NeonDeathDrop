using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupUIBehaviour : MonoBehaviour
{
    [HideInInspector]
    public PowerUpObject powerUpData;

    public Image powerupImage;
    public Text timer;

    private float totalTime;
    private float currentTimer;

    public void Init(PowerUpObject powerUpData)
    {
        this.powerUpData = powerUpData;
        totalTime = currentTimer = powerUpData.effectTime;
        powerupImage = powerUpData.powerupImage;
    }

    public void UpdateTimer(float newTime)
    {
        totalTime = currentTimer = newTime;
    }

    private void Update()
    {
        currentTimer -= Time.deltaTime;
        if(currentTimer < 10)
            timer.text = currentTimer.ToString("F1");
        else
            timer.text = currentTimer.ToString("N0");

        if (currentTimer <= 0)
            Destroy(gameObject);
    }
}
