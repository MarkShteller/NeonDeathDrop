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

    //private bool showTutorial = false;

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

        if (!PowerupFactory.Instance.powerupTutorialList[(int)type])
        {
            PowerupFactory.Instance.powerupTutorialList[(int)type] = true;
            UIManager.Instance.OpenPowerupTutorialDialog(type);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerInteractTrigger") || 
            (other.CompareTag("PlayerVisuals") && GameManager.Instance.PlayerInstance.currentAttackType == PlayerBehaviour.PlayerAttackType.Pull))
        {
            StartCoroutine(DrawIn(other.transform));
        }
    }

    private IEnumerator DrawIn(Transform target)
    {
        float dist = Vector3.Distance(transform.position, target.position);
        while (dist > 0.5f)
        {
            transform.Rotate(new Vector3(0, 20, 0));
            transform.position = transform.position + (transform.right * dist * 0.4f);
            transform.position = Vector3.Slerp(transform.position, target.position, 0.25f);
            dist = Vector3.Distance(transform.position, target.position);

            yield return null;
        }
        GameManager.Instance.PlayerInstance.AddPowerup(this);
        PickUpAction();
    }

}
