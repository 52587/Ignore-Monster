using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool requiresKey = true;
    [SerializeField] private string requiredKeyTag = "Key";
    
    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer doorSprite;
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.green;

    private bool isUnlocked = false;

    private void Start()
    {
        if (!requiresKey)
        {
            isUnlocked = true;
        }

        UpdateVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isUnlocked)
            {
                // Player escaped!
                GameManager.Instance?.OnPlayerEscaped();
            }
            else
            {
                Debug.Log("Door is locked! Find the key first.");
            }
        }
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateVisual();
        Debug.Log("Door unlocked!");
    }

    private void UpdateVisual()
    {
        if (doorSprite != null)
        {
            doorSprite.color = isUnlocked ? unlockedColor : lockedColor;
        }
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }
}
