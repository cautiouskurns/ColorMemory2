#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for Task 1.1.1.2 - GameGrid GameObject Creation
/// Creates a GameGrid GameObject as UI container for the 2x2 color square grid layout.
/// Establishes proper parent-child hierarchy with Canvas and configures center positioning.
/// 
/// Usage: Color Memory/Setup/Create GameGrid
/// Dependencies: Canvas GameObject (from Task 1.1.1.1)
/// </summary>
public static class CreateGameGridSetup
{
    // GameGrid sizing configuration for 2x2 color square layout
    private const float GAMEGRID_WIDTH = 400f;
    private const float GAMEGRID_HEIGHT = 400f;
    
    /// <summary>
    /// Creates the GameGrid GameObject with proper Canvas hierarchy and center positioning.
    /// Task ID: 1.1.1.2 - GameGrid GameObject Creation
    /// </summary>
    [MenuItem("Color Memory/Setup/Create GameGrid")]
    public static void CreateGameGrid()
    {
        Debug.Log("🎮 [Task 1.1.1.2] Starting GameGrid GameObject creation...");
        
        try
        {
            // Step 1: Validate Canvas dependency exists
            GameObject canvasGO = ValidateCanvasDependency();
            if (canvasGO == null) return;
            
            // Step 2: Create GameGrid GameObject with proper naming
            GameObject gameGridGO = CreateGameGridGameObject();
            
            // Step 3: Configure RectTransform for center positioning
            ConfigureGameGridTransform(gameGridGO);
            
            // Step 4: Establish parent-child hierarchy with Canvas
            EstablishHierarchyRelationship(gameGridGO, canvasGO);
            
            // Step 5: Configure initial sizing for 2x2 grid layout
            ConfigureInitialSizing(gameGridGO);
            
            // Step 6: Final validation and selection
            ValidateGameGridSetup(gameGridGO);
            Selection.activeGameObject = gameGridGO;
            
            LogSuccessfulSetup(gameGridGO);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.1.2] Failed to create GameGrid: {e.Message}");
            Debug.LogError("💡 Please ensure Canvas exists (complete Task 1.1.1.1 first)");
            Debug.LogError("📋 Troubleshooting: Check Unity UI package is properly installed");
        }
    }
    
    /// <summary>
    /// Validates that GameGrid can be created (Canvas exists, no GameGrid duplicate).
    /// </summary>
    [MenuItem("Color Memory/Setup/Create GameGrid", true)]
    public static bool ValidateCreateGameGrid()
    {
        GameObject existingGameGrid = GameObject.Find("GameGrid");
        GameObject canvasGO = GameObject.Find("Canvas");
        
        if (existingGameGrid != null)
        {
            Debug.LogWarning("⚠️ [Task 1.1.1.2] GameGrid already exists in scene. Delete existing or use current GameGrid.");
            Debug.LogWarning($"📍 Existing GameGrid found at: {GetGameObjectPath(existingGameGrid)}");
        }
        
        if (canvasGO == null)
        {
            Debug.LogWarning("⚠️ [Task 1.1.1.2] Canvas dependency missing. Complete Task 1.1.1.1 first.");
            Debug.LogWarning("💡 Run: Color Memory/Setup/Create Canvas Hierarchy");
        }
        
        return existingGameGrid == null && canvasGO != null;
    }
    
    /// <summary>
    /// Validates that Canvas GameObject exists as required dependency.
    /// </summary>
    private static GameObject ValidateCanvasDependency()
    {
        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null)
        {
            Debug.LogError("❌ [Dependency Error] Canvas GameObject not found!");
            Debug.LogError("📋 Required: Complete Task 1.1.1.1 (Canvas Hierarchy Setup) first");
            Debug.LogError("💡 Run: Color Memory/Setup/Create Canvas Hierarchy");
            return null;
        }
        
        // Validate Canvas has required components
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        
        if (canvas == null || canvasRect == null)
        {
            Debug.LogError("❌ [Dependency Error] Canvas missing required components!");
            Debug.LogError("📋 Canvas must have Canvas and RectTransform components");
            return null;
        }
        
        Debug.Log("✅ [Step 1/5] Canvas dependency validated successfully");
        Debug.Log($"   • Canvas found: {canvasGO.name}");
        Debug.Log($"   • Render Mode: {canvas.renderMode}");
        return canvasGO;
    }
    
    /// <summary>
    /// Creates the GameGrid GameObject with proper naming and UI layer assignment.
    /// </summary>
    private static GameObject CreateGameGridGameObject()
    {
        GameObject gameGridGO = new GameObject("GameGrid");
        gameGridGO.layer = LayerMask.NameToLayer("UI");
        
        // Add RectTransform component (required for UI GameObjects)
        RectTransform rectTransform = gameGridGO.AddComponent<RectTransform>();
        
        Debug.Log("📦 [Step 2/5] GameGrid GameObject created");
        Debug.Log($"   • Name: {gameGridGO.name}");
        Debug.Log($"   • Layer: UI");
        Debug.Log($"   • RectTransform: Added");
        
        return gameGridGO;
    }
    
    /// <summary>
    /// Configures GameGrid RectTransform for center positioning within Canvas.
    /// </summary>
    private static void ConfigureGameGridTransform(GameObject gameGridGO)
    {
        RectTransform rectTransform = gameGridGO.GetComponent<RectTransform>();
        
        // Set anchors to center of Canvas
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        
        // Center position within Canvas
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        
        Debug.Log("📐 [Step 3/5] GameGrid RectTransform configured for center positioning");
        Debug.Log($"   • Anchors: Center (0.5, 0.5)");
        Debug.Log($"   • Pivot: Center (0.5, 0.5)");
        Debug.Log($"   • Position: (0, 0) - Canvas center");
    }
    
    /// <summary>
    /// Establishes proper parent-child hierarchy with Canvas GameObject.
    /// </summary>
    private static void EstablishHierarchyRelationship(GameObject gameGridGO, GameObject canvasGO)
    {
        gameGridGO.transform.SetParent(canvasGO.transform, false);
        
        Debug.Log("🏗️ [Step 4/5] Parent-child hierarchy established");
        Debug.Log($"   • Parent: {canvasGO.name}");
        Debug.Log($"   • Child: {gameGridGO.name}");
        Debug.Log($"   • Hierarchy: Canvas/GameGrid");
        Debug.Log($"   • World Position Stay: false (maintains UI positioning)");
    }
    
    /// <summary>
    /// Configures initial sizing appropriate for 2x2 color square grid layout.
    /// </summary>
    private static void ConfigureInitialSizing(GameObject gameGridGO)
    {
        RectTransform rectTransform = gameGridGO.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(GAMEGRID_WIDTH, GAMEGRID_HEIGHT);
        
        Debug.Log("📏 [Step 5/5] Initial sizing configured for 2x2 grid layout");
        Debug.Log($"   • Width: {GAMEGRID_WIDTH}px");
        Debug.Log($"   • Height: {GAMEGRID_HEIGHT}px");
        Debug.Log($"   • Size Delta: ({GAMEGRID_WIDTH}, {GAMEGRID_HEIGHT})");
        Debug.Log($"   • Ready for Grid Layout Group attachment (Task 1.1.1.3)");
    }
    
    /// <summary>
    /// Validates that GameGrid is properly configured with all required components.
    /// </summary>
    private static void ValidateGameGridSetup(GameObject gameGridGO)
    {
        RectTransform rectTransform = gameGridGO.GetComponent<RectTransform>();
        Transform parent = gameGridGO.transform.parent;
        
        bool isValid = rectTransform != null && 
                      parent != null && 
                      parent.name == "Canvas" &&
                      gameGridGO.layer == LayerMask.NameToLayer("UI");
        
        if (isValid)
        {
            Debug.Log("✅ [Validation] GameGrid setup properly configured and validated");
            Debug.Log($"   • RectTransform: Present");
            Debug.Log($"   • Parent: {parent.name} (correct)");
            Debug.Log($"   • Layer: UI (correct)");
        }
        else
        {
            Debug.LogError("❌ [Validation] GameGrid setup incomplete - validation failed");
        }
    }
    
    /// <summary>
    /// Logs comprehensive success message with configuration summary.
    /// </summary>
    private static void LogSuccessfulSetup(GameObject gameGridGO)
    {
        Debug.Log("✅ [Task 1.1.1.2] GameGrid GameObject created successfully!");
        Debug.Log("📋 GameGrid Configuration Summary:");
        Debug.Log($"   • GameObject Name: {gameGridGO.name}");
        Debug.Log($"   • Layer: UI");
        Debug.Log($"   • Parent: Canvas (proper hierarchy established)");
        Debug.Log($"   • Anchors: Center (0.5, 0.5) for responsive positioning");
        Debug.Log($"   • Size: {GAMEGRID_WIDTH}x{GAMEGRID_HEIGHT}px (optimized for 2x2 grid)");
        Debug.Log($"   • Position: Canvas center (0, 0)");
        Debug.Log("🚀 Ready for next steps:");
        Debug.Log("   → Grid Layout Group component attachment (Task 1.1.1.3)");
        Debug.Log("   → ColorSquare GameObject creation (Feature 1.1.2)");
        Debug.Log("   → 2x2 grid layout configuration and spacing setup");
        Debug.Log("   → Integration with Canvas responsive scaling system");
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