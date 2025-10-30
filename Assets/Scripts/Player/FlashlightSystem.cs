using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Battery Settings")]
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float currentBattery = 100f;
    [SerializeField] private float batteryDrainRate = 5f; // Per second when on
    [SerializeField] private float lowBatteryThreshold = 20f;

    [Header("Light Settings")]
    [SerializeField] private Light2D flashlight;
    [SerializeField] private float normalIntensity = 1f;
    [SerializeField] private float flickerIntensity = 0.3f;
    [SerializeField] private float flickerSpeed = 0.1f;

    [Header("Controls")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F;
    
    private bool isOn = true;
    private bool isFlickering = false;
    private Coroutine flickerCoroutine;

    private void Start()
    {
        if (flashlight == null)
        {
            flashlight = GetComponentInChildren<Light2D>();
        }

        if (flashlight != null)
        {
            flashlight.enabled = isOn;
            flashlight.intensity = normalIntensity;
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateBattery();
        UpdateFlicker();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }
    }

    private void UpdateBattery()
    {
        if (isOn && currentBattery > 0)
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;
            currentBattery = Mathf.Max(currentBattery, 0f);

            // Auto-turn off when battery dies
            if (currentBattery <= 0)
            {
                isOn = false;
                if (flashlight != null)
                {
                    flashlight.enabled = false;
                }
            }
        }
    }

    private void UpdateFlicker()
    {
        bool shouldFlicker = isOn && currentBattery <= lowBatteryThreshold && currentBattery > 0;

        if (shouldFlicker && !isFlickering)
        {
            isFlickering = true;
            if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
            flickerCoroutine = StartCoroutine(FlickerEffect());
        }
        else if (!shouldFlicker && isFlickering)
        {
            isFlickering = false;
            if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
            
            if (flashlight != null && isOn)
            {
                flashlight.intensity = normalIntensity;
            }
        }
    }

    private IEnumerator FlickerEffect()
    {
        while (isFlickering)
        {
            if (flashlight != null)
            {
                // Random flicker
                flashlight.intensity = Random.Range(flickerIntensity, normalIntensity);
                
                // Occasionally turn off briefly
                if (Random.value < 0.1f)
                {
                    flashlight.enabled = false;
                    yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
                    flashlight.enabled = true;
                }
            }

            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    public void ToggleFlashlight()
    {
        if (currentBattery > 0)
        {
            isOn = !isOn;
            
            if (flashlight != null)
            {
                flashlight.enabled = isOn;
                if (isOn)
                {
                    flashlight.intensity = normalIntensity;
                }
            }
        }
    }

    public void AddBattery(float amount)
    {
        currentBattery += amount;
        currentBattery = Mathf.Min(currentBattery, maxBattery);
    }

    public float GetBatteryPercentage()
    {
        return currentBattery / maxBattery;
    }

    public bool IsOn()
    {
        return isOn;
    }

    public bool IsLowBattery()
    {
        return currentBattery <= lowBatteryThreshold;
    }
}
