using System.Collections;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
   
    public float charsPerSecond = 40f;

  
    public float endSentencePauseMultiplier = 8f;

   
    public float commaPauseMultiplier = 3f;


    public bool playOnStart = true;

   
    public bool skipOnInput = true;

    public UnityEngine.Events.UnityEvent onTypingComplete;

    
    private TMP_Text _label;
    private string _fullText;
    private Coroutine _typingRoutine;
    private bool _isTyping;

    private void Awake()
    {
        _label = GetComponent<TMP_Text>();

        _fullText = _label.text;

        _label.text = string.Empty;
    }

    private void Start()
    {
        if (playOnStart)
            StartTyping();
    }

    private void Update()
    {
        if (_isTyping && skipOnInput)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                Skip();
        }
    }


    public void StartTyping()
    {
        if (_typingRoutine != null)
            StopCoroutine(_typingRoutine);

        _label.text = string.Empty;
        _typingRoutine = StartCoroutine(TypeRoutine());
    }

    public void StartTyping(string customText)
    {
        _fullText = customText;
        StartTyping();
    }

    public void Skip()
    {
        if (!_isTyping) return;

        StopCoroutine(_typingRoutine);
        _label.text = _fullText;
        _isTyping = false;
        onTypingComplete?.Invoke();
    }

    public void Clear()
    {
        if (_typingRoutine != null)
            StopCoroutine(_typingRoutine);

        _label.text = string.Empty;
        _isTyping = false;
    }

    public bool IsTyping => _isTyping;

    
    private IEnumerator TypeRoutine()
    {
        _isTyping = true;
        float baseDelay = 1f / Mathf.Max(charsPerSecond, 0.01f);

        for (int i = 0; i < _fullText.Length; i++)
        {
            _label.text = _fullText.Substring(0, i + 1);
            yield return new WaitForSeconds(baseDelay * GetPauseMultiplier(_fullText[i]));
        }

        _isTyping = false;
        onTypingComplete?.Invoke();
    }

    private float GetPauseMultiplier(char c)
    {
        switch (c)
        {
            case '.': case '!': case '?': case '…':
                return endSentencePauseMultiplier;
            case ',': case ';': case ':':
                return commaPauseMultiplier;
            default:
                return 1f;
        }
    }
}