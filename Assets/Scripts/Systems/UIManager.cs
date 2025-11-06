using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private Image fearMeterFill;
    [SerializeField] private Image batteryMeterFill;
    [SerializeField] private TextMeshProUGUI fearText;
    [SerializeField] private TextMeshProUGUI batteryText;
    [SerializeField] private GameObject lowBatteryWarning;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Instructions")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private float instructionsDisplayTime = 5f;

    [Header("References")]
    [SerializeField] private FearMeter fearMeter;
    [SerializeField] private FlashlightSystem flashlightSystem;

    [Header("Colors")]
    [SerializeField] private Color normalFearColor = Color.green;
    [SerializeField] private Color highFearColor = Color.yellow;
    [SerializeField] private Color panicFearColor = Color.red;

    private void Start()
    {
        // Find references if not set
        if (fearMeter == null)
        {
            fearMeter = FindObjectOfType<FearMeter>();
        }

        if (flashlightSystem == null)
        {
            flashlightSystem = FindObjectOfType<FlashlightSystem>();
        }

        // Hide panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (lowBatteryWarning != null) lowBatteryWarning.SetActive(false);

        // Show instructions briefly
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
            Invoke(nameof(HideInstructions), instructionsDisplayTime);
        }
    }

    private void Update()
    {
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        // Update Fear Meter
        if (fearMeter != null)
        {
            float fearPercent = fearMeter.GetFearPercentage();
            float fearNormalized = fearPercent / 100f; // Convert 0-100 to 0-1
            
            if (fearMeterFill != null)
            {
                fearMeterFill.fillAmount = fearNormalized;

                // Change color based on fear level
                if (fearNormalized < 0.5f)
                {
                    fearMeterFill.color = Color.Lerp(normalFearColor, highFearColor, fearNormalized * 2f);
                }
                else
                {
                    fearMeterFill.color = Color.Lerp(highFearColor, panicFearColor, (fearNormalized - 0.5f) * 2f);
                }
            }

            if (fearText != null)
            {
                fearText.text = $"Fear: {Mathf.RoundToInt(fearPercent)}%";
            }
        }

        // Update Battery Meter
        if (flashlightSystem != null)
        {
            float batteryPercent = flashlightSystem.GetBatteryPercentage();
            
            if (batteryMeterFill != null)
            {
                batteryMeterFill.fillAmount = batteryPercent;
            }

            if (batteryText != null)
            {
                batteryText.text = $"Battery: {Mathf.RoundToInt(batteryPercent * 100)}%";
            }

            // Show low battery warning
            if (lowBatteryWarning != null)
            {
                lowBatteryWarning.SetActive(flashlightSystem.IsLowBattery());
            }
        }
    }

    private void HideInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }
    }

    public void ShowGameOver(bool won)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverText != null)
            {
                if (won)
                {
                    gameOverText.text = "YOU ESCAPED!\n\nPress R to Restart";
                }
                else
                {
                    gameOverText.text = "YOU WERE CAUGHT\n\nPress R to Restart";
                }
            }
        }
    }

    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        
        // Also show instructions when paused
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
        }
    }

    public void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Hide instructions when unpausing
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }
    }

    public void OnResumeButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
    }

    public void OnRestartButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}
