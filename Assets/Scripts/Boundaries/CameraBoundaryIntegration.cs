using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Integrates boundary system with camera bounds to ensure consistent visual presentation.
/// Calculates camera visible area and aligns boundary walls with screen edges for proper gameplay.
/// Provides update system for dynamic camera changes and visual debugging tools.
/// </summary>
[System.Serializable]
public class CameraBoundaryIntegration : MonoBehaviour
{
    [Header("Camera Integration")]
    [Tooltip("Main game camera for bounds calculation")]
    public Camera gameCamera;
    
    [Tooltip("Boundary configuration for positioning parameters")]
    public BoundaryConfig boundaryConfig;
    
    [Header("Bounds Calculation")]
    [Tooltip("Bottom-left corner of camera view in world space")]
    public Vector2 cameraWorldMin;
    
    [Tooltip("Top-right corner of camera view in world space")]
    public Vector2 cameraWorldMax;
    
    [Tooltip("Camera view size in world units")]
    [SerializeField] private Vector2 cameraWorldSize;
    
    [Tooltip("Camera aspect ratio (width/height)")]
    [SerializeField] private float cameraAspectRatio;
    
    [Header("Alignment Settings")]
    [Tooltip("Tolerance for boundary alignment verification")]
    [Range(0.01f, 1f)]
    public float alignmentTolerance = 0.1f;
    
    [Tooltip("Automatically update boundaries when camera changes")]
    public bool autoUpdateBoundaries = true;
    
    [Header("Debug Visualization")]
    [Tooltip("Show camera bounds in Scene view")]
    public bool showCameraBounds = true;
    
    [Tooltip("Show boundary alignment visualization")]
    public bool showBoundaryAlignment = true;
    
    [Tooltip("Color for camera bounds visualization")]
    public Color cameraBoundsColor = new Color(1f, 1f, 0f, 0.5f); // Yellow
    
    [Tooltip("Color for alignment visualization")]
    public Color alignmentColor = new Color(0f, 1f, 1f, 0.5f); // Cyan
    
    [Header("Runtime Info")]
    [Tooltip("Is camera bounds calculation valid")]
    [SerializeField] private bool boundsCalculated = false;
    
    [Tooltip("Are boundaries aligned with camera")]
    [SerializeField] private bool boundariesAligned = false;
    
    [Tooltip("Last calculated screen resolution")]
    [SerializeField] private Vector2Int lastScreenResolution;
    
    // Cached references
    private List<BoundaryWall> boundaryWalls = new List<BoundaryWall>();
    private Transform boundarySystemTransform;
    
    // Previous camera state for change detection
    private float lastOrthographicSize;
    private Vector3 lastCameraPosition;
    private float lastAspectRatio;
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize component and find references.
    /// </summary>
    private void Awake()
    {
        InitializeReferences();
        CalculateCameraBounds();
    }
    
    /// <summary>
    /// Set up initial alignment and validation.
    /// </summary>
    private void Start()
    {
        if (ValidateSetup())
        {
            VerifyBoundaryAlignment();
            if (autoUpdateBoundaries)
            {
                UpdateBoundaryPositions();
            }
        }
    }
    
    /// <summary>
    /// Check for camera changes and update if needed.
    /// </summary>
    private void Update()
    {
        if (autoUpdateBoundaries && HasCameraChanged())
        {
            CalculateCameraBounds();
            UpdateBoundaryPositions();
        }
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initialize camera and boundary references.
    /// </summary>
    private void InitializeReferences()
    {
        // Find camera if not assigned
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
            if (gameCamera == null)
            {
                gameCamera = FindFirstObjectByType<Camera>();
            }
        }
        
        if (gameCamera == null)
        {
            Debug.LogError("[CameraBoundaryIntegration] No camera found - camera bounds cannot be calculated");
            return;
        }
        
        // Find boundary configuration
        if (boundaryConfig == null)
        {
            boundaryConfig = Resources.Load<BoundaryConfig>("BoundaryConfig");
        }
        
        // Cache boundary walls
        boundaryWalls.Clear();
        boundaryWalls.AddRange(FindObjectsOfType<BoundaryWall>());
        
        // Find boundary system transform
        GameObject boundarySystem = GameObject.Find("Boundary System");
        if (boundarySystem != null)
        {
            boundarySystemTransform = boundarySystem.transform;
        }
        
        // Store initial camera state
        StoreCameraState();
        
        Debug.Log($"[CameraBoundaryIntegration] Initialized with {boundaryWalls.Count} boundary walls");
    }
    
