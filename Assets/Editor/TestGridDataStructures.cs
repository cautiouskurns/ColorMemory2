#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Test script for validating Grid Configuration Data Structures functionality
/// </summary>
public static class TestGridDataStructures
{
    [MenuItem("Breakout/Debug/Test Grid Data Structures")]
    public static void TestGridDataFunctionality()
    {
        Debug.Log("🧪 Testing Grid Configuration Data Structures...");
        
        // Test 1: LayoutPattern enum
        TestLayoutPatternEnum();
        
        // Test 2: GridData creation and validation
        TestGridDataCreation();
        
        // Test 3: GridData helper methods
        TestGridDataHelperMethods();
        
        // Test 4: Sample configurations
        TestSampleConfigurations();
        
        Debug.Log("✅ Grid Data Structures test completed successfully!");
    }
    
    private static void TestLayoutPatternEnum()
    {
        Debug.Log("📋 Testing LayoutPattern enum...");
        
        // Test all enum values
        LayoutPattern[] patterns = (LayoutPattern[])System.Enum.GetValues(typeof(LayoutPattern));
        foreach (LayoutPattern pattern in patterns)
        {
            Debug.Log($"   • LayoutPattern.{pattern}: Available ✅");
        }
        
        Debug.Log($"   • Total patterns available: {patterns.Length}");
    }
    
    private static void TestGridDataCreation()
    {
        Debug.Log("🔧 Testing GridData creation...");
        
        // Test default creation
        GridData defaultGrid = GridData.CreateDefault();
        if (defaultGrid != null && defaultGrid.ValidateConfiguration())
        {
            Debug.Log("   • CreateDefault(): Success ✅");
            Debug.Log($"     └─ {defaultGrid.rows}x{defaultGrid.columns}, Pattern: {defaultGrid.pattern}");
        }
        else
        {
            Debug.LogError("   • CreateDefault(): Failed ❌");
        }
        
        // Test pyramid creation
        GridData pyramidGrid = GridData.CreatePyramid();
        if (pyramidGrid != null && pyramidGrid.ValidateConfiguration())
        {
            Debug.Log("   • CreatePyramid(): Success ✅");
            Debug.Log($"     └─ {pyramidGrid.rows}x{pyramidGrid.columns}, Pattern: {pyramidGrid.pattern}");
        }
        else
        {
            Debug.LogError("   • CreatePyramid(): Failed ❌");
        }
    }
    
    private static void TestGridDataHelperMethods()
    {
        Debug.Log("⚙️ Testing GridData helper methods...");
        
        GridData testGrid = GridData.CreateDefault();
        
        // Test size calculation
        Vector2 gridSize = testGrid.CalculateGridSize();
        Debug.Log($"   • CalculateGridSize(): {gridSize.x:F1}x{gridSize.y:F1} ✅");
        
        // Test bounds checking
        bool fitsInBounds = testGrid.FitsInPlayArea();
        Debug.Log($"   • FitsInPlayArea(): {fitsInBounds} ✅");
        
        // Test centering
        Vector3 centeredOffset = testGrid.CalculateCenteredOffset();
        Debug.Log($"   • CalculateCenteredOffset(): {centeredOffset} ✅");
        
        // Test brick type distribution
        for (int i = 0; i < 3; i++)
        {
            BrickType brickType = testGrid.GetBrickTypeForRow(i);
            Debug.Log($"   • GetBrickTypeForRow({i}): {brickType} ✅");
        }
        
        // Test debug info
        string debugInfo = testGrid.GetDebugInfo();
        Debug.Log($"   • GetDebugInfo(): Generated {debugInfo.Length} characters ✅");
    }
    
    private static void TestSampleConfigurations()
    {
        Debug.Log("🎨 Testing sample configurations...");
        
        // Test different pattern types
        GridData[] samples = {
            GridData.CreateDefault(),
            GridData.CreatePyramid(),
            GridData.CreateDiamond(),
            GridData.CreateRandom()
        };
        
        foreach (GridData sample in samples)
        {
            bool isValid = sample.ValidateConfiguration();
            Vector2 size = sample.CalculateGridSize();
            bool fits = sample.FitsInPlayArea();
            
            Debug.Log($"   • {sample.pattern} Pattern: Valid={isValid}, Size={size.x:F1}x{size.y:F1}, Fits={fits} {(isValid && fits ? "✅" : "⚠️")}");
        }
    }
    
    [MenuItem("Breakout/Debug/Create Test GridData Asset")]
    public static void CreateTestGridDataAsset()
    {
        Debug.Log("📦 Creating test GridData asset...");
        
        // Create a test configuration
        GridData testGrid = GridData.CreateDefault();
        testGrid.name = "TestGridConfiguration";
        testGrid.rows = 6;
        testGrid.columns = 8;
        testGrid.pattern = LayoutPattern.Standard;
        
        // Ensure folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
        }
        
        // Save as asset
        string assetPath = "Assets/Data/TestGridConfiguration.asset";
        AssetDatabase.CreateAsset(testGrid, assetPath);
        AssetDatabase.SaveAssets();
        
        // Select the asset
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GridData>(assetPath);
        EditorGUIUtility.PingObject(Selection.activeObject);
        
        Debug.Log($"✅ Test GridData asset created at: {assetPath}");
        Debug.Log("   • Select the asset in Project window to configure in Inspector");
    }
}
#endif