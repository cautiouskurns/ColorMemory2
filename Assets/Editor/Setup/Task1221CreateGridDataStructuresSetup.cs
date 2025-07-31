#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for creating Grid Configuration Data Structures and default assets.
/// Provides automated setup for GridData ScriptableObject assets with various layout patterns.
/// </summary>
public static class Task1221CreateGridDataStructuresSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1221 Create Grid Data Structures";
    private const string DATA_FOLDER = "Assets/Data";
    private const string GRID_DATA_FOLDER = "Assets/Data/GridConfigurations";
    
    /// <summary>
    /// Creates Grid Data Structures and default configuration assets.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateGridDataStructures()
    {
        Debug.Log("🔧 [Task 1.2.2.1] Starting Grid Data Structures creation...");
        
        try
        {
            // Step 1: Validate dependencies and setup
            ValidateDependencies();
            
            // Step 2: Create folder structure
            CreateFolderStructure();
            
            // Step 3: Create default GridData assets
            CreateDefaultGridDataAssets();
            
            // Step 4: Create sample pattern configurations
            CreateSamplePatternConfigurations();
            
            // Step 5: Validate created assets
            ValidateCreatedAssets();
            
            // Step 6: Save assets and log success
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            LogSuccessfulSetup();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.2.1] Grid Data Structures creation failed: {e.Message}");
            Debug.LogError("📋 Please ensure BrickData structures are available from previous tasks");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if assets don't already exist
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateGridDataStructures()
    {
        // Check if main default asset already exists
        return !AssetDatabase.LoadAssetAtPath<GridData>($"{GRID_DATA_FOLDER}/DefaultGridData.asset");
    }
    
    /// <summary>
    /// Validates that all dependencies are available for grid data structures
    /// </summary>
    private static void ValidateDependencies()
    {
        Debug.Log("🔍 [Step 1/6] Validating dependencies...");
        
        // Check if BrickType enum is available
        try
        {
            // Test BrickType enum availability
            BrickType _ = BrickType.Normal; // Test enum access
            Debug.Log("   • BrickType enum: Available and functional ✅");
            
            // Test all enum values
            foreach (BrickType enumValue in System.Enum.GetValues(typeof(BrickType)))
            {
                Debug.Log($"   • BrickType.{enumValue}: Available ✅");
            }
        }
        catch (System.Exception e)
        {
            throw new System.Exception($"BrickType enum validation failed: {e.Message}. Please ensure BrickData from Task 1.2.1.1 is implemented.");
        }
        
        // Check if GridData script compiles properly
        try
        {
            System.Type gridDataType = typeof(GridData);
            System.Type layoutPatternType = typeof(LayoutPattern);
            
            if (gridDataType != null && layoutPatternType != null)
            {
                Debug.Log("   • GridData and LayoutPattern types: Available ✅");
            }
        }
        catch (System.Exception e)
        {
            throw new System.Exception($"GridData type validation failed: {e.Message}");
        }
        
        Debug.Log("✅ [Step 1/6] Dependencies validated successfully");
    }
    
    /// <summary>
    /// Creates required folder structure for grid data assets
    /// </summary>
    private static void CreateFolderStructure()
    {
        Debug.Log("📁 [Step 2/6] Creating folder structure...");
        
        // Create main Data folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(DATA_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
            Debug.Log($"   • Created folder: {DATA_FOLDER}");
        }
        
        // Create GridConfigurations subfolder
        if (!AssetDatabase.IsValidFolder(GRID_DATA_FOLDER))
        {
            AssetDatabase.CreateFolder(DATA_FOLDER, "GridConfigurations");
            Debug.Log($"   • Created folder: {GRID_DATA_FOLDER}");
        }
        
        Debug.Log("✅ [Step 2/6] Folder structure creation complete");
    }
    
    /// <summary>
    /// Creates default GridData ScriptableObject assets
    /// </summary>
    private static void CreateDefaultGridDataAssets()
    {
        Debug.Log("📋 [Step 3/6] Creating default GridData assets...");
        
        // Create default standard grid configuration
        CreateAndSaveGridData(
            GridData.CreateDefault(),
            "DefaultGridData",
            "Standard Breakout grid configuration with 8 rows and 10 columns"
        );
        
        Debug.Log("✅ [Step 3/6] Default GridData assets created successfully");
    }
    
    /// <summary>
    /// Creates sample pattern configurations for testing different layouts
    /// </summary>
    private static void CreateSamplePatternConfigurations()
    {
        Debug.Log("🎨 [Step 4/6] Creating sample pattern configurations...");
        
        // Create pyramid pattern
        CreateAndSaveGridData(
            GridData.CreatePyramid(),
            "PyramidGridData",
            "Pyramid pattern with triangular brick arrangement"
        );
        
        // Create diamond pattern
        CreateAndSaveGridData(
            GridData.CreateDiamond(),
            "DiamondGridData",
            "Diamond pattern with symmetric rhombus arrangement"
        );
        
        // Create random pattern
        CreateAndSaveGridData(
            GridData.CreateRandom(),
            "RandomGridData",
            "Random pattern with procedural brick placement"
        );
        
        // Create custom test pattern
        GridData customGrid = GridData.CreateDefault();
        customGrid.name = "CustomTestGridData";
        customGrid.pattern = LayoutPattern.Custom;
        customGrid.rows = 5;
        customGrid.columns = 15;
        customGrid.horizontalSpacing = 0.9f;
        customGrid.verticalSpacing = 0.8f;
        customGrid.enableStaggering = true;
        customGrid.staggerOffset = 0.45f;
        
        CreateAndSaveGridData(
            customGrid,
            "CustomTestGridData",
            "Custom pattern for testing advanced layout features"
        );
        
        Debug.Log("✅ [Step 4/6] Sample pattern configurations created successfully");
    }
    
    /// <summary>
    /// Helper method to create and save GridData assets
    /// </summary>
    /// <param name="gridData">GridData instance to save</param>
    /// <param name="fileName">File name for the asset</param>
    /// <param name="description">Description for logging</param>
    private static void CreateAndSaveGridData(GridData gridData, string fileName, string description)
    {
        string assetPath = $"{GRID_DATA_FOLDER}/{fileName}.asset";
        
        // Check if asset already exists
        if (AssetDatabase.LoadAssetAtPath<GridData>(assetPath) != null)
        {
            Debug.Log($"   • {fileName}: Already exists, skipping creation");
            return;
        }
        
        // Create and save the asset
        AssetDatabase.CreateAsset(gridData, assetPath);
        
        // Validate the created asset
        if (gridData.ValidateConfiguration())
        {
            Debug.Log($"   • {fileName}: Created and validated ✅");
            Debug.Log($"     └─ {description}");
            Debug.Log($"     └─ Pattern: {gridData.pattern}, Size: {gridData.rows}x{gridData.columns}");
        }
        else
        {
            Debug.LogWarning($"   • {fileName}: Created but validation failed ⚠️");
        }
        
        // Mark asset as dirty for saving
        EditorUtility.SetDirty(gridData);
    }
    
    /// <summary>
    /// Validates that all created assets are properly configured
    /// </summary>
    private static void ValidateCreatedAssets()
    {
        Debug.Log("🧪 [Step 5/6] Validating created assets...");
        
        string[] assetNames = {
            "DefaultGridData",
            "PyramidGridData", 
            "DiamondGridData",
            "RandomGridData",
            "CustomTestGridData"
        };
        
        int validAssets = 0;
        int totalAssets = assetNames.Length;
        
        foreach (string assetName in assetNames)
        {
            string assetPath = $"{GRID_DATA_FOLDER}/{assetName}.asset";
            GridData asset = AssetDatabase.LoadAssetAtPath<GridData>(assetPath);
            
            if (asset != null)
            {
                if (asset.ValidateConfiguration())
                {
                    validAssets++;
                    Debug.Log($"   • {assetName}: Valid configuration ✅");
                    
                    // Log key configuration details
                    Vector2 gridSize = asset.CalculateGridSize();
                    bool fitsInBounds = asset.FitsInPlayArea();
                    Debug.Log($"     └─ Grid Size: {gridSize.x:F1}x{gridSize.y:F1}, Fits in bounds: {fitsInBounds}");
                }
                else
                {
                    Debug.LogWarning($"   • {assetName}: Invalid configuration ⚠️");
                }
            }
            else
            {
                Debug.LogError($"   • {assetName}: Asset not found ❌");
            }
        }
        
        Debug.Log($"   • Validation Score: {validAssets}/{totalAssets} assets valid");
        Debug.Log("✅ [Step 5/6] Asset validation complete");
    }
    
    /// <summary>
    /// Logs successful setup summary with usage instructions
    /// </summary>
    private static void LogSuccessfulSetup()
    {
        Debug.Log("✅ [Task 1.2.2.1] Grid Data Structures creation completed successfully!");
        Debug.Log("📋 Grid Data Structures Summary:");
        Debug.Log("   • Data Structures: LayoutPattern enum and GridData class with comprehensive configuration");
        Debug.Log("   • Unity Integration: Serializable with Inspector organization and CreateAssetMenu support");
        Debug.Log("   • BrickData Integration: Compatible with existing BrickType system from Feature 1.2.1");
        Debug.Log("   • Asset Creation: ScriptableObject workflow with organized folder structure");
        
        Debug.Log("🔧 Created Data Structures:");
        Debug.Log("   → LayoutPattern Enum: Standard, Pyramid, Diamond, Random, Custom patterns");
        Debug.Log("   → GridData Class: Complete configuration system with validation and helper methods");
        Debug.Log("   → Validation System: Data integrity checks and boundary constraint validation");
        Debug.Log("   → Helper Methods: Size calculation, centering, brick type distribution");
        
        Debug.Log("📦 Created Assets:");
        Debug.Log($"   → DefaultGridData: Standard 8x10 Breakout configuration at {GRID_DATA_FOLDER}/DefaultGridData.asset");
        Debug.Log($"   → PyramidGridData: Triangular formation pattern at {GRID_DATA_FOLDER}/PyramidGridData.asset");
        Debug.Log($"   → DiamondGridData: Symmetric diamond arrangement at {GRID_DATA_FOLDER}/DiamondGridData.asset");
        Debug.Log($"   → RandomGridData: Procedural placement pattern at {GRID_DATA_FOLDER}/RandomGridData.asset");
        Debug.Log($"   → CustomTestGridData: Advanced features testing at {GRID_DATA_FOLDER}/CustomTestGridData.asset");
        
        Debug.Log("⚙️ Configuration Features:");
        Debug.Log("   • Grid Dimensions: Rows, columns with validation constraints");
        Debug.Log("   • Spacing Control: Horizontal and vertical spacing with Unity range sliders");
        Debug.Log("   • Layout Patterns: Five different arrangement strategies for level variety");
        Debug.Log("   • Brick Distribution: Per-row brick type configuration for gameplay progression");
        Debug.Log("   • Boundary Management: Play area bounds with automatic centering and edge margins");
        Debug.Log("   • Advanced Options: Brick staggering, density control, custom offsets");
        
        Debug.Log("🎮 Integration Points:");
        Debug.Log("   • BrickGrid Manager: Ready for integration with grid generation system");
        Debug.Log("   • Level Design: ScriptableObject assets for different level configurations");
        Debug.Log("   • Inspector Workflow: Organized sections with tooltips and range validation");
        Debug.Log("   • Runtime Validation: Built-in configuration validation and debugging");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Select GridData assets in Project window to configure in Inspector");
        Debug.Log("   2. Use 'Create > Breakout > Grid Configuration' to create new configurations");
        Debug.Log("   3. Modify pattern, dimensions, spacing, and brick distribution as needed");
        Debug.Log("   4. Call ValidateConfiguration() to check data integrity");
        Debug.Log("   5. Use CalculateGridSize() and FitsInPlayArea() for layout validation");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Implement BrickGrid Manager using these data structures");
        Debug.Log("   → Create level progression system with different GridData configurations");
        Debug.Log("   → Test pattern generation with various layout types");
        Debug.Log("   → Integrate with brick instantiation system using the prefab from Task 1.2.1.7");
        
        Debug.Log("📊 Configuration Examples:");
        Debug.Log("   • Standard Breakout: 8 rows × 10 columns, 1.1f spacing, centered layout");
        Debug.Log("   • Pyramid Challenge: 6 rows × 12 columns, triangular formation, 0.8f density");
        Debug.Log("   • Diamond Formation: 7 rows × 9 columns, symmetric arrangement, 0.9f density");
        Debug.Log("   • Random Layout: 10 rows × 12 columns, procedural placement, 0.6f density");
        Debug.Log("   • Custom Pattern: 5 rows × 15 columns, staggered rows, advanced features");
        
        Debug.Log("⚠️ Integration Notes:");
        Debug.Log("   → GridData assets are ScriptableObjects - reference them in BrickGrid Manager");
        Debug.Log("   → Use GetBrickTypeForRow() for consistent brick type distribution");
        Debug.Log("   → Call CalculateCenteredOffset() for automatic grid positioning");
        Debug.Log("   → Validate configurations before runtime using ValidateConfiguration()");
        Debug.Log("   → All data structures are serializable for Inspector and asset workflow");
    }
    
    /// <summary>
    /// Utility method to clean up created assets for fresh testing
    /// </summary>
    [MenuItem("Breakout/Setup/Clean Grid Data Assets", false, 1001)]
    public static void CleanGridDataAssets()
    {
        Debug.Log("🧹 [Cleanup] Removing Grid Data assets...");
        
        string[] assetNames = {
            "DefaultGridData",
            "PyramidGridData",
            "DiamondGridData", 
            "RandomGridData",
            "CustomTestGridData"
        };
        
        int removedCount = 0;
        
        foreach (string assetName in assetNames)
        {
            string assetPath = $"{GRID_DATA_FOLDER}/{assetName}.asset";
            if (File.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
                removedCount++;
                Debug.Log($"   • Removed: {assetName}");
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log($"🧹 [Cleanup] Removed {removedCount} Grid Data assets");
    }
}
#endif