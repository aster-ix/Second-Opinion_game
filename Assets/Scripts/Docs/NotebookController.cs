using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Синглтон. Хранит все записи плоским списком

// API для диалогов:
//   NotebookController.Instance.HasEntry(npcId, "Имя Жертвы")
//   NotebookController.Instance.HasDialogueUnlock(npcId)
//   NotebookController.Instance.GetEntries(npcId)

public class NotebookController : MonoBehaviour
{
    public static NotebookController Instance { get; private set; }


    public RectTransform notebookPanel;
    public Vector2 hiddenPos;          // позиция за экраном (правый край)
    public Vector2 visiblePos;         // позиция когда блокнот открыт


    public RectTransform notifStrip;      // ← новый отдельный объект
    public TMP_Text      newEntryLabel;
    public Vector2       notifHiddenPos;  // за экраном
    public Vector2       notifPeekPos;    // видимая позиция
    public float         slideInTime  = 0.2f;
    public float         holdTime     = 0.5f;
    public float         slideOutTime = 0.2f;


    public TMP_Text entriesText;       // поле где отображаются записи при открытии


    public int maxEntriesPerNpc = 3;

  
    private readonly List<NotebookEntry> _entries = new();


    private readonly Queue<NotebookEntry> _animQueue = new();
    private bool _isNotifPlaying;


    private bool _isOpen;
    private Coroutine _panelCoroutine;


    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        notebookPanel.anchoredPosition = hiddenPos;
        notifStrip.anchoredPosition = notifHiddenPos;
    }


    public bool TryAddEntry(string npcId, SelectableWord word)
    {
        int count = 0;
        foreach (var e in _entries)
            if (e.npcId == npcId) count++;

        if (count >= maxEntriesPerNpc) return false;
        if (_entries.Exists(e => e.npcId == npcId && e.text == word.notebookEntry)) return false;

        var entry = new NotebookEntry(word.notebookEntry, word.type, npcId, word.unlocksDialogue);
        _entries.Add(entry);


        if (!_isOpen)
        {
            _animQueue.Enqueue(entry);
            if (!_isNotifPlaying)
                StartCoroutine(ProcessNotifQueue());
        }
        else
        {
            RefreshEntriesText();
        }

        return true;
    }

    // асе записи конкретного NPC
    public NotebookEntry[] GetEntries(string npcId)
    {
        return _entries.FindAll(e => e.npcId == npcId).ToArray();
    }

    // проверить наличие конкретной записи — для условий диалога
    public bool HasEntry(string npcId, string entryText)
    {
        return _entries.Exists(e => e.npcId == npcId && e.text == entryText);
    }

    // есть ли хоть одна запись, разблокирующая диалог у этого NPC
    public bool HasDialogueUnlock(string npcId)
    {
        return _entries.Exists(e => e.npcId == npcId && e.unlocksDialogue);
    }



    public void ToggleNotebook()
    {
        if (_panelCoroutine != null) StopCoroutine(_panelCoroutine);
        _isOpen = !_isOpen;

        if (_isOpen)
        {
            RefreshEntriesText();
            _panelCoroutine = StartCoroutine(Slide(notebookPanel, notebookPanel.anchoredPosition, visiblePos, slideInTime));
        }
        else
        {
            _panelCoroutine = StartCoroutine(Slide(notebookPanel, notebookPanel.anchoredPosition, hiddenPos, slideOutTime));
        }
    }



    void RefreshEntriesText()
    {
        if (entriesText == null) return;

        var sb = new System.Text.StringBuilder();
        string currentNpc = null;

        foreach (var entry in _entries)
        {
            if (entry.npcId != currentNpc)
            {
                if (currentNpc != null) sb.AppendLine();
                sb.AppendLine($"<b>{entry.npcId}</b>");
                currentNpc = entry.npcId;
            }
            sb.AppendLine($"  — {entry.text}");
        }

        entriesText.text = sb.ToString();
    }


    IEnumerator ProcessNotifQueue()
    {
        _isNotifPlaying = true;

        while (_animQueue.Count > 0)
        {
            var entry = _animQueue.Dequeue();
            yield return PlayNotif(entry);
        }

        _isNotifPlaying = false;
    }

    IEnumerator PlayNotif(NotebookEntry entry)
    {
        newEntryLabel.text = $"— {entry.text}";
        yield return Slide(notifStrip, notifHiddenPos, notifPeekPos, slideInTime);
        yield return new WaitForSeconds(holdTime);
        yield return Slide(notifStrip, notifPeekPos, notifHiddenPos, slideOutTime);
    }

    IEnumerator Slide(RectTransform rt, Vector2 from, Vector2 to, float duration)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
        {
            rt.anchoredPosition = Vector2.Lerp(from, to, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        rt.anchoredPosition = to;
    }
}