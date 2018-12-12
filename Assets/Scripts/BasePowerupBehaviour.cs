using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePowerupBehaviour : MonoBehaviour {

    public PowerUpObject powerUpData;


    public void PickUpAction()
    {
        //todo: do some particles 
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerInteractTrigger"))
        {
            GameManager.Instance.PlayerInstance.AddPowerup(powerUpData);
            PickUpAction();
        }
    }

}
