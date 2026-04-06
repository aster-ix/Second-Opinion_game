using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class BellClick: MonoBehaviour, IPointerClickHandler
{
    public NPCManager npcManager;

    private bool _isAnimating;
    private float _cooldown = 1f;
    private float _lastClickTime = -999f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isAnimating) return;
        if (Time.time - _lastClickTime < _cooldown) return;

        _lastClickTime = Time.time;
        npcManager.NextNPC();
    }
}