using UnityEngine;

/// <summary>
/// Core MonoBehaviour component for individual brick behavior and state management.
/// Manages brick lifecycle, configuration, and provides framework for collision detection
/// and destruction systems. Integrates with BrickData for configuration-driven behavior.
/// </summary>
// [RequireComponent(typeof(Collider2D))] // Temporarily disabled for testing
public class Brick : MonoBehaviour
{
    #region Destruction Events
    
    /// <summary>
    /// Global static event triggered when any brick is destroyed.
    /// Useful for system-wide listeners like scoring, level progression, and collision management.
    /// </summary>
    public static System.Action<Brick> OnBrickDestroyed;
    
    /// <summary>
    /// Instance-specific event triggered when this specific brick is destroyed.
    /// Useful for localized effects, particle systems, and audio components attached to this brick.
    /// </summary>
    public System.Action<Brick> OnThisBrickDestroyed;
    
    #endregion
    #region Serialized Fields
    
    [Header("Configuration")]
    [Tooltip("Brick configuration data defining behavior, scoring, and visual properties")]
    [SerializeField] private BrickData brickData;
    
    [Header("State Management")]
    [Tooltip("Current hit points remaining before destruction")]
    [SerializeField] private int currentHitPoints;
    
    [Tooltip("Whether this brick has been destroyed and is pending cleanup")]
    [SerializeField] private bool isDestroyed = false;
    
    [Tooltip("Whether this brick has been properly initialized")]
    [SerializeField] private bool isInitialized = false;
    
    [Header("Debug Information")]
    [Tooltip("Current brick type for runtime inspection")]
    [SerializeField] private BrickType currentBrickType;
    
    [Tooltip("Original hit points for damage calculation")]
    [SerializeField] private int originalHitPoints;
    
    [Tooltip("Enable debug logging for this brick instance")]
    [SerializeField] private bool enableDebugLogging = false;
    
    [Header("Collision Detection")]
    [Tooltip("Layer mask for ball collision filtering - only objects on these layers will trigger brick hits")]
    [SerializeField] private LayerMask ballLayerMask = -1; // Default: all layers
    
    [Tooltip("Alternative ball identification using GameObject tags")]
    [SerializeField] private string ballTag = "Ball";
    
    [Tooltip("Enable collision timestamping for debugging simultaneous hits")]
    [SerializeField] private bool enableCollisionTimestamps = false;
    
    [Header("Destruction Mechanics")]
    [Tooltip("Delay before GameObject destruction for visual effects coordination")]
    [Range(0.0f, 2.0f)]
    [SerializeField] private float destructionDelay = 0.1f;
    
    [Tooltip("Enable destruction event notifications to external systems")]
    [SerializeField] private bool enableDestructionEvents = true;
    
    [Tooltip("Enable detailed destruction logging for debugging")]
    [SerializeField] private bool enableDestructionLogging = false;
    
    #endregion
    
    #region Private Fields
    
    // Component references cached during initialization
    private Collider2D brickCollider;
    private Renderer brickRenderer;
    private SpriteRenderer spriteRenderer;
    
    // State tracking
    private bool awakeCompleted = false;
    private bool startCompleted = false;
    
    // Collision tracking
    private CollisionManager collisionManager;
    private float lastCollisionTime = 0f;
    private int collisionCount = 0;
    private bool collisionSystemInitialized = false;
    
    // Destruction tracking
    private bool destructionTriggered = false;
    private float destructionStartTime = 0f;
    private bool cleanupCompleted = false;
    
    #endregion
    
    #region Public Properties
    
    /// <summary>
    /// Gets the current brick configuration data (read-only)
    /// </summary>
    public BrickData BrickData => brickData;
    
    /// <summary>
    /// Gets the current hit points remaining
    /// </summary>
    public int CurrentHitPoints => currentHitPoints;
    
    /// <summary>
    /// Gets whether this brick has been destroyed
    /// </summary>
    public bool IsDestroyed => isDestroyed;
    
    /// <summary>
    /// Gets whether this brick has been properly initialized
    /// </summary>
    public bool IsInitialized => isInitialized;
    
    /// <summary>
    /// Gets the current brick type
    /// </summary>
    public BrickType BrickType => currentBrickType;
    
    /// <summary>
    /// Gets whether this brick can be destroyed by collision
    /// </summary>
    public bool IsDestructible => brickData != null && brickData.IsDestructible() && !isDestroyed;
    
