using UnityEngine;

/// <summary>
/// Core MonoBehaviour controller that manages ball physics behavior and component integration.
/// Provides basic movement methods, collision detection, and BallData configuration integration.
/// Foundation architecture for velocity management, launch mechanics, and collision response systems.
/// </summary>
public class BallController : MonoBehaviour
{
    [Header("Physics Configuration")]
    [SerializeField] private BallData ballData;
    
    [Header("Component References")]
    private Rigidbody2D rigidBody;
    private CircleCollider2D circleCollider;
    
    [Header("Physics State")]
    [SerializeField] private bool isMoving;
    [SerializeField] private Vector2 currentVelocity;
    
    [Header("Velocity Management")]
    [SerializeField] private bool velocityManagementEnabled = true;
    [SerializeField] private float velocityNormalizationThreshold = 0.1f;
    [SerializeField] private float speedConstraintTolerance = 0.05f;
    [SerializeField] private bool maintainConstantSpeed = true;
    [SerializeField] private float speedStabilizationRate = 5f;
    
    [Header("Launch Mechanics")]
    [SerializeField] private BallLaunchState currentState = BallLaunchState.Ready;
    [SerializeField] private float launchAngleRange = 60f;
    [SerializeField] private Vector2 defaultLaunchDirection = Vector2.up;
    [SerializeField] private Transform paddleTransform;
    [SerializeField] private float paddleOffset = 0.5f;
    [SerializeField] private bool enableLaunchDebugging = true;
    
    // Component validation flag
    private bool componentsValid = false;
    
    // Velocity management state
    private Vector2 targetVelocity;
    private float targetSpeed;
    private bool hasTargetSpeed = false;
    
    // Launch mechanics state
    private Vector2 launchDirection;
    private bool isReadyToLaunch = true;
    private Vector3 paddlePositionCache;
    private bool paddleValidated = false;
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Cache component references for performance optimization.
    /// Called before Start() to ensure all components are available.
    /// </summary>
    private void Awake()
    {
        CacheComponentReferences();
        ValidateComponents();
    }
    
    /// <summary>
    /// Initialize configuration after all components are ready.
    /// Sets up BallData integration, physics state, and launch mechanics.
    /// </summary>
    private void Start()
    {
        InitializeConfiguration();
        InitializePhysicsState();
        InitializeLaunchMechanics();
    }
    
    /// <summary>
    /// Update physics state tracking and manage velocity consistency.
    /// Implements velocity management system for arcade-style physics.
    /// </summary>
    private void FixedUpdate()
    {
        if (componentsValid && rigidBody != null)
        {
            currentVelocity = rigidBody.linearVelocity;
            isMoving = currentVelocity.magnitude > 0.1f;
            
            // Update BallData state if available
            if (ballData != null)
            {
                ballData.currentVelocity = currentVelocity;
            }
            
            // Apply velocity management if enabled and in appropriate state
            if (velocityManagementEnabled && isMoving && currentState.RequiresVelocityManagement())
            {
                ApplyVelocityManagement();
            }
        }
    }
    
    /// <summary>
    /// Handle input polling and launch state management.
    /// Processes launch input and paddle positioning in Update loop.
    /// </summary>
    private void Update()
    {
        if (!componentsValid) return;
        
        // Handle launch input if in appropriate state
        if (currentState.ShouldPollForInput())
        {
            HandleLaunchInput();
        }
        
        // Update paddle positioning if required
        if (currentState.RequiresPaddlePositioning())
        {
            PositionOnPaddle();
        }
    }
    
    #endregion
    
    #region Component Management
    
    /// <summary>
    /// Cache references to required physics components.
    /// </summary>
    private void CacheComponentReferences()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        
        if (rigidBody == null)
        {
            Debug.LogError($"[BallController] Rigidbody2D component missing on {gameObject.name}. Run 'Breakout/Setup/Create Ball GameObject' first.", this);
        }
        
        if (circleCollider == null)
        {
            Debug.LogError($"[BallController] CircleCollider2D component missing on {gameObject.name}. Run 'Breakout/Setup/Create Ball GameObject' first.", this);
        }
        
