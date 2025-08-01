using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages physics material configuration and application for boundary walls.
/// Ensures consistent arcade-style ball bouncing behavior with energy conservation.
/// Provides validation and testing utilities for bounce behavior verification.
/// </summary>
[System.Serializable]
public class BoundaryPhysicsMaterial : MonoBehaviour
{
    [Header("Physics Configuration")]
    [Tooltip("Primary physics material for wall collision behavior")]
    public PhysicsMaterial2D wallMaterial;
    
    [Tooltip("Bounciness coefficient (0 = no bounce, 1 = perfect bounce)")]
    [Range(0f, 2f)]
    public float bounciness = 1.0f;
    
    [Tooltip("Friction coefficient (0 = no friction, 1 = high friction)")]
    [Range(0f, 1f)]
    public float friction = 0.0f;
    
    [Header("Material Settings")]
    [Tooltip("How to combine friction values of colliding objects")]
    public PhysicsMaterialCombine2D frictionCombine = PhysicsMaterialCombine2D.Minimum;
    
    [Tooltip("How to combine bounce values of colliding objects")]
    public PhysicsMaterialCombine2D bounceCombine = PhysicsMaterialCombine2D.Maximum;
    
    [Header("Validation")]
    [Tooltip("Enable physics behavior validation and testing")]
    public bool enablePhysicsValidation = true;
    
    [Tooltip("Tolerance for bounce angle validation")]
    [Range(0.01f, 5f)]
    public float angleTolerance = 1f;
    
    [Tooltip("Tolerance for velocity magnitude validation")]
    [Range(0.01f, 0.1f)]
    public float velocityTolerance = 0.05f;
    
    [Header("Runtime Info")]
    [Tooltip("Currently applied physics material")]
    [SerializeField] private PhysicsMaterial2D appliedMaterial;
    
    [Tooltip("Number of boundary walls with physics materials")]
    [SerializeField] private int wallsWithMaterials = 0;
    
    [Tooltip("Validation status of physics configuration")]
    [SerializeField] private bool isPhysicsValid = false;
    
    // Cached references
    private BoundaryConfig boundaryConfig;
    private List<BoundaryWall> boundaryWalls = new List<BoundaryWall>();
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize component and find boundary references.
    /// </summary>
    private void Awake()
    {
        CacheBoundaryReferences();
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Apply physics materials on start.
    /// </summary>
    private void Start()
    {
        if (wallMaterial != null)
        {
            ApplyPhysicsMaterial(wallMaterial);
        }
        else
        {
            Debug.LogWarning("[BoundaryPhysicsMaterial] No physics material assigned - creating default");
            CreateAndApplyDefaultMaterial();
        }
    }
    
    #endregion
    
    #region Material Management
    
    /// <summary>
    /// Creates a physics material with specified properties.
    /// </summary>
    /// <param name="materialName">Name for the physics material</param>
    /// <param name="bounceValue">Bounciness coefficient</param>
    /// <param name="frictionValue">Friction coefficient</param>
    /// <returns>Created PhysicsMaterial2D</returns>
    public PhysicsMaterial2D CreatePhysicsMaterial(string materialName, float bounceValue, float frictionValue)
    {
        PhysicsMaterial2D material = new PhysicsMaterial2D(materialName)
        {
            bounciness = bounceValue,
            friction = frictionValue,
            frictionCombine = frictionCombine,
            bounceCombine = bounceCombine
        };
        
        Debug.Log($"[BoundaryPhysicsMaterial] Created physics material: {materialName} (Bounce: {bounceValue:F2}, Friction: {frictionValue:F2})");
        return material;
    }
    
    /// <summary>
    /// Creates and applies a default arcade-style physics material.
    /// </summary>
    private void CreateAndApplyDefaultMaterial()
    {
        wallMaterial = CreatePhysicsMaterial("DefaultBoundaryMaterial", bounciness, friction);
        ApplyPhysicsMaterial(wallMaterial);
    }
    
    /// <summary>
    /// Applies physics material to all boundary wall colliders.
    /// </summary>
    /// <param name="material">PhysicsMaterial2D to apply</param>
    public void ApplyPhysicsMaterial(PhysicsMaterial2D material)
    {
        if (material == null)
        {
            Debug.LogError("[BoundaryPhysicsMaterial] Cannot apply null physics material");
            return;
        }
        
        wallsWithMaterials = 0;
        
        foreach (BoundaryWall wall in boundaryWalls)
        {
            if (ApplyMaterialToWall(wall, material))
            {
                wallsWithMaterials++;
            }
        }
        
        appliedMaterial = material;
        
        Debug.Log($"[BoundaryPhysicsMaterial] Applied physics material to {wallsWithMaterials}/{boundaryWalls.Count} walls");
        
        if (enablePhysicsValidation)
        {
            ValidatePhysicsConfiguration();
        }
    }
    
    /// <summary>
    /// Applies physics material to a specific boundary wall.
    /// </summary>
    /// <param name="wall">BoundaryWall to apply material to</param>
    /// <param name="material">PhysicsMaterial2D to apply</param>
    /// <returns>True if material was applied successfully</returns>
    private bool ApplyMaterialToWall(BoundaryWall wall, PhysicsMaterial2D material)
    {
        if (wall == null) return false;
        
        Collider2D collider = wall.GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning($"[BoundaryPhysicsMaterial] No collider found on {wall.wallType} boundary wall");
            return false;
        }
        
        // Check if collision is enabled for this wall
        BoundaryWallConfig wallConfig = wall.config != null ? 
            wall.config.GetBoundaryConfig(wall.wallType) : 
            BoundaryWallConfig.CreateDefault(wall.wallType);
        
        if (!wallConfig.enableCollision)
        {
            Debug.Log($"[BoundaryPhysicsMaterial] Skipping {wall.wallType} wall - collision disabled");
            return false;
        }
        
        collider.sharedMaterial = material;
        Debug.Log($"[BoundaryPhysicsMaterial] Applied material to {wall.wallType} boundary wall");
        return true;
    }
    