    /// <summary>
    /// Gets the damage remaining (percentage of original hit points)
    /// </summary>
    public float DamagePercentage 
    { 
        get 
        { 
            if (originalHitPoints <= 0) return 0f;
            return 1f - ((float)currentHitPoints / originalHitPoints);
        } 
    }
    
    /// <summary>
    /// Gets the current collision detection configuration
    /// </summary>
    public LayerMask BallLayerMask => ballLayerMask;
    
    /// <summary>
    /// Gets the total number of collisions this brick has experienced
    /// </summary>
    public int CollisionCount => collisionCount;
    
    /// <summary>
    /// Gets the time of the last collision in Time.time
    /// </summary>
    public float LastCollisionTime => lastCollisionTime;
    
    /// <summary>
    /// Gets whether collision system has been initialized
    /// </summary>
    public bool CollisionSystemInitialized => collisionSystemInitialized;
    
    /// <summary>
    /// Gets whether brick destruction has been triggered
    /// </summary>
    public bool DestructionTriggered => destructionTriggered;
    
    /// <summary>
    /// Gets the time when destruction was triggered (Time.time)
    /// </summary>
    public float DestructionStartTime => destructionStartTime;
    
    /// <summary>
    /// Gets whether cleanup has been completed
    /// </summary>
    public bool CleanupCompleted => cleanupCompleted;
    
    /// <summary>
    /// Gets the configured destruction delay
    /// </summary>
    public float DestructionDelay => destructionDelay;
    
    #endregion
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize component references and prepare for configuration.
    /// Called once when component is created.
    /// </summary>
    private void Awake()
    {
        LogDebug("[Brick] Awake() - Caching component references");
        
        // Cache component references for performance
        CacheComponentReferences();
        
        // Validate GameObject setup
        ValidateGameObjectSetup();
        
        awakeCompleted = true;
        LogDebug("[Brick] Awake() completed successfully");
    }
    
    /// <summary>
    /// Complete initialization if not already done externally.
    /// Called after all objects are initialized.
    /// </summary>
    private void Start()
    {
        LogDebug("[Brick] Start() - Completing initialization");
        
        // Initialize with default data if not already initialized
        if (!isInitialized && brickData != null)
        {
            Initialize(brickData);
        }
        else if (!isInitialized)
        {
            // Create default brick data if none provided
            LogWarning("[Brick] No BrickData assigned. Creating default Normal brick configuration.");
            Initialize(BrickData.CreateNormal());
        }
        
        // Initialize collision detection system
        InitializeCollisionSystem();
        
        startCompleted = true;
        LogDebug("[Brick] Start() completed successfully");
    }
    
