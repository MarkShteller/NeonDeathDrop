using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GateTileBehaviour : BaseTileBehaviour
{
    public int gateEnemyDeathGoal = 0;
    public bool isOpen = false;
    public TMP_Text gateText;

    private void Start()
    {
        StartCoroutine(UpdateGateEvery(0.5f));
    }

    private IEnumerator UpdateGateEvery(float time)
    {
        while (true)
        {
            int count = gateEnemyDeathGoal - GameManager.Instance.PlayerInstance.enemyDefeatedCount;
            if (count <= 0)
            {
                gateText.text = "0";
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.AreaUnlock, transform.position);
                break;
            }
            else
            {
                gateText.text = count.ToString();
            }
            yield return new WaitForSeconds(time);
        }
    }
}