    /// <summary>
    /// Updates physics material properties on the current material.
    /// </summary>
    public void UpdateMaterialProperties()
    {
        if (wallMaterial == null)
        {
            Debug.LogWarning("[BoundaryPhysicsMaterial] No physics material to update");
            return;
        }
        
        wallMaterial.bounciness = bounciness;
        wallMaterial.friction = friction;
        wallMaterial.frictionCombine = frictionCombine;
        wallMaterial.bounceCombine = bounceCombine;
        
        Debug.Log($"[BoundaryPhysicsMaterial] Updated material properties - Bounce: {bounciness:F2}, Friction: {friction:F2}");
        
        // Reapply to ensure changes take effect
        if (appliedMaterial == wallMaterial)
        {
            ApplyPhysicsMaterial(wallMaterial);
        }
    }
    
    #endregion
    
    #region Validation
    
    /// <summary>
    /// Validates the physics configuration for proper arcade bouncing.
    /// </summary>
    public bool ValidatePhysicsConfiguration()
    {
        isPhysicsValid = true;
        
        // Validate material exists
        if (appliedMaterial == null)
        {
            Debug.LogError("[BoundaryPhysicsMaterial] No physics material applied");
            isPhysicsValid = false;
            return false;
        }
        
        // Validate bounciness for arcade style
        if (appliedMaterial.bounciness < 0.95f || appliedMaterial.bounciness > 1.05f)
        {
            Debug.LogWarning($"[BoundaryPhysicsMaterial] Bounciness {appliedMaterial.bounciness:F2} may not provide arcade-style bouncing");
        }
        
        // Validate friction is minimal
        if (appliedMaterial.friction > 0.1f)
        {
            Debug.LogWarning($"[BoundaryPhysicsMaterial] Friction {appliedMaterial.friction:F2} may slow ball unnecessarily");
        }
        
        // Validate combine modes
        if (appliedMaterial.bounceCombine != PhysicsMaterialCombine2D.Maximum)
        {
            Debug.LogWarning("[BoundaryPhysicsMaterial] Bounce combine mode should be Maximum for consistent bouncing");
        }
        
        // Validate wall application
        if (wallsWithMaterials < boundaryWalls.Count(w => w.config == null || w.config.GetBoundaryConfig(w.wallType).enableCollision))
        {
            Debug.LogWarning($"[BoundaryPhysicsMaterial] Not all collision-enabled walls have physics materials");
            isPhysicsValid = false;
        }
        
        Debug.Log($"[BoundaryPhysicsMaterial] Physics validation {(isPhysicsValid ? "PASSED" : "FAILED")}");
        return isPhysicsValid;
    }
    
    /// <summary>
    /// Tests bounce behavior with a simulated ball collision.
    /// </summary>
    /// <param name="incomingVelocity">Incoming ball velocity</param>
    /// <param name="wallNormal">Wall normal vector</param>
    /// <returns>Expected outgoing velocity after bounce</returns>
    public Vector2 TestBounceCalculation(Vector2 incomingVelocity, Vector2 wallNormal)
    {
        // Perfect elastic collision formula: v' = v - 2(v·n)n
        float dotProduct = Vector2.Dot(incomingVelocity, wallNormal);
        Vector2 outgoingVelocity = incomingVelocity - 2f * dotProduct * wallNormal;
        
        // Apply bounciness factor
        outgoingVelocity *= bounciness;
        
        return outgoingVelocity;
    }
    