    /// <summary>
    /// Unity OnDestroy callback - framework method for cleanup logic.
    /// Override this method in future tasks for destruction effects.
    /// </summary>
    private void OnDestroy()
    {
        LogDestructionDebug("[Brick] OnDestroy() - Unity cleanup callback");
        
        try
        {
            // Ensure cleanup is completed if not done during destruction sequence
            if (!cleanupCompleted)
            {
                LogDestructionDebug("[Brick] OnDestroy() performing final cleanup");
                CleanupReferences();
            }
            
            // Final event cleanup - remove this brick from static events if still subscribed
            // This is a safety measure in case normal cleanup didn't complete
            if (OnBrickDestroyed != null)
            {
                // We can't easily remove specific instances from static events,
                // but Unity's garbage collection will handle this properly
                LogDestructionDebug("[Brick] OnDestroy() - Static event cleanup handled by garbage collection");
            }
            
            LogDestructionDebug("[Brick] OnDestroy() cleanup completed successfully");
        }
        catch (System.Exception e)
        {
            // Use Debug.LogError directly since our logging might not work during destruction
            Debug.LogError($"[Brick] Error during OnDestroy cleanup: {e.Message}");
        }
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Initializes brick with specified configuration data.
    /// Validates configuration and sets up initial state.
    /// </summary>
    /// <param name="data">Brick configuration data</param>
    public void Initialize(BrickData data)
    {
        LogDebug($"[Brick] Initialize() called with data: {data?.brickType}");
        
        // Validation
        if (data == null)
        {
            LogError("[Brick] Initialize() called with null BrickData. Using default Normal configuration.");
            data = BrickData.CreateNormal();
        }
        
        // Validate and fix configuration if needed
        data.ValidateConfiguration();
        
        // Store configuration
        brickData = data.Clone(); // Use clone to prevent external modification
        
        // Initialize state from configuration
        InitializeStateFromData();
        
        // Apply visual configuration
        ApplyVisualConfiguration();
        
        // Mark as initialized
        isInitialized = true;
        
        LogDebug($"[Brick] Successfully initialized as {currentBrickType} brick with {currentHitPoints} HP");
    }
    
    /// <summary>
    /// Framework method for collision detection - prepared for future implementation.
    /// This method will be expanded in collision detection tasks.
    /// </summary>
    public void OnCollisionDetected()
    {
        LogDebug("[Brick] OnCollisionDetected() - Framework method called");
        
        // Framework method - prepared for future collision logic implementation
        // Future tasks will implement:
        // - Hit point reduction
        // - Damage state updates
        // - Collision feedback triggering
        // - Destruction checking
        
        if (!IsInitialized)
        {
            LogWarning("[Brick] OnCollisionDetected() called on uninitialized brick");
            return;
        }
        
        if (IsDestroyed)
        {
            LogDebug("[Brick] OnCollisionDetected() called on destroyed brick - ignoring");
            return;
        }
        
        if (!IsDestructible)
        {
            LogDebug("[Brick] OnCollisionDetected() called on indestructible brick");
            return;
        }
        
        // Placeholder for collision handling logic
        LogDebug($"[Brick] Collision detected on {currentBrickType} brick (HP: {currentHitPoints})");
    }
    
    /// <summary>
    /// Framework method for manual brick destruction - prepared for future implementation.
    /// This method will be expanded in destruction logic tasks.
    /// </summary>
    public void DestroyBrick()
    {
        LogDestructionDebug("[Brick] DestroyBrick() - Legacy framework method called, routing to new destruction system");
        
        // Route to new destruction system for proper handling
        if (!destructionTriggered)
        {
            // Force destruction regardless of hit points for manual calls
            currentHitPoints = 0;
            ProcessDestruction();
        }
        else
        {
            LogDestructionDebug("[Brick] DestroyBrick() called but destruction already in progress");
        }
    }
    
    /// <summary>
    /// Resets brick to initial state with current configuration.
    /// Useful for level restart or brick pool recycling.
    /// </summary>
    public void ResetBrick()
    {
        LogDebug("[Brick] ResetBrick() - Resetting to initial state");
        
        if (!isInitialized || brickData == null)
        {
            LogWarning("[Brick] Cannot reset uninitialized brick");
            return;
        }
        
        // Reset state
        isDestroyed = false;
        InitializeStateFromData();
        ApplyVisualConfiguration();
        
        LogDebug($"[Brick] Brick reset successfully: {currentBrickType} with {currentHitPoints} HP");
    }
    
    /// <summary>
    /// Updates brick configuration with new data.
    /// Preserves current damage state if applicable.
    /// </summary>
    /// <param name="newData">New brick configuration</param>
    /// <param name="preserveDamage">Whether to preserve current damage percentage</param>
    public void UpdateConfiguration(BrickData newData, bool preserveDamage = false)
    {
        LogDebug($"[Brick] UpdateConfiguration() called with {newData?.brickType}, preserve damage: {preserveDamage}");
        
        if (newData == null)
        {
            LogError("[Brick] UpdateConfiguration() called with null data");
            return;
        }
        
        float currentDamagePercentage = preserveDamage ? DamagePercentage : 0f;
        
        // Update configuration
        Initialize(newData);
        
        // Restore damage state if requested
        if (preserveDamage && currentDamagePercentage > 0f)
        {
            int damageAmount = Mathf.RoundToInt(originalHitPoints * currentDamagePercentage);
            currentHitPoints = Mathf.Max(1, originalHitPoints - damageAmount);
            LogDebug($"[Brick] Preserved damage: {currentDamagePercentage:P0} -> {currentHitPoints} HP");
        }
    }
    
    #endregion
    
    #region Collision Detection System
    
    /// <summary>
    /// Unity collision event handler - detects ball collisions and processes hits.
    /// Only processes collisions from objects on the ball layer mask.
    /// </summary>
    /// <param name="collision">Collision data from Unity physics system</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        LogDebug($"[Brick] OnCollisionEnter2D triggered with {collision.gameObject.name}");
        
        // Validate collision and ensure brick is ready for collision processing
        if (!ValidateBallCollision(collision))
        {
            LogDebug($"[Brick] Collision with {collision.gameObject.name} rejected by validation");
            return;
        }
        
        // Process the valid ball hit
        ProcessBallHit(collision);
    }
    
