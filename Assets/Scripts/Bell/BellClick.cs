using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class BellClick : MonoBehaviour, IPointerClickHandler
{
    public NPCManager npcManager;

    public Animator animator;
    public TMP_Text hintText;
    public float charDelay = 0.05f;

    private bool _isAnimating;
    private float _cooldown = 1f;
    private float _lastClickTime = -999f;
    private Coroutine _hintCoroutine;

    void Start()
    {
        if (hintText != null) hintText.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isAnimating) return;
        if (Time.time - _lastClickTime < _cooldown) return;
        _lastClickTime = Time.time;

        animator.Play("Bell_ringing", 0, 0.0f);

        if (npcManager.CurrentQuestionNum == npcManager.QuestionNum)
        {
            if (hintText != null)
            {
                if (_hintCoroutine != null) StopCoroutine(_hintCoroutine);
                _hintCoroutine = StartCoroutine(ShowHint());
            }
            return;
        }

        npcManager.NextNPC();
    }

    IEnumerator ShowHint()
    {
        string message = "I'm tired, maybe I should go to bed";
        hintText.gameObject.SetActive(true);
        hintText.text = "";

        foreach (char ch in message)
        {
            hintText.text += ch;
            yield return new WaitForSeconds(charDelay);
        }

        yield return new WaitForSeconds(3f);

        hintText.gameObject.SetActive(false);
    }
}