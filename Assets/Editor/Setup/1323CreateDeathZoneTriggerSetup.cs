#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for creating Death Zone Trigger detection system.
/// Creates trigger GameObject with BoxCollider2D and DeathZoneTrigger component
/// configured for reliable ball collision detection with event system integration.
/// </summary>
public static class CreateDeathZoneTriggerSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Death Zone Trigger";
    private const string DEATHZONE_SYSTEM_NAME = "Death Zone System";
    private const string DEATHZONE_TRIGGER_NAME = "Death Zone Trigger";
    private const string DEATHZONE_CONFIG_PATH = "DeathZoneConfig";
    
    /// <summary>
    /// Creates death zone trigger detection system with collision handling.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateDeathZoneTrigger()
    {
        Debug.Log("🏗️ [Death Zone Trigger] Starting trigger detection system setup...");
        
        try
        {
            // Step 1: Validate prerequisites
            GameObject deathZoneSystem = ValidatePrerequisites();
            
            // Step 2: Create trigger GameObject
            GameObject triggerGameObject = CreateTriggerGameObject(deathZoneSystem);
            
            // Step 3: Add and configure BoxCollider2D
            BoxCollider2D triggerCollider = AddTriggerCollider(triggerGameObject);
            
            // Step 4: Add DeathZoneTrigger component
            DeathZoneTrigger triggerComponent = AddTriggerComponent(triggerGameObject);
            
            // Step 5: Configure trigger system
            ConfigureTriggerSystem(triggerComponent, triggerCollider, deathZoneSystem);
            
            // Step 6: Set up ball detection
            ConfigureBallDetection(triggerComponent);
            
            // Step 7: Initialize event system
            InitializeEventSystem(triggerComponent);
            
            // Step 8: Validate trigger setup
            ValidateTriggerSetup(triggerComponent);
            
            // Step 9: Save and log success
            SaveAndLogSuccess(triggerGameObject, triggerComponent);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Death Zone Trigger] Setup failed: {e.Message}");
            Debug.LogError($"📋 Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if positioning system exists and trigger doesn't exist.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateDeathZoneTrigger()
    {
        // Check if Death Zone System exists (prerequisite)
        GameObject deathZoneSystem = GameObject.Find(DEATHZONE_SYSTEM_NAME);
        if (deathZoneSystem == null)
        {
            return false;
        }
        
        // Check if Death Zone Trigger already exists
        Transform existingTrigger = deathZoneSystem.transform.Find(DEATHZONE_TRIGGER_NAME);
        return existingTrigger == null;
    }
    
    /// <summary>
    /// Validates that prerequisite systems exist.
    /// </summary>
    /// <returns>Death Zone System GameObject</returns>
    private static GameObject ValidatePrerequisites()
    {
        Debug.Log("🔍 [Death Zone Trigger] Validating prerequisites...");
        
        // Check for Death Zone System
        GameObject deathZoneSystem = GameObject.Find(DEATHZONE_SYSTEM_NAME);
        if (deathZoneSystem == null)
        {
            throw new System.Exception($"Death Zone System not found. Please run 'Breakout/Setup/Create Death Zone Positioning' first.");
        }
        
        // Check for DeathZonePositioning component
        DeathZonePositioning positioning = deathZoneSystem.GetComponent<DeathZonePositioning>();
        if (positioning == null)
        {
            throw new System.Exception("DeathZonePositioning component not found on Death Zone System. Please run positioning setup first.");
        }
        
        // Check for DeathZoneConfig
        DeathZoneConfig config = Resources.Load<DeathZoneConfig>(DEATHZONE_CONFIG_PATH);
        if (config == null)
        {
            Debug.LogWarning("⚠️ [Death Zone Trigger] DeathZoneConfig not found - trigger will use default settings");
        }
        
        Debug.Log("✅ [Death Zone Trigger] Prerequisites validated");
        return deathZoneSystem;
    }
    
    /// <summary>
    /// Creates the trigger GameObject as child of Death Zone System.
    /// </summary>
    /// <param name="parentSystem">Parent Death Zone System</param>
    /// <returns>Created trigger GameObject</returns>
    private static GameObject CreateTriggerGameObject(GameObject parentSystem)
    {
        Debug.Log("🏗️ [Death Zone Trigger] Creating trigger GameObject...");
        
        // Create trigger GameObject
        GameObject triggerObject = new GameObject(DEATHZONE_TRIGGER_NAME);
        
        // Set as child of Death Zone System
        triggerObject.transform.SetParent(parentSystem.transform);
        triggerObject.transform.localPosition = Vector3.zero;
        triggerObject.transform.localRotation = Quaternion.identity;
        triggerObject.transform.localScale = Vector3.one;
        
        // Set appropriate layer and tag
        triggerObject.layer = LayerMask.NameToLayer("Default");
        triggerObject.tag = "Untagged";
        
        Debug.Log($"✅ [Death Zone Trigger] Created GameObject: {triggerObject.name}");
        return triggerObject;
    }
    
    /// <summary>
    /// Adds and configures BoxCollider2D component for trigger detection.
    /// </summary>
    /// <param name="triggerObject">Trigger GameObject</param>
    /// <returns>Configured BoxCollider2D component</returns>
    private static BoxCollider2D AddTriggerCollider(GameObject triggerObject)
    {
        Debug.Log("🔧 [Death Zone Trigger] Adding trigger collider...");
        
        // Add BoxCollider2D component
        BoxCollider2D triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        
        if (triggerCollider == null)
        {
            throw new System.Exception("Failed to add BoxCollider2D component");
        }
        
        // Configure as trigger
        triggerCollider.isTrigger = true;
        
        // Set default trigger size (will be updated by trigger component)
        triggerCollider.size = new Vector2(20f, 2f);
        
        // Set trigger material to None for performance
        triggerCollider.sharedMaterial = null;
        
        Debug.Log("✅ [Death Zone Trigger] BoxCollider2D added and configured as trigger");
        return triggerCollider;
    }
    
    /// <summary>
    /// Adds DeathZoneTrigger component to the trigger GameObject.
    /// </summary>
    /// <param name="triggerObject">Trigger GameObject</param>
    /// <returns>DeathZoneTrigger component</returns>
    private static DeathZoneTrigger AddTriggerComponent(GameObject triggerObject)
    {
        Debug.Log("🔧 [Death Zone Trigger] Adding DeathZoneTrigger component...");
        
        DeathZoneTrigger triggerComponent = triggerObject.AddComponent<DeathZoneTrigger>();
        
        if (triggerComponent == null)
        {
            throw new System.Exception("Failed to add DeathZoneTrigger component");
        }
        
        Debug.Log("✅ [Death Zone Trigger] DeathZoneTrigger component added");
        return triggerComponent;
    }
    
    /// <summary>
    /// Configures the trigger system with references and settings.
    /// </summary>
    /// <param name="triggerComponent">DeathZoneTrigger component</param>
    /// <param name="triggerCollider">BoxCollider2D component</param>
    /// <param name="deathZoneSystem">Parent system GameObject</param>
    private static void ConfigureTriggerSystem(DeathZoneTrigger triggerComponent, BoxCollider2D triggerCollider, GameObject deathZoneSystem)
    {
        Debug.Log("⚙️ [Death Zone Trigger] Configuring trigger system...");
        
        // Set collider reference
        triggerComponent.triggerCollider = triggerCollider;
        
        // Set positioning reference
        DeathZonePositioning positioning = deathZoneSystem.GetComponent<DeathZonePositioning>();
        triggerComponent.positioning = positioning;
        
        // Load and set configuration
        DeathZoneConfig config = Resources.Load<DeathZoneConfig>(DEATHZONE_CONFIG_PATH);
        if (config != null)
        {
            triggerComponent.config = config;
            triggerComponent.triggerSize = config.triggerSize;
        }
        else
        {
            // Set default trigger size
            triggerComponent.triggerSize = new Vector2(20f, 2f);
        }
        
        // Configure trigger settings
        triggerComponent.autoUpdateTriggerSize = true;
        triggerComponent.enableTriggerCooldown = true;
        triggerComponent.triggerCooldownDuration = 0.5f;
        triggerComponent.enableValidation = true;
        
        EditorUtility.SetDirty(triggerComponent);
        
        Debug.Log("✅ [Death Zone Trigger] Trigger system configured");
    }
    
    /// <summary>
    /// Configures ball detection settings.
    /// </summary>
    /// <param name="triggerComponent">DeathZoneTrigger component</param>
    private static void ConfigureBallDetection(DeathZoneTrigger triggerComponent)
    {
        Debug.Log("🎮 [Death Zone Trigger] Configuring ball detection...");
        
        // Set ball detection parameters
        triggerComponent.ballLayer = 1; // Default layer
        triggerComponent.ballTag = "Ball";
        triggerComponent.useComponentDetection = true;
        triggerComponent.minimumVelocityThreshold = 0.1f;
        
        Debug.Log("✅ [Death Zone Trigger] Ball detection configured");
        Debug.Log("   • Ball Tag: 'Ball'");
        Debug.Log("   • Ball Layer: Default (Layer 0)");
        Debug.Log("   • Component Detection: Enabled");
        Debug.Log("   • Velocity Threshold: 0.1 units/second");
    }
    
    /// <summary>
    /// Initializes the event system for trigger notifications.
    /// </summary>
    /// <param name="triggerComponent">DeathZoneTrigger component</param>
    private static void InitializeEventSystem(DeathZoneTrigger triggerComponent)
    {
        Debug.Log("📡 [Death Zone Trigger] Initializing event system...");
        
        // Initialize UnityEvents (they're created automatically by component)
        // Events are ready for external system connections
        
        // Configure debug settings
        triggerComponent.showDebugGizmos = true;
        triggerComponent.enableDebugLogging = false; // Disabled by default for performance
        
        Debug.Log("✅ [Death Zone Trigger] Event system initialized");
        Debug.Log("   • UnityEvent: OnBallEnterDeathZone");
        Debug.Log("   • UnityEvent<GameObject>: OnBallEnterDeathZoneWithBall");
        Debug.Log("   • C# Event: BallLostEvent");
        Debug.Log("   • C# Event: DeathZoneTriggeredEvent");
    }
    
    /// <summary>
    /// Validates the created trigger detection system.
    /// </summary>
    /// <param name="triggerComponent">DeathZoneTrigger component to validate</param>
    private static void ValidateTriggerSetup(DeathZoneTrigger triggerComponent)
    {
        Debug.Log("🔍 [Death Zone Trigger] Validating trigger setup...");
        
        bool isValid = true;
        
        // Check trigger collider
        if (triggerComponent.triggerCollider == null)
        {
            Debug.LogError("   ❌ Trigger collider reference missing");
            isValid = false;
        }
        else if (!triggerComponent.triggerCollider.isTrigger)
        {
            Debug.LogError("   ❌ Collider is not configured as trigger");
            isValid = false;
        }
        
        // Check positioning reference
        if (triggerComponent.positioning == null)
        {
            Debug.LogWarning("   ⚠️ DeathZonePositioning reference missing - trigger position may not update");
        }
        
        // Check configuration reference
        if (triggerComponent.config == null)
        {
            Debug.LogWarning("   ⚠️ DeathZoneConfig reference missing - using default settings");
        }
        
        // Validate trigger size
        Vector2 triggerSize = triggerComponent.triggerSize;
        if (triggerSize.x <= 0f || triggerSize.y <= 0f)
        {
            Debug.LogError("   ❌ Invalid trigger size");
            isValid = false;
        }
        
        // Test event system initialization
        if (triggerComponent.OnBallEnterDeathZone == null)
        {
            Debug.LogWarning("   ⚠️ Unity Event not initialized");
        }
        
        // Validate ball detection settings
        if (string.IsNullOrEmpty(triggerComponent.ballTag))
        {
            Debug.LogWarning("   ⚠️ Ball tag not set - may affect ball detection");
        }
        
        if (isValid)
        {
            Debug.Log("✅ [Death Zone Trigger] Trigger setup validation passed");
        }
        else
        {
            Debug.LogWarning("⚠️ [Death Zone Trigger] Trigger setup validation issues detected");
        }
        
        // Log trigger status
        string status = triggerComponent.GetTriggerStatus();
        Debug.Log($"📋 [Death Zone Trigger] Trigger Status:\\n{status}");
    }
    
    /// <summary>
    /// Saves changes and logs successful setup.
    /// </summary>
    /// <param name="triggerObject">Created trigger GameObject</param>
    /// <param name="triggerComponent">DeathZoneTrigger component</param>
    private static void SaveAndLogSuccess(GameObject triggerObject, DeathZoneTrigger triggerComponent)
    {
        Debug.Log("💾 [Death Zone Trigger] Saving setup...");
        
        // Mark objects as dirty for saving
        EditorUtility.SetDirty(triggerObject);
        EditorUtility.SetDirty(triggerComponent);
        
        // Select the created trigger in hierarchy
        Selection.activeGameObject = triggerObject;
        EditorGUIUtility.PingObject(triggerObject);
        
        LogSuccessfulSetup(triggerObject, triggerComponent);
    }
    
    /// <summary>
    /// Logs successful setup with comprehensive usage instructions.
    /// </summary>
    /// <param name="triggerObject">Created trigger GameObject</param>
    /// <param name="triggerComponent">DeathZoneTrigger component</param>
    private static void LogSuccessfulSetup(GameObject triggerObject, DeathZoneTrigger triggerComponent)
    {
        Debug.Log("✅ [Death Zone Trigger Detection] Setup completed successfully!");
        Debug.Log("📋 Death Zone Trigger Detection System Summary:");
        Debug.Log("   • Collision detection system created for reliable ball detection");
        Debug.Log("   • Event system configured for loose coupling with other systems");
        Debug.Log("   • False positive prevention implemented with validation");
        Debug.Log("   • Debug visualization enabled for development support");
        
        Debug.Log("🏗️ System Components:");
        Debug.Log($"   → GameObject: {triggerObject.name}");
        Debug.Log("   → BoxCollider2D: Invisible trigger for collision detection");
        Debug.Log("   → DeathZoneTrigger: Core trigger detection and event system");
        Debug.Log("   → Ball identification: Tag, layer, and component-based detection");
        
        Debug.Log("⚙️ Trigger Configuration:");
        Debug.Log($"   • Trigger Size: {triggerComponent.triggerSize}");
        Debug.Log($"   • Ball Tag: '{triggerComponent.ballTag}'");
        Debug.Log($"   • Ball Layer: {triggerComponent.ballLayer}");
        Debug.Log($"   • Velocity Threshold: {triggerComponent.minimumVelocityThreshold}");
        Debug.Log($"   • Cooldown Duration: {triggerComponent.triggerCooldownDuration}s");
        Debug.Log($"   • Validation Enabled: {triggerComponent.enableValidation}");
        
        Debug.Log("🎯 Ball Detection Features:");
        Debug.Log("   • Tag-based identification with CompareTag() for performance");
        Debug.Log("   • Layer mask filtering for precise ball detection");
        Debug.Log("   • Component-based detection as fallback identification");
        Debug.Log("   • Velocity validation to prevent false positives");
        Debug.Log("   • Direction validation (downward movement detection)");
        
        Debug.Log("📡 Event System:");
        Debug.Log("   • UnityEvent: OnBallEnterDeathZone (parameterless)");
        Debug.Log("   • UnityEvent<GameObject>: OnBallEnterDeathZoneWithBall (with ball reference)");
        Debug.Log("   • C# Static Event: BallLostEvent (loose coupling)");
        Debug.Log("   • C# Static Event: DeathZoneTriggeredEvent (simple notification)");
        
        Debug.Log("🛡️ False Positive Prevention:");
        Debug.Log("   • Ball identification prevents non-ball triggers");
        Debug.Log("   • Velocity threshold prevents stationary object triggers");
        Debug.Log("   • Direction validation prevents upward-moving ball triggers");
        Debug.Log("   • Cooldown system prevents multiple rapid triggers");
        Debug.Log("   • Component validation ensures legitimate ball objects");
        
        Debug.Log("🎨 Debug Features:");
        Debug.Log("   • Scene view gizmos show trigger area and status");
        Debug.Log("   • Color-coded visualization (red=active, yellow=cooldown)");
        Debug.Log("   • Trigger count display with status indicators");
        Debug.Log("   • Comprehensive status reporting via GetTriggerStatus()");
        Debug.Log("   • Optional debug logging for detailed troubleshooting");
        
        Debug.Log("🔄 Integration Points:");
        if (triggerComponent.positioning != null)
        {
            Debug.Log($"   • Positioning System: Connected to {triggerComponent.positioning.name}");
            Debug.Log("   • Automatic position updates when death zone moves");
        }
        else
        {
            Debug.Log("   • Positioning System: Not connected - manual positioning");
        }
        
        if (triggerComponent.config != null)
        {
            Debug.Log("   • Configuration: Loaded from DeathZoneConfig asset");
            Debug.Log("   • Trigger dimensions and settings applied from config");
        }
        else
        {
            Debug.Log("   • Configuration: Using default settings");
        }
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Create ball GameObject with 'Ball' tag or configure ball detection settings");
        Debug.Log("   2. Ensure ball has Rigidbody2D and Collider2D components for detection");
        Debug.Log("   3. Connect event listeners to OnBallEnterDeathZone for life management");
        Debug.Log("   4. Subscribe to static events for loose coupling with other systems");
        Debug.Log("   5. Test ball detection using TestBallDetection() method");
        
        Debug.Log("🔧 Configuration Options:");
        Debug.Log("   • Trigger Size: Adjust detection area dimensions in Inspector");
        Debug.Log("   • Ball Detection: Configure tag, layer, and component detection");
        Debug.Log("   • Validation: Enable/disable velocity and direction checks");
        Debug.Log("   • Cooldown: Prevent multiple triggers with configurable duration");
        Debug.Log("   • Debug: Toggle gizmos, logging, and visualization options");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Ball must have 'Ball' tag or be on specified layer for detection");
        Debug.Log("   → Trigger collider is invisible but functional for ball detection");
        Debug.Log("   → Event system provides multiple integration options for different needs");
        Debug.Log("   → Validation prevents false positives from paddle and other objects");
        Debug.Log("   → Cooldown system prevents multiple rapid triggers from same ball");
        
        Debug.Log("🧪 Testing Procedures:");
        Debug.Log("   → Use TestBallDetection(ballGameObject) to validate ball identification");
        Debug.Log("   → Check GetTriggerStatus() for comprehensive system status");
        Debug.Log("   → Monitor Scene view gizmos for visual trigger area verification");
        Debug.Log("   → Enable debug logging for detailed trigger event information");
        Debug.Log("   → Test with different ball speeds and approach angles");
        
        Debug.Log("🔄 Next Steps:");
        Debug.Log("   → Create ball GameObject with appropriate components and tags");
        Debug.Log("   → Implement life management system to handle BallLostEvent");
        Debug.Log("   → Add audio-visual feedback system connected to trigger events");
        Debug.Log("   → Test collision detection with various ball movement patterns");
        Debug.Log("   → Integrate with game state management for respawn handling");
    }
}
#endif