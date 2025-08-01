    using UnityEngine;

/// <summary>
/// Enumeration defining the types of boundary walls in the game area.
/// Used to identify and configure different boundary wall positions.
/// </summary>
[System.Serializable]
public enum BoundaryType
{
    /// <summary>
    /// Top boundary wall - prevents ball from escaping upward.
    /// </summary>
    Top,
    
    /// <summary>
    /// Left boundary wall - prevents ball from escaping leftward.
    /// </summary>
    Left,
    
    /// <summary>
    /// Right boundary wall - prevents ball from escaping rightward.
    /// </summary>
    Right,
    
    /// <summary>
    /// Bottom boundary wall - typically the area where ball is lost (paddle miss).
    /// May be configured differently than other boundaries.
    /// </summary>
    Bottom
}

/// <summary>
/// Configuration data structure for individual boundary wall properties.
/// Contains all parameters needed to create and configure a single boundary wall.
/// </summary>
[System.Serializable]
public struct BoundaryWallConfig
{
    [Header("Boundary Identification")]
    [Tooltip("Type of boundary wall (Top, Left, Right, Bottom)")]
    public BoundaryType boundaryType;
    
    [Header("Dimensions")]
    [Tooltip("Width of the boundary wall in world units")]
    [Range(0.1f, 10f)]
    public float width;
    
    [Tooltip("Height of the boundary wall in world units")]
    [Range(0.1f, 10f)]
    public float height;
    
    [Tooltip("Thickness of the boundary wall in world units")]
    [Range(0.1f, 5f)]
    public float thickness;
    
    [Header("Positioning")]
    [Tooltip("Offset from calculated boundary position")]
    public Vector3 positionOffset;
    
    [Tooltip("Rotation offset for special boundary orientations")]
    public Vector3 rotationOffset;
    
    [Header("Physics Properties")]
    [Tooltip("Bounce coefficient for ball collisions (0 = no bounce, 1 = perfect bounce)")]
    [Range(0f, 2f)]
    public float bounceCoefficient;
    
    [Tooltip("Physics material to use for collision properties")]
    public PhysicsMaterial2D physicsMaterial;
    
    [Tooltip("Enable collision detection for this boundary")]
    public bool enableCollision;
    
    [Tooltip("Layer for collision detection and physics interactions")]
    public int collisionLayer;
    
    [Header("Visual Properties")]
    [Tooltip("Enable visual representation of this boundary")]
    public bool enableVisual;
    
    [Tooltip("Color for boundary visualization")]
    public Color visualColor;
    
    [Tooltip("Material for boundary rendering")]
    public Material visualMaterial;
    
    /// <summary>
    /// Creates a default boundary wall configuration for the specified type.
    /// </summary>
    /// <param name="type">Type of boundary to create default config for</param>
    /// <returns>Default boundary wall configuration</returns>
    public static BoundaryWallConfig CreateDefault(BoundaryType type)
    {
        BoundaryWallConfig config = new BoundaryWallConfig
        {
            boundaryType = type,
            width = 1f,
            height = 1f,
            thickness = 0.5f,
            positionOffset = Vector3.zero,
            rotationOffset = Vector3.zero,
            bounceCoefficient = 1f,
            physicsMaterial = null,
            enableCollision = true,
            collisionLayer = 0,
            enableVisual = true,
            visualColor = Color.white,
            visualMaterial = null
        };
        
        // Type-specific defaults
        switch (type)
        {
            case BoundaryType.Top:
                config.width = 20f;
                config.height = 1f;
                config.visualColor = Color.blue;
                break;
                
            case BoundaryType.Left:
            case BoundaryType.Right:
                config.width = 1f;
                config.height = 12f;
                config.visualColor = Color.green;
                break;
                
            case BoundaryType.Bottom:
                config.width = 20f;
                config.height = 1f;
                config.enableCollision = false; // Bottom boundary typically doesn't block
                config.visualColor = Color.red;
                break;
        }
        
        return config;
    }
    