    /// <summary>
    /// Validates that collision is from a ball and brick is ready for collision processing.
    /// Uses layer mask filtering and tag-based fallback for reliable ball detection.
    /// </summary>
    /// <param name="collision">Collision data to validate</param>
    /// <returns>True if collision should be processed as ball hit</returns>
    private bool ValidateBallCollision(Collision2D collision)
    {
        // Early exit if brick is not ready for collisions
        if (!isInitialized || isDestroyed || !IsDestructible)
        {
            LogDebug($"[Brick] Collision validation failed - Initialized: {isInitialized}, Destroyed: {isDestroyed}, Destructible: {IsDestructible}");
            return false;
        }
        
        // Validate collision object exists
        if (collision == null || collision.gameObject == null)
        {
            LogWarning("[Brick] Collision validation failed - null collision or GameObject");
            return false;
        }
        
        GameObject collisionObject = collision.gameObject;
        
        // Primary validation: Layer mask filtering
        int objectLayer = collisionObject.layer;
        bool layerMatch = (ballLayerMask.value & (1 << objectLayer)) != 0;
        
        if (!layerMatch)
        {
            // Fallback validation: Tag-based identification
            if (!string.IsNullOrEmpty(ballTag) && collisionObject.CompareTag(ballTag))
            {
                LogDebug($"[Brick] Collision validated using tag fallback: {ballTag}");
                return true;
            }
            
            LogDebug($"[Brick] Collision rejected - Layer {objectLayer} not in mask {ballLayerMask.value}, Tag: {collisionObject.tag}");
            return false;
        }
        
        LogDebug($"[Brick] Collision validated - Layer {objectLayer} matches mask {ballLayerMask.value}");
        return true;
    }
    
    /// <summary>
    /// Processes valid ball hit by reducing hit points and coordinating with CollisionManager.
    /// Handles hit point reduction, destruction triggering, and collision event communication.
    /// </summary>
    /// <param name="collision">Validated collision data from ball</param>
    private void ProcessBallHit(Collision2D collision)
    {
        LogDebug($"[Brick] Processing ball hit from {collision.gameObject.name}");
        
        // Update collision tracking
        UpdateCollisionTracking();
        
        // Reduce hit points for destructible bricks
        if (IsDestructible && currentHitPoints > 0)
        {
            int previousHitPoints = currentHitPoints;
            currentHitPoints = Mathf.Max(0, currentHitPoints - 1);
            
            LogDebug($"[Brick] Hit points reduced: {previousHitPoints} -> {currentHitPoints}");
            
            // Check if brick should be destroyed
            if (currentHitPoints <= 0)
            {
                LogDebug($"[Brick] Hit points depleted - triggering destruction");
                TriggerBrickDestruction();
            }
        }
        
        // Communicate collision to CollisionManager for system coordination
        CommunicateCollisionToManager(collision);
        
        // Update framework method for future integration
        OnCollisionDetected();
    }
    
    /// <summary>
    /// Communicates collision event to CollisionManager for centralized handling.
    /// Provides collision data for feedback systems, scoring, and collision tracking.
    /// </summary>
    /// <param name="collision">Collision data to communicate</param>
    private void CommunicateCollisionToManager(Collision2D collision)
    {
        if (collisionManager == null)
        {
            LogDebug("[Brick] CollisionManager not available - skipping collision communication");
            return;
        }
        
        try
        {
            // Get collision contact information
            Vector2 contactPoint = collision.contacts.Length > 0 ? 
                collision.contacts[0].point : 
                (Vector2)transform.position;
            
            // Calculate collision intensity based on relative velocity
            float intensity = collision.relativeVelocity.magnitude / 20f; // Normalize to 0-1 range
            intensity = Mathf.Clamp01(intensity);
            
            // Communicate collision details to manager
            // Note: This assumes CollisionManager has appropriate methods for brick collisions
            LogDebug($"[Brick] Communicating collision to CollisionManager - Contact: {contactPoint}, Intensity: {intensity:F2}");
            
            // Framework for future CollisionManager integration
            // collisionManager.HandleBrickCollision(this, collision, contactPoint, intensity);
            
        }
        catch (System.Exception e)
        {
            LogError($"[Brick] Failed to communicate collision to CollisionManager: {e.Message}");
        }
    }
    
