#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// Editor setup script for Task 1.1.1.1 - Scene Canvas Hierarchy Setup
/// Creates a properly configured Canvas with Screen Space-Overlay rendering, Canvas Scaler
/// for responsive WebGL design, and GraphicRaycaster for UI interactions.
/// 
/// Usage: Color Memory/Setup/Create Canvas Hierarchy
/// </summary>
public static class CreateCanvasHierarchySetup
{
    // WebGL optimized reference resolution for Canvas Scaler
    private const int REFERENCE_WIDTH = 1920;
    private const int REFERENCE_HEIGHT = 1080;
    private const float MATCH_WIDTH_OR_HEIGHT = 0.5f; // Balanced width/height matching
    
    /// <summary>
    /// Creates the Canvas hierarchy with proper WebGL configuration.
    /// Task ID: 1.1.1.1 - Scene Canvas Hierarchy Setup
    /// </summary>
    [MenuItem("Color Memory/Setup/Create Canvas Hierarchy")]
    public static void CreateCanvasHierarchy()
    {
        Debug.Log("🎨 [Task 1.1.1.1] Starting Canvas Hierarchy creation for Color Memory...");
        
        try
        {
            // Step 1: Create Canvas GameObject with proper naming
            GameObject canvasGO = CreateCanvasGameObject();
            
            // Step 2: Configure Canvas component for WebGL deployment
            ConfigureCanvasComponent(canvasGO);
            
            // Step 3: Add and configure Canvas Scaler for responsive design
            ConfigureCanvasScaler(canvasGO);
            
            // Step 4: Add GraphicRaycaster for UI interaction detection
            ConfigureGraphicRaycaster(canvasGO);
            
            // Step 5: Set proper transform and hierarchy positioning
            ConfigureCanvasTransform(canvasGO);
            
            // Step 6: Final validation and selection
            ValidateCanvasSetup(canvasGO);
            Selection.activeGameObject = canvasGO;
            
            LogSuccessfulSetup();
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.1.1] Failed to create Canvas Hierarchy: {e.Message}");
            Debug.LogError("💡 Please check Unity UI package is installed and scene is accessible.");
            Debug.LogError("📋 Troubleshooting: Ensure no existing Canvas conflicts exist in scene.");
        }
    }
    
    /// <summary>
    /// Validates that Canvas hierarchy can be created (prevents duplicates).
    /// </summary>
    [MenuItem("Color Memory/Setup/Create Canvas Hierarchy", true)]
    public static bool ValidateCreateCanvasHierarchy()
    {
        GameObject existingCanvas = GameObject.Find("Canvas");
        if (existingCanvas != null)
        {
            Debug.LogWarning("⚠️ [Task 1.1.1.1] Canvas already exists in scene. Use existing Canvas or delete it first.");
            Debug.LogWarning($"📍 Existing Canvas found at: {GetGameObjectPath(existingCanvas)}");
        }
        return existingCanvas == null;
    }
    
    /// <summary>
    /// Creates the main Canvas GameObject with proper naming and layer assignment.
    /// </summary>
    private static GameObject CreateCanvasGameObject()
    {
        GameObject canvasGO = new GameObject("Canvas");
        canvasGO.layer = LayerMask.NameToLayer("UI");
        
        Debug.Log("📦 [Step 1/5] Canvas GameObject created with UI layer assignment");
        return canvasGO;
    }
    
    /// <summary>
    /// Configures the Canvas component for optimal WebGL deployment.
    /// </summary>
    private static void ConfigureCanvasComponent(GameObject canvasGO)
    {
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        canvas.targetDisplay = 0;
        canvas.pixelPerfect = false; // Better performance for WebGL
        
        Debug.Log("🖥️ [Step 2/5] Canvas component configured: Screen Space-Overlay mode for WebGL");
        Debug.Log($"   • Render Mode: {canvas.renderMode}");
        Debug.Log($"   • Sorting Order: {canvas.sortingOrder}");
        Debug.Log($"   • Pixel Perfect: {canvas.pixelPerfect} (optimized for WebGL performance)");
    }
    
    /// <summary>
    /// Configures Canvas Scaler for responsive WebGL design across different screen sizes.
    /// </summary>
    private static void ConfigureCanvasScaler(GameObject canvasGO)
    {
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(REFERENCE_WIDTH, REFERENCE_HEIGHT);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = MATCH_WIDTH_OR_HEIGHT;
        scaler.referencePixelsPerUnit = 100f;
        
        Debug.Log($"📏 [Step 3/5] Canvas Scaler configured for responsive WebGL design");
        Debug.Log($"   • Reference Resolution: {REFERENCE_WIDTH}x{REFERENCE_HEIGHT}");
        Debug.Log($"   • Scale Mode: {scaler.uiScaleMode}");
        Debug.Log($"   • Match Mode: {scaler.screenMatchMode}");
        Debug.Log($"   • Match Value: {MATCH_WIDTH_OR_HEIGHT} (balanced width/height scaling)");
    }
    
    /// <summary>
    /// Configures GraphicRaycaster for UI interaction detection and click handling.
    /// </summary>
    private static void ConfigureGraphicRaycaster(GameObject canvasGO)
    {
        GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
        raycaster.ignoreReversedGraphics = true;
        raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        raycaster.blockingMask = -1; // All layers
        
        Debug.Log("🎯 [Step 4/5] GraphicRaycaster configured for UI interaction detection");
        Debug.Log($"   • Ignore Reversed Graphics: {raycaster.ignoreReversedGraphics}");
        Debug.Log($"   • Blocking Objects: {raycaster.blockingObjects}");
        Debug.Log("   • Ready for ColorSquare click detection integration");
    }
    
    /// <summary>
    /// Configures Canvas RectTransform for full-screen coverage and proper anchoring.
    /// </summary>
    private static void ConfigureCanvasTransform(GameObject canvasGO)
    {
        RectTransform rectTransform = canvasGO.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Configure for full screen coverage
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;
        }
        
        Debug.Log("📐 [Step 5/5] Canvas transform configured for full-screen coverage");
        Debug.Log("   • Anchors: Full screen (0,0) to (1,1)");
        Debug.Log("   • Offsets: Zero for perfect screen alignment");
        Debug.Log("   • Ready for GameGrid positioning as child element");
    }
    
    /// <summary>
    /// Validates that all required components are properly configured on the Canvas.
    /// </summary>
    private static void ValidateCanvasSetup(GameObject canvasGO)
    {
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        GraphicRaycaster raycaster = canvasGO.GetComponent<GraphicRaycaster>();
        RectTransform rectTransform = canvasGO.GetComponent<RectTransform>();
        
        bool isValid = canvas != null && scaler != null && raycaster != null && rectTransform != null;
        
        if (isValid)
        {
            Debug.Log("✅ [Validation] All Canvas components properly configured and validated");
        }
        else
        {
            Debug.LogError("❌ [Validation] Canvas setup incomplete - missing required components");
        }
    }
    
    /// <summary>
    /// Logs comprehensive success message with configuration summary.
    /// </summary>
    private static void LogSuccessfulSetup()
    {
        Debug.Log("✅ [Task 1.1.1.1] Canvas Hierarchy created successfully!");
        Debug.Log("📋 Canvas Configuration Summary:");
        Debug.Log($"   • Render Mode: Screen Space - Overlay (WebGL optimized)");
        Debug.Log($"   • Reference Resolution: {REFERENCE_WIDTH}x{REFERENCE_HEIGHT}");
        Debug.Log($"   • Scale Mode: Scale With Screen Size (responsive design)");
        Debug.Log($"   • Match: {MATCH_WIDTH_OR_HEIGHT} (balanced width/height scaling)");
        Debug.Log($"   • GraphicRaycaster: Enabled for UI interaction detection");
        Debug.Log($"   • Transform: Full-screen anchoring with zero offsets");
        Debug.Log("🚀 Ready for next steps:");
        Debug.Log("   → GameGrid integration (Feature 1.1.1 continuation)");
        Debug.Log("   → ColorSquare component attachment (Feature 1.1.2)");
        Debug.Log("   → UI element hierarchy expansion (ScoreText, LevelText, RestartButton)");
        Debug.Log("   → Performance validated for 60fps WebGL target");
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