    /// <summary>
    /// Validates the boundary wall configuration for completeness and correctness.
    /// </summary>
    /// <returns>True if configuration is valid</returns>
    public bool ValidateConfiguration()
    {
        bool isValid = true;
        
        // Validate dimensions
        if (width <= 0f || height <= 0f || thickness <= 0f)
        {
            Debug.LogWarning($"[BoundaryWallConfig] Invalid dimensions for {boundaryType}: {width}x{height}x{thickness}");
            isValid = false;
        }
        
        // Validate bounce coefficient
        if (bounceCoefficient < 0f || bounceCoefficient > 2f)
        {
            Debug.LogWarning($"[BoundaryWallConfig] Invalid bounce coefficient for {boundaryType}: {bounceCoefficient}");
            isValid = false;
        }
        
        // Validate collision layer
        if (collisionLayer < 0 || collisionLayer > 31)
        {
            Debug.LogWarning($"[BoundaryWallConfig] Invalid collision layer for {boundaryType}: {collisionLayer}");
            isValid = false;
        }
        
        return isValid;
    }
}

/// <summary>
/// Main boundary configuration ScriptableObject containing all boundary wall configurations
/// and global boundary system parameters. Provides centralized configuration for boundary
/// wall creation, physics properties, and resolution scaling.
/// </summary>
[CreateAssetMenu(fileName = "BoundaryConfig", menuName = "Breakout/Boundary Configuration", order = 1)]
public class BoundaryConfig : ScriptableObject
{
    [Header("Global Boundary Settings")]
    [Tooltip("Enable boundary system for physics containment")]
    public bool enableBoundaries = true;
    
    [Tooltip("Default physics material for all boundaries if not individually specified")]
    public PhysicsMaterial2D defaultPhysicsMaterial;
    
    [Tooltip("Default visual material for all boundaries if not individually specified")]
    public Material defaultVisualMaterial;
    
    [Header("Aspect Ratio Configuration")]
    [Tooltip("Target aspect ratio for gameplay area (16:10 = 1.6)")]
    [Range(1f, 3f)]
    public float targetAspectRatio = 1.6f; // 16:10 aspect ratio
    
    [Tooltip("Reference resolution for boundary calculations")]
    public Vector2 referenceResolution = new Vector2(1920, 1200); // 16:10 reference
    
    [Tooltip("Boundary margin from screen edges in world units")]
    [Range(0f, 5f)]
    public float boundaryMargin = 1f;
    
    [Header("Play Area Dimensions")]
    [Tooltip("Width of the gameplay area in world units")]
    [Range(5f, 50f)]
    public float playAreaWidth = 20f;
    
    [Tooltip("Height of the gameplay area in world units")]
    [Range(3f, 30f)]
    public float playAreaHeight = 12f;
    
    [Tooltip("Center position of the play area in world space")]
    public Vector3 playAreaCenter = Vector3.zero;
    
    [Header("Resolution Scaling")]
    [Tooltip("Enable automatic scaling based on screen resolution")]
    public bool enableResolutionScaling = true;
    
    [Tooltip("Minimum scale factor to prevent boundaries from becoming too small")]
    [Range(0.1f, 1f)]
    public float minimumScaleFactor = 0.5f;
    
    [Tooltip("Maximum scale factor to prevent boundaries from becoming too large")]
    [Range(1f, 5f)]
    public float maximumScaleFactor = 2f;
    
    [Header("Individual Boundary Configurations")]
    [Tooltip("Configuration for the top boundary wall")]
    public BoundaryWallConfig topBoundary = BoundaryWallConfig.CreateDefault(BoundaryType.Top);
    
    [Tooltip("Configuration for the left boundary wall")]
    public BoundaryWallConfig leftBoundary = BoundaryWallConfig.CreateDefault(BoundaryType.Left);
    
    [Tooltip("Configuration for the right boundary wall")]
    public BoundaryWallConfig rightBoundary = BoundaryWallConfig.CreateDefault(BoundaryType.Right);
    
    [Tooltip("Configuration for the bottom boundary wall")]
    public BoundaryWallConfig bottomBoundary = BoundaryWallConfig.CreateDefault(BoundaryType.Bottom);
    
    [Header("Physics Tuning")]
    [Tooltip("Global bounce multiplier applied to all boundaries")]
    [Range(0f, 2f)]
    public float globalBounceMultiplier = 1f;
    
