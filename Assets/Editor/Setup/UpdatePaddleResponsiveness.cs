#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Utility script to update paddle responsiveness after smoothing implementation.
/// Fixes slow movement by adjusting smoothing parameters for better responsiveness.
/// </summary>
public static class UpdatePaddleResponsiveness
{
    private const string MENU_PATH = "Breakout/Setup/Fix Paddle Responsiveness";
    
    /// <summary>
    /// Updates paddle settings for more responsive movement.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void FixPaddleResponsiveness()
    {
        Debug.Log("🔧 [Paddle Fix] Updating paddle responsiveness settings...");
        
        try
        {
            // Find paddle in scene
            GameObject paddle = GameObject.Find("Paddle");
            if (paddle == null)
            {
                Debug.LogWarning("⚠️ No Paddle GameObject found in scene.");
                return;
            }
            
            PaddleController controller = paddle.GetComponent<PaddleController>();
            if (controller == null)
            {
                Debug.LogWarning("⚠️ No PaddleController found on Paddle GameObject.");
                return;
            }
            
            PaddleData paddleData = controller.GetPaddleData();
            if (paddleData == null)
            {
                Debug.LogWarning("⚠️ No PaddleData found on PaddleController.");
                return;
            }
            
            Debug.Log("📊 Current Settings:");
            Debug.Log($"   • Movement Speed: {paddleData.movementSpeed:F1}");
            Debug.Log($"   • Smooth Time: {paddleData.smoothTime:F3}s");
            Debug.Log($"   • Input Smooth Time: {paddleData.inputSmoothTime:F3}s");
            Debug.Log($"   • Smooth Input Enabled: {paddleData.enableSmoothInput}");
            
            // Apply responsive settings
            paddleData.movementSpeed = 15.0f;        // Increase speed
            paddleData.smoothTime = 0.03f;           // Very fast smoothing
            paddleData.inputSmoothTime = 0.01f;      // Nearly instant input response
            paddleData.enableSmoothInput = false;    // Disable input smoothing for max responsiveness
            
            // Update acceleration curve for immediate response
            paddleData.accelerationCurve = AnimationCurve.EaseInOut(0f, 0.95f, 1f, 1f);
            
            // Validate parameters
            paddleData.ValidateParameters();
            
            // Force controller to reload settings
            controller.RevalidateSetup();
            
            Debug.Log("✅ [Paddle Fix] Updated Settings:");
            Debug.Log($"   • Movement Speed: {paddleData.movementSpeed:F1} (increased for responsiveness)");
            Debug.Log($"   • Smooth Time: {paddleData.smoothTime:F3}s (reduced for faster response)");
            Debug.Log($"   • Input Smooth Time: {paddleData.inputSmoothTime:F3}s (minimal delay)");
            Debug.Log($"   • Smooth Input Enabled: {paddleData.enableSmoothInput} (disabled for direct response)");
            Debug.Log($"   • Acceleration Curve: Starts at {paddleData.accelerationCurve.Evaluate(0f):F2} for immediate response");
            
            // Mark scene as dirty
            EditorUtility.SetDirty(paddle);
            
            Debug.Log("🚀 Paddle responsiveness has been optimized!");
            Debug.Log("💡 The paddle should now feel much more responsive while maintaining smooth movement for MoveTowards() calls.");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to update paddle responsiveness: {e.Message}");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if paddle exists in scene.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateFixPaddleResponsiveness()
    {
        GameObject paddle = GameObject.Find("Paddle");
        return paddle != null && paddle.GetComponent<PaddleController>() != null;
    }
}
#endif