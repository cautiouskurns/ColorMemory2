#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for creating Death Zone Configuration data structures and assets.
/// Creates default DeathZoneConfig ScriptableObject with balanced settings for Breakout gameplay.
/// Handles folder structure creation and provides comprehensive configuration validation.
/// </summary>
public static class CreateDeathZoneConfigSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Death Zone Configuration";
    private const string RESOURCES_PATH = "Assets/Resources";
    private const string DEATHZONE_CONFIG_PATH = "Assets/Resources/DeathZoneConfig.asset";
    private const string DEATHZONE_FOLDER = "Assets/Scripts/DeathZone";
    
    /// <summary>
    /// Creates the death zone configuration system with default values for Breakout gameplay.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateDeathZoneConfiguration()
    {
        Debug.Log("🏗️ [Death Zone Config] Starting death zone configuration system setup...");
        
        try
        {
            // Step 1: Create folder structure
            CreateFolderStructure();
            
            // Step 2: Create DeathZoneConfig ScriptableObject
            DeathZoneConfig deathZoneConfig = CreateDeathZoneConfigAsset();
            
            // Step 3: Configure default values
            ConfigureDefaultValues(deathZoneConfig);
            
            // Step 4: Validate configuration
            ValidateConfiguration(deathZoneConfig);
            
            // Step 5: Save asset and refresh
            SaveAndRefreshAssets(deathZoneConfig);
            
            // Step 6: Log success and usage instructions
            LogSuccessfulSetup(deathZoneConfig);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Death Zone Config] Setup failed: {e.Message}");
            Debug.LogError("📋 Please check Unity console for detailed error information");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if configuration doesn't already exist
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateDeathZoneConfiguration()
    {
        // Check if DeathZoneConfig asset already exists
        string[] existingAssets = AssetDatabase.FindAssets("t:DeathZoneConfig");
        return existingAssets.Length == 0;
    }
    
    /// <summary>
    /// Creates necessary folder structure for death zone system organization
    /// </summary>
    private static void CreateFolderStructure()
    {
        Debug.Log("📁 [Death Zone Config] Creating folder structure...");
        
        // Create Resources folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(RESOURCES_PATH))
        {
            string parentFolder = Path.GetDirectoryName(RESOURCES_PATH);
            string folderName = Path.GetFileName(RESOURCES_PATH);
            AssetDatabase.CreateFolder(parentFolder, folderName);
            Debug.Log($"   • Created Resources folder: {RESOURCES_PATH}");
        }
        
        // Create DeathZone folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(DEATHZONE_FOLDER))
        {
            string parentFolder = Path.GetDirectoryName(DEATHZONE_FOLDER);
            string folderName = Path.GetFileName(DEATHZONE_FOLDER);
            AssetDatabase.CreateFolder(parentFolder, folderName);
            Debug.Log($"   • Created DeathZone folder: {DEATHZONE_FOLDER}");
        }
        
        Debug.Log("✅ [Death Zone Config] Folder structure created successfully");
    }
    
    /// <summary>
    /// Creates the DeathZoneConfig ScriptableObject asset
    /// </summary>
    /// <returns>Created DeathZoneConfig instance</returns>
    private static DeathZoneConfig CreateDeathZoneConfigAsset()
    {
        Debug.Log("🔧 [Death Zone Config] Creating DeathZoneConfig ScriptableObject...");
        
        // Create new DeathZoneConfig instance
        DeathZoneConfig deathZoneConfig = ScriptableObject.CreateInstance<DeathZoneConfig>();
        
        if (deathZoneConfig == null)
        {
            throw new System.Exception("Failed to create DeathZoneConfig ScriptableObject instance");
        }
        
        // Set asset name
        deathZoneConfig.name = "DeathZoneConfig";
        
        Debug.Log("✅ [Death Zone Config] DeathZoneConfig ScriptableObject created");
        return deathZoneConfig;
    }
    
    /// <summary>
    /// Configures default values optimized for balanced Breakout gameplay
    /// </summary>
    /// <param name="deathZoneConfig">DeathZoneConfig to configure</param>
    private static void ConfigureDefaultValues(DeathZoneConfig deathZoneConfig)
    {
        Debug.Log("⚙️ [Death Zone Config] Configuring default values for Breakout gameplay...");
        
        // Reset to ensure clean defaults
        deathZoneConfig.ResetToDefaults();
        
        // Trigger configuration
        deathZoneConfig.triggerType = DeathZoneTriggerType.BelowPaddle;
        deathZoneConfig.triggerSize = new Vector2(30f, 2f); // Wide trigger to catch ball
        deathZoneConfig.detectionSensitivity = 0.1f;
        deathZoneConfig.showTriggerGizmos = true;
        Debug.Log("   • Trigger: Below paddle, 30x2 units, sensitivity 0.1");
        
        // Positioning parameters for classic Breakout feel
        deathZoneConfig.paddleOffset = 2f; // 2 units below paddle
        deathZoneConfig.positioningOffsets = Vector2.zero;
        deathZoneConfig.enableResolutionScaling = true;
        deathZoneConfig.minimumBottomDistance = 1f;
        Debug.Log("   • Positioning: 2 units below paddle, resolution scaling enabled");
        
        // Life management for balanced difficulty
        deathZoneConfig.startingLives = 3; // Classic 3 lives
        deathZoneConfig.livesReduction = 1; // Lose 1 life per death
        deathZoneConfig.enableGameOverDetection = true;
        deathZoneConfig.respawnDelay = 1.5f; // 1.5 second respawn delay
        Debug.Log("   • Lives: Start with 3, lose 1 per death, 1.5s respawn delay");
        
        // Feedback configuration for good player experience
        deathZoneConfig.feedbackType = DeathZoneFeedbackType.AudioVisual;
        deathZoneConfig.audioVolume = 0.7f;
        deathZoneConfig.feedbackDuration = 1f;
        deathZoneConfig.effectIntensity = 0.8f;
        Debug.Log("   • Feedback: Audio-visual, 70% volume, 1s duration, 80% intensity");
        
        // Visual feedback settings
        deathZoneConfig.deathZoneColor = new Color(1f, 0.2f, 0.2f, 0.5f); // Semi-transparent red
        deathZoneConfig.screenFlashColor = new Color(1f, 0f, 0f, 0.3f); // Red flash
        deathZoneConfig.enableParticleEffects = true;
        Debug.Log("   • Visual: Red death zone, screen flash, particle effects enabled");
        
        // Performance settings
        deathZoneConfig.enableDeathZone = true;
        deathZoneConfig.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        deathZoneConfig.deathZoneLayer = 0; // Default layer
        Debug.Log("   • Performance: Enabled, continuous collision detection, default layer");
        
        Debug.Log("✅ [Death Zone Config] Default values configured successfully");
    }
    
    /// <summary>
    /// Validates the created configuration for completeness and correctness
    /// </summary>
    /// <param name="deathZoneConfig">DeathZoneConfig to validate</param>
    private static void ValidateConfiguration(DeathZoneConfig deathZoneConfig)
    {
        Debug.Log("🔍 [Death Zone Config] Validating configuration...");
        
        bool isValid = deathZoneConfig.ValidateConfiguration();
        
        if (isValid)
        {
            Debug.Log("✅ [Death Zone Config] Configuration validation passed");
            
            // Log configuration summary
            string summary = deathZoneConfig.GetConfigurationSummary();
            Debug.Log($"📋 [Death Zone Config] Configuration Summary:\n{summary}");
        }
        else
        {
            Debug.LogWarning("⚠️ [Death Zone Config] Configuration validation failed - check individual settings");
        }
        
        // Additional validation checks
        ValidateGameplayBalance(deathZoneConfig);
        ValidateFeedbackSettings(deathZoneConfig);
    }
    
    /// <summary>
    /// Validates gameplay balance parameters
    /// </summary>
    /// <param name="deathZoneConfig">DeathZoneConfig to validate</param>
    private static void ValidateGameplayBalance(DeathZoneConfig deathZoneConfig)
    {
        Debug.Log("   🎮 Validating gameplay balance...");
        
        // Check life management balance
        if (deathZoneConfig.startingLives < 2)
        {
            Debug.LogWarning("   ⚠️ Starting lives is very low - may be too difficult");
        }
        else if (deathZoneConfig.startingLives > 5)
        {
            Debug.LogWarning("   ⚠️ Starting lives is very high - may be too easy");
        }
        else
        {
            Debug.Log($"   ✅ Life balance: {deathZoneConfig.startingLives} lives is well balanced");
        }
        
        // Check respawn delay balance
        if (deathZoneConfig.respawnDelay < 1f)
        {
            Debug.LogWarning("   ⚠️ Respawn delay is very short - may feel rushed");
        }
        else if (deathZoneConfig.respawnDelay > 3f)
        {
            Debug.LogWarning("   ⚠️ Respawn delay is very long - may feel slow");
        }
        else
        {
            Debug.Log($"   ✅ Respawn timing: {deathZoneConfig.respawnDelay}s is well balanced");
        }
        
        // Check trigger size appropriateness
        float triggerArea = deathZoneConfig.triggerSize.x * deathZoneConfig.triggerSize.y;
        if (triggerArea < 20f)
        {
            Debug.LogWarning("   ⚠️ Death zone trigger area is small - may miss ball");
        }
        else if (triggerArea > 200f)
        {
            Debug.LogWarning("   ⚠️ Death zone trigger area is large - may feel unfair");
        }
        else
        {
            Debug.Log($"   ✅ Trigger size: {deathZoneConfig.triggerSize} provides good coverage");
        }
    }
    
    /// <summary>
    /// Validates feedback settings for good user experience
    /// </summary>
    /// <param name="deathZoneConfig">DeathZoneConfig to validate</param>
    private static void ValidateFeedbackSettings(DeathZoneConfig deathZoneConfig)
    {
        Debug.Log("   🔊 Validating feedback settings...");
        
        // Check audio settings
        if (deathZoneConfig.ShouldPlayAudio())
        {
            if (deathZoneConfig.audioVolume < 0.3f)
            {
                Debug.LogWarning("   ⚠️ Audio volume is low - players may not hear death zone trigger");
            }
            Debug.Log($"   ✅ Audio feedback: {deathZoneConfig.audioVolume * 100:F0}% volume");
        }
        else
        {
            Debug.Log("   • Audio feedback disabled");
        }
        
        // Check visual feedback
        if (deathZoneConfig.ShouldShowVisualFeedback())
        {
            Debug.Log($"   ✅ Visual feedback: {deathZoneConfig.effectIntensity * 100:F0}% intensity");
        }
        else
        {
            Debug.Log("   • Visual feedback disabled");
        }
        
        // Check feedback duration
        if (deathZoneConfig.feedbackDuration < 0.5f)
        {
            Debug.LogWarning("   ⚠️ Feedback duration is very short - may be hard to notice");
        }
        else if (deathZoneConfig.feedbackDuration > 2f)
        {
            Debug.LogWarning("   ⚠️ Feedback duration is long - may interrupt gameplay flow");
        }
        else
        {
            Debug.Log($"   ✅ Feedback duration: {deathZoneConfig.feedbackDuration}s is appropriate");
        }
    }
    
    /// <summary>
    /// Saves the asset and refreshes the AssetDatabase
    /// </summary>
    /// <param name="deathZoneConfig">DeathZoneConfig to save</param>
    private static void SaveAndRefreshAssets(DeathZoneConfig deathZoneConfig)
    {
        Debug.Log("💾 [Death Zone Config] Saving asset...");
        
        // Create the asset
        AssetDatabase.CreateAsset(deathZoneConfig, DEATHZONE_CONFIG_PATH);
        
        // Mark as dirty and save
        EditorUtility.SetDirty(deathZoneConfig);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the asset in Project window
        Selection.activeObject = deathZoneConfig;
        EditorGUIUtility.PingObject(deathZoneConfig);
        
        Debug.Log($"✅ [Death Zone Config] Asset saved successfully: {DEATHZONE_CONFIG_PATH}");
    }
    
    /// <summary>
    /// Logs successful setup with comprehensive usage instructions
    /// </summary>
    /// <param name="deathZoneConfig">Created DeathZoneConfig</param>
    private static void LogSuccessfulSetup(DeathZoneConfig deathZoneConfig)
    {
        Debug.Log("✅ [Death Zone Configuration] Setup completed successfully!");
        Debug.Log("📋 Death Zone Configuration System Summary:");
        Debug.Log("   • Data structures created for death zone trigger detection");
        Debug.Log("   • Life management system configured for balanced gameplay");
        Debug.Log("   • Positioning parameters set for paddle-relative placement");
        Debug.Log("   • Feedback configuration optimized for player experience");
        
        Debug.Log("🏗️ Data Structures Created:");
        Debug.Log("   → DeathZoneTriggerType enum: BelowPaddle, BottomBoundary, CustomZone, DynamicPaddleRelative");
        Debug.Log("   → DeathZoneFeedbackType enum: None, Audio, Visual, AudioVisual, Haptic");
        Debug.Log("   → DeathZoneConfig ScriptableObject: Comprehensive configuration management");
        
        Debug.Log("⚙️ Configuration Features:");
        Debug.Log("   • Flexible trigger types for different gameplay scenarios");
        Debug.Log("   • Resolution scaling for consistent behavior across devices");
        Debug.Log("   • Life management with configurable starting lives and reduction");
        Debug.Log("   • Multi-modal feedback system (audio, visual, haptic)");
        Debug.Log("   • Performance optimization settings");
        
        Debug.Log("🎮 Gameplay Configuration:");
        Debug.Log($"   • Trigger Type: {deathZoneConfig.triggerType}");
        Debug.Log($"   • Trigger Size: {deathZoneConfig.triggerSize} world units");
        Debug.Log($"   • Paddle Offset: {deathZoneConfig.paddleOffset} units below paddle");
        Debug.Log($"   • Starting Lives: {deathZoneConfig.startingLives}");
        Debug.Log($"   • Lives Reduction: {deathZoneConfig.livesReduction} per trigger");
        
        Debug.Log("🔊 Feedback Settings:");
        Debug.Log($"   • Feedback Type: {deathZoneConfig.feedbackType}");
        Debug.Log($"   • Audio Volume: {deathZoneConfig.audioVolume * 100:F0}%");
        Debug.Log($"   • Effect Intensity: {deathZoneConfig.effectIntensity * 100:F0}%");
        Debug.Log($"   • Feedback Duration: {deathZoneConfig.feedbackDuration}s");
        Debug.Log($"   • Particle Effects: {(deathZoneConfig.enableParticleEffects ? "Enabled" : "Disabled")}");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. The DeathZoneConfig asset is now available in Resources/DeathZoneConfig.asset");
        Debug.Log("   2. Modify configuration values in Inspector to customize gameplay balance");
        Debug.Log("   3. Use Resources.Load<DeathZoneConfig>(\"DeathZoneConfig\") to access at runtime");
        Debug.Log("   4. Call ValidateConfiguration() before using to ensure settings are valid");
        Debug.Log("   5. Use CalculateDeathZonePosition() for dynamic positioning calculations");
        
        Debug.Log("📐 Position Calculation:");
        Debug.Log("   • Supports paddle-relative and absolute positioning modes");
        Debug.Log("   • Automatic resolution scaling for consistent placement");
        Debug.Log("   • Minimum distance constraints prevent off-screen placement");
        Debug.Log("   • Custom offset support for fine-tuning placement");
        
        Debug.Log("🎨 Visualization:");
        Debug.Log("   • Death zone gizmos enabled for Scene view debugging");
        Debug.Log($"   • Death zone color: {deathZoneConfig.deathZoneColor}");
        Debug.Log($"   • Screen flash color: {deathZoneConfig.screenFlashColor}");
        Debug.Log("   • Toggle visualization through showTriggerGizmos setting");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Configuration is automatically validated for gameplay balance");
        Debug.Log("   → Feedback settings can be adjusted without affecting core mechanics");
        Debug.Log("   → Trigger sensitivity affects detection precision vs performance");
        Debug.Log("   → Resolution scaling maintains consistent gameplay across devices");
        
        Debug.Log("🔄 Next Steps:");
        Debug.Log("   → Implement death zone trigger detection system");
        Debug.Log("   → Create life management and game over detection");
        Debug.Log("   → Add audio-visual feedback components");
        Debug.Log("   → Integrate with ball physics and paddle positioning");
        Debug.Log("   → Test death zone placement and trigger accuracy");
    }
}
#endif