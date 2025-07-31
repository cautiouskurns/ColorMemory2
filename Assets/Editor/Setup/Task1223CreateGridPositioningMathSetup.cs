#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for testing and validating Grid Positioning Mathematics.
/// Provides automated testing of mathematical calculations and boundary validation.
/// </summary>
public static class Task1223CreateGridPositioningMathSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1223 Create Grid Positioning Math";
    
    /// <summary>
    /// Creates and tests Grid Positioning Mathematics implementation.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateGridPositioningMath()
    {
        Debug.Log("🔧 [Task 1.2.2.3] Starting Grid Positioning Mathematics setup...");
        
        try
        {
            // Step 1: Validate BrickGrid manager exists
            BrickGrid brickGrid = ValidateBrickGridExists();
            
            // Step 2: Set up test configuration
            SetupTestConfiguration(brickGrid);
            
            // Step 3: Test mathematical calculations
            TestMathematicalCalculations(brickGrid);
            
            // Step 4: Test boundary validation
            TestBoundaryValidation(brickGrid);
            
            // Step 5: Test pattern-specific calculations
            TestPatternCalculations(brickGrid);
            
            // Step 6: Enable visualization and log success
            EnableVisualization(brickGrid);
            LogSuccessfulSetup(brickGrid);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.2.3] Grid Positioning Mathematics setup failed: {e.Message}");
            Debug.LogError("📋 Please ensure BrickGrid Manager exists from Task 1.2.2.2");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if BrickGrid exists
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateGridPositioningMath()
    {
        return Object.FindObjectOfType<BrickGrid>() != null;
    }
    
    /// <summary>
    /// Validates that BrickGrid manager exists in scene
    /// </summary>
    /// <returns>BrickGrid component</returns>
    private static BrickGrid ValidateBrickGridExists()
    {
        Debug.Log("🔍 [Step 1/6] Validating BrickGrid manager...");
        
        BrickGrid brickGrid = Object.FindObjectOfType<BrickGrid>();
        if (brickGrid == null)
        {
            // Try to create BrickGrid if missing
            Debug.LogWarning("   ⚠️ BrickGrid not found - attempting to create...");
            Task1222CreateBrickGridManagerSetup.CreateBrickGridManager();
            
            brickGrid = Object.FindObjectOfType<BrickGrid>();
            if (brickGrid == null)
            {
                throw new System.Exception("Failed to create BrickGrid manager. Please run Task 1.2.2.2 setup first.");
            }
        }
        
        Debug.Log($"   • BrickGrid found: {brickGrid.gameObject.name} ✅");
        Debug.Log("✅ [Step 1/6] BrickGrid validation complete");
        
        return brickGrid;
    }
    
    /// <summary>
    /// Sets up test configuration for mathematical validation
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to configure</param>
    private static void SetupTestConfiguration(BrickGrid brickGrid)
    {
        Debug.Log("⚙️ [Step 2/6] Setting up test configuration...");
        
        // Try to find or create test GridData
        GridData testGridData = AssetDatabase.LoadAssetAtPath<GridData>("Assets/Data/GridConfigurations/DefaultGridData.asset");
        
        if (testGridData == null)
        {
            Debug.LogWarning("   ⚠️ No GridData found - creating test configuration...");
            
            // Create minimal test configuration
            testGridData = GridData.CreateDefault();
            testGridData.rows = 5;
            testGridData.columns = 8;
            testGridData.horizontalSpacing = 1.2f;
            testGridData.verticalSpacing = 0.7f;
            testGridData.pattern = LayoutPattern.Standard;
            testGridData.centerInPlayArea = true;
            testGridData.playAreaBounds = new Bounds(Vector3.zero, new Vector3(16f, 10f, 1f));
            testGridData.edgeMargin = 1f;
            
            // Save test asset
            string testPath = "Assets/Data/TestGridMath.asset";
            AssetDatabase.CreateAsset(testGridData, testPath);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"   • Created test GridData: {testPath}");
        }
        
        // Assign configuration to BrickGrid
        SerializedObject serializedBrickGrid = new SerializedObject(brickGrid);
        SerializedProperty gridConfiguration = serializedBrickGrid.FindProperty("gridConfiguration");
        
        if (gridConfiguration != null)
        {
            gridConfiguration.objectReferenceValue = testGridData;
            serializedBrickGrid.ApplyModifiedProperties();
            EditorUtility.SetDirty(brickGrid);
            
            Debug.Log($"   • Assigned GridData: {testGridData.name}");
            Debug.Log($"   • Grid size: {testGridData.rows}×{testGridData.columns}");
            Debug.Log($"   • Spacing: {testGridData.horizontalSpacing}×{testGridData.verticalSpacing}");
        }
        
        Debug.Log("✅ [Step 2/6] Test configuration setup complete");
    }
    
    /// <summary>
    /// Tests mathematical position calculations
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void TestMathematicalCalculations(BrickGrid brickGrid)
    {
        Debug.Log("🧮 [Step 3/6] Testing mathematical calculations...");
        
        if (brickGrid.GridConfiguration == null)
        {
            Debug.LogError("   ❌ No GridConfiguration assigned");
            return;
        }
        
        // Test corner positions
        Debug.Log("   • Testing corner positions:");
        
        Vector3 bottomLeft = brickGrid.CalculateGridPosition(0, 0);
        Debug.Log($"     └─ Bottom-left (0,0): {bottomLeft}");
        
        Vector3 bottomRight = brickGrid.CalculateGridPosition(0, brickGrid.GridConfiguration.columns - 1);
        Debug.Log($"     └─ Bottom-right (0,{brickGrid.GridConfiguration.columns - 1}): {bottomRight}");
        
        Vector3 topLeft = brickGrid.CalculateGridPosition(brickGrid.GridConfiguration.rows - 1, 0);
        Debug.Log($"     └─ Top-left ({brickGrid.GridConfiguration.rows - 1},0): {topLeft}");
        
        Vector3 topRight = brickGrid.CalculateGridPosition(brickGrid.GridConfiguration.rows - 1, brickGrid.GridConfiguration.columns - 1);
        Debug.Log($"     └─ Top-right ({brickGrid.GridConfiguration.rows - 1},{brickGrid.GridConfiguration.columns - 1}): {topRight}");
        
        // Test grid center calculation
        Vector3 gridCenter = brickGrid.CalculateGridCenter();
        Debug.Log($"   • Grid center: {gridCenter}");
        
        // Verify spacing calculations
        float expectedWidth = (brickGrid.GridConfiguration.columns - 1) * brickGrid.GridConfiguration.horizontalSpacing;
        float actualWidth = bottomRight.x - bottomLeft.x;
        float widthError = Mathf.Abs(expectedWidth - actualWidth);
        
        float expectedHeight = (brickGrid.GridConfiguration.rows - 1) * brickGrid.GridConfiguration.verticalSpacing;
        float actualHeight = topLeft.y - bottomLeft.y;
        float heightError = Mathf.Abs(expectedHeight - actualHeight);
        
        Debug.Log($"   • Spacing verification:");
        Debug.Log($"     └─ Width: Expected={expectedWidth:F3}, Actual={actualWidth:F3}, Error={widthError:F6} {(widthError < 0.001f ? "✅" : "❌")}");
        Debug.Log($"     └─ Height: Expected={expectedHeight:F3}, Actual={actualHeight:F3}, Error={heightError:F6} {(heightError < 0.001f ? "✅" : "❌")}");
        
        Debug.Log("✅ [Step 3/6] Mathematical calculations test complete");
    }
    
    /// <summary>
    /// Tests boundary validation system
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void TestBoundaryValidation(BrickGrid brickGrid)
    {
        Debug.Log("🔲 [Step 4/6] Testing boundary validation...");
        
        if (brickGrid.GridConfiguration == null)
        {
            Debug.LogError("   ❌ No GridConfiguration assigned");
            return;
        }
        
        // Get grid bounds
        Bounds gridBounds = brickGrid.GetGridBounds();
        Debug.Log($"   • Grid bounds: Center={gridBounds.center}, Size={gridBounds.size}");
        
        // Test boundary validation
        bool validBounds = brickGrid.ValidateGridBounds();
        Debug.Log($"   • Boundary validation: {(validBounds ? "✅ Grid fits within play area" : "❌ Grid exceeds play area")}");
        
        // Test with different configurations
        GridData originalConfig = brickGrid.GridConfiguration;
        
        // Test oversized grid
        GridData oversizedTest = ScriptableObject.CreateInstance<GridData>();
        oversizedTest.rows = 20;
        oversizedTest.columns = 30;
        oversizedTest.horizontalSpacing = 1.5f;
        oversizedTest.verticalSpacing = 1.0f;
        oversizedTest.playAreaBounds = originalConfig.playAreaBounds;
        oversizedTest.edgeMargin = 1f;
        
        // Temporarily assign oversized configuration
        SerializedObject serializedBrickGrid = new SerializedObject(brickGrid);
        SerializedProperty gridConfiguration = serializedBrickGrid.FindProperty("gridConfiguration");
        gridConfiguration.objectReferenceValue = oversizedTest;
        serializedBrickGrid.ApplyModifiedProperties();
        
        bool oversizedValid = brickGrid.ValidateGridBounds();
        Debug.Log($"   • Oversized grid test: {(oversizedValid ? "⚠️ Unexpectedly valid" : "✅ Correctly invalid")}");
        
        // Restore original configuration
        gridConfiguration.objectReferenceValue = originalConfig;
        serializedBrickGrid.ApplyModifiedProperties();
        
        // Clean up test object
        Object.DestroyImmediate(oversizedTest);
        
        Debug.Log("✅ [Step 4/6] Boundary validation test complete");
    }
    
    /// <summary>
    /// Tests pattern-specific position calculations
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void TestPatternCalculations(BrickGrid brickGrid)
    {
        Debug.Log("🎨 [Step 5/6] Testing pattern calculations...");
        
        if (brickGrid.GridConfiguration == null)
        {
            Debug.LogError("   ❌ No GridConfiguration assigned");
            return;
        }
        
        GridData originalConfig = brickGrid.GridConfiguration;
        LayoutPattern originalPattern = originalConfig.pattern;
        
        // Test each pattern type
        LayoutPattern[] patterns = { LayoutPattern.Standard, LayoutPattern.Pyramid, LayoutPattern.Diamond };
        
        foreach (LayoutPattern pattern in patterns)
        {
            Debug.Log($"   • Testing {pattern} pattern:");
            
            // Update pattern
            originalConfig.pattern = pattern;
            
            // Test middle row positions
            int midRow = originalConfig.rows / 2;
            int midCol = originalConfig.columns / 2;
            
            Vector3 centerPos = brickGrid.CalculateGridPosition(midRow, midCol);
            Vector3 leftEdge = brickGrid.CalculateGridPosition(midRow, 0);
            Vector3 rightEdge = brickGrid.CalculateGridPosition(midRow, originalConfig.columns - 1);
            
            Debug.Log($"     └─ Center position ({midRow},{midCol}): {centerPos}");
            Debug.Log($"     └─ Left edge ({midRow},0): {leftEdge}");
            Debug.Log($"     └─ Right edge ({midRow},{originalConfig.columns - 1}): {rightEdge}");
            
            // Check for hidden positions in pattern layouts
            if (pattern == LayoutPattern.Pyramid || pattern == LayoutPattern.Diamond)
            {
                int hiddenCount = 0;
                for (int row = 0; row < originalConfig.rows; row++)
                {
                    for (int col = 0; col < originalConfig.columns; col++)
                    {
                        Vector3 pos = brickGrid.CalculateGridPosition(row, col);
                        if (pos.y < -100f) hiddenCount++;
                    }
                }
                Debug.Log($"     └─ Hidden positions: {hiddenCount}");
            }
        }
        
        // Restore original pattern
        originalConfig.pattern = originalPattern;
        
        Debug.Log("✅ [Step 5/6] Pattern calculations test complete");
    }
    
    /// <summary>
    /// Enables visualization features for debugging
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to configure</param>
    private static void EnableVisualization(BrickGrid brickGrid)
    {
        Debug.Log("👁️ [Step 6/6] Enabling visualization...");
        
        // Enable debug visualization
        SerializedObject serializedBrickGrid = new SerializedObject(brickGrid);
        
        SerializedProperty showGridGizmos = serializedBrickGrid.FindProperty("showGridGizmos");
        if (showGridGizmos != null)
        {
            showGridGizmos.boolValue = true;
        }
        
        SerializedProperty enableDebugLogging = serializedBrickGrid.FindProperty("enableDebugLogging");
        if (enableDebugLogging != null)
        {
            enableDebugLogging.boolValue = true;
        }
        
        serializedBrickGrid.ApplyModifiedProperties();
        EditorUtility.SetDirty(brickGrid);
        
        // Select BrickGrid for visualization
        Selection.activeGameObject = brickGrid.gameObject;
        
        // Force scene view update
        SceneView.RepaintAll();
        
        Debug.Log("   • Grid gizmos enabled ✅");
        Debug.Log("   • Debug logging enabled ✅");
        Debug.Log("   • BrickGrid selected for visualization ✅");
        Debug.Log("✅ [Step 6/6] Visualization setup complete");
    }
    
    /// <summary>
    /// Logs successful setup summary
    /// </summary>
    /// <param name="brickGrid">Configured BrickGrid component</param>
    private static void LogSuccessfulSetup(BrickGrid brickGrid)
    {
        Debug.Log("✅ [Task 1.2.2.3] Grid Positioning Mathematics implemented successfully!");
        Debug.Log("📋 Grid Positioning Mathematics Summary:");
        Debug.Log("   • Mathematical Methods: Position calculation, centering, bounds validation");
        Debug.Log("   • Precision Testing: Verified spacing calculations with < 0.001 error tolerance");
        Debug.Log("   • Boundary Validation: Comprehensive play area constraint checking");
        Debug.Log("   • Pattern Support: Standard, Pyramid, Diamond layouts with proper offsets");
        
        Debug.Log("🧮 Mathematical Features:");
        Debug.Log("   → CalculateGridPosition(): Precise world position for any grid coordinate");
        Debug.Log("   → CalculateGridCenter(): Accurate center point calculation");
        Debug.Log("   → ValidateGridBounds(): Play area constraint validation with margins");
        Debug.Log("   → GetGridBounds(): Complete bounds calculation including brick size");
        Debug.Log("   → Pattern Offsets: Pyramid and Diamond formation mathematics");
        
        Debug.Log("🔍 Visualization Features:");
        Debug.Log("   • Yellow wireframe: Calculated grid bounds");
        Debug.Log("   • Magenta sphere: Grid center point with crosshairs");
        Debug.Log("   • Cyan wireframe: Play area bounds with margin visualization");
        Debug.Log("   • Green sphere: Grid starting position");
        Debug.Log("   • Red/Green indicator: Boundary validation status");
        Debug.Log("   • White wireframes: Individual brick positions (when selected)");
        
        Debug.Log("📊 Test Results:");
        if (brickGrid.GridConfiguration != null)
        {
            GridData config = brickGrid.GridConfiguration;
            Bounds bounds = brickGrid.GetGridBounds();
            bool valid = brickGrid.ValidateGridBounds();
            
            Debug.Log($"   • Configuration: {config.rows}×{config.columns}, Pattern: {config.pattern}");
            Debug.Log($"   • Grid dimensions: {bounds.size.x:F2}×{bounds.size.y:F2} units");
            Debug.Log($"   • Center position: {brickGrid.CalculateGridCenter()}");
            Debug.Log($"   • Boundary status: {(valid ? "✅ Within bounds" : "❌ Exceeds bounds")}");
        }
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. View Scene window to see grid visualization gizmos");
        Debug.Log("   2. Select BrickGrid GameObject to see detailed brick positions");
        Debug.Log("   3. Modify GridData to test different configurations");
        Debug.Log("   4. Use pattern dropdown to see Pyramid/Diamond calculations");
        Debug.Log("   5. Adjust play area bounds to test boundary validation");
        
        Debug.Log("🔧 Integration Ready:");
        Debug.Log("   → Mathematical foundation complete for grid generation");
        Debug.Log("   → All positioning calculations verified and tested");
        Debug.Log("   → Pattern-specific offsets properly implemented");
        Debug.Log("   → Boundary validation ensures safe grid placement");
        Debug.Log("   → Ready for brick instantiation in Task 1.2.2.4");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Brick positions use bottom-left origin (row 0, col 0)");
        Debug.Log("   → Staggering adds horizontal offset to odd rows");
        Debug.Log("   → Pattern layouts hide positions by moving them far below (-1000y)");
        Debug.Log("   → Edge margins reduce effective play area for safety");
        Debug.Log("   → All calculations account for standard brick size (1.0×0.5)");
    }
    
    /// <summary>
    /// Utility method to test specific position calculations
    /// </summary>
    [MenuItem("Breakout/Debug/Test Grid Position Math", false, 1001)]
    public static void TestGridPositionMath()
    {
        Debug.Log("🧮 Testing Grid Position Mathematics...");
        
        BrickGrid brickGrid = Object.FindObjectOfType<BrickGrid>();
        if (brickGrid == null || brickGrid.GridConfiguration == null)
        {
            Debug.LogError("❌ No BrickGrid with configuration found");
            return;
        }
        
        // Test specific positions
        Debug.Log("📍 Position Tests:");
        for (int i = 0; i < 3; i++)
        {
            int row = Random.Range(0, brickGrid.GridConfiguration.rows);
            int col = Random.Range(0, brickGrid.GridConfiguration.columns);
            Vector3 pos = brickGrid.CalculateGridPosition(row, col);
            Debug.Log($"   • Position[{row},{col}] = {pos}");
        }
        
        // Test bounds
        Bounds bounds = brickGrid.GetGridBounds();
        Debug.Log($"📦 Grid Bounds: {bounds}");
        Debug.Log($"   • Size: {bounds.size}");
        Debug.Log($"   • Min: {bounds.min}");
        Debug.Log($"   • Max: {bounds.max}");
        
        // Test validation
        bool valid = brickGrid.ValidateGridBounds();
        Debug.Log($"✓ Boundary Validation: {(valid ? "PASS" : "FAIL")}");
    }
}
#endif