using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DocumentReaderUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text bodyText;

    public string availableColor = "#ffedab";
    public string usedColor = "#393640";

    private DocumentData _currentDoc;
    
    private List<SelectableWord> _pendingWords = new List<SelectableWord>();

    public void Open(DocumentData doc)
    {
        _currentDoc = doc;
        _pendingWords = new List<SelectableWord>(doc.selectableWords);
        
        panel.SetActive(true);
        Refresh();
        
    }
    public void Close()
    {panel.SetActive(false);}

    void Update()
    {
        if (!panel.activeSelf) return;
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        int linkIdx = TMP_TextUtilities.FindIntersectingLink(
            bodyText, Mouse.current.position.ReadValue(), null);

        
        if (linkIdx >= 0)
            TryCollect(bodyText.textInfo.linkInfo[linkIdx].GetLinkID());
    }

    void TryCollect(string word)
    {
        SelectableWord entry = _pendingWords.Find(x => x.word == word);
        if (entry == null) return;
        
        bool accepted = NotebookController.Instance.TryAddEntry(_currentDoc.npcId, entry);
        if (accepted)
        {
            _pendingWords.Remove(entry);
            Refresh();
            
        }
        else
        {
            Debug.Log($"Блокнот НПС '{_currentDoc.npcId}' заполнен. Макс {NotebookController.Instance.maxEntriesPerNpc}");
            
        }
    }

    void Refresh()
    {
        titleText.text = _currentDoc.documentTitle;
        bodyText.text = BuildRichText();
    }

    string BuildRichText()
    {
        var pending = new HashSet<string>();
        foreach (var w in _pendingWords) pending.Add(w.word);
        
        return Regex.Replace(_currentDoc.rawText, @"\[(.+?)\]", match =>
        {
            string word = match.Groups[1].Value;

            if (pending.Contains(word))
                // кликабельное  слово
                return $"<link=\"{word}\"><color={availableColor}><u>{word}</u></color></link>";
            else
                // уже записано серое, не кликабельное
                return $"<color={usedColor}>{word}</color>";
        });
    }
}

