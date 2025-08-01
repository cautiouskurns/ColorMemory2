#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for creating Death Zone Positioning system.
/// Creates positioning system GameObject with DeathZonePositioning component configured
/// for adaptive placement relative to paddle location with resolution scaling support.
/// </summary>
public static class CreateDeathZonePositioningSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Death Zone Positioning";
    private const string DEATHZONE_SYSTEM_NAME = "Death Zone System";
    private const string DEATHZONE_CONFIG_PATH = "DeathZoneConfig";
    
    /// <summary>
    /// Creates death zone positioning system with adaptive placement configuration.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateDeathZonePositioning()
    {
        Debug.Log("🏗️ [Death Zone Positioning] Starting positioning system setup...");
        
        try
        {
            // Step 1: Check prerequisites
            DeathZoneConfig config = ValidatePrerequisites();
            
            // Step 2: Create death zone system GameObject
            GameObject deathZoneSystem = CreateDeathZoneSystemGameObject();
            
            // Step 3: Add positioning component
            DeathZonePositioning positioning = AddPositioningComponent(deathZoneSystem);
            
            // Step 4: Configure positioning settings
            ConfigurePositioningSettings(positioning, config);
            
            // Step 5: Set up paddle reference
            SetupPaddleReference(positioning);
            
            // Step 6: Configure resolution adaptation
            ConfigureResolutionAdaptation(positioning);
            
            // Step 7: Validate positioning system
            ValidatePositioningSystem(positioning);
            
            // Step 8: Save and log success
            SaveAndLogSuccess(deathZoneSystem, positioning);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Death Zone Positioning] Setup failed: {e.Message}");
            Debug.LogError($"📋 Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if positioning system doesn't exist.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateDeathZonePositioning()
    {
        // Check if Death Zone System already exists
        GameObject existingSystem = GameObject.Find(DEATHZONE_SYSTEM_NAME);
        return existingSystem == null;
    }
    
    /// <summary>
    /// Validates that prerequisite components exist.
    /// </summary>
    /// <returns>DeathZoneConfig asset</returns>
    private static DeathZoneConfig ValidatePrerequisites()
    {
        Debug.Log("🔍 [Death Zone Positioning] Validating prerequisites...");
        
        // Check for DeathZoneConfig
        DeathZoneConfig config = Resources.Load<DeathZoneConfig>(DEATHZONE_CONFIG_PATH);
        if (config == null)
        {
            throw new System.Exception($"DeathZoneConfig asset not found at Resources/{DEATHZONE_CONFIG_PATH}. Please run 'Breakout/Setup/Create Death Zone Configuration' first.");
        }
        
        // Validate configuration
        if (!config.ValidateConfiguration())
        {
            Debug.LogWarning("⚠️ [Death Zone Positioning] DeathZoneConfig has validation issues - check configuration");
        }
        
        Debug.Log("✅ [Death Zone Positioning] Prerequisites validated");
        return config;
    }
    
    /// <summary>
    /// Creates the death zone system GameObject.
    /// </summary>
    /// <returns>Created GameObject</returns>
    private static GameObject CreateDeathZoneSystemGameObject()
    {
        Debug.Log("🏗️ [Death Zone Positioning] Creating Death Zone System GameObject...");
        
        GameObject deathZoneSystem = new GameObject(DEATHZONE_SYSTEM_NAME);
        deathZoneSystem.transform.position = Vector3.zero;
        deathZoneSystem.transform.rotation = Quaternion.identity;
        deathZoneSystem.transform.localScale = Vector3.one;
        
        // Set appropriate layer and tag
        deathZoneSystem.layer = LayerMask.NameToLayer("Default");
        deathZoneSystem.tag = "Untagged";
        
        Debug.Log($"✅ [Death Zone Positioning] Created GameObject: {deathZoneSystem.name}");
        return deathZoneSystem;
    }
    
    /// <summary>
    /// Adds DeathZonePositioning component to the system GameObject.
    /// </summary>
    /// <param name="deathZoneSystem">Parent GameObject</param>
    /// <returns>Added positioning component</returns>
    private static DeathZonePositioning AddPositioningComponent(GameObject deathZoneSystem)
    {
        Debug.Log("🔧 [Death Zone Positioning] Adding DeathZonePositioning component...");
        
        DeathZonePositioning positioning = deathZoneSystem.AddComponent<DeathZonePositioning>();
        
        if (positioning == null)
        {
            throw new System.Exception("Failed to add DeathZonePositioning component");
        }
        
        Debug.Log("✅ [Death Zone Positioning] DeathZonePositioning component added");
        return positioning;
    }
    
    /// <summary>
    /// Configures positioning component settings.
    /// </summary>
    /// <param name="positioning">Positioning component to configure</param>
    /// <param name="config">DeathZoneConfig for settings</param>
    private static void ConfigurePositioningSettings(DeathZonePositioning positioning, DeathZoneConfig config)
    {
        Debug.Log("⚙️ [Death Zone Positioning] Configuring positioning settings...");
        
        // Set configuration reference
        positioning.config = config;
        
        // Configure paddle integration
        positioning.paddleOffset = -config.paddleOffset; // Negative for below paddle
        positioning.trackPaddleMovement = true;
        
        // Configure centering mode based on trigger type
        switch (config.triggerType)
        {
            case DeathZoneTriggerType.BelowPaddle:
            case DeathZoneTriggerType.DynamicPaddleRelative:
                positioning.centeringMode = PositionCenteringMode.FollowPaddle;
                break;
                
            case DeathZoneTriggerType.BottomBoundary:
                positioning.centeringMode = PositionCenteringMode.ScreenCenter;
                break;
                
            case DeathZoneTriggerType.CustomZone:
                positioning.centeringMode = PositionCenteringMode.CustomPosition;
                break;
        }
        
        // Configure positioning constraints
        positioning.minimumBottomDistance = config.minimumBottomDistance;
        positioning.maximumPaddleDistance = 5f; // Reasonable default
        
        EditorUtility.SetDirty(positioning);
        
        Debug.Log($"✅ [Death Zone Positioning] Settings configured - Mode: {positioning.centeringMode}, Offset: {positioning.paddleOffset}");
    }
    
    /// <summary>
    /// Sets up paddle reference connection.
    /// </summary>
    /// <param name="positioning">Positioning component</param>
    private static void SetupPaddleReference(DeathZonePositioning positioning)
    {
        Debug.Log("🎮 [Death Zone Positioning] Setting up paddle reference...");
        
        // Try to find existing paddle
        Transform paddleTransform = FindPaddleTransform();
        
        if (paddleTransform != null)
        {
            positioning.paddleTransform = paddleTransform;
            Debug.Log($"✅ [Death Zone Positioning] Paddle reference set: {paddleTransform.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ [Death Zone Positioning] Paddle not found - will use screen-center positioning");
            Debug.LogWarning("   → Connect paddle reference manually or run paddle setup first");
            
            // Set centering mode to screen center as fallback
            positioning.centeringMode = PositionCenteringMode.ScreenCenter;
        }
    }
    
    /// <summary>
    /// Attempts to find paddle transform in scene.
    /// </summary>
    /// <returns>Paddle transform or null</returns>
    private static Transform FindPaddleTransform()
    {
        // Try common paddle names
        string[] paddleNames = { "Paddle", "Player Paddle", "PlayerPaddle", "Ball Paddle" };
        
        foreach (string paddleName in paddleNames)
        {
            GameObject paddle = GameObject.Find(paddleName);
            if (paddle != null)
            {
                return paddle.transform;
            }
        }
        
        // Try finding by component
        PaddleController paddleController = Object.FindFirstObjectByType<PaddleController>();
        if (paddleController != null)
        {
            return paddleController.transform;
        }
        
        return null;
    }
    
    /// <summary>
    /// Configures resolution adaptation settings.
    /// </summary>
    /// <param name="positioning">Positioning component</param>
    private static void ConfigureResolutionAdaptation(DeathZonePositioning positioning)
    {
        Debug.Log("📐 [Death Zone Positioning] Configuring resolution adaptation...");
        
        // Enable resolution adaptation for cross-platform support
        positioning.adaptToResolution = true;
        positioning.detectResolutionChanges = true;
        
        // Set reference resolution for 16:10 aspect ratio
        positioning.referenceResolution = new Vector2(1920f, 1200f);
        
        Debug.Log("✅ [Death Zone Positioning] Resolution adaptation configured");
        Debug.Log($"   • Reference resolution: {positioning.referenceResolution}");
        Debug.Log("   • Automatic resolution detection: Enabled");
    }
    
    /// <summary>
    /// Validates the created positioning system.
    /// </summary>
    /// <param name="positioning">Positioning component to validate</param>
    private static void ValidatePositioningSystem(DeathZonePositioning positioning)
    {
        Debug.Log("🔍 [Death Zone Positioning] Validating positioning system...");
        
        bool isValid = true;
        
        // Check configuration reference
        if (positioning.config == null)
        {
            Debug.LogWarning("   ⚠️ DeathZoneConfig reference missing");
            isValid = false;
        }
        
        // Check paddle reference (warning only, not error)
        if (positioning.paddleTransform == null)
        {
            Debug.LogWarning("   ⚠️ Paddle reference not set - using fallback positioning");
        }
        
        // Validate positioning constraints
        if (positioning.minimumBottomDistance < 0f)
        {
            Debug.LogWarning("   ⚠️ Minimum bottom distance is negative");
            isValid = false;
        }
        
        if (positioning.maximumPaddleDistance <= 0f)
        {
            Debug.LogWarning("   ⚠️ Maximum paddle distance must be positive");
            isValid = false;
        }
        
        // Test initial position calculation
        try
        {
            Vector3 testPosition = positioning.CalculateDeathZonePosition();
            Debug.Log($"   • Test position calculation: {testPosition}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"   ❌ Position calculation failed: {e.Message}");
            isValid = false;
        }
        
        if (isValid)
        {
            Debug.Log("✅ [Death Zone Positioning] System validation passed");
        }
        else
        {
            Debug.LogWarning("⚠️ [Death Zone Positioning] System validation issues detected - check settings");
        }
    }
    
    /// <summary>
    /// Saves changes and logs successful setup.
    /// </summary>
    /// <param name="deathZoneSystem">Created system GameObject</param>
    /// <param name="positioning">Positioning component</param>
    private static void SaveAndLogSuccess(GameObject deathZoneSystem, DeathZonePositioning positioning)
    {
        Debug.Log("💾 [Death Zone Positioning] Saving setup...");
        
        // Mark objects as dirty for saving
        EditorUtility.SetDirty(deathZoneSystem);
        EditorUtility.SetDirty(positioning);
        
        // Select the created system in hierarchy
        Selection.activeGameObject = deathZoneSystem;
        EditorGUIUtility.PingObject(deathZoneSystem);
        
        LogSuccessfulSetup(deathZoneSystem, positioning);
    }
    
    /// <summary>
    /// Logs successful setup with comprehensive usage instructions.
    /// </summary>
    /// <param name="deathZoneSystem">Created system GameObject</param>
    /// <param name="positioning">Positioning component</param>
    private static void LogSuccessfulSetup(GameObject deathZoneSystem, DeathZonePositioning positioning)
    {
        Debug.Log("✅ [Death Zone Positioning System] Setup completed successfully!");
        Debug.Log("📋 Death Zone Positioning System Summary:");
        Debug.Log("   • Adaptive positioning system created for paddle-relative death zone placement");
        Debug.Log("   • Resolution scaling configured for cross-platform consistency");
        Debug.Log("   • Position tracking system active for paddle movement");
        Debug.Log("   • Positioning constraints configured for gameplay balance");
        
        Debug.Log("🏗️ System Components:");
        Debug.Log($"   → GameObject: {deathZoneSystem.name}");
        Debug.Log("   → DeathZonePositioning: Adaptive placement management");
        Debug.Log("   → Position tracking: Paddle movement and resolution changes");
        Debug.Log("   → Constraint system: Screen bounds and distance limits");
        
        Debug.Log("⚙️ Configuration:");
        Debug.Log($"   • Centering Mode: {positioning.centeringMode}");
        Debug.Log($"   • Paddle Offset: {positioning.paddleOffset} units");
        Debug.Log($"   • Track Paddle Movement: {positioning.trackPaddleMovement}");
        Debug.Log($"   • Resolution Adaptation: {positioning.adaptToResolution}");
        Debug.Log($"   • Reference Resolution: {positioning.referenceResolution}");
        
        Debug.Log("🎮 Paddle Integration:");
        if (positioning.paddleTransform != null)
        {
            Debug.Log($"   • Paddle Reference: {positioning.paddleTransform.name}");
            Debug.Log("   • Paddle tracking: Active");
            Debug.Log("   • Position updates: Automatic on paddle movement");
        }
        else
        {
            Debug.Log("   • Paddle Reference: Not set (using fallback positioning)");
            Debug.Log("   • Fallback mode: Screen-center positioning");
            Debug.Log("   • Manual connection: Required for paddle-relative positioning");
        }
        
        Debug.Log("📐 Positioning Features:");
        Debug.Log("   • Paddle-relative placement with configurable offset");
        Debug.Log("   • Screen bounds constraint system");
        Debug.Log("   • Resolution scaling for consistent placement");
        Debug.Log("   • Real-time position updates on paddle movement");
        Debug.Log("   • Minimum distance constraints from screen edges");
        
        Debug.Log("🔄 Dynamic Adaptation:");
        Debug.Log("   • Automatic position updates on paddle movement");
        Debug.Log("   • Resolution change detection and repositioning");
        Debug.Log("   • Positioning constraint validation");
        Debug.Log("   • Scale factor calculation for different screen sizes");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Death zone positioning system is now active in scene");
        Debug.Log("   2. Connect paddle reference in Inspector if not automatically detected");
        Debug.Log("   3. Adjust positioning settings in DeathZonePositioning component");
        Debug.Log("   4. Test position updates by moving paddle or changing screen size");
        Debug.Log("   5. Use ForcePositionUpdate() method for manual position recalculation");
        
        Debug.Log("🔧 Configuration Options:");
        Debug.Log("   • Paddle Offset: Distance below paddle for death zone placement");
        Debug.Log("   • Centering Mode: FollowPaddle, ScreenCenter, or CustomPosition");
        Debug.Log("   • Resolution Adaptation: Enable/disable automatic scaling");
        Debug.Log("   • Tracking Settings: Paddle movement and resolution change detection");
        Debug.Log("   • Constraints: Minimum distances and maximum offsets");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Death zone position updates automatically when paddle moves");
        Debug.Log("   → Resolution scaling maintains consistent gameplay across devices");
        Debug.Log("   → Positioning constraints prevent death zone from going off-screen");
        Debug.Log("   → Manual paddle reference connection may be required");
        
        Debug.Log("🔄 Next Steps:");
        Debug.Log("   → Connect paddle reference if not automatically detected");
        Debug.Log("   → Implement death zone trigger detection system");
        Debug.Log("   → Test positioning across different screen resolutions");
        Debug.Log("   → Integrate with life management and feedback systems");
        Debug.Log("   → Validate positioning during paddle movement");
        
        // Log current status
        Debug.Log("\n" + positioning.GetPositioningStatus());
    }
}
#endif