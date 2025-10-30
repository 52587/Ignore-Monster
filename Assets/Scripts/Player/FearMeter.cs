using UnityEngine;
using UnityEngine.Events;

public class FearMeter : MonoBehaviour
{
    [Header("Fear Settings")]
    [SerializeField] private float maxFear = 100f;
    [SerializeField] private float currentFear = 0f;
    [SerializeField] private float fearIncreaseRate = 10f; // Per second when looking at monster
    [SerializeField] private float fearDecreaseRate = 5f; // Per second when not looking
    [SerializeField] private float panicThreshold = 80f; // Visual/audio effects kick in

    [Header("References")]
    [SerializeField] private PlayerVision playerVision;

    [Header("Events")]
    public UnityEvent OnPanicThresholdReached;
    public UnityEvent OnFearMaxed;

    private bool isPanicking = false;

    private void Start()
    {
        if (playerVision == null)
        {
            playerVision = GetComponent<PlayerVision>();
        }
    }

    private void Update()
    {
        UpdateFear();
        CheckPanicState();
    }

    private void UpdateFear()
    {
        if (playerVision.IsLookingAtMonster())
        {
            // Increase fear when looking at monsters
            currentFear += fearIncreaseRate * Time.deltaTime;
            currentFear = Mathf.Min(currentFear, maxFear);

            // Check if fear is maxed
            if (currentFear >= maxFear)
            {
                OnFearMaxed?.Invoke();
            }
        }
        else
        {
            // Decrease fear when not looking
            currentFear -= fearDecreaseRate * Time.deltaTime;
            currentFear = Mathf.Max(currentFear, 0f);

            // Reset panic if fear drops below threshold
            if (currentFear < panicThreshold - 10f)
            {
                isPanicking = false;
            }
        }
    }

    private void CheckPanicState()
    {
        if (!isPanicking && currentFear >= panicThreshold)
        {
            isPanicking = true;
            OnPanicThresholdReached?.Invoke();
        }
    }

    public float GetFearPercentage()
    {
        return currentFear / maxFear;
    }

    public float GetCurrentFear()
    {
        return currentFear;
    }

    public bool IsPanicking()
    {
        return isPanicking;
    }

    public void AddFear(float amount)
    {
        currentFear += amount;
        currentFear = Mathf.Min(currentFear, maxFear);
    }

    public void ReduceFear(float amount)
    {
        currentFear -= amount;
        currentFear = Mathf.Max(currentFear, 0f);
    }

    public void ResetFear()
    {
        currentFear = 0f;
        isPanicking = false;
    }
}
