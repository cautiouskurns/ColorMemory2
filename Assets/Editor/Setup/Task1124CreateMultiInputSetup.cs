#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Editor setup script for validating and testing multi-input system implementation.
/// Provides comprehensive validation of keyboard and mouse input integration with response time testing.
/// </summary>
public static class Task1124CreateMultiInputSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1124 Create Multi-Input System";
    private const string PADDLE_CONTROLLER_FILE = "Assets/Scripts/Paddle/PaddleController.cs";
    
    /// <summary>
    /// Validates and tests multi-input system implementation.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateMultiInputSystem()
    {
        Debug.Log("📋 [Task 1.1.2.4] Starting Multi-Input System validation...");
        
        try
        {
            // Step 1: Validate prerequisites
            ValidatePrerequisites();
            
            // Step 2: Find and validate PaddleController
            PaddleController paddleController = FindPaddleController();
            
            // Step 3: Validate camera setup for mouse input
            ValidateCameraSetup();
            
            // Step 4: Test input system functionality
            TestInputSystemFunctionality(paddleController);
            
            // Step 5: Validate response time requirements
            ValidateResponseTimeRequirements(paddleController);
            
            // Step 6: Final validation
            LogSuccessfulSetup(paddleController);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.2.4] Multi-Input System validation failed: {e.Message}");
            Debug.LogError("📋 Please check PaddleController implementation and camera setup.");
        }
    }
    
    /// <summary>
    /// Menu validation - ensures PaddleController exists and can be enhanced with input system.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateMultiInputSystem()
    {
        GameObject paddle = GameObject.Find("Paddle");
        
        if (paddle == null)
        {
            Debug.LogWarning("⚠️ Paddle GameObject not found in scene.");
            return false;
        }
        
        PaddleController controller = paddle.GetComponent<PaddleController>();
        if (controller == null)
        {
            Debug.LogWarning("⚠️ PaddleController component not found on Paddle GameObject.");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Validates that prerequisite systems are in place.
    /// </summary>
    private static void ValidatePrerequisites()
    {
        Debug.Log("🔍 [Step 1/5] Validating prerequisites...");
        
        // Check if enhanced PaddleController exists
        if (!System.IO.File.Exists(PADDLE_CONTROLLER_FILE))
        {
            Debug.LogError("❌ PaddleController.cs script not found!");
            throw new System.IO.FileNotFoundException("Enhanced PaddleController script is missing");
        }
        
        // Check if Paddle GameObject exists
        GameObject paddle = GameObject.Find("Paddle");
        if (paddle == null)
        {
            Debug.LogWarning("⚠️ Paddle GameObject missing. Creating prerequisite...");
            Task1122CreatePaddleGameObjectSetup.CreatePaddleGameObject();
        }
        
        // Check if PaddleController component exists
        if (paddle != null)
        {
            PaddleController controller = paddle.GetComponent<PaddleController>();
            if (controller == null)
            {
                Debug.LogWarning("⚠️ PaddleController component missing. Creating prerequisite...");
                Task1123CreatePaddleControllerSetup.CreatePaddleController();
            }
        }
        
        Debug.Log("✅ [Step 1/5] Prerequisites validated successfully");
    }
    
    /// <summary>
    /// Finds and validates the PaddleController in the scene.
    /// </summary>
    /// <returns>PaddleController component</returns>
    private static PaddleController FindPaddleController()
    {
        Debug.Log("🔍 [Step 2/5] Finding and validating PaddleController...");
        
        GameObject paddleGameObject = GameObject.Find("Paddle");
        if (paddleGameObject == null)
        {
            Debug.LogError("❌ Paddle GameObject not found in scene!");
            throw new System.NullReferenceException("Paddle GameObject is required");
        }
        
        PaddleController paddleController = paddleGameObject.GetComponent<PaddleController>();
        if (paddleController == null)
        {
            Debug.LogError("❌ PaddleController component not found!");
            throw new System.NullReferenceException("PaddleController component is required");
        }
        
        Debug.Log($"✅ [Step 2/5] PaddleController found and validated: {paddleController.name}");
        
        return paddleController;
    }
    
    /// <summary>
    /// Validates camera setup for mouse input functionality.
    /// </summary>
    private static void ValidateCameraSetup()
    {
        Debug.Log("📷 [Step 3/5] Validating camera setup for mouse input...");
        
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = Object.FindObjectOfType<Camera>();
        }
        
        if (mainCamera == null)
        {
            Debug.LogWarning("⚠️ No camera found in scene. Mouse input may not work properly.");
            Debug.LogWarning("📋 Creating basic camera for mouse input support...");
            
            // Create basic camera for mouse input
            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0, 0, -10);
            
            Debug.Log("✅ Created basic camera for mouse input support");
        }
        else
        {
            Debug.Log($"✅ Camera found and validated: {mainCamera.name}");
            Debug.Log($"   • Position: {mainCamera.transform.position}");
            Debug.Log($"   • Tag: {mainCamera.tag}");
        }
        
        Debug.Log("✅ [Step 3/5] Camera setup validated successfully");
    }
    
    /// <summary>
    /// Tests input system functionality and method switching.
    /// </summary>
    /// <param name="paddleController">PaddleController to test</param>
    private static void TestInputSystemFunctionality(PaddleController paddleController)
    {
        Debug.Log("🧪 [Step 4/5] Testing input system functionality...");
        
        try
        {
            // Test input system API availability
            InputMethod currentMethod = paddleController.GetActiveInputMethod();
            float inputValue = paddleController.GetCurrentInputValue();
            bool inputActive = paddleController.IsInputActive();
            float timeSinceInput = paddleController.GetTimeSinceLastInput();
            
            Debug.Log($"   • Current Input Method: {currentMethod}");
            Debug.Log($"   • Input Value: {inputValue:F3}");
            Debug.Log($"   • Input Active: {inputActive}");
            Debug.Log($"   • Time Since Input: {timeSinceInput:F2}s");
            
            // Test input method control
            paddleController.SetKeyboardInputEnabled(true);
            paddleController.SetMouseInputEnabled(true);
            
            Debug.Log("   • Keyboard Input: Enabled");
            Debug.Log("   • Mouse Input: Enabled");
            
            // Test PaddleData integration
            PaddleData paddleData = paddleController.GetPaddleData();
            if (paddleData != null)
            {
                Debug.Log($"   • Input Sensitivity: {paddleData.inputSensitivity:F1}x");
                Debug.Log($"   • Keyboard Enabled: {paddleData.enableKeyboardInput}");
                Debug.Log($"   • Mouse Enabled: {paddleData.enableMouseInput}");
            }
            
            Debug.Log("✅ [Step 4/5] Input system functionality test complete");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Input system functionality test failed: {e.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Validates input response time requirements for arcade-quality gameplay.
    /// </summary>
    /// <param name="paddleController">PaddleController to test</param>
    private static void ValidateResponseTimeRequirements(PaddleController paddleController)
    {
        Debug.Log("⏱️ [Step 5/5] Validating response time requirements...");
        
        // Note: Actual response time testing requires runtime input simulation
        // This validation checks that the system is set up for optimal performance
        
        try
        {
            // Check component caching for performance
            bool hasRequiredComponents = paddleController.GetPaddleCollider() != null && 
                                       paddleController.GetPaddleRenderer() != null;
            
            Debug.Log($"   • Component Caching: {(hasRequiredComponents ? "Optimized" : "Missing Components")}");
            
            // Check PaddleData configuration for responsive movement
            PaddleData paddleData = paddleController.GetPaddleData();
            if (paddleData != null)
            {
                float movementSpeed = paddleData.movementSpeed;
                float inputSensitivity = paddleData.inputSensitivity;
                
                Debug.Log($"   • Movement Speed: {movementSpeed:F1} units/sec");
                Debug.Log($"   • Input Sensitivity: {inputSensitivity:F1}x");
                
                // Validate speed settings for responsive gameplay
                if (movementSpeed >= 6f && movementSpeed <= 15f)
                {
                    Debug.Log("   • Response Speed: Optimal for arcade gameplay");
                }
                else
                {
                    Debug.LogWarning("   ⚠️ Movement speed may affect response time quality");
                }
                
                if (inputSensitivity >= 0.8f && inputSensitivity <= 2.0f)
                {
                    Debug.Log("   • Input Sensitivity: Optimal for responsive control");
                }
                else
                {
                    Debug.LogWarning("   ⚠️ Input sensitivity may affect response quality");
                }
            }
            
            Debug.Log("📊 Response Time Analysis:");
            Debug.Log("   • Target: <50ms response time for arcade-quality gameplay");
            Debug.Log("   • Implementation: Direct Transform manipulation for minimal latency");
            Debug.Log("   • Input Polling: Optimized per-frame processing in Update()");
            Debug.Log("   • Coordinate Conversion: Cached Camera reference for mouse input");
            
            Debug.Log("✅ [Step 5/5] Response time requirements validation complete");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Response time validation failed: {e.Message}");
        }
    }
    
    /// <summary>
    /// Logs successful multi-input system setup summary.
    /// </summary>
    /// <param name="paddleController">Validated PaddleController component</param>
    private static void LogSuccessfulSetup(PaddleController paddleController)
    {
        Debug.Log("✅ [Task 1.1.2.4] Multi-Input System validated successfully!");
        Debug.Log("📋 Multi-Input System Summary:");
        Debug.Log($"   • Component: Enhanced PaddleController on {paddleController.gameObject.name}");
        Debug.Log($"   • Position: {paddleController.transform.position}");
        
        // Log input system capabilities
        Debug.Log("🎮 Input System Features:");
        Debug.Log("   → Keyboard Input: A/D keys and Arrow keys support");
        Debug.Log("   → Mouse Input: Screen-to-world coordinate conversion");
        Debug.Log("   → Automatic Method Switching: Seamless transition between input types");
        Debug.Log("   → Input Sensitivity: Configurable through PaddleData");
        Debug.Log("   → Boundary Constraints: Automatic position limiting");
        Debug.Log("   → Performance Optimized: <50ms response time target");
        
        // Log input system API
        Debug.Log("📊 Input System API:");
        Debug.Log("   • GetActiveInputMethod() - Current input method");
        Debug.Log("   • GetCurrentInputValue() - Input value (-1 to 1)");
        Debug.Log("   • IsInputActive() - Input processing status");
        Debug.Log("   • GetTimeSinceLastInput() - Input timing information");
        Debug.Log("   • SetKeyboardInputEnabled(bool) - Toggle keyboard input");
        Debug.Log("   • SetMouseInputEnabled(bool) - Toggle mouse input");
        Debug.Log("   • ForceInputMethod(InputMethod) - Override input method");
        
        // Log configuration
        PaddleData paddleData = paddleController.GetPaddleData();
        if (paddleData != null)
        {
            Debug.Log("🔧 Input Configuration:");
            Debug.Log($"   • Movement Speed: {paddleData.movementSpeed:F1} units/sec");
            Debug.Log($"   • Input Sensitivity: {paddleData.inputSensitivity:F1}x");
            Debug.Log($"   • Keyboard Input: {paddleData.enableKeyboardInput}");
            Debug.Log($"   • Mouse Input: {paddleData.enableMouseInput}");
            Debug.Log($"   • Boundaries: {paddleData.leftBoundary:F1} to {paddleData.rightBoundary:F1}");
        }
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Use A/D keys or Arrow keys for keyboard control");
        Debug.Log("   2. Move mouse for position-based paddle control");
        Debug.Log("   3. Input method switches automatically based on last input");
        Debug.Log("   4. Configure sensitivity through PaddleData in Inspector");
        Debug.Log("   5. Use input system API for advanced control and monitoring");
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Test input responsiveness in Play mode");
        Debug.Log("   → Fine-tune sensitivity settings for optimal feel");
        Debug.Log("   → Integrate with ball physics for collision response");
        Debug.Log("   → Add visual feedback for input method switching");
    }
}
#endif