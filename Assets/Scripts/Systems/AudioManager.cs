using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource heartbeatSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip ambientDrone;
    [SerializeField] private AudioClip heartbeatClip;
    [SerializeField] private AudioClip breathingClip;
    [SerializeField] private AudioClip staticClip;
    [SerializeField] private AudioClip monsterChaseMusic;
    [SerializeField] private AudioClip footstepClip;

    [Header("Settings")]
    [SerializeField] private float monsterProximityThreshold = 5f;
    [SerializeField] private float maxHeartbeatVolume = 0.7f;
    [SerializeField] private float maxStaticVolume = 0.3f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private FearMeter fearMeter;

    private MonsterAI[] monsters;
    private float nearestMonsterDistance = float.MaxValue;
    private bool isPlayingChaseMusic = false;

    private void Start()
    {
        // Setup audio sources
        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        if (heartbeatSource == null)
        {
            heartbeatSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure sources
        ambientSource.loop = true;
        ambientSource.volume = 0.3f;
        
        heartbeatSource.loop = true;
        heartbeatSource.volume = 0f;

        sfxSource.loop = false;

        // Find references
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (fearMeter == null)
        {
            fearMeter = player?.GetComponent<FearMeter>();
        }

        monsters = FindObjectsOfType<MonsterAI>();

        // Play ambient sound
        if (ambientDrone != null)
        {
            ambientSource.clip = ambientDrone;
            ambientSource.Play();
        }

        // Setup heartbeat
        if (heartbeatClip != null)
        {
            heartbeatSource.clip = heartbeatClip;
            heartbeatSource.Play();
        }
    }

    private void Update()
    {
        UpdateMonsterProximity();
        UpdateHeartbeat();
        UpdateChaseMusic();
    }

    private void UpdateMonsterProximity()
    {
        if (player == null) return;

        nearestMonsterDistance = float.MaxValue;

        foreach (MonsterAI monster in monsters)
        {
            if (monster != null)
            {
                float distance = Vector2.Distance(player.position, monster.transform.position);
                if (distance < nearestMonsterDistance)
                {
                    nearestMonsterDistance = distance;
                }
            }
        }
    }

    private void UpdateHeartbeat()
    {
        if (heartbeatSource == null || fearMeter == null) return;

        // Heartbeat volume increases with fear
        float fearLevel = fearMeter.GetFearPercentage();
        float targetVolume = Mathf.Lerp(0f, maxHeartbeatVolume, fearLevel);

        // Also increase with monster proximity
        if (nearestMonsterDistance < monsterProximityThreshold)
        {
            float proximityFactor = 1f - (nearestMonsterDistance / monsterProximityThreshold);
            targetVolume = Mathf.Max(targetVolume, proximityFactor * maxHeartbeatVolume);
        }

        heartbeatSource.volume = Mathf.Lerp(heartbeatSource.volume, targetVolume, Time.deltaTime * 2f);

        // Speed up heartbeat when panicking
        if (fearMeter.IsPanicking())
        {
            heartbeatSource.pitch = Mathf.Lerp(heartbeatSource.pitch, 1.3f, Time.deltaTime * 2f);
        }
        else
        {
            heartbeatSource.pitch = Mathf.Lerp(heartbeatSource.pitch, 1f, Time.deltaTime);
        }
    }

    private void UpdateChaseMusic()
    {
        bool anyMonsterChasing = false;
        
        foreach (MonsterAI monster in monsters)
        {
            if (monster != null && monster.currentState == MonsterAI.MonsterState.Chase)
            {
                anyMonsterChasing = true;
                break;
            }
        }

        if (anyMonsterChasing && !isPlayingChaseMusic)
        {
            StartChaseMusic();
        }
        else if (!anyMonsterChasing && isPlayingChaseMusic)
        {
            StopChaseMusic();
        }
    }

    private void StartChaseMusic()
    {
        if (monsterChaseMusic != null)
        {
            isPlayingChaseMusic = true;
            ambientSource.clip = monsterChaseMusic;
            ambientSource.volume = 0.5f;
            ambientSource.Play();
        }
    }

    private void StopChaseMusic()
    {
        isPlayingChaseMusic = false;
        
        if (ambientDrone != null)
        {
            ambientSource.clip = ambientDrone;
            ambientSource.volume = 0.3f;
            ambientSource.Play();
        }
    }

    public void PlayStaticEffect()
    {
        if (staticClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(staticClip, maxStaticVolume);
        }
    }

    public void PlayFootstep()
    {
        if (footstepClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(footstepClip, 0.2f);
        }
    }

    public void PlayBreathing()
    {
        if (breathingClip != null && sfxSource != null && !sfxSource.isPlaying)
        {
            sfxSource.PlayOneShot(breathingClip, 0.4f);
        }
    }
}
