#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// Editor setup script for Task 1.1.1.3 - Grid Layout Group Configuration
/// Configures Grid Layout Group component on GameGrid GameObject to automatically arrange
/// four ColorSquare child objects in a precise 2x2 grid pattern with optimal spacing.
/// 
/// Usage: Color Memory/Setup/Create Grid Layout Group
/// Dependencies: GameGrid GameObject (from Task 1.1.1.2)
/// </summary>
public static class CreateGridLayoutGroupSetup
{
    // Grid Layout Group configuration constants for 2x2 color square arrangement
    private const float CELL_SIZE = 160f;           // Square cell size for color squares
    private const float SPACING = 20f;              // Spacing between grid cells
    private const int COLUMN_COUNT = 2;             // Fixed 2x2 grid constraint
    
    /// <summary>
    /// Configures Grid Layout Group component on GameGrid for 2x2 color square arrangement.
    /// Task ID: 1.1.1.3 - Grid Layout Group Configuration
    /// </summary>
    [MenuItem("Color Memory/Setup/Create Grid Layout Group")]
    public static void CreateGridLayoutGroup()
    {
        Debug.Log("🎯 [Task 1.1.1.3] Starting Grid Layout Group configuration...");
        
        try
        {
            // Step 1: Validate GameGrid dependency exists
            GameObject gameGridGO = ValidateGameGridDependency();
            if (gameGridGO == null) return;
            
            // Step 2: Attach Grid Layout Group component
            GridLayoutGroup gridLayout = AttachGridLayoutComponent(gameGridGO);
            
            // Step 3: Configure Fixed Column Count constraint for 2x2 arrangement
            ConfigureConstraintSettings(gridLayout);
            
            // Step 4: Set cell size and spacing for optimal color square display
            ConfigureCellSizeAndSpacing(gridLayout);
            
            // Step 5: Configure start corner, axis, and child alignment settings
            ConfigureLayoutArrangement(gridLayout);
            
            // Step 6: Final validation and selection
            ValidateGridLayoutSetup(gridLayout);
            Selection.activeGameObject = gameGridGO;
            
            LogSuccessfulSetup(gameGridGO, gridLayout);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.1.3] Failed to configure Grid Layout Group: {e.Message}");
            Debug.LogError("💡 Please ensure GameGrid exists (complete Task 1.1.1.2 first)");
            Debug.LogError("📋 Troubleshooting: Check Unity UI package is properly installed");
        }
    }
    
    /// <summary>
    /// Validates that Grid Layout Group can be configured (GameGrid exists, no duplicate component).
    /// </summary>
    [MenuItem("Color Memory/Setup/Create Grid Layout Group", true)]
    public static bool ValidateCreateGridLayoutGroup()
    {
        GameObject gameGridGO = GameObject.Find("GameGrid");
        
        if (gameGridGO == null)
        {
            Debug.LogWarning("⚠️ [Task 1.1.1.3] GameGrid dependency missing. Complete Task 1.1.1.2 first.");
            Debug.LogWarning("💡 Run: Color Memory/Setup/Create GameGrid");
            return false;
        }
        
        GridLayoutGroup existingComponent = gameGridGO.GetComponent<GridLayoutGroup>();
        if (existingComponent != null)
        {
            Debug.LogWarning("⚠️ [Task 1.1.1.3] Grid Layout Group already configured on GameGrid.");
            Debug.LogWarning("📍 Existing component found - use current configuration or remove to recreate");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Validates that GameGrid GameObject exists as required dependency.
    /// </summary>
    private static GameObject ValidateGameGridDependency()
    {
        GameObject gameGridGO = GameObject.Find("GameGrid");
        if (gameGridGO == null)
        {
            Debug.LogError("❌ [Dependency Error] GameGrid GameObject not found!");
            Debug.LogError("📋 Required: Complete Task 1.1.1.2 (GameGrid GameObject Creation) first");
            Debug.LogError("💡 Run: Color Memory/Setup/Create GameGrid");
            return null;
        }
        
        // Validate GameGrid has required components
        RectTransform rectTransform = gameGridGO.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("❌ [Dependency Error] GameGrid missing RectTransform component!");
            Debug.LogError("📋 GameGrid must have RectTransform for UI layout functionality");
            return null;
        }
        
        // Validate GameGrid is child of Canvas
        Transform parent = gameGridGO.transform.parent;
        if (parent == null || parent.name != "Canvas")
        {
            Debug.LogError("❌ [Dependency Error] GameGrid not properly parented to Canvas!");
            Debug.LogError($"📋 Current parent: {(parent != null ? parent.name : "None")} (should be Canvas)");
            return null;
        }
        
        Debug.Log("✅ [Step 1/5] GameGrid dependency validated successfully");
        Debug.Log($"   • GameGrid found: {gameGridGO.name}");
        Debug.Log($"   • Parent: {parent.name} (correct)");
        Debug.Log($"   • RectTransform: Present");
        return gameGridGO;
    }
    
    /// <summary>
    /// Attaches Grid Layout Group component to GameGrid GameObject.
    /// </summary>
    private static GridLayoutGroup AttachGridLayoutComponent(GameObject gameGridGO)
    {
        GridLayoutGroup gridLayout = gameGridGO.AddComponent<GridLayoutGroup>();
        
        Debug.Log("🔧 [Step 2/5] Grid Layout Group component attached");
        Debug.Log($"   • Component: {gridLayout.GetType().Name}");
        Debug.Log($"   • Target GameObject: {gameGridGO.name}");
        Debug.Log($"   • Ready for 2x2 configuration");
        
        return gridLayout;
    }
    
    /// <summary>
    /// Configures Fixed Column Count constraint for 2x2 grid arrangement.
    /// </summary>
    private static void ConfigureConstraintSettings(GridLayoutGroup gridLayout)
    {
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = COLUMN_COUNT;
        
        Debug.Log("📊 [Step 3/5] Constraint settings configured for 2x2 arrangement");
        Debug.Log($"   • Constraint Type: {gridLayout.constraint}");
        Debug.Log($"   • Column Count: {COLUMN_COUNT} (enforces 2x2 grid)");
        Debug.Log($"   • Automatic row creation: Enabled for 4 ColorSquare objects");
    }
    
    /// <summary>
    /// Configures cell size and spacing for optimal color square display.
    /// </summary>
    private static void ConfigureCellSizeAndSpacing(GridLayoutGroup gridLayout)
    {
        gridLayout.cellSize = new Vector2(CELL_SIZE, CELL_SIZE);
        gridLayout.spacing = new Vector2(SPACING, SPACING);
        
        Debug.Log("📏 [Step 4/5] Cell size and spacing configured");
        Debug.Log($"   • Cell Size: {CELL_SIZE}x{CELL_SIZE}px (square cells for color squares)");
        Debug.Log($"   • Spacing: {SPACING}px horizontal and vertical");
        Debug.Log($"   • Total Grid Size: {(CELL_SIZE * 2) + SPACING}x{(CELL_SIZE * 2) + SPACING}px");
        Debug.Log($"   • Optimized for visual separation and touch targets");
    }
    
    /// <summary>
    /// Configures start corner, start axis, and child alignment for proper arrangement.
    /// </summary>
    private static void ConfigureLayoutArrangement(GridLayoutGroup gridLayout)
    {
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        
        Debug.Log("🧭 [Step 5/5] Layout arrangement configured");
        Debug.Log($"   • Start Corner: {gridLayout.startCorner} (standard top-left origin)");
        Debug.Log($"   • Start Axis: {gridLayout.startAxis} (fill horizontally first)");
        Debug.Log($"   • Child Alignment: {gridLayout.childAlignment} (centered positioning)");
        Debug.Log($"   • Arrangement Order: Top-Left → Top-Right → Bottom-Left → Bottom-Right");
    }
    
    /// <summary>
    /// Validates that Grid Layout Group is properly configured with all required settings.
    /// </summary>
    private static void ValidateGridLayoutSetup(GridLayoutGroup gridLayout)
    {
        bool isValid = gridLayout != null &&
                      gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount &&
                      gridLayout.constraintCount == COLUMN_COUNT &&
                      gridLayout.cellSize == new Vector2(CELL_SIZE, CELL_SIZE) &&
                      gridLayout.spacing == new Vector2(SPACING, SPACING);
        
        if (isValid)
        {
            Debug.Log("✅ [Validation] Grid Layout Group properly configured and validated");
            Debug.Log($"   • Component: Present and configured");
            Debug.Log($"   • Constraint: Fixed Column Count = {COLUMN_COUNT}");
            Debug.Log($"   • Cell Configuration: Valid");
            Debug.Log($"   • Spacing Configuration: Valid");
        }
        else
        {
            Debug.LogError("❌ [Validation] Grid Layout Group setup incomplete - validation failed");
        }
    }
    
    /// <summary>
    /// Logs comprehensive success message with configuration summary.
    /// </summary>
    private static void LogSuccessfulSetup(GameObject gameGridGO, GridLayoutGroup gridLayout)
    {
        Debug.Log("✅ [Task 1.1.1.3] Grid Layout Group configured successfully!");
        Debug.Log("📋 Grid Layout Configuration Summary:");
        Debug.Log($"   • Target GameObject: {gameGridGO.name}");
        Debug.Log($"   • Component: Grid Layout Group (attached and configured)");
        Debug.Log($"   • Layout Constraint: Fixed Column Count = {COLUMN_COUNT} (2x2 arrangement)");
        Debug.Log($"   • Cell Size: {CELL_SIZE}x{CELL_SIZE}px (square cells for color squares)");
        Debug.Log($"   • Spacing: {SPACING}px (visual separation between squares)");
        Debug.Log($"   • Start Corner: Upper Left (standard grid origin)");
        Debug.Log($"   • Start Axis: Horizontal (left-to-right, then top-to-bottom)");
        Debug.Log($"   • Child Alignment: Middle Center (centered positioning)");
        Debug.Log($"   • Total Grid Dimensions: {(CELL_SIZE * 2) + SPACING}x{(CELL_SIZE * 2) + SPACING}px");
        Debug.Log("🚀 Ready for next steps:");
        Debug.Log("   → ColorSquare GameObject creation (Feature 1.1.2)");
        Debug.Log("   → Automatic 2x2 arrangement of 4 ColorSquare child objects");
        Debug.Log("   → Color assignment (Red, Blue, Green, Yellow)");
        Debug.Log("   → Click detection integration (Feature 1.1.3)");
        Debug.Log("   → Grid Layout Group will automatically position ColorSquares");
    }
    
    /// <summary>
    /// Utility method to get full hierarchy path of a GameObject.
    /// </summary>
    private static string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
}
#endif