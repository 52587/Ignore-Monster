using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FearVisualEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private FearMeter fearMeter;
    [SerializeField] private CameraFollow cameraFollow;

    [Header("Vignette Settings")]
    [SerializeField] private float maxVignetteIntensity = 0.5f;
    
    [Header("Chromatic Aberration")]
    [SerializeField] private float maxChromaticAberration = 0.3f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeThreshold = 0.7f;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        if (fearMeter == null)
        {
            fearMeter = FindObjectOfType<FearMeter>();
        }

        if (cameraFollow == null)
        {
            cameraFollow = Camera.main?.GetComponent<CameraFollow>();
        }

        // Get post-processing effects from volume
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out colorAdjustments);
        }
    }

    private void Update()
    {
        if (fearMeter == null) return;

        float fearLevel = fearMeter.GetFearPercentage();
        ApplyVisualEffects(fearLevel);
    }

    private void ApplyVisualEffects(float fearLevel)
    {
        // Vignette effect increases with fear
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(0.2f, maxVignetteIntensity, fearLevel);
        }

        // Chromatic aberration for panic state
        if (chromaticAberration != null)
        {
            if (fearMeter.IsPanicking)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(
                    chromaticAberration.intensity.value,
                    maxChromaticAberration,
                    Time.deltaTime * 2f
                );
            }
            else
            {
                chromaticAberration.intensity.value = Mathf.Lerp(
                    chromaticAberration.intensity.value,
                    0f,
                    Time.deltaTime
                );
            }
        }

        // Desaturate colors at high fear
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = Mathf.Lerp(0f, -30f, fearLevel);
        }

        // Camera shake at high fear
        if (cameraFollow != null && fearLevel > shakeThreshold)
        {
            float shakeAmount = (fearLevel - shakeThreshold) / (1f - shakeThreshold);
            if (Random.value < 0.1f) // Occasional shake
            {
                cameraFollow.Shake(shakeAmount);
            }
        }
    }
}
