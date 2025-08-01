using UnityEngine;

/// <summary>
/// MonoBehaviour component for individual boundary wall GameObjects.
/// Manages collider setup, positioning, and configuration for a single boundary wall.
/// Integrates with BoundaryConfig data structures for consistent setup across boundary types.
/// </summary>
public class BoundaryWall : MonoBehaviour
{
    [Header("Wall Configuration")]
    [Tooltip("Type of boundary wall (Top, Left, Right, Bottom)")]
    public BoundaryType wallType = BoundaryType.Top;
    
    [Tooltip("Boundary configuration data for wall setup")]
    public BoundaryConfig config;
    
    [Header("Wall Properties")]
    [Tooltip("Width of the wall collider in world units")]
    [SerializeField] private float wallWidth = 1f;
    
    [Tooltip("Height of the wall collider in world units")]
    [SerializeField] private float wallHeight = 1f;
    
    [Tooltip("Thickness of the wall collider in world units")]
    [SerializeField] private float wallThickness = 1f;
    
    [Header("Debug Information")]
    [Tooltip("Whether this wall is properly initialized")]
    [SerializeField] private bool isInitialized = false;
    
    [Tooltip("Current wall position in world space")]
    [SerializeField] private Vector3 currentPosition;
    
    [Tooltip("Current wall bounds for debugging")]
    [SerializeField] private Bounds wallBounds;
    
    // Component references
    private Collider2D wallCollider;
    private BoxCollider2D boxCollider;
    
    // Cached values for performance
    private Camera mainCamera;
    private BoundaryWallConfig wallConfig;
    
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
    /// Complete wall setup with configuration and positioning.
    /// </summary>
    private void Start()
    {
        if (config != null)
        {
            SetupWallFromConfig();
        }
        else
        {
            Debug.LogWarning($"[BoundaryWall] {wallType} wall has no configuration - using default setup");
            SetupDefaultWall();
        }
        
        UpdateDebugInformation();
    }
    
    #endregion
    
    #region Component Management
    
    /// <summary>
    /// Initialize and cache component references.
    /// </summary>
    private void InitializeComponents()
    {
        // Get or create BoxCollider2D component
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log($"[BoundaryWall] Added BoxCollider2D to {wallType} wall");
        }
        
        wallCollider = boxCollider;
        
        // Cache main camera reference
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
        