    [Tooltip("Enable physics debugging for boundary collisions")]
    public bool enablePhysicsDebugging = false;
    
    [Tooltip("Show boundary visualization in Scene view")]
    public bool showBoundaryGizmos = true;
    
    [Header("Performance Settings")]
    [Tooltip("Enable boundary pooling for performance optimization")]
    public bool enableBoundaryPooling = false;
    
    [Tooltip("Collision detection precision level")]
    public CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.Discrete;
    
    /// <summary>
    /// Gets the boundary wall configuration for the specified boundary type.
    /// </summary>
    /// <param name="boundaryType">Type of boundary to get configuration for</param>
    /// <returns>Boundary wall configuration for the specified type</returns>
    public BoundaryWallConfig GetBoundaryConfig(BoundaryType boundaryType)
    {
        switch (boundaryType)
        {
            case BoundaryType.Top:
                return topBoundary;
            case BoundaryType.Left:
                return leftBoundary;
            case BoundaryType.Right:
                return rightBoundary;
            case BoundaryType.Bottom:
                return bottomBoundary;
            default:
                Debug.LogWarning($"[BoundaryConfig] Unknown boundary type: {boundaryType}");
                return BoundaryWallConfig.CreateDefault(boundaryType);
        }
    }
    
    /// <summary>
    /// Sets the boundary wall configuration for the specified boundary type.
    /// </summary>
    /// <param name="boundaryType">Type of boundary to set configuration for</param>
    /// <param name="config">New configuration for the boundary</param>
    public void SetBoundaryConfig(BoundaryType boundaryType, BoundaryWallConfig config)
    {
        switch (boundaryType)
        {
            case BoundaryType.Top:
                topBoundary = config;
                break;
            case BoundaryType.Left:
                leftBoundary = config;
                break;
            case BoundaryType.Right:
                rightBoundary = config;
                break;
            case BoundaryType.Bottom:
                bottomBoundary = config;
                break;
            default:
                Debug.LogWarning($"[BoundaryConfig] Cannot set configuration for unknown boundary type: {boundaryType}");
                break;
        }
    }
    
    /// <summary>
    /// Calculates the scale factor for resolution scaling based on current screen size.
    /// </summary>
    /// <returns>Scale factor to apply to boundary dimensions</returns>
    public float CalculateResolutionScaleFactor()
    {
        if (!enableResolutionScaling)
        {
            return 1f;
        }
        
        float currentAspectRatio = (float)Screen.width / Screen.height;
        float targetAspect = targetAspectRatio;
        
        // Calculate scale based on aspect ratio difference
        float aspectScale = currentAspectRatio / targetAspect;
        
        // Also consider resolution difference
        float resolutionScale = Mathf.Min(
            Screen.width / referenceResolution.x,
            Screen.height / referenceResolution.y
        );
        
        // Combine both factors
        float combinedScale = Mathf.Sqrt(aspectScale * resolutionScale);
        
        // Clamp to min/max values
        return Mathf.Clamp(combinedScale, minimumScaleFactor, maximumScaleFactor);
    }
    
    /// <summary>
    /// Calculates the world position for a boundary based on play area and boundary type.
    /// </summary>
    /// <param name="boundaryType">Type of boundary to calculate position for</param>
    /// <returns>World position for the boundary</returns>
    public Vector3 CalculateBoundaryPosition(BoundaryType boundaryType)
    {
        Vector3 basePosition = playAreaCenter;
        BoundaryWallConfig config = GetBoundaryConfig(boundaryType);
        
        switch (boundaryType)
        {
            case BoundaryType.Top:
                basePosition.y += playAreaHeight * 0.5f + config.thickness * 0.5f + boundaryMargin;
                break;
                
            case BoundaryType.Bottom:
                basePosition.y -= playAreaHeight * 0.5f + config.thickness * 0.5f + boundaryMargin;
                break;
                
            case BoundaryType.Left:
                basePosition.x -= playAreaWidth * 0.5f + config.thickness * 0.5f + boundaryMargin;
                break;
                
            case BoundaryType.Right:
                basePosition.x += playAreaWidth * 0.5f + config.thickness * 0.5f + boundaryMargin;
                break;
        }
        
        return basePosition + config.positionOffset;
    }
    
