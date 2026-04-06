using UnityEngine;
using System.Collections.Generic;

// Создать через ПКМ → NPC → Document
// Кликабельные слова в тексте оборачивай в [скобки]: [Имя], [Дата] и т.д.

[CreateAssetMenu(fileName = "NewDoc", menuName = "NPC/Document")]
public class DocumentData : ScriptableObject
{
    public string npcId;          // к какому NPC относится документ
    public string documentTitle;

    [TextArea(5, 20)]
    public string rawText;

    public List<SelectableWord> selectableWords = new List<SelectableWord>();
}

[System.Serializable]
public class SelectableWord
{
    [Tooltip("Точное совпадение с текстом в [скобках]")]
    public string word;

    [Tooltip("Текст который запишется в блокнот")]
    public string notebookEntry;

    public WordType type;

    [Header("Разблокировка диалога")]
    [Tooltip("ID нпс у которого откроется диалог. Можно оставить пустым")]
    public string targetNpcId;

    [Tooltip("Нода диалога которая откроется после сбора улики. Можно оставить пустым")]
    public DialogObject nodeToUnlock;
}

public enum WordType { Name, Date, Fact }