using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ExitDoor exitDoor;
    [SerializeField] private AudioClip pickupSound;

    private void Start()
    {
        if (exitDoor == null)
        {
            exitDoor = FindObjectOfType<ExitDoor>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Unlock the door
            if (exitDoor != null)
            {
                exitDoor.Unlock();
            }

            // Play pickup sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            Debug.Log("Key collected!");

            // Destroy the key
            Destroy(gameObject);
        }
    }
}
