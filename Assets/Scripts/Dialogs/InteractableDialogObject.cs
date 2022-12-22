using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableDialogObject : MonoBehaviour
{
    public string conversationID;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //show interact button
            UIManager.Instance.SetInteractableVisible(true);
            //attach interact action
            GameManager.Instance.PlayerInstance.interactionEvent += OnInteract;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.SetInteractableVisible(false);
            //detach interact action
            GameManager.Instance.PlayerInstance.interactionEvent -= OnInteract;

        }
    }

    private void OnInteract()
    { 
        UIManager.Instance.SetInteractableVisible(false);
        DialogManager.Instance.ShowCharacterDialog(conversationID);

    }
}