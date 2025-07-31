#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for configuring edge case handling and validation systems.
/// Provides automated validation parameter configuration and CollisionDebugger setup.
/// </summary>
public static class Task1135CreateEdgeCaseHandlingSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1135 Configure Edge Case Handling";
    private const string DEBUG_FOLDER_PATH = "Assets/Scripts/Debug";
    
    /// <summary>
    /// Configures edge case handling and validation systems.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void ConfigureEdgeCaseHandling()
    {
        Debug.Log("📋 [Task 1.1.3.5] Starting Edge Case Handling configuration...");
        
        try
        {
            // Step 1: Find and validate CollisionManager
            CollisionManager collisionManager = FindCollisionManager();
            
            // Step 2: Configure validation parameters
            ConfigureValidationParameters(collisionManager);
            
            // Step 3: Create Debug folder if needed
            EnsureDebugFolderExists();
            
            // Step 4: Add CollisionDebugger to Ball GameObject
            CollisionDebugger debugger = ConfigureCollisionDebugger();
            
            // Step 5: Configure debug visualization settings
            ConfigureDebugVisualization(debugger);
            
            // Step 6: Validate edge case handling setup
            ValidateEdgeCaseSetup(collisionManager, debugger);
            
            // Step 7: Save and finalize
            if (collisionManager != null) EditorUtility.SetDirty(collisionManager.gameObject);
            if (debugger != null) EditorUtility.SetDirty(debugger.gameObject);
            AssetDatabase.SaveAssets();
            
            LogSuccessfulSetup(collisionManager, debugger);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.3.5] Edge Case Handling configuration failed: {e.Message}");
            Debug.LogError("📋 Please ensure CollisionManager exists and is properly configured from previous tasks.");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if CollisionManager exists without CollisionDebugger.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateConfigureEdgeCaseHandling()
    {
        CollisionManager cm = GameObject.FindFirstObjectByType<CollisionManager>();
        CollisionDebugger debugger = GameObject.FindFirstObjectByType<CollisionDebugger>();
        return cm != null && debugger == null;
    }
    
    /// <summary>
    /// Finds and validates the CollisionManager in the scene.
    /// </summary>
    /// <returns>CollisionManager component</returns>
    private static CollisionManager FindCollisionManager()
    {
        Debug.Log("🔍 [Step 1/6] Finding CollisionManager...");
        
        CollisionManager collisionManager = GameObject.FindFirstObjectByType<CollisionManager>();
        
        if (collisionManager == null)
        {
            Debug.LogError("❌ CollisionManager not found in scene!");
            Debug.LogError("📋 Please run previous setup tasks: Task1132 (Create Collision Manager) and Task1134 (Configure Collision Feedback)");
            throw new System.NullReferenceException("CollisionManager component is required");
        }
        
        Debug.Log($"✅ [Step 1/6] CollisionManager found: {collisionManager.gameObject.name}");
        return collisionManager;
    }
    
    /// <summary>
    /// Configures validation parameters on CollisionManager.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to configure</param>
    private static void ConfigureValidationParameters(CollisionManager collisionManager)
    {
        Debug.Log("⚙️ [Step 2/6] Configuring validation parameters...");
        
        SerializedObject serializedManager = new SerializedObject(collisionManager);
        
        // Configure edge case handling parameters
        SerializedProperty minBallSpeed = serializedManager.FindProperty("minBallSpeed");
        SerializedProperty maxBallSpeed = serializedManager.FindProperty("maxBallSpeed");
        SerializedProperty stuckDetectionTime = serializedManager.FindProperty("stuckDetectionTime");
        SerializedProperty stuckVelocityThreshold = serializedManager.FindProperty("stuckVelocityThreshold");
        SerializedProperty stuckCorrectionForce = serializedManager.FindProperty("stuckCorrectionForce");
        SerializedProperty enableValidationDebug = serializedManager.FindProperty("enableValidationDebug");
        SerializedProperty maxSimultaneousCollisions = serializedManager.FindProperty("maxSimultaneousCollisions");
        
        if (minBallSpeed != null && maxBallSpeed != null && stuckDetectionTime != null && 
            stuckVelocityThreshold != null && stuckCorrectionForce != null)
        {
            // Set optimal validation parameters for arcade gameplay
            minBallSpeed.floatValue = 3.0f;    // Minimum ball speed
            maxBallSpeed.floatValue = 15.0f;   // Maximum ball speed
            stuckDetectionTime.floatValue = 2.0f;  // Stuck detection timeout
            stuckVelocityThreshold.floatValue = 0.1f;  // Velocity threshold for stuck detection
            stuckCorrectionForce.floatValue = 5.0f;    // Force for stuck ball correction
            
            if (enableValidationDebug != null)
            {
                enableValidationDebug.boolValue = true;  // Enable debug visualization
            }
            
            if (maxSimultaneousCollisions != null)
            {
                maxSimultaneousCollisions.intValue = 3;  // Max simultaneous collisions to process
            }
            
            // Apply changes
            serializedManager.ApplyModifiedProperties();
            
            Debug.Log($"   • Min Ball Speed: {minBallSpeed.floatValue:F1} units/sec");
            Debug.Log($"   • Max Ball Speed: {maxBallSpeed.floatValue:F1} units/sec");
            Debug.Log($"   • Stuck Detection Time: {stuckDetectionTime.floatValue:F1}s");
            Debug.Log($"   • Stuck Velocity Threshold: {stuckVelocityThreshold.floatValue:F2} units/sec");
            Debug.Log($"   • Stuck Correction Force: {stuckCorrectionForce.floatValue:F1}");
            Debug.Log($"   • Validation Debug: {(enableValidationDebug?.boolValue ?? false ? "Enabled" : "Disabled")}");
            Debug.Log($"   • Max Simultaneous Collisions: {maxSimultaneousCollisions?.intValue ?? 3}");
        }
        else
        {
            Debug.LogError("❌ Could not find validation parameters in CollisionManager!");
            Debug.LogError("📋 Ensure CollisionManager has been updated with edge case handling fields.");
            throw new System.NullReferenceException("Validation parameters not found in CollisionManager");
        }
        
        Debug.Log("✅ [Step 2/6] Validation parameters configured");
    }
    
    /// <summary>
    /// Ensures Debug folder exists for CollisionDebugger script.
    /// </summary>
    private static void EnsureDebugFolderExists()
    {
        Debug.Log("📁 [Step 3/6] Ensuring Debug folder exists...");
        
        if (!AssetDatabase.IsValidFolder(DEBUG_FOLDER_PATH))
        {
            // Create Debug folder
            string parentFolder = Path.GetDirectoryName(DEBUG_FOLDER_PATH);
            string folderName = Path.GetFileName(DEBUG_FOLDER_PATH);
            
            string guid = AssetDatabase.CreateFolder(parentFolder, folderName);
            if (!string.IsNullOrEmpty(guid))
            {
                Debug.Log($"   • Created Debug folder: {DEBUG_FOLDER_PATH}");
            }
            else
            {
                Debug.LogWarning($"   ⚠️ Failed to create Debug folder: {DEBUG_FOLDER_PATH}");
            }
        }
        else
        {
            Debug.Log($"   • Debug folder already exists: {DEBUG_FOLDER_PATH}");
        }
        
        Debug.Log("✅ [Step 3/6] Debug folder validation complete");
    }
    
    /// <summary>
    /// Configures CollisionDebugger component on Ball GameObject.
    /// </summary>
    /// <returns>Configured CollisionDebugger component</returns>
    private static CollisionDebugger ConfigureCollisionDebugger()
    {
        Debug.Log("🐛 [Step 4/6] Configuring CollisionDebugger...");
        
        // Find Ball GameObject
        GameObject ball = GameObject.Find("Ball");
        
        if (ball == null)
        {
            Debug.LogWarning("   ⚠️ Ball GameObject not found. CollisionDebugger will be added to a placeholder GameObject.");
            
            // Create placeholder GameObject for debugger
            ball = new GameObject("CollisionDebugger_Placeholder");
            Debug.Log("   • Created placeholder GameObject for CollisionDebugger");
        }
        else
        {
            Debug.Log($"   • Found Ball GameObject: {ball.name}");
        }
        
        // Add or get CollisionDebugger component
        CollisionDebugger debugger = ball.GetComponent<CollisionDebugger>();
        
        if (debugger == null)
        {
            debugger = ball.AddComponent<CollisionDebugger>();
            Debug.Log("   • Added CollisionDebugger component");
        }
        else
        {
            Debug.Log("   • CollisionDebugger component already present");
        }
        
        Debug.Log("✅ [Step 4/6] CollisionDebugger configuration complete");
        return debugger;
    }
    
    /// <summary>
    /// Configures debug visualization settings for CollisionDebugger.
    /// </summary>
    /// <param name="debugger">CollisionDebugger to configure</param>
    private static void ConfigureDebugVisualization(CollisionDebugger debugger)
    {
        if (debugger == null) return;
        
        Debug.Log("👁️ [Step 5/6] Configuring debug visualization...");
        
        SerializedObject serializedDebugger = new SerializedObject(debugger);
        
        // Configure debug visualization settings
        SerializedProperty showCollisionPoints = serializedDebugger.FindProperty("showCollisionPoints");
        SerializedProperty showVelocityVectors = serializedDebugger.FindProperty("showVelocityVectors");
        SerializedProperty showValidationStatus = serializedDebugger.FindProperty("showValidationStatus");
        SerializedProperty logCollisionEvents = serializedDebugger.FindProperty("logCollisionEvents");
        SerializedProperty debugDisplayDuration = serializedDebugger.FindProperty("debugDisplayDuration");
        SerializedProperty maxTrackedCollisions = serializedDebugger.FindProperty("maxTrackedCollisions");
        SerializedProperty collisionPointSize = serializedDebugger.FindProperty("collisionPointSize");
        SerializedProperty velocityVectorScale = serializedDebugger.FindProperty("velocityVectorScale");
        SerializedProperty developmentOnly = serializedDebugger.FindProperty("developmentOnly");
        
        // Set optimal debug settings
        if (showCollisionPoints != null) showCollisionPoints.boolValue = true;
        if (showVelocityVectors != null) showVelocityVectors.boolValue = true;
        if (showValidationStatus != null) showValidationStatus.boolValue = true;
        if (logCollisionEvents != null) logCollisionEvents.boolValue = true;
        if (debugDisplayDuration != null) debugDisplayDuration.floatValue = 1.0f;
        if (maxTrackedCollisions != null) maxTrackedCollisions.intValue = 50;
        if (collisionPointSize != null) collisionPointSize.floatValue = 0.1f;
        if (velocityVectorScale != null) velocityVectorScale.floatValue = 1.5f;
        if (developmentOnly != null) developmentOnly.boolValue = true;
        
        // Apply changes
        serializedDebugger.ApplyModifiedProperties();
        
        Debug.Log("   • Collision point visualization: Enabled");
        Debug.Log("   • Velocity vector visualization: Enabled");
        Debug.Log("   • Validation status visualization: Enabled");
        Debug.Log("   • Collision event logging: Enabled");
        Debug.Log($"   • Debug display duration: {debugDisplayDuration?.floatValue ?? 1.0f:F1}s");
        Debug.Log($"   • Max tracked collisions: {maxTrackedCollisions?.intValue ?? 50}");
        Debug.Log($"   • Collision point size: {collisionPointSize?.floatValue ?? 0.1f:F2}");
        Debug.Log($"   • Velocity vector scale: {velocityVectorScale?.floatValue ?? 1.5f:F1}");
        Debug.Log($"   • Development builds only: {developmentOnly?.boolValue ?? true}");
        
        Debug.Log("✅ [Step 5/6] Debug visualization configured");
    }
    
    /// <summary>
    /// Validates edge case handling setup.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to validate</param>
    /// <param name="debugger">CollisionDebugger to validate</param>
    private static void ValidateEdgeCaseSetup(CollisionManager collisionManager, CollisionDebugger debugger)
    {
        Debug.Log("🧪 [Step 6/6] Validating edge case handling setup...");
        
        int validationScore = 0;
        
        // Validate CollisionManager
        if (collisionManager != null)
        {
            validationScore++;
            Debug.Log("   • CollisionManager: Present and configured");
            
            // Check if Ball GameObject is connected
            GameObject ball = GameObject.Find("Ball");
            if (ball != null)
            {
                Rigidbody2D ballRigidbody = ball.GetComponent<Rigidbody2D>();
                if (ballRigidbody != null)
                {
                    validationScore++;
                    Debug.Log("   • Ball Rigidbody2D: Present for validation system");
                }
                else
                {
                    Debug.LogWarning("   ⚠️ Ball Rigidbody2D missing - validation system will have limited functionality");
                }
            }
            else
            {
                Debug.LogWarning("   ⚠️ Ball GameObject not found - create Ball for full validation functionality");
            }
        }
        else
        {
            Debug.LogError("   ❌ CollisionManager missing!");
        }
        
        // Validate CollisionDebugger
        if (debugger != null)
        {
            validationScore++;
            Debug.Log($"   • CollisionDebugger: Present on {debugger.gameObject.name}");
        }
        else
        {
            Debug.LogError("   ❌ CollisionDebugger missing!");
        }
        
        // Validate Debug folder
        if (AssetDatabase.IsValidFolder(DEBUG_FOLDER_PATH))
        {
            validationScore++;
            Debug.Log($"   • Debug folder: {DEBUG_FOLDER_PATH}");
        }
        else
        {
            Debug.LogWarning($"   ⚠️ Debug folder missing: {DEBUG_FOLDER_PATH}");
        }
        
        Debug.Log($"   • Validation Score: {validationScore}/4");
        
        if (validationScore >= 3) // CollisionManager, CollisionDebugger, Debug folder minimum
        {
            Debug.Log("   ✅ Edge case handling system ready for robust collision validation");
        }
        else
        {
            Debug.LogWarning("   ⚠️ Edge case handling setup incomplete - some functionality may not work correctly");
        }
        
        Debug.Log("✅ [Step 6/6] Edge case handling validation complete");
    }
    
    /// <summary>
    /// Logs successful edge case handling setup summary.
    /// </summary>
    /// <param name="collisionManager">Configured CollisionManager</param>
    /// <param name="debugger">Configured CollisionDebugger</param>
    private static void LogSuccessfulSetup(CollisionManager collisionManager, CollisionDebugger debugger)
    {
        Debug.Log("✅ [Task 1.1.3.5] Edge Case Handling configured successfully!");
        Debug.Log("📋 Edge Case Handling Summary:");
        Debug.Log($"   • CollisionManager: Enhanced with validation systems on {collisionManager?.gameObject.name ?? "N/A"}");
        Debug.Log($"   • CollisionDebugger: Added to {debugger?.gameObject.name ?? "N/A"}");
        Debug.Log($"   • Debug Folder: {DEBUG_FOLDER_PATH}");
        
        // Log edge case handling capabilities
        Debug.Log("🛡️ Edge Case Prevention Features:");
        Debug.Log("   → Stuck Ball Detection: Monitors ball velocity and position over time, applies correction forces");
        Debug.Log("   → Speed Validation: Enforces min/max ball speed constraints continuously in FixedUpdate()");
        Debug.Log("   → Tunneling Prevention: Validates collision contact points and corrects ball position");
        Debug.Log("   → Simultaneous Collision Handling: Processes multiple collisions by priority and distance");
        Debug.Log("   → Automatic Correction: Applies physics corrections without breaking gameplay flow");
        
        // Log validation parameters
        Debug.Log("⚙️ Validation Parameters:");
        Debug.Log("   • Ball Speed Range: 3.0 - 15.0 units/sec (prevents stuck balls and physics instability)");
        Debug.Log("   • Stuck Detection: 2.0s timeout with 0.1 velocity threshold");
        Debug.Log("   • Stuck Correction: 5.0 impulse force with upward bias");
        Debug.Log("   • Simultaneous Collisions: Max 3 processed per frame by distance priority");
        Debug.Log("   • Validation Debug: Enabled for development testing");
        
        // Log debugging capabilities
        Debug.Log("🐛 Debug Visualization Features:");
        Debug.Log("   → Collision Points: Color-coded spheres show recent collision locations");
        Debug.Log("   → Velocity Vectors: Green arrows show ball movement direction and speed");
        Debug.Log("   → Validation Status: Speed constraint rings around ball (green/red/orange)");
        Debug.Log("   → Event Logging: Detailed console output for collision and validation events");
        Debug.Log("   → Development Only: Debug visuals disabled in release builds");
        
        // Log validation algorithms
        Debug.Log("🔍 Validation Algorithms:");
        Debug.Log("   • Stuck Detection: velocity.magnitude < threshold for > timeout duration");
        Debug.Log("   • Speed Constraint: Clamp velocity between min/max while preserving direction");
        Debug.Log("   • Tunneling Check: Validate contact point distance from ball center");
        Debug.Log("   • Collision Queue: Priority-based processing of simultaneous physics events");
        Debug.Log("   • Memory Management: Automatic cleanup of old validation data");
        
        // Log integration points
        Debug.Log("🔗 System Integration:");
        Debug.Log("   • FixedUpdate Validation: Continuous physics anomaly detection");
        Debug.Log("   • Collision Handler Integration: Validation queue processing in all collision events");
        Debug.Log("   • Feedback System Compatibility: Works with existing audio-visual collision feedback");
        Debug.Log("   • Debug API: Public methods for external validation event logging");
        
        // Log validation system API
        Debug.Log("📊 Validation System API:");
        Debug.Log("   • GetValidationStatus() - Current validation system status");
        Debug.Log("   • CollisionDebugger.LogCollisionEvent() - Log collision for debugging");
        Debug.Log("   • CollisionDebugger.LogValidationEvent() - Log validation action");
        Debug.Log("   • CollisionDebugger.GetDebugInfo() - Formatted debug information");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Validation system automatically runs during gameplay - no manual intervention needed");
        Debug.Log("   2. Debug visualizations appear in Scene view when selecting objects with debugger");
        Debug.Log("   3. Console logging provides detailed information about validation events");
        Debug.Log("   4. Adjust validation parameters in CollisionManager Inspector for different game feel");
        Debug.Log("   5. Debug settings in CollisionDebugger Inspector control visualization detail");
        
        Debug.Log("🧪 Testing Instructions:");
        Debug.Log("   → Create high-speed ball scenarios to test speed constraint validation");
        Debug.Log("   → Create stuck ball scenarios (ball wedged between objects) to test correction");
        Debug.Log("   → Test simultaneous collisions (ball hitting corner between two objects)");
        Debug.Log("   → Monitor console output for validation events during gameplay");
        Debug.Log("   → Observe Scene view visualizations for collision points and velocity vectors");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Test validation system with Ball GameObject in various collision scenarios");
        Debug.Log("   → Fine-tune validation parameters based on gameplay testing results");
        Debug.Log("   → Monitor debug output for anomaly detection and automatic corrections");
        Debug.Log("   → Verify debug visualizations provide useful development information");
        
        Debug.Log("⚠️ Integration Requirements:");
        Debug.Log("   → Ball GameObject with Rigidbody2D required for full validation functionality");
        Debug.Log("   → CollisionManager must be properly configured from previous tasks (1.1.3.2-1.1.3.4)");
        Debug.Log("   → Debug visualizations require Scene view selection of objects with CollisionDebugger");
        Debug.Log("   → Console logging requires Unity Console window for detailed validation information");
    }
    
    /// <summary>
    /// Gets the full hierarchy path of a GameObject.
    /// </summary>
    /// <param name="gameObject">GameObject to get path for</param>
    /// <returns>Hierarchy path string</returns>
    private static string GetGameObjectPath(GameObject gameObject)
    {
        if (gameObject == null) return "N/A";
        
        string path = gameObject.name;
        Transform current = gameObject.transform.parent;
        
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        
        return path;
    }
}
#endif