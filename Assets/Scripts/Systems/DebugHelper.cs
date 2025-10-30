using UnityEngine;

/// <summary>
/// Debug utility for testing the Don't Look game
/// Press keys to trigger various debug functions
/// REMOVE THIS FROM FINAL BUILD
/// </summary>
public class DebugHelper : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private KeyCode godModeKey = KeyCode.G;
    [SerializeField] private KeyCode refillBatteryKey = KeyCode.B;
    [SerializeField] private KeyCode resetFearKey = KeyCode.N;
    [SerializeField] private KeyCode triggerChaseKey = KeyCode.C;
    [SerializeField] private KeyCode toggleSlowMotionKey = KeyCode.T;

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private FearMeter fearMeter;
    [SerializeField] private FlashlightSystem flashlight;
    [SerializeField] private MonsterAI[] monsters;

    private bool godMode = false;
    private bool slowMotion = false;

    private void Start()
    {
        if (!enableDebug)
        {
            enabled = false;
            return;
        }

        // Find references if not set
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        
        if (fearMeter == null)
            fearMeter = FindObjectOfType<FearMeter>();
        
        if (flashlight == null)
            flashlight = FindObjectOfType<FlashlightSystem>();
        
        if (monsters == null || monsters.Length == 0)
            monsters = FindObjectsOfType<MonsterAI>();

        Debug.Log("=== DEBUG MODE ENABLED ===");
        Debug.Log($"G - Toggle God Mode");
        Debug.Log($"B - Refill Battery");
        Debug.Log($"N - Reset Fear");
        Debug.Log($"C - Force Monster Chase");
        Debug.Log($"T - Toggle Slow Motion");
    }

    private void Update()
    {
        if (!enableDebug) return;

        // God Mode - player can't be caught
        if (Input.GetKeyDown(godModeKey))
        {
            godMode = !godMode;
            Debug.Log($"God Mode: {(godMode ? "ON" : "OFF")}");
        }

        // Refill Battery
        if (Input.GetKeyDown(refillBatteryKey))
        {
            if (flashlight != null)
            {
                flashlight.AddBattery(100f);
                Debug.Log("Battery refilled!");
            }
        }

        // Reset Fear
        if (Input.GetKeyDown(resetFearKey))
        {
            if (fearMeter != null)
            {
                fearMeter.ResetFear();
                Debug.Log("Fear reset!");
            }
        }

        // Force Monster Chase
        if (Input.GetKeyDown(triggerChaseKey))
        {
            foreach (MonsterAI monster in monsters)
            {
                if (monster != null)
                {
                    // Simulate looking at monster for long enough
                    for (int i = 0; i < 200; i++) // Simulate 2 seconds of looking
                    {
                        monster.OnPlayerLookingAt();
                    }
                    Debug.Log($"Forced {monster.name} to chase!");
                }
            }
        }

        // Toggle Slow Motion
        if (Input.GetKeyDown(toggleSlowMotionKey))
        {
            slowMotion = !slowMotion;
            Time.timeScale = slowMotion ? 0.3f : 1f;
            Debug.Log($"Slow Motion: {(slowMotion ? "ON (0.3x)" : "OFF")}");
        }
    }

    private void OnGUI()
    {
        if (!enableDebug) return;

        // Display debug info
        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.Label("=== DEBUG INFO ===");
        
        if (godMode)
            GUILayout.Label("GOD MODE: ON");
        
        if (slowMotion)
            GUILayout.Label("SLOW MOTION: ON");

        if (player != null)
        {
            GUILayout.Label($"Player Position: {player.transform.position}");
        }

        if (fearMeter != null)
        {
            GUILayout.Label($"Fear: {fearMeter.GetCurrentFear():F1}/100");
            GUILayout.Label($"Fear %: {fearMeter.GetFearPercentage() * 100:F0}%");
            GUILayout.Label($"Panicking: {fearMeter.IsPanicking()}");
        }

        if (flashlight != null)
        {
            GUILayout.Label($"Battery: {flashlight.GetBatteryPercentage() * 100:F0}%");
            GUILayout.Label($"Flashlight: {(flashlight.IsOn() ? "ON" : "OFF")}");
        }

        if (monsters != null && monsters.Length > 0)
        {
            GUILayout.Label("--- Monsters ---");
            foreach (MonsterAI monster in monsters)
            {
                if (monster != null && player != null)
                {
                    float distance = Vector2.Distance(player.transform.position, monster.transform.position);
                    GUILayout.Label($"{monster.name}: {monster.currentState} (Dist: {distance:F1})");
                }
            }
        }

        GUILayout.Label("--- Controls ---");
        GUILayout.Label($"[{godModeKey}] God Mode");
        GUILayout.Label($"[{refillBatteryKey}] Refill Battery");
        GUILayout.Label($"[{resetFearKey}] Reset Fear");
        GUILayout.Label($"[{triggerChaseKey}] Force Chase");
        GUILayout.Label($"[{toggleSlowMotionKey}] Slow Motion");

        GUILayout.EndArea();
    }

    public bool IsGodModeActive()
    {
        return godMode && enableDebug;
    }

    private void OnDestroy()
    {
        // Reset time scale on destroy
        Time.timeScale = 1f;
    }
}