    /// <summary>
    /// Validates that all required components are present.
    /// </summary>
    /// <returns>True if setup is valid</returns>
    private bool ValidateSetup()
    {
        bool isValid = true;
        
        if (gameCamera == null)
        {
            Debug.LogError("[CameraBoundaryIntegration] Camera reference missing");
            isValid = false;
        }
        
        if (boundaryWalls.Count == 0)
        {
            Debug.LogWarning("[CameraBoundaryIntegration] No boundary walls found");
            isValid = false;
        }
        
        if (boundaryConfig == null)
        {
            Debug.LogWarning("[CameraBoundaryIntegration] BoundaryConfig not found - using default calculations");
        }
        
        return isValid;
    }
    
    #endregion
    
    #region Camera Bounds Calculation
    
    /// <summary>
    /// Calculates the camera's visible bounds in world space.
    /// </summary>
    public void CalculateCameraBounds()
    {
        if (gameCamera == null)
        {
            Debug.LogError("[CameraBoundaryIntegration] Cannot calculate bounds - camera is null");
            return;
        }
        
        // Store screen resolution
        lastScreenResolution = new Vector2Int(Screen.width, Screen.height);
        
        if (gameCamera.orthographic)
        {
            CalculateOrthographicBounds();
        }
        else
        {
            CalculatePerspectiveBounds();
        }
        
        // Calculate derived values
        cameraWorldSize = cameraWorldMax - cameraWorldMin;
        cameraAspectRatio = cameraWorldSize.x / cameraWorldSize.y;
        
        boundsCalculated = true;
        StoreCameraState();
        
        Debug.Log($"[CameraBoundaryIntegration] Camera bounds calculated: Min={cameraWorldMin}, Max={cameraWorldMax}, Size={cameraWorldSize}");
    }
    
    /// <summary>
    /// Calculates bounds for orthographic camera (typical for 2D games).
    /// </summary>
    private void CalculateOrthographicBounds()
    {
        float verticalSize = gameCamera.orthographicSize;
        float horizontalSize = verticalSize * gameCamera.aspect;
        
        Vector3 cameraPos = gameCamera.transform.position;
        
        cameraWorldMin = new Vector2(
            cameraPos.x - horizontalSize,
            cameraPos.y - verticalSize
        );
        
        cameraWorldMax = new Vector2(
            cameraPos.x + horizontalSize,
            cameraPos.y + verticalSize
        );
    }
    
