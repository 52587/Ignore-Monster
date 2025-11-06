using UnityEngine;
using UnityEngine.Events;

public class FearMeter : MonoBehaviour
{
    [Header("Fear Settings")]
    [SerializeField] private float maxFear = 100f;
    [SerializeField] private float currentFear = 0f;
    
    [Header("Distance-Based Fear")]
    [SerializeField] private float maxFearDistance = 3f;
    [SerializeField] private float noFearDistance = 15f;
    [SerializeField] private float fearIncreaseRate = 20f;
    [SerializeField] private float fearDecreaseRate = 10f;
    
    [Header("Death Settings")]
    [SerializeField] private float maxFearDeathDelay = 3f;
    [SerializeField] private float panicThreshold = 80f;

    [Header("References")]
    [SerializeField] private Transform monsterTransform;

    [Header("Events")]
    public UnityEvent OnPanicThresholdReached;
    public UnityEvent OnFearMaxed;

    private bool isPanicking = false;
    private float maxFearTimer = 0f;

    public float CurrentFear => currentFear;
    public float MaxFear => maxFear;
    public float FearPercentage => (currentFear / maxFear) * 100f;
    public bool IsPanicking => isPanicking;

    private void Start()
    {
        if (monsterTransform == null)
        {
            GameObject monster = GameObject.FindGameObjectWithTag("Monster");
            if (monster != null)
            {
                monsterTransform = monster.transform;
            }
        }
    }

    private void Update()
    {
        UpdateFear();
        CheckPanicState();
        CheckFearDeath();
    }

    private void UpdateFear()
    {
        if (monsterTransform == null || GameManager.Instance == null || GameManager.Instance.IsGameOver()) return;

        float distanceToMonster = Vector2.Distance(transform.position, monsterTransform.position);
        float targetFear = CalculateFearFromDistance(distanceToMonster);

        if (currentFear < targetFear)
        {
            currentFear += fearIncreaseRate * Time.deltaTime;
        }
        else if (currentFear > targetFear)
        {
            currentFear -= fearDecreaseRate * Time.deltaTime;
        }

        currentFear = Mathf.Clamp(currentFear, 0f, maxFear);

        if (currentFear >= maxFear)
        {
            OnFearMaxed?.Invoke();
        }
    }

    private float CalculateFearFromDistance(float distance)
    {
        if (distance <= maxFearDistance)
        {
            return maxFear;
        }

        if (distance >= noFearDistance)
        {
            return 0f;
        }

        float t = 1f - ((distance - maxFearDistance) / (noFearDistance - maxFearDistance));
        return Mathf.Lerp(0f, maxFear, t);
    }

    private void CheckFearDeath()
    {
        if (currentFear >= maxFear)
        {
            maxFearTimer += Time.deltaTime;
            
            if (maxFearTimer >= maxFearDeathDelay)
            {
                Die();
            }
        }
        else
        {
            maxFearTimer = 0f;
        }
    }

    private void Die()
    {
        Debug.Log("Player died from maximum fear!");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDiedFromFear();
        }
    }

    private void CheckPanicState()
    {
        if (!isPanicking && currentFear >= panicThreshold)
        {
            isPanicking = true;
            OnPanicThresholdReached?.Invoke();
        }
        else if (isPanicking && currentFear < panicThreshold - 10f)
        {
            isPanicking = false;
        }
    }

    public float GetFearPercentage()
    {
        return (currentFear / maxFear) * 100f;
    }

    public Color GetFearColor()
    {
        float t = FearPercentage / 100f;
        
        if (t < 0.33f)
        {
            return Color.green;
        }
        else if (t < 0.66f)
        {
            return Color.yellow;
        }
        else
        {
            return Color.red;
        }
    }

    public string GetFearStatusText()
    {
        float percent = FearPercentage;
        
        if (percent < 20f)
        {
            return "Calm";
        }
        else if (percent < 40f)
        {
            return "Uneasy";
        }
        else if (percent < 60f)
        {
            return "Nervous";
        }
        else if (percent < 80f)
        {
            return "Terrified";
        }
        else
        {
            return "PANIC!";
        }
    }

    public float GetDistanceToMonster()
    {
        if (monsterTransform == null) return float.MaxValue;
        return Vector2.Distance(transform.position, monsterTransform.position);
    }

    // Compatibility methods for other systems
    public float GetCurrentFear()
    {
        return currentFear;
    }

    public void ResetFear()
    {
        currentFear = 0f;
        isPanicking = false;
        maxFearTimer = 0f;
    }

    public void SetFear(float amount)
    {
        currentFear = Mathf.Clamp(amount, 0f, maxFear);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxFearDistance);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, (maxFearDistance + noFearDistance) / 2f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, noFearDistance);
    }
}