    /// <summary>
    /// Validates bounce angle preservation for arcade gameplay.
    /// </summary>
    /// <param name="incomingAngle">Incoming angle in degrees</param>
    /// <param name="outgoingAngle">Outgoing angle in degrees</param>
    /// <returns>True if angles match within tolerance</returns>
    public bool ValidateBounceAngle(float incomingAngle, float outgoingAngle)
    {
        float expectedAngle = 180f - incomingAngle; // Mirror angle
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(outgoingAngle, expectedAngle));
        
        bool isValid = angleDifference <= angleTolerance;
        
        if (!isValid && enablePhysicsValidation)
        {
            Debug.LogWarning($"[BoundaryPhysicsMaterial] Bounce angle validation failed - Expected: {expectedAngle:F1}°, Got: {outgoingAngle:F1}° (Diff: {angleDifference:F1}°)");
        }
        
        return isValid;
    }
    
    /// <summary>
    /// Validates velocity magnitude preservation for energy conservation.
    /// </summary>
    /// <param name="incomingSpeed">Incoming velocity magnitude</param>
    /// <param name="outgoingSpeed">Outgoing velocity magnitude</param>
    /// <returns>True if speeds match within tolerance</returns>
    public bool ValidateVelocityMagnitude(float incomingSpeed, float outgoingSpeed)
    {
        float expectedSpeed = incomingSpeed * bounciness;
        float speedRatio = outgoingSpeed / expectedSpeed;
        
        bool isValid = Mathf.Abs(1f - speedRatio) <= velocityTolerance;
        
        if (!isValid && enablePhysicsValidation)
        {
            Debug.LogWarning($"[BoundaryPhysicsMaterial] Velocity magnitude validation failed - Expected: {expectedSpeed:F2}, Got: {outgoingSpeed:F2} (Ratio: {speedRatio:F2})");
        }
        
        return isValid;
    }
    
    #endregion
    
    #region Component Management
    
    /// <summary>
    /// Caches references to boundary walls and configuration.
    /// </summary>
    private void CacheBoundaryReferences()
    {
        // Find boundary configuration
        boundaryConfig = Resources.Load<BoundaryConfig>("BoundaryConfig");
        
        // Find all boundary walls in scene
        boundaryWalls.Clear();
        boundaryWalls.AddRange(FindObjectsOfType<BoundaryWall>());
        
        Debug.Log($"[BoundaryPhysicsMaterial] Found {boundaryWalls.Count} boundary walls");
    }
    
    /// <summary>
    /// Validates initial configuration and setup.
    /// </summary>
    private void ValidateConfiguration()
    {
        if (boundaryWalls.Count == 0)
        {
            Debug.LogError("[BoundaryPhysicsMaterial] No boundary walls found - run boundary wall setup first");
        }
        
        if (boundaryConfig == null)
        {
            Debug.LogWarning("[BoundaryPhysicsMaterial] No boundary configuration found - using default values");
        }
    }
    
    /// <summary>
    /// Refreshes boundary wall references and reapplies materials.
    /// </summary>
    public void RefreshBoundaryMaterials()
    {
        CacheBoundaryReferences();
        
        if (appliedMaterial != null)
        {
            ApplyPhysicsMaterial(appliedMaterial);
        }
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Gets the currently applied physics material.
    /// </summary>
    /// <returns>Applied PhysicsMaterial2D or null</returns>
    public PhysicsMaterial2D GetAppliedMaterial()
    {
        return appliedMaterial;
    }
    
    /// <summary>
    /// Gets the number of walls with physics materials applied.
    /// </summary>
    /// <returns>Count of walls with materials</returns>
    public int GetWallsWithMaterials()
    {
        return wallsWithMaterials;
    }
    
    /// <summary>
    /// Checks if physics configuration is valid.
    /// </summary>
    /// <returns>True if physics is properly configured</returns>
    public bool IsPhysicsValid()
    {
        return isPhysicsValid;
    }
    
    /// <summary>
    /// Gets a summary of the physics configuration.
    /// </summary>
    /// <returns>Configuration summary string</returns>
    public string GetPhysicsSummary()
    {
        return $"BoundaryPhysicsMaterial Configuration:\n" +
               $"• Applied Material: {(appliedMaterial != null ? appliedMaterial.name : "None")}\n" +
               $"• Bounciness: {bounciness:F2}\n" +
               $"• Friction: {friction:F2}\n" +
               $"• Walls with Materials: {wallsWithMaterials}/{boundaryWalls.Count}\n" +
               $"• Friction Combine: {frictionCombine}\n" +
               $"• Bounce Combine: {bounceCombine}\n" +
               $"• Validation: {(isPhysicsValid ? "PASSED" : "FAILED")}";
    }
    
    #endregion
    
    #region Editor Support
    
    /// <summary>
    /// Called when values change in Inspector (Editor only).
    /// </summary>
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        
        if (wallMaterial != null && appliedMaterial == wallMaterial)
        {
            UpdateMaterialProperties();
        }
    }
    
    #endregion
}