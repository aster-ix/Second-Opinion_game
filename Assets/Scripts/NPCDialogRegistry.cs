using System.Collections.Generic;
using UnityEngine;

// Синглтон-реестр диалоговых менеджеров NPC


public class NPCDialogRegistry : MonoBehaviour
{
    public static NPCDialogRegistry Instance { get; private set; }

    private readonly Dictionary<string, DialogManager> _managers = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(string npcId, DialogManager manager)
    {
        if (string.IsNullOrEmpty(npcId))
        {
            Debug.LogWarning("[NPCDialogRegistry] Попытка зарегистрировать NPC с пустым ID.");
            return;
        }
        _managers[npcId] = manager;
        Debug.Log($"[NPCDialogRegistry] Зарегистрирован NPC: '{npcId}'");
    }

    public void Unregister(string npcId)
    {
        _managers.Remove(npcId);
    }

    public void UnlockNode(string npcId, DialogObject node)
    {
        if (node == null) return;

        if (!_managers.TryGetValue(npcId, out DialogManager manager))
        {
            Debug.LogWarning($"[NPCDialogRegistry] NPC '{npcId}' не найден в реестре. " +
                             $"Убедись что DialogManager зарегистрирован и npcId совпадает.");
            return;
        }

        manager.UnlockNode(node);
        Debug.Log($"[NPCDialogRegistry] Разблокирована нода '{node.name}' у NPC '{npcId}'");
    }

    public bool HasManager(string npcId) => _managers.ContainsKey(npcId);
}