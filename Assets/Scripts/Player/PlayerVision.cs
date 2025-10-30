using UnityEngine;
using System.Collections.Generic;

public class PlayerVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 45f; // Cone angle in degrees
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    private List<MonsterAI> visibleMonsters = new List<MonsterAI>();

    private void Start()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        DetectMonsters();
    }

    private void DetectMonsters()
    {
        visibleMonsters.Clear();

        // Find all monsters in range
        Collider2D[] monstersInRange = Physics2D.OverlapCircleAll(transform.position, visionRange, monsterLayer);

        foreach (Collider2D monsterCollider in monstersInRange)
        {
            Vector2 directionToMonster = (monsterCollider.transform.position - transform.position).normalized;
            Vector2 lookDirection = playerController.GetLookDirection();

            // Check if monster is within vision cone
            float angleToMonster = Vector2.Angle(lookDirection, directionToMonster);

            if (angleToMonster <= visionAngle / 2f)
            {
                // Check if there's line of sight (no obstacles)
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    directionToMonster,
                    visionRange,
                    monsterLayer | obstacleLayer
                );

                if (hit.collider != null && hit.collider == monsterCollider)
                {
                    // Player is looking at this monster
                    MonsterAI monster = hit.collider.GetComponent<MonsterAI>();
                    if (monster != null)
                    {
                        visibleMonsters.Add(monster);
                        monster.OnPlayerLookingAt();
                    }
                }
            }
        }
    }

    public bool IsLookingAtMonster()
    {
        return visibleMonsters.Count > 0;
    }

    public List<MonsterAI> GetVisibleMonsters()
    {
        return visibleMonsters;
    }

    // Visualize vision cone in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        if (playerController != null)
        {
            Vector3 lookDirection = playerController.GetLookDirection();
            Vector3 rightBoundary = Quaternion.Euler(0, 0, visionAngle / 2f) * lookDirection * visionRange;
            Vector3 leftBoundary = Quaternion.Euler(0, 0, -visionAngle / 2f) * lookDirection * visionRange;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, rightBoundary);
            Gizmos.DrawRay(transform.position, leftBoundary);
            Gizmos.DrawRay(transform.position, lookDirection * visionRange);
        }
    }
}
