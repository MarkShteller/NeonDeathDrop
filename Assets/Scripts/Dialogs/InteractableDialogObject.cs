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
    public bool isSubtitles;

    private bool isEnabled = true;
    public UnityEvent triggerEnterEvent;
    public UnityEvent triggerExitEvent;

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
                    OnInteract();
                    triggerEnterEvent.Invoke();
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
                triggerExitEvent.Invoke();
            }
        }
    }

    private void OnInteract()
    {
        if (isTriggeredOnce)
            isEnabled = false;
        UIManager.Instance.SetInteractableVisible(false);

        if (!conversationID.Equals("") && DialogManager.Instance.IDExists(conversationID))
        {
            if (!isSubtitles)
            {
                DialogManager.Instance.ShowCharacterDialog(conversationID, triggerExitEvent);
                GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
            }
            else
            {
                DialogManager.Instance.ShowSubtitlesDialog(conversationID);
            }
        }
        else
        {
            Debug.LogError("Conversation ID "+ conversationID +" does not exist or empty.");
            GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        }
    }


}