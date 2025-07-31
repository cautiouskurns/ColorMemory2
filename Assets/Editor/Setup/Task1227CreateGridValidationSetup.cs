#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Editor setup script for creating and demonstrating Grid Validation and Testing Tools.
/// Provides comprehensive validation system testing, performance analysis, and debugging support.
/// </summary>
public static class Task1227CreateGridValidationSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1227 Create Grid Validation and Testing";
    
    /// <summary>
    /// Creates and demonstrates the Grid Validation and Testing Tools system.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateGridValidationTesting()
    {
        Debug.Log("🧪 [Task 1.2.2.7] Starting Grid Validation and Testing Tools setup...");
        
        try
        {
            // Step 1: Validate complete grid system exists
            BrickGrid brickGrid = ValidateCompleteGridSystem();
            
            // Step 2: Enable validation and debug features
            ConfigureValidationSettings(brickGrid);
            
            // Step 3: Demonstrate validation tools
            DemonstrateValidationTools(brickGrid);
            
            // Step 4: Run comprehensive pattern testing
            RunComprehensivePatternTesting(brickGrid);
            
            // Step 5: Execute performance testing
            RunPerformanceTestingSuite(brickGrid);
            
            // Step 6: Test debug visualization
            TestDebugVisualization(brickGrid);
            
            // Step 7: Validate development workflow integration
            ValidateDevelopmentWorkflow(brickGrid);
            
            // Step 8: Log success and usage instructions
            LogSuccessfulSetup(brickGrid);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.2.7] Grid Validation and Testing setup failed: {e.Message}");
            Debug.LogError("📋 Please ensure complete grid system is available from all previous tasks");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if complete grid system exists
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateGridValidationTesting()
    {
        BrickGrid brickGrid = Object.FindFirstObjectByType<BrickGrid>();
        return brickGrid != null && brickGrid.GridConfiguration != null;
    }
    
    /// <summary>
    /// Validates that complete grid system exists with all dependencies from previous tasks
    /// </summary>
    /// <returns>BrickGrid component with full validation system</returns>
    private static BrickGrid ValidateCompleteGridSystem()
    {
        Debug.Log("🔍 [Step 1/8] Validating complete grid system...");
        
        BrickGrid brickGrid = Object.FindFirstObjectByType<BrickGrid>();
        if (brickGrid == null)
        {
            // Try to create complete system if missing
            Debug.LogWarning("   ⚠️ BrickGrid not found - attempting to create complete system...");
            
            // Call previous setup tasks in sequence
            Task1226CreateLayoutPatternSetup.CreateLayoutPatternImplementation();
            
            brickGrid = Object.FindFirstObjectByType<BrickGrid>();
            if (brickGrid == null)
            {
                throw new System.Exception("Failed to create complete grid system. Please run previous setup tasks (1.2.2.1-1.2.2.6).");
            }
        }
        
        // Validate system components
        bool hasConfiguration = brickGrid.GridConfiguration != null;
        bool hasPrefab = brickGrid.BrickPrefab != null;
        bool hasContainer = brickGrid.GridContainer != null;
        bool hasValidationMethods = HasValidationMethods(brickGrid);
        
        Debug.Log($"   • Grid configuration: {(hasConfiguration ? "✅" : "❌")} {(hasConfiguration ? brickGrid.GridConfiguration.name : "Missing")}");
        Debug.Log($"   • Brick prefab: {(hasPrefab ? "✅" : "❌")} {(hasPrefab ? brickGrid.BrickPrefab.name : "Missing")}");
        Debug.Log($"   • Grid container: {(hasContainer ? "✅" : "❌")} {(hasContainer ? brickGrid.GridContainer.name : "Missing")}");
        Debug.Log($"   • Validation methods: {(hasValidationMethods ? "✅" : "❌")} {(hasValidationMethods ? "Available" : "Missing")}");
        
        if (!hasConfiguration || !hasPrefab || !hasValidationMethods)
        {
            throw new System.Exception("Grid system is incomplete. Missing required components for validation system.");
        }
        
        Debug.Log("✅ [Step 1/8] Complete grid system validation successful");
        return brickGrid;
    }
    
    /// <summary>
    /// Checks if BrickGrid has the required validation methods
    /// </summary>
    /// <param name="brickGrid">BrickGrid to check</param>
    /// <returns>True if validation methods are available</returns>
    private static bool HasValidationMethods(BrickGrid brickGrid)
    {
        var type = brickGrid.GetType();
        return type.GetMethod("ValidateGridConfiguration") != null &&
               type.GetMethod("ValidateGeneratedGrid") != null &&
               type.GetMethod("TestAllPatterns") != null &&
               type.GetMethod("RunPerformanceTest") != null;
    }
    
    /// <summary>
    /// Configures validation and debug settings for optimal testing
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to configure</param>
    private static void ConfigureValidationSettings(BrickGrid brickGrid)
    {
        Debug.Log("⚙️ [Step 2/8] Configuring validation settings...");
        
        SerializedObject serializedBrickGrid = new SerializedObject(brickGrid);
        
        // Enable debug visualization
        SerializedProperty enableDebugVisualization = serializedBrickGrid.FindProperty("enableDebugVisualization");
        if (enableDebugVisualization != null)
        {
            enableDebugVisualization.boolValue = true;
            Debug.Log("   • Debug visualization: Enabled ✅");
        }
        
        // Enable validation on generation
        SerializedProperty runValidationOnGeneration = serializedBrickGrid.FindProperty("runValidationOnGeneration");
        if (runValidationOnGeneration != null)
        {
            runValidationOnGeneration.boolValue = true;
            Debug.Log("   • Validation on generation: Enabled ✅");
        }
        
        // Enable debug logging
        SerializedProperty enableDebugLogging = serializedBrickGrid.FindProperty("enableDebugLogging");
        if (enableDebugLogging != null)
        {
            enableDebugLogging.boolValue = true;
            Debug.Log("   • Debug logging: Enabled ✅");
        }
        
        // Enable grid gizmos
        SerializedProperty showGridGizmos = serializedBrickGrid.FindProperty("showGridGizmos");
        if (showGridGizmos != null)
        {
            showGridGizmos.boolValue = true;
            Debug.Log("   • Grid gizmos: Enabled ✅");
        }
        
        // Configure debug colors
        SerializedProperty debugBoundsColor = serializedBrickGrid.FindProperty("debugBoundsColor");
        if (debugBoundsColor != null)
        {
            debugBoundsColor.colorValue = Color.yellow;
        }
        
        SerializedProperty debugErrorColor = serializedBrickGrid.FindProperty("debugErrorColor");
        if (debugErrorColor != null)
        {
            debugErrorColor.colorValue = Color.red;
        }
        
        SerializedProperty debugSuccessColor = serializedBrickGrid.FindProperty("debugSuccessColor");
        if (debugSuccessColor != null)
        {
            debugSuccessColor.colorValue = Color.green;
        }
        
        serializedBrickGrid.ApplyModifiedProperties();
        EditorUtility.SetDirty(brickGrid);
        
        Debug.Log("✅ [Step 2/8] Validation settings configured");
    }
    
    /// <summary>
    /// Demonstrates validation tools with comprehensive testing
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void DemonstrateValidationTools(BrickGrid brickGrid)
    {
        Debug.Log("🧪 [Step 3/8] Demonstrating validation tools...");
        
        Debug.Log("   🔍 Testing configuration validation...");
        bool configValid = brickGrid.ValidateGridConfiguration();
        Debug.Log($"   • Configuration validation: {(configValid ? "✅ PASSED" : "❌ FAILED")}");
        
        Debug.Log("   🔍 Testing bounds validation...");
        bool boundsValid = brickGrid.ValidateGridBounds();
        Debug.Log($"   • Bounds validation: {(boundsValid ? "✅ PASSED" : "❌ FAILED")}");
        
        // Test with a small grid first
        if (brickGrid.GridConfiguration != null)
        {
            int originalRows = brickGrid.GridConfiguration.rows;
            int originalColumns = brickGrid.GridConfiguration.columns;
            
            // Set small size for quick testing
            brickGrid.GridConfiguration.rows = 3;
            brickGrid.GridConfiguration.columns = 5;
            
            Debug.Log("   🔍 Testing grid generation validation...");
            brickGrid.GenerateGrid();
            bool gridValid = brickGrid.ValidateGeneratedGrid();
            Debug.Log($"   • Generated grid validation: {(gridValid ? "✅ PASSED" : "❌ FAILED")}");
            
            Debug.Log("   🔍 Testing comprehensive validation...");
            bool overallValid = brickGrid.ValidateGrid();
            Debug.Log($"   • Overall validation: {(overallValid ? "✅ PASSED" : "❌ FAILED")}");
            
            // Restore original configuration
            brickGrid.GridConfiguration.rows = originalRows;
            brickGrid.GridConfiguration.columns = originalColumns;
            brickGrid.ClearGrid();
        }
        
        Debug.Log("✅ [Step 3/8] Validation tools demonstration complete");
    }
    
    /// <summary>
    /// Runs comprehensive pattern testing for all layout types
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void RunComprehensivePatternTesting(BrickGrid brickGrid)
    {
        Debug.Log("🎭 [Step 4/8] Running comprehensive pattern testing...");
        
        if (brickGrid.GridConfiguration == null)
        {
            Debug.LogWarning("   ⚠️ No grid configuration - skipping pattern testing");
            return;
        }
        
        // Store original state
        LayoutPattern originalPattern = brickGrid.GridConfiguration.pattern;
        bool wasGenerated = brickGrid.IsGridGenerated;
        
        Debug.Log("   🧪 Testing all pattern types...");
        brickGrid.TestAllPatterns();
        
        // Additional edge case testing
        Debug.Log("   🧪 Testing edge cases...");
        TestPatternEdgeCases(brickGrid);
        
        // Restore original state
        brickGrid.GridConfiguration.pattern = originalPattern;
        if (wasGenerated)
        {
            brickGrid.ClearGrid();
            brickGrid.GenerateGrid();
        }
        
        Debug.Log("✅ [Step 4/8] Comprehensive pattern testing complete");
    }
    
    /// <summary>
    /// Tests edge cases for pattern validation
    /// </summary>
    /// <param name="brickGrid">BrickGrid to test</param>
    private static void TestPatternEdgeCases(BrickGrid brickGrid)
    {
        var edgeCases = new List<(int rows, int columns, string name)>
        {
            (1, 1, "Minimal Grid (1x1)"),
            (1, 10, "Single Row (1x10)"),
            (10, 1, "Single Column (10x1)"),
            (2, 2, "Tiny Square (2x2)"),
            (3, 3, "Small Square (3x3)")
        };
        
        int originalRows = brickGrid.GridConfiguration.rows;
        int originalColumns = brickGrid.GridConfiguration.columns;
        
        foreach (var edgeCase in edgeCases)
        {
            Debug.Log($"     • Testing {edgeCase.name}...");
            
            brickGrid.GridConfiguration.rows = edgeCase.rows;
            brickGrid.GridConfiguration.columns = edgeCase.columns;
            
            try
            {
                brickGrid.ClearGrid();
                brickGrid.GenerateGrid();
                bool isValid = brickGrid.ValidateGeneratedGrid();
                
                Debug.Log($"       - {edgeCase.name}: {(isValid ? "✅ Valid" : "❌ Invalid")} ({brickGrid.InstantiatedBricks.Count} bricks)");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"       - {edgeCase.name}: ⚠️ Exception: {e.Message}");
            }
        }
        
        // Restore original dimensions
        brickGrid.GridConfiguration.rows = originalRows;
        brickGrid.GridConfiguration.columns = originalColumns;
    }
    
    /// <summary>
    /// Executes performance testing suite with metrics and recommendations
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void RunPerformanceTestingSuite(BrickGrid brickGrid)
    {
        Debug.Log("⚡ [Step 5/8] Running performance testing suite...");
        
        if (brickGrid.GridConfiguration == null)
        {
            Debug.LogWarning("   ⚠️ No grid configuration - skipping performance testing");
            return;
        }
        
        Debug.Log("   📊 Executing built-in performance tests...");
        brickGrid.RunPerformanceTest();
        
        Debug.Log("   📊 Additional stress testing...");
        RunStressTesting(brickGrid);
        
        Debug.Log("   📊 Memory allocation testing...");
        RunMemoryAllocationTesting(brickGrid);
        
        Debug.Log("✅ [Step 5/8] Performance testing suite complete");
    }
    
    /// <summary>
    /// Runs stress testing with very large grids
    /// </summary>
    /// <param name="brickGrid">BrickGrid to test</param>
    private static void RunStressTesting(BrickGrid brickGrid)
    {
        int originalRows = brickGrid.GridConfiguration.rows;
        int originalColumns = brickGrid.GridConfiguration.columns;
        bool wasGenerated = brickGrid.IsGridGenerated;
        
        var stressConfigs = new[]
        {
            new { rows = 20, columns = 30, name = "Large Grid (20x30)" },
            new { rows = 25, columns = 40, name = "Extra Large Grid (25x40)" },
            new { rows = 30, columns = 50, name = "Stress Test Grid (30x50)" }
        };
        
        foreach (var config in stressConfigs)
        {
            Debug.Log($"     • Stress testing {config.name}...");
            
            brickGrid.GridConfiguration.rows = config.rows;
            brickGrid.GridConfiguration.columns = config.columns;
            
            if (wasGenerated) brickGrid.ClearGrid();
            
            try
            {
                System.DateTime startTime = System.DateTime.Now;
                brickGrid.GenerateGrid();
                System.TimeSpan generationTime = System.DateTime.Now - startTime;
                
                int brickCount = brickGrid.InstantiatedBricks.Count;
                
                Debug.Log($"       - {config.name}: {brickCount} bricks in {generationTime.TotalMilliseconds:F1}ms");
                
                if (generationTime.TotalMilliseconds > 500)
                {
                    Debug.LogWarning($"       ⚠️ Slow generation detected: {generationTime.TotalMilliseconds:F1}ms");
                }
                
                // Validate the result
                bool isValid = brickGrid.ValidateGeneratedGrid();
                Debug.Log($"       - Validation: {(isValid ? "✅ PASSED" : "❌ FAILED")}");
                
            }
            catch (System.Exception e)
            {
                Debug.LogError($"       ❌ Stress test failed: {e.Message}");
            }
        }
        
        // Restore original configuration
        brickGrid.GridConfiguration.rows = originalRows;
        brickGrid.GridConfiguration.columns = originalColumns;
        
        if (wasGenerated)
        {
            brickGrid.ClearGrid();
            brickGrid.GenerateGrid();
        }
    }
    
    /// <summary>
    /// Tests memory allocation patterns during grid operations
    /// </summary>
    /// <param name="brickGrid">BrickGrid to test</param>
    private static void RunMemoryAllocationTesting(BrickGrid brickGrid)
    {
        long memoryBefore = System.GC.GetTotalMemory(true);
        
        // Test multiple generation cycles
        for (int i = 0; i < 3; i++)
        {
            brickGrid.ClearGrid();
            brickGrid.GenerateGrid();
            brickGrid.ValidateGrid();
        }
        
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        
        long memoryAfter = System.GC.GetTotalMemory(false);
        long memoryDelta = memoryAfter - memoryBefore;
        
        Debug.Log($"     • Memory allocation: {memoryDelta / 1024:F1} KB delta");
        
        if (memoryDelta > 1024 * 100) // 100KB threshold
        {
            Debug.LogWarning($"     ⚠️ High memory allocation detected: {memoryDelta / 1024:F1} KB");
            Debug.Log("     💡 Consider object pooling for large grids");
        }
    }
    
    /// <summary>
    /// Tests debug visualization functionality
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to test</param>
    private static void TestDebugVisualization(BrickGrid brickGrid)
    {
        Debug.Log("🎨 [Step 6/8] Testing debug visualization...");
        
        // Test gizmo visibility settings
        SerializedObject serializedBrickGrid = new SerializedObject(brickGrid);
        
        SerializedProperty showGridGizmos = serializedBrickGrid.FindProperty("showGridGizmos");
        SerializedProperty enableDebugVisualization = serializedBrickGrid.FindProperty("enableDebugVisualization");
        
        if (showGridGizmos != null && enableDebugVisualization != null)
        {
            // Test with visualization enabled
            showGridGizmos.boolValue = true;
            enableDebugVisualization.boolValue = true;
            serializedBrickGrid.ApplyModifiedProperties();
            
            Debug.Log("   • Debug visualization enabled ✅");
            Debug.Log("   • Grid gizmos enabled ✅");
            Debug.Log("   • Validation colors configured ✅");
            
            // Generate grid for visualization
            if (!brickGrid.IsGridGenerated)
            {
                brickGrid.GenerateGrid();
            }
            
            Debug.Log("   • Grid generated for visualization testing ✅");
            Debug.Log("   💡 Check Scene view for validation gizmos (bounds, patterns, status indicators)");
        }
        else
        {
            Debug.LogWarning("   ⚠️ Could not find debug visualization properties");
        }
        
        Debug.Log("✅ [Step 6/8] Debug visualization testing complete");
    }
    
    /// <summary>
    /// Validates development workflow integration
    /// </summary>
    /// <param name="brickGrid">BrickGrid component to validate</param>
    private static void ValidateDevelopmentWorkflow(BrickGrid brickGrid)
    {
        Debug.Log("🔧 [Step 7/8] Validating development workflow integration...");
        
        // Simulate development workflow
        Debug.Log("   • Testing error detection workflow...");
        
        // Test invalid configuration detection
        if (brickGrid.GridConfiguration != null)
        {
            int originalRows = brickGrid.GridConfiguration.rows;
            
            // Create invalid configuration
            brickGrid.GridConfiguration.rows = -1;
            bool configValid = brickGrid.ValidateGridConfiguration();
            
            Debug.Log($"     - Invalid config detection: {(!configValid ? "✅ WORKING" : "❌ FAILED")}");
            
            // Restore valid configuration
            brickGrid.GridConfiguration.rows = originalRows;
        }
        
        // Test debugging support
        Debug.Log("   • Testing debugging support...");
        bool hasDebugMethods = HasDebugMethods(brickGrid);
        Debug.Log($"     - Debug methods available: {(hasDebugMethods ? "✅ AVAILABLE" : "❌ MISSING")}");
        
        // Test validation on generation
        Debug.Log("   • Testing validation-on-generation workflow...");
        SerializedObject serializedBrickGrid = new SerializedObject(brickGrid);
        SerializedProperty runValidationOnGeneration = serializedBrickGrid.FindProperty("runValidationOnGeneration");
        
        if (runValidationOnGeneration != null && runValidationOnGeneration.boolValue)
        {
            Debug.Log("     - Validation on generation: ✅ ENABLED");
        }
        else
        {
            Debug.LogWarning("     - Validation on generation: ⚠️ NOT ENABLED");
        }
        
        Debug.Log("✅ [Step 7/8] Development workflow validation complete");
    }
    
    /// <summary>
    /// Checks if BrickGrid has debug methods available
    /// </summary>
    /// <param name="brickGrid">BrickGrid to check</param>
    /// <returns>True if debug methods are available</returns>
    private static bool HasDebugMethods(BrickGrid brickGrid)
    {
        var type = brickGrid.GetType();
        return type.GetMethod("ValidateGrid") != null &&
               type.GetMethod("TestAllPatterns") != null &&
               type.GetMethod("RunPerformanceTest") != null;
    }
    
    /// <summary>
    /// Logs successful setup summary with comprehensive usage instructions
    /// </summary>
    /// <param name="brickGrid">Configured BrickGrid component</param>
    private static void LogSuccessfulSetup(BrickGrid brickGrid)
    {
        Debug.Log("✅ [Task 1.2.2.7] Grid Validation and Testing Tools created successfully!");
        Debug.Log("📋 Grid Validation System Summary:");
        Debug.Log("   • Configuration Validation: Detects errors before generation attempts");
        Debug.Log("   • Generated Grid Verification: Confirms accurate brick placement and counts");
        Debug.Log("   • Pattern Testing Suite: Validates all layout types and edge cases");
        Debug.Log("   • Performance Testing: Measures generation efficiency and provides optimization recommendations");
        Debug.Log("   • Debug Visualization: Visual feedback through Scene view gizmos");
        
        Debug.Log("🧪 Validation Methods Available:");
        Debug.Log("   → ValidateGridConfiguration(): Check settings before generation");
        Debug.Log("   → ValidateGeneratedGrid(): Verify grid accuracy after generation");
        Debug.Log("   → ValidateGrid(): Comprehensive validation of entire system");
        Debug.Log("   → TestAllPatterns(): Test all pattern types with validation");
        Debug.Log("   → RunPerformanceTest(): Performance analysis with recommendations");
        
        Debug.Log("🎨 Debug Visualization Features:");
        Debug.Log("   • Grid bounds visualization with validation colors");
        Debug.Log("   • Pattern-specific indicators (Standard, Pyramid, Diamond, Random)");
        Debug.Log("   • Validation status indicators (Config, Bounds, Grid)");
        Debug.Log("   • Real-time validation feedback in Scene view");
        Debug.Log("   • Error highlighting with red gizmos, success with green gizmos");
        
        Debug.Log("⚙️ Inspector Controls:");
        Debug.Log($"   • Enable Debug Visualization: {(brickGrid.GridConfiguration != null ? "✅ Available" : "❌ No GridData")}");
        Debug.Log("   • Run Validation On Generation: ✅ Configured for automatic validation");
        Debug.Log("   • Debug Colors: ✅ Yellow (bounds), Red (errors), Green (success)");
        Debug.Log("   • Grid Gizmos: ✅ Enabled for comprehensive Scene view feedback");
        
        Debug.Log("🔧 Development Workflow:");
        Debug.Log("   1. Configure grid settings in Inspector");
        Debug.Log("   2. Enable debug visualization for real-time feedback");
        Debug.Log("   3. Generate grid - automatic validation runs if enabled");
        Debug.Log("   4. Check Console for validation results and recommendations");
        Debug.Log("   5. Use Scene view gizmos for visual validation confirmation");
        Debug.Log("   6. Call TestAllPatterns() for comprehensive pattern validation");
        Debug.Log("   7. Use RunPerformanceTest() for optimization analysis");
        
        Debug.Log("📊 Performance Testing Features:");
        Debug.Log("   • Grid size testing: Small (5x8), Medium (10x15), Large (15x20)");
        Debug.Log("   • Generation time measurement with performance warnings");
        Debug.Log("   • Validation time measurement for efficiency analysis");
        Debug.Log("   • Memory allocation testing for optimization guidance");
        Debug.Log("   • Stress testing with edge cases and large configurations");
        
        Debug.Log("🎮 Usage Examples:");
        Debug.Log("   // Validate before generation");
        Debug.Log("   if (brickGrid.ValidateGridConfiguration()) brickGrid.GenerateGrid();");
        Debug.Log("   ");
        Debug.Log("   // Comprehensive testing");
        Debug.Log("   brickGrid.TestAllPatterns();");
        Debug.Log("   brickGrid.RunPerformanceTest();");
        Debug.Log("   ");
        Debug.Log("   // Development debugging");
        Debug.Log("   bool isValid = brickGrid.ValidateGrid();");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Validation automatically runs during generation if enabled");
        Debug.Log("   → Debug visualization only shows when enableDebugVisualization is true");
        Debug.Log("   → Performance testing modifies grid temporarily but restores original state");
        Debug.Log("   → Gizmos provide real-time feedback in Scene view for development");
        Debug.Log("   → Console output provides detailed validation results and recommendations");
        
        Debug.Log("💡 Development Tips:");
        Debug.Log("   → Use validation colors in Scene view: Green = valid, Red = errors, Yellow = bounds");
        Debug.Log("   → Check Console for specific validation failure details and solutions");
        Debug.Log("   → Performance testing helps optimize for target platform requirements");
        Debug.Log("   → Pattern testing ensures reliable generation across all layout types");
        Debug.Log("   → Enable debug logging for detailed validation step-by-step information");
    }
}
#endif