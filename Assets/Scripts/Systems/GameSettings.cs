using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Don't Look/Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Player Settings")]
    [Tooltip("How fast the player moves")]
    public float playerMoveSpeed = 5f;
    
    [Tooltip("How fast the player rotates to face mouse")]
    public float playerRotationSpeed = 10f;

    [Header("Vision Settings")]
    [Tooltip("How far the player can see")]
    public float visionRange = 10f;
    
    [Tooltip("Width of the vision cone in degrees")]
    [Range(30f, 90f)]
    public float visionAngle = 45f;

    [Header("Fear System")]
    [Tooltip("How fast fear increases when looking at monster (per second)")]
    public float fearIncreaseRate = 10f;
    
    [Tooltip("How fast fear decreases when not looking (per second)")]
    public float fearDecreaseRate = 5f;
    
    [Tooltip("Fear level that triggers panic effects (0-100)")]
    [Range(0f, 100f)]
    public float panicThreshold = 80f;

    [Header("Flashlight Settings")]
    [Tooltip("Starting battery percentage")]
    [Range(0f, 100f)]
    public float startingBattery = 100f;
    
    [Tooltip("Battery drain per second when on")]
    public float batteryDrainRate = 5f;
    
    [Tooltip("Battery level that triggers low warning")]
    [Range(0f, 50f)]
    public float lowBatteryThreshold = 20f;
    
    [Tooltip("Normal flashlight intensity")]
    public float flashlightIntensity = 1f;

    [Header("Monster Settings")]
    [Tooltip("Monster movement speed when patrolling")]
    public float monsterPatrolSpeed = 2f;
    
    [Tooltip("Monster movement speed when chasing")]
    public float monsterChaseSpeed = 4f;
    
    [Tooltip("How long player must look before monster chases (seconds)")]
    public float chaseActivationTime = 1.5f;
    
    [Tooltip("How long monster chases after player looks away (seconds)")]
    public float chaseDeactivationTime = 3f;
    
    [Tooltip("How close monster must be to catch player")]
    public float catchDistance = 1f;
    
    [Tooltip("How long monster waits at each waypoint")]
    public float waypointIdleTime = 2f;

    [Header("Audio Settings")]
    [Tooltip("Base volume for ambient sounds")]
    [Range(0f, 1f)]
    public float ambientVolume = 0.3f;
    
    [Tooltip("Maximum volume for heartbeat")]
    [Range(0f, 1f)]
    public float maxHeartbeatVolume = 0.7f;
    
    [Tooltip("Distance at which monster proximity affects audio")]
    public float audioProximityRange = 5f;

    [Header("Camera Settings")]
    [Tooltip("How smoothly camera follows player")]
    public float cameraSmoothSpeed = 5f;
    
    [Tooltip("Strength of camera shake effect")]
    public float cameraShakeMagnitude = 0.1f;

    // Difficulty Preset Methods
    public void SetEasy()
    {
        fearIncreaseRate = 5f;
        fearDecreaseRate = 8f;
        batteryDrainRate = 3f;
        chaseActivationTime = 2.5f;
        monsterChaseSpeed = 3.5f;
    }

    public void SetNormal()
    {
        fearIncreaseRate = 10f;
        fearDecreaseRate = 5f;
        batteryDrainRate = 5f;
        chaseActivationTime = 1.5f;
        monsterChaseSpeed = 4f;
    }

    public void SetHard()
    {
        fearIncreaseRate = 15f;
        fearDecreaseRate = 3f;
        batteryDrainRate = 7f;
        chaseActivationTime = 1f;
        monsterChaseSpeed = 5f;
    }
}
