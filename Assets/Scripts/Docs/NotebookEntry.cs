using UnityEngine;

// моделька для одной записи в блокноте
public class NotebookEntry 
{
    public string text;
    public WordType type;
    public string npcId;
    public bool unlocksDialogue;

    public NotebookEntry(string text, WordType type, string npcId, bool unlocksDialogue = false)
    {
        this.text = text;
        this.type = type;
        this.npcId = npcId;
        this.unlocksDialogue = unlocksDialogue;
    }
}
