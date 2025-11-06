using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private bool isGameOver = false;
    [SerializeField] private bool hasWon = false;

    [Header("References")]
    [SerializeField] private UIManager uiManager;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        hasWon = false;

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
    }

    private void Update()
    {
        // Restart on R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        // Escape/Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void OnPlayerCaught()
    {
        if (isGameOver) return;

        isGameOver = true;
        hasWon = false;

        Debug.Log("Game Over - Player was caught by monster!");
        
        if (uiManager != null)
        {
            uiManager.ShowGameOver(false);
        }

        Time.timeScale = 0f;
    }

    public void OnPlayerDiedFromFear()
    {
        if (isGameOver) return;

        isGameOver = true;
        hasWon = false;

        Debug.Log("Game Over - Player died from fear!");
        
        if (uiManager != null)
        {
            uiManager.ShowGameOver(false);
        }

        Time.timeScale = 0f;
    }

    public void OnPlayerEscaped()
    {
        if (isGameOver) return;

        isGameOver = true;
        hasWon = true;

        Debug.Log("Victory - Player escaped!");
        
        if (uiManager != null)
        {
            uiManager.ShowGameOver(true);
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TogglePause()
    {
        if (isGameOver) return;

        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            if (uiManager != null) uiManager.HidePauseMenu();
        }
        else
        {
            Time.timeScale = 0f;
            if (uiManager != null) uiManager.ShowPauseMenu();
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool HasWon()
    {
        return hasWon;
    }
}
