using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableDialogObject : MonoBehaviour
{
    public string conversationID;
    public bool isPlayerTriggered;
    public bool isTriggeredOnce;
    private bool isEnabled = true;
    public UnityEvent automatedEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isEnabled)
            {
                if (isPlayerTriggered)
                {
                    //show interact button
                    UIManager.Instance.SetInteractableVisible(true);
                    //attach interact action
                    GameManager.Instance.PlayerInstance.interactionEvent += OnInteract;
                }
                else
                {
                    if (isTriggeredOnce)
                        isEnabled = false;
                    automatedEvent.Invoke();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isPlayerTriggered)
            {

                UIManager.Instance.SetInteractableVisible(false);
                //detach interact action
                GameManager.Instance.PlayerInstance.interactionEvent -= OnInteract;
            }
        }
    }

    private void OnInteract()
    {
        if (isTriggeredOnce)
            isEnabled = false;
        UIManager.Instance.SetInteractableVisible(false);
        DialogManager.Instance.ShowCharacterDialog(conversationID);
    }


}