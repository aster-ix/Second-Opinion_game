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

    public float titleCharDelay = 0.07f;
    public float buttonStaggerDelay = 0.4f;

    public float charDelay = 0.045f;
    public float linePause = 1.2f;
    public float finalPause = 1.8f;

    public string mainSceneName = "MainScene";

    private bool isRunning = false;

    private readonly List<string> phrases = new List<string>
    {
        "A body was found at 3:47 AM in the flat on the 5th floor.",
        "You have been assigned to investigate the case.",
        "Find who the killer is, and bring them to justice.",
        "Good luck, detective."
    };

    void Awake()
    {
        titleText.maxVisibleCharacters = 0;

        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        typewriterText.gameObject.SetActive(false);
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

        // тайтл посимвольно
        int total = titleText.text.Length;
        for (int i = 0; i <= total; i++)
        {
            titleText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(titleCharDelay);
        }

        yield return new WaitForSeconds(0.2f);

        // кнопки по очереди без анимации
        startButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(buttonStaggerDelay);
        exitButton.gameObject.SetActive(true);
    }

    void OnStartClicked()
    {
        if (isRunning) return;
        isRunning = true;

        startButton.interactable = false;
        exitButton.interactable = false;

        StartCoroutine(TypewriterSequence());
    }

    void OnExitClicked()
    {
        StartCoroutine(QuitSequence());
    }

    IEnumerator TypewriterSequence()
    {
        // всё меню скрываем мгновенно
        titleText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        typewriterText.gameObject.SetActive(true);

        for (int i = 0; i < phrases.Count; i++)
        {
            yield return StartCoroutine(TypeLine(phrases[i]));
            yield return new WaitForSeconds(linePause);

            if (i < phrases.Count - 1)
            {
                typewriterText.text = "";
            }
        }

        yield return new WaitForSeconds(finalPause);

        typewriterText.gameObject.SetActive(false);

        if (backgroundImage != null)
            yield return StartCoroutine(FadeImageToBlack(backgroundImage, 0.5f));

        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(mainSceneName);
    }

    IEnumerator TypeLine(string line)
    {
        typewriterText.text = "";
        foreach (char ch in line)
        {
            typewriterText.text += ch;
            yield return new WaitForSeconds(charDelay);
        }
    }

    IEnumerator QuitSequence()
    {
        startButton.interactable = false;
        exitButton.interactable = false;

        titleText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
}