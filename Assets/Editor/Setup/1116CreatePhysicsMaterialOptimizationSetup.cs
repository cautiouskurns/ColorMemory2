#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for creating and optimizing Physics Material 2D assets for arcade-style ball physics.
/// Provides consistent bouncing behavior with arcade-appropriate parameters for Breakout gameplay.
/// </summary>
public static class CreatePhysicsMaterialOptimizationSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Physics Material Optimization";
    private const string BALL_NAME = "Ball";
    private const string MATERIALS_FOLDER = "Assets/Materials";
    private const string MATERIAL_ASSET_PATH = "Assets/Materials/BallPhysics.physicsMaterial2D";
    private const string MATERIAL_NAME = "BallPhysics";
    
    // Optimized arcade physics parameters
    private const float ARCADE_FRICTION = 0.1f;
    private const float ARCADE_BOUNCINESS = 0.9f;
    private const PhysicsMaterialCombine2D FRICTION_COMBINE = PhysicsMaterialCombine2D.Minimum;
    private const PhysicsMaterialCombine2D BOUNCINESS_COMBINE = PhysicsMaterialCombine2D.Maximum;
    
    /// <summary>
    /// Creates and configures optimized physics material for ball GameObject.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreatePhysicsMaterialOptimization()
    {
        Debug.Log("⚙️ [Task 1.1.1.6] Starting Physics Material Optimization...");
        
        try
        {
            // Step 1: Ensure Materials folder exists
            EnsureMaterialsFolderExists();
            
            // Step 2: Create optimized physics material asset
            PhysicsMaterial2D ballPhysicsMaterial = CreateOptimizedBallMaterial();
            
            // Step 3: Validate material parameters
            if (!ValidateMaterialParameters(ballPhysicsMaterial))
            {
                Debug.LogError("❌ [Task 1.1.1.6] Material parameter validation failed");
                return;
            }
            
            // Step 4: Apply material to Ball GameObject
            ApplyMaterialToBallCollider(ballPhysicsMaterial);
            
            // Step 5: Final validation and cleanup
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            LogSuccessfulSetup(ballPhysicsMaterial);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.1.6] Physics Material Optimization creation failed: {e.Message}");
            Debug.LogError("📋 Please ensure Ball GameObject with CircleCollider2D exists.");
        }
    }
    
    /// <summary>
    /// Menu validation - prevents duplicate material creation.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreatePhysicsMaterialOptimization()
    {
        // Check if material asset already exists
        PhysicsMaterial2D existingMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(MATERIAL_ASSET_PATH);
        if (existingMaterial != null)
        {
            // Check if material is already applied to ball
            GameObject ball = GameObject.Find(BALL_NAME);
            if (ball != null)
            {
                CircleCollider2D collider = ball.GetComponent<CircleCollider2D>();
                if (collider != null && collider.sharedMaterial == existingMaterial)
                {
                    Debug.LogWarning("⚠️ Physics Material Optimization already exists and is applied to Ball.");
                    return false;
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Ensures Materials folder exists, creating it if necessary.
    /// </summary>
    private static void EnsureMaterialsFolderExists()
    {
        if (!AssetDatabase.IsValidFolder(MATERIALS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("📁 [Step 1/4] Created Materials folder");
        }
        else
        {
            Debug.Log("📁 [Step 1/4] Materials folder exists");
        }
    }
    
    /// <summary>
    /// Creates optimized Physics Material 2D asset with arcade-tuned parameters.
    /// </summary>
    /// <returns>Created Physics Material 2D asset</returns>
    private static PhysicsMaterial2D CreateOptimizedBallMaterial()
    {
        // Check if material already exists
        PhysicsMaterial2D existingMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(MATERIAL_ASSET_PATH);
        if (existingMaterial != null)
        {
            Debug.Log("📄 [Step 2/4] Using existing physics material");
            return existingMaterial;
        }
        
        // Create new physics material
        PhysicsMaterial2D ballMaterial = new PhysicsMaterial2D(MATERIAL_NAME);
        
        // Configure arcade physics parameters
        ballMaterial.friction = ARCADE_FRICTION;
        ballMaterial.bounciness = ARCADE_BOUNCINESS;
        ballMaterial.frictionCombine = FRICTION_COMBINE;
        ballMaterial.bounceCombine = BOUNCINESS_COMBINE;
        
        // Save as asset
        AssetDatabase.CreateAsset(ballMaterial, MATERIAL_ASSET_PATH);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("📄 [Step 2/4] Physics Material 2D created:");
        Debug.Log($"   • Asset Path: {MATERIAL_ASSET_PATH}");
        Debug.Log($"   • Friction: {ballMaterial.friction} (minimal surface drag)");
        Debug.Log($"   • Bounciness: {ballMaterial.bounciness} (high arcade bounce)");
        Debug.Log($"   • Friction Combine: {ballMaterial.frictionCombine}");
        Debug.Log($"   • Bounce Combine: {ballMaterial.bounceCombine}");
        
        return ballMaterial;
    }
    
    /// <summary>
    /// Validates physics material parameters are within acceptable ranges.
    /// </summary>
    /// <param name="material">Physics material to validate</param>
    /// <returns>True if parameters are valid</returns>
    private static bool ValidateMaterialParameters(PhysicsMaterial2D material)
    {
        if (material == null)
        {
            Debug.LogError("❌ [Step 3/4] Material validation failed: Material is null");
            return false;
        }
        
        // Validate friction range (0.0-0.2 for arcade physics)
        if (material.friction < 0.0f || material.friction > 0.2f)
        {
            Debug.LogError($"❌ [Step 3/4] Friction value {material.friction} outside acceptable range (0.0-0.2)");
            return false;
        }
        
        // Validate bounciness range (0.8-1.0 for arcade physics)
        if (material.bounciness < 0.8f || material.bounciness > 1.0f)
        {
            Debug.LogError($"❌ [Step 3/4] Bounciness value {material.bounciness} outside acceptable range (0.8-1.0)");
            return false;
        }
        
        // Validate combine modes for arcade behavior
        if (material.frictionCombine != PhysicsMaterialCombine2D.Minimum)
        {
            Debug.LogWarning($"⚠️ [Step 3/4] Friction combine mode {material.frictionCombine} not optimal for arcade physics");
        }
        
        if (material.bounceCombine != PhysicsMaterialCombine2D.Maximum)
        {
            Debug.LogWarning($"⚠️ [Step 3/4] Bounce combine mode {material.bounceCombine} not optimal for arcade physics");
        }
        
        Debug.Log("✅ [Step 3/4] Material parameter validation successful:");
        Debug.Log($"   • Friction: {material.friction} (within 0.0-0.2 range)");
        Debug.Log($"   • Bounciness: {material.bounciness} (within 0.8-1.0 range)");
        Debug.Log($"   • Combine modes: Optimized for arcade physics");
        
        return true;
    }
    
    /// <summary>
    /// Applies physics material to Ball GameObject's CircleCollider2D component.
    /// </summary>
    /// <param name="material">Physics material to apply</param>
    private static void ApplyMaterialToBallCollider(PhysicsMaterial2D material)
    {
        // Find Ball GameObject
        GameObject ball = GameObject.Find(BALL_NAME);
        if (ball == null)
        {
            Debug.LogError("❌ [Step 4/4] Ball GameObject not found!");
            Debug.LogError("📋 Required: Complete Task 1.1.1.2 (Ball GameObject Configuration) first");
            Debug.LogError("💡 Run: Breakout/Setup/Create Ball GameObject");
            return;
        }
        
        // Get CircleCollider2D component
        CircleCollider2D circleCollider = ball.GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            Debug.LogError("❌ [Step 4/4] CircleCollider2D component not found on Ball GameObject!");
            Debug.LogError("📋 Ball GameObject must have CircleCollider2D component for physics material application");
            return;
        }
        
        // Apply physics material
        circleCollider.sharedMaterial = material;
        EditorUtility.SetDirty(ball);
        
        Debug.Log("✅ [Step 4/4] Physics material applied to Ball:");
        Debug.Log($"   • Target GameObject: {ball.name}");
        Debug.Log($"   • Component: CircleCollider2D");
        Debug.Log($"   • Material Applied: {material.name}");
        Debug.Log($"   • Hierarchy Path: {GetGameObjectPath(ball)}");
        
        // Additional validation
        if (circleCollider.sharedMaterial == material)
        {
            Debug.Log("✅ Material application verified successfully");
        }
        else
        {
            Debug.LogError("❌ Material application verification failed");
        }
    }
    
    /// <summary>
    /// Logs successful physics material optimization setup.
    /// </summary>
    /// <param name="material">Created physics material</param>
    private static void LogSuccessfulSetup(PhysicsMaterial2D material)
    {
        GameObject ball = GameObject.Find(BALL_NAME);
        
        Debug.Log("✅ [Task 1.1.1.6] Physics Material Optimization created successfully!");
        Debug.Log("📋 Physics Material Configuration Summary:");
        Debug.Log($"   • Asset Created: {MATERIAL_ASSET_PATH}");
        Debug.Log($"   • Material Name: {material.name}");
        Debug.Log($"   • Friction: {material.friction} (minimal surface drag)");
        Debug.Log($"   • Bounciness: {material.bounciness} (high arcade bounce)");
        Debug.Log($"   • Friction Combine: {material.frictionCombine} (prevents velocity reduction)");
        Debug.Log($"   • Bounce Combine: {material.bounceCombine} (consistent high bounce)");
        Debug.Log($"   • Applied to: {(ball != null ? ball.name : "Not Applied")}");
        Debug.Log($"   • Component: CircleCollider2D.sharedMaterial");
        Debug.Log("⚙️ Physics Material Optimization Features:");
        Debug.Log("   → Arcade-appropriate bouncing behavior for consistent gameplay");
        Debug.Log("   → Minimal friction for smooth ball movement");
        Debug.Log("   → High bounciness for satisfying collision response");
        Debug.Log("   → Optimized combine modes for predictable physics behavior");
        Debug.Log("   → Zero runtime overhead with native Unity physics processing");
        Debug.Log("   → Parameter validation ensuring arcade physics requirements");
        Debug.Log("💡 Testing Instructions:");
        Debug.Log("   1. Enter Play mode");
        Debug.Log("   2. Launch ball and observe bounce behavior");
        Debug.Log("   3. Test collisions with different surfaces");
        Debug.Log("   4. Verify consistent bounce height and angle response");
        Debug.Log("   5. Check for immediate collision response without lag");
        Debug.Log("🔧 Parameter Tuning Guidelines:");
        Debug.Log("   • Friction Range: 0.0-0.2 (0.1 optimal for arcade feel)");
        Debug.Log("   • Bounciness Range: 0.8-1.0 (0.9 optimal for satisfying bounce)");
        Debug.Log("   • Lower friction = smoother movement, higher friction = more control");
        Debug.Log("   • Higher bounciness = more energetic gameplay, lower = more controlled");
    }
    
    /// <summary>
    /// Gets full hierarchy path of a GameObject.
    /// </summary>
    /// <param name="obj">GameObject to get path for</param>
    /// <returns>Full hierarchy path string</returns>
    private static string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
    
    /// <summary>
    /// Creates a comprehensive physics material test for validation.
    /// </summary>
    /// <param name="material">Material to test</param>
    /// <returns>Test results summary</returns>
    public static string TestPhysicsMaterialBehavior(PhysicsMaterial2D material)
    {
        if (material == null)
        {
            return "❌ Material Test Failed: Material is null";
        }
        
        string testResults = $"🧪 Physics Material Test Results for '{material.name}':\n";
        testResults += $"   • Friction: {material.friction} ";
        testResults += (material.friction >= 0.0f && material.friction <= 0.2f) ? "✅ (Valid)" : "❌ (Invalid)";
        testResults += $"\n   • Bounciness: {material.bounciness} ";
        testResults += (material.bounciness >= 0.8f && material.bounciness <= 1.0f) ? "✅ (Valid)" : "❌ (Invalid)";
        testResults += $"\n   • Friction Combine: {material.frictionCombine} ";
        testResults += (material.frictionCombine == PhysicsMaterialCombine2D.Minimum) ? "✅ (Optimal)" : "⚠️ (Suboptimal)";
        testResults += $"\n   • Bounce Combine: {material.bounceCombine} ";
        testResults += (material.bounceCombine == PhysicsMaterialCombine2D.Maximum) ? "✅ (Optimal)" : "⚠️ (Suboptimal)";
        
        // Calculate arcade suitability score
        int score = 0;
        if (material.friction >= 0.0f && material.friction <= 0.2f) score++;
        if (material.bounciness >= 0.8f && material.bounciness <= 1.0f) score++;
        if (material.frictionCombine == PhysicsMaterialCombine2D.Minimum) score++;
        if (material.bounceCombine == PhysicsMaterialCombine2D.Maximum) score++;
        
        testResults += $"\n   • Arcade Suitability: {score}/4 ";
        testResults += (score == 4) ? "✅ (Excellent)" : (score >= 2) ? "⚠️ (Acceptable)" : "❌ (Poor)";
        
        return testResults;
    }
}
#endif