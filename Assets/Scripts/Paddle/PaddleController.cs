using UnityEngine;

/// <summary>
/// Performance metrics data structure for monitoring paddle controller performance.
/// </summary>
[System.Serializable]
public struct PerformanceMetrics
{
    public float currentResponseTime;    // Current frame response time in ms
    public float averageResponseTime;    // Average response time over recent frames
    public bool performanceTargetsMet;   // Whether performance targets are being met
    public float targetResponseTime;     // Target response time in ms
    public float frameRate;              // Current frame rate
}

/// <summary>
/// Core PaddleController MonoBehaviour providing basic movement logic and component integration.
/// Serves as foundation for paddle movement with Transform and physics integration, designed for extension with input systems and boundary constraints.
/// </summary>
public class PaddleController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] 
    [Tooltip("Paddle configuration data controlling movement properties and constraints")]
    private PaddleData paddleData;
    
    [Header("Debug Information")]
    [SerializeField] 
    [Tooltip("Current paddle position in world space")]
    private Vector3 currentPosition;
    
    [SerializeField] 
    [Tooltip("Whether the controller has been properly initialized")]
    private bool isInitialized = false;
    
    [Header("Movement Smoothing")]
    [SerializeField]
    [Tooltip("Current smoothing velocity for SmoothDamp interpolation")]
    private float smoothingVelocity = 0f;
    
    [SerializeField]
    [Tooltip("Current acceleration multiplier from curve")]
    private float currentAcceleration = 1f;
    
    [SerializeField]
    [Tooltip("Movement progress for acceleration curve (0-1)")]
    private float movementProgress = 0f;
    
    [Header("Performance Monitoring")]
    [SerializeField]
    [Tooltip("Current frame response time in milliseconds")]
    private float currentResponseTime = 0f;
    
    [SerializeField]
    [Tooltip("Average response time over recent frames")]
    private float averageResponseTime = 0f;
    
    [SerializeField]
    [Tooltip("Whether performance targets are being met")]
    private bool performanceTargetsMet = true;
    
    [Header("Input State")]
    [SerializeField]
    [Tooltip("Currently active input method")]
    private InputMethod currentInputMethod = InputMethod.None;
    
    [SerializeField]
    [Tooltip("Current horizontal input value (-1 to 1)")]
    private float inputHorizontal = 0f;
    
    [SerializeField]
    [Tooltip("Whether input system is actively processing")]
    private bool inputActive = false;
    
    [Header("Boundary Constraints")]
    [SerializeField]
    [Tooltip("Effective left boundary considering paddle width")]
    private float effectiveLeftBoundary = -7.0f;
    
    [SerializeField]
    [Tooltip("Effective right boundary considering paddle width")]
    private float effectiveRightBoundary = 7.0f;
    
    [SerializeField]
    [Tooltip("Half-width of paddle for boundary calculations")]
    private float paddleHalfWidth = 1.0f;
    
    [SerializeField]
    [Tooltip("Whether boundaries were auto-detected from GameArea")]
    private bool boundariesAutoDetected = false;
    
    // Component References (cached for performance)
    private Transform paddleTransform;
    private BoxCollider2D paddleCollider;
    private SpriteRenderer paddleRenderer;
    private Camera mainCamera;
    private GameObject gameAreaContainer;
    
    // Movement state
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float movementStartTime;
    private Vector3 movementStartPosition;
    private float totalMovementDistance;
    
    // Performance monitoring
    private float frameStartTime;
    private float[] recentResponseTimes = new float[30]; // 30-frame rolling average
    private int responseTimeIndex = 0;
    private bool performanceInitialized = false;
    
    // Input system state
    private Vector3 lastMousePosition;
    private float lastInputTime;
    private bool keyboardInputDetected = false;
    private bool mouseInputDetected = false;
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize component references and validate setup.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
        ValidateSetup();
    }
    
    /// <summary>
    /// Complete initialization with PaddleData configuration and start performance monitoring.
    /// </summary>
    private void Start()
    {
        if (isInitialized)
        {
            InitializePaddleState();
            
            // Start performance monitoring session
            if (paddleData != null)
            {
                PerformanceProfiler.SetWebGLTargets(60f, paddleData.targetResponseTime);
                PerformanceProfiler.StartSession($"Paddle_{gameObject.name}");
            }
        }
    }
    
    /// <summary>
    /// Update paddle movement and input processing with performance monitoring.
    /// </summary>
    private void Update()
    {
        if (!isInitialized) return;
        
        // Start performance timing
        frameStartTime = Time.realtimeSinceStartup * 1000f; // Convert to milliseconds
        
        // Process input system
        ProcessInput();
        
        // Update movement if target position is set
        if (isMoving)
        {
            UpdateSmoothMovement();
        }
        
        // Update debug information (only when needed to avoid allocations)
        if (Time.frameCount % 5 == 0) // Update every 5 frames to reduce overhead
        {
            currentPosition = paddleTransform != null ? paddleTransform.position : Vector3.zero;
        }
        
        // Update performance monitoring
        UpdatePerformanceMetrics();
    }
    
    #endregion
    
    #region Component Management
    
    /// <summary>
    /// Initialize and cache component references for performance.
    /// </summary>
    private void InitializeComponents()
    {
        Debug.Log("[PaddleController] Initializing component references...");
        
        // Cache Transform reference
        paddleTransform = transform;
        
        // Cache BoxCollider2D reference
        paddleCollider = GetComponent<BoxCollider2D>();
        if (paddleCollider == null)
        {
            Debug.LogWarning("[PaddleController] BoxCollider2D component not found. Collision detection may not work properly.");
        }
        
        // Cache SpriteRenderer reference
        paddleRenderer = GetComponent<SpriteRenderer>();
        if (paddleRenderer == null)
        {
            Debug.LogWarning("[PaddleController] SpriteRenderer component not found. Visual representation may not work properly.");
        }
        
        // Cache Camera reference for mouse input
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        if (mainCamera == null)
        {
            Debug.LogWarning("[PaddleController] No camera found. Mouse input will be disabled.");
        }
        else
        {
            Debug.Log($"[PaddleController] Camera reference cached: {mainCamera.name}");
        }
        
        // Cache GameArea container reference for boundary detection
        gameAreaContainer = GameObject.Find("GameArea");
        if (gameAreaContainer == null)
        {
            gameAreaContainer = transform.parent?.gameObject;
        }
        if (gameAreaContainer != null)
        {
            Debug.Log($"[PaddleController] GameArea container found: {gameAreaContainer.name}");
        }
        else
        {
            Debug.LogWarning("[PaddleController] GameArea container not found. Using PaddleData boundaries.");
        }
        
        Debug.Log("[PaddleController] Component references initialized");
    }
    
    /// <summary>
    /// Validate component setup and PaddleData configuration.
    /// </summary>
    private void ValidateSetup()
    {
        Debug.Log("[PaddleController] Validating controller setup...");
        
        bool setupValid = true;
        
        // Validate Transform reference
        if (paddleTransform == null)
        {
            Debug.LogError("[PaddleController] Transform component is missing! This should never happen.");
            setupValid = false;
        }
        
        // Validate or create PaddleData
        if (paddleData == null)
        {
            Debug.LogWarning("[PaddleController] PaddleData not assigned. Creating default configuration.");
            paddleData = new PaddleData();
            paddleData.Initialize();
        }
        else
        {
            Debug.Log("[PaddleController] Using existing PaddleData configuration from Inspector.");
            Debug.Log($"[PaddleController] Inspector values - Speed: {paddleData.movementSpeed}, Acceleration: {paddleData.acceleration}");
            // Don't call Initialize() - it would override Inspector values
            // Just validate parameters without resetting state  
        }
        
        // Validate PaddleData parameters - use different method based on source
        bool parametersValid = false;
        if (paddleData == null)
        {
            Debug.LogError("[PaddleController] PaddleData is null after validation attempt!");
            setupValid = false;
        }
        else
        {
            // For Inspector-configured data, use validation that preserves settings
            parametersValid = paddleData.ValidateExistingConfiguration();
            if (!parametersValid)
            {
                Debug.LogWarning("[PaddleController] PaddleData contains invalid parameters. Using corrected values.");
            }
        }
        
        // Initialize boundary constraints
        InitializeBoundaryConstraints();
        
        isInitialized = setupValid;
        
        if (isInitialized)
        {
            Debug.Log("[PaddleController] Controller setup validation successful");
        }
        else
        {
            Debug.LogError("[PaddleController] Controller setup validation failed. Some functionality may not work properly.");
        }
    }
    
    /// <summary>
    /// Initialize paddle state based on PaddleData configuration.
    /// </summary>
    private void InitializePaddleState()
    {
        Debug.Log("[PaddleController] Initializing paddle state...");
        
        // Apply paddle dimensions to visual size
        ApplyPaddleDimensions();
        
        // Set initial position to center of playable area
        float centerX = paddleData.GetCenterPosition();
        Vector3 initialPosition = new Vector3(centerX, paddleTransform.position.y, paddleTransform.position.z);
        paddleTransform.position = initialPosition;
        
        // Update PaddleData current position
        paddleData.CurrentPosition = new Vector2(initialPosition.x, initialPosition.y);
        
        // Initialize movement state
        targetPosition = initialPosition;
        isMoving = false;
        
        Debug.Log($"[PaddleController] Paddle state initialized at position: {initialPosition}");
    }
    
    #endregion
    
    #region Visual Size Management
    
    /// <summary>
    /// Apply PaddleData dimensions to the paddle's visual representation.
    /// Updates Transform scale and collider size to match configured dimensions.
    /// </summary>
    private void ApplyPaddleDimensions()
    {
        if (paddleData == null || paddleTransform == null)
        {
            Debug.LogWarning("[PaddleController] Cannot apply dimensions - components not available");
            return;
        }
        
        Vector2 targetDimensions = paddleData.paddleDimensions;
        Debug.Log($"[PaddleController] Applying paddle dimensions: {targetDimensions.x:F2} x {targetDimensions.y:F2}");
        
        // Method 1: Scale the transform (works for most cases)
        ApplyScaleBasedDimensions(targetDimensions);
        
        // Method 2: Update collider size directly (ensures accurate collision)
        UpdateColliderDimensions(targetDimensions);
        
        // Recalculate paddle half-width for boundary calculations
        CalculatePaddleHalfWidth();
        
        // Force boundary recalculation with new paddle size
        InitializeBoundaryConstraints();
        
        Debug.Log($"[PaddleController] Paddle dimensions applied successfully");
    }
    
    /// <summary>
    /// Apply dimensions by scaling the Transform (affects visual and collider).
    /// </summary>
    /// <param name="targetDimensions">Target width and height</param>
    private void ApplyScaleBasedDimensions(Vector2 targetDimensions)
    {
        // Get the original size from the sprite or collider
        Vector2 originalSize = GetOriginalPaddleSize();
        
        if (originalSize.x <= 0 || originalSize.y <= 0)
        {
            Debug.LogWarning("[PaddleController] Cannot determine original paddle size for scaling");
            return;
        }
        
        // Calculate scale factors
        float scaleX = targetDimensions.x / originalSize.x;
        float scaleY = targetDimensions.y / originalSize.y;
        
        // Apply scale to transform
        Vector3 newScale = new Vector3(scaleX, scaleY, paddleTransform.localScale.z);
        paddleTransform.localScale = newScale;
        
        Debug.Log($"[PaddleController] Transform scaled to: {newScale} (factors: {scaleX:F2}x, {scaleY:F2}x)");
    }
    
    /// <summary>
    /// Update collider dimensions directly to ensure accurate collision detection.
    /// </summary>
    /// <param name="targetDimensions">Target width and height</param>
    private void UpdateColliderDimensions(Vector2 targetDimensions)
    {
        if (paddleCollider != null)
        {
            paddleCollider.size = targetDimensions;
            Debug.Log($"[PaddleController] Collider size updated to: {targetDimensions}");
        }
        else
        {
            Debug.LogWarning("[PaddleController] No collider available for size update");
        }
    }
    
    /// <summary>
    /// Get the original size of the paddle from sprite or collider.
    /// </summary>
    /// <returns>Original paddle dimensions</returns>
    private Vector2 GetOriginalPaddleSize()
    {
        // Try to get size from SpriteRenderer first
        if (paddleRenderer != null && paddleRenderer.sprite != null)
        {
            Sprite sprite = paddleRenderer.sprite;
            Vector2 spriteSize = new Vector2(
                sprite.bounds.size.x,
                sprite.bounds.size.y
            );
            Debug.Log($"[PaddleController] Original sprite size: {spriteSize}");
            return spriteSize;
        }
        
        // Fallback to collider size
        if (paddleCollider != null)
        {
            Vector2 colliderSize = paddleCollider.size;
            Debug.Log($"[PaddleController] Original collider size: {colliderSize}");
            return colliderSize;
        }
        
        // Emergency fallback - use current PaddleData dimensions
        Debug.LogWarning("[PaddleController] Cannot determine original size - using current PaddleData dimensions");
        return paddleData.paddleDimensions;
    }
    
    /// <summary>
    /// Public method to update paddle size at runtime.
    /// </summary>
    /// <param name="newDimensions">New paddle dimensions (width, height)</param>
    public void SetPaddleDimensions(Vector2 newDimensions)
    {
        if (paddleData == null)
        {
            Debug.LogError("[PaddleController] Cannot set dimensions - no PaddleData available");
            return;
        }
        
        if (newDimensions.x <= 0f || newDimensions.y <= 0f)
        {
            Debug.LogError("[PaddleController] Invalid dimensions - width and height must be greater than 0");
            return;
        }
        
        Debug.Log($"[PaddleController] Setting paddle dimensions from {paddleData.paddleDimensions} to {newDimensions}");
        
        // Update PaddleData
        paddleData.paddleDimensions = newDimensions;
        
        // Apply the visual changes
        ApplyPaddleDimensions();
        
        // Validate that the paddle still fits within boundaries
        if (!IsWithinBoundaries())
        {
            Debug.LogWarning("[PaddleController] Paddle size change caused it to exceed boundaries - adjusting position");
            float centerX = paddleData.GetCenterPosition();
            SetPosition(centerX);
        }
    }
    
    /// <summary>
    /// Get current paddle dimensions from PaddleData.
    /// </summary>
    /// <returns>Current paddle dimensions</returns>
    public Vector2 GetPaddleDimensions()
    {
        return paddleData != null ? paddleData.paddleDimensions : Vector2.zero;
    }
    
    #endregion
    
    #region Public Movement API
    
    /// <summary>
    /// Set paddle position to specific X coordinate with boundary constraints.
    /// </summary>
    /// <param name="x">Target X position</param>
    public void SetPosition(float x)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[PaddleController] Cannot set position - controller not properly initialized");
            return;
        }
        
        // Apply enhanced boundary constraints with paddle width consideration
        float constrainedX = ApplyBoundaryConstraints(x);
        
        // Update Transform position
        Vector3 newPosition = new Vector3(constrainedX, paddleTransform.position.y, paddleTransform.position.z);
        paddleTransform.position = newPosition;
        
        // Update PaddleData state
        paddleData.CurrentPosition = new Vector2(constrainedX, paddleTransform.position.y);
        
        // Stop any ongoing movement
        isMoving = false;
        targetPosition = newPosition;
        
        Debug.Log($"[PaddleController] Position set to: {newPosition}");
    }
    
    /// <summary>
    /// Move paddle towards target X coordinate with smooth interpolation and acceleration curves.
    /// </summary>
    /// <param name="targetX">Target X position</param>
    public void MoveTowards(float targetX)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[PaddleController] Cannot move - controller not properly initialized");
            return;
        }
        
        // Apply enhanced boundary constraints to target
        float constrainedTargetX = ApplyBoundaryConstraints(targetX);
        
        // Initialize smooth movement parameters
        Vector3 currentPos = paddleTransform.position;
        targetPosition = new Vector3(constrainedTargetX, currentPos.y, currentPos.z);
        
        // Reset smoothing state for new movement
        movementStartTime = Time.time;
        movementStartPosition = currentPos;
        totalMovementDistance = Mathf.Abs(constrainedTargetX - currentPos.x);
        smoothingVelocity = 0f;
        movementProgress = 0f;
        
        isMoving = true;
        
        Debug.Log($"[PaddleController] Moving towards: {targetPosition} (Distance: {totalMovementDistance:F2})");
    }
    
    /// <summary>
    /// Get current paddle position in world space.
    /// </summary>
    /// <returns>Current paddle position</returns>
    public Vector3 GetCurrentPosition()
    {
        return paddleTransform != null ? paddleTransform.position : Vector3.zero;
    }
    
    /// <summary>
    /// Stop all paddle movement immediately.
    /// </summary>
    public void Stop()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[PaddleController] Cannot stop - controller not properly initialized");
            return;
        }
        
        isMoving = false;
        targetPosition = paddleTransform.position;
        paddleData.CurrentVelocity = 0f;
        
        Debug.Log("[PaddleController] Movement stopped");
    }
    
    #endregion
    
    #region Movement Implementation
    
    /// <summary>
    /// Update smooth movement towards target position with acceleration curves and performance optimization.
    /// </summary>
    private void UpdateSmoothMovement()
    {
        if (!isMoving || paddleTransform == null) return;
        
        Vector3 currentPos = paddleTransform.position;
        float distanceToTarget = Mathf.Abs(targetPosition.x - currentPos.x);
        
        // Stop if we're close enough to target
        if (distanceToTarget < 0.01f)
        {
            paddleTransform.position = targetPosition;
            paddleData.CurrentPosition = new Vector2(targetPosition.x, targetPosition.y);
            paddleData.CurrentVelocity = 0f;
            smoothingVelocity = 0f;
            isMoving = false;
            return;
        }
        
        // Calculate movement progress for acceleration curve
        if (totalMovementDistance > 0f)
        {
            float distanceTraveled = Mathf.Abs(currentPos.x - movementStartPosition.x);
            movementProgress = Mathf.Clamp01(distanceTraveled / totalMovementDistance);
        }
        
        // Apply acceleration curve from PaddleData
        currentAcceleration = paddleData.accelerationCurve.Evaluate(movementProgress);
        
        // Calculate target speed with acceleration multiplier
        float targetSpeed = paddleData.movementSpeed * currentAcceleration;
        
        // Use SmoothDamp for smooth interpolation
        float newX = Mathf.SmoothDamp(
            currentPos.x, 
            targetPosition.x, 
            ref smoothingVelocity, 
            paddleData.smoothTime,
            targetSpeed,
            Time.deltaTime
        );
        
        // Update position
        Vector3 newPosition = new Vector3(newX, currentPos.y, currentPos.z);
        paddleTransform.position = newPosition;
        
        // Update PaddleData state
        paddleData.CurrentPosition = new Vector2(newX, currentPos.y);
        paddleData.CurrentVelocity = smoothingVelocity;
    }
    
    #endregion
    
    #region Component Access
    
    /// <summary>
    /// Get reference to paddle's BoxCollider2D component.
    /// </summary>
    /// <returns>BoxCollider2D component or null if not available</returns>
    public BoxCollider2D GetPaddleCollider()
    {
        return paddleCollider;
    }
    
    /// <summary>
    /// Get reference to paddle's SpriteRenderer component.
    /// </summary>
    /// <returns>SpriteRenderer component or null if not available</returns>
    public SpriteRenderer GetPaddleRenderer()
    {
        return paddleRenderer;
    }
    
    /// <summary>
    /// Get reference to PaddleData configuration.
    /// </summary>
    /// <returns>PaddleData configuration object</returns>
    public PaddleData GetPaddleData()
    {
        return paddleData;
    }
    
    /// <summary>
    /// Set PaddleData configuration (useful for runtime configuration changes).
    /// </summary>
    /// <param name="newPaddleData">New PaddleData configuration</param>
    public void SetPaddleData(PaddleData newPaddleData)
    {
        if (newPaddleData != null)
        {
            paddleData = newPaddleData;
            paddleData.ValidateParameters();
            Debug.Log("[PaddleController] PaddleData configuration updated");
        }
        else
        {
            Debug.LogWarning("[PaddleController] Cannot set null PaddleData");
        }
    }
    
    #endregion
    
    #region State Queries
    
    /// <summary>
    /// Check if paddle is currently moving.
    /// </summary>
    /// <returns>True if paddle is moving towards a target</returns>
    public bool IsMoving()
    {
        return isMoving;
    }
    
    /// <summary>
    /// Check if controller is properly initialized and ready for use.
    /// </summary>
    /// <returns>True if controller is initialized</returns>
    public bool IsInitialized()
    {
        return isInitialized;
    }
    
    /// <summary>
    /// Get distance to current movement target.
    /// </summary>
    /// <returns>Distance to target, or 0 if not moving</returns>
    public float GetDistanceToTarget()
    {
        if (!isMoving || paddleTransform == null) return 0f;
        return Mathf.Abs(targetPosition.x - paddleTransform.position.x);
    }
    
    /// <summary>
    /// Check if paddle is within playable boundaries.
    /// </summary>
    /// <returns>True if paddle is within boundaries</returns>
    public bool IsWithinBoundaries()
    {
        if (paddleData == null || paddleTransform == null) return false;
        return IsPositionWithinBounds(paddleTransform.position.x);
    }
    
    #endregion
    
    #region Boundary Constraint System
    
    /// <summary>
    /// Initialize boundary constraints using GameArea detection and PaddleData configuration.
    /// </summary>
    private void InitializeBoundaryConstraints()
    {
        Debug.Log("[PaddleController] Initializing boundary constraints...");
        
        // Calculate paddle half-width from collider or PaddleData
        CalculatePaddleHalfWidth();
        
        // Attempt GameArea boundary detection
        bool gameAreaDetected = DetectGameAreaBoundaries();
        
        // Fallback to PaddleData boundaries if GameArea detection failed
        if (!gameAreaDetected)
        {
            UsePaddleDataBoundaries();
        }
        
        // Validate and log boundary setup
        ValidateBoundarySetup();
        
        Debug.Log($"[PaddleController] Boundary constraints initialized: Left={effectiveLeftBoundary:F2}, Right={effectiveRightBoundary:F2}");
    }
    
    /// <summary>
    /// Calculate paddle half-width for accurate boundary calculations.
    /// </summary>
    private void CalculatePaddleHalfWidth()
    {
        if (paddleCollider != null)
        {
            // Use collider size for accurate paddle width
            paddleHalfWidth = paddleCollider.size.x * 0.5f;
            Debug.Log($"[PaddleController] Paddle half-width calculated from collider: {paddleHalfWidth:F2}");
        }
        else if (paddleData != null)
        {
            // Fallback to PaddleData dimensions
            paddleHalfWidth = paddleData.paddleDimensions.x * 0.5f;
            Debug.Log($"[PaddleController] Paddle half-width calculated from PaddleData: {paddleHalfWidth:F2}");
        }
        else
        {
            // Safe default fallback
            paddleHalfWidth = 1.0f;
            Debug.LogWarning("[PaddleController] Using default paddle half-width: 1.0");
        }
    }
    
    /// <summary>
    /// Attempt to detect boundaries from GameArea container.
    /// </summary>
    /// <returns>True if GameArea boundaries were detected successfully</returns>
    private bool DetectGameAreaBoundaries()
    {
        if (gameAreaContainer == null) return false;
        
        // Try to get boundaries from GameArea collider
        Collider2D gameAreaCollider = gameAreaContainer.GetComponent<Collider2D>();
        if (gameAreaCollider != null)
        {
            Bounds bounds = gameAreaCollider.bounds;
            effectiveLeftBoundary = bounds.min.x + paddleHalfWidth;
            effectiveRightBoundary = bounds.max.x - paddleHalfWidth;
            boundariesAutoDetected = true;
            
            Debug.Log($"[PaddleController] GameArea boundaries detected from collider: {bounds.min.x:F2} to {bounds.max.x:F2}");
            return true;
        }
        
        // Try to detect from child objects (walls, boundaries)
        Transform[] boundaries = gameAreaContainer.GetComponentsInChildren<Transform>();
        float leftmostX = float.MaxValue;
        float rightmostX = float.MinValue;
        bool foundBoundaries = false;
        
        foreach (Transform boundary in boundaries)
        {
            if (boundary.name.ToLower().Contains("wall") || boundary.name.ToLower().Contains("boundary"))
            {
                leftmostX = Mathf.Min(leftmostX, boundary.position.x);
                rightmostX = Mathf.Max(rightmostX, boundary.position.x);
                foundBoundaries = true;
            }
        }
        
        if (foundBoundaries)
        {
            effectiveLeftBoundary = leftmostX + paddleHalfWidth;
            effectiveRightBoundary = rightmostX - paddleHalfWidth;
            boundariesAutoDetected = true;
            
            Debug.Log($"[PaddleController] GameArea boundaries detected from child objects: {leftmostX:F2} to {rightmostX:F2}");
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Use PaddleData boundary configuration as fallback.
    /// </summary>
    private void UsePaddleDataBoundaries()
    {
        if (paddleData != null)
        {
            effectiveLeftBoundary = paddleData.leftBoundary + paddleHalfWidth;
            effectiveRightBoundary = paddleData.rightBoundary - paddleHalfWidth;
            boundariesAutoDetected = false;
            
            Debug.Log("[PaddleController] Using PaddleData boundaries with paddle width offset");
        }
        else
        {
            // Emergency fallback values
            effectiveLeftBoundary = -7.0f;
            effectiveRightBoundary = 7.0f;
            boundariesAutoDetected = false;
            
            Debug.LogWarning("[PaddleController] Using emergency fallback boundaries: -7.0 to 7.0");
        }
    }
    
    /// <summary>
    /// Apply boundary constraints to a target X position.
    /// </summary>
    /// <param name="targetX">Target X position</param>
    /// <returns>Constrained X position within boundaries</returns>
    private float ApplyBoundaryConstraints(float targetX)
    {
        float constrainedX = Mathf.Clamp(targetX, effectiveLeftBoundary, effectiveRightBoundary);
        
        // Handle boundary collision feedback
        if (constrainedX != targetX)
        {
            HandleBoundaryCollision(targetX, constrainedX);
        }
        
        return constrainedX;
    }
    
    /// <summary>
    /// Check if a position is within the effective boundaries.
    /// </summary>
    /// <param name="xPosition">X position to check</param>
    /// <returns>True if position is within boundaries</returns>
    private bool IsPositionWithinBounds(float xPosition)
    {
        return xPosition >= effectiveLeftBoundary && xPosition <= effectiveRightBoundary;
    }
    
    /// <summary>
    /// Handle boundary collision events for debugging and feedback.
    /// </summary>
    /// <param name="attemptedX">Original attempted position</param>
    /// <param name="clampedX">Clamped position after constraint</param>
    private void HandleBoundaryCollision(float attemptedX, float clampedX)
    {
        string boundaryType = (attemptedX < clampedX) ? "Left" : "Right";
        Debug.Log($"[PaddleController] {boundaryType} boundary collision: {attemptedX:F2} → {clampedX:F2}");
        
        // Could add visual/audio feedback here in the future
        // Could trigger haptic feedback for controllers
        // Could add particle effects or screen shake
    }
    
    /// <summary>
    /// Validate boundary setup and configuration.
    /// </summary>
    private void ValidateBoundarySetup()
    {
        if (effectiveRightBoundary <= effectiveLeftBoundary)
        {
            Debug.LogError("[PaddleController] Invalid boundary setup: Right boundary must be greater than left boundary!");
            
            // Emergency correction
            effectiveLeftBoundary = -7.0f;
            effectiveRightBoundary = 7.0f;
            Debug.LogWarning("[PaddleController] Applied emergency boundary correction");
        }
        
        float playableWidth = effectiveRightBoundary - effectiveLeftBoundary;
        if (playableWidth < paddleHalfWidth * 4) // Minimum reasonable playable area
        {
            Debug.LogWarning($"[PaddleController] Playable area ({playableWidth:F2}) may be too narrow for paddle movement");
        }
        
        Debug.Log($"[PaddleController] Boundary validation complete. Playable width: {playableWidth:F2}");
    }
    
    #endregion
    
    #region Input System
    
    /// <summary>
    /// Process all input methods and apply movement based on active input.
    /// </summary>
    private void ProcessInput()
    {
        if (!isInitialized || paddleData == null) return;
        
        // Reset input detection flags
        keyboardInputDetected = false;
        mouseInputDetected = false;
        
        // Handle keyboard input
        if (paddleData.enableKeyboardInput)
        {
            HandleKeyboardInput();
        }
        
        // Handle mouse input
        if (paddleData.enableMouseInput && mainCamera != null)
        {
            HandleMouseInput();
        }
        
        // Detect input method switching
        DetectInputMethodSwitch();
        
        // Apply movement input if any method is active
        if (inputActive && Mathf.Abs(inputHorizontal) > 0.01f)
        {
            ApplyMovementInput(inputHorizontal);
            lastInputTime = Time.time;
        }
        else
        {
            inputActive = false;
            inputHorizontal = 0f;
        }
        
        // Update PaddleData input state
        paddleData.ActiveInputMethod = currentInputMethod;
        paddleData.InputValue = inputHorizontal;
    }
    
    /// <summary>
    /// Handle keyboard input for paddle movement.
    /// </summary>
    private void HandleKeyboardInput()
    {
        float keyboardInput = 0f;
        
        // Check for A/D keys
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            keyboardInput -= 1f;
            keyboardInputDetected = true;
        }
        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            keyboardInput += 1f;
            keyboardInputDetected = true;
        }
        
        // Apply keyboard input if detected
        if (keyboardInputDetected)
        {
            inputHorizontal = keyboardInput;
            inputActive = true;
            
            // Apply input sensitivity
            inputHorizontal = paddleData.ApplyInputSensitivity(inputHorizontal);
        }
    }
    
    /// <summary>
    /// Handle mouse input for paddle movement with screen-to-world conversion.
    /// </summary>
    private void HandleMouseInput()
    {
        Vector3 mousePosition = Input.mousePosition;
        
        // Check if mouse has moved significantly
        if (Vector3.Distance(mousePosition, lastMousePosition) > 1f)
        {
            mouseInputDetected = true;
            lastMousePosition = mousePosition;
            
            // Convert screen position to world position
            Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(mainCamera.transform.position.z)));
            
            // Calculate target X position with boundary constraints
            Vector2 constrainedPosition = paddleData.ApplyBoundaryConstraints(new Vector2(worldMousePosition.x, paddleTransform.position.y));
            
            // Calculate input value based on current position difference
            float currentX = paddleTransform.position.x;
            float targetX = constrainedPosition.x;
            float distance = targetX - currentX;
            
            // Normalize distance to input value (-1 to 1)
            float maxDistance = paddleData.GetPlayableWidth() * 0.5f;
            if (maxDistance > 0)
            {
                inputHorizontal = Mathf.Clamp(distance / maxDistance, -1f, 1f);
                inputHorizontal = paddleData.ApplyInputSensitivity(inputHorizontal);
                inputActive = true;
            }
        }
    }
    
    /// <summary>
    /// Detect and handle input method switching.
    /// </summary>
    private void DetectInputMethodSwitch()
    {
        InputMethod previousMethod = currentInputMethod;
        
        // Determine active input method based on detection flags
        if (keyboardInputDetected && !mouseInputDetected)
        {
            currentInputMethod = InputMethod.Keyboard;
        }
        else if (mouseInputDetected && !keyboardInputDetected)
        {
            currentInputMethod = InputMethod.Mouse;
        }
        else if (keyboardInputDetected && mouseInputDetected)
        {
            // Keyboard has priority in simultaneous input
            currentInputMethod = InputMethod.Keyboard;
        }
        else if (!inputActive)
        {
            currentInputMethod = InputMethod.None;
        }
        
        // Log input method changes
        if (previousMethod != currentInputMethod && currentInputMethod != InputMethod.None)
        {
            Debug.Log($"[PaddleController] Input method switched to: {currentInputMethod}");
        }
    }
    
    /// <summary>
    /// Apply movement input to paddle position with sensitivity, boundary constraints, and smooth interpolation.
    /// </summary>
    /// <param name="input">Normalized input value (-1 to 1)</param>
    private void ApplyMovementInput(float input)
    {
        if (paddleTransform == null || paddleData == null) return;
        
        Vector3 currentPos = paddleTransform.position;
        
        // For direct input, use immediate smoothed movement
        if (paddleData.enableSmoothInput)
        {
            // Calculate target position based on input
            float inputDelta = input * paddleData.movementSpeed * Time.deltaTime;
            float targetX = currentPos.x + inputDelta;
            
            // Apply boundary constraints
            float constrainedTargetX = ApplyBoundaryConstraints(targetX);
            
            // Use smooth interpolation for input-based movement
            float smoothedX = Mathf.SmoothDamp(
                currentPos.x,
                constrainedTargetX,
                ref smoothingVelocity,
                paddleData.inputSmoothTime,
                paddleData.movementSpeed,
                Time.deltaTime
            );
            
            Vector3 newPosition = new Vector3(smoothedX, currentPos.y, currentPos.z);
            paddleTransform.position = newPosition;
            
            // Update state
            paddleData.CurrentPosition = new Vector2(smoothedX, currentPos.y);
            paddleData.CurrentVelocity = smoothingVelocity;
        }
        else
        {
            // Direct movement without smoothing for maximum responsiveness
            float movementDelta = input * paddleData.movementSpeed * Time.deltaTime;
            float newX = currentPos.x + movementDelta;
            float constrainedX = ApplyBoundaryConstraints(newX);
            
            Vector3 newPosition = new Vector3(constrainedX, currentPos.y, currentPos.z);
            paddleTransform.position = newPosition;
            
            // Update state
            paddleData.CurrentPosition = new Vector2(constrainedX, currentPos.y);
            paddleData.CurrentVelocity = movementDelta / Time.deltaTime;
        }
        
        // Stop any ongoing movement-based animation
        isMoving = false;
        targetPosition = paddleTransform.position;
    }
    
    #endregion
    
    #region Input System Public API
    
    /// <summary>
    /// Get the currently active input method.
    /// </summary>
    /// <returns>Current input method</returns>
    public InputMethod GetActiveInputMethod()
    {
        return currentInputMethod;
    }
    
    /// <summary>
    /// Get the current input value from the active input method.
    /// </summary>
    /// <returns>Input value (-1 to 1)</returns>
    public float GetCurrentInputValue()
    {
        return inputHorizontal;
    }
    
    /// <summary>
    /// Check if input system is currently active.
    /// </summary>
    /// <returns>True if input is being processed</returns>
    public bool IsInputActive()
    {
        return inputActive;
    }
    
    /// <summary>
    /// Get time since last input was received.
    /// </summary>
    /// <returns>Time since last input in seconds</returns>
    public float GetTimeSinceLastInput()
    {
        return Time.time - lastInputTime;
    }
    
    /// <summary>
    /// Enable or disable keyboard input processing.
    /// </summary>
    /// <param name="enabled">Whether keyboard input should be enabled</param>
    public void SetKeyboardInputEnabled(bool enabled)
    {
        if (paddleData != null)
        {
            paddleData.enableKeyboardInput = enabled;
            Debug.Log($"[PaddleController] Keyboard input {(enabled ? "enabled" : "disabled")}");
        }
    }
    
    /// <summary>
    /// Enable or disable mouse input processing.
    /// </summary>
    /// <param name="enabled">Whether mouse input should be enabled</param>
    public void SetMouseInputEnabled(bool enabled)
    {
        if (paddleData != null)
        {
            paddleData.enableMouseInput = enabled;
            Debug.Log($"[PaddleController] Mouse input {(enabled ? "enabled" : "disabled")}");
        }
    }
    
    /// <summary>
    /// Force a specific input method (for testing or special cases).
    /// </summary>
    /// <param name="method">Input method to force</param>
    public void ForceInputMethod(InputMethod method)
    {
        currentInputMethod = method;
        Debug.Log($"[PaddleController] Input method forced to: {method}");
    }
    
    #endregion
    
    #region Performance Monitoring
    
    /// <summary>
    /// Update performance metrics and monitoring with PerformanceProfiler integration.
    /// </summary>
    private void UpdatePerformanceMetrics()
    {
        // Calculate current frame response time
        currentResponseTime = (Time.realtimeSinceStartup * 1000f) - frameStartTime;
        
        // Record to PerformanceProfiler
        PerformanceProfiler.RecordFrameTime($"Paddle_{gameObject.name}", Time.deltaTime);
        PerformanceProfiler.RecordResponseTime($"Paddle_{gameObject.name}", currentResponseTime);
        
        // Update rolling average (optimized to avoid per-frame allocations)
        if (!performanceInitialized)
        {
            // Initialize all values
            for (int i = 0; i < recentResponseTimes.Length; i++)
            {
                recentResponseTimes[i] = currentResponseTime;
            }
            performanceInitialized = true;
        }
        else
        {
            // Update rolling buffer
            recentResponseTimes[responseTimeIndex] = currentResponseTime;
            responseTimeIndex = (responseTimeIndex + 1) % recentResponseTimes.Length;
        }
        
        // Calculate average response time (only every few frames to reduce overhead)
        if (Time.frameCount % 10 == 0)
        {
            float total = 0f;
            for (int i = 0; i < recentResponseTimes.Length; i++)
            {
                total += recentResponseTimes[i];
            }
            averageResponseTime = total / recentResponseTimes.Length;
            
            // Check performance targets
            performanceTargetsMet = averageResponseTime <= paddleData.targetResponseTime;
            
            // Log performance warnings if targets not met (throttled logging)
            if (!performanceTargetsMet && Time.time - lastInputTime > 1f)
            {
                Debug.LogWarning($"[PaddleController] Performance target not met: {averageResponseTime:F2}ms > {paddleData.targetResponseTime:F2}ms");
            }
        }
    }
    
    /// <summary>
    /// Get current performance metrics.
    /// </summary>
    /// <returns>Performance metrics data</returns>
    public PerformanceMetrics GetPerformanceMetrics()
    {
        return new PerformanceMetrics
        {
            currentResponseTime = this.currentResponseTime,
            averageResponseTime = this.averageResponseTime,
            performanceTargetsMet = this.performanceTargetsMet,
            targetResponseTime = paddleData != null ? paddleData.targetResponseTime : 50f,
            frameRate = 1f / Time.deltaTime
        };
    }
    
    /// <summary>
    /// Force performance metrics reset and restart PerformanceProfiler session.
    /// </summary>
    public void ResetPerformanceMetrics()
    {
        performanceInitialized = false;
        responseTimeIndex = 0;
        currentResponseTime = 0f;
        averageResponseTime = 0f;
        performanceTargetsMet = true;
        
        // Restart PerformanceProfiler session
        string sessionName = $"Paddle_{gameObject.name}";
        PerformanceProfiler.StopSession(sessionName);
        PerformanceProfiler.StartSession(sessionName);
        
        Debug.Log("[PaddleController] Performance metrics reset and profiler session restarted");
    }
    
    #endregion
    
    #region Movement Smoothing API
    
    /// <summary>
    /// Get current smoothing velocity.
    /// </summary>
    /// <returns>Smoothing velocity value</returns>
    public float GetSmoothingVelocity()
    {
        return smoothingVelocity;
    }
    
    /// <summary>
    /// Get current acceleration multiplier from curve.
    /// </summary>
    /// <returns>Current acceleration multiplier</returns>
    public float GetCurrentAcceleration()
    {
        return currentAcceleration;
    }
    
    /// <summary>
    /// Get movement progress for acceleration curve evaluation.
    /// </summary>
    /// <returns>Movement progress (0-1)</returns>
    public float GetMovementProgress()
    {
        return movementProgress;
    }
    
    /// <summary>
    /// Enable or disable smooth input processing.
    /// </summary>
    /// <param name="enabled">Whether smooth input should be enabled</param>
    public void SetSmoothInputEnabled(bool enabled)
    {
        if (paddleData != null)
        {
            paddleData.enableSmoothInput = enabled;
            Debug.Log($"[PaddleController] Smooth input {(enabled ? "enabled" : "disabled")}");
        }
    }
    
    /// <summary>
    /// Test smooth movement performance with comprehensive analysis.
    /// </summary>
    /// <param name="distance">Test movement distance</param>
    /// <param name="targetTime">Expected completion time</param>
    public void TestSmoothMovementPerformance(float distance, float targetTime)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[PaddleController] Cannot test performance - controller not initialized");
            return;
        }
        
        Vector3 currentPos = paddleTransform.position;
        float targetX = currentPos.x + distance;
        
        Debug.Log($"[PaddleController] Starting performance test: {distance:F2} units in {targetTime:F2}s");
        
        // Use PerformanceProfiler for comprehensive testing
        string testResults = PerformanceProfiler.TestPaddlePerformance(this, targetTime);
        Debug.Log(testResults);
        
        // Reset performance metrics for clean test
        ResetPerformanceMetrics();
        
        // Start movement
        MoveTowards(targetX);
    }
    
    #endregion
    
    #region Boundary Constraint Public API
    
    /// <summary>
    /// Get the effective left boundary considering paddle width.
    /// </summary>
    /// <returns>Left boundary X position</returns>
    public float GetEffectiveLeftBoundary()
    {
        return effectiveLeftBoundary;
    }
    
    /// <summary>
    /// Get the effective right boundary considering paddle width.
    /// </summary>
    /// <returns>Right boundary X position</returns>
    public float GetEffectiveRightBoundary()
    {
        return effectiveRightBoundary;
    }
    
    /// <summary>
    /// Get the playable width between boundaries.
    /// </summary>
    /// <returns>Playable width in units</returns>
    public float GetPlayableWidth()
    {
        return effectiveRightBoundary - effectiveLeftBoundary;
    }
    
    /// <summary>
    /// Get the paddle half-width used for boundary calculations.
    /// </summary>
    /// <returns>Paddle half-width in units</returns>
    public float GetPaddleHalfWidth()
    {
        return paddleHalfWidth;
    }
    
    /// <summary>
    /// Check if boundaries were automatically detected from GameArea.
    /// </summary>
    /// <returns>True if boundaries were auto-detected</returns>
    public bool AreBoundariesAutoDetected()
    {
        return boundariesAutoDetected;
    }
    
    /// <summary>
    /// Force boundary recalculation (useful after GameArea changes).
    /// </summary>
    public void RecalculateBoundaries()
    {
        Debug.Log("[PaddleController] Forcing boundary recalculation...");
        InitializeBoundaryConstraints();
    }
    
    /// <summary>
    /// Manually set effective boundaries (overrides auto-detection).
    /// </summary>
    /// <param name="leftBoundary">Left boundary X position</param>
    /// <param name="rightBoundary">Right boundary X position</param>
    public void SetManualBoundaries(float leftBoundary, float rightBoundary)
    {
        if (rightBoundary <= leftBoundary)
        {
            Debug.LogError("[PaddleController] Invalid boundaries: Right must be greater than left");
            return;
        }
        
        effectiveLeftBoundary = leftBoundary + paddleHalfWidth;
        effectiveRightBoundary = rightBoundary - paddleHalfWidth;
        boundariesAutoDetected = false;
        
        Debug.Log($"[PaddleController] Manual boundaries set: {effectiveLeftBoundary:F2} to {effectiveRightBoundary:F2}");
        
        // Validate current position within new boundaries
        if (paddleTransform != null && !IsPositionWithinBounds(paddleTransform.position.x))
        {
            float constrainedX = ApplyBoundaryConstraints(paddleTransform.position.x);
            SetPosition(constrainedX);
            Debug.Log("[PaddleController] Paddle position adjusted to new boundaries");
        }
    }
    
    /// <summary>
    /// Test boundary constraints with a specific position.
    /// </summary>
    /// <param name="testX">X position to test</param>
    /// <returns>Constrained X position</returns>
    public float TestBoundaryConstraints(float testX)
    {
        return ApplyBoundaryConstraints(testX);
    }
    
    /// <summary>
    /// Generate comprehensive performance report using PerformanceProfiler.
    /// </summary>
    /// <returns>Detailed performance analysis</returns>
    public string GetPerformanceReport()
    {
        string sessionName = $"Paddle_{gameObject.name}";
        
        // Stop current session to get report, then restart
        string report = PerformanceProfiler.StopSession(sessionName);
        PerformanceProfiler.StartSession(sessionName);
        
        return report;
    }
    
    #endregion
    
    #region Debug and Validation
    
    /// <summary>
    /// Get comprehensive status information for debugging including performance metrics.
    /// </summary>
    /// <returns>Status information string</returns>
    public string GetStatusInfo()
    {
        if (!isInitialized) return "PaddleController: Not Initialized";
        
        PerformanceMetrics metrics = GetPerformanceMetrics();
        
        return $"PaddleController Status:\\n" +
               $"• Position: {GetCurrentPosition()}\\n" +
               $"• Target: {targetPosition}\\n" +
               $"• Moving: {isMoving}\\n" +
               $"• Distance to Target: {GetDistanceToTarget():F3}\\n" +
               $"• Within Boundaries: {IsWithinBoundaries()}\\n" +
               $"• Current Velocity: {paddleData.CurrentVelocity:F2}\\n" +
               $"• Smoothing Velocity: {smoothingVelocity:F2}\\n" +
               $"• Current Acceleration: {currentAcceleration:F2}x\\n" +
               $"• Movement Progress: {movementProgress:F2}\\n" +
               $"• Input Method: {currentInputMethod}\\n" +
               $"• Input Value: {inputHorizontal:F3}\\n" +
               $"• Input Active: {inputActive}\\n" +
               $"• Time Since Input: {GetTimeSinceLastInput():F2}s\\n" +
               $"• Response Time: {metrics.currentResponseTime:F2}ms (Avg: {metrics.averageResponseTime:F2}ms)\\n" +
               $"• Performance Target Met: {metrics.performanceTargetsMet}\\n" +
               $"• Frame Rate: {metrics.frameRate:F1}fps\\n" +
               $"• Boundary Left: {effectiveLeftBoundary:F2}\\n" +
               $"• Boundary Right: {effectiveRightBoundary:F2}\\n" +
               $"• Playable Width: {GetPlayableWidth():F2}\\n" +
               $"• Boundaries Auto-Detected: {boundariesAutoDetected}\\n" +
               $"• Paddle Half-Width: {paddleHalfWidth:F2}\\n" +
               $"• Components: Collider={paddleCollider != null}, Renderer={paddleRenderer != null}, Camera={mainCamera != null}";
    }
    
    /// <summary>
    /// Force re-validation of controller setup with performance optimization.
    /// </summary>
    public void RevalidateSetup()
    {
        Debug.Log("[PaddleController] Forcing setup re-validation...");
        ValidateSetup();
        if (isInitialized)
        {
            InitializePaddleState();
            
            // Apply WebGL optimizations if needed
            if (paddleData != null && !paddleData.IsOptimizedForWebGL())
            {
                Debug.Log("[PaddleController] Applying WebGL optimizations...");
                paddleData.ConfigureForWebGL();
            }
            
            ResetPerformanceMetrics();
        }
    }
    
    #endregion
}