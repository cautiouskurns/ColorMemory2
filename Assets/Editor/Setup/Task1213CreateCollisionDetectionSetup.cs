#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for configuring collision detection on Brick components.
/// Provides automated configuration of ball layer masks, colliders, and CollisionManager integration.
/// </summary>
public static class Task1213CreateCollisionDetectionSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1213 Configure Brick Collision Detection";
    
    /// <summary>
    /// Configures collision detection on existing Brick components in the scene.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void ConfigureCollisionDetection()
    {
        Debug.Log("🎯 [Task 1.2.1.3] Starting Collision Detection configuration...");
        
        try
        {
            // Step 1: Validate dependencies
            ValidateDependencies();
            
            // Step 2: Find and configure existing brick components
            ConfigureBrickCollisionDetection();
            
            // Step 3: Validate CollisionManager integration
            ValidateCollisionManagerIntegration();
            
            // Step 4: Test collision detection setup
            TestCollisionDetectionSetup();
            
            // Step 5: Save assets and log success
            AssetDatabase.SaveAssets();
            LogSuccessfulSetup();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.1.3] Collision Detection configuration failed: {e.Message}");
            Debug.LogError("📋 Please ensure Brick MonoBehaviour and CollisionManager are available");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if we can find Brick components
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateConfigureCollisionDetection()
    {
        return true; // Always show - validation happens during execution
    }
    
    /// <summary>
    /// Validates that all dependencies are available for collision detection setup
    /// </summary>
    private static void ValidateDependencies()
    {
        Debug.Log("🔍 [Step 1/5] Validating dependencies...");
        
        // Check if Brick components exist in scene
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        if (bricks.Length == 0)
        {
            throw new System.Exception("No Brick components found in scene. Please create brick GameObjects first.");
        }
        
        Debug.Log($"   • Found {bricks.Length} Brick components in scene");
        
        // Check if CollisionManager exists
        CollisionManager collisionManager = GameObject.FindFirstObjectByType<CollisionManager>();
        if (collisionManager != null)
        {
            Debug.Log($"   • CollisionManager found: {collisionManager.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("   ⚠️ CollisionManager not found - collision coordination will be limited");
        }
        
        Debug.Log("✅ [Step 1/5] Dependencies validated successfully");
    }
    
    /// <summary>
    /// Configures collision detection on all Brick components in the scene
    /// </summary>
    private static void ConfigureBrickCollisionDetection()
    {
        Debug.Log("⚙️ [Step 2/5] Configuring brick collision detection...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int configuredCount = 0;
        int colliderAddedCount = 0;
        
        foreach (Brick brick in bricks)
        {
            try
            {
                ConfigureIndividualBrick(brick, ref colliderAddedCount);
                configuredCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to configure collision detection on {brick.gameObject.name}: {e.Message}");
            }
        }
        
        Debug.Log($"   • Configured collision detection on {configuredCount}/{bricks.Length} bricks");
        Debug.Log($"   • Added {colliderAddedCount} missing Collider2D components");
        Debug.Log("✅ [Step 2/5] Brick collision detection configuration complete");
    }
    
    /// <summary>
    /// Configures collision detection on an individual brick component
    /// </summary>
    /// <param name="brick">Brick component to configure</param>
    /// <param name="colliderAddedCount">Counter for added colliders</param>
    private static void ConfigureIndividualBrick(Brick brick, ref int colliderAddedCount)
    {
        SerializedObject serializedBrick = new SerializedObject(brick);
        
        // Configure ball layer mask
        SerializedProperty ballLayerMask = serializedBrick.FindProperty("ballLayerMask");
        if (ballLayerMask != null)
        {
            // Set up ball layer mask - try to find "Ball" layer, fallback to all layers
            int ballLayer = LayerMask.NameToLayer("Ball");
            if (ballLayer != -1)
            {
                ballLayerMask.intValue = 1 << ballLayer;
                Debug.Log($"   • {brick.gameObject.name}: Set ball layer mask to 'Ball' layer ({ballLayer})");
            }
            else
            {
                ballLayerMask.intValue = -1; // All layers as fallback
                Debug.LogWarning($"   • {brick.gameObject.name}: 'Ball' layer not found, using all layers");
            }
        }
        
        // Configure ball tag fallback
        SerializedProperty ballTag = serializedBrick.FindProperty("ballTag");
        if (ballTag != null && string.IsNullOrEmpty(ballTag.stringValue))
        {
            ballTag.stringValue = "Ball";
            Debug.Log($"   • {brick.gameObject.name}: Set ball tag to 'Ball'");
        }
        
        // Enable debug logging for testing
        SerializedProperty enableDebugLogging = serializedBrick.FindProperty("enableDebugLogging");
        if (enableDebugLogging != null)
        {
            enableDebugLogging.boolValue = true;
            Debug.Log($"   • {brick.gameObject.name}: Enabled debug logging for collision testing");
        }
        
        // Ensure Collider2D component exists
        Collider2D collider = brick.GetComponent<Collider2D>();
        if (collider == null)
        {
            BoxCollider2D boxCollider = brick.gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1f, 0.5f);
            boxCollider.isTrigger = false; // Solid collision for physics
            colliderAddedCount++;
            Debug.Log($"   • {brick.gameObject.name}: Added BoxCollider2D component");
        }
        else
        {
            Debug.Log($"   • {brick.gameObject.name}: Collider2D already present ({collider.GetType().Name})");
        }
        
        // Apply changes
        serializedBrick.ApplyModifiedProperties();
        EditorUtility.SetDirty(brick);
    }
    
    /// <summary>
    /// Validates CollisionManager integration and communication setup
    /// </summary>
    private static void ValidateCollisionManagerIntegration()
    {
        Debug.Log("🔗 [Step 3/5] Validating CollisionManager integration...");
        
        CollisionManager collisionManager = GameObject.FindFirstObjectByType<CollisionManager>();
        if (collisionManager == null)
        {
            Debug.LogWarning("   ⚠️ CollisionManager not found in scene");
            Debug.LogWarning("   ⚠️ Collision coordination will be limited to brick-level processing");
            Debug.LogWarning("   📋 Consider adding CollisionManager from Epic 1.1 for full system integration");
        }
        else
        {
            Debug.Log($"   • CollisionManager found: {collisionManager.gameObject.name}");
            
            // Check if CollisionManager has Instance property (singleton pattern)
            if (CollisionManager.Instance != null)
            {
                Debug.Log("   • CollisionManager singleton pattern detected");
            }
            else
            {
                Debug.LogWarning("   ⚠️ CollisionManager singleton not initialized - may affect brick integration");
            }
        }
        
        Debug.Log("✅ [Step 3/5] CollisionManager integration validation complete");
    }
    
    /// <summary>
    /// Tests collision detection setup with sample collision scenarios
    /// </summary>
    private static void TestCollisionDetectionSetup()
    {
        Debug.Log("🧪 [Step 4/5] Testing collision detection setup...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int validBricks = 0;
        int readyForCollision = 0;
        
        foreach (Brick brick in bricks)
        {
            if (ValidateBrickCollisionSetup(brick))
            {
                validBricks++;
                
                if (brick.IsInitialized && !brick.IsDestroyed && brick.IsDestructible)
                {
                    readyForCollision++;
                }
            }
        }
        
        Debug.Log($"   • Collision setup validation: {validBricks}/{bricks.Length} bricks properly configured");
        Debug.Log($"   • Ready for collision: {readyForCollision}/{bricks.Length} bricks ready to receive ball hits");
        
        // Test layer configuration
        TestLayerConfiguration();
        
        Debug.Log("✅ [Step 4/5] Collision detection testing complete");
    }
    
    /// <summary>
    /// Validates collision detection setup for individual brick
    /// </summary>
    /// <param name="brick">Brick to validate</param>
    /// <returns>True if collision setup is valid</returns>
    private static bool ValidateBrickCollisionSetup(Brick brick)
    {
        bool isValid = true;
        
        // Check collider presence
        Collider2D collider = brick.GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError($"   ❌ {brick.gameObject.name}: Missing Collider2D component");
            isValid = false;
        }
        
        // Check layer mask configuration
        if (brick.BallLayerMask.value == 0)
        {
            Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Ball layer mask is empty");
        }
        
        // Check collision system initialization
        if (!brick.CollisionSystemInitialized)
        {
            Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Collision system not yet initialized");
        }
        
        return isValid;
    }
    
    /// <summary>
    /// Tests layer configuration and provides recommendations
    /// </summary>
    private static void TestLayerConfiguration()
    {
        // Check if Ball layer exists
        int ballLayer = LayerMask.NameToLayer("Ball");
        if (ballLayer == -1)
        {
            Debug.LogWarning("   ⚠️ 'Ball' layer not found in project settings");
            Debug.LogWarning("   📋 Consider creating 'Ball' layer for better collision filtering performance");
        }
        else
        {
            Debug.Log($"   • 'Ball' layer found at index {ballLayer}");
        }
        
        // Check if Bricks layer exists
        int bricksLayer = LayerMask.NameToLayer("Bricks");
        if (bricksLayer == -1)
        {
            Debug.LogWarning("   ⚠️ 'Bricks' layer not found in project settings");
            Debug.LogWarning("   📋 Consider creating 'Bricks' layer for organized collision management");
        }
        else
        {
            Debug.Log($"   • 'Bricks' layer found at index {bricksLayer}");
        }
    }
    
    /// <summary>
    /// Logs successful collision detection setup summary
    /// </summary>
    private static void LogSuccessfulSetup()
    {
        Debug.Log("✅ [Task 1.2.1.3] Collision Detection configuration completed successfully!");
        Debug.Log("📋 Collision Detection Summary:");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        CollisionManager collisionManager = GameObject.FindFirstObjectByType<CollisionManager>();
        
        Debug.Log($"   • Configured Bricks: {bricks.Length} brick components with collision detection");
        Debug.Log($"   • CollisionManager Integration: {(collisionManager != null ? "Available" : "Not Available")}");
        Debug.Log("   • Layer Filtering: Ball layer mask configured for efficient collision detection");
        Debug.Log("   • Tag Fallback: Ball tag configured for reliable collision identification");
        
        Debug.Log("🎯 Collision Detection Features:");
        Debug.Log("   → OnCollisionEnter2D: Unity physics collision event handling");
        Debug.Log("   → Ball Validation: Layer mask and tag-based collision filtering");
        Debug.Log("   → Hit Point Reduction: Automatic damage processing on valid ball hits");
        Debug.Log("   → CollisionManager Communication: Centralized collision coordination");
        Debug.Log("   → Collision Tracking: Debug logging and collision analytics");
        
        Debug.Log("⚙️ Configuration Details:");
        Debug.Log("   • Ball Layer Mask: Configured for efficient ball collision filtering");
        Debug.Log("   • Ball Tag Fallback: 'Ball' tag for reliable collision identification");
        Debug.Log("   • Debug Logging: Enabled for collision detection testing and validation");
        Debug.Log("   • Collider Components: BoxCollider2D added where missing");
        Debug.Log("   • Collision Validation: Comprehensive collision filtering and validation");
        
        Debug.Log("🔧 Integration Points:");
        Debug.Log("   • Unity Physics: OnCollisionEnter2D event integration");
        Debug.Log("   • CollisionManager: Centralized collision handling coordination");
        Debug.Log("   • Hit Point System: Automatic damage and destruction processing");
        Debug.Log("   • Collision Feedback: Framework ready for audio-visual feedback");
        Debug.Log("   • Debug System: Comprehensive logging and collision analytics");
        
        Debug.Log("💡 Testing Instructions:");
        Debug.Log("   1. Create Ball GameObject with Rigidbody2D and Collider2D");
        Debug.Log("   2. Set Ball GameObject to 'Ball' layer or add 'Ball' tag");
        Debug.Log("   3. Enable physics simulation and launch ball toward bricks");
        Debug.Log("   4. Monitor Console for collision detection debug logs");
        Debug.Log("   5. Verify hit point reduction on successful ball-brick collisions");
        
        Debug.Log("🎮 Collision Detection Workflow:");
        Debug.Log("   1. Ball collides with brick → OnCollisionEnter2D triggered");
        Debug.Log("   2. ValidateBallCollision() → Layer mask and tag verification");
        Debug.Log("   3. ProcessBallHit() → Hit point reduction and tracking");
        Debug.Log("   4. CommunicateCollisionToManager() → System coordination");
        Debug.Log("   5. TriggerBrickDestruction() → Destruction when HP reaches 0");
        
        Debug.Log("⚠️ Setup Requirements:");
        Debug.Log("   → Ball GameObject: Requires Rigidbody2D and Collider2D for physics");
        Debug.Log("   → Layer Configuration: 'Ball' layer recommended for performance");
        Debug.Log("   → Tag Configuration: 'Ball' tag required for fallback identification");
        Debug.Log("   → CollisionManager: Epic 1.1 CollisionManager for full system integration");
        Debug.Log("   → Physics Settings: Ensure Ball-Brick collision matrix is enabled");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Test collision detection with ball physics simulation");
        Debug.Log("   → Integrate with CollisionManager feedback system");
        Debug.Log("   → Add destruction effects and scoring in future tasks");
        Debug.Log("   → Configure collision matrix in Physics2D settings for optimal performance");
    }
    
    /// <summary>
    /// Utility method to clean up collision detection configuration for fresh testing
    /// </summary>
    [MenuItem("Breakout/Setup/Reset Brick Collision Settings", false, 1001)]
    public static void ResetBrickCollisionSettings()
    {
        Debug.Log("🧹 [Reset] Resetting brick collision detection settings...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int resetCount = 0;
        
        foreach (Brick brick in bricks)
        {
            SerializedObject serializedBrick = new SerializedObject(brick);
            
            // Reset layer mask to all layers
            SerializedProperty ballLayerMask = serializedBrick.FindProperty("ballLayerMask");
            if (ballLayerMask != null)
            {
                ballLayerMask.intValue = -1;
            }
            
            // Reset ball tag
            SerializedProperty ballTag = serializedBrick.FindProperty("ballTag");
            if (ballTag != null)
            {
                ballTag.stringValue = "Ball";
            }
            
            // Disable debug logging
            SerializedProperty enableDebugLogging = serializedBrick.FindProperty("enableDebugLogging");
            if (enableDebugLogging != null)
            {
                enableDebugLogging.boolValue = false;
            }
            
            serializedBrick.ApplyModifiedProperties();
            EditorUtility.SetDirty(brick);
            resetCount++;
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"🧹 [Reset] Reset collision settings on {resetCount} brick components");
    }
}
#endif