        Debug.Log($"[BallController] Component references cached for {gameObject.name}");
    }
    
    /// <summary>
    /// Validates that all required components are properly configured.
    /// </summary>
    private bool ValidateComponents()
    {
        componentsValid = rigidBody != null && circleCollider != null;
        
        if (!componentsValid)
        {
            Debug.LogWarning($"[BallController] Component validation failed. Ball physics may not function correctly.", this);
            return false;
        }
        
        // Validate Rigidbody2D configuration
        if (rigidBody.collisionDetectionMode != CollisionDetectionMode2D.Continuous)
        {
            Debug.LogWarning($"[BallController] Rigidbody2D should use Continuous collision detection to prevent tunneling.", this);
        }
        
        Debug.Log($"[BallController] Component validation successful");
        return true;
    }
    
    #endregion
    
    #region Launch Mechanics
    
    /// <summary>
    /// Initializes launch mechanics system and validates paddle reference.
    /// </summary>
    private void InitializeLaunchMechanics()
    {
        // Validate paddle reference
        ValidatePaddleReference();
        
        // Initialize launch state
        TransitionToState(BallLaunchState.Ready);
        
        // Set default launch direction
        launchDirection = defaultLaunchDirection.normalized;
        
        // Cache initial paddle position if available
        if (paddleTransform != null)
        {
            paddlePositionCache = paddleTransform.position;
        }
        
        Debug.Log($"[LaunchMechanics] Launch system initialized: State={currentState}, Direction={launchDirection}");
    }
    
    /// <summary>
    /// Validates paddle Transform reference and logs warnings if missing.
    /// </summary>
    private void ValidatePaddleReference()
    {
        paddleValidated = paddleTransform != null;
        
        if (!paddleValidated)
        {
            Debug.LogWarning($"[LaunchMechanics] Paddle Transform not assigned. Ball will launch from current position with default direction.", this);
            Debug.LogWarning($"💡 Assign paddle Transform in Inspector for proper launch positioning.");
        }
        else
        {
            Debug.Log($"[LaunchMechanics] Paddle reference validated: {paddleTransform.name}");
        }
    }
    
    /// <summary>
    /// Handles spacebar input detection for launch triggering.
    /// </summary>
    private void HandleLaunchInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isReadyToLaunch)
        {
            if (enableLaunchDebugging)
            {
                Debug.Log($"[LaunchMechanics] Launch input detected - triggering ball launch");
            }
            
            TransitionToState(BallLaunchState.Launching);
            CalculateLaunchDirection();
            ExecuteLaunch();
        }
    }
    
    /// <summary>
    /// Calculates launch direction based on paddle position and angle range.
    /// </summary>
    private void CalculateLaunchDirection()
    {
        if (paddleValidated && paddleTransform != null)
        {
            // Calculate launch angle based on ball position relative to paddle center
            Vector3 ballPosition = transform.position;
            Vector3 paddlePosition = paddleTransform.position;
            
            // Get horizontal offset from paddle center (normalized to -1 to 1)
            float paddleWidth = GetPaddleWidth();
            float horizontalOffset = (ballPosition.x - paddlePosition.x) / (paddleWidth * 0.5f);
            horizontalOffset = Mathf.Clamp(horizontalOffset, -1f, 1f);
            
            // Convert offset to launch angle
            float launchAngle = horizontalOffset * (launchAngleRange * 0.5f);
            
            // Create launch direction vector
            float radianAngle = launchAngle * Mathf.Deg2Rad;
            launchDirection = new Vector2(Mathf.Sin(radianAngle), Mathf.Cos(radianAngle)).normalized;
            
            if (enableLaunchDebugging)
            {
                Debug.Log($"[LaunchMechanics] Launch direction calculated: Offset={horizontalOffset:F2}, Angle={launchAngle:F1}°, Direction={launchDirection}");
            }
        }
        else
        {
            // Use default launch direction if no paddle reference
            launchDirection = defaultLaunchDirection.normalized;
            
            if (enableLaunchDebugging)
            {
                Debug.Log($"[LaunchMechanics] Using default launch direction: {launchDirection}");
            }
        }
    }
    
    /// <summary>
    /// Executes the ball launch with calculated direction and integration with velocity system.
    /// </summary>
    private void ExecuteLaunch()
    {
        if (!componentsValid)
        {
            Debug.LogWarning($"[LaunchMechanics] Cannot execute launch - components not valid", this);
            return;
        }
        
        // Get launch speed from BallData or use default
        float launchSpeed = ballData != null ? ballData.baseSpeed : 8f;
        
        // Create launch velocity vector
        Vector2 launchVelocity = GetLaunchVector() * launchSpeed;
        
        // Apply launch velocity using existing SetVelocity method (integrates with velocity management)
        SetVelocity(launchVelocity);
        
        // Transition to InPlay state
        TransitionToState(BallLaunchState.InPlay);
        
        if (enableLaunchDebugging)
        {
            Debug.Log($"[LaunchMechanics] Ball launched: Velocity={launchVelocity}, Speed={launchSpeed:F1}, State={currentState}");
        }
    }
    
    /// <summary>
    /// Manages state machine transitions with validation.
    /// </summary>
    /// <param name="newState">Target state to transition to</param>
    private void TransitionToState(BallLaunchState newState)
    {
        if (!currentState.CanTransitionTo(newState))
        {
            Debug.LogWarning($"[LaunchMechanics] Invalid state transition: {currentState} → {newState}", this);
            return;
        }
        
        BallLaunchState previousState = currentState;
        currentState = newState;
        
        // Handle state-specific logic
        switch (newState)
        {
            case BallLaunchState.Ready:
                isReadyToLaunch = true;
                // Stop ball movement when returning to ready state
                if (componentsValid)
                {
                    rigidBody.linearVelocity = Vector2.zero;
                }
                break;
                
            case BallLaunchState.Launching:
                isReadyToLaunch = false;
                break;
                
            case BallLaunchState.InPlay:
                isReadyToLaunch = false;
                break;
        }
        
        if (enableLaunchDebugging)
        {
            Debug.Log($"[LaunchMechanics] State transition: {previousState} → {newState} ({newState.GetDescription()})");
        }
    }
    
    /// <summary>
    /// Positions ball relative to paddle during ready state.
    /// </summary>
    private void PositionOnPaddle()
    {
        if (!paddleValidated || paddleTransform == null)
            return;
            
        // Update paddle position cache
        paddlePositionCache = paddleTransform.position;
        
        // Position ball on paddle with offset
        Vector3 targetPosition = new Vector3(
            paddlePositionCache.x,
            paddlePositionCache.y + paddleOffset,
            transform.position.z
        );
        
        transform.position = targetPosition;
    }
    
    /// <summary>
    /// Gets the launch vector based on calculated direction.
    /// </summary>
    /// <returns>Normalized launch direction vector</returns>
    private Vector2 GetLaunchVector()
    {
        return launchDirection.normalized;
    }
    
    /// <summary>
    /// Estimates paddle width for launch angle calculations.
    /// </summary>
    /// <returns>Estimated paddle width in world units</returns>
    private float GetPaddleWidth()
    {
        if (!paddleValidated || paddleTransform == null)
            return 2f; // Default paddle width fallback
            
        // Try to get width from collider or renderer
        Collider2D paddleCollider = paddleTransform.GetComponent<Collider2D>();
        if (paddleCollider != null)
        {
            return paddleCollider.bounds.size.x;
        }
        
        SpriteRenderer paddleRenderer = paddleTransform.GetComponent<SpriteRenderer>();
        if (paddleRenderer != null)
        {
            return paddleRenderer.bounds.size.x;
        }
        
        // Fallback to default width
        return 2f;
    }
    
    /// <summary>
    /// Resets ball to ready state for new launch.
    /// </summary>
    public void ResetForLaunch()
    {
        TransitionToState(BallLaunchState.Ready);
        ClearTargetSpeed();
        
        Debug.Log($"[LaunchMechanics] Ball reset for new launch");
    }
    
    /// <summary>
    /// Gets the current launch state.
    /// </summary>
    /// <returns>Current BallLaunchState</returns>
    public BallLaunchState GetLaunchState()
    {
        return currentState;
    }
    
    /// <summary>
    /// Sets the paddle Transform reference for launch positioning.
    /// </summary>
    /// <param name="paddle">Paddle Transform to use for positioning</param>
    public void SetPaddleReference(Transform paddle)
    {
        paddleTransform = paddle;
        ValidatePaddleReference();
        
        Debug.Log($"[LaunchMechanics] Paddle reference updated: {(paddle != null ? paddle.name : "null")}");
    }
    
    /// <summary>
    /// Configures launch mechanics parameters.
    /// </summary>
    /// <param name="angleRange">Launch angle range in degrees</param>
    /// <param name="offset">Paddle offset distance</param>
    /// <param name="defaultDirection">Default launch direction vector</param>
    public void ConfigureLaunchMechanics(float angleRange, float offset, Vector2 defaultDirection)
    {
        launchAngleRange = angleRange;
        paddleOffset = offset;
        defaultLaunchDirection = defaultDirection.normalized;
        launchDirection = defaultLaunchDirection;
        
        Debug.Log($"[LaunchMechanics] Configuration updated: AngleRange={angleRange}°, Offset={offset}, DefaultDirection={defaultDirection}");
    }
    
    #endregion
    
    #region Configuration
    
    /// <summary>
    /// Initialize BallData configuration and handle missing data gracefully.
    /// </summary>
    private void InitializeConfiguration()
    {
        if (ballData == null)
        {
            Debug.LogWarning($"[BallController] BallData not assigned. Using default physics configuration.", this);
            CreateDefaultBallData();
        }
        else
        {
            ballData.ValidateSpeedConstraints();
            Debug.Log($"[BallController] BallData configuration loaded: Base Speed = {ballData.baseSpeed}");
        }
    }
    
    /// <summary>
    /// Creates a default BallData configuration if none is assigned.
    /// </summary>
    private void CreateDefaultBallData()
    {
        ballData = new BallData();
        Debug.Log($"[BallController] Created default BallData configuration");
    }
    
    /// <summary>
    /// Initialize physics state tracking.
    /// </summary>
    private void InitializePhysicsState()
    {
        if (ballData != null)
        {
            ballData.ResetState();
        }
        
        isMoving = false;
        currentVelocity = Vector2.zero;
        
        Debug.Log($"[BallController] Physics state initialized");
    }
    
    #endregion
    
    #region Velocity Management
    
    /// <summary>
    /// Core velocity management system that ensures consistent ball speed and arcade physics behavior.
    /// </summary>
    private void ApplyVelocityManagement()
    {
        if (ballData == null) return;
        
        Vector2 currentPhysicsVelocity = rigidBody.linearVelocity;
        float currentSpeed = currentPhysicsVelocity.magnitude;
        
        // Skip if velocity is too small (ball is essentially stopped)
        if (currentSpeed < velocityNormalizationThreshold)
        {
            return;
        }
        
        // Determine target speed for management
        float managedTargetSpeed = GetTargetSpeedForManagement(currentSpeed);
        
        // Apply speed constraints from BallData
        managedTargetSpeed = Mathf.Clamp(managedTargetSpeed, ballData.minSpeed, ballData.maxSpeed);
        
        // Calculate speed difference for stabilization
        float speedDifference = Mathf.Abs(currentSpeed - managedTargetSpeed);
        
        // Apply stabilization if speed difference exceeds tolerance
        if (speedDifference > speedConstraintTolerance)
        {
            Vector2 normalizedDirection = currentPhysicsVelocity.normalized;
            
            if (maintainConstantSpeed)
            {
                // Immediate speed correction for arcade feel
                Vector2 correctedVelocity = normalizedDirection * managedTargetSpeed;
                rigidBody.linearVelocity = correctedVelocity;
                
                Debug.Log($"[VelocityManagement] Speed corrected: {currentSpeed:F2} → {managedTargetSpeed:F2}");
            }
            else
            {
                // Gradual speed adjustment using stabilization rate
                float adjustedSpeed = Mathf.Lerp(currentSpeed, managedTargetSpeed, speedStabilizationRate * Time.fixedDeltaTime);
                Vector2 adjustedVelocity = normalizedDirection * adjustedSpeed;
                rigidBody.linearVelocity = adjustedVelocity;
                
                Debug.Log($"[VelocityManagement] Speed adjusted: {currentSpeed:F2} → {adjustedSpeed:F2} (Target: {managedTargetSpeed:F2})");
            }
        }
    }
    
    /// <summary>
    /// Determines the target speed for velocity management based on current state.
    /// </summary>
    /// <param name="currentSpeed">Current ball speed</param>
    /// <returns>Target speed for management system</returns>
    private float GetTargetSpeedForManagement(float currentSpeed)
    {
        // Use explicit target speed if set
        if (hasTargetSpeed)
        {
            return targetSpeed;
        }
        
        // Use BallData base speed as default target
        if (ballData != null)
        {
            return ballData.baseSpeed;
        }
        
        // Fallback to current speed if no configuration available
        return currentSpeed;
    }
    
    /// <summary>
    /// Sets a specific target speed for velocity management.
    /// </summary>
    /// <param name="speed">Target speed to maintain</param>
    public void SetTargetSpeed(float speed)
    {
        if (ballData != null)
        {
            targetSpeed = Mathf.Clamp(speed, ballData.minSpeed, ballData.maxSpeed);
        }
        else
        {
            targetSpeed = speed;
        }
        
        hasTargetSpeed = true;
        Debug.Log($"[VelocityManagement] Target speed set to {targetSpeed:F2}");
    }
    
    /// <summary>
    /// Clears the target speed, allowing velocity management to use base speed.
    /// </summary>
    public void ClearTargetSpeed()
    {
        hasTargetSpeed = false;
        targetSpeed = 0f;
        Debug.Log($"[VelocityManagement] Target speed cleared, using base speed");
    }
    
    /// <summary>
    /// Enables or disables the velocity management system.
    /// </summary>
    /// <param name="enabled">Whether velocity management should be active</param>
    public void SetVelocityManagementEnabled(bool enabled)
    {
        velocityManagementEnabled = enabled;
        Debug.Log($"[VelocityManagement] Velocity management {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Configures velocity management parameters for different gameplay scenarios.
    /// </summary>
    /// <param name="maintainConstant">Whether to maintain perfectly constant speed</param>
    /// <param name="stabilizationRate">Rate of speed adjustment (if not maintaining constant)</param>
    /// <param name="tolerance">Speed difference tolerance before correction</param>
    public void ConfigureVelocityManagement(bool maintainConstant, float stabilizationRate = 5f, float tolerance = 0.05f)
    {
        maintainConstantSpeed = maintainConstant;
        speedStabilizationRate = stabilizationRate;
        speedConstraintTolerance = tolerance;
        
        Debug.Log($"[VelocityManagement] Configuration updated: Constant={maintainConstant}, Rate={stabilizationRate:F1}, Tolerance={tolerance:F3}");
    }
    
    #endregion
    
    #region Movement Methods
    
    /// <summary>
    /// Sets the ball's velocity with BallData constraint integration and velocity management.
    /// </summary>
    /// <param name="velocity">Desired velocity vector</param>
    public void SetVelocity(Vector2 velocity)
    {
        if (!componentsValid)
        {
            Debug.LogWarning($"[BallController] Cannot set velocity - components not valid", this);
            return;
        }
        
        // Apply BallData constraints if available
        Vector2 constrainedVelocity = velocity;
        if (ballData != null)
        {
            constrainedVelocity = ballData.ApplySpeedConstraints(velocity);
        }
        
        rigidBody.linearVelocity = constrainedVelocity;
        currentVelocity = constrainedVelocity;
        isMoving = constrainedVelocity.magnitude > 0.1f;
        
        // Update velocity management target if enabled
        if (velocityManagementEnabled && isMoving)
        {
            float newSpeed = constrainedVelocity.magnitude;
            if (newSpeed > velocityNormalizationThreshold)
            {
                SetTargetSpeed(newSpeed);
            }
        }
        
        Debug.Log($"[BallController] Velocity set to {constrainedVelocity} (Original: {velocity})");
    }
    
    /// <summary>
    /// Applies a force to the ball using Unity's physics system.
    /// </summary>
    /// <param name="force">Force vector to apply</param>
    public void AddForce(Vector2 force)
    {
        if (!componentsValid)
        {
            Debug.LogWarning($"[BallController] Cannot add force - components not valid", this);
            return;
        }
        
        rigidBody.AddForce(force, ForceMode2D.Impulse);
        
        // Apply constraints after force application
        if (ballData != null)
        {
            Vector2 constrainedVelocity = ballData.ApplySpeedConstraints(rigidBody.linearVelocity);
            rigidBody.linearVelocity = constrainedVelocity;
        }
        
        Debug.Log($"[BallController] Force applied: {force}");
    }
    
    /// <summary>
    /// Stops the ball by zeroing its velocity and clearing velocity management targets.
    /// </summary>
    public void Stop()
    {
        if (!componentsValid)
        {
            Debug.LogWarning($"[BallController] Cannot stop - components not valid", this);
            return;
        }
        
        rigidBody.linearVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
        isMoving = false;
        
        // Clear velocity management state
        ClearTargetSpeed();
        
        if (ballData != null)
        {
            ballData.currentVelocity = Vector2.zero;
        }
        
        Debug.Log($"[BallController] Ball stopped");
    }
    
    /// <summary>
    /// Returns whether the ball is currently in motion.
    /// </summary>
    /// <returns>True if ball is moving above threshold velocity</returns>
    public bool IsMoving()
    {
        return isMoving;
    }
    
    /// <summary>
    /// Gets the current velocity vector.
    /// </summary>
    /// <returns>Current velocity of the ball</returns>
    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }
    
    /// <summary>
    /// Gets the current speed (velocity magnitude).
    /// </summary>
    /// <returns>Current speed of the ball</returns>
    public float GetCurrentSpeed()
    {
        return currentVelocity.magnitude;
    }
    
    #endregion
    
    #region Physics Callbacks
    
    /// <summary>
    /// Handles collision events for debugging and future collision response systems.
    /// </summary>
    /// <param name="collision">Collision data from Unity physics system</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Update collision count if BallData available
        if (ballData != null)
        {
            ballData.collisionCount++;
        }
        
        // Log collision details for debugging
        string colliderName = collision.gameObject.name;
        Vector2 collisionPoint = collision.contacts[0].point;
        Vector2 collisionNormal = collision.contacts[0].normal;
        
        Debug.Log($"[BallController] Collision with {colliderName} at {collisionPoint}, Normal: {collisionNormal}");
        
        // Validate physics constraints after collision
        if (ballData != null && componentsValid)
        {
            Vector2 constrainedVelocity = ballData.ApplySpeedConstraints(rigidBody.linearVelocity);
            if (Vector2.Distance(constrainedVelocity, rigidBody.linearVelocity) > 0.1f)
            {
                rigidBody.linearVelocity = constrainedVelocity;
                Debug.Log($"[BallController] Velocity constrained after collision: {constrainedVelocity}");
            }
        }
    }
    
    /// <summary>
    /// Handles trigger events for special collision detection.
    /// </summary>
    /// <param name="other">Collider that triggered the event</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[BallController] Trigger event with {other.gameObject.name}");
    }
    
    #endregion
    
    #region Public Interface
    
    /// <summary>
    /// Gets the BallData configuration reference.
    /// </summary>
    /// <returns>Current BallData configuration</returns>
    public BallData GetBallData()
    {
        return ballData;
    }
    
    /// <summary>
    /// Sets a new BallData configuration.
    /// </summary>
    /// <param name="newBallData">New BallData configuration to use</param>
    public void SetBallData(BallData newBallData)
    {
        ballData = newBallData;
        if (ballData != null)
        {
            ballData.ValidateSpeedConstraints();
            Debug.Log($"[BallController] BallData configuration updated");
        }
    }
    
    /// <summary>
    /// Gets component references for external system integration.
    /// </summary>
    /// <returns>Tuple containing Rigidbody2D and CircleCollider2D references</returns>
    public (Rigidbody2D rigidBody, CircleCollider2D collider) GetComponentReferences()
    {
        return (rigidBody, circleCollider);
    }
    
    /// <summary>
    /// Validates that the controller is properly configured and ready to use.
    /// </summary>
    /// <returns>True if controller is ready for physics operations</returns>
    public bool IsReady()
    {
        return componentsValid && ballData != null;
    }
    
    #endregion
    
    #region Debug Information
    
    /// <summary>
    /// Provides debug information about the current controller state including launch mechanics.
    /// </summary>
    /// <returns>Debug information string</returns>
    public string GetDebugInfo()
    {
        if (!componentsValid)
        {
            return "BallController: Components not valid";
        }
        
        return $"BallController Debug Info:\n" +
               $"• Is Moving: {isMoving}\n" +
               $"• Current Velocity: {currentVelocity}\n" +
               $"• Current Speed: {GetCurrentSpeed():F2}\n" +
               $"• Collision Count: {(ballData != null ? ballData.collisionCount : 0)}\n" +
               $"• Components Valid: {componentsValid}\n" +
               $"• BallData Assigned: {ballData != null}\n" +
               $"• Velocity Management: {(velocityManagementEnabled ? "Enabled" : "Disabled")}\n" +
               $"• Target Speed: {(hasTargetSpeed ? targetSpeed.ToString("F2") : "Auto")}\n" +
               $"• Maintain Constant: {maintainConstantSpeed}\n" +
               $"• Speed Tolerance: {speedConstraintTolerance:F3}\n" +
               $"• Launch State: {currentState} ({currentState.GetDescription()})\n" +
               $"• Ready to Launch: {isReadyToLaunch}\n" +
               $"• Paddle Reference: {(paddleValidated ? paddleTransform.name : "Missing")}\n" +
               $"• Launch Direction: {launchDirection}\n" +
               $"• Launch Angle Range: {launchAngleRange:F1}°";
    }
    
    #endregion
    
    #region Editor Support
    
