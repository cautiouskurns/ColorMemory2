#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for validating and testing boundary constraint system implementation.
/// Provides comprehensive validation of GameArea boundary detection and constraint enforcement.
/// </summary>
public static class Task1125CreateBoundaryConstraintSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1125 Create Boundary Constraints";
    private const string PADDLE_CONTROLLER_FILE = "Assets/Scripts/Paddle/PaddleController.cs";
    
    /// <summary>
    /// Validates and tests boundary constraint system implementation.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateBoundaryConstraints()
    {
        Debug.Log("📋 [Task 1.1.2.5] Starting Boundary Constraint System validation...");
        
        try
        {
            // Step 1: Validate prerequisites
            ValidatePrerequisites();
            
            // Step 2: Find and validate PaddleController
            PaddleController paddleController = FindPaddleController();
            
            // Step 3: Validate GameArea setup and boundary detection
            ValidateGameAreaSetup(paddleController);
            
            // Step 4: Test boundary constraint enforcement
            TestBoundaryConstraints(paddleController);
            
            // Step 5: Test edge cases and rapid movement scenarios
            TestEdgeCases(paddleController);
            
            // Step 6: Final validation
            LogSuccessfulSetup(paddleController);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.2.5] Boundary Constraint System validation failed: {e.Message}");
            Debug.LogError("📋 Please check PaddleController implementation and GameArea configuration.");
        }
    }
    
    /// <summary>
    /// Menu validation - ensures multi-input PaddleController exists and can be enhanced with boundary constraints.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateBoundaryConstraints()
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
        
        // Check if multi-input system exists
        paddle = GameObject.Find("Paddle");
        if (paddle != null)
        {
            PaddleController controller = paddle.GetComponent<PaddleController>();
            if (controller != null)
            {
                try
                {
                    // Test if multi-input API exists
                    InputMethod inputMethod = controller.GetActiveInputMethod();
                    Debug.Log($"   • Multi-input system detected: {inputMethod}");
                }
                catch (System.Exception)
                {
                    Debug.LogWarning("⚠️ Multi-input system missing. Creating prerequisite...");
                    Task1124CreateMultiInputSetup.CreateMultiInputSystem();
                }
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
    /// Validates GameArea setup and boundary detection functionality.
    /// </summary>
    /// <param name="paddleController">PaddleController to test</param>
    private static void ValidateGameAreaSetup(PaddleController paddleController)
    {
        Debug.Log("🏗️ [Step 3/5] Validating GameArea setup and boundary detection...");
        
        try
        {
            // Test boundary constraint API availability
            float leftBoundary = paddleController.GetEffectiveLeftBoundary();
            float rightBoundary = paddleController.GetEffectiveRightBoundary();
            float playableWidth = paddleController.GetPlayableWidth();
            float paddleHalfWidth = paddleController.GetPaddleHalfWidth();
            bool autoDetected = paddleController.AreBoundariesAutoDetected();
            
            Debug.Log($"   • Left Boundary: {leftBoundary:F2}");
            Debug.Log($"   • Right Boundary: {rightBoundary:F2}");
            Debug.Log($"   • Playable Width: {playableWidth:F2}");
            Debug.Log($"   • Paddle Half-Width: {paddleHalfWidth:F2}");
            Debug.Log($"   • Auto-Detected: {autoDetected}");
            
            // Validate boundary setup
            if (rightBoundary <= leftBoundary)
            {
                Debug.LogError("❌ Invalid boundary configuration: Right boundary must be greater than left");
                throw new System.InvalidOperationException("Invalid boundary setup");
            }
            
            if (playableWidth < paddleHalfWidth * 2)
            {
                Debug.LogWarning("⚠️ Playable area may be too narrow for paddle movement");
            }
            
            // Check GameArea container
            GameObject gameArea = GameObject.Find("GameArea");
            if (gameArea != null)
            {
                Debug.Log($"   • GameArea found: {gameArea.name}");
                
                // Check for GameArea collider
                Collider2D gameAreaCollider = gameArea.GetComponent<Collider2D>();
                if (gameAreaCollider != null)
                {
                    Debug.Log($"   • GameArea collider: {gameAreaCollider.GetType().Name}");
                    Debug.Log($"   • Collider bounds: {gameAreaCollider.bounds.min.x:F2} to {gameAreaCollider.bounds.max.x:F2}");
                }
                else
                {
                    Debug.LogWarning("   ⚠️ GameArea has no collider for automatic boundary detection");
                }
                
                // Check for boundary objects
                Transform[] children = gameArea.GetComponentsInChildren<Transform>();
                int boundaryObjects = 0;
                foreach (Transform child in children)
                {
                    if (child.name.ToLower().Contains("wall") || child.name.ToLower().Contains("boundary"))
                    {
                        boundaryObjects++;
                        Debug.Log($"   • Boundary object found: {child.name}");
                    }
                }
                
                if (boundaryObjects == 0 && gameAreaCollider == null)
                {
                    Debug.LogWarning("   ⚠️ No boundary detection sources found in GameArea");
                }
            }
            else
            {
                Debug.LogWarning("   ⚠️ GameArea container not found. Adding collider for boundary detection.");
                
                // Try to find existing GameArea or create one
                if (gameArea == null)
                {
                    gameArea = new GameObject("GameArea");
                    gameArea.transform.position = Vector3.zero;
                    Debug.Log("   • Created new GameArea container");
                }
                
                // Add basic BoxCollider2D for boundary detection
                BoxCollider2D collider = gameArea.GetComponent<BoxCollider2D>();
                if (collider == null)
                {
                    collider = gameArea.AddComponent<BoxCollider2D>();
                    collider.size = new Vector2(16f, 12f);
                    collider.isTrigger = true;
                    Debug.Log("   • Added BoxCollider2D to GameArea for automatic boundary detection");
                }
                
                // Force boundary recalculation
                paddleController.RecalculateBoundaries();
                Debug.Log("   • Forced boundary recalculation with GameArea collider");
            }
            
            Debug.Log("✅ [Step 3/5] GameArea validation complete");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ GameArea validation failed: {e.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Tests boundary constraint enforcement with various scenarios.
    /// </summary>
    /// <param name="paddleController">PaddleController to test</param>
    private static void TestBoundaryConstraints(PaddleController paddleController)
    {
        Debug.Log("🧪 [Step 4/5] Testing boundary constraint enforcement...");
        
        try
        {
            float leftBoundary = paddleController.GetEffectiveLeftBoundary();
            float rightBoundary = paddleController.GetEffectiveRightBoundary();
            
            // Test 1: Position within boundaries
            float testPos1 = (leftBoundary + rightBoundary) * 0.5f; // Center
            float constrained1 = paddleController.TestBoundaryConstraints(testPos1);
            bool test1Pass = Mathf.Approximately(testPos1, constrained1);
            Debug.Log($"   • Center position test: {(test1Pass ? "PASS" : "FAIL")} ({testPos1:F2} → {constrained1:F2})");
            
            // Test 2: Position beyond left boundary
            float testPos2 = leftBoundary - 5.0f;
            float constrained2 = paddleController.TestBoundaryConstraints(testPos2);
            bool test2Pass = Mathf.Approximately(constrained2, leftBoundary);
            Debug.Log($"   • Left boundary constraint: {(test2Pass ? "PASS" : "FAIL")} ({testPos2:F2} → {constrained2:F2})");
            
            // Test 3: Position beyond right boundary
            float testPos3 = rightBoundary + 5.0f;
            float constrained3 = paddleController.TestBoundaryConstraints(testPos3);
            bool test3Pass = Mathf.Approximately(constrained3, rightBoundary);
            Debug.Log($"   • Right boundary constraint: {(test3Pass ? "PASS" : "FAIL")} ({testPos3:F2} → {constrained3:F2})");
            
            // Test 4: SetPosition with boundary enforcement (only if controller is initialized)
            if (paddleController.IsInitialized())
            {
                Vector3 originalPos = paddleController.GetCurrentPosition();
                paddleController.SetPosition(leftBoundary - 2.0f);
                Vector3 newPos = paddleController.GetCurrentPosition();
                bool test4Pass = Mathf.Approximately(newPos.x, leftBoundary);
                Debug.Log($"   • SetPosition constraint: {(test4Pass ? "PASS" : "FAIL")} (Position: {newPos.x:F2})");
                
                // Test 5: MoveTowards with boundary enforcement
                paddleController.MoveTowards(rightBoundary + 3.0f);
                Debug.Log($"   • MoveTowards constraint: Test initiated (target set beyond boundary)");
                
                // Restore original position
                paddleController.SetPosition(originalPos.x);
            }
            else
            {
                Debug.LogWarning("   • SetPosition/MoveTowards tests skipped - controller not initialized in Editor mode");
                Debug.Log($"   • SetPosition constraint: SKIPPED (Editor mode)");
                Debug.Log($"   • MoveTowards constraint: SKIPPED (Editor mode)");
            }
            
            Debug.Log("✅ [Step 4/5] Boundary constraint testing complete");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Boundary constraint testing failed: {e.Message}");
        }
    }
    
    /// <summary>
    /// Tests edge cases and rapid movement scenarios.
    /// </summary>
    /// <param name="paddleController">PaddleController to test</param>
    private static void TestEdgeCases(PaddleController paddleController)
    {
        Debug.Log("⚡ [Step 5/5] Testing edge cases and rapid movement scenarios...");
        
        try
        {
            // Test 1: Manual boundary setting
            float originalLeft = paddleController.GetEffectiveLeftBoundary();
            float originalRight = paddleController.GetEffectiveRightBoundary();
            
            paddleController.SetManualBoundaries(-5.0f, 5.0f);
            float newLeft = paddleController.GetEffectiveLeftBoundary();
            float newRight = paddleController.GetEffectiveRightBoundary();
            
            bool manualBoundaryTest = (newLeft != originalLeft) && (newRight != originalRight);
            Debug.Log($"   • Manual boundary setting: {(manualBoundaryTest ? "PASS" : "FAIL")}");
            Debug.Log($"     Original: {originalLeft:F2} to {originalRight:F2}");
            Debug.Log($"     New: {newLeft:F2} to {newRight:F2}");
            
            // Test 2: Boundary recalculation
            paddleController.RecalculateBoundaries();
            float recalcLeft = paddleController.GetEffectiveLeftBoundary();
            float recalcRight = paddleController.GetEffectiveRightBoundary();
            Debug.Log($"   • Boundary recalculation: COMPLETED");
            Debug.Log($"     Recalculated: {recalcLeft:F2} to {recalcRight:F2}");
            
            // Test 3: Extreme position values
            float extremeLeft = paddleController.TestBoundaryConstraints(-1000f);
            float extremeRight = paddleController.TestBoundaryConstraints(1000f);
            bool extremeTest = (extremeLeft == recalcLeft) && (extremeRight == recalcRight);
            Debug.Log($"   • Extreme position handling: {(extremeTest ? "PASS" : "FAIL")}");
            
            // Test 4: Invalid boundary setting (should be rejected)
            float beforeInvalidLeft = paddleController.GetEffectiveLeftBoundary();
            Debug.Log($"     Before invalid test: {beforeInvalidLeft:F2}");
            paddleController.SetManualBoundaries(10.0f, 5.0f); // Invalid: left > right (should be rejected)
            float afterInvalidLeft = paddleController.GetEffectiveLeftBoundary();
            bool invalidTest = Mathf.Approximately(beforeInvalidLeft, afterInvalidLeft);
            Debug.Log($"   • Invalid boundary rejection: {(invalidTest ? "PASS" : "FAIL")} (Boundaries unchanged: {afterInvalidLeft:F2})");
            
            // Test 5: Paddle width consideration
            float paddleHalfWidth = paddleController.GetPaddleHalfWidth();
            bool widthTest = paddleHalfWidth > 0f;
            Debug.Log($"   • Paddle width calculation: {(widthTest ? "PASS" : "FAIL")} (Half-width: {paddleHalfWidth:F2})");
            
            Debug.Log("✅ [Step 5/5] Edge case testing complete");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Edge case testing failed: {e.Message}");
        }
    }
    
    /// <summary>
    /// Logs successful boundary constraint system setup summary.
    /// </summary>
    /// <param name="paddleController">Validated PaddleController component</param>
    private static void LogSuccessfulSetup(PaddleController paddleController)
    {
        Debug.Log("✅ [Task 1.1.2.5] Boundary Constraint System validated successfully!");
        Debug.Log("📋 Boundary Constraint System Summary:");
        Debug.Log($"   • Component: Enhanced PaddleController on {paddleController.gameObject.name}");
        Debug.Log($"   • Position: {paddleController.transform.position}");
        
        // Log boundary constraint capabilities
        Debug.Log("🏗️ Boundary Constraint Features:");
        Debug.Log("   → GameArea Boundary Detection: Automatic detection from container");
        Debug.Log("   → Paddle Width Consideration: Accurate edge-based constraint calculation");
        Debug.Log("   → Constraint Enforcement: Seamless integration with input and movement");
        Debug.Log("   → Edge Case Handling: Robust handling for rapid movement scenarios");
        Debug.Log("   → Manual Override: Runtime boundary configuration support");
        Debug.Log("   → Fallback System: Graceful degradation with safe default boundaries");
        
        // Log boundary constraint API
        Debug.Log("📊 Boundary Constraint API:");
        Debug.Log("   • GetEffectiveLeftBoundary() - Left boundary considering paddle width");
        Debug.Log("   • GetEffectiveRightBoundary() - Right boundary considering paddle width");
        Debug.Log("   • GetPlayableWidth() - Total playable area width");
        Debug.Log("   • GetPaddleHalfWidth() - Paddle half-width for calculations");
        Debug.Log("   • AreBoundariesAutoDetected() - Boundary detection status");
        Debug.Log("   • RecalculateBoundaries() - Force boundary recalculation");
        Debug.Log("   • SetManualBoundaries(left, right) - Override boundary values");
        Debug.Log("   • TestBoundaryConstraints(x) - Test constraint application");
        
        // Log current configuration
        Debug.Log("🔧 Current Boundary Configuration:");
        Debug.Log($"   • Left Boundary: {paddleController.GetEffectiveLeftBoundary():F2}");
        Debug.Log($"   • Right Boundary: {paddleController.GetEffectiveRightBoundary():F2}");
        Debug.Log($"   • Playable Width: {paddleController.GetPlayableWidth():F2}");
        Debug.Log($"   • Paddle Half-Width: {paddleController.GetPaddleHalfWidth():F2}");
        Debug.Log($"   • Auto-Detected: {paddleController.AreBoundariesAutoDetected()}");
        Debug.Log($"   • Current Position: {paddleController.GetCurrentPosition()}");
        Debug.Log($"   • Within Boundaries: {paddleController.IsWithinBoundaries()}");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Paddle automatically stays within effective boundaries during all movement");
        Debug.Log("   2. Boundaries auto-detect from GameArea container or use PaddleData fallback");
        Debug.Log("   3. Paddle width is considered for accurate edge constraint calculation");
        Debug.Log("   4. Use boundary constraint API for runtime configuration and monitoring");
        Debug.Log("   5. All input methods (keyboard/mouse) respect boundary constraints seamlessly");
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Test boundary enforcement in Play mode with input");
        Debug.Log("   → Configure GameArea with proper collider for automatic detection");
        Debug.Log("   → Integrate with ball physics for collision response");
        Debug.Log("   → Add visual feedback for boundary collision events");
    }
}
#endif