    /// <summary>
    /// Calculates bounds for perspective camera at z=0 plane.
    /// </summary>
    private void CalculatePerspectiveBounds()
    {
        // Calculate bounds at z=0 (typical 2D gameplay plane)
        float distanceToPlane = Mathf.Abs(gameCamera.transform.position.z);
        
        // Get viewport corners in world space
        Vector3 bottomLeft = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceToPlane));
        Vector3 topRight = gameCamera.ViewportToWorldPoint(new Vector3(1, 1, distanceToPlane));
        
        cameraWorldMin = new Vector2(bottomLeft.x, bottomLeft.y);
        cameraWorldMax = new Vector2(topRight.x, topRight.y);
    }
    
    /// <summary>
    /// Gets the camera bounds as a Unity Bounds struct.
    /// </summary>
    /// <returns>Camera bounds in world space</returns>
    public Bounds GetCameraBounds()
    {
        Vector3 center = (cameraWorldMin + cameraWorldMax) * 0.5f;
        Vector3 size = cameraWorldMax - cameraWorldMin;
        return new Bounds(center, size);
    }
    
    #endregion
    
    #region Boundary Alignment
    
    /// <summary>
    /// Updates boundary wall positions to align with camera bounds.
    /// </summary>
    public void UpdateBoundaryPositions()
    {
        if (!boundsCalculated)
        {
            CalculateCameraBounds();
        }
        
        Debug.Log("[CameraBoundaryIntegration] Updating boundary positions to match camera bounds");
        
        foreach (BoundaryWall wall in boundaryWalls)
        {
            UpdateWallPosition(wall);
        }
        
        // Verify alignment after update
        VerifyBoundaryAlignment();
    }
    
    /// <summary>
    /// Updates a specific wall's position based on camera bounds.
    /// </summary>
    /// <param name="wall">BoundaryWall to update</param>
    private void UpdateWallPosition(BoundaryWall wall)
    {
        if (wall == null) return;
        
        Vector3 newPosition = CalculateWallPosition(wall.wallType);
        
        // Apply position
        wall.transform.position = newPosition;
        
        // Update wall's internal position tracking
        wall.UpdateWallPosition();
        
        Debug.Log($"[CameraBoundaryIntegration] Updated {wall.wallType} wall position to {newPosition}");
    }
    
    /// <summary>
    /// Calculates the ideal position for a wall based on camera bounds.
    /// </summary>
    /// <param name="wallType">Type of boundary wall</param>
    /// <returns>Calculated world position</returns>
    private Vector3 CalculateWallPosition(BoundaryType wallType)
    {
        return CalculateIdealWallPosition(wallType);
    }
    
    /// <summary>
    /// Calculates the ideal position for a wall based on camera bounds (for positioning).
    /// </summary>
    /// <param name="wallType">Type of boundary wall</param>
    /// <returns>Ideal world position</returns>
    private Vector3 CalculateIdealWallPosition(BoundaryType wallType)
    {
        Vector3 position = Vector3.zero;
        
        // Get wall configuration
        BoundaryWallConfig wallConfig = boundaryConfig != null ? 
            boundaryConfig.GetBoundaryConfig(wallType) : 
            BoundaryWallConfig.CreateDefault(wallType);
        
        // Calculate base position from camera bounds (walls should be AT the camera edges)
        switch (wallType)
        {
            case BoundaryType.Top:
                position.x = (cameraWorldMin.x + cameraWorldMax.x) * 0.5f;
                position.y = cameraWorldMax.y; // At camera edge, not offset
                break;
                
            case BoundaryType.Bottom:
                position.x = (cameraWorldMin.x + cameraWorldMax.x) * 0.5f;
                position.y = cameraWorldMin.y; // At camera edge, not offset
                break;
                
            case BoundaryType.Left:
                position.x = cameraWorldMin.x; // At camera edge, not offset
                position.y = (cameraWorldMin.y + cameraWorldMax.y) * 0.5f;
                break;
                
            case BoundaryType.Right:
                position.x = cameraWorldMax.x; // At camera edge, not offset
                position.y = (cameraWorldMin.y + cameraWorldMax.y) * 0.5f;
                break;
        }
        
        // Apply position offset from configuration
        position += wallConfig.positionOffset;
        
        // Ensure z position is 0 for 2D gameplay
        position.z = 0f;
        
        return position;
    }
    
    #endregion
    
    #region Alignment Verification
    
    /// <summary>
    /// Verifies that boundary walls are properly aligned with camera bounds.
    /// </summary>
    public bool VerifyBoundaryAlignment()
    {
        if (!boundsCalculated || boundaryWalls.Count == 0)
        {
            boundariesAligned = false;
            return false;
        }
        
        bool allAligned = true;
        
        foreach (BoundaryWall wall in boundaryWalls)
        {
            if (!VerifyWallAlignment(wall))
            {
                allAligned = false;
            }
        }
        
        boundariesAligned = allAligned;
        
        if (boundariesAligned)
        {
            Debug.Log("[CameraBoundaryIntegration] ✅ All boundaries properly aligned with camera bounds");
        }
        else
        {
            Debug.LogWarning("[CameraBoundaryIntegration] ⚠️ Some boundaries are not properly aligned");
        }
        
        return boundariesAligned;
    }
    
    /// <summary>
    /// Verifies alignment of a specific wall.
    /// </summary>
    /// <param name="wall">BoundaryWall to verify</param>
    /// <returns>True if wall is properly aligned</returns>
    private bool VerifyWallAlignment(BoundaryWall wall)
    {
        if (wall == null) return false;
        
        bool isAligned = IsWallProperlyAligned(wall);
        
        if (!isAligned)
        {
            Vector3 cameraEdge = GetCameraEdgePosition(wall.wallType);
            Vector3 actualPos = wall.transform.position;
            Debug.LogWarning($"[CameraBoundaryIntegration] {wall.wallType} wall not properly aligned with camera edge");
            Debug.LogWarning($"   Camera edge: {cameraEdge}, Wall position: {actualPos}");
        }
        
        return isAligned;
    }
    
    /// <summary>
    /// Gets alignment status for all walls.
    /// </summary>
    /// <returns>Dictionary of wall types and their alignment status</returns>
    public Dictionary<BoundaryType, bool> GetAlignmentStatus()
    {
        Dictionary<BoundaryType, bool> status = new Dictionary<BoundaryType, bool>();
        
        foreach (BoundaryWall wall in boundaryWalls)
        {
            status[wall.wallType] = VerifyWallAlignment(wall);
        }
        
        return status;
    }
    
    #endregion
    
    #region Change Detection
    
    /// <summary>
    /// Checks if camera settings have changed since last update.
    /// </summary>
    /// <returns>True if camera has changed</returns>
    private bool HasCameraChanged()
    {
        if (gameCamera == null) return false;
        
        // Check resolution change
        if (Screen.width != lastScreenResolution.x || Screen.height != lastScreenResolution.y)
        {
            return true;
        }
        
        // Check camera properties
        bool changed = false;
        
        if (gameCamera.orthographic)
        {
            changed |= Mathf.Abs(gameCamera.orthographicSize - lastOrthographicSize) > 0.01f;
        }
        
        changed |= Vector3.Distance(gameCamera.transform.position, lastCameraPosition) > 0.01f;
        changed |= Mathf.Abs(gameCamera.aspect - lastAspectRatio) > 0.01f;
        
        return changed;
    }
    
    /// <summary>
    /// Stores current camera state for change detection.
    /// </summary>
    private void StoreCameraState()
    {
        if (gameCamera == null) return;
        
        lastOrthographicSize = gameCamera.orthographicSize;
        lastCameraPosition = gameCamera.transform.position;
        lastAspectRatio = gameCamera.aspect;
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Forces a recalculation and alignment update.
    /// </summary>
    public void ForceUpdate()
    {
        CalculateCameraBounds();
        UpdateBoundaryPositions();
    }
    
    /// <summary>
    /// Gets the camera's visible world size.
    /// </summary>
    /// <returns>World size in units</returns>
    public Vector2 GetCameraWorldSize()
    {
        return cameraWorldSize;
    }
    
    /// <summary>
    /// Gets the camera's aspect ratio.
    /// </summary>
    /// <returns>Width/Height ratio</returns>
    public float GetCameraAspectRatio()
    {
        return cameraAspectRatio;
    }
    
    /// <summary>
    /// Checks if a world position is within camera bounds.
    /// </summary>
    /// <param name="worldPosition">Position to check</param>
    /// <returns>True if position is visible</returns>
    public bool IsPositionInCameraBounds(Vector3 worldPosition)
    {
        return worldPosition.x >= cameraWorldMin.x && worldPosition.x <= cameraWorldMax.x &&
               worldPosition.y >= cameraWorldMin.y && worldPosition.y <= cameraWorldMax.y;
    }
    
    /// <summary>
    /// Gets integration status summary.
    /// </summary>
    /// <returns>Status summary string</returns>
    public string GetIntegrationSummary()
    {
        return $"CameraBoundaryIntegration Status:\n" +
               $"• Camera: {(gameCamera != null ? gameCamera.name : "None")}\n" +
               $"• Camera Type: {(gameCamera != null && gameCamera.orthographic ? "Orthographic" : "Perspective")}\n" +
               $"• Bounds Calculated: {boundsCalculated}\n" +
               $"• World Bounds: {cameraWorldMin} to {cameraWorldMax}\n" +
               $"• World Size: {cameraWorldSize}\n" +
               $"• Aspect Ratio: {cameraAspectRatio:F2}\n" +
               $"• Boundaries Aligned: {boundariesAligned}\n" +
               $"• Auto Update: {autoUpdateBoundaries}\n" +
               $"• Boundary Walls: {boundaryWalls.Count}";
    }
    
    #endregion
    
    #region Gizmos
    
    /// <summary>
    /// Draws debug visualization in Scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showCameraBounds && !showBoundaryAlignment) return;
        
        if (showCameraBounds && boundsCalculated)
        {
            DrawCameraBounds();
        }
        
        if (showBoundaryAlignment && boundaryWalls.Count > 0)
        {
            DrawBoundaryAlignment();
        }
    }
    
    /// <summary>
    /// Draws camera bounds visualization.
    /// </summary>
    private void DrawCameraBounds()
    {
        Gizmos.color = cameraBoundsColor;
        
        // Draw camera bounds rectangle
        Vector3 bottomLeft = new Vector3(cameraWorldMin.x, cameraWorldMin.y, 0);
        Vector3 bottomRight = new Vector3(cameraWorldMax.x, cameraWorldMin.y, 0);
        Vector3 topLeft = new Vector3(cameraWorldMin.x, cameraWorldMax.y, 0);
        Vector3 topRight = new Vector3(cameraWorldMax.x, cameraWorldMax.y, 0);
        
        // Draw filled rectangle
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
        
        // Draw semi-transparent fill
        Color fillColor = cameraBoundsColor;
        fillColor.a *= 0.2f;
        Gizmos.color = fillColor;
        
        Vector3 center = (bottomLeft + topRight) * 0.5f;
        Vector3 size = new Vector3(cameraWorldSize.x, cameraWorldSize.y, 0.1f);
        Gizmos.DrawCube(center, size);
        
        // Draw center cross
        Gizmos.color = cameraBoundsColor;
        float crossSize = Mathf.Min(cameraWorldSize.x, cameraWorldSize.y) * 0.05f;
        Gizmos.DrawLine(center - Vector3.right * crossSize, center + Vector3.right * crossSize);
        Gizmos.DrawLine(center - Vector3.up * crossSize, center + Vector3.up * crossSize);
    }
    
    /// <summary>
    /// Draws boundary alignment visualization.
    /// </summary>
    private void DrawBoundaryAlignment()
    {
        foreach (BoundaryWall wall in boundaryWalls)
        {
            if (wall == null) continue;
            
            Vector3 actualPos = wall.transform.position;
            
            // Check if the wall's inner edge aligns with camera bounds (correct for Breakout)
            bool isAligned = IsWallProperlyAligned(wall);
            
            // Draw actual position marker
            Gizmos.color = isAligned ? Color.green : Color.red;
            Gizmos.DrawWireSphere(actualPos, 0.2f);
            
            // Draw the camera edge that this wall should align with
            Vector3 cameraEdgePos = GetCameraEdgePosition(wall.wallType);
            Gizmos.color = alignmentColor;
            Gizmos.DrawWireSphere(cameraEdgePos, 0.1f);
            
            // Draw line from wall to camera edge
            Gizmos.color = isAligned ? Color.green : Color.red;
            Gizmos.DrawLine(actualPos, cameraEdgePos);
        }
    }
    
    /// <summary>
    /// Gets the camera edge position for a specific wall type.
    /// </summary>
    /// <param name="wallType">Type of boundary wall</param>
    /// <returns>Camera edge position</returns>
    private Vector3 GetCameraEdgePosition(BoundaryType wallType)
    {
        Vector3 position = Vector3.zero;
        
        switch (wallType)
        {
            case BoundaryType.Top:
                position.x = (cameraWorldMin.x + cameraWorldMax.x) * 0.5f;
                position.y = cameraWorldMax.y;
                break;
                
            case BoundaryType.Bottom:
                position.x = (cameraWorldMin.x + cameraWorldMax.x) * 0.5f;
                position.y = cameraWorldMin.y;
                break;
                
            case BoundaryType.Left:
                position.x = cameraWorldMin.x;
                position.y = (cameraWorldMin.y + cameraWorldMax.y) * 0.5f;
                break;
                
            case BoundaryType.Right:
                position.x = cameraWorldMax.x;
                position.y = (cameraWorldMin.y + cameraWorldMax.y) * 0.5f;
                break;
        }
        
        return position;
    }
    
    /// <summary>
    /// Checks if a wall is properly aligned (inner edge at camera boundary).
    /// </summary>
    /// <param name="wall">Wall to check</param>
    /// <returns>True if properly aligned</returns>
    private bool IsWallProperlyAligned(BoundaryWall wall)
    {
        if (wall == null) 
        {
            Debug.LogWarning("[CameraBoundaryIntegration] Wall is null");
            return false;
        }
        
        Vector3 wallPos = wall.transform.position;
        BoxCollider2D collider = wall.GetComponent<BoxCollider2D>();
        if (collider == null) 
        {
            Debug.LogWarning($"[CameraBoundaryIntegration] {wall.wallType} wall has no BoxCollider2D");
            return false;
        }
        
        // Get wall's inner edge position
        Vector3 innerEdgePos = wallPos;
        Vector2 colliderSize = collider.size;
        
        float difference = 0f;
        bool isAligned = false;
        
        switch (wall.wallType)
        {
            case BoundaryType.Top:
                innerEdgePos.y -= colliderSize.y * 0.5f; // Bottom edge of top wall
                difference = Mathf.Abs(innerEdgePos.y - cameraWorldMax.y);
                isAligned = difference <= alignmentTolerance;
                Debug.Log($"[CameraBoundaryIntegration] {wall.wallType}: Inner edge Y={innerEdgePos.y:F2}, Camera max Y={cameraWorldMax.y:F2}, Diff={difference:F2}, Aligned={isAligned}");
                break;
                
            case BoundaryType.Bottom:
                innerEdgePos.y += colliderSize.y * 0.5f; // Top edge of bottom wall
                difference = Mathf.Abs(innerEdgePos.y - cameraWorldMin.y);
                isAligned = difference <= alignmentTolerance;
                Debug.Log($"[CameraBoundaryIntegration] {wall.wallType}: Inner edge Y={innerEdgePos.y:F2}, Camera min Y={cameraWorldMin.y:F2}, Diff={difference:F2}, Aligned={isAligned}");
                break;
                
            case BoundaryType.Left:
                innerEdgePos.x += colliderSize.x * 0.5f; // Right edge of left wall
                difference = Mathf.Abs(innerEdgePos.x - cameraWorldMin.x);
                isAligned = difference <= alignmentTolerance;
                Debug.Log($"[CameraBoundaryIntegration] {wall.wallType}: Inner edge X={innerEdgePos.x:F2}, Camera min X={cameraWorldMin.x:F2}, Diff={difference:F2}, Aligned={isAligned}");
                break;
                
            case BoundaryType.Right:
                innerEdgePos.x -= colliderSize.x * 0.5f; // Left edge of right wall
                difference = Mathf.Abs(innerEdgePos.x - cameraWorldMax.x);
                isAligned = difference <= alignmentTolerance;
                Debug.Log($"[CameraBoundaryIntegration] {wall.wallType}: Inner edge X={innerEdgePos.x:F2}, Camera max X={cameraWorldMax.x:F2}, Diff={difference:F2}, Aligned={isAligned}");
                break;
                
            default:
                Debug.LogWarning($"[CameraBoundaryIntegration] Unknown wall type: {wall.wallType}");
                return false;
        }
        
        return isAligned;
    }
    
    #endregion
    
    #region Editor Support
    
    /// <summary>
    /// Called when values change in Inspector (Editor only).
    /// </summary>
    private void OnValidate()
    {
        if (Application.isPlaying && autoUpdateBoundaries)
        {
            // Force update when settings change in Inspector
            Invoke(nameof(ForceUpdate), 0.1f);
        }
    }
    
    #endregion
}