using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Collision debugging utility that provides visual debugging and detailed logging
/// for collision events and validation system status.
/// </summary>
public class CollisionDebugger : MonoBehaviour
{
    #region Inspector Configuration
    
    [Header("Debug Visualization")]
    [Tooltip("Show collision points as spheres in Scene view")]
    [SerializeField] private bool showCollisionPoints = true;
    
    [Tooltip("Show ball velocity vectors in Scene view")]
    [SerializeField] private bool showVelocityVectors = true;
    
    [Tooltip("Show validation status information in Scene view")]
    [SerializeField] private bool showValidationStatus = true;
    
    [Tooltip("Log collision events to console")]
    [SerializeField] private bool logCollisionEvents = true;
    
    [Tooltip("Duration to keep debug visualizations active")]
    [Range(0.5f, 5f)]
    [SerializeField] private float debugDisplayDuration = 1.0f;
    
    [Tooltip("Maximum number of recent collisions to track")]
    [Range(10, 100)]
    [SerializeField] private int maxTrackedCollisions = 50;
    
    [Header("Visual Settings")]
    [Tooltip("Size of collision point spheres")]
    [Range(0.05f, 0.5f)]
    [SerializeField] private float collisionPointSize = 0.1f;
    
    [Tooltip("Length of velocity vector lines")]
    [Range(0.5f, 3f)]
    [SerializeField] private float velocityVectorScale = 1.5f;
    
    [Tooltip("Enable debug visualizations only in development builds")]
    [SerializeField] private bool developmentOnly = true;
    
    #endregion
    
    #region Private Fields
    
    // Debug data tracking
    private readonly List<CollisionDebugInfo> recentCollisions = new List<CollisionDebugInfo>();
    private CollisionManager collisionManager;
    private Rigidbody2D ballRigidbody;
    
    // Visual debug colors
    private readonly Color paddleColor = Color.cyan;
    private readonly Color brickColor = Color.yellow;
    private readonly Color boundaryColor = Color.white;
    private readonly Color powerUpColor = Color.magenta;
    private readonly Color unknownColor = Color.gray;
    private readonly Color velocityColor = Color.green;
    private readonly Color validationColor = Color.red;
    
    #endregion
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize collision debugger and find required components.
    /// </summary>
    private void Start()
    {
        InitializeDebugger();
    }
    
    /// <summary>
    /// Update debug visualizations and clean up old data.
    /// </summary>
    private void Update()
    {
        CleanupOldCollisions();
        
        // Update ball rigidbody reference if needed
        if (ballRigidbody == null && collisionManager != null)
        {
            FindBallRigidbody();
        }
    }
    
    /// <summary>
    /// Draw debug visualizations in Scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!ShouldShowDebug()) return;
        
        // Draw collision points
        if (showCollisionPoints)
        {
            DrawCollisionPoints();
        }
        
        // Draw velocity vectors
        if (showVelocityVectors && ballRigidbody != null)
        {
            DrawVelocityVector();
        }
        
        // Draw validation status
        if (showValidationStatus && collisionManager != null)
        {
            DrawValidationStatus();
        }
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initialize collision debugger components and references.
    /// </summary>
    private void InitializeDebugger()
    {
        Debug.Log("[CollisionDebugger] Initializing collision debugging system...");
        
        // Find CollisionManager
        collisionManager = CollisionManager.Instance;
        if (collisionManager == null)
        {
            collisionManager = FindFirstObjectByType<CollisionManager>();
        }
        
        if (collisionManager != null)
        {
            Debug.Log("[CollisionDebugger] CollisionManager found and connected");
        }
        else
        {
            Debug.LogWarning("[CollisionDebugger] CollisionManager not found. Debug functionality will be limited.");
        }
        
        // Find Ball Rigidbody2D
        FindBallRigidbody();
        
        // Log debug configuration
        Debug.Log("[CollisionDebugger] Configuration:");
        Debug.Log($"   • Collision Points: {showCollisionPoints}");
        Debug.Log($"   • Velocity Vectors: {showVelocityVectors}");
        Debug.Log($"   • Validation Status: {showValidationStatus}");
        Debug.Log($"   • Event Logging: {logCollisionEvents}");
        Debug.Log($"   • Display Duration: {debugDisplayDuration:F1}s");
        Debug.Log($"   • Max Tracked: {maxTrackedCollisions}");
        Debug.Log($"   • Development Only: {developmentOnly}");
        
        Debug.Log("[CollisionDebugger] Collision debugger initialized successfully");
    }
    
