#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for creating and configuring PaddleController component on existing Paddle GameObject.
/// Provides automated controller attachment with proper PaddleData configuration and component validation.
/// </summary>
public static class Task1123CreatePaddleControllerSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Paddle Controller";
    private const string PADDLE_CONTROLLER_FILE = "Assets/Scripts/Paddle/PaddleController.cs";
    private const string PADDLE_DATA_FILE = "Assets/Scripts/Paddle/PaddleData.cs";
    
    /// <summary>
    /// Creates and configures PaddleController component on existing Paddle GameObject.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreatePaddleController()
    {
        Debug.Log("📋 [Task 1.1.2.3] Starting PaddleController creation...");
        
        try
        {
            // Step 1: Validate prerequisites
            ValidatePrerequisites();
            
            // Step 2: Find and validate Paddle GameObject
            GameObject paddleGameObject = FindPaddleGameObject();
            
            // Step 3: Attach PaddleController component
            PaddleController paddleController = AttachPaddleController(paddleGameObject);
            
            // Step 4: Configure PaddleData reference
            ConfigurePaddleData();
            
            // Step 5: Validate component setup
            ValidateControllerSetup(paddleController);
            
            // Step 6: Final validation and cleanup
            EditorUtility.SetDirty(paddleGameObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            LogSuccessfulSetup(paddleController);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.2.3] PaddleController creation failed: {e.Message}");
            Debug.LogError("📋 Please check GameObject setup and component dependencies.");
        }
    }
    
    /// <summary>
    /// Menu validation - prevents duplicate creation when PaddleController already exists.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreatePaddleController()
    {
        GameObject paddle = GameObject.Find("Paddle");
        
        if (paddle == null)
        {
            Debug.LogWarning("⚠️ Paddle GameObject not found in scene.");
            return false;
        }
        
        bool controllerExists = paddle.GetComponent<PaddleController>() != null;
        
        if (controllerExists)
        {
            Debug.LogWarning("⚠️ PaddleController component already exists on Paddle GameObject.");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Validates that prerequisite systems are in place before creating PaddleController.
    /// </summary>
    private static void ValidatePrerequisites()
    {
        Debug.Log("🔍 [Step 1/5] Validating prerequisites...");
        
        // Check if PaddleController script exists
        if (!File.Exists(PADDLE_CONTROLLER_FILE))
        {
            Debug.LogError("❌ PaddleController.cs script not found!");
            Debug.LogError($"📋 Expected location: {PADDLE_CONTROLLER_FILE}");
            throw new FileNotFoundException("PaddleController script is missing");
        }
        
        // Check if PaddleData exists
        if (!File.Exists(PADDLE_DATA_FILE))
        {
            Debug.LogWarning("⚠️ PaddleData structure missing. Creating prerequisite...");
            Task1121CreatePaddleDataSetup.CreatePaddleDataStructure();
        }
        
        // Check if Paddle GameObject exists
        GameObject paddle = GameObject.Find("Paddle");
        if (paddle == null)
        {
            Debug.LogWarning("⚠️ Paddle GameObject missing. Creating prerequisite...");
            Task1122CreatePaddleGameObjectSetup.CreatePaddleGameObject();
        }
        
        Debug.Log("✅ [Step 1/5] Prerequisites validated successfully");
    }
    
    /// <summary>
    /// Finds and validates the Paddle GameObject in the scene.
    /// </summary>
    /// <returns>Paddle GameObject</returns>
    private static GameObject FindPaddleGameObject()
    {
        Debug.Log("🔍 [Step 2/5] Finding Paddle GameObject...");
        
        GameObject paddleGameObject = GameObject.Find("Paddle");
        
        if (paddleGameObject == null)
        {
            Debug.LogError("❌ Paddle GameObject not found in scene!");
            throw new System.NullReferenceException("Paddle GameObject is required but not found");
        }
        
        // Validate required components
        BoxCollider2D collider = paddleGameObject.GetComponent<BoxCollider2D>();
        SpriteRenderer renderer = paddleGameObject.GetComponent<SpriteRenderer>();
        
        if (collider == null)
        {
            Debug.LogWarning("⚠️ BoxCollider2D component missing from Paddle GameObject");
        }
        
        if (renderer == null)
        {
            Debug.LogWarning("⚠️ SpriteRenderer component missing from Paddle GameObject");
        }
        
        Debug.Log($"✅ [Step 2/5] Paddle GameObject found: {paddleGameObject.name}");
        Debug.Log($"   • Position: {paddleGameObject.transform.position}");
        Debug.Log($"   • Components: BoxCollider2D={collider != null}, SpriteRenderer={renderer != null}");
        
        return paddleGameObject;
    }
    
    /// <summary>
    /// Attaches PaddleController component to the Paddle GameObject.
    /// </summary>
    /// <param name="paddleGameObject">Paddle GameObject to attach controller to</param>
    /// <returns>Attached PaddleController component</returns>
    private static PaddleController AttachPaddleController(GameObject paddleGameObject)
    {
        Debug.Log("⚙️ [Step 3/5] Attaching PaddleController component...");
        
        // Check if controller already exists
        if (paddleGameObject.TryGetComponent<PaddleController>(out var existingController))
        {
            Debug.LogWarning("⚠️ PaddleController already exists. Using existing component.");
            return existingController;
        }
        
        // Add PaddleController component
        var paddleController = paddleGameObject.AddComponent<PaddleController>();
        
        if (paddleController == null)
        {
            Debug.LogError("❌ Failed to attach PaddleController component!");
            throw new System.NullReferenceException("PaddleController component attachment failed");
        }
        
        Debug.Log("✅ [Step 3/5] PaddleController component attached successfully");
        
        return paddleController;
    }
    
    /// <summary>
    /// Configures PaddleData reference for the PaddleController.
    /// </summary>
    private static void ConfigurePaddleData()
    {
        Debug.Log("🔧 [Step 4/5] Configuring PaddleData reference...");
        
        // The PaddleController will automatically create a default PaddleData instance
        // during its validation process if none is assigned in the Inspector.
        // We don't need to manually configure it here since the controller handles this.
        
        Debug.Log("   • PaddleData will be created automatically by controller validation");
        Debug.Log("   • Controller handles default PaddleData creation if needed");
        
        Debug.Log("✅ [Step 4/5] PaddleData configuration complete");
    }
    
    /// <summary>
    /// Validates the PaddleController component setup and configuration.
    /// </summary>
    /// <param name="paddleController">PaddleController to validate</param>
    private static void ValidateControllerSetup(PaddleController paddleController)
    {
        Debug.Log("🧪 [Step 5/5] Validating controller setup...");
        
        try
        {
            // Test controller initialization (this will happen in Awake)
            // Force component validation by accessing IsInitialized after a frame
            EditorApplication.delayCall += () => {
                if (paddleController != null)
                {
                    bool isInitialized = paddleController.IsInitialized();
                    Debug.Log($"   • Controller Initialized: {isInitialized}");
                    
                    if (isInitialized)
                    {
                        // Test basic functionality
                        Vector3 currentPos = paddleController.GetCurrentPosition();
                        Debug.Log($"   • Current Position: {currentPos}");
                        
                        bool withinBoundaries = paddleController.IsWithinBoundaries();
                        Debug.Log($"   • Within Boundaries: {withinBoundaries}");
                        
                        PaddleData paddleData = paddleController.GetPaddleData();
                        Debug.Log($"   • PaddleData Valid: {paddleData != null}");
                    }
                }
            };
            
            // Validate component references
            BoxCollider2D collider = paddleController.GetPaddleCollider();
            SpriteRenderer renderer = paddleController.GetPaddleRenderer();
            
            Debug.Log($"   • Component References: Collider={collider != null}, Renderer={renderer != null}");
            
            Debug.Log("✅ [Step 5/5] Controller setup validation complete");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Controller validation failed: {e.Message}");
        }
    }
    
    /// <summary>
    /// Logs successful PaddleController setup summary.
    /// </summary>
    /// <param name="paddleController">Created PaddleController component</param>
    private static void LogSuccessfulSetup(PaddleController paddleController)
    {
        Debug.Log("✅ [Task 1.1.2.3] PaddleController created successfully!");
        Debug.Log("📋 PaddleController Summary:");
        Debug.Log($"   • Component: PaddleController attached to {paddleController.gameObject.name}");
        Debug.Log($"   • Position: {paddleController.transform.position}");
        Debug.Log($"   • Parent: {(paddleController.transform.parent != null ? paddleController.transform.parent.name : "None")}");
        Debug.Log("🔧 Component Configuration:");
        
        // Log component references
        BoxCollider2D collider = paddleController.GetPaddleCollider();  
        SpriteRenderer renderer = paddleController.GetPaddleRenderer();
        
        Debug.Log($"   → BoxCollider2D: {(collider != null ? "Present" : "Missing")}");
        if (collider != null)
        {
            Debug.Log($"     • Size: {collider.size}");
            Debug.Log($"     • Physics Material: {(collider.sharedMaterial != null ? collider.sharedMaterial.name : "None")}");
        }
        
        Debug.Log($"   → SpriteRenderer: {(renderer != null ? "Present" : "Missing")}");
        if (renderer != null)
        {
            Debug.Log($"     • Color: {renderer.color}");
            Debug.Log($"     • Sorting Order: {renderer.sortingOrder}");
        }
        
        Debug.Log("📊 Movement API:");
        Debug.Log("   • SetPosition(float x) - Set paddle to specific X position");
        Debug.Log("   • MoveTowards(float targetX) - Smooth movement to target");
        Debug.Log("   • GetCurrentPosition() - Get current world position");
        Debug.Log("   • Stop() - Stop all movement immediately");
        Debug.Log("   • IsMoving() - Check if paddle is currently moving");
        Debug.Log("   • IsWithinBoundaries() - Validate position constraints");
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. PaddleController automatically initializes with PaddleData configuration");
        Debug.Log("   2. Use movement API methods for position control");
        Debug.Log("   3. Controller validates component references and handles missing dependencies");
        Debug.Log("   4. Position updates respect boundary constraints from PaddleData");
        Debug.Log("   5. Call GetStatusInfo() for comprehensive debug information");
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Implement input handling system for player control");
        Debug.Log("   → Add boundary constraint integration with GameArea");
        Debug.Log("   → Integrate with ball physics for collision response");
        Debug.Log("   → Add visual feedback and animation systems");
    }
}
#endif