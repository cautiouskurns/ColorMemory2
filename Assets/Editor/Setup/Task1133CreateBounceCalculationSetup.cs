#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for configuring bounce angle calculation parameters in CollisionManager.
/// Provides automated bounce calculation setup with parameter tuning and validation.
/// </summary>
public static class Task1133CreateBounceCalculationSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1133 Configure Bounce Calculation";
    
    /// <summary>
    /// Configures bounce calculation parameters on existing CollisionManager.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void ConfigureBounceCalculation()
    {
        Debug.Log("📋 [Task 1.1.3.3] Starting Bounce Calculation configuration...");
        
        try
        {
            // Step 1: Find and validate CollisionManager
            CollisionManager collisionManager = FindCollisionManager();
            
            // Step 2: Configure bounce calculation parameters
            ConfigureBounceParameters(collisionManager);
            
            // Step 3: Validate Ball and Paddle references
            ValidateGameObjectReferences(collisionManager);
            
            // Step 4: Test bounce calculation functionality
            TestBounceCalculationSetup(collisionManager);
            
            // Step 5: Configure visualization settings
            ConfigureVisualizationSettings(collisionManager);
            
            // Step 6: Save and finalize
            EditorUtility.SetDirty(collisionManager.gameObject);
            AssetDatabase.SaveAssets();
            
            LogSuccessfulSetup(collisionManager);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.3.3] Bounce Calculation configuration failed: {e.Message}");
            Debug.LogError("📋 Please ensure CollisionManager exists and is properly configured.");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if CollisionManager exists.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateConfigureBounceCalculation()
    {
        return GameObject.FindFirstObjectByType<CollisionManager>() != null;
    }
    
    /// <summary>
    /// Finds and validates the CollisionManager in the scene.
    /// </summary>
    /// <returns>CollisionManager component</returns>
    private static CollisionManager FindCollisionManager()
    {
        Debug.Log("🔍 [Step 1/5] Finding CollisionManager...");
        
        CollisionManager collisionManager = GameObject.FindFirstObjectByType<CollisionManager>();
        
        if (collisionManager == null)
        {
            Debug.LogError("❌ CollisionManager not found in scene!");
            Debug.LogError("📋 Please run 'Breakout/Setup/Task1132 Create Collision Manager' first.");
            throw new System.NullReferenceException("CollisionManager component is required");
        }
        
        Debug.Log($"✅ [Step 1/5] CollisionManager found: {collisionManager.gameObject.name}");
        return collisionManager;
    }
    
    /// <summary>
    /// Configures bounce calculation parameters using SerializedObject for proper Inspector integration.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to configure</param>
    private static void ConfigureBounceParameters(CollisionManager collisionManager)
    {
        Debug.Log("⚙️ [Step 2/5] Configuring bounce calculation parameters...");
        
        SerializedObject serializedManager = new SerializedObject(collisionManager);
        
        // Configure bounce angle parameters
        SerializedProperty minBounceAngle = serializedManager.FindProperty("minBounceAngle");
        SerializedProperty maxBounceAngle = serializedManager.FindProperty("maxBounceAngle");
        SerializedProperty paddleWidth = serializedManager.FindProperty("paddleWidth");
        SerializedProperty enableBounceVisualization = serializedManager.FindProperty("enableBounceVisualization");
        
        if (minBounceAngle != null && maxBounceAngle != null && paddleWidth != null)
        {
            // Set optimal bounce angle parameters for arcade gameplay
            minBounceAngle.floatValue = 15f;  // Minimum angle (right edge)
            maxBounceAngle.floatValue = 165f; // Maximum angle (left edge)
            
            // Auto-detect paddle width from Paddle GameObject
            GameObject paddle = GameObject.Find("Paddle");
            if (paddle != null)
            {
                BoxCollider2D paddleCollider = paddle.GetComponent<BoxCollider2D>();
                if (paddleCollider != null)
                {
                    paddleWidth.floatValue = paddleCollider.size.x;
                    Debug.Log($"   • Auto-detected paddle width: {paddleCollider.size.x:F2}");
                }
                else
                {
                    paddleWidth.floatValue = 2.0f; // Safe default
                    Debug.LogWarning("   ⚠️ Paddle collider not found. Using default width: 2.0");
                }
            }
            else
            {
                paddleWidth.floatValue = 2.0f; // Safe default
                Debug.LogWarning("   ⚠️ Paddle GameObject not found. Using default width: 2.0");
            }
            
            // Enable bounce visualization by default
            if (enableBounceVisualization != null)
            {
                enableBounceVisualization.boolValue = true;
            }
            
            // Apply changes
            serializedManager.ApplyModifiedProperties();
            
            Debug.Log($"   • Min Bounce Angle: {minBounceAngle.floatValue:F1}°");
            Debug.Log($"   • Max Bounce Angle: {maxBounceAngle.floatValue:F1}°");
            Debug.Log($"   • Paddle Width: {paddleWidth.floatValue:F2}");
            Debug.Log($"   • Bounce Visualization: Enabled");
        }
        else
        {
            Debug.LogError("❌ Could not find bounce calculation parameters in CollisionManager!");
            throw new System.NullReferenceException("Bounce calculation properties not found");
        }
        
        Debug.Log("✅ [Step 2/5] Bounce calculation parameters configured");
    }
    
    /// <summary>
    /// Validates Ball and Paddle GameObject references for bounce calculation.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to validate</param>
    private static void ValidateGameObjectReferences(CollisionManager collisionManager)
    {
        Debug.Log("🧪 [Step 3/5] Validating GameObject references...");
        
        // Validate Ball GameObject
        GameObject ball = GameObject.Find("Ball");
        if (ball != null)
        {
            Rigidbody2D ballRigidbody = ball.GetComponent<Rigidbody2D>();
            Collider2D ballCollider = ball.GetComponent<Collider2D>();
            
            Debug.Log($"   • Ball GameObject: {ball.name}");
            Debug.Log($"   • Ball Rigidbody2D: {(ballRigidbody != null ? "Present" : "Missing!")}");
            Debug.Log($"   • Ball Collider2D: {(ballCollider != null ? "Present" : "Missing!")}");
            
            if (ballRigidbody == null)
            {
                Debug.LogError("❌ Ball GameObject missing Rigidbody2D! Bounce calculation requires Rigidbody2D for velocity modification.");
            }
            
            if (ballCollider == null)
            {
                Debug.LogError("❌ Ball GameObject missing Collider2D! Collision detection requires Collider2D.");
            }
            
            // Check Ball layer
            int ballLayer = LayerMask.NameToLayer("Ball");
            if (ballLayer != -1 && ball.layer == ballLayer)
            {
                Debug.Log($"   • Ball Layer: Correct ({LayerMask.LayerToName(ball.layer)})");
            }
            else
            {
                Debug.LogWarning($"   ⚠️ Ball should be on 'Ball' layer for proper collision detection");
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️ Ball GameObject not found. Create Ball GameObject for bounce calculation testing.");
        }
        
        // Validate Paddle GameObject
        GameObject paddle = GameObject.Find("Paddle");
        if (paddle != null)
        {
            BoxCollider2D paddleCollider = paddle.GetComponent<BoxCollider2D>();
            
            Debug.Log($"   • Paddle GameObject: {paddle.name}");
            Debug.Log($"   • Paddle BoxCollider2D: {(paddleCollider != null ? "Present" : "Missing!")}");
            
            if (paddleCollider != null)
            {
                Debug.Log($"   • Paddle Size: {paddleCollider.size.x:F2} x {paddleCollider.size.y:F2}");
            }
            else
            {
                Debug.LogWarning("   ⚠️ Paddle GameObject missing BoxCollider2D! Hit position calculation may be inaccurate.");
            }
            
            // Check Paddle layer
            int paddleLayer = LayerMask.NameToLayer("Paddle");
            if (paddleLayer != -1 && paddle.layer == paddleLayer)
            {
                Debug.Log($"   • Paddle Layer: Correct ({LayerMask.LayerToName(paddle.layer)})");
            }
            else
            {
                Debug.LogWarning($"   ⚠️ Paddle should be on 'Paddle' layer for proper collision detection");
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️ Paddle GameObject not found. Create Paddle GameObject for bounce calculation testing.");
        }
        
        Debug.Log("✅ [Step 3/5] GameObject reference validation complete");
    }
    
    /// <summary>
    /// Tests bounce calculation functionality with sample hit positions.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to test</param>
    private static void TestBounceCalculationSetup(CollisionManager collisionManager)
    {
        Debug.Log("🧪 [Step 4/5] Testing bounce calculation functionality...");
        
        try
        {
            // Test bounce calculation at different hit positions
            Debug.Log("   • Testing bounce angles at different paddle positions:");
            
            // Left edge (-1.0)
            float leftAngle = collisionManager.TestBounceCalculation(-1f);
            Debug.Log($"     - Left Edge (-1.0): {leftAngle:F1}° (should be ~165°)");
            
            // Center (0.0)
            float centerAngle = collisionManager.TestBounceCalculation(0f);
            Debug.Log($"     - Center (0.0): {centerAngle:F1}° (should be ~90°)");
            
            // Right edge (1.0)
            float rightAngle = collisionManager.TestBounceCalculation(1f);
            Debug.Log($"     - Right Edge (1.0): {rightAngle:F1}° (should be ~15°)");
            
            // Validate angle progression
            if (leftAngle > centerAngle && centerAngle > rightAngle)
            {
                Debug.Log("   ✅ Bounce angle progression correct: Left > Center > Right");
            }
            else
            {
                Debug.LogWarning("   ⚠️ Bounce angle progression may be incorrect");
            }
            
            // Test edge cases
            float extremeLeft = collisionManager.TestBounceCalculation(-2f); // Should clamp to -1.0
            float extremeRight = collisionManager.TestBounceCalculation(2f); // Should clamp to 1.0
            
            Debug.Log($"     - Extreme Left (-2.0): {extremeLeft:F1}° (should equal left edge)");
            Debug.Log($"     - Extreme Right (2.0): {extremeRight:F1}° (should equal right edge)");
            
            // Get debug information
            string debugInfo = collisionManager.GetBounceCalculationDebug();
            Debug.Log($"   • Bounce Calculation Debug Info:\n{debugInfo}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Bounce calculation test failed: {e.Message}");
        }
        
        Debug.Log("✅ [Step 4/5] Bounce calculation testing complete");
    }
    
    /// <summary>
    /// Configures visualization settings for bounce angle debugging.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to configure</param>
    private static void ConfigureVisualizationSettings(CollisionManager collisionManager)
    {
        Debug.Log("👁️ [Step 5/5] Configuring bounce visualization...");
        
        // Enable collision logging for detailed bounce debugging
        SerializedObject serializedManager = new SerializedObject(collisionManager);
        SerializedProperty enableCollisionLogging = serializedManager.FindProperty("enableCollisionLogging");
        
        if (enableCollisionLogging != null)
        {
            enableCollisionLogging.boolValue = true;
            serializedManager.ApplyModifiedProperties();
            Debug.Log("   • Collision logging enabled for bounce debugging");
        }
        
        Debug.Log("   • Scene view bounce visualization enabled (green paddle width, blue angle rays)");
        Debug.Log("   • Red sphere and ray will show last hit position and bounce direction during play");
        
        Debug.Log("✅ [Step 5/5] Bounce visualization configured");
    }
    
    /// <summary>
    /// Logs successful bounce calculation setup summary.
    /// </summary>
    /// <param name="collisionManager">Configured CollisionManager</param>
    private static void LogSuccessfulSetup(CollisionManager collisionManager)
    {
        Debug.Log("✅ [Task 1.1.3.3] Bounce Calculation configured successfully!");
        Debug.Log("📋 Bounce Calculation Summary:");
        Debug.Log($"   • Component: Enhanced CollisionManager on {collisionManager.gameObject.name}");
        Debug.Log($"   • Location: {GetGameObjectPath(collisionManager.gameObject)}");
        
        // Log bounce calculation capabilities
        Debug.Log("🎯 Bounce Calculation Features:");
        Debug.Log("   → Player Control: Ball direction controlled by paddle hit position");
        Debug.Log("   → Arcade Physics: Bounce angles between 15-165 degrees prevent horizontal bounces");
        Debug.Log("   → Speed Preservation: Ball speed maintained through direction changes");
        Debug.Log("   → Hit Position Detection: Accurate contact point calculation for precise control");
        Debug.Log("   → Angle Mapping: Smooth transition from edge bounces (165°) to center bounces (15°)");
        Debug.Log("   → Scene Visualization: Real-time bounce angle display in Scene view");
        
        // Log bounce mechanics
        Debug.Log("🎮 Player Control Mechanics:");
        Debug.Log("   • Left Edge Hit: Ball bounces at ~165° (more horizontal, leftward)");
        Debug.Log("   • Center Hit: Ball bounces at ~90° (straight up)");
        Debug.Log("   • Right Edge Hit: Ball bounces at ~15° (more horizontal, rightward)");
        Debug.Log("   • Smooth Progression: Hit position smoothly maps to bounce angle");
        
        // Log configuration parameters
        string debugInfo = collisionManager.GetBounceCalculationDebug();
        Debug.Log($"🔧 Current Configuration:\n{debugInfo}");
        
        // Log bounce calculation API
        Debug.Log("📊 Bounce Calculation API:");
        Debug.Log("   • ConfigureBounceCalculation(minAngle, maxAngle, width) - Update parameters");
        Debug.Log("   • TestBounceCalculation(hitPosition) - Test angle calculation");
        Debug.Log("   • GetBounceCalculationDebug() - Get detailed debug information");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Bounce calculation automatically activates during Ball-Paddle collisions");
        Debug.Log("   2. Enable collision logging to monitor bounce calculations in real-time");
        Debug.Log("   3. Scene view shows bounce angle visualization (select CollisionManager)");
        Debug.Log("   4. Red indicators appear during play to show hit position and bounce direction");
        Debug.Log("   5. Adjust min/max bounce angles in Inspector for different gameplay feel");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Connect Ball collision events to CollisionManager.OnCollisionEnter2D()");
        Debug.Log("   → Test bounce calculation in Play mode with Ball-Paddle collisions");
        Debug.Log("   → Fine-tune bounce angle parameters for optimal gameplay feel");
        Debug.Log("   → Verify ball speed preservation through bounce direction changes");
        
        Debug.Log("⚠️ Integration Requirements:");
        Debug.Log("   → Ball GameObject must forward collision events to CollisionManager");
        Debug.Log("   → Ball requires Rigidbody2D for velocity modification");
        Debug.Log("   → Paddle requires BoxCollider2D for accurate hit position calculation");
        Debug.Log("   → Both Ball and Paddle should be on correct physics layers");
    }
    
    /// <summary>
    /// Gets the full hierarchy path of a GameObject.
    /// </summary>
    /// <param name="gameObject">GameObject to get path for</param>
    /// <returns>Hierarchy path string</returns>
    private static string GetGameObjectPath(GameObject gameObject)
    {
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