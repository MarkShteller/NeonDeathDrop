using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogConversation
{
   // public string convTag;

    public List<DialogEntry> dialogEntries { get; set; }
    //timeline asset

    public DialogConversation(/*string conversationTag*/)
    {
        //convTag = conversationTag;
        dialogEntries = new List<DialogEntry>();
    }

    public void AddDialogEntry(string tag, string speaker, string message, string flags)
    {
        DialogEntry de = new DialogEntry();
        de.tag = tag;
        de.speaker = speaker;
        de.message = message;
        de.flags = flags;

        dialogEntries.Add(de);
    }

}

public class DialogEntry
{
    public string tag { get; set; }
    public string speaker { get; set; }
    public string message { get; set; }
    public string flags { get; set; }
    //voice line id
    //emote image id
}