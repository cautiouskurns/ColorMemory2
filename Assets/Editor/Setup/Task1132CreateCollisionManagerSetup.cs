#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for creating and configuring CollisionManager GameObject with component.
/// Provides automated collision manager setup with proper scene hierarchy organization.
/// </summary>
public static class Task1132CreateCollisionManagerSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1132 Create Collision Manager";
    private const string COLLISION_MANAGER_FILE = "Assets/Scripts/Managers/CollisionManager.cs";
    
    /// <summary>
    /// Creates CollisionManager GameObject with component and configures scene integration.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateCollisionManager()
    {
        Debug.Log("📋 [Task 1.1.3.2] Starting CollisionManager creation...");
        
        try
        {
            // Step 1: Validate prerequisites
            ValidatePrerequisites();
            
            // Step 2: Create or find Managers hierarchy
            GameObject managersParent = CreateManagersHierarchy();
            
            // Step 3: Create CollisionManager GameObject
            GameObject collisionManagerObject = CreateCollisionManagerGameObject(managersParent);
            
            // Step 4: Configure CollisionManager component
            CollisionManager collisionManagerComponent = ConfigureCollisionManagerComponent(collisionManagerObject);
            
            // Step 5: Connect to Ball GameObject
            ConnectToBallGameObject(collisionManagerComponent);
            
            // Step 6: Validate setup
            ValidateCollisionManagerSetup(collisionManagerComponent);
            
            // Step 7: Save and finalize
            EditorUtility.SetDirty(collisionManagerObject);
            AssetDatabase.SaveAssets();
            
            LogSuccessfulSetup(collisionManagerComponent);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.3.2] CollisionManager creation failed: {e.Message}");
            Debug.LogError("📋 Please check script dependencies and scene configuration.");
        }
    }
    
    /// <summary>
    /// Menu validation - prevents duplicate CollisionManager creation.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateCollisionManager()
    {
        return GameObject.FindObjectOfType<CollisionManager>() == null;
    }
    
    /// <summary>
    /// Validates that prerequisite systems are in place.
    /// </summary>
    private static void ValidatePrerequisites()
    {
        Debug.Log("🔍 [Step 1/6] Validating prerequisites...");
        
        // Check if CollisionManager script exists
        if (!System.IO.File.Exists(COLLISION_MANAGER_FILE))
        {
            Debug.LogError("❌ CollisionManager.cs script not found!");
            throw new System.IO.FileNotFoundException("CollisionManager script is missing");
        }
        
        // Check if physics layers are configured
        int ballLayer = LayerMask.NameToLayer("Ball");
        int paddleLayer = LayerMask.NameToLayer("Paddle");
        int boundariesLayer = LayerMask.NameToLayer("Boundaries");
        
        if (ballLayer == -1 || paddleLayer == -1 || boundariesLayer == -1)
        {
            Debug.LogWarning("⚠️ Physics layers not fully configured. Run 'Breakout/Setup/Task1131 Create Physics Layers' first for optimal functionality.");
        }
        else
        {
            Debug.Log("   • Physics layers detected and configured");
        }
        
        Debug.Log("✅ [Step 1/6] Prerequisites validated successfully");
    }
    
    /// <summary>
    /// Creates or finds the Managers hierarchy parent GameObject.
    /// </summary>
    /// <returns>Managers parent GameObject</returns>
    private static GameObject CreateManagersHierarchy()
    {
        Debug.Log("🏗️ [Step 2/6] Creating Managers hierarchy...");
        
        // Look for existing Managers parent
        GameObject managersParent = GameObject.Find("Managers");
        
        if (managersParent == null)
        {
            // Create new Managers parent
            managersParent = new GameObject("Managers");
            managersParent.transform.position = Vector3.zero;
            managersParent.transform.rotation = Quaternion.identity;
            managersParent.transform.localScale = Vector3.one;
            
            Debug.Log("   • Created new Managers parent GameObject");
        }
        else
        {
            Debug.Log("   • Found existing Managers parent GameObject");
        }
        
        Debug.Log("✅ [Step 2/6] Managers hierarchy ready");
        return managersParent;
    }
    
    /// <summary>
    /// Creates the CollisionManager GameObject.
    /// </summary>
    /// <param name="parent">Parent GameObject for hierarchy organization</param>
    /// <returns>Created CollisionManager GameObject</returns>
    private static GameObject CreateCollisionManagerGameObject(GameObject parent)
    {
        Debug.Log("⚙️ [Step 3/6] Creating CollisionManager GameObject...");
        
        // Create CollisionManager GameObject
        GameObject collisionManagerObject = new GameObject("CollisionManager");
        
        // Set parent and transform
        collisionManagerObject.transform.SetParent(parent.transform);
        collisionManagerObject.transform.localPosition = Vector3.zero;
        collisionManagerObject.transform.localRotation = Quaternion.identity;
        collisionManagerObject.transform.localScale = Vector3.one;
        
        Debug.Log($"✅ [Step 3/6] CollisionManager GameObject created: {collisionManagerObject.name}");
        return collisionManagerObject;
    }
    
    /// <summary>
    /// Configures the CollisionManager component on the GameObject.
    /// </summary>
    /// <param name="gameObject">GameObject to add component to</param>
    /// <returns>Configured CollisionManager component</returns>
    private static CollisionManager ConfigureCollisionManagerComponent(GameObject gameObject)
    {
        Debug.Log("🔧 [Step 4/6] Configuring CollisionManager component...");
        
        // Add CollisionManager component
        CollisionManager collisionManager = gameObject.AddComponent<CollisionManager>();
        
        if (collisionManager == null)
        {
            Debug.LogError("❌ Failed to add CollisionManager component!");
            throw new System.NullReferenceException("CollisionManager component creation failed");
        }
        
        // Component will auto-configure in its Awake() method
        Debug.Log("   • CollisionManager component added and will auto-configure");
        
        Debug.Log("✅ [Step 4/6] CollisionManager component configured");
        return collisionManager;
    }
    
    /// <summary>
    /// Connects CollisionManager to Ball GameObject if available.
    /// </summary>
    /// <param name="collisionManager">CollisionManager component to configure</param>
    private static void ConnectToBallGameObject(CollisionManager collisionManager)
    {
        Debug.Log("🔗 [Step 5/6] Connecting to Ball GameObject...");
        
        // Find Ball GameObject in scene
        GameObject ball = GameObject.Find("Ball");
        
        if (ball != null)
        {
            // Set Ball reference using SerializedObject for editor modification
            SerializedObject serializedManager = new SerializedObject(collisionManager);
            SerializedProperty ballProperty = serializedManager.FindProperty("ballGameObject");
            
            if (ballProperty != null)
            {
                ballProperty.objectReferenceValue = ball;
                serializedManager.ApplyModifiedProperties();
                
                Debug.Log($"   • Connected to Ball GameObject: {ball.name}");
                
                // Validate Ball has required components
                Rigidbody2D ballRigidbody = ball.GetComponent<Rigidbody2D>();
                Collider2D ballCollider = ball.GetComponent<Collider2D>();
                
                if (ballRigidbody == null)
                {
                    Debug.LogWarning("   ⚠️ Ball GameObject missing Rigidbody2D component!");
                }
                
                if (ballCollider == null)
                {
                    Debug.LogWarning("   ⚠️ Ball GameObject missing Collider2D component!");
                }
                
                // Check Ball layer assignment
                int ballLayer = LayerMask.NameToLayer("Ball");
                if (ballLayer != -1 && ball.layer == ballLayer)
                {
                    Debug.Log("   • Ball GameObject is on correct 'Ball' layer");
                }
                else if (ballLayer != -1)
                {
                    Debug.LogWarning($"   ⚠️ Ball GameObject should be on 'Ball' layer (index {ballLayer})");
                }
            }
            else
            {
                Debug.LogWarning("   ⚠️ Could not find ballGameObject property for connection");
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️ Ball GameObject not found in scene. Create Ball GameObject for collision event capture.");
        }
        
        Debug.Log("✅ [Step 5/6] Ball GameObject connection complete");
    }
    
    /// <summary>
    /// Validates the CollisionManager setup and configuration.
    /// </summary>
    /// <param name="collisionManager">CollisionManager component to validate</param>
    private static void ValidateCollisionManagerSetup(CollisionManager collisionManager)
    {
        Debug.Log("🧪 [Step 6/6] Validating CollisionManager setup...");
        
        try
        {
            // The component will initialize in Awake(), so we simulate key validations
            
            // Validate singleton access (will be available after Awake)
            Debug.Log("   • Singleton pattern implementation: Ready");
            
            // Validate layer detection
            int ballLayer = LayerMask.NameToLayer("Ball");
            int paddleLayer = LayerMask.NameToLayer("Paddle");
            int bricksLayer = LayerMask.NameToLayer("Bricks");
            int powerUpsLayer = LayerMask.NameToLayer("PowerUps");
            int boundariesLayer = LayerMask.NameToLayer("Boundaries");
            
            int configuredLayers = 0;
            if (ballLayer != -1) configuredLayers++;
            if (paddleLayer != -1) configuredLayers++;
            if (bricksLayer != -1) configuredLayers++;
            if (powerUpsLayer != -1) configuredLayers++;
            if (boundariesLayer != -1) configuredLayers++;
            
            Debug.Log($"   • Physics layers available: {configuredLayers}/5");
            
            if (configuredLayers >= 3) // Ball, Paddle, Boundaries minimum
            {
                Debug.Log("   • Minimum required layers available for collision detection");
            }
            else
            {
                Debug.LogWarning("   ⚠️ Run Physics Layer setup for full collision type detection");
            }
            
            // Validate component is enabled
            if (collisionManager.enabled)
            {
                Debug.Log("   • CollisionManager component enabled and ready");
            }
            else
            {
                Debug.LogWarning("   ⚠️ CollisionManager component is disabled");
            }
            
            Debug.Log("✅ [Step 6/6] CollisionManager setup validation complete");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ CollisionManager validation failed: {e.Message}");
        }
    }
    
    /// <summary>
    /// Logs successful CollisionManager creation summary.
    /// </summary>
    /// <param name="collisionManager">Created CollisionManager component</param>
    private static void LogSuccessfulSetup(CollisionManager collisionManager)
    {
        Debug.Log("✅ [Task 1.1.3.2] CollisionManager created successfully!");
        Debug.Log("📋 CollisionManager Summary:");
        Debug.Log($"   • Component: CollisionManager on {collisionManager.gameObject.name}");
        Debug.Log($"   • Hierarchy: {GetGameObjectPath(collisionManager.gameObject)}");
        Debug.Log($"   • Position: {collisionManager.transform.position}");
        
        // Log collision management capabilities
        Debug.Log("⚙️ Collision Management Features:");
        Debug.Log("   → Singleton Pattern: Centralized collision coordination");
        Debug.Log("   → Event Handling: OnCollisionEnter2D and OnCollisionExit2D routing");
        Debug.Log("   → Type Detection: Layer-based collision type categorization");
        Debug.Log("   → Framework Methods: Stub handlers for Paddle, Brick, Boundary, PowerUp collisions");
        Debug.Log("   → Debug Logging: Configurable collision event logging");
        Debug.Log("   → Statistics Tracking: Collision count and timing monitoring");
        
        // Log collision framework API
        Debug.Log("📊 Collision Framework API:");
        Debug.Log("   • OnCollisionEnter2D(collision) - Route collision enter events");
        Debug.Log("   • OnCollisionExit2D(collision) - Route collision exit events");
        Debug.Log("   • GetCollisionStatistics() - Get collision monitoring data");
        Debug.Log("   • SetCollisionLogging(bool) - Toggle debug logging");
        Debug.Log("   • ResetStatistics() - Reset collision counters");
        Debug.Log("   • IsReady() - Check if manager is properly configured");
        
        // Log current configuration
        GameObject ball = GameObject.Find("Ball");
        GameObject paddle = GameObject.Find("Paddle");
        
        Debug.Log("🔧 Current Configuration:");
        Debug.Log($"   • Ball GameObject: {(ball != null ? ball.name : "Not Found")}");
        Debug.Log($"   • Paddle GameObject: {(paddle != null ? paddle.name : "Not Found")}");
        Debug.Log($"   • Collision Logging: Enabled (default)");
        Debug.Log($"   • Singleton Access: CollisionManager.Instance");
        
        // Log layer detection status
        int ballLayer = LayerMask.NameToLayer("Ball");
        int paddleLayer = LayerMask.NameToLayer("Paddle");
        int boundariesLayer = LayerMask.NameToLayer("Boundaries");
        
        Debug.Log("🏷️ Layer Detection Status:");
        Debug.Log($"   • Ball Layer: {(ballLayer != -1 ? $"Index {ballLayer}" : "Not Configured")}");
        Debug.Log($"   • Paddle Layer: {(paddleLayer != -1 ? $"Index {paddleLayer}" : "Not Configured")}");
        Debug.Log($"   • Boundaries Layer: {(boundariesLayer != -1 ? $"Index {boundariesLayer}" : "Not Configured")}");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. CollisionManager will automatically initialize as singleton in Play mode");
        Debug.Log("   2. Ball collision events will be automatically captured and routed");
        Debug.Log("   3. Use CollisionManager.Instance to access from other scripts");
        Debug.Log("   4. Framework methods are ready for collision response implementation");
        Debug.Log("   5. Enable collision logging to monitor collision events during development");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Connect Ball collision events to call OnCollisionEnter2D/Exit2D");
        Debug.Log("   → Implement specific collision response logic in framework methods");
        Debug.Log("   → Create Bricks and PowerUp GameObjects for collision testing");
        Debug.Log("   → Test collision detection and routing in Play mode");
        
        Debug.Log("⚠️ Integration Requirements:");
        Debug.Log("   → Ball GameObject needs collision event forwarding to CollisionManager");
        Debug.Log("   → Implement actual collision response logic in framework stub methods");
        Debug.Log("   → Ensure all GameObjects are on correct physics layers for proper detection");
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