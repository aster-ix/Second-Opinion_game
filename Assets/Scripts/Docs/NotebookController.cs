using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// Синглтон. Хранит записи по каждому NPC 
// при добавлении записи блокнот выезжает, показывает запись, уезжает

public class NotebookController : MonoBehaviour
{
    public static NotebookController Instance { get; private set; }

    public RectTransform notebookPanel;
    public TMP_Text newEntryLabel;      // текст на выезжающем блокноте

    public Vector2 hiddenAnchorPos;    // за экраном (e.g. правый край: 1200, 0)
    public Vector2 visibleAnchorPos;   // видимая позиция (e.g. 800, 0)

    public float slideInTime  = 0.2f;
    public float holdTime     = 0.5f;
    public float slideOutTime = 0.2f;

    public int maxEntriesPerNpc = 3;

    // npcId = список записей
    private readonly Dictionary<string, List<NotebookEntry>> _entries = new();

    // очередь анимаций 
    private readonly Queue<NotebookEntry> _animQueue = new();
    private bool _isPlaying;


    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        notebookPanel.anchoredPosition = hiddenAnchorPos;
    }


    // Пробует добавить запись. возвращает true если успешно
    public bool TryAddEntry(string npcId, SelectableWord word)
    {
        if (!_entries.ContainsKey(npcId))
            _entries[npcId] = new List<NotebookEntry>();

        var list = _entries[npcId];

        //  лимит и дубликаты
        if (list.Count >= maxEntriesPerNpc) return false;
        if (list.Exists(e => e.text == word.notebookEntry)) return false;

        var entry = new NotebookEntry(word.notebookEntry, word.type, npcId);
        list.Add(entry);

        //  очередь анимации
        _animQueue.Enqueue(entry);
        if (!_isPlaying)
            StartCoroutine(ProcessQueue());

        return true;
    }

    // все записи по конкретному NPC 
    public List<NotebookEntry> GetEntries(string npcId)
    {
        return _entries.TryGetValue(npcId, out var list) ? list : new List<NotebookEntry>();
    }


    IEnumerator ProcessQueue()
    {
        _isPlaying = true;

        while (_animQueue.Count > 0)
        {
            var entry = _animQueue.Dequeue();
            yield return PlayAnimation(entry);
        }

        _isPlaying = false;
    }

    IEnumerator PlayAnimation(NotebookEntry entry)
    {
        //  метка по типу
        newEntryLabel.text = entry.type switch
        {
            WordType.Name => $"★ {entry.text}",
            WordType.Date => $"[date] {entry.text}",
            WordType.Fact => $"◆ {entry.text}",
            _             => entry.text
        };

        yield return Slide(hiddenAnchorPos, visibleAnchorPos, slideInTime);
        yield return new WaitForSeconds(holdTime);
        yield return Slide(visibleAnchorPos, hiddenAnchorPos, slideOutTime);
    }

    IEnumerator Slide(Vector2 from, Vector2 to, float duration)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
        {
            notebookPanel.anchoredPosition = Vector2.Lerp(from, to, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        notebookPanel.anchoredPosition = to;
    }
}
