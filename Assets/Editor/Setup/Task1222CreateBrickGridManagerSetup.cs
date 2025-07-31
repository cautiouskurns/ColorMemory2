#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for creating BrickGrid Manager GameObject with component configuration.
/// Provides automated setup for BrickGrid MonoBehaviour with GridData integration and scene hierarchy management.
/// </summary>
public static class Task1222CreateBrickGridManagerSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1222 Create BrickGrid Manager";
    private const string BRICKGRID_GAMEOBJECT_NAME = "BrickGrid_Manager";
    
    /// <summary>
    /// Creates BrickGrid Manager GameObject with configured BrickGrid component.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateBrickGridManager()
    {
        Debug.Log("🔧 [Task 1.2.2.2] Starting BrickGrid Manager creation...");
        
        try
        {
            // Step 1: Validate dependencies and prerequisites
            ValidateDependencies();
            
            // Step 2: Check for existing BrickGrid in scene
            CheckForExistingBrickGrid();
            
            // Step 3: Create BrickGrid GameObject
            GameObject brickGridObject = CreateBrickGridGameObject();
            
            // Step 4: Add and configure BrickGrid component
            BrickGrid brickGridComponent = ConfigureBrickGridComponent(brickGridObject);
            
            // Step 5: Setup GridData configuration
            SetupGridDataConfiguration(brickGridComponent);
            
            // Step 6: Position and organize in scene hierarchy
            PositionInSceneHierarchy(brickGridObject);
            
            // Step 7: Validate setup and log success
            ValidateSetup(brickGridComponent);
            LogSuccessfulSetup(brickGridObject, brickGridComponent);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.2.2] BrickGrid Manager creation failed: {e.Message}");
            Debug.LogError("📋 Please ensure GridData structures are available from Task 1.2.2.1");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if BrickGrid doesn't already exist in scene
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateBrickGridManager()
    {
        // Check if BrickGrid already exists in scene
        BrickGrid existingBrickGrid = Object.FindObjectOfType<BrickGrid>();
        return existingBrickGrid == null;
    }
    
    /// <summary>
    /// Validates that all dependencies are available for BrickGrid Manager creation
    /// </summary>
    private static void ValidateDependencies()
    {
        Debug.Log("🔍 [Step 1/7] Validating dependencies...");
        
        // Check if GridData structures are available
        try
        {
            System.Type gridDataType = typeof(GridData);
            System.Type layoutPatternType = typeof(LayoutPattern);
            
            if (gridDataType != null && layoutPatternType != null)
            {
                Debug.Log("   • GridData structures: Available ✅");
            }
            
            // Test GridData creation
            GridData testGrid = GridData.CreateDefault();
            if (testGrid != null && testGrid.ValidateConfiguration())
            {
                Debug.Log("   • GridData functionality: Working ✅");
                
                // Clean up test instance
                Object.DestroyImmediate(testGrid);
            }
        }
        catch (System.Exception e)
        {
            throw new System.Exception($"GridData structures validation failed: {e.Message}. Please run Task 1.2.2.1 setup first.");
        }
        
        // Check if BrickGrid script compiles properly
        try
        {
            System.Type brickGridType = typeof(BrickGrid);
            if (brickGridType != null)
            {
                Debug.Log("   • BrickGrid MonoBehaviour: Available ✅");
            }
        }
        catch (System.Exception e)
        {
            throw new System.Exception($"BrickGrid script validation failed: {e.Message}");
        }
        
        Debug.Log("✅ [Step 1/7] Dependencies validated successfully");
    }
    
    /// <summary>
    /// Checks for existing BrickGrid in scene and handles conflicts
    /// </summary>
    private static void CheckForExistingBrickGrid()
    {
        Debug.Log("🔍 [Step 2/7] Checking for existing BrickGrid...");
        
        BrickGrid existingBrickGrid = Object.FindObjectOfType<BrickGrid>();
        if (existingBrickGrid != null)
        {
            throw new System.Exception($"BrickGrid already exists in scene: {existingBrickGrid.gameObject.name}. Please remove existing BrickGrid or use the existing one.");
        }
        
        // Check for GameObject with the same name
        GameObject existingObject = GameObject.Find(BRICKGRID_GAMEOBJECT_NAME);
        if (existingObject != null)
        {
            Debug.LogWarning($"   ⚠️ GameObject named '{BRICKGRID_GAMEOBJECT_NAME}' already exists");
            Debug.LogWarning("   📋 Creating with unique name to avoid conflicts");
        }
        
        Debug.Log("✅ [Step 2/7] Scene validation complete");
    }
    
    /// <summary>
    /// Creates the BrickGrid GameObject with proper naming
    /// </summary>
    /// <returns>Created GameObject</returns>
    private static GameObject CreateBrickGridGameObject()
    {
        Debug.Log("🏗️ [Step 3/7] Creating BrickGrid GameObject...");
        
        // Generate unique name if needed
        string objectName = BRICKGRID_GAMEOBJECT_NAME;
        GameObject existingObject = GameObject.Find(objectName);
        
        int counter = 1;
        while (existingObject != null)
        {
            objectName = $"{BRICKGRID_GAMEOBJECT_NAME}_{counter}";
            existingObject = GameObject.Find(objectName);
            counter++;
        }
        
        // Create GameObject
        GameObject brickGridObject = new GameObject(objectName);
        
        Debug.Log($"   • Created GameObject: {objectName}");
        Debug.Log("✅ [Step 3/7] GameObject creation complete");
        
        return brickGridObject;
    }
    
    /// <summary>
    /// Adds and configures the BrickGrid component
    /// </summary>
    /// <param name="brickGridObject">GameObject to add component to</param>
    /// <returns>Configured BrickGrid component</returns>
    private static BrickGrid ConfigureBrickGridComponent(GameObject brickGridObject)
    {
        Debug.Log("⚙️ [Step 4/7] Configuring BrickGrid component...");
        
        // Add BrickGrid component
        BrickGrid brickGridComponent = brickGridObject.AddComponent<BrickGrid>();
        
        if (brickGridComponent == null)
        {
            throw new System.Exception("Failed to add BrickGrid component to GameObject");
        }
        
        // Configure component using SerializedObject for editor-time setup
        SerializedObject serializedBrickGrid = new SerializedObject(brickGridComponent);
        
        // Enable debug logging for initial testing
        SerializedProperty enableDebugLogging = serializedBrickGrid.FindProperty("enableDebugLogging");
        if (enableDebugLogging != null)
        {
            enableDebugLogging.boolValue = true;
        }
        
        // Enable grid gizmos for visual debugging
        SerializedProperty showGridGizmos = serializedBrickGrid.FindProperty("showGridGizmos");
        if (showGridGizmos != null)
        {
            showGridGizmos.boolValue = true;
        }
        
        // Apply changes
        serializedBrickGrid.ApplyModifiedProperties();
        EditorUtility.SetDirty(brickGridComponent);
        
        Debug.Log("   • Added BrickGrid component");
        Debug.Log("   • Configured debug settings");
        Debug.Log("✅ [Step 4/7] Component configuration complete");
        
        return brickGridComponent;
    }
    
    /// <summary>
    /// Sets up GridData configuration for the BrickGrid component
    /// </summary>
    /// <param name="brickGridComponent">BrickGrid component to configure</param>
    private static void SetupGridDataConfiguration(BrickGrid brickGridComponent)
    {
        Debug.Log("📋 [Step 5/7] Setting up GridData configuration...");
        
        // Try to find existing GridData assets
        string[] gridDataPaths = {
            "Assets/Data/GridConfigurations/DefaultGridData.asset",
            "Assets/Data/DefaultGridData.asset"
        };
        
        GridData foundGridData = null;
        string foundPath = null;
        
        foreach (string path in gridDataPaths)
        {
            GridData gridData = AssetDatabase.LoadAssetAtPath<GridData>(path);
            if (gridData != null)
            {
                foundGridData = gridData;
                foundPath = path;
                break;
            }
        }
        
        if (foundGridData != null)
        {
            // Assign found GridData asset
            SerializedObject serializedBrickGrid = new SerializedObject(brickGridComponent);
            SerializedProperty gridConfiguration = serializedBrickGrid.FindProperty("gridConfiguration");
            
            if (gridConfiguration != null)
            {
                gridConfiguration.objectReferenceValue = foundGridData;
                serializedBrickGrid.ApplyModifiedProperties();
                EditorUtility.SetDirty(brickGridComponent);
                
                Debug.Log($"   • Assigned GridData: {foundPath}");
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️ No GridData assets found");
            Debug.LogWarning("   📋 Run 'Breakout/Setup/Task1221 Create Grid Data Structures' to create GridData assets");
            Debug.LogWarning("   📋 Or manually assign GridData in Inspector");
        }
        
        Debug.Log("✅ [Step 5/7] GridData configuration setup complete");
    }
    
    /// <summary>
    /// Positions the BrickGrid GameObject appropriately in scene hierarchy
    /// </summary>
    /// <param name="brickGridObject">BrickGrid GameObject to position</param>
    private static void PositionInSceneHierarchy(GameObject brickGridObject)
    {
        Debug.Log("📍 [Step 6/7] Positioning in scene hierarchy...");
        
        // Position at origin for consistent setup
        brickGridObject.transform.position = Vector3.zero;
        brickGridObject.transform.rotation = Quaternion.identity;
        brickGridObject.transform.localScale = Vector3.one;
        
        // Try to find or create a Game Management parent
        GameObject gameManager = GameObject.Find("Game_Manager");
        if (gameManager == null)
        {
            gameManager = GameObject.Find("GameManager");
        }
        
        if (gameManager != null)
        {
            brickGridObject.transform.SetParent(gameManager.transform);
            Debug.Log($"   • Parented to: {gameManager.name}");
        }
        else
        {
            Debug.Log("   • Positioned at scene root (no GameManager found)");
        }
        
        // Select the created GameObject for easy inspection
        Selection.activeGameObject = brickGridObject;
        
        Debug.Log("   • Positioned at origin");
        Debug.Log("   • Selected for inspection");
        Debug.Log("✅ [Step 6/7] Hierarchy positioning complete");
    }
    
    /// <summary>
    /// Validates the complete setup
    /// </summary>
    /// <param name="brickGridComponent">BrickGrid component to validate</param>
    private static void ValidateSetup(BrickGrid brickGridComponent)
    {
        Debug.Log("🧪 [Step 7/7] Validating setup...");
        
        // Validate component initialization
        if (brickGridComponent == null)
        {
            throw new System.Exception("BrickGrid component is null after setup");
        }
        
        // Check configuration validation
        bool configValid = brickGridComponent.ValidateConfiguration();
        if (configValid)
        {
            Debug.Log("   • Configuration validation: ✅");
        }
        else
        {
            Debug.LogWarning("   • Configuration validation: ⚠️ (GridData may need assignment)");
        }
        
        // Validate component properties
        if (brickGridComponent.GridContainer != null)
        {
            Debug.Log("   • Grid container: ✅");
        }
        
        Debug.Log("✅ [Step 7/7] Setup validation complete");
    }
    
    /// <summary>
    /// Logs successful setup summary
    /// </summary>
    /// <param name="brickGridObject">Created GameObject</param>
    /// <param name="brickGridComponent">Configured component</param>
    private static void LogSuccessfulSetup(GameObject brickGridObject, BrickGrid brickGridComponent)
    {
        Debug.Log("✅ [Task 1.2.2.2] BrickGrid Manager creation completed successfully!");
        Debug.Log("📋 BrickGrid Manager Summary:");
        Debug.Log($"   • GameObject: {brickGridObject.name} created in scene");
        Debug.Log("   • BrickGrid Component: Added and configured with framework methods");
        Debug.Log("   • Unity Integration: Inspector sections organized with headers and tooltips");
        Debug.Log("   • State Management: Grid generation status, brick counting, completion detection");
        
        Debug.Log("🔧 Component Features:");
        Debug.Log("   → GridData Integration: Configuration system with validation and error handling");
        Debug.Log("   → Lifecycle Management: Proper Awake/Start initialization with component caching");
        Debug.Log("   → Framework Methods: GenerateGrid(), ClearGrid(), ValidateGrid() stubs prepared");
        Debug.Log("   → State Tracking: Grid generation status, active brick count, completion detection");
        Debug.Log("   → Hierarchy Management: Automatic grid container creation and parenting");
        Debug.Log("   → Debug System: Comprehensive logging with configurable debug levels");
        
        Debug.Log("⚙️ Inspector Configuration:");
        Debug.Log("   • Grid Configuration: GridData asset reference with validation");
        Debug.Log("   • Runtime State: Grid generated status, brick count, completion status");
        Debug.Log("   • Hierarchy Management: Grid container reference for organization");
        Debug.Log("   • Debug Settings: Enable logging and gizmo visualization options");
        
        Debug.Log("🎮 Framework Integration:");
        Debug.Log("   • GridData Compatibility: Full integration with Task 1.2.2.1 data structures");
        Debug.Log("   • Brick System Ready: Prepared for integration with brick prefabs from Task 1.2.1.7");
        Debug.Log("   • Scene Management: Automatic hierarchy organization and positioning");
        Debug.Log("   • Validation System: Configuration validation with clear error reporting");
        
        Debug.Log("💡 Next Steps:");
        Debug.Log("   1. Assign GridData asset in Inspector (if not automatically assigned)");
        Debug.Log("   2. Configure grid parameters using GridData configuration");
        Debug.Log("   3. Implement GenerateGrid() method in future tasks for brick instantiation");
        Debug.Log("   4. Test grid visualization using Scene view gizmos");
        Debug.Log("   5. Integrate with level management and game state systems");
        
        Debug.Log("🔧 Usage Instructions:");
        Debug.Log("   • Inspector Setup: Configure GridData reference and debug options");
        Debug.Log("   • Scene Gizmos: Enable 'Show Grid Gizmos' to visualize grid layout");
        Debug.Log("   • Debug Logging: Enable 'Enable Debug Logging' for detailed operation logs");
        Debug.Log("   • Grid Generation: Call GenerateGrid() when implementation is complete");
        Debug.Log("   • State Monitoring: Use IsGridGenerated, ActiveBrickCount, IsComplete properties");
        
        Debug.Log("📊 Current Status:");
        Debug.Log($"   • Initialized: {brickGridComponent.IsInitialized}");
        Debug.Log($"   • Configuration Valid: {brickGridComponent.ValidateConfiguration()}");
        Debug.Log($"   • GridData Assigned: {(brickGridComponent.GridConfiguration != null ? "✅" : "❌ - Assign in Inspector")}");
        Debug.Log($"   • Grid Container: {(brickGridComponent.GridContainer != null ? "✅" : "❌")}");
        Debug.Log($"   • Ready for Generation: {brickGridComponent.ValidateGridGeneration()}");
        
        Debug.Log("⚠️ Framework Notes:");
        Debug.Log("   → GenerateGrid(), ClearGrid(), ValidateGrid() are framework stubs");
        Debug.Log("   → Full implementation will be added in future grid generation tasks");
        Debug.Log("   → GridData asset assignment may be required for full validation");
        Debug.Log("   → Scene gizmos show grid layout when GridData is properly configured");
        Debug.Log("   → Debug logging provides detailed information about manager state and operations");
    }
    
    /// <summary>
    /// Utility method to clean up BrickGrid GameObjects for fresh testing
    /// </summary>
    [MenuItem("Breakout/Setup/Clean BrickGrid Managers", false, 1001)]
    public static void CleanBrickGridManagers()
    {
        Debug.Log("🧹 [Cleanup] Removing BrickGrid Manager GameObjects...");
        
        BrickGrid[] brickGrids = Object.FindObjectsOfType<BrickGrid>();
        int removedCount = 0;
        
        foreach (BrickGrid brickGrid in brickGrids)
        {
            if (brickGrid != null && brickGrid.gameObject != null)
            {
                string objectName = brickGrid.gameObject.name;
                Object.DestroyImmediate(brickGrid.gameObject);
                removedCount++;
                Debug.Log($"   • Removed: {objectName}");
            }
        }
        
        Debug.Log($"🧹 [Cleanup] Removed {removedCount} BrickGrid Manager GameObjects");
    }
}
#endif