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
    public bool isAutoScroll = false;

    private DialogConversation currentConversation;
    private int currentEntryIndex;
    private UnityEvent exitEvent;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        dialogData = new Dictionary<string, DialogConversation>();
    }

    public void InitLevelDialogManager(string dialogSheetPath)
    {
        if (dialogSheetPath == "")
        {
            print("## dialog sheet path empty");
            return;
        }

        print("## initing dialog manager with sheet: "+dialogSheetPath);
        if (dialogData.Count > 0)
            dialogData.Clear();

        dialogDataAssetPath = dialogSheetPath;
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

            DialogConversation dc = new DialogConversation(conversationID);
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
        print("## parsing TSV in dialog manager");
        TextAsset fileData = Resources.Load<TextAsset>(asset);
        var lines = fileData.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var lineData = lines[i].Split('\t');
            string conversationID = lineData[0];

            DialogConversation dc = new DialogConversation(conversationID);
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
        AudioManager.Instance.StopCurrentVoiceline();

        if (currentEntryIndex < currentConversation.dialogEntries.Count)
        {
            DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];
            characterDialogBox.Populate(de.speaker, de.message, de.tag, UIManager.Instance.iconManager.currentIcons);

            CreateVoicelineTagAndPlay(de, currentEntryIndex);
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
            
            if (!GameManager.Instance.isTutorial)
                GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            else
                GameManager.Instance.PlayerInstance.GetComponent<PlayerInput>().SwitchCurrentActionMap("PlayerTutorial"); 
            
            UIManager.Instance.SetHUDVisible(true);
            GameManager.Instance.SetDuckMusicIntensity(0f);
        }
    }

    public void ShowCharacterDialog(string conversationID, UnityEvent exitEvent = null)
    {
        this.exitEvent = exitEvent;

        StopSubtitles();

        currentConversation = dialogData[conversationID];

        currentEntryIndex = 0;
        DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];

        characterDialogBox.gameObject.SetActive(true);
        characterDialogBox.Populate(de.speaker, de.message, de.tag, UIManager.Instance.iconManager.currentIcons);
        
        CreateVoicelineTagAndPlay(de, currentEntryIndex);

        EnemyManager.Instance.SetUpdateEnemies(false);
        UIManager.Instance.SetHUDVisible(false);
        GameManager.Instance.SetDuckMusicIntensity(1f);


        if (!isAutoScroll)
            GameManager.Instance.PlayerInstance.submitEvent += PressNextAction;
    }

    /*private void ProgressCharacterDialog()
    {
        //dialogSubtitles.gameObject.SetActive(true);
        if (currentEntryIndex < currentConversation.dialogEntries.Count)
        {
            DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];
            dialogSubtitles.Populate(de.speaker, de.message);

            CreateVoicelineTagAndPlay(de, currentEntryIndex, ProgressCharacterDialog);
            currentEntryIndex++;

        }
        else
            dialogSubtitles.gameObject.SetActive(false);
    }*/

    private void CreateVoicelineTagAndPlay(DialogEntry de, int index, Action callback = null)
    {
        string speakerName = de.speaker.Equals("AL-X") ? "Alex" : de.speaker;
        string s = string.Format("Level1/{0}/level1.{1}.{2}.{3}", speakerName, currentConversation.conversationID, speakerName.ToLower(), index + 1);

        if (de.flags != "")
            s += "." + de.flags;

        AudioManager.Instance.PlayVoiceline(s, callback);
    }

    public void ShowSubtitlesDialog(string conversationID, int startIndex =0)
    {
        currentConversation = dialogData[conversationID];
        
        currentEntryIndex = startIndex;
        dialogSubtitles.gameObject.SetActive(true);

        DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];
        dialogSubtitles.Populate(de.speaker, de.message);

        CreateVoicelineTagAndPlay(de, currentEntryIndex, ProgressSubtitlesDialog);
        currentEntryIndex++;
    }

    private void ProgressSubtitlesDialog()
    {
        //dialogSubtitles.gameObject.SetActive(true);
        if (currentEntryIndex < currentConversation.dialogEntries.Count)
        {
            DialogEntry de = currentConversation.dialogEntries[currentEntryIndex];
            dialogSubtitles.Populate(de.speaker, de.message);

            CreateVoicelineTagAndPlay(de, currentEntryIndex, ProgressSubtitlesDialog);
            currentEntryIndex++;
           
        }
        else
            dialogSubtitles.gameObject.SetActive(false);
    }

    

    private void StopSubtitles()
    {
        AudioManager.Instance.StopCurrentVoiceline();
        dialogSubtitles.gameObject.SetActive(false);
    }

}
