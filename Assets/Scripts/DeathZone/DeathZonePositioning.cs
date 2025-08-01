using UnityEngine;
using System.Collections;

/// <summary>
/// Adaptive positioning system for death zone placement relative to paddle location.
/// Handles screen resolution adaptation and maintains consistent gameplay balance
/// across different aspect ratios and screen sizes.
/// </summary>
[System.Serializable]
public class DeathZonePositioning : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Death zone configuration asset")]
    public DeathZoneConfig config;
    
    [Header("Paddle Integration")]
    [Tooltip("Reference to paddle transform for relative positioning")]
    public Transform paddleTransform;
    
    [Tooltip("Offset below paddle position (negative values place below paddle)")]
    [Range(-10f, 0f)]
    public float paddleOffset = -2.0f;
    
    [Tooltip("Automatically track paddle movement")]
    public bool trackPaddleMovement = true;
    
    [Header("Resolution Adaptation")]
    [Tooltip("Enable automatic resolution adaptation")]
    public bool adaptToResolution = true;
    
    [Tooltip("Reference resolution for scaling calculations")]
    public Vector2 referenceResolution = new Vector2(1920f, 1200f);
    
    [Tooltip("Update positioning when screen size changes")]
    public bool detectResolutionChanges = true;
    
    [Header("Positioning Constraints")]
    [Tooltip("Minimum distance from screen bottom")]
    [Range(0f, 5f)]
    public float minimumBottomDistance = 1f;
    
    [Tooltip("Maximum distance from paddle")]
    [Range(1f, 10f)]
    public float maximumPaddleDistance = 5f;
    
    [Tooltip("Horizontal centering mode")]
    public PositionCenteringMode centeringMode = PositionCenteringMode.FollowPaddle;
    
    [Header("Runtime Data")]
    [Tooltip("Current calculated death zone position")]
    [SerializeField] private Vector3 currentPosition;
    
    [Tooltip("Current screen dimensions")]
    [SerializeField] private Vector2 currentScreenSize;
    
    [Tooltip("Current resolution scale factor")]
    [SerializeField] private float currentScaleFactor = 1f;
    
    [Tooltip("Is positioning system initialized")]
    [SerializeField] private bool isInitialized = false;
    
    // Cached references
    private Camera mainCamera;
    private Vector3 lastPaddlePosition;
    private Vector2 lastScreenSize;
    private Coroutine positionUpdateCoroutine;
    
    // Position calculation cache
    private Vector3 cachedScreenBounds;
    private bool screenBoundsCached = false;
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize positioning system and cache references.
    /// </summary>
    private void Awake()
    {
        InitializePositioning();
    }
    
    /// <summary>
    /// Set up initial positioning and start tracking systems.
    /// </summary>
    private void Start()
    {
        ValidateSetup();
        CalculateInitialPosition();
        StartPositionTracking();
    }
    
    /// <summary>
    /// Clean up tracking coroutines.
    /// </summary>
    private void OnDestroy()
    {
        StopPositionTracking();
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initialize positioning system components and references.
    /// </summary>
    private void InitializePositioning()
    {
        // Find main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
        
        // Load configuration if not assigned
        if (config == null)
        {
            config = Resources.Load<DeathZoneConfig>("DeathZoneConfig");
        }
        
        // Find paddle if not assigned
        if (paddleTransform == null)
        {
            GameObject paddle = FindPaddleGameObject();
            if (paddle != null)
            {
                paddleTransform = paddle.transform;
            }
        }
        
        // Initialize screen tracking
        currentScreenSize = new Vector2(Screen.width, Screen.height);
        lastScreenSize = currentScreenSize;
        
        // Store initial paddle position if available
        if (paddleTransform != null)
        {
            lastPaddlePosition = paddleTransform.position;
        }
        
        isInitialized = true;
        Debug.Log($"[DeathZonePositioning] Initialized - Paddle: {(paddleTransform != null ? paddleTransform.name : "None")}, Config: {(config != null ? "Loaded" : "Missing")}");
    }
    
    /// <summary>
    /// Attempts to find paddle GameObject in scene.
    /// </summary>
    /// <returns>Paddle GameObject or null if not found</returns>
    private GameObject FindPaddleGameObject()
    {
        // Try common paddle names
        string[] paddleNames = { "Paddle", "Player Paddle", "PlayerPaddle", "Ball Paddle" };
        
        foreach (string paddleName in paddleNames)
        {
            GameObject paddle = GameObject.Find(paddleName);
            if (paddle != null)
            {
                Debug.Log($"[DeathZonePositioning] Found paddle: {paddle.name}");
                return paddle;
            }
        }
        
        // Try finding by component
        var paddleController = FindFirstObjectByType<PaddleController>();
        if (paddleController != null)
        {
            Debug.Log($"[DeathZonePositioning] Found paddle by component: {paddleController.gameObject.name}");
            return paddleController.gameObject;
        }
        
        Debug.LogWarning("[DeathZonePositioning] No paddle GameObject found - positioning will use screen center");
        return null;
    }
    
    /// <summary>
    /// Validates positioning system setup.
    /// </summary>
    private void ValidateSetup()
    {
        if (config == null)
        {
            Debug.LogWarning("[DeathZonePositioning] DeathZoneConfig not found - using default values");
        }
        
        if (paddleTransform == null)
        {
            Debug.LogWarning("[DeathZonePositioning] Paddle reference not set - death zone will use fixed positioning");
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("[DeathZonePositioning] No camera found - positioning calculations will be inaccurate");
        }
    }
    
    #endregion
    
    #region Position Calculation
    
    /// <summary>
    /// Calculates initial death zone position.
    /// </summary>
    private void CalculateInitialPosition()
    {
        currentPosition = CalculateDeathZonePosition();
        transform.position = currentPosition;
        
        Debug.Log($"[DeathZonePositioning] Initial position calculated: {currentPosition}");
    }
    
    /// <summary>
    /// Calculates death zone position based on current configuration and paddle location.
    /// </summary>
    /// <returns>Calculated world position for death zone</returns>
    public Vector3 CalculateDeathZonePosition()
    {
        Vector3 position = Vector3.zero;
        
        // Get base position based on centering mode
        switch (centeringMode)
        {
            case PositionCenteringMode.FollowPaddle:
                position = CalculatePaddleRelativePosition();
                break;
                
            case PositionCenteringMode.ScreenCenter:
                position = CalculateScreenCenterPosition();
                break;
                
            case PositionCenteringMode.CustomPosition:
                position = CalculateCustomPosition();
                break;
        }
        
        // Apply resolution scaling if enabled
        if (adaptToResolution)
        {
            position = ApplyResolutionScaling(position);
        }
        
        // Apply positioning constraints
        position = ApplyPositioningConstraints(position);
        
        // Apply configuration offsets
        if (config != null)
        {
            position.x += config.positioningOffsets.x;
            position.y += config.positioningOffsets.y;
        }
        
        // Ensure z position is appropriate for 2D
        position.z = 0f;
        
        return position;
    }
    
    /// <summary>
    /// Calculates paddle-relative position.
    /// </summary>
    /// <returns>Position relative to paddle</returns>
    private Vector3 CalculatePaddleRelativePosition()
    {
        Vector3 position = Vector3.zero;
        
        if (paddleTransform != null)
        {
            position = paddleTransform.position;
            position.y += paddleOffset;
            
            // Use config paddle offset if available
            if (config != null)
            {
                position.y = paddleTransform.position.y - config.paddleOffset;
            }
        }
        else
        {
            // Fallback to screen-based positioning
            position = CalculateScreenCenterPosition();
        }
        
        return position;
    }
    
    /// <summary>
    /// Calculates screen center-based position.
    /// </summary>
    /// <returns>Position based on screen center</returns>
    private Vector3 CalculateScreenCenterPosition()
    {
        Vector3 position = Vector3.zero;
        
        if (mainCamera != null)
        {
            // Calculate screen bounds
            Bounds screenBounds = CalculateScreenBounds();
            
            position.x = screenBounds.center.x;
            position.y = screenBounds.min.y + minimumBottomDistance;
            
            // Adjust based on configuration
            if (config != null)
            {
                position.y += config.minimumBottomDistance;
            }
        }
        
        return position;
    }
    
    /// <summary>
    /// Calculates custom position based on configuration.
    /// </summary>
    /// <returns>Custom configured position</returns>
    private Vector3 CalculateCustomPosition()
    {
        Vector3 position = Vector3.zero;
        
        if (config != null)
        {
            // Use positioning offsets as absolute position for custom mode
            position.x = config.positioningOffsets.x;
            position.y = config.positioningOffsets.y;
        }
        
        return position;
    }
    
    /// <summary>
    /// Calculates screen bounds in world space.
    /// </summary>
    /// <returns>Screen bounds</returns>
    private Bounds CalculateScreenBounds()
    {
        if (screenBoundsCached && !HasScreenSizeChanged())
        {
            return new Bounds(cachedScreenBounds, Vector3.zero);
        }
        
        Bounds bounds = new Bounds();
        
        if (mainCamera != null)
        {
            if (mainCamera.orthographic)
            {
                float height = mainCamera.orthographicSize * 2f;
                float width = height * mainCamera.aspect;
                
                Vector3 center = mainCamera.transform.position;
                bounds = new Bounds(center, new Vector3(width, height, 0f));
            }
            else
            {
                // Perspective camera - calculate at z=0
                float distance = Mathf.Abs(mainCamera.transform.position.z);
                Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distance));
                Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distance));
                
                Vector3 center = (bottomLeft + topRight) * 0.5f;
                Vector3 size = topRight - bottomLeft;
                bounds = new Bounds(center, size);
            }
        }
        
        // Cache the bounds
        cachedScreenBounds = bounds.center;
        screenBoundsCached = true;
        
        return bounds;
    }
    
    #endregion
    
    #region Resolution Adaptation
    
    /// <summary>
    /// Applies resolution scaling to position.
    /// </summary>
    /// <param name="basePosition">Base position before scaling</param>
    /// <returns>Scaled position</returns>
    private Vector3 ApplyResolutionScaling(Vector3 basePosition)
    {
        currentScaleFactor = CalculateResolutionScaleFactor();
        
        // Apply scaling to offset components
        Vector3 scaledPosition = basePosition;
        
        // Scale the paddle offset based on resolution
        if (paddleTransform != null)
        {
            float scaledOffset = paddleOffset * currentScaleFactor;
            scaledPosition.y = paddleTransform.position.y + scaledOffset;
        }
        
        return scaledPosition;
    }
    
    /// <summary>
    /// Calculates resolution scale factor.
    /// </summary>
    /// <returns>Scale factor for current resolution</returns>
    private float CalculateResolutionScaleFactor()
    {
        if (!adaptToResolution)
        {
            return 1f;
        }
        
        float widthScale = Screen.width / referenceResolution.x;
        float heightScale = Screen.height / referenceResolution.y;
        
        // Use minimum scale to maintain aspect ratio
        float scaleFactor = Mathf.Min(widthScale, heightScale);
        
        // Clamp to reasonable range
        return Mathf.Clamp(scaleFactor, 0.5f, 2f);
    }
    
    /// <summary>
    /// Checks if screen size has changed.
    /// </summary>
    /// <returns>True if screen size changed</returns>
    private bool HasScreenSizeChanged()
    {
        Vector2 currentSize = new Vector2(Screen.width, Screen.height);
        bool changed = currentSize != lastScreenSize;
        
        if (changed)
        {
            lastScreenSize = currentSize;
            currentScreenSize = currentSize;
            screenBoundsCached = false; // Invalidate cache
        }
        
        return changed;
    }
    
    #endregion
    
    #region Positioning Constraints
    
    /// <summary>
    /// Applies positioning constraints to ensure valid placement.
    /// </summary>
    /// <param name="position">Position to constrain</param>
    /// <returns>Constrained position</returns>
    private Vector3 ApplyPositioningConstraints(Vector3 position)
    {
        Vector3 constrainedPosition = position;
        
        // Calculate screen bounds for constraint checking
        Bounds screenBounds = CalculateScreenBounds();
        
        // Minimum bottom distance constraint
        float minY = screenBounds.min.y + minimumBottomDistance;
        if (constrainedPosition.y < minY)
        {
            constrainedPosition.y = minY;
        }
        
        // Maximum paddle distance constraint
        if (paddleTransform != null)
        {
            float maxDistance = paddleTransform.position.y - maximumPaddleDistance;
            if (constrainedPosition.y < maxDistance)
            {
                constrainedPosition.y = maxDistance;
            }
        }
        
        // Keep within screen horizontal bounds (with margin)
        float margin = 1f;
        constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, 
            screenBounds.min.x + margin, 
            screenBounds.max.x - margin);
        
        return constrainedPosition;
    }
    
    #endregion
    
    #region Position Tracking
    
    /// <summary>
    /// Starts position tracking systems.
    /// </summary>
    private void StartPositionTracking()
    {
        if (trackPaddleMovement || detectResolutionChanges)
        {
            positionUpdateCoroutine = StartCoroutine(PositionUpdateLoop());
        }
    }
    
    /// <summary>
    /// Stops position tracking systems.
    /// </summary>
    private void StopPositionTracking()
    {
        if (positionUpdateCoroutine != null)
        {
            StopCoroutine(positionUpdateCoroutine);
            positionUpdateCoroutine = null;
        }
    }
    
    /// <summary>
    /// Position update loop coroutine.
    /// </summary>
    /// <returns>Coroutine enumerator</returns>
    private IEnumerator PositionUpdateLoop()
    {
        WaitForSeconds updateInterval = new WaitForSeconds(0.1f); // 10 FPS update rate
        
        while (true)
        {
            bool needsUpdate = false;
            
            // Check for paddle movement
            if (trackPaddleMovement && paddleTransform != null)
            {
                if (Vector3.Distance(paddleTransform.position, lastPaddlePosition) > 0.01f)
                {
                    lastPaddlePosition = paddleTransform.position;
                    needsUpdate = true;
                }
            }
            
            // Check for resolution changes
            if (detectResolutionChanges && HasScreenSizeChanged())
            {
                needsUpdate = true;
            }
            
            // Update position if needed
            if (needsUpdate)
            {
                UpdatePosition();
            }
            
            yield return updateInterval;
        }
    }
    
    /// <summary>
    /// Updates death zone position.
    /// </summary>
    public void UpdatePosition()
    {
        if (!isInitialized) return;
        
        Vector3 newPosition = CalculateDeathZonePosition();
        
        // Only update if position changed significantly
        if (Vector3.Distance(newPosition, currentPosition) > 0.01f)
        {
            currentPosition = newPosition;
            transform.position = currentPosition;
            
            Debug.Log($"[DeathZonePositioning] Position updated: {currentPosition}");
        }
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Forces immediate position recalculation and update.
    /// </summary>
    public void ForcePositionUpdate()
    {
        screenBoundsCached = false; // Invalidate cache
        UpdatePosition();
    }
    
    /// <summary>
    /// Sets paddle reference and updates position.
    /// </summary>
    /// <param name="newPaddleTransform">New paddle transform reference</param>
    public void SetPaddleReference(Transform newPaddleTransform)
    {
        paddleTransform = newPaddleTransform;
        if (paddleTransform != null)
        {
            lastPaddlePosition = paddleTransform.position;
            ForcePositionUpdate();
            Debug.Log($"[DeathZonePositioning] Paddle reference set: {paddleTransform.name}");
        }
    }
    
    /// <summary>
    /// Gets current death zone position.
    /// </summary>
    /// <returns>Current world position</returns>
    public Vector3 GetCurrentPosition()
    {
        return currentPosition;
    }
    
    /// <summary>
    /// Gets current resolution scale factor.
    /// </summary>
    /// <returns>Current scale factor</returns>
    public float GetCurrentScaleFactor()
    {
        return currentScaleFactor;
    }
    
    /// <summary>
    /// Checks if positioning system is properly initialized.
    /// </summary>
    /// <returns>True if initialized</returns>
    public bool IsInitialized()
    {
        return isInitialized;
    }
    
    /// <summary>
    /// Gets positioning system status summary.
    /// </summary>
    /// <returns>Status summary string</returns>
    public string GetPositioningStatus()
    {
        return $"DeathZonePositioning Status:\n" +
               $"• Initialized: {isInitialized}\n" +
               $"• Paddle Reference: {(paddleTransform != null ? paddleTransform.name : "None")}\n" +
               $"• Current Position: {currentPosition}\n" +
               $"• Screen Size: {currentScreenSize}\n" +
               $"• Scale Factor: {currentScaleFactor:F2}\n" +
               $"• Centering Mode: {centeringMode}\n" +
               $"• Track Paddle: {trackPaddleMovement}\n" +
               $"• Resolution Adaptation: {adaptToResolution}\n" +
               $"• Config Loaded: {(config != null ? "Yes" : "No")}";
    }
    
    #endregion
    
    #region Editor Support
    
    /// <summary>
    /// Called when values change in Inspector (Editor only).
    /// </summary>
    private void OnValidate()
    {
        // Clamp values to valid ranges
        paddleOffset = Mathf.Clamp(paddleOffset, -10f, 0f);
        minimumBottomDistance = Mathf.Max(0f, minimumBottomDistance);
        maximumPaddleDistance = Mathf.Max(1f, maximumPaddleDistance);
        
        // Ensure reference resolution is valid
        referenceResolution.x = Mathf.Max(100f, referenceResolution.x);
        referenceResolution.y = Mathf.Max(100f, referenceResolution.y);
        
        // Force position update if playing
        if (Application.isPlaying && isInitialized)
        {
            ForcePositionUpdate();
        }
    }
    
    #endregion
}

/// <summary>
/// Enumeration for death zone horizontal centering modes.
/// </summary>
[System.Serializable]
public enum PositionCenteringMode
{
    /// <summary>
    /// Follow paddle horizontal position
    /// </summary>
    FollowPaddle,
    
    /// <summary>
    /// Center on screen regardless of paddle position
    /// </summary>
    ScreenCenter,
    
    /// <summary>
    /// Use custom position from configuration
    /// </summary>
    CustomPosition
}