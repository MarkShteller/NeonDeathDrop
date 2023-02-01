using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public Dictionary<string, DialogConversation> dialogData;
    public string dialogDataAssetPath;
    public CharacterDialogBox characterDialogBox;
    public DialogSubtitles dialogSubtitles;

    private DialogConversation currentConversation;
    private int currentEntryIndex;
    private UnityEvent exitEvent;

    void Awake()
    {
        print("## initing dialog manager");
        if (Instance == null)
            Instance = this;

        dialogData = new Dictionary<string, DialogConversation>();
        //ParseCSV(dialogDataAssetPath);
        ParseTSV(dialogDataAssetPath);
    }

    private void ParseCSV(string asset)
    {
        print("## parsing CSV in dialog manager");
        TextAsset fileData = Resources.Load<TextAsset>(asset);
        var lines = fileData.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var lineData = lines[i].Split(",");
            string conversationID = lineData[0];

            DialogConversation dc = new DialogConversation();
            dc.AddDialogEntry(lineData[1], lineData[2], lineData[3], lineData[4]);

            if (i + 1 < lines.Length) // prevent EOF overflow
            { 
                var nextLine = lines[i + 1].Split(",");
                while (conversationID == nextLine[0])
                {
                    dc.AddDialogEntry(nextLine[1], nextLine[2], nextLine[3], nextLine[4]);
                    i++;
                    nextLine = lines[i + 1].Split(",");
                } 
            }

           // Debug.Log("Adding conversation ID: " + conversationID);
            dialogData.Add(conversationID, dc);
        }

        Debug.Log("## Parsed num conversations: "+dialogData.Count);
    }

    internal bool IDExists(string conversationID)
    {
        return dialogData.ContainsKey(conversationID);
    }

    private void ParseTSV(string asset)
    {
        print("## parsing CSV in dialog manager");
        TextAsset fileData = Resources.Load<TextAsset>(asset);
        var lines = fileData.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var lineData = lines[i].Split('\t');
            string conversationID = lineData[0];

            DialogConversation dc = new DialogConversation();
            dc.AddDialogEntry(lineData[1], lineData[2], lineData[3], lineData[4]);

            if (i + 1 < lines.Length) // prevent EOF overflow
            {
                var nextLine = lines[i + 1].Split('\t');
                while (conversationID == nextLine[0])
                {
                    dc.AddDialogEntry(nextLine[1], nextLine[2], nextLine[3], nextLine[4]);
                    i++;
                    if (i + 1 == lines.Length)
                        break;
                    nextLine = lines[i + 1].Split('\t');
                }
            }

            // Debug.Log("Adding conversation ID: " + conversationID);
            dialogData.Add(conversationID, dc);
        }

        Debug.Log("## Parsed num conversations: " + dialogData.Count);
    }

    private void PressNextAction()
    {
        currentEntryIndex++;

        if (currentEntryIndex < currentConversation.dialogEntries.Count)
        {
            DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];
            characterDialogBox.Populate(de.speaker, de.message, de.tag);
        }
        else
        {
            if (exitEvent != null)
            {
                exitEvent.Invoke();
                exitEvent.RemoveAllListeners();
            }
            EnemyManager.Instance.SetUpdateEnemies(true);
            characterDialogBox.gameObject.SetActive(false);
            GameManager.Instance.PlayerInstance.submitEvent -= PressNextAction;
            GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            UIManager.Instance.SetHUDVisible(true);
        }
    }

    public void ShowCharacterDialog(string conversationID, UnityEvent exitEvent = null)
    {
        this.exitEvent = exitEvent;

        dialogSubtitles.gameObject.SetActive(false);
        StopCoroutine(ProgressSubtitlesDialog());

        currentConversation = dialogData[conversationID];
        
        currentEntryIndex = 0;
        DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];

        characterDialogBox.gameObject.SetActive(true);
        characterDialogBox.Populate(de.speaker, de.message, de.tag);

        EnemyManager.Instance.SetUpdateEnemies(false);
        UIManager.Instance.SetHUDVisible(false);

        GameManager.Instance.PlayerInstance.submitEvent += PressNextAction;
    }

    public void ShowSubtitlesDialog(string conversationID)
    {
        currentConversation = dialogData[conversationID];
        StartCoroutine(ProgressSubtitlesDialog());
    }

    private IEnumerator ProgressSubtitlesDialog()
    {
        currentEntryIndex = 0;
        dialogSubtitles.gameObject.SetActive(true);
        while (currentEntryIndex < currentConversation.dialogEntries.Count)
        {
            DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];
            dialogSubtitles.Populate(de.speaker, de.message);
            currentEntryIndex++;

            yield return new WaitForSeconds(2);
        }
        dialogSubtitles.gameObject.SetActive(false);
    }


}
