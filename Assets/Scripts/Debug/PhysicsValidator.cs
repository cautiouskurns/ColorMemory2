using UnityEngine;

/// <summary>
/// Physics validation system for detecting collision anomalies, tunneling, and physics edge cases.
/// Provides continuous monitoring and automated recovery suggestions for ball physics system.
/// </summary>
public class PhysicsValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    [SerializeField] private float stuckBallThreshold = 0.1f;
    [SerializeField] private float stuckTimeLimit = 2f;
    [SerializeField] private float tunnelDetectionDistance = 1f;
    [SerializeField] private float extremeSpeedThreshold = 25f;
    [SerializeField] private bool enableValidationLogging = true;
    
    [Header("Component References")]
    [SerializeField] private BallController ballController;
    [SerializeField] private Rigidbody2D ballRigidbody;
    [SerializeField] private CircleCollider2D ballCollider;
    
    [Header("Validation State")]
    [SerializeField] private bool validationActive = true;
    [SerializeField] private int totalValidationChecks = 0;
    [SerializeField] private int anomaliesDetected = 0;
    [SerializeField] private float lastValidationTime;
    
    // Stuck ball detection
    private Vector3 lastPosition;
    private float stuckTimer;
    private bool stuckBallDetected = false;
    
    // Tunneling detection
    private Vector3 previousPosition;
    private Vector3 previousVelocity;
    private bool tunnelingDetected = false;
    
    // Speed validation
    private float lastValidSpeed;
    private bool extremeSpeedDetected = false;
    
    // Collision validation
    private int lastCollisionCount = 0;
    private Vector3 lastCollisionPosition;
    private float collisionValidationTimer = 0f;
    
    // Component validation
    private bool componentsValid = false;
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize physics validation system and cache component references.
    /// </summary>
    private void Start()
    {
        ValidateComponents();
        InitializeValidation();
        
        if (enableValidationLogging)
        {
            Debug.Log("[PhysicsValidator] Physics validation system initialized");
        }
    }
    
    /// <summary>
    /// Continuous validation monitoring in physics update loop.
    /// </summary>
    private void FixedUpdate()
    {
        if (!validationActive || !componentsValid) return;
        
        // Increment validation check counter
        totalValidationChecks++;
        lastValidationTime = Time.fixedTime;
        
        // Perform validation checks
        ValidateMovement();
        DetectTunneling();
        ValidateSpeed();
        ValidateCollisions();
        
        // Update tracking data for next frame
        UpdateTrackingData();
    }
    
    #endregion
    
    #region Component Management
    
    /// <summary>
    /// Validates and caches required component references.
    /// </summary>
    private void ValidateComponents()
    {
        // Find BallController if not assigned
        if (ballController == null)
        {
            ballController = GetComponent<BallController>();
            if (ballController == null)
            {
                ballController = FindObjectOfType<BallController>();
            }
        }
        
        // Find Rigidbody2D if not assigned
        if (ballRigidbody == null)
        {
            ballRigidbody = GetComponent<Rigidbody2D>();
        }
        
        // Find CircleCollider2D if not assigned
        if (ballCollider == null)
        {
            ballCollider = GetComponent<CircleCollider2D>();
        }
        
        // Validate all components exist
        if (ballController == null)
        {
            Debug.LogError("[PhysicsValidator] BallController component not found! Validation disabled.");
            componentsValid = false;
            return;
        }
        
        if (ballRigidbody == null)
        {
            Debug.LogError("[PhysicsValidator] Rigidbody2D component not found! Validation disabled.");
            componentsValid = false;
            return;
        }
        
        if (ballCollider == null)
        {
            Debug.LogError("[PhysicsValidator] CircleCollider2D component not found! Validation disabled.");
            componentsValid = false;
            return;
        }
        
        componentsValid = true;
        
        if (enableValidationLogging)
        {
            Debug.Log("[PhysicsValidator] Component validation successful");
        }
    }
    
    /// <summary>
    /// Initializes validation system with current ball state.
    /// </summary>
    private void InitializeValidation()
    {
        if (!componentsValid) return;
        
        // Initialize position tracking
        lastPosition = transform.position;
        previousPosition = transform.position;
        
        // Initialize velocity tracking
        previousVelocity = ballRigidbody.linearVelocity;
        lastValidSpeed = ballRigidbody.linearVelocity.magnitude;
        
        // Initialize collision tracking
        BallData ballData = ballController.GetBallData();
        if (ballData != null)
        {
            lastCollisionCount = ballData.collisionCount;
        }
        
        // Reset timers
        stuckTimer = 0f;
        collisionValidationTimer = 0f;
        
        if (enableValidationLogging)
        {
            Debug.Log("[PhysicsValidator] Validation initialization complete");
        }
    }
    
    #endregion
    
    #region Movement Validation
    
    /// <summary>
    /// Validates ball movement and detects stuck scenarios.
    /// </summary>
    /// <returns>True if movement is valid</returns>
    private bool ValidateMovement()
    {
        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);
        bool isMoving = ballController.IsMoving();
        
        // Check for stuck ball
        if (isMoving && distanceMoved < stuckBallThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
            
            if (stuckTimer >= stuckTimeLimit && !stuckBallDetected)
            {
                stuckBallDetected = true;
                anomaliesDetected++;
                
                LogPhysicsEvent("STUCK_BALL", 
                    $"Ball stuck detected: Position={currentPosition}, Distance moved={distanceMoved:F4}, Timer={stuckTimer:F2}s");
                
                HandlePhysicsAnomaly("STUCK_BALL");
                return false;
            }
        }
        else
        {
            if (stuckBallDetected && distanceMoved >= stuckBallThreshold)
            {
                stuckBallDetected = false;
                LogPhysicsEvent("RECOVERY", "Ball movement recovered - stuck state resolved");
            }
            stuckTimer = 0f;
        }
        
        return true;
    }
    
    #endregion
    
    #region Tunneling Detection
    
    /// <summary>
    /// Detects collision tunneling by analyzing position and velocity changes.
    /// </summary>
    /// <returns>True if tunneling is detected</returns>
    private bool DetectTunneling()
    {
        Vector3 currentPosition = transform.position;
        Vector2 currentVelocity = ballRigidbody.linearVelocity;
        
        // Calculate distance traveled this frame
        float distanceTraveled = Vector3.Distance(currentPosition, previousPosition);
        
        // Calculate expected distance based on velocity
        float expectedDistance = previousVelocity.magnitude * Time.fixedDeltaTime;
        
        // Detect potential tunneling
        bool potentialTunneling = distanceTraveled > tunnelDetectionDistance && 
                                 distanceTraveled > expectedDistance * 2f;
        
        if (potentialTunneling && !tunnelingDetected)
        {
            tunnelingDetected = true;
            anomaliesDetected++;
            
            LogPhysicsEvent("TUNNELING", 
                $"Potential tunneling detected: Distance={distanceTraveled:F2}, Expected={expectedDistance:F2}, Speed={currentVelocity.magnitude:F2}");
            
            HandlePhysicsAnomaly("TUNNELING");
            return true;
        }
        else if (!potentialTunneling && tunnelingDetected)
        {
            tunnelingDetected = false;
            LogPhysicsEvent("RECOVERY", "Tunneling resolved - normal collision detection resumed");
        }
        
        return false;
    }
    
    #endregion
    
    #region Speed Validation
    
    /// <summary>
    /// Validates ball speed and detects extreme velocity scenarios.
    /// </summary>
    /// <returns>True if speed is within acceptable range</returns>
    private bool ValidateSpeed()
    {
        float currentSpeed = ballRigidbody.linearVelocity.magnitude;
        
        // Check for extreme speeds
        bool currentExtremeSpeed = currentSpeed > extremeSpeedThreshold;
        
        if (currentExtremeSpeed && !extremeSpeedDetected)
        {
            extremeSpeedDetected = true;
            anomaliesDetected++;
            
            LogPhysicsEvent("EXTREME_SPEED", 
                $"Extreme speed detected: {currentSpeed:F2} units/sec (threshold: {extremeSpeedThreshold})");
            
            HandlePhysicsAnomaly("EXTREME_SPEED");
            return false;
        }
        else if (!currentExtremeSpeed && extremeSpeedDetected)
        {
            extremeSpeedDetected = false;
            LogPhysicsEvent("RECOVERY", "Speed normalized - extreme speed resolved");
        }
        
        // Check for sudden speed changes (potential physics glitch)
        if (lastValidSpeed > 0f)
        {
            float speedChangeRatio = currentSpeed / lastValidSpeed;
            if (speedChangeRatio > 3f || speedChangeRatio < 0.3f)
            {
                LogPhysicsEvent("SPEED_ANOMALY", 
                    $"Sudden speed change: {lastValidSpeed:F2} -> {currentSpeed:F2} (ratio: {speedChangeRatio:F2})");
            }
        }
        
        if (currentSpeed > 0.1f)
        {
            lastValidSpeed = currentSpeed;
        }
        
        return true;
    }
    
    #endregion
    
    #region Collision Validation
    
    /// <summary>
    /// Validates collision detection and response behavior.
    /// </summary>
    /// <returns>True if collision behavior is valid</returns>
    private bool ValidateCollisions()
    {
        BallData ballData = ballController.GetBallData();
        if (ballData == null) return true;
        
        int currentCollisionCount = ballData.collisionCount;
        
        // Check for collision count increase
        if (currentCollisionCount > lastCollisionCount)
        {
            Vector3 currentPosition = transform.position;
            
            // Validate collision position change
            if (lastCollisionPosition != Vector3.zero)
            {
                float collisionDistance = Vector3.Distance(currentPosition, lastCollisionPosition);
                
                // Log collision validation
                LogPhysicsEvent("COLLISION", 
                    $"Collision #{currentCollisionCount}: Position={currentPosition}, Distance from last={collisionDistance:F2}");
            }
            
            lastCollisionPosition = currentPosition;
            lastCollisionCount = currentCollisionCount;
            collisionValidationTimer = 0f;
        }
        
        // Update collision validation timer
        collisionValidationTimer += Time.fixedDeltaTime;
        
        return true;
    }
    
    #endregion
    
    #region Anomaly Handling
    
    /// <summary>
    /// Handles detected physics anomalies and provides recovery suggestions.
    /// </summary>
    /// <param name="anomalyType">Type of detected anomaly</param>
    private void HandlePhysicsAnomaly(string anomalyType)
    {
        switch (anomalyType)
        {
            case "STUCK_BALL":
                HandleStuckBall();
                break;
                
            case "TUNNELING":
                HandleTunneling();
                break;
                
            case "EXTREME_SPEED":
                HandleExtremeSpeed();
                break;
                
            default:
                LogPhysicsEvent("UNKNOWN_ANOMALY", $"Unknown anomaly type: {anomalyType}");
                break;
        }
    }
    
    /// <summary>
    /// Handles stuck ball scenario with recovery suggestions.
    /// </summary>
    private void HandleStuckBall()
    {
        LogPhysicsEvent("RECOVERY_SUGGESTION", "Stuck ball detected - Consider:");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "1. Check for collision overlap issues");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "2. Verify physics material configuration");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "3. Review velocity management constraints");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "4. Check for geometry collision problems");
        
        // Optional: Attempt automatic recovery
        if (ballController != null)
        {
            // Small impulse to unstick ball
            Vector2 randomImpulse = Random.insideUnitCircle.normalized * 2f;
            ballRigidbody.AddForce(randomImpulse, ForceMode2D.Impulse);
            
            LogPhysicsEvent("AUTO_RECOVERY", $"Applied recovery impulse: {randomImpulse}");
        }
    }
    
    /// <summary>
    /// Handles tunneling detection with collision system recommendations.
    /// </summary>
    private void HandleTunneling()
    {
        LogPhysicsEvent("RECOVERY_SUGGESTION", "Tunneling detected - Consider:");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "1. Enable Continuous collision detection");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "2. Reduce ball speed or increase collider size");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "3. Check for thin collision geometry");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "4. Review physics timestep settings");
        
        // Validate current collision detection mode
        if (ballRigidbody.collisionDetectionMode != CollisionDetectionMode2D.Continuous)
        {
            LogPhysicsEvent("VALIDATION_WARNING", 
                $"Collision detection mode is {ballRigidbody.collisionDetectionMode}, recommend Continuous");
        }
    }
    
    /// <summary>
    /// Handles extreme speed scenarios with velocity management recommendations.
    /// </summary>
    private void HandleExtremeSpeed()
    {
        LogPhysicsEvent("RECOVERY_SUGGESTION", "Extreme speed detected - Consider:");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "1. Check velocity management constraints");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "2. Review physics material bounce settings");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "3. Verify collision response calculations");
        LogPhysicsEvent("RECOVERY_SUGGESTION", "4. Check for force accumulation issues");
        
        // Optional: Apply speed limiting
        if (ballController != null)
        {
            BallData ballData = ballController.GetBallData();
            if (ballData != null && ballRigidbody.linearVelocity.magnitude > ballData.maxSpeed)
            {
                Vector2 limitedVelocity = ballRigidbody.linearVelocity.normalized * ballData.maxSpeed;
                ballRigidbody.linearVelocity = limitedVelocity;
                
                LogPhysicsEvent("AUTO_RECOVERY", $"Applied speed limit: {limitedVelocity.magnitude:F2}");
            }
        }
    }
    
    #endregion
    
    #region Data Management
    
    /// <summary>
    /// Updates tracking data for next frame validation.
    /// </summary>
    private void UpdateTrackingData()
    {
        lastPosition = transform.position;
        previousPosition = transform.position;
        previousVelocity = ballRigidbody.linearVelocity;
    }
    
    #endregion
    
    #region Logging System
    
    /// <summary>
    /// Logs categorized physics validation events.
    /// </summary>
    /// <param name="eventType">Type of validation event</param>
    /// <param name="details">Detailed event information</param>
    private void LogPhysicsEvent(string eventType, string details)
    {
        if (!enableValidationLogging) return;
        
        string timestamp = Time.fixedTime.ToString("F2");
        string logMessage = $"[PhysicsValidator][{eventType}][{timestamp}s] {details}";
        
        switch (eventType)
        {
            case "STUCK_BALL":
            case "TUNNELING":
            case "EXTREME_SPEED":
            case "SPEED_ANOMALY":
            case "VALIDATION_WARNING":
                Debug.LogWarning(logMessage);
                break;
                
            case "RECOVERY":
            case "AUTO_RECOVERY":
                Debug.Log(logMessage);
                break;
                
            case "RECOVERY_SUGGESTION":
                Debug.LogWarning(logMessage);
                break;
                
            default:
                Debug.Log(logMessage);
                break;
        }
    }
    
    #endregion
    
    #region Public Interface
    
    /// <summary>
    /// Sets validation active state.
    /// </summary>
    /// <param name="active">Whether validation should be active</param>
    public void SetValidationActive(bool active)
    {
        validationActive = active;
        
        if (enableValidationLogging)
        {
            Debug.Log($"[PhysicsValidator] Validation {(active ? "activated" : "deactivated")}");
        }
    }
    
    /// <summary>
    /// Sets validation logging enabled state.
    /// </summary>
    /// <param name="enabled">Whether validation logging should be enabled</param>
    public void SetValidationLogging(bool enabled)
    {
        enableValidationLogging = enabled;
        Debug.Log($"[PhysicsValidator] Validation logging {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Gets validation statistics summary.
    /// </summary>
    /// <returns>Validation statistics string</returns>
    public string GetValidationStats()
    {
        float validationRate = totalValidationChecks > 0 ? 
            (float)(totalValidationChecks - anomaliesDetected) / totalValidationChecks * 100f : 100f;
        
        return $"Validation Stats: {totalValidationChecks} checks, {anomaliesDetected} anomalies ({validationRate:F1}% success rate)";
    }
    
    /// <summary>
    /// Resets validation statistics.
    /// </summary>
    public void ResetValidationStats()
    {
        totalValidationChecks = 0;
        anomaliesDetected = 0;
        
        if (enableValidationLogging)
        {
            Debug.Log("[PhysicsValidator] Validation statistics reset");
        }
    }
    
    /// <summary>
    /// Forces validation of current physics state.
    /// </summary>
    public void ForceValidation()
    {
        if (!componentsValid) return;
        
        if (enableValidationLogging)
        {
            Debug.Log("[PhysicsValidator] Forcing validation check...");
        }
        
        ValidateMovement();
        DetectTunneling();
        ValidateSpeed();
        ValidateCollisions();
        
        if (enableValidationLogging)
        {
            Debug.Log("[PhysicsValidator] Forced validation complete");
        }
    }
    
    /// <summary>
    /// Gets current validation state summary.
    /// </summary>
    /// <returns>Current validation state information</returns>
    public string GetValidationState()
    {
        if (!componentsValid)
        {
            return "PhysicsValidator: Components not valid";
        }
        
        return $"PhysicsValidator State:\n" +
               $"• Active: {validationActive}\n" +
               $"• Stuck Ball: {stuckBallDetected}\n" +
               $"• Tunneling: {tunnelingDetected}\n" +
               $"• Extreme Speed: {extremeSpeedDetected}\n" +
               $"• Total Checks: {totalValidationChecks}\n" +
               $"• Anomalies: {anomaliesDetected}\n" +
               $"• Last Validation: {lastValidationTime:F2}s";
    }
    
    #endregion
}