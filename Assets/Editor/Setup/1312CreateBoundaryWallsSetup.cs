#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for creating Physical Boundary Wall GameObjects and scene setup.
/// Creates individual boundary wall GameObjects with proper hierarchy organization, 
/// component setup, and positioning based on BoundaryConfig configuration.
/// </summary>
public static class CreateBoundaryWallsSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Boundary Walls";
    private const string BOUNDARY_SYSTEM_NAME = "Boundary System";
    private const string BOUNDARY_CONFIG_PATH = "BoundaryConfig";
    
    /// <summary>
    /// Creates the complete boundary wall system with GameObjects, components, and positioning.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateBoundaryWalls()
    {
        Debug.Log("🏗️ [Boundary Walls] Starting physical boundary wall creation...");
        
        try
        {
            // Step 1: Load or validate BoundaryConfig
            BoundaryConfig boundaryConfig = LoadBoundaryConfiguration();
            
            // Step 2: Create boundary system hierarchy
            GameObject boundarySystem = CreateBoundarySystemHierarchy();
            
            // Step 3: Create individual boundary walls
            CreateIndividualBoundaryWalls(boundarySystem, boundaryConfig);
            
            // Step 4: Configure wall properties and positioning
            ConfigureBoundaryWalls(boundarySystem, boundaryConfig);
            
            // Step 5: Validate created system
            ValidateCreatedBoundaries(boundarySystem, boundaryConfig);
            
            // Step 6: Log success and usage instructions
            LogSuccessfulCreation(boundarySystem, boundaryConfig);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Boundary Walls] Creation failed: {e.Message}");
            Debug.LogError("📋 Please check Unity console for detailed error information");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if boundary system doesn't already exist
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateBoundaryWalls()
    {
        // Check if Boundary System already exists in scene
        GameObject existingSystem = GameObject.Find(BOUNDARY_SYSTEM_NAME);
        return existingSystem == null;
    }
    
    /// <summary>
    /// Loads the BoundaryConfig asset from Resources folder
    /// </summary>
    /// <returns>Loaded BoundaryConfig asset</returns>
    private static BoundaryConfig LoadBoundaryConfiguration()
    {
        Debug.Log("📄 [Boundary Walls] Loading boundary configuration...");
        
        BoundaryConfig config = Resources.Load<BoundaryConfig>(BOUNDARY_CONFIG_PATH);
        
        if (config == null)
        {
            throw new System.Exception($"BoundaryConfig asset not found at Resources/{BOUNDARY_CONFIG_PATH}. Please create configuration first using 'Breakout/Setup/Create Boundary Configuration'.");
        }
        
        // Validate configuration
        if (!config.ValidateConfiguration())
        {
            throw new System.Exception("BoundaryConfig validation failed. Please check configuration settings in Inspector.");
        }
        
        Debug.Log($"✅ [Boundary Walls] BoundaryConfig loaded successfully: {config.name}");
        return config;
    }
    
    /// <summary>
    /// Creates the boundary system hierarchy organization
    /// </summary>
    /// <returns>Parent GameObject for boundary system</returns>
    private static GameObject CreateBoundarySystemHierarchy()
    {
        Debug.Log("🏗️ [Boundary Walls] Creating boundary system hierarchy...");
        
        // Create main boundary system container
        GameObject boundarySystem = new GameObject(BOUNDARY_SYSTEM_NAME);
        boundarySystem.transform.position = Vector3.zero;
        boundarySystem.transform.rotation = Quaternion.identity;
        boundarySystem.transform.localScale = Vector3.one;
        
        // Add identifying tag
        boundarySystem.tag = "Untagged"; // Can be customized later
        boundarySystem.layer = LayerMask.NameToLayer("Default");
        
        Debug.Log($"✅ [Boundary Walls] Boundary system hierarchy created: {boundarySystem.name}");
        return boundarySystem;
    }
    
    /// <summary>
    /// Creates individual boundary wall GameObjects for each boundary type
    /// </summary>
    /// <param name="parent">Parent GameObject for organization</param>
    /// <param name="config">Boundary configuration</param>
    private static void CreateIndividualBoundaryWalls(GameObject parent, BoundaryConfig config)
    {
        Debug.Log("🧱 [Boundary Walls] Creating individual boundary wall GameObjects...");
        
        // Create walls for each boundary type
        System.Array boundaryTypes = System.Enum.GetValues(typeof(BoundaryType));
        
        foreach (BoundaryType boundaryType in boundaryTypes)
        {
            CreateBoundaryWallGameObject(parent, boundaryType, config);
        }
        
        Debug.Log($"✅ [Boundary Walls] Created {boundaryTypes.Length} boundary wall GameObjects");
    }
    
    /// <summary>
    /// Creates a single boundary wall GameObject with components
    /// </summary>
    /// <param name="parent">Parent GameObject</param>
    /// <param name="boundaryType">Type of boundary to create</param>
    /// <param name="config">Boundary configuration</param>
    /// <returns>Created boundary wall GameObject</returns>
    private static GameObject CreateBoundaryWallGameObject(GameObject parent, BoundaryType boundaryType, BoundaryConfig config)
    {
        string wallName = $"{boundaryType} Boundary Wall";
        Debug.Log($"   • Creating {wallName}...");
        
        // Create GameObject
        GameObject wallObject = new GameObject(wallName);
        wallObject.transform.SetParent(parent.transform);
        wallObject.transform.localScale = Vector3.one;
        
        // Add BoundaryWall component
        BoundaryWall boundaryWall = wallObject.AddComponent<BoundaryWall>();
        boundaryWall.wallType = boundaryType;
        boundaryWall.config = config;
        
        // Add BoxCollider2D component (since Awake won't run in edit mode)
        BoxCollider2D boxCollider = wallObject.AddComponent<BoxCollider2D>();
        Debug.Log($"     • Added BoxCollider2D to {wallName}");
        
        // Get wall-specific configuration
        BoundaryWallConfig wallConfig = config.GetBoundaryConfig(boundaryType);
        
        // Configure collision layer
        if (wallConfig.collisionLayer >= 0 && wallConfig.collisionLayer <= 31)
        {
            wallObject.layer = wallConfig.collisionLayer;
        }
        
        Debug.Log($"     ✅ {wallName} created with BoundaryWall component");
        return wallObject;
    }
    
    /// <summary>
    /// Configures all boundary wall properties and positioning
    /// </summary>
    /// <param name="boundarySystem">Boundary system parent GameObject</param>
    /// <param name="config">Boundary configuration</param>
    private static void ConfigureBoundaryWalls(GameObject boundarySystem, BoundaryConfig config)
    {
        Debug.Log("⚙️ [Boundary Walls] Configuring wall properties and positioning...");
        
        BoundaryWall[] boundaryWalls = boundarySystem.GetComponentsInChildren<BoundaryWall>();
        
        foreach (BoundaryWall wall in boundaryWalls)
        {
            ConfigureIndividualWall(wall, config);
        }
        
        Debug.Log($"✅ [Boundary Walls] Configured {boundaryWalls.Length} boundary walls");
    }
    
    /// <summary>
    /// Configures an individual boundary wall with proper setup
    /// </summary>
    /// <param name="wall">BoundaryWall component to configure</param>
    /// <param name="config">Boundary configuration</param>
    private static void ConfigureIndividualWall(BoundaryWall wall, BoundaryConfig config)
    {
        BoundaryType wallType = wall.wallType;
        Debug.Log($"   • Configuring {wallType} boundary wall...");
        
        // Set configuration reference
        wall.config = config;
        
        // Calculate and apply position
        Vector3 calculatedPosition = config.CalculateBoundaryPosition(wallType);
        wall.transform.position = calculatedPosition;
        
        // Apply rotation if specified
        BoundaryWallConfig wallConfig = config.GetBoundaryConfig(wallType);
        if (wallConfig.rotationOffset != Vector3.zero)
        {
            wall.transform.rotation = Quaternion.Euler(wallConfig.rotationOffset);
        }
        
        // Configure BoxCollider2D dimensions
        BoxCollider2D collider = wall.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            // Apply resolution scaling if enabled
            float scaleFactor = config.enableResolutionScaling ? config.CalculateResolutionScaleFactor() : 1f;
            
            // Set collider size based on wall configuration
            collider.size = new Vector2(
                wallConfig.width * scaleFactor,
                wallConfig.height * scaleFactor
            );
            
            // Enable/disable based on configuration
            collider.enabled = wallConfig.enableCollision;
            
            Debug.Log($"     • Collider configured: Size={collider.size}, Enabled={collider.enabled}");
        }
        
        // Force component initialization (will trigger Start() method)
        wall.UpdateWallPosition();
        
        Debug.Log($"     ✅ {wallType} wall configured at position: {calculatedPosition}");
    }
    
    /// <summary>
    /// Validates the created boundary system for completeness and correctness
    /// </summary>
    /// <param name="boundarySystem">Created boundary system</param>
    /// <param name="config">Boundary configuration used</param>
    private static void ValidateCreatedBoundaries(GameObject boundarySystem, BoundaryConfig config)
    {
        Debug.Log("🔍 [Boundary Walls] Validating created boundary system...");
        
        BoundaryWall[] walls = boundarySystem.GetComponentsInChildren<BoundaryWall>();
        bool systemValid = true;
        
        // Check that all boundary types are present
        System.Array boundaryTypes = System.Enum.GetValues(typeof(BoundaryType));
        foreach (BoundaryType expectedType in boundaryTypes)
        {
            bool typeFound = false;
            foreach (BoundaryWall wall in walls)
            {
                if (wall.wallType == expectedType)
                {
                    typeFound = true;
                    
                    // Validate individual wall
                    if (!wall.ValidateWall())
                    {
                        Debug.LogWarning($"   ⚠️ {expectedType} boundary wall validation failed");
                        systemValid = false;
                    }
                    break;
                }
            }
            
            if (!typeFound)
            {
                Debug.LogError($"   ❌ Missing {expectedType} boundary wall");
                systemValid = false;
            }
        }
        
        // Check wall count
        if (walls.Length != boundaryTypes.Length)
        {
            Debug.LogWarning($"   ⚠️ Expected {boundaryTypes.Length} walls, found {walls.Length}");
            systemValid = false;
        }
        
        // Validate positioning
        ValidateWallPositioning(walls, config);
        
        if (systemValid)
        {
            Debug.Log("✅ [Boundary Walls] System validation passed");
        }
        else
        {
            Debug.LogWarning("⚠️ [Boundary Walls] System validation issues detected - check individual walls");
        }
    }
    
    /// <summary>
    /// Validates boundary wall positioning relative to play area
    /// </summary>
    /// <param name="walls">Array of boundary walls to validate</param>
    /// <param name="config">Boundary configuration</param>
    private static void ValidateWallPositioning(BoundaryWall[] walls, BoundaryConfig config)
    {
        Debug.Log("   📐 Validating wall positioning...");
        
        Vector3 playAreaCenter = config.playAreaCenter;
        float playAreaWidth = config.playAreaWidth;
        float playAreaHeight = config.playAreaHeight;
        
        foreach (BoundaryWall wall in walls)
        {
            Vector3 wallPosition = wall.transform.position;
            BoundaryType wallType = wall.wallType;
            
            // Check positioning relative to play area
            switch (wallType)
            {
                case BoundaryType.Top:
                    if (wallPosition.y <= playAreaCenter.y + playAreaHeight * 0.5f)
                    {
                        Debug.LogWarning($"     ⚠️ Top wall may be too low: {wallPosition.y}");
                    }
                    break;
                    
                case BoundaryType.Bottom:
                    if (wallPosition.y >= playAreaCenter.y - playAreaHeight * 0.5f)
                    {
                        Debug.LogWarning($"     ⚠️ Bottom wall may be too high: {wallPosition.y}");
                    }
                    break;
                    
                case BoundaryType.Left:
                    if (wallPosition.x >= playAreaCenter.x - playAreaWidth * 0.5f)
                    {
                        Debug.LogWarning($"     ⚠️ Left wall may be too far right: {wallPosition.x}");
                    }
                    break;
                    
                case BoundaryType.Right:
                    if (wallPosition.x <= playAreaCenter.x + playAreaWidth * 0.5f)
                    {
                        Debug.LogWarning($"     ⚠️ Right wall may be too far left: {wallPosition.x}");
                    }
                    break;
            }
        }
        
        Debug.Log("     ✅ Wall positioning validation completed");
    }
    
    /// <summary>
    /// Logs successful creation with comprehensive usage instructions
    /// </summary>
    /// <param name="boundarySystem">Created boundary system</param>
    /// <param name="config">Used boundary configuration</param>
    private static void LogSuccessfulCreation(GameObject boundarySystem, BoundaryConfig config)
    {
        BoundaryWall[] walls = boundarySystem.GetComponentsInChildren<BoundaryWall>();
        
        Debug.Log("✅ [Physical Boundary Wall Creation] Setup completed successfully!");
        Debug.Log("📋 Boundary Wall System Summary:");
        Debug.Log("   • Physical boundary wall GameObjects created in scene");
        Debug.Log("   • BoundaryWall components configured with collision detection");
        Debug.Log("   • Wall positioning calculated based on play area and aspect ratio");
        Debug.Log("   • Hierarchy organization with centralized system container");
        
        Debug.Log("🏗️ Created GameObjects:");
        Debug.Log($"   → {boundarySystem.name}: Parent container for boundary system");
        foreach (BoundaryWall wall in walls)
        {
            BoundaryWallConfig wallConfig = config.GetBoundaryConfig(wall.wallType);
            string collisionStatus = wallConfig.enableCollision ? "Collision Enabled" : "No Collision";
            Debug.Log($"   → {wall.gameObject.name}: {collisionStatus}, Bounce: {wallConfig.bounceCoefficient:F1}");
        }
        
        Debug.Log("⚙️ Component Configuration:");
        Debug.Log("   • BoundaryWall MonoBehaviour: Individual wall management");
        Debug.Log("   • BoxCollider2D: Physics collision detection");
        Debug.Log("   • Transform: Positioning and scaling");
        Debug.Log("   • Configuration reference: Links to BoundaryConfig asset");
        
        Debug.Log("🎮 Gameplay Features:");
        Debug.Log($"   • Play Area: {config.playAreaWidth} x {config.playAreaHeight} world units");
        Debug.Log($"   • Boundary Margin: {config.boundaryMargin} world units from edges");
        Debug.Log($"   • Resolution Scaling: {(config.enableResolutionScaling ? "Enabled" : "Disabled")}");
        Debug.Log($"   • Global Bounce Multiplier: {config.globalBounceMultiplier:F1}");
        
        Debug.Log("🔧 Physics Configuration:");
        foreach (BoundaryWall wall in walls)
        {
            BoundaryWallConfig wallConfig = config.GetBoundaryConfig(wall.wallType);
            string physicsInfo = wallConfig.enableCollision ? 
                $"Bounce {wallConfig.bounceCoefficient:F1}" : "No Collision";
            Debug.Log($"   • {wall.wallType} Boundary: {physicsInfo}");
        }
        
        Debug.Log("📐 Positioning System:");
        Debug.Log($"   • Calculated positions based on play area center: {config.playAreaCenter}");
        Debug.Log("   • Automatic wall thickness and margin considerations");
        Debug.Log("   • Support for custom position offsets per boundary");
        Debug.Log("   • Camera-independent positioning system");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log($"   1. Boundary system created as '{boundarySystem.name}' in scene hierarchy");
        Debug.Log("   2. Select individual walls to modify properties in Inspector");
        Debug.Log("   3. Wall positions update automatically when BoundaryConfig changes");
        Debug.Log("   4. Call UpdateWallPosition() on walls for runtime repositioning");
        Debug.Log("   5. Use ValidateWall() methods to check wall configuration");
        
        Debug.Log("🎨 Visualization:");
        Debug.Log("   • Scene view gizmos show boundary positions and dimensions");
        Debug.Log("   • Color coding matches boundary configuration");
        Debug.Log("   • Wire frame display when walls are selected");
        Debug.Log("   • Real-time gizmo updates when configuration changes");
        
        Debug.Log("🔄 Runtime Management:");
        Debug.Log("   • Wall positions recalculate on configuration changes");
        Debug.Log("   • Individual wall enable/disable through configuration");
        Debug.Log("   • Physics material assignment per boundary");
        Debug.Log("   • Collision layer configuration support");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Bottom boundary collision disabled by default (configure for ball loss)");
        Debug.Log("   → Wall colliders sized automatically based on configuration");
        Debug.Log("   → Resolution scaling affects wall dimensions but not collision logic");
        Debug.Log("   → Modify BoundaryConfig asset to adjust all walls simultaneously");
        
        Debug.Log("🔄 Next Steps:");
        Debug.Log("   → Test ball physics with boundary collisions");
        Debug.Log("   → Assign physics materials for different bounce behaviors");
        Debug.Log("   → Configure collision layers for selective interactions");
        Debug.Log("   → Implement ball loss detection for bottom boundary");
        Debug.Log("   → Add visual materials and effects to boundaries");
        
        // Select the boundary system in hierarchy
        Selection.activeGameObject = boundarySystem;
        EditorGUIUtility.PingObject(boundarySystem);
    }
}
#endif