    /// <summary>
    /// Triggers brick destruction when hit points are depleted.
    /// Validates destruction state and calls ProcessDestruction() for full destruction logic.
    /// </summary>
    private void TriggerBrickDestruction()
    {
        LogDestructionDebug($"[Brick] TriggerBrickDestruction() called for {currentBrickType} brick");
        
        // Validate destruction can be triggered
        if (!CanTriggerDestruction())
        {
            LogDestructionDebug("[Brick] Destruction trigger rejected by validation");
            return;
        }
        
        LogDestructionDebug($"[Brick] Triggering destruction for {currentBrickType} brick");
        ProcessDestruction();
    }
    
    /// <summary>
    /// Public method to manually trigger destruction with validation.
    /// Validates hit points and destruction state before processing.
    /// </summary>
    public void TriggerDestruction()
    {
        LogDestructionDebug("[Brick] TriggerDestruction() called manually");
        
        if (currentHitPoints <= 0 && !isDestroyed && !destructionTriggered)
        {
            ProcessDestruction();
        }
        else
        {
            LogDestructionDebug($"[Brick] Manual destruction rejected - HP: {currentHitPoints}, Destroyed: {isDestroyed}, Triggered: {destructionTriggered}");
        }
    }
    
    /// <summary>
    /// Validates whether destruction can be triggered based on current state.
    /// Prevents multiple destruction calls and validates hit point requirements.
    /// </summary>
    /// <returns>True if destruction can be triggered</returns>
    private bool CanTriggerDestruction()
    {
        // Check if already destroyed or destruction already triggered
        if (isDestroyed || destructionTriggered)
        {
            LogDestructionDebug($"[Brick] Destruction already in progress - Destroyed: {isDestroyed}, Triggered: {destructionTriggered}");
            return false;
        }
        
        // Check if brick is initialized
        if (!isInitialized)
        {
            LogWarning("[Brick] Cannot destroy uninitialized brick");
            return false;
        }
        
        // Check if brick is destructible
        if (!IsDestructible)
        {
            LogDestructionDebug($"[Brick] Brick is not destructible - Type: {currentBrickType}");
            return false;
        }
        
        // For hit point validation, allow destruction if HP is 0 or below
        if (currentHitPoints > 0)
        {
            LogDestructionDebug($"[Brick] Hit points remaining: {currentHitPoints} - destruction not yet required");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Processes brick destruction with event notification and cleanup.
    /// Handles the complete destruction sequence including events and GameObject removal.
    /// </summary>
    private void ProcessDestruction()
    {
        LogDestructionDebug("[Brick] ProcessDestruction() - Beginning destruction sequence");
        
        // Mark destruction as triggered immediately
        destructionTriggered = true;
        destructionStartTime = Time.time;
        isDestroyed = true;
        
        try
        {
            // Notify destruction systems before cleanup
            NotifyDestructionSystems();
            
            // Clean up references and subscriptions
            CleanupReferences();
            
            // Destroy GameObject with configured delay
            DestroyGameObject();
            
            LogDestructionDebug($"[Brick] Destruction sequence completed for {currentBrickType} brick");
        }
        catch (System.Exception e)
        {
            LogError($"[Brick] Error during destruction sequence: {e.Message}");
            
            // Ensure GameObject is destroyed even if cleanup fails
            try
            {
                DestroyGameObject();
            }
            catch (System.Exception destroyException)
            {
                LogError($"[Brick] Critical error: Failed to destroy GameObject: {destroyException.Message}");
            }
        }
    }
    
    /// <summary>
    /// Notifies external systems of brick destruction through event system.
    /// Fires both static global events and instance-specific events safely.
    /// </summary>
    private void NotifyDestructionSystems()
    {
        if (!enableDestructionEvents)
        {
            LogDestructionDebug("[Brick] Destruction event notifications disabled");
            return;
        }
        
        LogDestructionDebug("[Brick] Notifying destruction systems via events");
        
        try
        {
            // Fire static global event for system-wide listeners
            if (OnBrickDestroyed != null)
            {
                LogDestructionDebug($"[Brick] Notifying {OnBrickDestroyed.GetInvocationList().Length} global destruction subscribers");
                OnBrickDestroyed.Invoke(this);
            }
            else
            {
                LogDestructionDebug("[Brick] No global destruction subscribers registered");
            }
        }
        catch (System.Exception e)
        {
            LogError($"[Brick] Error invoking global destruction event: {e.Message}");
        }
        
        try
        {
            // Fire instance-specific event for localized listeners
            if (OnThisBrickDestroyed != null)
            {
                LogDestructionDebug($"[Brick] Notifying {OnThisBrickDestroyed.GetInvocationList().Length} instance destruction subscribers");
                OnThisBrickDestroyed.Invoke(this);
            }
            else
            {
                LogDestructionDebug("[Brick] No instance destruction subscribers registered");
            }
        }
        catch (System.Exception e)
        {
            LogError($"[Brick] Error invoking instance destruction event: {e.Message}");
        }
        
        LogDestructionDebug("[Brick] Destruction system notification completed");
    }
    
    /// <summary>
    /// Cleans up component references and event subscriptions to prevent memory leaks.
    /// Ensures proper memory management during brick destruction.
    /// </summary>
    private void CleanupReferences()
    {
        LogDestructionDebug("[Brick] Beginning reference cleanup");
        
        try
        {
            // Clear component references
            brickCollider = null;
            brickRenderer = null;
            spriteRenderer = null;
            collisionManager = null;
            
            // Clear instance event subscribers to prevent memory leaks
            if (OnThisBrickDestroyed != null)
            {
                // Clear all delegates from instance event
                System.Delegate[] delegates = OnThisBrickDestroyed.GetInvocationList();
                foreach (System.Delegate del in delegates)
                {
                    OnThisBrickDestroyed -= (System.Action<Brick>)del;
                }
                LogDestructionDebug($"[Brick] Cleared {delegates.Length} instance event subscribers");
            }
            
            // Note: We don't clear static OnBrickDestroyed as other bricks may be using it
            
            cleanupCompleted = true;
            LogDestructionDebug("[Brick] Reference cleanup completed successfully");
        }
        catch (System.Exception e)
        {
            LogError($"[Brick] Error during reference cleanup: {e.Message}");
            // Continue with destruction even if cleanup fails partially
        }
    }
    
    /// <summary>
    /// Destroys the GameObject with configured delay for visual effects coordination.
    /// Uses Unity's Destroy method with timing control.
    /// </summary>
    private void DestroyGameObject()
    {
        LogDestructionDebug($"[Brick] Destroying GameObject with {destructionDelay:F2}s delay");
        
        try
        {
            if (destructionDelay > 0f)
            {
                // Delayed destruction for visual effects
                Destroy(gameObject, destructionDelay);
                LogDestructionDebug($"[Brick] GameObject destruction scheduled in {destructionDelay:F2} seconds");
            }
            else
            {
                // Immediate destruction
                Destroy(gameObject);
                LogDestructionDebug("[Brick] GameObject destroyed immediately");
            }
        }
        catch (System.Exception e)
        {
            LogError($"[Brick] Failed to destroy GameObject: {e.Message}");
        }
    }
    
    /// <summary>
    /// Updates collision tracking data for debugging and analytics.
    /// Tracks collision count, timing, and timestamps for development support.
    /// </summary>
    private void UpdateCollisionTracking()
    {
        collisionCount++;
        lastCollisionTime = Time.time;
        
        if (enableCollisionTimestamps)
        {
            LogDebug($"[Brick] Collision #{collisionCount} at time {lastCollisionTime:F3}");
        }
    }
    
    /// <summary>
    /// Initializes collision detection system during Start().
    /// Sets up CollisionManager integration and validates collision configuration.
    /// </summary>
    private void InitializeCollisionSystem()
    {
        LogDebug("[Brick] Initializing collision detection system...");
        
        // Find CollisionManager in scene
        FindCollisionManager();
        
        // Validate collision configuration
        ValidateCollisionConfiguration();
        
        // Reset collision tracking
        collisionCount = 0;
        lastCollisionTime = 0f;
        
        collisionSystemInitialized = true;
        LogDebug("[Brick] Collision detection system initialized successfully");
    }
    
    /// <summary>
    /// Finds and caches CollisionManager reference for collision communication.
    /// Uses singleton pattern first, falls back to scene search.
    /// </summary>
    private void FindCollisionManager()
    {
        // Try singleton pattern first (common in collision systems)
        if (CollisionManager.Instance != null)
        {
            collisionManager = CollisionManager.Instance;
            LogDebug("[Brick] CollisionManager found via singleton pattern");
            return;
        }
        
        // Fallback: Search in scene
        collisionManager = FindFirstObjectByType<CollisionManager>();
        
        if (collisionManager != null)
        {
            LogDebug($"[Brick] CollisionManager found in scene: {collisionManager.gameObject.name}");
        }
        else
        {
            LogWarning("[Brick] CollisionManager not found in scene. Collision coordination will be limited.");
            LogWarning("[Brick] Please ensure CollisionManager from Epic 1.1 is present in the scene.");
        }
    }
    
    /// <summary>
    /// Validates collision detection configuration and logs warnings for setup issues.
    /// Checks layer mask configuration, collider presence, and tag setup.
    /// </summary>
    private void ValidateCollisionConfiguration()
    {
        // Validate layer mask configuration
        if (ballLayerMask.value == 0)
        {
            LogWarning("[Brick] Ball layer mask is empty - no collisions will be detected via layer filtering");
        }
        else if (ballLayerMask.value == -1)
        {
            LogWarning("[Brick] Ball layer mask set to 'Everything' - consider setting specific ball layer for performance");
        }
        else
        {
            LogDebug($"[Brick] Ball layer mask configured: {ballLayerMask.value} (layers: {GetLayerNames(ballLayerMask)})");
        }
        
        // Validate tag fallback
        if (string.IsNullOrEmpty(ballTag))
        {
            LogWarning("[Brick] Ball tag not configured - layer mask will be the only collision filtering method");
        }
        else
        {
            LogDebug($"[Brick] Ball tag configured for fallback: '{ballTag}'");
        }
        
        // Validate collider presence
        if (brickCollider == null)
        {
            LogError("[Brick] No Collider2D found - collision detection will not work");
            LogError("[Brick] Please ensure brick GameObject has a Collider2D component");
        }
        else
        {
            LogDebug($"[Brick] Collider2D validated: {brickCollider.GetType().Name}");
        }
    }
    
    /// <summary>
    /// Gets human-readable layer names from layer mask for debugging.
    /// </summary>
    /// <param name="layerMask">Layer mask to analyze</param>
    /// <returns>Comma-separated layer names</returns>
    private string GetLayerNames(LayerMask layerMask)
    {
        System.Text.StringBuilder layerNames = new System.Text.StringBuilder();
        
        for (int i = 0; i < 32; i++)
        {
            if ((layerMask.value & (1 << i)) != 0)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    if (layerNames.Length > 0) layerNames.Append(", ");
                    layerNames.Append($"{layerName}({i})");
                }
            }
        }
        
        return layerNames.Length > 0 ? layerNames.ToString() : "None";
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// Caches component references for performance optimization
    /// </summary>
    private void CacheComponentReferences()
    {
        brickCollider = GetComponent<Collider2D>();
        brickRenderer = GetComponent<Renderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        LogDebug($"[Brick] Component references cached - Collider: {brickCollider != null}, Renderer: {brickRenderer != null}, SpriteRenderer: {spriteRenderer != null}");
    }
    
    /// <summary>
    /// Validates essential GameObject setup and components
    /// </summary>
    private void ValidateGameObjectSetup()
    {
        // Validate required Collider2D
        if (brickCollider == null)
        {
            LogError("[Brick] No Collider2D found! Brick requires Collider2D for collision detection.");
        }
        
        // Warn about missing visual components
        if (brickRenderer == null && spriteRenderer == null)
        {
            LogWarning("[Brick] No Renderer or SpriteRenderer found. Brick will not be visible.");
        }
        
        // Validate GameObject naming
        if (string.IsNullOrEmpty(gameObject.name) || gameObject.name == "GameObject")
        {
            LogWarning("[Brick] GameObject has default name. Consider renaming for better debugging.");
        }
    }
    
    /// <summary>
    /// Initializes brick state properties from BrickData configuration
    /// </summary>
    private void InitializeStateFromData()
    {
        currentBrickType = brickData.brickType;
        currentHitPoints = brickData.hitPoints;
        originalHitPoints = brickData.hitPoints;
        isDestroyed = false;
        
        LogDebug($"[Brick] State initialized: Type={currentBrickType}, HP={currentHitPoints}, Destroyed={isDestroyed}");
    }
    
    /// <summary>
    /// Applies visual configuration from BrickData to renderers
    /// </summary>
    private void ApplyVisualConfiguration()
    {
        if (brickData == null) return;
        
        // Apply color to available renderers
        if (spriteRenderer != null)
        {
            spriteRenderer.color = brickData.brickColor;
            LogDebug($"[Brick] Applied color {brickData.brickColor} to SpriteRenderer");
        }
        else if (brickRenderer != null && brickRenderer.material != null)
        {
            brickRenderer.material.color = brickData.brickColor;
            LogDebug($"[Brick] Applied color {brickData.brickColor} to Renderer material");
        }
        
        // Set GameObject name for debugging
        gameObject.name = $"Brick_{currentBrickType}_{GetInstanceID()}";
    }
    
    #endregion
    
    #region Debug and Logging
    
    /// <summary>
    /// Logs debug message if debug logging is enabled
    /// </summary>
    /// <param name="message">Debug message</param>
    private void LogDebug(string message)
    {
        if (enableDebugLogging)
        {
            Debug.Log(message);
        }
    }
    
    /// <summary>
    /// Logs destruction debug message if destruction logging is enabled
    /// </summary>
    /// <param name="message">Destruction debug message</param>
    private void LogDestructionDebug(string message)
    {
        if (enableDestructionLogging)
        {
            Debug.Log(message);
        }
    }
    
    /// <summary>
    /// Logs warning message (always shown)
    /// </summary>
    /// <param name="message">Warning message</param>
    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }
    
