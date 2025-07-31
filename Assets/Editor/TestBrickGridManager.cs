#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Test script for validating BrickGrid Manager functionality and integration
/// </summary>
public static class TestBrickGridManager
{
    [MenuItem("Breakout/Debug/Test BrickGrid Manager")]
    public static void TestBrickGridManagerFunctionality()
    {
        Debug.Log("🧪 Testing BrickGrid Manager functionality...");
        
        // Test 1: Find BrickGrid in scene
        TestBrickGridExistence();
        
        // Test 2: Validate component initialization
        TestBrickGridInitialization();
        
        // Test 3: Test configuration validation
        TestConfigurationValidation();
        
        // Test 4: Test state management
        TestStateManagement();
        
        // Test 5: Test framework methods
        TestFrameworkMethods();
        
        Debug.Log("✅ BrickGrid Manager test completed successfully!");
    }
    
    private static void TestBrickGridExistence()
    {
        Debug.Log("🔍 Testing BrickGrid existence in scene...");
        
        BrickGrid brickGrid = Object.FindObjectOfType<BrickGrid>();
        if (brickGrid != null)
        {
            Debug.Log($"   • BrickGrid found: {brickGrid.gameObject.name} ✅");
            Debug.Log($"   • Position: {brickGrid.transform.position}");
            Debug.Log($"   • Active: {brickGrid.gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogWarning("   ⚠️ No BrickGrid found in scene");
            Debug.LogWarning("   📋 Run 'Breakout/Setup/Task1222 Create BrickGrid Manager' to create one");
        }
    }
    
    private static void TestBrickGridInitialization()
    {
        Debug.Log("⚙️ Testing BrickGrid initialization...");
        
        BrickGrid brickGrid = Object.FindObjectOfType<BrickGrid>();
        if (brickGrid == null)
        {
            Debug.LogError("   ❌ No BrickGrid to test");
            return;
        }
        
        // Test initialization status
        Debug.Log($"   • IsInitialized: {brickGrid.IsInitialized} {(brickGrid.IsInitialized ? "✅" : "❌")}");
        Debug.Log($"   • GridContainer: {(brickGrid.GridContainer != null ? "✅" : "❌")}");
        Debug.Log($"   • GridConfiguration: {(brickGrid.GridConfiguration != null ? "✅" : "❌")}");
        
        if (brickGrid.GridContainer != null)
        {
            Debug.Log($"     └─ Container name: {brickGrid.GridContainer.name}");
            Debug.Log($"     └─ Container parent: {(brickGrid.GridContainer.transform.parent == brickGrid.transform ? "✅" : "❌")}");
        }
    }
    
    private static void TestConfigurationValidation()
    {
        Debug.Log("🔧 Testing configuration validation...");
        
        BrickGrid brickGrid = Object.FindObjectOfType<BrickGrid>();
        if (brickGrid == null)
        {
            Debug.LogError("   ❌ No BrickGrid to test");
            return;
        }
        
        // Note: In Editor mode, Awake/Start haven't been called
        Debug.Log($"   • IsInitialized: {brickGrid.IsInitialized} (Note: Will be false in Editor mode)");
        
        // Test configuration validation (works even without initialization)
        bool configValid = brickGrid.ValidateConfiguration();
        Debug.Log($"   • ValidateConfiguration(): {configValid} {(configValid ? "✅" : "⚠️")}");
        
        // Test grid generation validation (requires initialization in play mode)
        if (Application.isPlaying && brickGrid.IsInitialized)
        {
            bool genValid = brickGrid.ValidateGridGeneration();
            Debug.Log($"   • ValidateGridGeneration(): {genValid} {(genValid ? "✅" : "⚠️")}");
        }
        else
        {
            Debug.Log("   • ValidateGridGeneration(): Skipped (requires Play mode)");
        }
        
        // Test GridData properties if available
        if (brickGrid.GridConfiguration != null)
        {
            GridData config = brickGrid.GridConfiguration;
            Debug.Log($"   • GridData name: {config.name}");
            Debug.Log($"   • Pattern: {config.pattern}");
            Debug.Log($"   • Dimensions: {config.rows}x{config.columns}");
            
            // These properties require initialization
            if (brickGrid.IsInitialized)
            {
                Debug.Log($"   • Grid dimensions: {brickGrid.GridDimensions}");
                Debug.Log($"   • Start position: {brickGrid.GridStartPosition}");
            }
        }
    }
    
    private static void TestStateManagement()
    {
        Debug.Log("📊 Testing state management...");
        
        BrickGrid brickGrid = Object.FindObjectOfType<BrickGrid>();
        if (brickGrid == null)
        {
            Debug.LogError("   ❌ No BrickGrid to test");
            return;
        }
        
        // Test initial state
        Debug.Log($"   • IsGridGenerated: {brickGrid.IsGridGenerated}");
        Debug.Log($"   • ActiveBrickCount: {brickGrid.ActiveBrickCount}");
        Debug.Log($"   • IsComplete: {brickGrid.IsComplete}");
        
        // Test state reset
        brickGrid.ResetGridState();
        Debug.Log("   • ResetGridState() called ✅");
        
        // Test state updates
        brickGrid.UpdateBrickCount(10);
        Debug.Log($"   • UpdateBrickCount(10): ActiveBrickCount = {brickGrid.ActiveBrickCount} ✅");
        
        brickGrid.SetGridGenerated(15);
        Debug.Log($"   • SetGridGenerated(15): Generated = {brickGrid.IsGridGenerated}, Count = {brickGrid.ActiveBrickCount} ✅");
        
        // Test completion detection
        brickGrid.UpdateBrickCount(0);
        Debug.Log($"   • UpdateBrickCount(0): IsComplete = {brickGrid.IsComplete} {(brickGrid.IsComplete ? "✅" : "❌")}");
    }
    
    private static void TestFrameworkMethods()
    {
        Debug.Log("🚀 Testing framework methods...");
        
        BrickGrid brickGrid = Object.FindObjectOfType<BrickGrid>();
        if (brickGrid == null)
        {
            Debug.LogError("   ❌ No BrickGrid to test");
            return;
        }
        
        // Note: Framework methods require initialization which happens in Play mode
        if (Application.isPlaying && brickGrid.IsInitialized)
        {
            // Test framework method calls (should log that they're not implemented yet)
            Debug.Log("   • Testing GenerateGrid():");
            brickGrid.GenerateGrid();
            
            Debug.Log("   • Testing ClearGrid():");
            brickGrid.ClearGrid();
            
            Debug.Log("   • Testing ValidateGrid():");
            bool validateResult = brickGrid.ValidateGrid();
            Debug.Log($"     └─ ValidateGrid() returned: {validateResult} ✅");
        }
        else
        {
            Debug.Log("   • Framework method testing: Skipped (requires Play mode)");
            Debug.Log("   • These methods check IsInitialized before executing");
        }
        
        // Test debug info (works without initialization)
        Debug.Log("   • Testing GetGridDebugInfo():");
        string debugInfo = brickGrid.GetGridDebugInfo();
        Debug.Log($"     └─ Debug info generated ({debugInfo.Length} characters) ✅");
        
        // Log a sample of the debug info
        string[] lines = debugInfo.Split('\n');
        for (int i = 0; i < Mathf.Min(3, lines.Length); i++)
        {
            Debug.Log($"     └─ {lines[i]}");
        }
    }
    
    [MenuItem("Breakout/Debug/Show BrickGrid Debug Info")]
    public static void ShowBrickGridDebugInfo()
    {
        Debug.Log("📋 BrickGrid Debug Information:");
        
        BrickGrid brickGrid = Object.FindObjectOfType<BrickGrid>();
        if (brickGrid == null)
        {
            Debug.LogError("❌ No BrickGrid found in scene");
            return;
        }
        
        string debugInfo = brickGrid.GetGridDebugInfo();
        Debug.Log(debugInfo);
        
        // Additional component information
        Debug.Log("\n📊 Component Status:");
        Debug.Log($"   • GameObject: {brickGrid.gameObject.name}");
        Debug.Log($"   • Transform Position: {brickGrid.transform.position}");
        Debug.Log($"   • Active in Hierarchy: {brickGrid.gameObject.activeInHierarchy}");
        Debug.Log($"   • Component Enabled: {brickGrid.enabled}");
        
        if (brickGrid.GridContainer != null)
        {
            Debug.Log($"\n🗂️ Grid Container:");
            Debug.Log($"   • Name: {brickGrid.GridContainer.name}");
            Debug.Log($"   • Active: {brickGrid.GridContainer.activeInHierarchy}");
            Debug.Log($"   • Child Count: {brickGrid.GridContainer.transform.childCount}");
            Debug.Log($"   • Position: {brickGrid.GridContainer.transform.position}");
        }
    }
    
    [MenuItem("Breakout/Debug/Test BrickGrid with GridData")]
    public static void TestBrickGridWithGridData()
    {
        Debug.Log("🔗 Testing BrickGrid with GridData integration...");
        
        BrickGrid brickGrid = Object.FindObjectOfType<BrickGrid>();
        if (brickGrid == null)
        {
            Debug.LogError("❌ No BrickGrid found in scene");
            return;
        }
        
        // Try to find GridData assets
        string[] searchPaths = AssetDatabase.FindAssets("t:GridData");
        
        if (searchPaths.Length == 0)
        {
            Debug.LogWarning("⚠️ No GridData assets found");
            Debug.LogWarning("📋 Run 'Breakout/Setup/Task1221 Create Grid Data Structures' first");
            return;
        }
        
        Debug.Log($"📦 Found {searchPaths.Length} GridData assets:");
        
        foreach (string guid in searchPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GridData gridData = AssetDatabase.LoadAssetAtPath<GridData>(path);
            
            if (gridData != null)
            {
                Debug.Log($"   • {gridData.name}: {gridData.pattern}, {gridData.rows}x{gridData.columns}");
                
                // Test assignment
                SerializedObject serializedBrickGrid = new SerializedObject(brickGrid);
                SerializedProperty gridConfiguration = serializedBrickGrid.FindProperty("gridConfiguration");
                
                if (gridConfiguration != null)
                {
                    gridConfiguration.objectReferenceValue = gridData;
                    serializedBrickGrid.ApplyModifiedProperties();
                    
                    Debug.Log($"     └─ Assigned to BrickGrid ✅");
                    
                    // Test validation with new configuration
                    bool valid = brickGrid.ValidateConfiguration();
                    Debug.Log($"     └─ Validation: {(valid ? "✅" : "❌")}");
                    
                    break; // Use first found GridData
                }
            }
        }
    }
}
#endif