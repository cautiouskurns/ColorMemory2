using UnityEngine;

/// <summary>
/// Serializable data structure for ball physics configuration and state management.
/// Provides Inspector-based tuning for arcade-style Breakout ball physics.
/// </summary>
[System.Serializable]
public class BallData
{
    [Header("Speed Configuration")]
    [Tooltip("Base movement speed of the ball in units per second")]
    public float baseSpeed = 8f;
    
    [Tooltip("Minimum allowed ball speed to prevent stalling")]
    public float minSpeed = 5f;
    
    [Tooltip("Maximum allowed ball speed to maintain gameplay balance")]
    public float maxSpeed = 15f;
    
    [Header("Launch Settings")]
    [Tooltip("Initial launch direction (normalized automatically)")]
    public Vector2 launchDirection = new Vector2(0.5f, 1f);
    
    [Tooltip("Random angle variance in degrees for launch direction")]
    [Range(0f, 45f)]
    public float launchAngleRange = 30f;
    
    [Header("Physics State")]
    [Tooltip("Current velocity of the ball (runtime)")]
    public Vector2 currentVelocity = Vector2.zero;
    
    [Tooltip("Number of collisions since launch (for scoring/effects)")]
    public int collisionCount = 0;
    
    [Tooltip("Position where ball was last launched from")]
    public Vector3 launchPosition = Vector3.zero;
    
    [Header("Arcade Physics Tuning")]
    [Tooltip("Velocity retention on bounce (1.0 = no damping, 0.9 = 10% speed loss)")]
    [Range(0.8f, 1.2f)]
    public float bounceDamping = 1.0f;
    
    [Tooltip("Force ball to maintain constant speed magnitude for arcade feel")]
    public bool maintainConstantSpeed = true;
    
    /// <summary>
    /// Validates and clamps speed values to ensure logical constraints.
    /// </summary>
    public void ValidateSpeedConstraints()
    {
        // Ensure min speed doesn't exceed max speed
        minSpeed = Mathf.Min(minSpeed, maxSpeed);
        
        // Ensure base speed is within min/max bounds
        baseSpeed = Mathf.Clamp(baseSpeed, minSpeed, maxSpeed);
        
        // Ensure all speeds are positive
        minSpeed = Mathf.Max(0.1f, minSpeed);
        maxSpeed = Mathf.Max(minSpeed, maxSpeed);
        baseSpeed = Mathf.Max(minSpeed, baseSpeed);
    }
    
    /// <summary>
    /// Resets runtime state values to initial conditions.
    /// </summary>
    public void ResetState()
    {
        currentVelocity = Vector2.zero;
        collisionCount = 0;
        launchPosition = Vector3.zero;
    }
    
    /// <summary>
    /// Gets a randomized launch direction based on configured parameters.
    /// </summary>
    /// <returns>Normalized launch direction vector with applied angle variance</returns>
    public Vector2 GetRandomizedLaunchDirection()
    {
        // Normalize the base launch direction
        Vector2 normalizedDirection = launchDirection.normalized;
        
        // Apply random angle variance
        float randomAngle = Random.Range(-launchAngleRange, launchAngleRange);
        float angleInRadians = randomAngle * Mathf.Deg2Rad;
        
        // Rotate the direction vector by the random angle
        float cos = Mathf.Cos(angleInRadians);
        float sin = Mathf.Sin(angleInRadians);
        
        Vector2 randomizedDirection = new Vector2(
            normalizedDirection.x * cos - normalizedDirection.y * sin,
            normalizedDirection.x * sin + normalizedDirection.y * cos
        );
        
        return randomizedDirection.normalized;
    }
    
    /// <summary>
    /// Applies speed constraints to a velocity vector based on configuration.
    /// </summary>
    /// <param name="velocity">The velocity to constrain</param>
    /// <returns>Constrained velocity vector</returns>
    public Vector2 ApplySpeedConstraints(Vector2 velocity)
    {
        float currentSpeed = velocity.magnitude;
        
        if (maintainConstantSpeed)
        {
            // Force constant speed at base speed value
            return velocity.normalized * baseSpeed;
        }
        else
        {
            // Clamp speed within min/max bounds
            float clampedSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
            
            // Apply only if velocity is non-zero to avoid division by zero
            if (currentSpeed > 0.001f)
            {
                return velocity.normalized * clampedSpeed;
            }
            
            return velocity;
        }
    }
}