#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Simple test script to verify Grid Validation system compilation
/// </summary>
public static class TestGridValidation
{
    [MenuItem("Breakout/Test/Validate Grid Compilation")]
    public static void TestValidationCompilation()
    {
        Debug.Log("🧪 Testing Grid Validation System Compilation...");
        
        BrickGrid brickGrid = Object.FindFirstObjectByType<BrickGrid>();
        if (brickGrid == null)
        {
            Debug.LogWarning("⚠️ No BrickGrid found in scene for testing");
            return;
        }
        
        try
        {
            // Test property access
            bool isGenerated = brickGrid.IsGridGenerated;
            GridData config = brickGrid.GridConfiguration;
            GameObject prefab = brickGrid.BrickPrefab;
            GameObject container = brickGrid.GridContainer;
            var instantiatedBricks = brickGrid.InstantiatedBricks;
            
            Debug.Log($"✅ Property access test passed:");
            Debug.Log($"   • IsGridGenerated: {isGenerated}");
            Debug.Log($"   • GridConfiguration: {(config != null ? config.name : "null")}");
            Debug.Log($"   • BrickPrefab: {(prefab != null ? prefab.name : "null")}");
            Debug.Log($"   • GridContainer: {(container != null ? container.name : "null")}");
            Debug.Log($"   • InstantiatedBricks count: {instantiatedBricks.Count}");
            
            // Test validation methods
            bool configValid = brickGrid.ValidateGridConfiguration();
            bool boundsValid = brickGrid.ValidateGridBounds();
            
            Debug.Log($"✅ Validation methods test passed:");
            Debug.Log($"   • ValidateGridConfiguration(): {configValid}");
            Debug.Log($"   • ValidateGridBounds(): {boundsValid}");
            
            if (isGenerated)
            {
                bool gridValid = brickGrid.ValidateGeneratedGrid();
                bool overallValid = brickGrid.ValidateGrid();
                
                Debug.Log($"   • ValidateGeneratedGrid(): {gridValid}");
                Debug.Log($"   • ValidateGrid(): {overallValid}");
            }
            
            Debug.Log("✅ Grid Validation System compilation test PASSED");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Grid Validation System compilation test FAILED: {e.Message}");
        }
    }
}
#endif