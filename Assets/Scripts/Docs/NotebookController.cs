using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Синглтон. Хранит все собранные улики
//
// API для диалогов:
//   NotebookController.Instance.HasEntry(npcId, "Текст записи")
//   NotebookController.Instance.GetEntries(npcId)

public class NotebookController : MonoBehaviour
{
    public static NotebookController Instance { get; private set; }

    public RectTransform notebookPanel;
    public Vector2 hiddenPos;
    public Vector2 visiblePos;

    public RectTransform notifStrip;
    public TMP_Text      newEntryLabel;
    public Vector2       notifHiddenPos;
    public Vector2       notifPeekPos;
    public float         slideInTime  = 0.2f;
    public float         holdTime     = 0.5f;
    public float         slideOutTime = 0.2f;

    public TMP_Text entriesText;

    public int maxEntriesPerNpc = 3;


    private readonly List<NotebookEntry> _entries  = new();
    private readonly Queue<NotebookEntry> _animQueue = new();

    private bool _isNotifPlaying;
    private bool _isOpen;
    private Coroutine _panelCoroutine;


    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        notebookPanel.anchoredPosition = hiddenPos;
        notifStrip.anchoredPosition    = notifHiddenPos;
    }


    // Добавить улику в блокнот
    // Возвращает true если запись добавлена, false если блокнот полон или запись уже есть.
    public bool TryAddEntry(string npcId, SelectableWord word)
    {
        // лимит записей для этого NPC
        int count = 0;
        foreach (var e in _entries)
            if (e.npcId == npcId) count++;

        if (count >= maxEntriesPerNpc) return false;

        // check дубликата
        if (_entries.Exists(e => e.npcId == npcId && e.text == word.notebookEntry)) return false;

        // Добавляем запись
        var entry = new NotebookEntry(word.notebookEntry, word.type, npcId);
        _entries.Add(entry);

        // рзблокируем диалог если задан
        if (word.nodeToUnlock != null && !string.IsNullOrEmpty(word.targetNpcId))
        {
            if (NPCDialogRegistry.Instance != null)
                NPCDialogRegistry.Instance.UnlockNode(word.targetNpcId, word.nodeToUnlock);
            else
                Debug.LogWarning("[NotebookController] NPCDialogRegistry не найден на сцене.");
        }

        
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

    public NotebookEntry[] GetEntries(string npcId)
    {
        return _entries.FindAll(e => e.npcId == npcId).ToArray();
    }

    public bool HasEntry(string npcId, string entryText)
    {
        return _entries.Exists(e => e.npcId == npcId && e.text == entryText);
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
            yield return PlayNotif(_animQueue.Dequeue());
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