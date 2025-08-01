#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for creating Boundary Configuration data structures and assets.
/// Creates default BoundaryConfig ScriptableObject with optimized settings for 16:10 aspect ratio gameplay.
/// </summary>
public static class CreateBoundaryConfigSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Boundary Configuration";
    private const string RESOURCES_PATH = "Assets/Resources";
    private const string BOUNDARY_CONFIG_PATH = "Assets/Resources/BoundaryConfig.asset";
    private const string BOUNDARIES_FOLDER = "Assets/Scripts/Boundaries";
    
    /// <summary>
    /// Creates the boundary configuration system with default values for 16:10 aspect ratio gameplay.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateBoundaryConfiguration()
    {
        Debug.Log("🏗️ [Boundary Config] Starting boundary configuration system setup...");
        
        try
        {
            // Step 1: Create folder structure
            CreateFolderStructure();
            
            // Step 2: Create BoundaryConfig ScriptableObject
            BoundaryConfig boundaryConfig = CreateBoundaryConfigAsset();
            
            // Step 3: Configure default values
            ConfigureDefaultValues(boundaryConfig);
            
            // Step 4: Validate configuration
            ValidateConfiguration(boundaryConfig);
            
            // Step 5: Save asset and refresh
            SaveAndRefreshAssets(boundaryConfig);
            
            // Step 6: Log success and usage instructions
            LogSuccessfulSetup(boundaryConfig);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Boundary Config] Setup failed: {e.Message}");
            Debug.LogError("📋 Please check Unity console for detailed error information");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if configuration doesn't already exist
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateBoundaryConfiguration()
    {
        // Check if BoundaryConfig asset already exists
        string[] existingAssets = AssetDatabase.FindAssets("t:BoundaryConfig");
        return existingAssets.Length == 0;
    }
    
    /// <summary>
    /// Creates necessary folder structure for boundary system organization
    /// </summary>
    private static void CreateFolderStructure()
    {
        Debug.Log("📁 [Boundary Config] Creating folder structure...");
        
        // Create Resources folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(RESOURCES_PATH))
        {
            string parentFolder = Path.GetDirectoryName(RESOURCES_PATH);
            string folderName = Path.GetFileName(RESOURCES_PATH);
            AssetDatabase.CreateFolder(parentFolder, folderName);
            Debug.Log($"   • Created Resources folder: {RESOURCES_PATH}");
        }
        
        // Create Boundaries folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(BOUNDARIES_FOLDER))
        {
            string parentFolder = Path.GetDirectoryName(BOUNDARIES_FOLDER);
            string folderName = Path.GetFileName(BOUNDARIES_FOLDER);
            AssetDatabase.CreateFolder(parentFolder, folderName);
            Debug.Log($"   • Created Boundaries folder: {BOUNDARIES_FOLDER}");
        }
        
        Debug.Log("✅ [Boundary Config] Folder structure created successfully");
    }
    
    /// <summary>
    /// Creates the BoundaryConfig ScriptableObject asset
    /// </summary>
    /// <returns>Created BoundaryConfig instance</returns>
    private static BoundaryConfig CreateBoundaryConfigAsset()
    {
        Debug.Log("🔧 [Boundary Config] Creating BoundaryConfig ScriptableObject...");
        
        // Create new BoundaryConfig instance
        BoundaryConfig boundaryConfig = ScriptableObject.CreateInstance<BoundaryConfig>();
        
        if (boundaryConfig == null)
        {
            throw new System.Exception("Failed to create BoundaryConfig ScriptableObject instance");
        }
        
        // Set asset name
        boundaryConfig.name = "BoundaryConfig";
        
        Debug.Log("✅ [Boundary Config] BoundaryConfig ScriptableObject created");
        return boundaryConfig;
    }
    
    /// <summary>
    /// Configures default values optimized for 16:10 aspect ratio Breakout gameplay
    /// </summary>
    /// <param name="boundaryConfig">BoundaryConfig to configure</param>
    private static void ConfigureDefaultValues(BoundaryConfig boundaryConfig)
    {
        Debug.Log("⚙️ [Boundary Config] Configuring default values for 16:10 aspect ratio...");
        
        // Reset to ensure clean defaults
        boundaryConfig.ResetToDefaults();
        
        // Global boundary settings
        boundaryConfig.enableBoundaries = true;
        Debug.Log("   • Boundaries enabled: ✅");
        
        // Aspect ratio configuration for 16:10 gameplay
        boundaryConfig.targetAspectRatio = 1.6f; // 16:10 aspect ratio
        boundaryConfig.referenceResolution = new Vector2(1920, 1200); // 16:10 reference resolution
        boundaryConfig.boundaryMargin = 1f;
        Debug.Log("   • Target aspect ratio: 16:10 (1.6)");
        Debug.Log("   • Reference resolution: 1920x1200");
        
        // Play area dimensions optimized for Breakout gameplay
        boundaryConfig.playAreaWidth = 20f;
        boundaryConfig.playAreaHeight = 12f;
        boundaryConfig.playAreaCenter = Vector3.zero;
        Debug.Log($"   • Play area: {boundaryConfig.playAreaWidth} x {boundaryConfig.playAreaHeight}");
        
        // Resolution scaling settings
        boundaryConfig.enableResolutionScaling = true;
        boundaryConfig.minimumScaleFactor = 0.5f;
        boundaryConfig.maximumScaleFactor = 2f;
        Debug.Log("   • Resolution scaling: Enabled (0.5x - 2x)");
        
        // Physics settings for arcade-style bouncing
        boundaryConfig.globalBounceMultiplier = 1f;
        boundaryConfig.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Debug.Log("   • Physics: Arcade-style bouncing with continuous collision detection");
        
        // Debug and visualization settings
        boundaryConfig.enablePhysicsDebugging = false;
        boundaryConfig.showBoundaryGizmos = true;
        Debug.Log("   • Visualization: Gizmos enabled, physics debugging disabled");
        
        // Performance settings
        boundaryConfig.enableBoundaryPooling = false; // Can be enabled later for optimization
        Debug.Log("   • Performance: Standard instantiation (pooling disabled)");
        
        // Configure individual boundary walls with arcade-appropriate settings
        ConfigureIndividualBoundaries(boundaryConfig);
        
        Debug.Log("✅ [Boundary Config] Default values configured successfully");
    }
    
    /// <summary>
    /// Configures individual boundary wall settings for optimal Breakout gameplay
    /// </summary>
    /// <param name="boundaryConfig">BoundaryConfig to configure boundaries for</param>
    private static void ConfigureIndividualBoundaries(BoundaryConfig boundaryConfig)
    {
        Debug.Log("🧱 [Boundary Config] Configuring individual boundary walls...");
        
        // Top boundary - solid wall for ball containment
        var topConfig = BoundaryWallConfig.CreateDefault(BoundaryType.Top);
        topConfig.width = boundaryConfig.playAreaWidth + 2f; // Slightly wider than play area
        topConfig.height = 1f;
        topConfig.thickness = 1f;
        topConfig.bounceCoefficient = 1f; // Perfect bounce for arcade feel
        topConfig.enableCollision = true;
        topConfig.enableVisual = true;
        topConfig.visualColor = new Color(0.2f, 0.4f, 0.8f, 1f); // Blue
        topConfig.collisionLayer = LayerMask.NameToLayer("Default");
        boundaryConfig.SetBoundaryConfig(BoundaryType.Top, topConfig);
        Debug.Log("   • Top boundary: Perfect bounce, blue visualization");
        
        // Left boundary - solid wall for ball containment
        var leftConfig = BoundaryWallConfig.CreateDefault(BoundaryType.Left);
        leftConfig.width = 1f;
        leftConfig.height = boundaryConfig.playAreaHeight + 2f; // Slightly taller than play area
        leftConfig.thickness = 1f;
        leftConfig.bounceCoefficient = 1f; // Perfect bounce for arcade feel
        leftConfig.enableCollision = true;
        leftConfig.enableVisual = true;
        leftConfig.visualColor = new Color(0.2f, 0.8f, 0.4f, 1f); // Green
        leftConfig.collisionLayer = LayerMask.NameToLayer("Default");
        boundaryConfig.SetBoundaryConfig(BoundaryType.Left, leftConfig);
        Debug.Log("   • Left boundary: Perfect bounce, green visualization");
        
        // Right boundary - solid wall for ball containment
        var rightConfig = BoundaryWallConfig.CreateDefault(BoundaryType.Right);
        rightConfig.width = 1f;
        rightConfig.height = boundaryConfig.playAreaHeight + 2f; // Slightly taller than play area
        rightConfig.thickness = 1f;
        rightConfig.bounceCoefficient = 1f; // Perfect bounce for arcade feel
        rightConfig.enableCollision = true;
        rightConfig.enableVisual = true;
        rightConfig.visualColor = new Color(0.2f, 0.8f, 0.4f, 1f); // Green
        rightConfig.collisionLayer = LayerMask.NameToLayer("Default");
        boundaryConfig.SetBoundaryConfig(BoundaryType.Right, rightConfig);
        Debug.Log("   • Right boundary: Perfect bounce, green visualization");
        
        // Bottom boundary - typically acts as ball loss area (no collision by default)
        var bottomConfig = BoundaryWallConfig.CreateDefault(BoundaryType.Bottom);
        bottomConfig.width = boundaryConfig.playAreaWidth + 2f; // Slightly wider than play area
        bottomConfig.height = 1f;
        bottomConfig.thickness = 1f;
        bottomConfig.bounceCoefficient = 0f; // No bounce - ball should be lost
        bottomConfig.enableCollision = false; // Disabled by default for ball loss detection
        bottomConfig.enableVisual = true;
        bottomConfig.visualColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Red for danger
        bottomConfig.collisionLayer = LayerMask.NameToLayer("Default");
        boundaryConfig.SetBoundaryConfig(BoundaryType.Bottom, bottomConfig);
        Debug.Log("   • Bottom boundary: No collision (ball loss area), red visualization");
        
        Debug.Log("✅ [Boundary Config] Individual boundary walls configured");
    }
    
    /// <summary>
    /// Validates the created configuration for completeness and correctness
    /// </summary>
    /// <param name="boundaryConfig">BoundaryConfig to validate</param>
    private static void ValidateConfiguration(BoundaryConfig boundaryConfig)
    {
        Debug.Log("🔍 [Boundary Config] Validating configuration...");
        
        bool isValid = boundaryConfig.ValidateConfiguration();
        
        if (isValid)
        {
            Debug.Log("✅ [Boundary Config] Configuration validation passed");
            
            // Log configuration summary
            string summary = boundaryConfig.GetConfigurationSummary();
            Debug.Log($"📋 [Boundary Config] Configuration Summary:\n{summary}");
        }
        else
        {
            Debug.LogWarning("⚠️ [Boundary Config] Configuration validation failed - check individual boundary settings");
        }
        
        // Additional validation checks
        ValidateResolutionScaling(boundaryConfig);
        ValidatePhysicsSettings(boundaryConfig);
    }
    
    /// <summary>
    /// Validates resolution scaling configuration
    /// </summary>
    /// <param name="boundaryConfig">BoundaryConfig to validate</param>
    private static void ValidateResolutionScaling(BoundaryConfig boundaryConfig)
    {
        float currentScaleFactor = boundaryConfig.CalculateResolutionScaleFactor();
        Debug.Log($"   • Current resolution scale factor: {currentScaleFactor:F2}");
        
        if (currentScaleFactor < boundaryConfig.minimumScaleFactor || currentScaleFactor > boundaryConfig.maximumScaleFactor)
        {
            Debug.LogWarning($"   ⚠️ Scale factor {currentScaleFactor:F2} outside valid range [{boundaryConfig.minimumScaleFactor:F2}, {boundaryConfig.maximumScaleFactor:F2}]");
        }
        else
        {
            Debug.Log("   ✅ Resolution scaling configuration valid");
        }
    }
    
    /// <summary>
    /// Validates physics settings for arcade gameplay
    /// </summary>
    /// <param name="boundaryConfig">BoundaryConfig to validate</param>
    private static void ValidatePhysicsSettings(BoundaryConfig boundaryConfig)
    {
        var boundaries = boundaryConfig.GetAllBoundaryConfigs();
        int bouncingBoundaries = 0;
        
        foreach (var boundary in boundaries)
        {
            if (boundary.enableCollision && boundary.bounceCoefficient > 0f)
            {
                bouncingBoundaries++;
            }
        }
        
        Debug.Log($"   • Bouncing boundaries: {bouncingBoundaries}/4");
        
        if (bouncingBoundaries < 3)
        {
            Debug.LogWarning("   ⚠️ Less than 3 bouncing boundaries - ball may escape play area");
        }
        else
        {
            Debug.Log("   ✅ Physics settings configured for proper ball containment");
        }
    }
    
    /// <summary>
    /// Saves the asset and refreshes the AssetDatabase
    /// </summary>
    /// <param name="boundaryConfig">BoundaryConfig to save</param>
    private static void SaveAndRefreshAssets(BoundaryConfig boundaryConfig)
    {
        Debug.Log("💾 [Boundary Config] Saving asset...");
        
        // Create the asset
        AssetDatabase.CreateAsset(boundaryConfig, BOUNDARY_CONFIG_PATH);
        
        // Mark as dirty and save
        EditorUtility.SetDirty(boundaryConfig);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the asset in Project window
        Selection.activeObject = boundaryConfig;
        EditorGUIUtility.PingObject(boundaryConfig);
        
        Debug.Log($"✅ [Boundary Config] Asset saved successfully: {BOUNDARY_CONFIG_PATH}");
    }
    
    /// <summary>
    /// Logs successful setup with comprehensive usage instructions
    /// </summary>
    /// <param name="boundaryConfig">Created BoundaryConfig</param>
    private static void LogSuccessfulSetup(BoundaryConfig boundaryConfig)
    {
        Debug.Log("✅ [Boundary Configuration] Setup completed successfully!");
        Debug.Log("📋 Boundary Configuration System Summary:");
        Debug.Log("   • Data structures created for boundary wall management");
        Debug.Log("   • Configuration optimized for 16:10 aspect ratio gameplay");
        Debug.Log("   • Physics properties configured for arcade-style bouncing");
        Debug.Log("   • Resolution scaling enabled for cross-platform compatibility");
        
        Debug.Log("🏗️ Data Structures Created:");
        Debug.Log("   → BoundaryType enum: Top, Left, Right, Bottom boundary identification");
        Debug.Log("   → BoundaryWallConfig struct: Individual boundary wall configuration");
        Debug.Log("   → BoundaryConfig ScriptableObject: Centralized boundary system configuration");
        
        Debug.Log("⚙️ Configuration Features:");
        Debug.Log("   • 16:10 aspect ratio optimization (1920x1200 reference)");
        Debug.Log("   • Resolution scaling with configurable min/max factors");
        Debug.Log("   • Individual boundary physics and visual properties");
        Debug.Log("   • Play area dimensions and positioning control");
        Debug.Log("   • Performance settings for optimization");
        
        Debug.Log("🎮 Gameplay Configuration:");
        Debug.Log($"   • Play Area: {boundaryConfig.playAreaWidth} x {boundaryConfig.playAreaHeight} world units");
        Debug.Log($"   • Target Aspect Ratio: {boundaryConfig.targetAspectRatio:F1} (16:10)");
        Debug.Log($"   • Boundary Margin: {boundaryConfig.boundaryMargin} world units");
        Debug.Log($"   • Global Bounce Multiplier: {boundaryConfig.globalBounceMultiplier:F1}");
        
        Debug.Log("🔧 Physics Settings:");
        Debug.Log("   • Top/Left/Right boundaries: Perfect bounce (1.0 coefficient)");
        Debug.Log("   • Bottom boundary: No collision (ball loss area)");
        Debug.Log("   • Continuous collision detection for precise physics");
        Debug.Log("   • Configurable physics materials per boundary");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. The BoundaryConfig asset is now available in Resources/BoundaryConfig.asset");
        Debug.Log("   2. Modify boundary properties in Inspector to customize gameplay");
        Debug.Log("   3. Use BoundaryConfig.GetBoundaryConfig(BoundaryType) to access individual configurations");
        Debug.Log("   4. Call BoundaryConfig.CalculateBoundaryPosition(BoundaryType) for world positioning");
        Debug.Log("   5. Enable/disable boundaries through the global enableBoundaries setting");
        
        Debug.Log("📐 Resolution Scaling:");
        Debug.Log("   • Automatic scaling maintains 16:10 aspect ratio across devices");
        Debug.Log("   • Scale factors clamped between 0.5x and 2.0x for stability");
        Debug.Log("   • Current scale factor calculated based on screen size");
        Debug.Log("   • Disable scaling by setting enableResolutionScaling to false");
        
        Debug.Log("🎨 Visualization:");
        Debug.Log("   • Boundary gizmos enabled for Scene view visualization");
        Debug.Log("   • Color coding: Blue (Top), Green (Left/Right), Red (Bottom)");
        Debug.Log("   • Visual materials can be assigned per boundary");
        Debug.Log("   • Toggle visualization through showBoundaryGizmos setting");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Bottom boundary collision disabled by default (configure for ball loss detection)");
        Debug.Log("   → Physics materials can be assigned globally or per boundary");
        Debug.Log("   → Resolution scaling affects boundary positioning but not play area logic");
        Debug.Log("   → Validation methods ensure configuration completeness");
        
        Debug.Log("🔄 Next Steps:");
        Debug.Log("   → Implement boundary wall GameObject creation system");
        Debug.Log("   → Create physics materials for different bounce behaviors");
        Debug.Log("   → Integrate with ball physics and collision detection");
        Debug.Log("   → Add boundary visualization components");
        Debug.Log("   → Test resolution scaling across different screen sizes");
    }
}
#endif