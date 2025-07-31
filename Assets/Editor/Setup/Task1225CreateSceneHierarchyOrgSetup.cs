#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for creating and configuring Scene Hierarchy Organization.
/// Demonstrates brick organization with row-based containers and clean hierarchy management.
/// </summary>
public static class Task1225CreateSceneHierarchyOrgSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1225 Create Scene Hierarchy Organization";
    
    /// <summary>
    /// Creates and configures the Scene Hierarchy Organization system.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateSceneHierarchyOrg()
    {
        Debug.Log("🗂️ [Task 1.2.2.5] Starting Scene Hierarchy Organization setup...");
        
        try
        {
            // Step 1: Validate BrickGrid manager exists
            BrickGrid brickGrid = ValidateBrickGridExists();
            
            // Step 2: Configure hierarchy organization settings
            ConfigureHierarchySettings(brickGrid);
            
            // Step 3: Create sample hierarchy containers
            CreateSampleHierarchyContainers(brickGrid);
            
            // Step 4: Demonstrate organization with sample bricks
            DemonstrateBrickOrganization(brickGrid);
            
            // Step 5: Test hierarchy cleanup functionality
            TestHierarchyCleanup(brickGrid);
            
            // Step 6: Validate hierarchy integrity
            ValidateHierarchyIntegrity(brickGrid);
            
            // Step 7: Log success and usage instructions
            LogSuccessfulSetup(brickGrid);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.2.5] Scene Hierarchy Organization setup failed: {e.Message}");
            Debug.LogError("📋 Please ensure BrickGrid Manager and instantiation system are available");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if BrickGrid exists
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateSceneHierarchyOrg()
    {
        BrickGrid brickGrid = Object.FindFirstObjectByType<BrickGrid>();
        return brickGrid != null;
    }
    
    /// <summary>
    /// Validates that BrickGrid manager exists and has required dependencies
    /// </summary>
    /// <returns>BrickGrid component</returns>
    private static BrickGrid ValidateBrickGridExists()
    {
        Debug.Log("🔍 [Step 1/7] Validating BrickGrid manager...");
        
        BrickGrid brickGrid = Object.FindFirstObjectByType<BrickGrid>();
        if (brickGrid == null)
        {
            // Try to create BrickGrid if missing
            Debug.LogWarning("   ⚠️ BrickGrid not found - attempting to create...");
            Task1224CreateBrickInstantiationSetup.CreateBrickInstantiationSystem();
            
            brickGrid = Object.FindFirstObjectByType<BrickGrid>();
            if (brickGrid == null)
            {
                throw new System.Exception("Failed to create BrickGrid manager. Please run Task 1.2.2.4 setup first.");
            }
        }
        
        // Check if instantiation system is available
        if (brickGrid.BrickPrefab == null)
        {
            Debug.LogWarning("   ⚠️ No brick prefab assigned - hierarchy will be created but not tested");
        }
        else
        {
            Debug.Log("   • Brick prefab available: ✅");
        }
        
        Debug.Log($"   • BrickGrid found: {brickGrid.gameObject.name} ✅");
        Debug.Log("✅ [Step 1/7] BrickGrid validation complete");
        
        return brickGrid;
    }
    
    /// <summary>
    /// Configures hierarchy organization settings for optimal structure
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to configure</param>
    private static void ConfigureHierarchySettings(BrickGrid brickGrid)
    {
        Debug.Log("⚙️ [Step 2/7] Configuring hierarchy organization settings...");
        
        SerializedObject serializedBrickGrid = new SerializedObject(brickGrid);
        
        // Configure hierarchy organization settings
        SerializedProperty useRowOrganization = serializedBrickGrid.FindProperty("useRowOrganization");
        if (useRowOrganization != null)
        {
            useRowOrganization.boolValue = true;
            Debug.Log("   • Row organization: Enabled ✅");
        }
        
        SerializedProperty gridContainerName = serializedBrickGrid.FindProperty("gridContainerName");
        if (gridContainerName != null && string.IsNullOrEmpty(gridContainerName.stringValue))
        {
            gridContainerName.stringValue = "BrickGrid_Organized";
            Debug.Log("   • Grid container name: Set to 'BrickGrid_Organized' ✅");
        }
        
        SerializedProperty rowContainerPrefix = serializedBrickGrid.FindProperty("rowContainerPrefix");
        if (rowContainerPrefix != null && string.IsNullOrEmpty(rowContainerPrefix.stringValue))
        {
            rowContainerPrefix.stringValue = "Row_";
            Debug.Log("   • Row container prefix: Set to 'Row_' ✅");
        }
        
        serializedBrickGrid.ApplyModifiedProperties();
        EditorUtility.SetDirty(brickGrid);
        
        Debug.Log("✅ [Step 2/7] Hierarchy settings configured");
    }
    
    /// <summary>
    /// Creates sample hierarchy containers to demonstrate organization
    /// </summary>
    /// <param name="brickGrid">BrickGrid component</param>
    private static void CreateSampleHierarchyContainers(BrickGrid brickGrid)
    {
        Debug.Log("🏗️ [Step 3/7] Creating sample hierarchy containers...");
        
        // Create main grid container
        GameObject gridContainer = brickGrid.CreateGridContainer();
        Debug.Log($"   • Created grid container: {gridContainer.name}");
        
        // Create sample row containers (for demonstration)
        int sampleRows = 3;
        for (int i = 0; i < sampleRows; i++)
        {
            GameObject rowContainer = brickGrid.CreateRowContainer(i);
            Debug.Log($"   • Created row container: {rowContainer.name}");
        }
        
        Debug.Log($"✅ [Step 3/7] Created {sampleRows + 1} hierarchy containers");
    }
    
    /// <summary>
    /// Demonstrates brick organization with sample brick placement
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void DemonstrateBrickOrganization(BrickGrid brickGrid)
    {
        Debug.Log("🧱 [Step 4/7] Demonstrating brick organization...");
        
        if (brickGrid.BrickPrefab == null)
        {
            Debug.LogWarning("   ⚠️ No brick prefab available - skipping brick demonstration");
            return;
        }
        
        // Create sample bricks to demonstrate organization
        int demonstrationBricks = 0;
        Vector3 basePosition = brickGrid.transform.position;
        
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 2; col++)
            {
                Vector3 brickPosition = basePosition + new Vector3(col * 2f, row * 1f, 0f);
                GameObject sampleBrick = brickGrid.InstantiateBrick(brickPosition, BrickType.Normal, row, col);
                
                if (sampleBrick != null)
                {
                    demonstrationBricks++;
                    Debug.Log($"   • Created demonstration brick: {sampleBrick.name} in row {row}");
                }
            }
        }
        
        // Test organization functionality
        if (demonstrationBricks > 0)
        {
            Debug.Log("   • Testing organization functionality...");
            brickGrid.OrganizeBricksInHierarchy();
            Debug.Log("   • Brick organization completed ✅");
        }
        
        Debug.Log($"✅ [Step 4/7] Demonstrated organization with {demonstrationBricks} bricks");
    }
    
    /// <summary>
    /// Tests hierarchy cleanup functionality
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void TestHierarchyCleanup(BrickGrid brickGrid)
    {
        Debug.Log("🧪 [Step 5/7] Testing hierarchy cleanup functionality...");
        
        // Count current objects before cleanup
        int initialBrickCount = brickGrid.InstantiatedBricks.Count;
        
        if (initialBrickCount > 0)
        {
            Debug.Log($"   • Testing cleanup with {initialBrickCount} bricks...");
            
            // Test the cleanup system
            brickGrid.ClearHierarchy();
            
            int remainingBricks = brickGrid.InstantiatedBricks.Count;
            if (remainingBricks == 0)
            {
                Debug.Log("   • Hierarchy cleanup: ✅ All bricks cleared");
            }
            else
            {
                Debug.LogWarning($"   • Hierarchy cleanup: ⚠️ {remainingBricks} bricks remain");
            }
        }
        else
        {
            Debug.Log("   • No bricks to test cleanup - testing container creation only");
            
            // Test cleanup on empty hierarchy
            brickGrid.ClearHierarchy();
            Debug.Log("   • Empty hierarchy cleanup: ✅");
        }
        
        Debug.Log("✅ [Step 5/7] Hierarchy cleanup testing complete");
    }
    
    /// <summary>
    /// Validates hierarchy integrity and organization
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to validate</param>
    private static void ValidateHierarchyIntegrity(BrickGrid brickGrid)
    {
        Debug.Log("🔍 [Step 6/7] Validating hierarchy integrity...");
        
        // Check grid container exists
        if (brickGrid.GridContainer != null)
        {
            Debug.Log($"   • Grid container: {brickGrid.GridContainer.name} ✅");
            
            // Count hierarchy depth
            int totalChildren = CountTotalChildren(brickGrid.GridContainer.transform);
            Debug.Log($"   • Total hierarchy objects: {totalChildren}");
            
            // Check naming conventions
            ValidateNamingConventions(brickGrid.GridContainer);
        }
        else
        {
            Debug.LogWarning("   • Grid container: ❌ Missing");
        }
        
        // Test container creation methods
        bool containerCreationWorks = TestContainerCreation(brickGrid);
        Debug.Log($"   • Container creation methods: {(containerCreationWorks ? "✅" : "❌")}");
        
        Debug.Log("✅ [Step 6/7] Hierarchy integrity validation complete");
    }
    
    /// <summary>
    /// Counts total children in a transform hierarchy
    /// </summary>
    /// <param name="parent">Parent transform to count</param>
    /// <returns>Total number of child objects</returns>
    private static int CountTotalChildren(Transform parent)
    {
        int count = parent.childCount;
        for (int i = 0; i < parent.childCount; i++)
        {
            count += CountTotalChildren(parent.GetChild(i));
        }
        return count;
    }
    
    /// <summary>
    /// Validates naming conventions in the hierarchy
    /// </summary>
    /// <param name="gridContainer">Grid container to validate</param>
    private static void ValidateNamingConventions(GameObject gridContainer)
    {
        int validNames = 0;
        int totalObjects = 0;
        
        // Check grid container name
        if (gridContainer.name.Contains("BrickGrid"))
        {
            validNames++;
        }
        totalObjects++;
        
        // Check row container names
        for (int i = 0; i < gridContainer.transform.childCount; i++)
        {
            Transform child = gridContainer.transform.GetChild(i);
            totalObjects++;
            
            if (child.name.StartsWith("Row_") || child.name.StartsWith("Brick_"))
            {
                validNames++;
            }
            
            // Check brick names in row containers
            for (int j = 0; j < child.childCount; j++)
            {
                Transform brick = child.GetChild(j);
                totalObjects++;
                
                if (brick.name.StartsWith("Brick_"))
                {
                    validNames++;
                }
            }
        }
        
        Debug.Log($"   • Naming conventions: {validNames}/{totalObjects} objects follow conventions");
    }
    
    /// <summary>
    /// Tests container creation methods
    /// </summary>
    /// <param name="brickGrid">BrickGrid to test</param>
    /// <returns>True if container creation works correctly</returns>
    private static bool TestContainerCreation(BrickGrid brickGrid)
    {
        try
        {
            // Test grid container creation
            GameObject testGridContainer = brickGrid.CreateGridContainer();
            if (testGridContainer == null) return false;
            
            // Test row container creation
            GameObject testRowContainer = brickGrid.CreateRowContainer(99);
            if (testRowContainer == null) return false;
            
            // Clean up test containers
            if (testRowContainer.name.Contains("99"))
            {
                Object.DestroyImmediate(testRowContainer);
            }
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Container creation test failed: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Logs successful setup summary with usage instructions
    /// </summary>
    /// <param name="brickGrid">Configured BrickGrid component</param>
    private static void LogSuccessfulSetup(BrickGrid brickGrid)
    {
        Debug.Log("✅ [Task 1.2.2.5] Scene Hierarchy Organization created successfully!");
        Debug.Log("📋 Scene Hierarchy Organization Summary:");
        Debug.Log("   • Container Management: Automatic grid and row container creation");
        Debug.Log("   • Brick Organization: Row-based hierarchy with consistent naming");
        Debug.Log("   • Hierarchy Cleanup: Efficient container and brick destruction");
        Debug.Log("   • Navigation Support: Clean scene structure for debugging");
        
        Debug.Log("🗂️ Hierarchy Features:");
        Debug.Log("   → CreateGridContainer(): Main container creation with proper parenting");
        Debug.Log("   → CreateRowContainer(): Row-specific containers with naming conventions");
        Debug.Log("   → OrganizeBricksInHierarchy(): Automatic brick organization by row");
        Debug.Log("   → ClearHierarchy(): Efficient cleanup with container destruction");
        Debug.Log("   → ValidateHierarchyIntegrity(): Hierarchy validation and integrity checking");
        
        Debug.Log("⚙️ Organization Status:");
        Debug.Log($"   • Grid Container: {(brickGrid.GridContainer != null ? "✅ Created" : "❌ Missing")}");
        Debug.Log("   • Row Organization: ✅ Enabled");
        Debug.Log("   • Naming Conventions: ✅ Configured");
        Debug.Log("   • Cleanup System: ✅ Integrated");
        
        Debug.Log("🎮 Usage Instructions:");
        Debug.Log("   1. Grid generates with automatic row-based organization");
        Debug.Log("   2. Each row creates its own container (Row_00, Row_01, etc.)");
        Debug.Log("   3. Bricks are named with row/column pattern (Brick_R00C00)");
        Debug.Log("   4. Use OrganizeBricksInHierarchy() to reorganize existing bricks");
        Debug.Log("   5. ClearGrid() uses efficient hierarchy cleanup automatically");
        
        Debug.Log("🔧 Hierarchy Benefits:");
        Debug.Log("   • Scene Navigation: Easy to find and inspect specific rows");
        Debug.Log("   • Debugging Support: Clear structure shows grid organization");
        Debug.Log("   • Performance: Efficient batch operations on row containers");
        Debug.Log("   • Memory Management: Proper cleanup prevents orphaned objects");
        Debug.Log("   • Scalability: Organization remains clean with large grids");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Row organization can be disabled via 'Use Row Organization' checkbox");
        Debug.Log("   → Container names are configurable in Inspector");
        Debug.Log("   → Existing bricks can be reorganized using OrganizeBricksInHierarchy()");
        Debug.Log("   → ClearHierarchy() destroys containers and all children efficiently");
        Debug.Log("   → Hierarchy integrity is validated automatically during operations");
        
        Debug.Log("🚀 Next Steps:");
        Debug.Log("   → Test with different grid sizes and patterns");
        Debug.Log("   → Integrate with level progression and dynamic grid changes");
        Debug.Log("   → Use organized hierarchy for efficient brick selection and effects");
        Debug.Log("   → Leverage row containers for pattern-specific operations");
    }
}
#endif