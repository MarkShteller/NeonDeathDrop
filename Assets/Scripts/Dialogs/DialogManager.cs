using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public void ShowCharacterDialog(string conversationID)
    {
        characterDialogBox.gameObject.SetActive(true);
        currentConversation = dialogData[conversationID];
        
        currentEntryIndex = 0;
        DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];

        characterDialogBox.Populate(de.speaker, de.message, "image");

        EnemyManager.Instance.SetUpdateEnemies(false);

        GameManager.Instance.PlayerInstance.submitEvent += PressNextAction;
    }

    public void ShowSubtitlesDialog(string conversationID)
    {
        currentConversation = dialogData[conversationID];
        currentEntryIndex = 0;
        StartCoroutine(ProgressSubtitlesDialog());
    }

    private IEnumerator ProgressSubtitlesDialog()
    {
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

    private void PressNextAction()
    {
        currentEntryIndex++;

        if (currentEntryIndex < currentConversation.dialogEntries.Count)
        {
            DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];
            characterDialogBox.Populate(de.speaker, de.message, "image");
        }
        else
        {
            EnemyManager.Instance.SetUpdateEnemies(true);
            characterDialogBox.gameObject.SetActive(false);
            GameManager.Instance.PlayerInstance.submitEvent -= PressNextAction;
            GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        }
    }

}
