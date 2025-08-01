#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// Editor setup script for creating camera-boundary integration system.
/// Configures camera bounds calculation and ensures boundary walls align with visible game area.
/// Provides visual debugging tools for development and testing.
/// </summary>
public static class CreateCameraIntegrationSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Camera Integration";
    private const string BOUNDARY_SYSTEM_NAME = "Boundary System";
    private const string MAIN_CAMERA_NAME = "Main Camera";
    
    /// <summary>
    /// Creates camera integration system for boundary alignment.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateCameraIntegration()
    {
        Debug.Log("🏗️ [Camera Integration] Starting camera-boundary integration setup...");
        
        try
        {
            // Step 1: Check prerequisites
            GameObject boundarySystem = ValidatePrerequisites();
            
            // Step 2: Find or create main camera
            Camera gameCamera = SetupGameCamera();
            
            // Step 3: Add integration component
            CameraBoundaryIntegration integration = AddIntegrationComponent(boundarySystem);
            
            // Step 4: Configure integration settings
            ConfigureIntegration(integration, gameCamera);
            
            // Step 5: Calculate initial bounds and verify alignment
            PerformInitialCalculations(integration);
            
            // Step 6: Save and refresh
            SaveAndRefreshAssets();
            
            // Step 7: Log success and instructions
            LogSuccessfulSetup(integration);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Camera Integration] Setup failed: {e.Message}");
            Debug.LogError($"📋 Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if boundary system exists and integration not present.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateCameraIntegration()
    {
        // Check if Boundary System exists
        GameObject boundarySystem = GameObject.Find(BOUNDARY_SYSTEM_NAME);
        if (boundarySystem == null) return false;
        
        // Check if integration already exists
        CameraBoundaryIntegration existingIntegration = boundarySystem.GetComponent<CameraBoundaryIntegration>();
        return existingIntegration == null;
    }
    
    /// <summary>
    /// Validates that prerequisite components exist.
    /// </summary>
    /// <returns>Boundary system GameObject</returns>
    private static GameObject ValidatePrerequisites()
    {
        Debug.Log("🔍 [Camera Integration] Validating prerequisites...");
        
        GameObject boundarySystem = GameObject.Find(BOUNDARY_SYSTEM_NAME);
        if (boundarySystem == null)
        {
            throw new System.Exception($"Boundary System GameObject not found. Please run 'Breakout/Setup/Create Boundary Walls' first.");
        }
        
        // Check for boundary walls
        BoundaryWall[] walls = boundarySystem.GetComponentsInChildren<BoundaryWall>();
        if (walls.Length == 0)
        {
            throw new System.Exception("No BoundaryWall components found. Please ensure boundary walls are properly created.");
        }
        
        // Check for boundary configuration
        BoundaryConfig config = Resources.Load<BoundaryConfig>("BoundaryConfig");
        if (config == null)
        {
            Debug.LogWarning("⚠️ [Camera Integration] BoundaryConfig not found - will use default calculations");
        }
        
        Debug.Log($"✅ [Camera Integration] Prerequisites validated - Found {walls.Length} boundary walls");
        return boundarySystem;
    }
    
    /// <summary>
    /// Sets up the main game camera with appropriate settings.
    /// </summary>
    /// <returns>Configured game camera</returns>
    private static Camera SetupGameCamera()
    {
        Debug.Log("📷 [Camera Integration] Setting up game camera...");
        
        // Try to find existing main camera
        Camera gameCamera = Camera.main;
        if (gameCamera == null)
        {
            gameCamera = Object.FindFirstObjectByType<Camera>();
        }
        
        // Create camera if none exists
        if (gameCamera == null)
        {
            Debug.Log("   • No camera found - creating new Main Camera");
            GameObject cameraObject = new GameObject(MAIN_CAMERA_NAME);
            gameCamera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            
            // Configure for 2D Breakout
            ConfigureCameraFor2D(gameCamera);
        }
        else
        {
            Debug.Log($"   • Using existing camera: {gameCamera.name}");
            
            // Ensure camera is configured for 2D if needed
            if (!gameCamera.orthographic)
            {
                Debug.LogWarning("   ⚠️ Camera is perspective - switching to orthographic for 2D gameplay");
                ConfigureCameraFor2D(gameCamera);
            }
        }
        
        Debug.Log("✅ [Camera Integration] Camera setup complete");
        return gameCamera;
    }
    
    /// <summary>
    /// Configures camera for 2D Breakout gameplay.
    /// </summary>
    /// <param name="camera">Camera to configure</param>
    private static void ConfigureCameraFor2D(Camera camera)
    {
        // Set to orthographic for 2D
        camera.orthographic = true;
        
        // Set appropriate orthographic size for 16:10 aspect ratio
        // Default size of 6 works well for 20x12 play area
        camera.orthographicSize = 6f;
        
        // Position camera at center
        camera.transform.position = new Vector3(0, 0, -10);
        camera.transform.rotation = Quaternion.identity;
        
        // Configure camera settings
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.2f); // Dark blue background
        
        // Set culling mask to everything
        camera.cullingMask = -1;
        
        // Configure near/far planes for 2D
        camera.nearClipPlane = -100f;
        camera.farClipPlane = 100f;
        
        Debug.Log("   • Camera configured for 2D gameplay (orthographic, size=6)");
    }
    
    /// <summary>
    /// Adds CameraBoundaryIntegration component to boundary system.
    /// </summary>
    /// <param name="boundarySystem">Boundary system GameObject</param>
    /// <returns>Added integration component</returns>
    private static CameraBoundaryIntegration AddIntegrationComponent(GameObject boundarySystem)
    {
        Debug.Log("🔧 [Camera Integration] Adding integration component...");
        
        CameraBoundaryIntegration integration = boundarySystem.GetComponent<CameraBoundaryIntegration>();
        if (integration == null)
        {
            integration = boundarySystem.AddComponent<CameraBoundaryIntegration>();
            Debug.Log("   • Added CameraBoundaryIntegration component");
        }
        else
        {
            Debug.Log("   • CameraBoundaryIntegration component already exists");
        }
        
        return integration;
    }
    
    /// <summary>
    /// Configures integration component settings.
    /// </summary>
    /// <param name="integration">Integration component to configure</param>
    /// <param name="camera">Game camera reference</param>
    private static void ConfigureIntegration(CameraBoundaryIntegration integration, Camera camera)
    {
        Debug.Log("⚙️ [Camera Integration] Configuring integration settings...");
        
        // Set camera reference
        integration.gameCamera = camera;
        
        // Load boundary configuration
        BoundaryConfig boundaryConfig = Resources.Load<BoundaryConfig>("BoundaryConfig");
        integration.boundaryConfig = boundaryConfig;
        
        // Configure alignment settings
        integration.alignmentTolerance = 0.1f;
        integration.autoUpdateBoundaries = true;
        
        // Configure debug visualization
        integration.showCameraBounds = true;
        integration.showBoundaryAlignment = true;
        integration.cameraBoundsColor = new Color(1f, 1f, 0f, 0.5f); // Yellow
        integration.alignmentColor = new Color(0f, 1f, 1f, 0.5f); // Cyan
        
        EditorUtility.SetDirty(integration);
        
        Debug.Log("✅ [Camera Integration] Integration configured successfully");
    }
    
    /// <summary>
    /// Performs initial calculations and alignment verification.
    /// </summary>
    /// <param name="integration">Integration component</param>
    private static void PerformInitialCalculations(CameraBoundaryIntegration integration)
    {
        Debug.Log("📐 [Camera Integration] Performing initial calculations...");
        
        // Calculate camera bounds
        integration.CalculateCameraBounds();
        
        Vector2 worldMin = integration.cameraWorldMin;
        Vector2 worldMax = integration.cameraWorldMax;
        Vector2 worldSize = integration.GetCameraWorldSize();
        float aspectRatio = integration.GetCameraAspectRatio();
        
        Debug.Log($"   • Camera world bounds: Min={worldMin}, Max={worldMax}");
        Debug.Log($"   • Camera world size: {worldSize}");
        Debug.Log($"   • Camera aspect ratio: {aspectRatio:F2}");
        
        // Update boundary positions to match camera
        Debug.Log("   • Updating boundary positions to match camera bounds...");
        integration.UpdateBoundaryPositions();
        
        // Verify alignment
        bool aligned = integration.VerifyBoundaryAlignment();
        if (aligned)
        {
            Debug.Log("   ✅ All boundaries properly aligned with camera bounds");
        }
        else
        {
            Debug.LogWarning("   ⚠️ Some boundaries may not be perfectly aligned - check Scene view");
        }
        
        // Log alignment status
        var alignmentStatus = integration.GetAlignmentStatus();
        foreach (var kvp in alignmentStatus)
        {
            string status = kvp.Value ? "✅ Aligned" : "❌ Misaligned";
            Debug.Log($"   • {kvp.Key} boundary: {status}");
        }
    }
    
    /// <summary>
    /// Saves all modified assets and refreshes the database.
    /// </summary>
    private static void SaveAndRefreshAssets()
    {
        Debug.Log("💾 [Camera Integration] Saving assets...");
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ [Camera Integration] Assets saved and refreshed");
    }
    
    /// <summary>
    /// Logs successful setup with comprehensive usage instructions.
    /// </summary>
    /// <param name="integration">Created integration component</param>
    private static void LogSuccessfulSetup(CameraBoundaryIntegration integration)
    {
        Debug.Log("✅ [Camera Bounds Integration] Setup completed successfully!");
        Debug.Log("📋 Camera Integration System Summary:");
        Debug.Log("   • Camera-boundary alignment system configured");
        Debug.Log("   • Automatic bounds calculation enabled");
        Debug.Log("   • Dynamic resolution update system active");
        Debug.Log("   • Visual debugging tools configured");
        
        Debug.Log("📷 Camera Configuration:");
        Camera camera = integration.gameCamera;
        Debug.Log($"   → Camera: {camera.name}");
        Debug.Log($"   → Type: {(camera.orthographic ? "Orthographic" : "Perspective")}");
        Debug.Log($"   → Orthographic Size: {camera.orthographicSize}");
        Debug.Log($"   → Position: {camera.transform.position}");
        Debug.Log($"   → Background: {camera.backgroundColor}");
        
        Debug.Log("📐 Bounds Calculation:");
        Debug.Log($"   • World Min: {integration.cameraWorldMin}");
        Debug.Log($"   • World Max: {integration.cameraWorldMax}");
        Debug.Log($"   • World Size: {integration.GetCameraWorldSize()}");
        Debug.Log($"   • Aspect Ratio: {integration.GetCameraAspectRatio():F2}");
        
        Debug.Log("🎯 Alignment Status:");
        var alignmentStatus = integration.GetAlignmentStatus();
        foreach (var kvp in alignmentStatus)
        {
            Debug.Log($"   • {kvp.Key}: {(kvp.Value ? "Aligned ✅" : "Misaligned ❌")}");
        }
        
        Debug.Log("🔧 Integration Features:");
        Debug.Log("   • Auto-update on camera changes: Enabled");
        Debug.Log("   • Alignment tolerance: 0.1 units");
        Debug.Log("   • Resolution change detection: Active");
        Debug.Log("   • Camera property monitoring: Active");
        
        Debug.Log("🎨 Debug Visualization:");
        Debug.Log("   • Camera bounds: Yellow rectangle in Scene view");
        Debug.Log("   • Alignment markers: Cyan spheres at expected positions");
        Debug.Log("   • Alignment lines: Green (aligned) or Red (misaligned)");
        Debug.Log("   • Toggle visualization in Inspector settings");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Camera bounds automatically calculated from camera settings");
        Debug.Log("   2. Boundaries repositioned to match visible game area");
        Debug.Log("   3. Changes to camera trigger automatic boundary updates");
        Debug.Log("   4. Use ForceUpdate() method for manual recalculation");
        Debug.Log("   5. Check Scene view for visual alignment verification");
        
        Debug.Log("📊 Testing Camera Changes:");
        Debug.Log("   • Modify camera orthographic size to test zoom changes");
        Debug.Log("   • Move camera position to test panning");
        Debug.Log("   • Change game view aspect ratio to test resolution handling");
        Debug.Log("   • All changes should trigger automatic boundary updates");
        
        Debug.Log("⚠️ Important Notes:");
        Debug.Log("   → Boundaries align with camera edges, not play area");
        Debug.Log("   → Bottom boundary may be outside view for ball loss");
        Debug.Log("   → Camera should remain orthographic for 2D gameplay");
        Debug.Log("   → Integration updates only active during gameplay");
        
        Debug.Log("🔄 Next Steps:");
        Debug.Log("   → Test boundary alignment in different resolutions");
        Debug.Log("   → Verify ball stays within camera bounds");
        Debug.Log("   → Adjust camera size for desired gameplay area");
        Debug.Log("   → Configure camera background and effects");
        Debug.Log("   → Test with different aspect ratios");
        
        // Log integration summary
        Debug.Log("\n" + integration.GetIntegrationSummary());
        
        // Select integration component in Inspector
        Selection.activeGameObject = integration.gameObject;
        EditorGUIUtility.PingObject(integration);
    }
}
#endif