    /// <summary>
    /// Logs error message (always shown)
    /// </summary>
    /// <param name="message">Error message</param>
    private void LogError(string message)
    {
        Debug.LogError(message);
    }
    
    /// <summary>
    /// Returns detailed debug information about current brick state
    /// </summary>
    /// <returns>Debug information string</returns>
    public string GetDebugInfo()
    {
        if (brickData == null)
        {
            return "[Brick] Uninitialized brick (no BrickData)";
        }
        
        return $"[Brick] {currentBrickType} | HP: {currentHitPoints}/{originalHitPoints} | " +
               $"Destroyed: {isDestroyed} | Initialized: {isInitialized} | " +
               $"Damage: {DamagePercentage:P0} | Score: {brickData.scoreValue}";
    }
    
    /// <summary>
    /// Validates current brick state and reports any issues
    /// </summary>
    /// <returns>True if brick state is valid</returns>
    public bool ValidateState()
    {
        bool isValid = true;
        
        if (!awakeCompleted)
        {
            LogError("[Brick] Awake() has not completed properly");
            isValid = false;
        }
        
        if (!isInitialized)
        {
            LogWarning("[Brick] Brick has not been initialized");
            isValid = false;
        }
        
        if (brickData == null)
        {
            LogError("[Brick] BrickData is null");
            isValid = false;
        }
        else if (!brickData.IsValidConfiguration())
        {
            LogError($"[Brick] BrickData configuration is invalid: {brickData}");
            isValid = false;
        }
        
        if (currentHitPoints < 0)
        {
            LogError($"[Brick] Invalid hit points: {currentHitPoints}");
            isValid = false;
        }
        
        if (brickCollider == null)
        {
            LogError("[Brick] Missing required Collider2D component");
            isValid = false;
        }
        
        LogDebug($"[Brick] State validation result: {isValid}");
        return isValid;
    }
    
