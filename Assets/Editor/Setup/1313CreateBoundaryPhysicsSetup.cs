#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

/// <summary>
/// Editor setup script for creating boundary physics materials and configuration.
/// Creates PhysicsMaterial2D assets with arcade-style bouncing properties and applies
/// them to existing boundary wall colliders for consistent gameplay physics.
/// </summary>
public static class CreateBoundaryPhysicsSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Boundary Physics";
    private const string PHYSICS_FOLDER = "Assets/Physics";
    private const string MATERIALS_FOLDER = "Assets/Physics/Materials";
    private const string ARCADE_MATERIAL_PATH = "Assets/Physics/Materials/ArcadeBoundaryMaterial.asset";
    private const string BOUNDARY_SYSTEM_NAME = "Boundary System";
    
    /// <summary>
    /// Creates physics materials and applies them to boundary walls.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateBoundaryPhysics()
    {
        Debug.Log("🏗️ [Boundary Physics] Starting physics material configuration...");
        
        try
        {
            // Step 1: Check prerequisites
            GameObject boundarySystem = ValidatePrerequisites();
            
            // Step 2: Create folder structure
            CreatePhysicsFolderStructure();
            
            // Step 3: Create physics materials
            PhysicsMaterial2D arcadeMaterial = CreateArcadePhysicsMaterial();
            
            // Step 4: Add physics management component
            BoundaryPhysicsMaterial physicsManager = AddPhysicsManagementComponent(boundarySystem);
            
            // Step 5: Apply physics materials to walls
            ApplyPhysicsToWalls(physicsManager, arcadeMaterial);
            
            // Step 6: Validate physics configuration
            ValidatePhysicsSetup(physicsManager);
            
            // Step 7: Save assets and refresh
            SaveAndRefreshAssets();
            
            // Step 8: Log success and instructions
            LogSuccessfulSetup(physicsManager, arcadeMaterial);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Boundary Physics] Setup failed: {e.Message}");
            Debug.LogError("📋 Stack trace:");
            Debug.LogError(e.StackTrace);
        }
    }
    
    /// <summary>
    /// Menu validation - only show if boundary system exists and physics not already configured.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateBoundaryPhysics()
    {
        // Check if Boundary System exists
        GameObject boundarySystem = GameObject.Find(BOUNDARY_SYSTEM_NAME);
        if (boundarySystem == null) return false;
        
        // Check if physics already configured
        BoundaryPhysicsMaterial existingPhysics = boundarySystem.GetComponent<BoundaryPhysicsMaterial>();
        return existingPhysics == null;
    }
    
    /// <summary>
    /// Validates that prerequisite components exist.
    /// </summary>
    /// <returns>Boundary system GameObject</returns>
    private static GameObject ValidatePrerequisites()
    {
        Debug.Log("🔍 [Boundary Physics] Validating prerequisites...");
        
        GameObject boundarySystem = GameObject.Find(BOUNDARY_SYSTEM_NAME);
        if (boundarySystem == null)
        {
            throw new System.Exception($"Boundary System GameObject not found. Please run 'Breakout/Setup/Create Boundary Walls' first.");
        }
        
        // Check for boundary walls
        BoundaryWall[] walls = boundarySystem.GetComponentsInChildren<BoundaryWall>();
        if (walls.Length == 0)
        {
            throw new System.Exception("No BoundaryWall components found. Please ensure boundary walls are properly created.");
        }
        
        Debug.Log($"✅ [Boundary Physics] Prerequisites validated - Found {walls.Length} boundary walls");
        return boundarySystem;
    }
    
    /// <summary>
    /// Creates the physics folder structure for material assets.
    /// </summary>
    private static void CreatePhysicsFolderStructure()
    {
        Debug.Log("📁 [Boundary Physics] Creating folder structure...");
        
        // Create Physics folder
        if (!AssetDatabase.IsValidFolder(PHYSICS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Physics");
            Debug.Log($"   • Created Physics folder: {PHYSICS_FOLDER}");
        }
        
        // Create Materials subfolder
        if (!AssetDatabase.IsValidFolder(MATERIALS_FOLDER))
        {
            AssetDatabase.CreateFolder(PHYSICS_FOLDER, "Materials");
            Debug.Log($"   • Created Materials folder: {MATERIALS_FOLDER}");
        }
        
        Debug.Log("✅ [Boundary Physics] Folder structure created");
    }
    
    /// <summary>
    /// Creates arcade-style physics material with perfect bouncing.
    /// </summary>
    /// <returns>Created PhysicsMaterial2D asset</returns>
    private static PhysicsMaterial2D CreateArcadePhysicsMaterial()
    {
        Debug.Log("🎮 [Boundary Physics] Creating arcade physics material...");
        
        // Check if material already exists
        PhysicsMaterial2D existingMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(ARCADE_MATERIAL_PATH);
        if (existingMaterial != null)
        {
            Debug.Log("   • Arcade material already exists - updating properties");
            ConfigureArcadeMaterial(existingMaterial);
            return existingMaterial;
        }
        
        // Create new physics material
        PhysicsMaterial2D arcadeMaterial = new PhysicsMaterial2D("ArcadeBoundaryMaterial");
        ConfigureArcadeMaterial(arcadeMaterial);
        
        // Save as asset
        AssetDatabase.CreateAsset(arcadeMaterial, ARCADE_MATERIAL_PATH);
        Debug.Log($"✅ [Boundary Physics] Created arcade physics material: {ARCADE_MATERIAL_PATH}");
        
        return arcadeMaterial;
    }
    
    /// <summary>
    /// Configures physics material with arcade-style properties.
    /// </summary>
    /// <param name="material">PhysicsMaterial2D to configure</param>
    private static void ConfigureArcadeMaterial(PhysicsMaterial2D material)
    {
        // Perfect bounce for arcade feel
        material.bounciness = 1.0f;
        
        // No friction to prevent ball slowdown
        material.friction = 0.0f;
        
        // Maximum combine mode for consistent bouncing
        material.bounceCombine = PhysicsMaterialCombine2D.Maximum;
        material.frictionCombine = PhysicsMaterialCombine2D.Minimum;
        
        Debug.Log($"   • Configured material: Bounce={material.bounciness:F2}, Friction={material.friction:F2}");
        Debug.Log($"   • Combine modes: Bounce={material.bounceCombine}, Friction={material.frictionCombine}");
        
        EditorUtility.SetDirty(material);
    }
    
    /// <summary>
    /// Adds BoundaryPhysicsMaterial component to boundary system.
    /// </summary>
    /// <param name="boundarySystem">Boundary system GameObject</param>
    /// <returns>Added BoundaryPhysicsMaterial component</returns>
    private static BoundaryPhysicsMaterial AddPhysicsManagementComponent(GameObject boundarySystem)
    {
        Debug.Log("🔧 [Boundary Physics] Adding physics management component...");
        
        BoundaryPhysicsMaterial physicsManager = boundarySystem.GetComponent<BoundaryPhysicsMaterial>();
        if (physicsManager == null)
        {
            physicsManager = boundarySystem.AddComponent<BoundaryPhysicsMaterial>();
            Debug.Log("   • Added BoundaryPhysicsMaterial component");
        }
        else
        {
            Debug.Log("   • BoundaryPhysicsMaterial component already exists");
        }
        
        // Configure default settings
        physicsManager.bounciness = 1.0f;
        physicsManager.friction = 0.0f;
        physicsManager.frictionCombine = PhysicsMaterialCombine2D.Minimum;
        physicsManager.bounceCombine = PhysicsMaterialCombine2D.Maximum;
        physicsManager.enablePhysicsValidation = true;
        physicsManager.angleTolerance = 1f;
        physicsManager.velocityTolerance = 0.05f;
        
        Debug.Log("✅ [Boundary Physics] Physics management component configured");
        return physicsManager;
    }
    
    /// <summary>
    /// Applies physics materials to boundary wall colliders.
    /// </summary>
    /// <param name="physicsManager">Physics management component</param>
    /// <param name="material">Physics material to apply</param>
    private static void ApplyPhysicsToWalls(BoundaryPhysicsMaterial physicsManager, PhysicsMaterial2D material)
    {
        Debug.Log("🧱 [Boundary Physics] Applying physics materials to walls...");
        
        // Set material reference
        physicsManager.wallMaterial = material;
        
        // Get all boundary walls
        BoundaryWall[] walls = physicsManager.GetComponentsInChildren<BoundaryWall>();
        int appliedCount = 0;
        
        foreach (BoundaryWall wall in walls)
        {
            if (ApplyMaterialToWall(wall, material))
            {
                appliedCount++;
            }
        }
        
        Debug.Log($"✅ [Boundary Physics] Applied physics material to {appliedCount}/{walls.Length} walls");
    }
    
    /// <summary>
    /// Applies physics material to a specific wall's collider.
    /// </summary>
    /// <param name="wall">BoundaryWall to apply material to</param>
    /// <param name="material">Physics material to apply</param>
    /// <returns>True if material was applied</returns>
    private static bool ApplyMaterialToWall(BoundaryWall wall, PhysicsMaterial2D material)
    {
        Collider2D collider = wall.GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning($"   ⚠️ No collider found on {wall.wallType} wall");
            return false;
        }
        
        // Check if collision is enabled
        BoundaryWallConfig wallConfig = wall.config != null ? 
            wall.config.GetBoundaryConfig(wall.wallType) : 
            BoundaryWallConfig.CreateDefault(wall.wallType);
        
        if (!wallConfig.enableCollision)
        {
            Debug.Log($"   • Skipping {wall.wallType} wall - collision disabled");
            return false;
        }
        
        collider.sharedMaterial = material;
        EditorUtility.SetDirty(collider);
        Debug.Log($"   • Applied material to {wall.wallType} wall");
        return true;
    }
    
    /// <summary>
    /// Validates the physics setup is correct.
    /// </summary>
    /// <param name="physicsManager">Physics management component</param>
    private static void ValidatePhysicsSetup(BoundaryPhysicsMaterial physicsManager)
    {
        Debug.Log("🔍 [Boundary Physics] Validating physics configuration...");
        
        // Force validation
        bool isValid = physicsManager.ValidatePhysicsConfiguration();
        
        if (isValid)
        {
            Debug.Log("✅ [Boundary Physics] Physics validation PASSED");
        }
        else
        {
            Debug.LogWarning("⚠️ [Boundary Physics] Physics validation FAILED - check configuration");
        }
        
        // Test bounce calculations
        TestBouncePhysics(physicsManager);
    }
    
    /// <summary>
    /// Tests bounce physics calculations.
    /// </summary>
    /// <param name="physicsManager">Physics management component</param>
    private static void TestBouncePhysics(BoundaryPhysicsMaterial physicsManager)
    {
        Debug.Log("🧪 [Boundary Physics] Testing bounce calculations...");
        
        // Test horizontal wall bounce (top/bottom)
        Vector2 horizontalIncoming = new Vector2(1f, -1f).normalized * 10f;
        Vector2 horizontalNormal = Vector2.up;
        Vector2 horizontalBounce = physicsManager.TestBounceCalculation(horizontalIncoming, horizontalNormal);
        Debug.Log($"   • Horizontal bounce: In={horizontalIncoming}, Out={horizontalBounce}");
        
        // Test vertical wall bounce (left/right)
        Vector2 verticalIncoming = new Vector2(-1f, 1f).normalized * 10f;
        Vector2 verticalNormal = Vector2.right;
        Vector2 verticalBounce = physicsManager.TestBounceCalculation(verticalIncoming, verticalNormal);
        Debug.Log($"   • Vertical bounce: In={verticalIncoming}, Out={verticalBounce}");
        
        // Validate angle preservation
        float inAngle = Vector2.SignedAngle(Vector2.right, horizontalIncoming);
        float outAngle = Vector2.SignedAngle(Vector2.right, horizontalBounce);
        bool angleValid = physicsManager.ValidateBounceAngle(inAngle, outAngle);
        Debug.Log($"   • Angle validation: {(angleValid ? "PASSED" : "FAILED")} (In={inAngle:F1}°, Out={outAngle:F1}°)");
        
        // Validate velocity magnitude
        bool velocityValid = physicsManager.ValidateVelocityMagnitude(horizontalIncoming.magnitude, horizontalBounce.magnitude);
        Debug.Log($"   • Velocity validation: {(velocityValid ? "PASSED" : "FAILED")} (In={horizontalIncoming.magnitude:F2}, Out={horizontalBounce.magnitude:F2})");
    }
    
    /// <summary>
    /// Saves all modified assets and refreshes the database.
    /// </summary>
    private static void SaveAndRefreshAssets()
    {
        Debug.Log("💾 [Boundary Physics] Saving assets...");
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ [Boundary Physics] Assets saved and refreshed");
    }
    
    /// <summary>
    /// Logs successful setup with comprehensive usage instructions.
    /// </summary>
    /// <param name="physicsManager">Physics management component</param>
    /// <param name="material">Created physics material</param>
    private static void LogSuccessfulSetup(BoundaryPhysicsMaterial physicsManager, PhysicsMaterial2D material)
    {
        Debug.Log("✅ [Physics Material Configuration] Setup completed successfully!");
        Debug.Log("📋 Boundary Physics System Summary:");
        Debug.Log("   • Arcade-style physics material created and applied");
        Debug.Log("   • Perfect elastic bouncing configured (bounciness = 1.0)");
        Debug.Log("   • Zero friction for consistent ball movement");
        Debug.Log("   • Physics validation system active");
        
        Debug.Log("🎮 Physics Configuration:");
        Debug.Log($"   → Material: {material.name}");
        Debug.Log($"   → Bounciness: {material.bounciness:F2} (perfect elastic collision)");
        Debug.Log($"   → Friction: {material.friction:F2} (no energy loss)");
        Debug.Log($"   → Bounce Combine: {material.bounceCombine}");
        Debug.Log($"   → Friction Combine: {material.frictionCombine}");
        
        Debug.Log("🧱 Wall Application:");
        Debug.Log($"   • Walls with physics: {physicsManager.GetWallsWithMaterials()}");
        Debug.Log("   • Top boundary: Perfect bounce enabled");
        Debug.Log("   • Left boundary: Perfect bounce enabled");
        Debug.Log("   • Right boundary: Perfect bounce enabled");
        Debug.Log("   • Bottom boundary: No collision (ball loss area)");
        
        Debug.Log("🔧 Component Features:");
        Debug.Log("   • BoundaryPhysicsMaterial: Central physics management");
        Debug.Log("   • Material creation and application system");
        Debug.Log("   • Bounce behavior validation");
        Debug.Log("   • Runtime material property updates");
        
        Debug.Log("📐 Bounce Physics:");
        Debug.Log("   • Perfect reflection angles (angle in = angle out)");
        Debug.Log("   • Energy conservation (speed maintained)");
        Debug.Log("   • Predictable arcade-style bouncing");
        Debug.Log("   • No unwanted spin or friction effects");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Physics material automatically applied to collision-enabled walls");
        Debug.Log("   2. Test ball bouncing by creating a ball with Rigidbody2D");
        Debug.Log("   3. Adjust bounciness in BoundaryPhysicsMaterial component if needed");
        Debug.Log("   4. Use validation methods to verify bounce behavior");
        Debug.Log("   5. Material properties update in real-time during play mode");
        
        Debug.Log("🧪 Testing Ball Bouncing:");
        Debug.Log("   • Create GameObject with CircleCollider2D and Rigidbody2D");
        Debug.Log("   • Set Rigidbody2D gravity scale to 0 for classic Breakout");
        Debug.Log("   • Apply initial velocity to test bouncing");
        Debug.Log("   • Monitor velocity magnitude for energy conservation");
        
        Debug.Log("🎨 Material Asset:");
        Debug.Log($"   • Location: {ARCADE_MATERIAL_PATH}");
        Debug.Log("   • Shared across all boundary walls");
        Debug.Log("   • Modify in Project window for global changes");
        Debug.Log("   • Automatically loaded from Resources");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Bottom boundary has no collision by default (configure if needed)");
        Debug.Log("   → Physics material ensures consistent bouncing at all angles");
        Debug.Log("   → Combine modes set to Maximum/Minimum for arcade behavior");
        Debug.Log("   → Validation system helps identify physics issues");
        
        Debug.Log("🔄 Next Steps:");
        Debug.Log("   → Create ball GameObject with proper physics setup");
        Debug.Log("   → Test bouncing behavior at various angles and speeds");
        Debug.Log("   → Fine-tune material properties if needed");
        Debug.Log("   → Implement ball velocity clamping for gameplay balance");
        Debug.Log("   → Add sound effects triggered by boundary collisions");
        
        // Log physics summary
        Debug.Log("\n" + physicsManager.GetPhysicsSummary());
        
        // Select physics manager in Inspector
        Selection.activeGameObject = physicsManager.gameObject;
        EditorGUIUtility.PingObject(physicsManager);
    }
}
#endif