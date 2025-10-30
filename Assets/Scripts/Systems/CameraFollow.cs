using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Camera Shake")]
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private float shakeFrequency = 1f;
    private float shakeIntensity = 0f;

    private Vector3 originalPosition;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        originalPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Apply camera shake if active
        if (shakeIntensity > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude * shakeIntensity;
            shakeOffset.z = 0; // Keep shake in 2D plane
            transform.position += shakeOffset;

            // Decay shake
            shakeIntensity = Mathf.Max(0, shakeIntensity - Time.deltaTime * shakeFrequency);
        }
    }

    public void Shake(float intensity = 1f)
    {
        shakeIntensity = Mathf.Clamp01(intensity);
    }
}
