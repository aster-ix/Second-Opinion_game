using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] public GameObject confirmationPanel;
    [SerializeField] public TextMeshProUGUI confirmationText;
    [SerializeField] public Button confirmationButton;
    private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;

    void Start()
    {
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    public void ConfirmMainMenu()
    {
        confirmationPanel.SetActive(true);
        confirmationText.text = $"Your progress will be deleted. Exit to Main Menu ?";
        confirmationButton.onClick.AddListener( () => { LoadMainMenu(); } );
    }
    public void ConfirmExit()
    {
        confirmationPanel.SetActive(true);
        confirmationText.text = $"Your progress will be deleted. Exit ?";
        confirmationButton.onClick.AddListener(() => { QuitGame(); });
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);


        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        AudioListener.pause = false;
    }

    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu"); 
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}