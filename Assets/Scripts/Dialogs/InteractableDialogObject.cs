using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableDialogObject : MonoBehaviour
{
    public enum InteractionType { NONE, ConvoProtraits, ConvoSubtitles, TutorialDialog }
    public InteractionType interactionType;
    public string interactionID;
    public bool isPlayerTriggered;
    public bool isTriggeredOnce;
    //public bool isSubtitles;

    private bool isEnabled = true;
    public UnityEvent triggerEnterEvent;
    public UnityEvent triggerExitEvent;
   // public UnityEvent OnInteractionEndedEvent;

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
            }
            triggerExitEvent.Invoke();
        }
    }

    private void OnInteract()
    {
        if (isTriggeredOnce)
            isEnabled = false;
        UIManager.Instance.SetInteractableVisible(false);

        if (interactionID == "" || (interactionType != InteractionType.TutorialDialog && !DialogManager.Instance.IDExists(interactionID)))
        {
            Debug.LogError("Interaction ID " + interactionID + " does not exist or empty.");
            GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            return;
        }

        switch (interactionType)
        {
            case InteractionType.ConvoProtraits:
                DialogManager.Instance.ShowCharacterDialog(interactionID, triggerExitEvent);
                GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
                break;

            case InteractionType.ConvoSubtitles:
                DialogManager.Instance.ShowSubtitlesDialog(interactionID);
                break;

            case InteractionType.TutorialDialog:
                UIManager.Instance.OpenTutorialDialog(int.Parse(interactionID));
                break;
        }
    }
/*
    public void InteractionDoneEvent()
    {
        OnInteractionEndedEvent.Invoke();
    }
*/
}