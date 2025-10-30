using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class FootstepSounds : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private float volume = 0.3f;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private PlayerController playerController;

    private float stepTimer = 0f;
    private Rigidbody2D rb;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // 2D sound
        }

        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check if player is moving
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (footstepClips != null && footstepClips.Length > 0)
        {
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
