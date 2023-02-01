using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePowerupBehaviour : MonoBehaviour {

    public PowerUpObject powerUpData;


    [HideInInspector] public PowerUpType type;
    [HideInInspector] public float bonus;
    [HideInInspector] public float effectTime;
    [HideInInspector] public int count;
    [HideInInspector] public string powerUpName;

    private void Start()
    {
        type = powerUpData.type;
        bonus = powerUpData.bonus;
        effectTime = powerUpData.effectTime;
        count = powerUpData.count;
        powerUpName = powerUpData.powerUpName;
    }

    public void PickUpAction()
    {
        //todo: do some particles 
        //Destroy(gameObject);
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerInteractTrigger"))
        {
            StartCoroutine(DrawIn(other.transform));
        }
    }

    private IEnumerator DrawIn(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, 0.25f);
            yield return null;
        }
        GameManager.Instance.PlayerInstance.AddPowerup(this);
        PickUpAction();
    }

}
