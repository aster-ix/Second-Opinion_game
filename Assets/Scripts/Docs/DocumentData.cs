using UnityEngine;
using System.Collections.Generic;
// скриптбл объет пкм - NPC - Document
// маркировка кликабельных слов работает в виде: [имя], [дата] и тд
[CreateAssetMenu(fileName="NewDoc", menuName="NPC/Document")]
public class DocumentData : ScriptableObject
{
    public string npcId; // связывает с npc

    public string documentTitle;
    
    [TextArea(5,20)]
    public string rawText;
    
    // список кликабельных слов
    public List<SelectableWord> selectableWords = new List<SelectableWord>();
}

[System.Serializable]
public class SelectableWord
{
    public string word; // точное совпадение с [текстом]
    public string notebookEntry; // что запишется в блокнот
    public WordType type;
    public bool unlocksDialogue;
}

public enum WordType{Name, Date, Fact}