    #endregion
    
    #region Unity Editor Support
    
#if UNITY_EDITOR
    /// <summary>
    /// Called when component values change in Inspector (Editor only)
    /// </summary>
    private void OnValidate()
    {
        // Ensure hit points don't go below 0
        if (currentHitPoints < 0)
        {
            currentHitPoints = 0;
        }
        
        // Sync debug brick type with actual data
        if (brickData != null)
        {
            currentBrickType = brickData.brickType;
        }
        
        // Apply visual changes immediately in editor
        if (Application.isPlaying && isInitialized)
        {
            ApplyVisualConfiguration();
        }
    }
    
    /// <summary>
    /// Provides context menu commands for testing (Editor only)
    /// </summary>
    [ContextMenu("Initialize with Normal Brick")]
    private void InitializeWithNormal()
    {
        Initialize(BrickData.CreateNormal());
    }
    
    [ContextMenu("Initialize with Reinforced Brick")]
    private void InitializeWithReinforced()
    {
        Initialize(BrickData.CreateReinforced());
    }
    
    [ContextMenu("Initialize with PowerUp Brick")]
    private void InitializeWithPowerUp()
    {
        Initialize(BrickData.CreatePowerUp());
    }
    
    [ContextMenu("Test Collision Detection")]
    private void TestCollisionDetection()
    {
        OnCollisionDetected();
    }
    
    [ContextMenu("Test Brick Destruction")]
    private void TestBrickDestruction()
    {
        DestroyBrick();
    }
    
    [ContextMenu("Reset Brick State")]
    private void TestResetBrick()
    {
        ResetBrick();
    }
    
    [ContextMenu("Validate Brick State")]
    private void TestValidateState()
    {
        bool isValid = ValidateState();
        Debug.Log($"[Brick] Validation result: {isValid}\n{GetDebugInfo()}");
    }
#endif
    
    #endregion
}