    /// <summary>
    /// Validates the entire boundary configuration for completeness and correctness.
    /// </summary>
    /// <returns>True if all configurations are valid</returns>
    public bool ValidateConfiguration()
    {
        bool isValid = true;
        
        // Validate global settings
        if (playAreaWidth <= 0f || playAreaHeight <= 0f)
        {
            Debug.LogError("[BoundaryConfig] Invalid play area dimensions");
            isValid = false;
        }
        
        if (targetAspectRatio <= 0f)
        {
            Debug.LogError("[BoundaryConfig] Invalid target aspect ratio");
            isValid = false;
        }
        
        if (referenceResolution.x <= 0f || referenceResolution.y <= 0f)
        {
            Debug.LogError("[BoundaryConfig] Invalid reference resolution");
            isValid = false;
        }
        
        // Validate individual boundary configurations
        isValid &= topBoundary.ValidateConfiguration();
        isValid &= leftBoundary.ValidateConfiguration();
        isValid &= rightBoundary.ValidateConfiguration();
        isValid &= bottomBoundary.ValidateConfiguration();
        
        return isValid;
    }
    
    /// <summary>
    /// Resets all boundary configurations to default values.
    /// </summary>
    public void ResetToDefaults()
    {
        enableBoundaries = true;
        targetAspectRatio = 1.6f;
        referenceResolution = new Vector2(1920, 1200);
        boundaryMargin = 1f;
        playAreaWidth = 20f;
        playAreaHeight = 12f;
        playAreaCenter = Vector3.zero;
        enableResolutionScaling = true;
        minimumScaleFactor = 0.5f;
        maximumScaleFactor = 2f;
        globalBounceMultiplier = 1f;
        enablePhysicsDebugging = false;
        showBoundaryGizmos = true;
        enableBoundaryPooling = false;
        collisionDetectionMode = CollisionDetectionMode.Discrete;
        
        topBoundary = BoundaryWallConfig.CreateDefault(BoundaryType.Top);
        leftBoundary = BoundaryWallConfig.CreateDefault(BoundaryType.Left);
        rightBoundary = BoundaryWallConfig.CreateDefault(BoundaryType.Right);
        bottomBoundary = BoundaryWallConfig.CreateDefault(BoundaryType.Bottom);
    }
    
    /// <summary>
    /// Gets all boundary configurations as an array.
    /// </summary>
    /// <returns>Array of all boundary wall configurations</returns>
    public BoundaryWallConfig[] GetAllBoundaryConfigs()
    {
        return new BoundaryWallConfig[]
        {
            topBoundary,
            leftBoundary,
            rightBoundary,
            bottomBoundary
        };
    }
    
    /// <summary>
    /// Gets a summary of the current configuration for debugging purposes.
    /// </summary>
    /// <returns>Configuration summary string</returns>
    public string GetConfigurationSummary()
    {
        float scaleFactor = CalculateResolutionScaleFactor();
        
        return $"BoundaryConfig Summary:\n" +
               $"• Boundaries Enabled: {enableBoundaries}\n" +
               $"• Target Aspect Ratio: {targetAspectRatio:F2}\n" +
               $"• Play Area: {playAreaWidth:F1} x {playAreaHeight:F1}\n" +
               $"• Play Area Center: {playAreaCenter}\n" +
               $"• Boundary Margin: {boundaryMargin:F1}\n" +
               $"• Resolution Scaling: {enableResolutionScaling} (Factor: {scaleFactor:F2})\n" +
               $"• Global Bounce Multiplier: {globalBounceMultiplier:F2}\n" +
               $"• Physics Debugging: {enablePhysicsDebugging}\n" +
               $"• Gizmos Enabled: {showBoundaryGizmos}\n" +
               $"• Boundary Pooling: {enableBoundaryPooling}\n" +
               $"• Collision Detection: {collisionDetectionMode}\n" +
               $"• Configuration Valid: {ValidateConfiguration()}";
    }
}