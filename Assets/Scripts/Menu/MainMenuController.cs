using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Button startButton;
    public Button exitButton;
    public TextMeshProUGUI typewriterText;
    public Image backgroundImage;

    public float titleFadeDuration = 1.8f;
    public float buttonFadeDuration = 0.9f;
    public float buttonStaggerDelay = 0.25f;

    public float charDelay = 0.045f;
    public float linePause = 1.2f;
    public float finalPause = 1.8f;

    public string mainSceneName = "MainScene";

    private CanvasGroup titleG;
    private CanvasGroup startG;
    private CanvasGroup exitG;
    private CanvasGroup typewriterG;

    private bool isRunning = false;

    private readonly List<string> phrases = new List<string>
    {
        "A body was found at 3:47 AM in the flat on the 5th floor.",
        "You have been assigned to investigate the case and find out who the killer is.",
        "Find who the killer is, and bring them to justice.",
        "Good luck, detective."
    };

    void Awake()
    {
        titleG      = GetOrAddCG(titleText.gameObject);
        startG      = GetOrAddCG(startButton.gameObject);
        exitG       = GetOrAddCG(exitButton.gameObject);
        typewriterG = GetOrAddCG(typewriterText.gameObject);

        titleG.alpha      = 0f;
        startG.alpha      = 0f;
        exitG.alpha       = 0f;
        typewriterG.alpha = 0f;

        SetInteractable(startG, false);
        SetInteractable(exitG, false);

        typewriterText.text = "";
    }

    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        StartCoroutine(MenuIntro());
    }

    IEnumerator MenuIntro()
    {
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(Fade(titleG, 0f, 1f, titleFadeDuration));

        yield return new WaitForSeconds(0.15f);

        StartCoroutine(Fade(startG, 0f, 1f, buttonFadeDuration));
        yield return new WaitForSeconds(buttonStaggerDelay);
        yield return StartCoroutine(Fade(exitG, 0f, 1f, buttonFadeDuration));

        SetInteractable(startG, true);
        SetInteractable(exitG, true);
    }

    void OnStartClicked()
    {
        if (isRunning) return;
        isRunning = true;

        SetInteractable(startG, false);
        SetInteractable(exitG, false);

        StartCoroutine(TypewriterSequence());
    }

    void OnExitClicked()
    {
        StartCoroutine(QuitSequence());
    }

    IEnumerator TypewriterSequence()
    {
        float fadeOut = 0.5f;
        StartCoroutine(Fade(titleG, 1f, 0f, fadeOut));
        StartCoroutine(Fade(startG, 1f, 0f, fadeOut));
        yield return StartCoroutine(Fade(exitG, 1f, 0f, fadeOut));

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Fade(typewriterG, 0f, 1f, 0.4f));

        for (int i = 0; i < phrases.Count; i++)
        {
            yield return StartCoroutine(TypeLine(phrases[i]));
            yield return new WaitForSeconds(linePause);

            if (i < phrases.Count - 1)
                yield return StartCoroutine(FadeTMPAlpha(typewriterText, 1f, 0f, 0.35f));
        }

        yield return new WaitForSeconds(finalPause);
        yield return StartCoroutine(FadeTMPAlpha(typewriterText, 1f, 0f, 0.6f));

        if (backgroundImage != null)
            yield return StartCoroutine(FadeImageToBlack(backgroundImage, 0.5f));

        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(mainSceneName);
    }

    IEnumerator TypeLine(string line)
    {
        UnityEngine.Color c = typewriterText.color;
        c.a = 1f;
        typewriterText.color = c;
        typewriterText.text = "";

        foreach (char ch in line)
        {
            typewriterText.text += ch;
            yield return new WaitForSeconds(charDelay);
        }
    }

    IEnumerator QuitSequence()
    {
        SetInteractable(startG, false);
        SetInteractable(exitG, false);

        float d = 0.4f;
        StartCoroutine(Fade(titleG, 1f, 0f, d));
        StartCoroutine(Fade(startG, 1f, 0f, d));
        yield return StartCoroutine(Fade(exitG, 1f, 0f, d));

        yield return new WaitForSeconds(0.1f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        cg.alpha = from;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator FadeTMPAlpha(TextMeshProUGUI tmp, float from, float to, float duration)
    {
        float t = 0f;
        UnityEngine.Color c = tmp.color;
        c.a = from;
        tmp.color = c;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / duration));
            tmp.color = c;
            yield return null;
        }
        c.a = to;
        tmp.color = c;
    }

    IEnumerator FadeImageToBlack(Image img, float duration)
    {
        float t = 0f;
        UnityEngine.Color start = img.color;
        UnityEngine.Color target;
        ColorUtility.TryParseHtmlString("#2e224f", out target);

        while (t < duration)
        {
            t += Time.deltaTime;
            img.color = UnityEngine.Color.Lerp(start, target, t / duration);
            yield return null;
        }
        img.color = target;
    }

    void SetInteractable(CanvasGroup cg, bool value)
    {
        cg.interactable   = value;
        cg.blocksRaycasts = value;
    }

    CanvasGroup GetOrAddCG(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        return cg != null ? cg : obj.AddComponent<CanvasGroup>();
    }
}