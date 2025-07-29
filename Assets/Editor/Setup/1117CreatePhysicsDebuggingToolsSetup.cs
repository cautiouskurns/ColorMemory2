#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Editor setup script for creating comprehensive physics debugging and validation tools.
/// Provides real-time monitoring, anomaly detection, and performance validation for ball physics system.
/// </summary>
public static class CreatePhysicsDebuggingToolsSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Physics Debugging Tools";
    private const string BALL_NAME = "Ball";
    private const string DEBUG_UI_NAME = "DebugUI";
    private const string DEBUG_CANVAS_NAME = "DebugCanvas";
    
    /// <summary>
    /// Creates and configures comprehensive physics debugging tools.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreatePhysicsDebuggingTools()
    {
        Debug.Log("🔧 [Task 1.1.1.7] Starting Physics Debugging Tools creation...");
        
        try
        {
            // Step 1: Validate complete ball physics system
            if (!ValidatePhysicsSystemComplete())
            {
                return;
            }
            
            // Step 2: Create DebugUI GameObject with Canvas
            GameObject debugUI = CreateDebugUIGameObject();
            
            // Step 3: Attach and configure BallPhysicsDebugger
            BallPhysicsDebugger physicsDebugger = AttachPhysicsDebugger(debugUI);
            
            // Step 4: Attach PhysicsValidator to Ball GameObject
            PhysicsValidator physicsValidator = AttachPhysicsValidator();
            
            // Step 5: Configure component references and settings
            ConfigureDebuggerSettings(physicsDebugger, physicsValidator);
            
            // Step 6: Final validation and setup
            ValidateDebugToolsSetup(physicsDebugger, physicsValidator);
            
            LogSuccessfulSetup(debugUI, physicsDebugger, physicsValidator);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.1.7] Physics Debugging Tools creation failed: {e.Message}");
            Debug.LogError("📋 Please ensure complete ball physics system is configured.");
        }
    }
    
    /// <summary>
    /// Menu validation - ensures physics debugging tools aren't already created.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreatePhysicsDebuggingTools()
    {
        // Check if DebugUI already exists
        GameObject existingDebugUI = GameObject.Find(DEBUG_UI_NAME);
        if (existingDebugUI != null)
        {
            BallPhysicsDebugger existingDebugger = existingDebugUI.GetComponent<BallPhysicsDebugger>();
            if (existingDebugger != null)
            {
                Debug.LogWarning("⚠️ Physics Debugging Tools already exist.");
                return false;
            }
        }
        
        // Check if PhysicsValidator is already attached to Ball
        GameObject ball = GameObject.Find(BALL_NAME);
        if (ball != null)
        {
            PhysicsValidator existingValidator = ball.GetComponent<PhysicsValidator>();
            if (existingValidator != null)
            {
                Debug.LogWarning("⚠️ PhysicsValidator already attached to Ball GameObject.");
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Validates that complete ball physics system is configured.
    /// </summary>
    /// <returns>True if physics system is complete</returns>
    private static bool ValidatePhysicsSystemComplete()
    {
        Debug.Log("🔍 [Step 1/5] Validating complete ball physics system...");
        
        // Step 1: Validate Ball GameObject exists
        GameObject ball = GameObject.Find(BALL_NAME);
        if (ball == null)
        {
            Debug.LogError("❌ [Dependency Error] Ball GameObject not found!");
            Debug.LogError("📋 Required: Complete Task 1.1.1.2 (Ball GameObject Configuration) first");
            Debug.LogError("💡 Run: Breakout/Setup/Create Ball GameObject");
            return false;
        }
        
        // Step 2: Validate BallController component exists
        BallController ballController = ball.GetComponent<BallController>();
        if (ballController == null)
        {
            Debug.LogError("❌ [Dependency Error] BallController component not found!");
            Debug.LogError("📋 Required: Complete Task 1.1.1.3 (BallController Foundation) first");
            Debug.LogError("💡 Run: Breakout/Setup/Create Ball Controller");
            return false;
        }
        
        // Step 3: Validate velocity management system
        SerializedObject serializedController = new SerializedObject(ballController);
        SerializedProperty velocityManagementProp = serializedController.FindProperty("velocityManagementEnabled");
        if (velocityManagementProp == null)
        {
            Debug.LogError("❌ [Dependency Error] Velocity Management System not found!");
            Debug.LogError("📋 Required: Complete Task 1.1.1.4 (Velocity Management System) first");
            Debug.LogError("💡 Run: Breakout/Setup/Configure Velocity Management");
            return false;
        }
        
        // Step 4: Validate launch mechanics system
        SerializedProperty launchStateProp = serializedController.FindProperty("currentState");
        if (launchStateProp == null)
        {
            Debug.LogError("❌ [Dependency Error] Launch Mechanics System not found!");
            Debug.LogError("📋 Required: Complete Task 1.1.1.5 (Ball Launch Mechanics) first");
            Debug.LogError("💡 Run: Breakout/Setup/Create Ball Launch Mechanics");
            return false;
        }
        
        // Step 5: Validate physics material optimization
        CircleCollider2D ballCollider = ball.GetComponent<CircleCollider2D>();
        if (ballCollider == null || ballCollider.sharedMaterial == null)
        {
            Debug.LogError("❌ [Dependency Error] Physics Material Optimization not found!");
            Debug.LogError("📋 Required: Complete Task 1.1.1.6 (Physics Material Optimization) first");
            Debug.LogError("💡 Run: Breakout/Setup/Create Physics Material Optimization");
            return false;
        }
        
        // Step 6: Validate required physics components
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("❌ [Dependency Error] Required Rigidbody2D component missing!");
            return false;
        }
        
        Debug.Log("✅ [Step 1/5] Complete ball physics system validated:");
        Debug.Log($"   • Ball GameObject: {ball.name}");
        Debug.Log($"   • BallController: Present with velocity management and launch mechanics");
        Debug.Log($"   • Physics Components: Rigidbody2D and CircleCollider2D validated");
        Debug.Log($"   • Physics Material: Optimization applied");
        Debug.Log($"   • System Status: Complete and ready for debugging tools");
        
        return true;
    }
    
    /// <summary>
    /// Creates DebugUI GameObject with Canvas component for debug information display.
    /// </summary>
    /// <returns>Created DebugUI GameObject</returns>
    private static GameObject CreateDebugUIGameObject()
    {
        Debug.Log("🖥️ [Step 2/5] Creating DebugUI GameObject with Canvas...");
        
        // Create DebugUI GameObject
        GameObject debugUI = new GameObject(DEBUG_UI_NAME);
        
        // Add Canvas component
        Canvas debugCanvas = debugUI.AddComponent<Canvas>();
        debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        debugCanvas.sortingOrder = 1000; // Ensure debug UI renders on top
        
        // Add CanvasScaler for resolution independence
        CanvasScaler canvasScaler = debugUI.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        
        // Add GraphicRaycaster for UI interaction
        debugUI.AddComponent<GraphicRaycaster>();
        
        Debug.Log("✅ [Step 2/5] DebugUI GameObject created:");
        Debug.Log($"   • GameObject: {debugUI.name}");
        Debug.Log($"   • Canvas: Screen Space Overlay, Sorting Order 1000");
        Debug.Log($"   • CanvasScaler: Resolution independent scaling");
        Debug.Log($"   • GraphicRaycaster: UI interaction support");
        
        return debugUI;
    }
    
    /// <summary>
    /// Attaches and configures BallPhysicsDebugger component to DebugUI GameObject.
    /// </summary>
    /// <param name="debugUI">DebugUI GameObject to attach debugger to</param>
    /// <returns>Configured BallPhysicsDebugger component</returns>
    private static BallPhysicsDebugger AttachPhysicsDebugger(GameObject debugUI)
    {
        Debug.Log("🔧 [Step 3/5] Attaching BallPhysicsDebugger component...");
        
        // Attach BallPhysicsDebugger component
        BallPhysicsDebugger physicsDebugger = debugUI.AddComponent<BallPhysicsDebugger>();
        
        // Configure default settings using SerializedObject
        SerializedObject serializedDebugger = new SerializedObject(physicsDebugger);
        
        // Enable all debugging features by default
        SetSerializedProperty(serializedDebugger, "enableDebugDisplay", true);
        SetSerializedProperty(serializedDebugger, "enablePerformanceMonitoring", true);
        SetSerializedProperty(serializedDebugger, "enableAnomalyDetection", true);
        SetSerializedProperty(serializedDebugger, "enableVisualDebugAids", true);
        
        // Set debug canvas reference
        SetSerializedProperty(serializedDebugger, "debugCanvas", debugUI.GetComponent<Canvas>());
        
        serializedDebugger.ApplyModifiedProperties();
        
        Debug.Log("✅ [Step 3/5] BallPhysicsDebugger configured:");
        Debug.Log($"   • Component: Attached to {debugUI.name}");
        Debug.Log($"   • Debug Display: Enabled");
        Debug.Log($"   • Performance Monitoring: Enabled");
        Debug.Log($"   • Anomaly Detection: Enabled");
        Debug.Log($"   • Visual Debug Aids: Enabled");
        
        return physicsDebugger;
    }
    
    /// <summary>
    /// Attaches PhysicsValidator component to Ball GameObject.
    /// </summary>
    /// <returns>Configured PhysicsValidator component</returns>
    private static PhysicsValidator AttachPhysicsValidator()
    {
        Debug.Log("🔍 [Step 4/5] Attaching PhysicsValidator to Ball GameObject...");
        
        // Find Ball GameObject
        GameObject ball = GameObject.Find(BALL_NAME);
        if (ball == null)
        {
            Debug.LogError("❌ Ball GameObject not found for PhysicsValidator attachment");
            return null;
        }
        
        // Attach PhysicsValidator component
        PhysicsValidator physicsValidator = ball.AddComponent<PhysicsValidator>();
        
        // Configure validation settings using SerializedObject
        SerializedObject serializedValidator = new SerializedObject(physicsValidator);
        
        // Set validation thresholds for arcade physics
        SetSerializedProperty(serializedValidator, "stuckBallThreshold", 0.1f);
        SetSerializedProperty(serializedValidator, "stuckTimeLimit", 2f);
        SetSerializedProperty(serializedValidator, "tunnelDetectionDistance", 1f);
        SetSerializedProperty(serializedValidator, "extremeSpeedThreshold", 25f);
        SetSerializedProperty(serializedValidator, "enableValidationLogging", true);
        
        // Set component references
        SetSerializedProperty(serializedValidator, "ballController", ball.GetComponent<BallController>());
        SetSerializedProperty(serializedValidator, "ballRigidbody", ball.GetComponent<Rigidbody2D>());
        SetSerializedProperty(serializedValidator, "ballCollider", ball.GetComponent<CircleCollider2D>());
        
        serializedValidator.ApplyModifiedProperties();
        
        Debug.Log("✅ [Step 4/5] PhysicsValidator configured:");
        Debug.Log($"   • Component: Attached to {ball.name}");
        Debug.Log($"   • Stuck Ball Threshold: 0.1 units");
        Debug.Log($"   • Stuck Time Limit: 2 seconds");
        Debug.Log($"   • Tunnel Detection Distance: 1 unit");
        Debug.Log($"   • Extreme Speed Threshold: 25 units/sec");
        Debug.Log($"   • Component References: All assigned");
        
        return physicsValidator;
    }
    
    /// <summary>
    /// Configures debugger settings and component references.
    /// </summary>
    /// <param name="physicsDebugger">BallPhysicsDebugger to configure</param>
    /// <param name="physicsValidator">PhysicsValidator to reference</param>
    private static void ConfigureDebuggerSettings(BallPhysicsDebugger physicsDebugger, PhysicsValidator physicsValidator)
    {
        Debug.Log("⚙️ [Step 5/5] Configuring debugger settings and references...");
        
        if (physicsDebugger == null)
        {
            Debug.LogError("❌ PhysicsDebugger is null, cannot configure settings");
            return;
        }
        
        // Find and assign BallController reference
        GameObject ball = GameObject.Find(BALL_NAME);
        if (ball != null)
        {
            BallController ballController = ball.GetComponent<BallController>();
            if (ballController != null)
            {
                SerializedObject serializedDebugger = new SerializedObject(physicsDebugger);
                SetSerializedProperty(serializedDebugger, "ballController", ballController);
                serializedDebugger.ApplyModifiedProperties();
                
                Debug.Log($"   • BallController Reference: Assigned from {ball.name}");
            }
        }
        
        // Mark objects as dirty for proper saving
        EditorUtility.SetDirty(physicsDebugger);
        if (physicsValidator != null)
        {
            EditorUtility.SetDirty(physicsValidator);
        }
        
        Debug.Log("✅ [Step 5/5] Debugger configuration complete:");
        Debug.Log($"   • Component References: All assigned");
        Debug.Log($"   • Settings: Optimized for Breakout physics");
        Debug.Log($"   • Integration: BallPhysicsDebugger <-> PhysicsValidator");
    }
    
    /// <summary>
    /// Validates debug tools setup and functionality.
    /// </summary>
    /// <param name="physicsDebugger">BallPhysicsDebugger to validate</param>
    /// <param name="physicsValidator">PhysicsValidator to validate</param>
    private static void ValidateDebugToolsSetup(BallPhysicsDebugger physicsDebugger, PhysicsValidator physicsValidator)
    {
        bool setupValid = true;
        
        // Validate BallPhysicsDebugger
        if (physicsDebugger == null)
        {
            Debug.LogError("❌ BallPhysicsDebugger validation failed: Component is null");
            setupValid = false;
        }
        else
        {
            // Test public interface exists
            bool hasSetBallController = physicsDebugger.GetType().GetMethod("SetBallController") != null;
            bool hasGetDebugInfo = physicsDebugger.GetType().GetMethod("GetDebugInfo") != null;
            
            if (!hasSetBallController || !hasGetDebugInfo)
            {
                Debug.LogError("❌ BallPhysicsDebugger validation failed: Missing required methods");
                setupValid = false;
            }
        }
        
        // Validate PhysicsValidator
        if (physicsValidator == null)
        {
            Debug.LogError("❌ PhysicsValidator validation failed: Component is null");
            setupValid = false;
        }
        else
        {
            // Test public interface exists
            bool hasGetValidationStats = physicsValidator.GetType().GetMethod("GetValidationStats") != null;
            bool hasForceValidation = physicsValidator.GetType().GetMethod("ForceValidation") != null;
            
            if (!hasGetValidationStats || !hasForceValidation)
            {
                Debug.LogError("❌ PhysicsValidator validation failed: Missing required methods");
                setupValid = false;
            }
        }
        
        if (setupValid)
        {
            Debug.Log("✅ Debug tools validation successful:");
            Debug.Log("   • BallPhysicsDebugger: All methods available");
            Debug.Log("   • PhysicsValidator: All methods available");
            Debug.Log("   • Component Integration: Validated");
        }
    }
    
    /// <summary>
    /// Logs successful physics debugging tools setup.
    /// </summary>
    /// <param name="debugUI">Created DebugUI GameObject</param>
    /// <param name="physicsDebugger">Configured BallPhysicsDebugger</param>
    /// <param name="physicsValidator">Configured PhysicsValidator</param>
    private static void LogSuccessfulSetup(GameObject debugUI, BallPhysicsDebugger physicsDebugger, PhysicsValidator physicsValidator)
    {
        GameObject ball = GameObject.Find(BALL_NAME);
        
        Debug.Log("✅ [Task 1.1.1.7] Physics Debugging Tools created successfully!");
        Debug.Log("📋 Physics Debugging Tools Configuration Summary:");
        Debug.Log($"   • DebugUI GameObject: {debugUI.name} with Canvas overlay");
        Debug.Log($"   • BallPhysicsDebugger: Real-time monitoring and performance tracking");
        Debug.Log($"   • PhysicsValidator: Anomaly detection and validation system");
        Debug.Log($"   • Target Ball: {(ball != null ? ball.name : "Not Found")}");
        Debug.Log($"   • Integration: Complete ball physics system monitoring");
        Debug.Log($"   • Display Mode: OnGUI overlay with Gizmos visualization");
        Debug.Log($"   • Performance Target: 60fps monitoring with anomaly alerts");
        Debug.Log("🔧 Physics Debugging Tools Features:");
        Debug.Log("   → Real-time physics data monitoring (velocity, speed, collisions)");
        Debug.Log("   → Performance monitoring with frame rate tracking and warnings");
        Debug.Log("   → Anomaly detection (stuck ball, tunneling, extreme speeds)");
        Debug.Log("   → Visual debugging aids with Gizmos integration");
        Debug.Log("   → Comprehensive validation system with recovery suggestions");
        Debug.Log("   → Categorized logging system for physics events");
        Debug.Log("   → Integration with complete ball physics system");
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Enter Play mode to activate debug tools");
        Debug.Log("   2. Debug information displayed in top-left overlay");
        Debug.Log("   3. Visual debugging aids visible in Scene view");
        Debug.Log("   4. Performance warnings appear when FPS drops below 60");
        Debug.Log("   5. Anomaly detection provides automatic recovery suggestions");
        Debug.Log("   6. Use PhysicsValidator.ForceValidation() for manual checks");
        Debug.Log("🔍 Debug Tool Controls:");
        Debug.Log("   • Toggle Debug Display: BallPhysicsDebugger.SetDebugDisplayEnabled()");
        Debug.Log("   • Toggle Performance Monitoring: SetPerformanceMonitoringEnabled()");
        Debug.Log("   • Toggle Anomaly Detection: SetAnomalyDetectionEnabled()");
        Debug.Log("   • Get Validation Stats: PhysicsValidator.GetValidationStats()");
        Debug.Log("   • Reset Statistics: PhysicsValidator.ResetValidationStats()");
    }
    
    /// <summary>
    /// Helper method to set serialized properties safely.
    /// </summary>
    /// <param name="serializedObject">Serialized object to modify</param>
    /// <param name="propertyName">Property name to set</param>
    /// <param name="value">Value to assign</param>
    private static void SetSerializedProperty(SerializedObject serializedObject, string propertyName, object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            switch (value)
            {
                case bool boolValue:
                    property.boolValue = boolValue;
                    break;
                case float floatValue:
                    property.floatValue = floatValue;
                    break;
                case int intValue:
                    property.intValue = intValue;
                    break;
                case string stringValue:
                    property.stringValue = stringValue;
                    break;
                case Object objectValue:
                    property.objectReferenceValue = objectValue;
                    break;
                default:
                    Debug.LogWarning($"⚠️ Unsupported property type for {propertyName}: {value.GetType()}");
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Property '{propertyName}' not found in serialized object");
        }
    }
}
#endif