using UnityEngine;

/// <summary>
/// Debug script to test and verify that PaddleData changes in the Inspector are reflected at runtime.
/// This addresses the issue where Inspector values were being overridden by Initialize() calls.
/// </summary>
public class PaddleInspectorTest : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private PaddleController paddleController;
    [SerializeField] private bool logValuesAtStart = true;
    [SerializeField] private bool logValuesEverySecond = false;
    
    private float lastLogTime;
    
    /// <summary>
    /// Test PaddleData Inspector persistence at Start
    /// </summary>
    private void Start()
    {
        if (logValuesAtStart)
        {
            TestInspectorValues();
        }
        
        lastLogTime = Time.time;
    }
    
    /// <summary>
    /// Optional periodic logging for runtime monitoring
    /// </summary>
    private void Update()
    {
        if (logValuesEverySecond && Time.time - lastLogTime >= 1f)
        {
            TestInspectorValues();
            lastLogTime = Time.time;
        }
    }
    
    /// <summary>
    /// Test and log current PaddleData values
    /// </summary>
    private void TestInspectorValues()
    {
        if (paddleController == null)
        {
            paddleController = FindObjectOfType<PaddleController>();
            if (paddleController == null)
            {
                Debug.LogWarning("[PaddleInspectorTest] No PaddleController found in scene");
                return;
            }
        }
        
        PaddleData paddleData = paddleController.GetPaddleData();
        if (paddleData == null)
        {
            Debug.LogError("[PaddleInspectorTest] PaddleController has no PaddleData!");
            return;
        }
        
        Debug.Log("=== PaddleData Inspector Values Test ===");
        Debug.Log($"Movement Speed: {paddleData.movementSpeed} (Inspector setting should be preserved)");
        Debug.Log($"Acceleration: {paddleData.acceleration} (Inspector setting should be preserved)");
        Debug.Log($"Paddle Dimensions: {paddleData.paddleDimensions} (Inspector setting should be preserved)");
        Debug.Log($"Input Sensitivity: {paddleData.inputSensitivity} (Inspector setting should be preserved)");
        Debug.Log($"Left Boundary: {paddleData.leftBoundary} (Inspector setting should be preserved)");
        Debug.Log($"Right Boundary: {paddleData.rightBoundary} (Inspector setting should be preserved)");
        Debug.Log($"Smooth Time: {paddleData.smoothTime} (Inspector setting should be preserved)");
        Debug.Log($"Target Response Time: {paddleData.targetResponseTime} (Inspector setting should be preserved)");
        Debug.Log($"Keyboard Input Enabled: {paddleData.enableKeyboardInput} (Inspector setting should be preserved)");
        Debug.Log($"Mouse Input Enabled: {paddleData.enableMouseInput} (Inspector setting should be preserved)");
        Debug.Log("=== End Test ===");
        
        // Additional controller state info
        if (paddleController.IsInitialized())
        {
            Debug.Log($"Controller Status: Initialized ✅");
            Debug.Log($"Current Position: {paddleController.GetCurrentPosition()}");
            Debug.Log($"Is Moving: {paddleController.IsMoving()}");
            Debug.Log($"Within Boundaries: {paddleController.IsWithinBoundaries()}");
        }
        else
        {
            Debug.LogWarning("Controller Status: Not Initialized ❌");
        }
    }
    
    /// <summary>
    /// Manual test method that can be called from other scripts or inspector
    /// </summary>
    [ContextMenu("Test Inspector Values Now")]
    public void TestInspectorValuesManual()
    {
        TestInspectorValues();
    }
    
    /// <summary>
    /// Test changing values at runtime to verify they persist
    /// </summary>
    [ContextMenu("Test Runtime Value Changes")]
    public void TestRuntimeValueChanges()
    {
        if (paddleController == null || paddleController.GetPaddleData() == null)
        {
            Debug.LogError("[PaddleInspectorTest] Cannot test - no paddle controller or data");
            return;
        }
        
        PaddleData paddleData = paddleController.GetPaddleData();
        
        Debug.Log("=== Testing Runtime Value Changes ===");
        Debug.Log($"Original Speed: {paddleData.movementSpeed}");
        
        // Change speed at runtime
        float originalSpeed = paddleData.movementSpeed;
        paddleData.movementSpeed = originalSpeed * 1.5f;
        
        Debug.Log($"Modified Speed: {paddleData.movementSpeed}");
        
        // Validate parameters to ensure change persists
        paddleData.ValidateExistingConfiguration();
        
        Debug.Log($"Speed After Validation: {paddleData.movementSpeed}");
        
        if (paddleData.movementSpeed == originalSpeed * 1.5f)
        {
            Debug.Log("✅ Runtime changes preserved correctly");
        }
        else
        {
            Debug.LogError("❌ Runtime changes were overridden by validation");
        }
        
        // Restore original value
        paddleData.movementSpeed = originalSpeed;
        Debug.Log($"Restored Speed: {paddleData.movementSpeed}");
    }
    
    /// <summary>
    /// Test paddle size changes through Inspector and runtime modifications
    /// </summary>
    [ContextMenu("Test Paddle Size Changes")]
    public void TestPaddleSizeChanges()
    {
        if (paddleController == null || paddleController.GetPaddleData() == null)
        {
            Debug.LogError("[PaddleInspectorTest] Cannot test - no paddle controller or data");
            return;
        }
        
        Debug.Log("=== Testing Paddle Size Changes ===");
        
        Vector2 originalSize = paddleController.GetPaddleDimensions();
        Debug.Log($"Original Paddle Size: {originalSize}");
        
        // Test making paddle wider
        Vector2 widerSize = new Vector2(originalSize.x * 1.5f, originalSize.y);
        Debug.Log($"Setting wider size: {widerSize}");
        paddleController.SetPaddleDimensions(widerSize);
        
        // Verify change applied
        Vector2 currentSize = paddleController.GetPaddleDimensions();
        if (Vector2.Distance(currentSize, widerSize) < 0.01f)
        {
            Debug.Log("✅ Paddle width increase applied successfully");
        }
        else
        {
            Debug.LogError($"❌ Paddle width change failed. Expected: {widerSize}, Got: {currentSize}");
        }
        
        // Wait a bit (in a real test, you'd use coroutines)
        System.Threading.Thread.Sleep(500);
        
        // Test making paddle taller
        Vector2 tallerSize = new Vector2(originalSize.x, originalSize.y * 2f);
        Debug.Log($"Setting taller size: {tallerSize}");
        paddleController.SetPaddleDimensions(tallerSize);
        
        // Test making paddle smaller
        Vector2 smallerSize = new Vector2(originalSize.x * 0.5f, originalSize.y * 0.5f);
        Debug.Log($"Setting smaller size: {smallerSize}");
        paddleController.SetPaddleDimensions(smallerSize);
        
        // Restore original size
        Debug.Log($"Restoring original size: {originalSize}");
        paddleController.SetPaddleDimensions(originalSize);
        
        // Final verification
        Vector2 finalSize = paddleController.GetPaddleDimensions();
        if (Vector2.Distance(finalSize, originalSize) < 0.01f)
        {
            Debug.Log("✅ Paddle size restore successful");
        }
        else
        {
            Debug.LogError($"❌ Paddle size restore failed. Expected: {originalSize}, Got: {finalSize}");
        }
        
        Debug.Log("=== Paddle Size Test Complete ===");
    }
}