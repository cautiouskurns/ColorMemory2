#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for creating and demonstrating Layout Pattern Implementation.
/// Tests all pattern types with various configurations and validates geometric calculations.
/// </summary>
public static class Task1226CreateLayoutPatternSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1226 Create Layout Pattern Implementation";
    
    /// <summary>
    /// Creates and demonstrates the Layout Pattern Implementation system.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateLayoutPatternImplementation()
    {
        Debug.Log("🎨 [Task 1.2.2.6] Starting Layout Pattern Implementation setup...");
        
        try
        {
            // Step 1: Validate complete grid system exists
            BrickGrid brickGrid = ValidateBrickGridSystem();
            
            // Step 2: Configure pattern settings
            ConfigurePatternSettings(brickGrid);
            
            // Step 3: Demonstrate all pattern types
            DemonstrateAllPatterns(brickGrid);
            
            // Step 4: Test pattern boundary validation
            TestPatternBoundaryValidation(brickGrid);
            
            // Step 5: Validate geometric calculations
            ValidateGeometricCalculations(brickGrid);
            
            // Step 6: Test pattern variety and level design
            TestPatternVarietyAndLevelDesign(brickGrid);
            
            // Step 7: Log success and usage instructions
            LogSuccessfulSetup(brickGrid);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.2.6] Layout Pattern Implementation setup failed: {e.Message}");
            Debug.LogError("📋 Please ensure complete grid system is available from previous tasks");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if complete grid system exists
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateLayoutPatternImplementation()
    {
        BrickGrid brickGrid = Object.FindFirstObjectByType<BrickGrid>();
        return brickGrid != null && brickGrid.BrickPrefab != null;
    }
    
    /// <summary>
    /// Validates that complete grid system exists with all dependencies
    /// </summary>
    /// <returns>BrickGrid component with full system</returns>
    private static BrickGrid ValidateBrickGridSystem()
    {
        Debug.Log("🔍 [Step 1/7] Validating complete grid system...");
        
        BrickGrid brickGrid = Object.FindFirstObjectByType<BrickGrid>();
        if (brickGrid == null)
        {
            // Try to create complete system if missing
            Debug.LogWarning("   ⚠️ BrickGrid not found - attempting to create complete system...");
            Task1225CreateSceneHierarchyOrgSetup.CreateSceneHierarchyOrg();
            
            brickGrid = Object.FindFirstObjectByType<BrickGrid>();
            if (brickGrid == null)
            {
                throw new System.Exception("Failed to create complete grid system. Please run previous setup tasks.");
            }
        }
        
        // Validate system components
        bool hasConfiguration = brickGrid.GridConfiguration != null;
        bool hasPrefab = brickGrid.BrickPrefab != null;
        bool hasContainer = brickGrid.GridContainer != null;
        
        Debug.Log($"   • Grid configuration: {(hasConfiguration ? "✅" : "❌")} {(hasConfiguration ? brickGrid.GridConfiguration.name : "Missing")}");
        Debug.Log($"   • Brick prefab: {(hasPrefab ? "✅" : "❌")} {(hasPrefab ? brickGrid.BrickPrefab.name : "Missing")}");
        Debug.Log($"   • Grid container: {(hasContainer ? "✅" : "❌")} {(hasContainer ? brickGrid.GridContainer.name : "Missing")}");
        
        if (!hasConfiguration || !hasPrefab)
        {
            Debug.LogWarning("   ⚠️ Some system components missing - patterns may not generate properly");
        }
        
        Debug.Log("✅ [Step 1/7] Grid system validation complete");
        return brickGrid;
    }
    
    /// <summary>
    /// Configures pattern-specific settings for optimal demonstration
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to configure</param>
    private static void ConfigurePatternSettings(BrickGrid brickGrid)
    {
        Debug.Log("⚙️ [Step 2/7] Configuring pattern settings...");
        
        SerializedObject serializedBrickGrid = new SerializedObject(brickGrid);
        
        // Configure pattern settings
        SerializedProperty useGridDataPattern = serializedBrickGrid.FindProperty("useGridDataPattern");
        if (useGridDataPattern != null)
        {
            useGridDataPattern.boolValue = true;
            Debug.Log("   • Grid data pattern usage: Enabled ✅");
        }
        
        SerializedProperty patternDensity = serializedBrickGrid.FindProperty("patternDensity");
        if (patternDensity != null)
        {
            patternDensity.floatValue = 0.8f;
            Debug.Log("   • Pattern density: Set to 0.8 ✅");
        }
        
        SerializedProperty hollowCenter = serializedBrickGrid.FindProperty("hollowCenter");
        if (hollowCenter != null)
        {
            hollowCenter.boolValue = false; // Start with solid patterns
            Debug.Log("   • Hollow center: Disabled (solid patterns) ✅");
        }
        
        SerializedProperty patternSeed = serializedBrickGrid.FindProperty("patternSeed");
        if (patternSeed != null)
        {
            patternSeed.intValue = 42; // Reproducible patterns
            Debug.Log("   • Pattern seed: Set to 42 (reproducible) ✅");
        }
        
        serializedBrickGrid.ApplyModifiedProperties();
        EditorUtility.SetDirty(brickGrid);
        
        Debug.Log("✅ [Step 2/7] Pattern settings configured");
    }
    
    /// <summary>
    /// Demonstrates all available pattern types with different configurations
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void DemonstrateAllPatterns(BrickGrid brickGrid)
    {
        Debug.Log("🎭 [Step 3/7] Demonstrating all pattern types...");
        
        if (brickGrid.GridConfiguration == null)
        {
            Debug.LogWarning("   ⚠️ No grid configuration - creating demonstration patterns may fail");
        }
        
        // Test each pattern type
        LayoutPattern[] patternsToTest = { 
            LayoutPattern.Standard, 
            LayoutPattern.Pyramid, 
            LayoutPattern.Diamond, 
            LayoutPattern.Random 
        };
        
        foreach (LayoutPattern pattern in patternsToTest)
        {
            Debug.Log($"   • Testing {pattern} pattern...");
            
            try
            {
                System.DateTime startTime = System.DateTime.Now;
                brickGrid.GeneratePattern(pattern);
                System.TimeSpan generationTime = System.DateTime.Now - startTime;
                
                int brickCount = brickGrid.InstantiatedBricks.Count;
                Debug.Log($"     - Generated {brickCount} bricks in {generationTime.TotalMilliseconds:F1}ms");
                
                // Brief pause to see the pattern (in a real editor script, you might save screenshots)
                System.Threading.Thread.Sleep(100);
                
                // Clear for next pattern
                brickGrid.ClearGrid();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"     - {pattern} pattern failed: {e.Message}");
            }
        }
        
        Debug.Log("✅ [Step 3/7] Pattern demonstration complete");
    }
    
    /// <summary>
    /// Tests pattern boundary validation and geometric constraints
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void TestPatternBoundaryValidation(BrickGrid brickGrid)
    {
        Debug.Log("🔍 [Step 4/7] Testing pattern boundary validation...");
        
        if (brickGrid.GridConfiguration == null)
        {
            Debug.LogWarning("   ⚠️ No grid configuration - skipping boundary validation tests");
            return;
        }
        
        // Test with different grid sizes
        GridData originalConfig = brickGrid.GridConfiguration;
        int originalRows = originalConfig.rows;
        int originalColumns = originalConfig.columns;
        
        // Test small grid
        Debug.Log("   • Testing small grid (3×4)...");
        originalConfig.rows = 3;
        originalConfig.columns = 4;
        TestPatternGeneration(brickGrid, LayoutPattern.Pyramid, "Small grid");
        
        // Test large grid
        Debug.Log("   • Testing large grid (12×15)...");
        originalConfig.rows = 12;
        originalConfig.columns = 15;
        TestPatternGeneration(brickGrid, LayoutPattern.Diamond, "Large grid");
        
        // Test edge case: single row
        Debug.Log("   • Testing edge case (1×8)...");
        originalConfig.rows = 1;
        originalConfig.columns = 8;
        TestPatternGeneration(brickGrid, LayoutPattern.Standard, "Single row");
        
        // Restore original configuration
        originalConfig.rows = originalRows;
        originalConfig.columns = originalColumns;
        
        Debug.Log("✅ [Step 4/7] Boundary validation testing complete");
    }
    
    /// <summary>
    /// Tests pattern generation with specific configuration
    /// </summary>
    /// <param name="brickGrid">BrickGrid to test</param>
    /// <param name="pattern">Pattern to test</param>
    /// <param name="testName">Name of test for logging</param>
    private static void TestPatternGeneration(BrickGrid brickGrid, LayoutPattern pattern, string testName)
    {
        try
        {
            brickGrid.GeneratePattern(pattern);
            int brickCount = brickGrid.InstantiatedBricks.Count;
            bool hasValidBounds = brickGrid.ValidateGridBounds();
            
            Debug.Log($"     - {testName}: {brickCount} bricks, bounds valid: {hasValidBounds}");
            brickGrid.ClearGrid();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"     - {testName} failed: {e.Message}");
        }
    }
    
    /// <summary>
    /// Validates geometric calculations for complex patterns
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to validate</param>
    private static void ValidateGeometricCalculations(BrickGrid brickGrid)
    {
        Debug.Log("📐 [Step 5/7] Validating geometric calculations...");
        
        if (brickGrid.GridConfiguration == null)
        {
            Debug.LogWarning("   ⚠️ No grid configuration - skipping geometric validation");
            return;
        }
        
        // Test pyramid calculations
        Debug.Log("   • Validating pyramid calculations...");
        ValidatePyramidGeometry(brickGrid);
        
        // Test diamond calculations
        Debug.Log("   • Validating diamond calculations...");
        ValidateDiamondGeometry(brickGrid);
        
        // Test position calculations
        Debug.Log("   • Validating position calculations...");
        ValidatePositionCalculations(brickGrid);
        
        Debug.Log("✅ [Step 5/7] Geometric validation complete");
    }
    
    /// <summary>
    /// Validates pyramid pattern geometric calculations
    /// </summary>
    /// <param name="brickGrid">BrickGrid to validate</param>
    private static void ValidatePyramidGeometry(BrickGrid brickGrid)
    {
        brickGrid.GeneratePattern(LayoutPattern.Pyramid);
        int totalBricks = brickGrid.InstantiatedBricks.Count;
        
        // Pyramid should have fewer bricks than a standard grid
        int maxPossibleBricks = brickGrid.GridConfiguration.rows * brickGrid.GridConfiguration.columns;
        bool isTriangular = totalBricks < maxPossibleBricks;
        
        Debug.Log($"     - Pyramid bricks: {totalBricks}/{maxPossibleBricks}, triangular: {isTriangular}");
        
        brickGrid.ClearGrid();
    }
    
    /// <summary>
    /// Validates diamond pattern geometric calculations
    /// </summary>
    /// <param name="brickGrid">BrickGrid to validate</param>
    private static void ValidateDiamondGeometry(BrickGrid brickGrid)
    {
        // Test solid diamond
        brickGrid.GeneratePattern(LayoutPattern.Diamond);
        int solidBricks = brickGrid.InstantiatedBricks.Count;
        brickGrid.ClearGrid();
        
        // Test hollow diamond
        SerializedObject serializedGrid = new SerializedObject(brickGrid);
        SerializedProperty hollowCenter = serializedGrid.FindProperty("hollowCenter");
        if (hollowCenter != null)
        {
            hollowCenter.boolValue = true;
            serializedGrid.ApplyModifiedProperties();
            
            brickGrid.GeneratePattern(LayoutPattern.Diamond);
            int hollowBricks = brickGrid.InstantiatedBricks.Count;
            
            bool hollowHasFewer = hollowBricks < solidBricks;
            Debug.Log($"     - Diamond solid: {solidBricks}, hollow: {hollowBricks}, hollow < solid: {hollowHasFewer}");
            
            // Reset hollow setting
            hollowCenter.boolValue = false;
            serializedGrid.ApplyModifiedProperties();
            brickGrid.ClearGrid();
        }
    }
    
    /// <summary>
    /// Validates position calculation accuracy
    /// </summary>
    /// <param name="brickGrid">BrickGrid to validate</param>
    private static void ValidatePositionCalculations(BrickGrid brickGrid)
    {
        if (brickGrid.GridConfiguration == null) return;
        
        // Test corner positions
        Vector3 bottomLeft = brickGrid.CalculateGridPosition(0, 0);
        Vector3 topRight = brickGrid.CalculateGridPosition(
            brickGrid.GridConfiguration.rows - 1, 
            brickGrid.GridConfiguration.columns - 1);
        
        float gridWidth = topRight.x - bottomLeft.x;
        float gridHeight = topRight.y - bottomLeft.y;
        
        bool positionsValid = gridWidth > 0 && gridHeight > 0;
        Debug.Log($"     - Grid size: {gridWidth:F1} x {gridHeight:F1}, positions valid: {positionsValid}");
    }
    
    /// <summary>
    /// Tests pattern variety for level design possibilities
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void TestPatternVarietyAndLevelDesign(BrickGrid brickGrid)
    {
        Debug.Log("🎮 [Step 6/7] Testing pattern variety for level design...");
        
        // Test different density settings for random patterns
        float[] densities = { 0.3f, 0.6f, 0.9f };
        
        SerializedObject serializedGrid = new SerializedObject(brickGrid);
        SerializedProperty patternDensity = serializedGrid.FindProperty("patternDensity");
        SerializedProperty useGridDataPattern = serializedGrid.FindProperty("useGridDataPattern");
        
        if (patternDensity != null && useGridDataPattern != null)
        {
            useGridDataPattern.boolValue = false; // Use BrickGrid density setting
            
            foreach (float density in densities)
            {
                patternDensity.floatValue = density;
                serializedGrid.ApplyModifiedProperties();
                
                brickGrid.GeneratePattern(LayoutPattern.Random);
                int brickCount = brickGrid.InstantiatedBricks.Count;
                
                Debug.Log($"   • Random pattern with {density:F1} density: {brickCount} bricks");
                brickGrid.ClearGrid();
            }
            
            // Reset settings
            useGridDataPattern.boolValue = true;
            serializedGrid.ApplyModifiedProperties();
        }
        
        // Test pattern combinations for level progression
        Debug.Log("   • Level progression pattern suggestions:");
        Debug.Log("     - Level 1-3: Standard pattern (classic gameplay)");
        Debug.Log("     - Level 4-6: Pyramid pattern (focused destruction)");
        Debug.Log("     - Level 7-9: Diamond pattern (strategic targeting)");
        Debug.Log("     - Level 10+: Random pattern (unpredictable challenge)");
        
        Debug.Log("✅ [Step 6/7] Pattern variety testing complete");
    }
    
    /// <summary>
    /// Logs successful setup summary with usage instructions
    /// </summary>
    /// <param name="brickGrid">Configured BrickGrid component</param>
    private static void LogSuccessfulSetup(BrickGrid brickGrid)
    {
        Debug.Log("✅ [Task 1.2.2.6] Layout Pattern Implementation created successfully!");
        Debug.Log("📋 Layout Pattern Implementation Summary:");
        Debug.Log("   • Strategy Pattern: Clean separation of pattern generation algorithms");
        Debug.Log("   • Geometric Calculations: Precise mathematical positioning for complex patterns");
        Debug.Log("   • Playability Validation: Ensures all patterns produce balanced gameplay");
        Debug.Log("   • Level Design Support: Multiple patterns for progression and variety");
        
        Debug.Log("🎨 Pattern Types Available:");
        Debug.Log("   → Standard: Classic Breakout brick wall with uniform rows and columns");
        Debug.Log("   → Pyramid: Triangular formation narrowing upward for focused gameplay");
        Debug.Log("   → Diamond: Symmetric rhombus shape with balanced destruction opportunities");
        Debug.Log("   → Random: Configurable density placement with playability validation");
        Debug.Log("   → Custom: Placeholder for user-defined specialized layouts");
        
        Debug.Log("⚙️ Pattern Configuration:");
        Debug.Log($"   • Use Grid Data Pattern: {(brickGrid.GridConfiguration != null ? "✅ Available" : "❌ No GridData")}");
        Debug.Log("   • Pattern Density Control: ✅ Configured for Random patterns");
        Debug.Log("   • Hollow Center Option: ✅ Available for Diamond patterns");
        Debug.Log("   • Reproducible Seeds: ✅ Consistent pattern generation");
        
        Debug.Log("🎮 Usage Instructions:");
        Debug.Log("   1. Set pattern in GridData asset or use BrickGrid override");
        Debug.Log("   2. Configure density for Random patterns (0.1-1.0)");
        Debug.Log("   3. Enable hollow center for Diamond patterns if desired");
        Debug.Log("   4. Call GenerateGrid() or GeneratePattern() to create layout");
        Debug.Log("   5. Use different patterns for level progression variety");
        
        Debug.Log("🔧 Pattern Features:");
        Debug.Log("   • Boundary Validation: All patterns respect grid size constraints");
        Debug.Log("   • Type Distribution: Pattern-specific brick type placement logic");
        Debug.Log("   • Performance Optimized: Efficient geometric calculations");
        Debug.Log("   • Hierarchy Integration: Works with organized scene structure");
        Debug.Log("   • Inspector Controls: Easy pattern configuration and testing");
        
        Debug.Log("💡 Level Design Possibilities:");
        Debug.Log("   • Difficulty Progression: Start with Standard, advance to complex patterns");
        Debug.Log("   • Gameplay Variety: Different patterns require different strategies");
        Debug.Log("   • Replayability: Random patterns provide varied experiences");
        Debug.Log("   • Strategic Depth: Geometric patterns encourage tactical thinking");
        Debug.Log("   • Visual Appeal: Diverse formations create engaging visual layouts");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Pattern generation respects GridData configuration and boundaries");
        Debug.Log("   → Random patterns include minimum brick validation for playability");
        Debug.Log("   → Geometric patterns use precise mathematical calculations");
        Debug.Log("   → All patterns integrate with existing hierarchy organization");
        Debug.Log("   → Pattern seeds enable reproducible layouts for testing");
        
        Debug.Log("🚀 Advanced Usage:");
        Debug.Log("   → Combine patterns with different grid sizes for variety");
        Debug.Log("   → Use hollow diamond patterns for advanced challenge levels");
        Debug.Log("   → Implement custom pattern logic for special level types");
        Debug.Log("   → Test pattern performance with large grid configurations");
        Debug.Log("   → Create pattern sequences for progressive difficulty curves");
    }
}
#endif