    /// <summary>
    /// Find and cache Ball Rigidbody2D component.
    /// </summary>
    private void FindBallRigidbody()
    {
        GameObject ball = GameObject.Find("Ball");
        if (ball != null)
        {
            ballRigidbody = ball.GetComponent<Rigidbody2D>();
            if (ballRigidbody != null)
            {
                Debug.Log("[CollisionDebugger] Ball Rigidbody2D found for velocity debugging");
            }
            else
            {
                Debug.LogWarning("[CollisionDebugger] Ball GameObject found but missing Rigidbody2D component");
            }
        }
        else
        {
            Debug.LogWarning("[CollisionDebugger] Ball GameObject not found in scene");
        }
    }
    
    #endregion
    
    #region Public Debug API
    
    /// <summary>
    /// Log a collision event for debugging and visualization.
    /// </summary>
    /// <param name="collisionType">Type of collision</param>
    /// <param name="position">World position of collision</param>
    /// <param name="velocity">Ball velocity at collision</param>
    /// <param name="intensity">Collision intensity</param>
    public void LogCollisionEvent(CollisionType collisionType, Vector2 position, Vector2 velocity, float intensity)
    {
        // Create debug info entry
        CollisionDebugInfo debugInfo = new CollisionDebugInfo
        {
            type = collisionType,
            position = position,
            velocity = velocity,
            timestamp = Time.time,
            intensity = intensity,
            wasValidated = false,
            validationResult = ""
        };
        
        // Add to tracking list
        recentCollisions.Add(debugInfo);
        
        // Log to console if enabled
        if (logCollisionEvents)
        {
            Debug.Log($"[CollisionDebugger] Collision Event: {collisionType} at {position} with velocity {velocity.magnitude:F2} (intensity {intensity:F2})");
        }
        
        // Limit tracking list size
        if (recentCollisions.Count > maxTrackedCollisions)
        {
            recentCollisions.RemoveAt(0);
        }
    }
    
    /// <summary>
    /// Log a validation event for debugging.
    /// </summary>
    /// <param name="eventType">Type of validation event</param>
    /// <param name="position">Position where validation occurred</param>
    /// <param name="details">Detailed validation information</param>
    public void LogValidationEvent(string eventType, Vector2 position, string details)
    {
        CollisionDebugInfo debugInfo = new CollisionDebugInfo
        {
            type = CollisionType.Unknown,
            position = position,
            velocity = ballRigidbody != null ? ballRigidbody.linearVelocity : Vector2.zero,
            timestamp = Time.time,
            intensity = 0f,
            wasValidated = true,
            validationResult = $"{eventType}: {details}"
        };
        
        recentCollisions.Add(debugInfo);
        
        if (logCollisionEvents)
        {
            Debug.Log($"[CollisionDebugger] Validation Event: {eventType} at {position} - {details}");
        }
    }
    
    /// <summary>
    /// Get formatted debug information for display.
    /// </summary>
    /// <returns>Debug information string</returns>
    public string GetDebugInfo()
    {
        string info = "Collision Debug Information:\n";
        info += $"• Recent Collisions: {recentCollisions.Count}\n";
        info += $"• Ball Velocity: {(ballRigidbody != null ? ballRigidbody.linearVelocity.magnitude.ToString("F2") : "N/A")}\n";
        
        if (collisionManager != null)
        {
            info += $"• Validation Status: {collisionManager.GetValidationStatus()}\n";
        }
        
        return info;
    }
    
    #endregion
    
    #region Debug Visualization
    