#if UNITY_EDITOR
    /// <summary>
    /// Draws debug information in the Scene view when selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!componentsValid) return;
        
        // Draw velocity vector
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)currentVelocity * 0.5f);
        
        // Draw collision bounds
        Gizmos.color = Color.yellow;
        if (circleCollider != null)
        {
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }
        
        // Draw launch mechanics debug information
        if (currentState == BallLaunchState.Ready && paddleValidated && paddleTransform != null)
        {
            // Draw line from paddle to ball
            Gizmos.color = Color.green;
            Gizmos.DrawLine(paddleTransform.position, transform.position);
            
            // Draw launch direction preview
            Gizmos.color = Color.cyan;
            Vector3 launchPreview = transform.position + (Vector3)launchDirection * 2f;
            Gizmos.DrawLine(transform.position, launchPreview);
            Gizmos.DrawWireSphere(launchPreview, 0.1f);
            
            // Draw launch angle range
            if (launchAngleRange > 0)
            {
                Gizmos.color = Color.blue;
                float halfRange = launchAngleRange * 0.5f;
                
                // Left angle
                float leftAngle = -halfRange * Mathf.Deg2Rad;
                Vector2 leftDir = new Vector2(Mathf.Sin(leftAngle), Mathf.Cos(leftAngle));
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)leftDir * 1.5f);
                
                // Right angle
                float rightAngle = halfRange * Mathf.Deg2Rad;
                Vector2 rightDir = new Vector2(Mathf.Sin(rightAngle), Mathf.Cos(rightAngle));
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)rightDir * 1.5f);
            }
        }
    }
#endif
    
    #endregion
}