        if (mainCamera == null)
        {
            Debug.LogWarning("[BoundaryWall] No camera found - wall positioning may be incorrect");
        }
    }
    
    /// <summary>
    /// Validate component setup and configuration.
    /// </summary>
    private void ValidateSetup()
    {
        bool setupValid = true;
        
        // Validate collider component
        if (wallCollider == null)
        {
            Debug.LogError($"[BoundaryWall] {wallType} wall missing collider component");
            setupValid = false;
        }
        
        // Validate wall type
        if (!System.Enum.IsDefined(typeof(BoundaryType), wallType))
        {
            Debug.LogError($"[BoundaryWall] Invalid wall type: {wallType}");
            setupValid = false;
        }
        
        isInitialized = setupValid;
        
        if (isInitialized)
        {
            Debug.Log($"[BoundaryWall] {wallType} wall initialized successfully");
        }
        else
        {
            Debug.LogError($"[BoundaryWall] {wallType} wall initialization failed");
        }
    }
    
    #endregion
    
    #region Wall Setup
    
    /// <summary>
    /// Set up wall using BoundaryConfig data.
    /// </summary>
    private void SetupWallFromConfig()
    {
        if (config == null)
        {
            Debug.LogWarning($"[BoundaryWall] No configuration provided for {wallType} wall");
            return;
        }
        
        // Get wall-specific configuration
        wallConfig = config.GetBoundaryConfig(wallType);
        
        // Configure dimensions
        ConfigureWallDimensions(wallConfig);
        
        // Configure collider properties
        ConfigureColliderProperties(wallConfig);
        
        // Position wall based on configuration
        PositionWallFromConfig();
        
        Debug.Log($"[BoundaryWall] {wallType} wall configured from BoundaryConfig");
    }
    
    /// <summary>
    /// Set up wall with default values if no configuration is available.
    /// </summary>
    private void SetupDefaultWall()
    {
        // Create default configuration for wall type
        BoundaryWallConfig defaultConfig = BoundaryWallConfig.CreateDefault(wallType);
        
        // Configure dimensions
        ConfigureWallDimensions(defaultConfig);
        
        // Configure collider properties
        ConfigureColliderProperties(defaultConfig);
        
        // Position wall with default settings
        PositionWallDefault();
        
        Debug.Log($"[BoundaryWall] {wallType} wall configured with default values");
    }
    
    /// <summary>
    /// Configure wall dimensions based on wall configuration.
    /// </summary>
    /// <param name="wallConfig">Wall configuration data</param>
    private void ConfigureWallDimensions(BoundaryWallConfig wallConfig)
    {
        // Apply resolution scaling if enabled
        float scaleFactor = 1f;
        if (config != null && config.enableResolutionScaling)
        {
            scaleFactor = config.CalculateResolutionScaleFactor();
        }
        
        // Set dimensions with scaling
        wallWidth = wallConfig.width * scaleFactor;
        wallHeight = wallConfig.height * scaleFactor;
        wallThickness = wallConfig.thickness * scaleFactor;
        
        // Configure collider size
        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(wallWidth, wallHeight);
        }
        
        Debug.Log($"[BoundaryWall] {wallType} wall dimensions: {wallWidth:F2} x {wallHeight:F2} x {wallThickness:F2} (scale: {scaleFactor:F2})");
    }
    
    /// <summary>
    /// Configure collider physics properties.
    /// </summary>
    /// <param name="wallConfig">Wall configuration data</param>
    private void ConfigureColliderProperties(BoundaryWallConfig wallConfig)
    {
        if (boxCollider == null) return;
        
        // Set collider as solid wall (not trigger)
        boxCollider.isTrigger = false;
        
        // Apply physics material if provided
        if (wallConfig.physicsMaterial != null)
        {
            boxCollider.sharedMaterial = wallConfig.physicsMaterial;
        }
        else if (config != null && config.defaultPhysicsMaterial != null)
        {
            boxCollider.sharedMaterial = config.defaultPhysicsMaterial;
        }
        
        // Configure collision layer
        if (wallConfig.collisionLayer >= 0 && wallConfig.collisionLayer <= 31)
        {
            gameObject.layer = wallConfig.collisionLayer;
        }
        
        // Enable/disable collision
        boxCollider.enabled = wallConfig.enableCollision;
        
        Debug.Log($"[BoundaryWall] {wallType} wall collider configured - Collision: {wallConfig.enableCollision}, Material: {(boxCollider.sharedMaterial != null ? boxCollider.sharedMaterial.name : "Default")}");
    }
    
    #endregion
    
    #region Positioning
    
    /// <summary>
    /// Position wall based on BoundaryConfig calculations.
    /// </summary>
    private void PositionWallFromConfig()
    {
        if (config == null) return;
        
        // Calculate position using configuration
        Vector3 calculatedPosition = config.CalculateBoundaryPosition(wallType);
        
        // Apply position
        transform.position = calculatedPosition;
        currentPosition = calculatedPosition;
        
        Debug.Log($"[BoundaryWall] {wallType} wall positioned at: {currentPosition}");
    }
    
    /// <summary>
    /// Position wall using default calculations based on camera bounds.
    /// </summary>
    private void PositionWallDefault()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning($"[BoundaryWall] Cannot position {wallType} wall - no camera available");
            return;
        }
        
        // Calculate camera bounds for 16:10 aspect ratio
        Vector3 position = CalculateDefaultPosition();
        
        // Apply position
        transform.position = position;
        currentPosition = position;
        
        Debug.Log($"[BoundaryWall] {wallType} wall positioned at default location: {currentPosition}");
    }
    
    /// <summary>
    /// Calculate default wall position based on camera bounds and wall type.
    /// </summary>
    /// <returns>Calculated wall position</returns>
    private Vector3 CalculateDefaultPosition()
    {
        if (mainCamera == null) return Vector3.zero;
        
        // Get camera bounds in world space
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        Vector3 basePosition = mainCamera.transform.position;
        basePosition.z = 0f; // Ensure wall is at z=0 for 2D gameplay
        
        // Calculate position based on wall type
        switch (wallType)
        {
            case BoundaryType.Top:
                basePosition.y += cameraHeight * 0.5f + wallThickness * 0.5f;
                break;
                
            case BoundaryType.Left:
                basePosition.x -= cameraWidth * 0.5f + wallThickness * 0.5f;
                break;
                
            case BoundaryType.Right:
                basePosition.x += cameraWidth * 0.5f + wallThickness * 0.5f;
                break;
                
            case BoundaryType.Bottom:
                basePosition.y -= cameraHeight * 0.5f + wallThickness * 0.5f;
                break;
        }
        
        return basePosition;
    }
    
    /// <summary>
    /// Recalculate and update wall position (useful for runtime adjustments).
    /// </summary>
    public void UpdateWallPosition()
    {
        if (config != null)
        {
            PositionWallFromConfig();
        }
        else
        {
            PositionWallDefault();
        }
        
        UpdateDebugInformation();
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Set the boundary configuration for this wall.
    /// </summary>
    /// <param name="newConfig">New boundary configuration</param>
    public void SetBoundaryConfig(BoundaryConfig newConfig)
    {
        config = newConfig;
        
        if (isInitialized)
        {
            SetupWallFromConfig();
            UpdateDebugInformation();
        }
    }
    
    /// <summary>
    /// Set the wall type and reconfigure if initialized.
    /// </summary>
    /// <param name="newWallType">New wall type</param>
    public void SetWallType(BoundaryType newWallType)
    {
        wallType = newWallType;
        
        if (isInitialized)
        {
            if (config != null)
            {
                SetupWallFromConfig();
            }
            else
            {
                SetupDefaultWall();
            }
            UpdateDebugInformation();
        }
    }
    
    /// <summary>
    /// Get the current wall bounds in world space.
    /// </summary>
    /// <returns>Wall bounds</returns>
    public Bounds GetWallBounds()
    {
        if (wallCollider != null)
        {
            return wallCollider.bounds;
        }
        
        // Fallback calculation
        return new Bounds(currentPosition, new Vector3(wallWidth, wallHeight, wallThickness));
    }
    
    /// <summary>
    /// Check if a point is within the wall's collision area.
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <returns>True if point is within wall bounds</returns>
    public bool ContainsPoint(Vector3 point)
    {
        if (wallCollider != null)
        {
            return wallCollider.bounds.Contains(point);
        }
        
        return GetWallBounds().Contains(point);
    }
    
    /// <summary>
    /// Get wall configuration summary for debugging.
    /// </summary>
    /// <returns>Wall configuration summary</returns>
    public string GetWallSummary()
    {
        return $"BoundaryWall [{wallType}]:\n" +
               $"• Position: {currentPosition}\n" +
               $"• Dimensions: {wallWidth:F2} x {wallHeight:F2} x {wallThickness:F2}\n" +
               $"• Collision Enabled: {(wallCollider != null ? wallCollider.enabled.ToString() : "No Collider")}\n" +
               $"• Physics Material: {(wallCollider != null && wallCollider.sharedMaterial != null ? wallCollider.sharedMaterial.name : "Default")}\n" +
               $"• Initialized: {isInitialized}\n" +
               $"• Bounds: {wallBounds}";
    }
    
    #endregion
    
    #region Debug and Validation
    
    /// <summary>
    /// Update debug information for Inspector display.
    /// </summary>
    private void UpdateDebugInformation()
    {
        currentPosition = transform.position;
        wallBounds = GetWallBounds();
    }
    
    /// <summary>
    /// Validate wall setup and report any issues.
    /// </summary>
    /// <returns>True if wall is properly configured</returns>
    public bool ValidateWall()
    {
        bool isValid = true;
        
        // Check initialization
        if (!isInitialized)
        {
            Debug.LogWarning($"[BoundaryWall] {wallType} wall not properly initialized");
            isValid = false;
        }
        
        // Check collider
        if (wallCollider == null)
        {
            Debug.LogWarning($"[BoundaryWall] {wallType} wall missing collider");
            isValid = false;
        }
        
        // Check dimensions
        if (wallWidth <= 0f || wallHeight <= 0f || wallThickness <= 0f)
        {
            Debug.LogWarning($"[BoundaryWall] {wallType} wall has invalid dimensions: {wallWidth} x {wallHeight} x {wallThickness}");
            isValid = false;
        }
        
        // Check position (not at origin unless intentional)
        if (currentPosition == Vector3.zero && wallType != BoundaryType.Bottom)
        {
            Debug.LogWarning($"[BoundaryWall] {wallType} wall may be incorrectly positioned at origin");
        }
        
        return isValid;
    }
    
    #endregion
    
    #region Gizmos
    
    /// <summary>
    /// Draw wall gizmos in Scene view for visualization.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Only draw if configuration enables gizmos
        if (config != null && !config.showBoundaryGizmos) return;
        
        // Use wall-specific color if available
        Color gizmoColor = Color.white;
        if (config != null)
        {
            BoundaryWallConfig wallConf = config.GetBoundaryConfig(wallType);
            gizmoColor = wallConf.visualColor;
        }
        else
        {
            // Default colors based on wall type
            switch (wallType)
            {
                case BoundaryType.Top:
                    gizmoColor = Color.blue;
                    break;
                case BoundaryType.Left:
                case BoundaryType.Right:
                    gizmoColor = Color.green;
                    break;
                case BoundaryType.Bottom:
                    gizmoColor = Color.red;
                    break;
            }
        }
        
        // Draw wall bounds
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Vector3 size = new(wallWidth, wallHeight, 0.1f);
        Gizmos.DrawCube(transform.position, size);
        
        // Draw wire frame
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, size);
        
        // Draw wall type label position
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
    
    /// <summary>
    /// Draw selected gizmos with additional detail.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw bounds in yellow when selected
        Gizmos.color = Color.yellow;
        Vector3 size = new(wallWidth, wallHeight, 0.1f);
        Gizmos.DrawWireCube(transform.position, size);
        
        // Draw center point
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        
        // Draw bounds information
        if (wallCollider != null)
        {
            Bounds bounds = wallCollider.bounds;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
    
    #endregion
}