    /// <summary>
    /// Check if debug visualizations should be shown.
    /// </summary>
    /// <returns>True if debug should be shown</returns>
    private bool ShouldShowDebug()
    {
        if (developmentOnly && !Debug.isDebugBuild)
        {
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Draw collision points as colored spheres.
    /// </summary>
    private void DrawCollisionPoints()
    {
        foreach (CollisionDebugInfo collision in recentCollisions)
        {
            // Skip old collisions
            if (Time.time - collision.timestamp > debugDisplayDuration)
                continue;
            
            // Set color based on collision type
            Color color = GetCollisionColor(collision.type);
            
            // Fade based on age
            float age = Time.time - collision.timestamp;
            float alpha = 1f - (age / debugDisplayDuration);
            color.a = alpha;
            
            Gizmos.color = color;
            
            // Draw collision point
            if (collision.wasValidated)
            {
                // Draw validation events as cubes
                Gizmos.DrawCube(collision.position, Vector3.one * collisionPointSize);
            }
            else
            {
                // Draw collision events as spheres
                Gizmos.DrawSphere(collision.position, collisionPointSize);
            }
        }
    }
    
    /// <summary>
    /// Draw ball velocity vector.
    /// </summary>
    private void DrawVelocityVector()
    {
        if (ballRigidbody == null) return;
        
        Vector3 ballPosition = ballRigidbody.transform.position;
        Vector3 velocity = ballRigidbody.linearVelocity;
        
        if (velocity.magnitude < 0.1f) return; // Skip very slow velocities
        
        // Draw velocity vector
        Gizmos.color = velocityColor;
        Vector3 velocityEnd = ballPosition + velocity.normalized * velocityVectorScale;
        Gizmos.DrawLine(ballPosition, velocityEnd);
        
        // Draw arrowhead
        Vector3 arrowRight = Quaternion.Euler(0, 0, 20) * -velocity.normalized * 0.3f;
        Vector3 arrowLeft = Quaternion.Euler(0, 0, -20) * -velocity.normalized * 0.3f;
        Gizmos.DrawLine(velocityEnd, velocityEnd + arrowRight);
        Gizmos.DrawLine(velocityEnd, velocityEnd + arrowLeft);
    }
    
    /// <summary>
    /// Draw validation status information.
    /// </summary>
    private void DrawValidationStatus()
    {
        if (collisionManager == null || ballRigidbody == null) return;
        
        Vector3 ballPosition = ballRigidbody.transform.position;
        
        // Draw speed constraint visualization
        float currentSpeed = ballRigidbody.linearVelocity.magnitude;
        Color speedColor = Color.green;
        
        // Check if speed is outside constraints (using reasonable defaults)
        float minSpeed = 3.0f; // Default values
        float maxSpeed = 15.0f;
        
        if (currentSpeed < minSpeed)
        {
            speedColor = Color.red; // Too slow
        }
        else if (currentSpeed > maxSpeed)
        {
            speedColor = new Color(1f, 0.5f, 0f); // Orange - Too fast
        }
        
        // Draw speed status as a ring around the ball
        Gizmos.color = speedColor;
        DrawGizmosCircle(ballPosition, 0.3f);
    }
    
    /// <summary>
    /// Draw a circle using Gizmos lines.
    /// </summary>
    /// <param name="center">Center position</param>
    /// <param name="radius">Circle radius</param>
    private void DrawGizmosCircle(Vector3 center, float radius)
    {
        int segments = 20;
        float angleStep = 360f / segments;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
            
            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1)) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius;
            
            Gizmos.DrawLine(point1, point2);
        }
    }
    
    /// <summary>
    /// Get color for collision type visualization.
    /// </summary>
    /// <param name="collisionType">Collision type</param>
    /// <returns>Color for visualization</returns>
    private Color GetCollisionColor(CollisionType collisionType)
    {
        switch (collisionType)
        {
            case CollisionType.Paddle: return paddleColor;
            case CollisionType.Brick: return brickColor;
            case CollisionType.Boundary: return boundaryColor;
            case CollisionType.PowerUp: return powerUpColor;
            default: return unknownColor;
        }
    }
    
    #endregion
    
    #region Data Management
    
    /// <summary>
    /// Clean up old collision data to prevent memory buildup.
    /// </summary>
    private void CleanupOldCollisions()
    {
        float currentTime = Time.time;
        float maxAge = debugDisplayDuration * 2f; // Keep data twice as long as display duration
        
        recentCollisions.RemoveAll(collision => currentTime - collision.timestamp > maxAge);
    }
    
    #endregion
}

/// <summary>
/// Debug information structure for collision tracking.
/// </summary>
[System.Serializable]
public struct CollisionDebugInfo
{
    public CollisionType type;
    public Vector2 position;
    public Vector2 velocity;
    public float timestamp;
    public float intensity;
    public bool wasValidated;
    public string validationResult;
}