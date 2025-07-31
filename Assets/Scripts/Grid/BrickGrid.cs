using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Core management system for brick grid generation and state tracking.
/// Provides centralized control over grid operations including generation, clearing, and completion detection.
/// Integrates with GridData configuration system for flexible level design.
/// </summary>
public class BrickGrid : MonoBehaviour
{
    [Header("Grid Configuration")]
    [Tooltip("GridData asset containing layout parameters, spacing, and brick distribution settings.")]
    [SerializeField] private GridData gridConfiguration;
    
    [Header("Runtime State")]
    [Tooltip("True when grid has been generated and is active in the scene.")]
    [SerializeField] private bool gridGenerated = false;
    
    [Tooltip("Current number of active bricks in the grid. Updates as bricks are destroyed.")]
    [SerializeField] private int brickCount = 0;
    
    [Tooltip("True when all destructible bricks have been destroyed (level complete).")]
    [SerializeField] private bool completionStatus = false;
    
    [Header("Hierarchy Management")]
    [Tooltip("Parent GameObject containing all brick instances. Created automatically if not assigned.")]
    [SerializeField] private GameObject gridContainer;
    
    [Header("Debug Settings")]
    [Tooltip("Enable detailed logging for grid operations and state changes.")]
    [SerializeField] private bool enableDebugLogging = true;
    
    [Tooltip("Show grid bounds and visualization gizmos in Scene view.")]
    [SerializeField] private bool showGridGizmos = true;
    
    // Internal state management
    private List<Brick> activeBricks = new();
    private bool isInitialized = false;
    private Vector3 gridStartPosition;
    private Vector2 gridDimensions;
    
    #region Properties
    
    /// <summary>
    /// Gets the current GridData configuration.
    /// </summary>
    public GridData GridConfiguration => gridConfiguration;
    
    /// <summary>
    /// Gets whether the grid has been generated.
    /// </summary>
    public bool IsGridGenerated => gridGenerated;
    
    /// <summary>
    /// Gets the current number of active bricks.
    /// </summary>
    public int ActiveBrickCount => brickCount;
    
    /// <summary>
    /// Gets whether the grid is complete (all destructible bricks destroyed).
    /// </summary>
    public bool IsComplete => completionStatus;
    
    /// <summary>
    /// Gets whether the BrickGrid manager is properly initialized.
    /// </summary>
    public bool IsInitialized => isInitialized;
    
    /// <summary>
    /// Gets the grid container GameObject.
    /// </summary>
    public GameObject GridContainer => gridContainer;
    
    /// <summary>
    /// Gets the calculated start position for grid generation.
    /// </summary>
    public Vector3 GridStartPosition => gridStartPosition;
    
    /// <summary>
    /// Gets the total grid dimensions in world units.
    /// </summary>
    public Vector2 GridDimensions => gridDimensions;
    
    #endregion
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize the BrickGrid manager and validate configuration.
    /// </summary>
    private void Awake()
    {
        LogDebug("🔧 [BrickGrid] Initializing BrickGrid manager...");
        
        try
        {
            // Initialize internal state
            InitializeGridManager();
            
            // Validate configuration
            if (!ValidateConfiguration())
            {
                LogError("❌ [BrickGrid] Configuration validation failed during initialization");
                return;
            }
            
            // Setup grid container
            SetupGridContainer();
            
            // Calculate grid parameters
            CalculateGridParameters();
            
            isInitialized = true;
            LogDebug("✅ [BrickGrid] BrickGrid manager initialized successfully");
        }
        catch (System.Exception e)
        {
            LogError($"❌ [BrickGrid] Failed to initialize BrickGrid manager: {e.Message}");
            isInitialized = false;
        }
    }
    
    /// <summary>
    /// Setup initial grid state and prepare for generation.
    /// </summary>
    private void Start()
    {
        if (!isInitialized)
        {
            LogError("❌ [BrickGrid] Cannot start - initialization failed");
            return;
        }
        
        LogDebug("🚀 [BrickGrid] Starting BrickGrid manager...");
        
        // Initial state setup
        ResetGridState();
        
        // Log configuration summary
        LogConfigurationSummary();
        
        LogDebug("✅ [BrickGrid] BrickGrid manager ready for grid generation");
    }
    
    #endregion
    
    #region Initialization Methods
    
    /// <summary>
    /// Initialize core grid manager components.
    /// </summary>
    private void InitializeGridManager()
    {
        // Initialize collections
        activeBricks = new List<Brick>();
        
        // Reset state flags
        gridGenerated = false;
        brickCount = 0;
        completionStatus = false;
        
        LogDebug("   • Core components initialized");
    }
    
    /// <summary>
    /// Setup or create the grid container GameObject.
    /// </summary>
    private void SetupGridContainer()
    {
        if (gridContainer == null)
        {
            gridContainer = new GameObject("BrickGrid_Container");
            gridContainer.transform.SetParent(transform);
            gridContainer.transform.localPosition = Vector3.zero;
            LogDebug("   • Created grid container GameObject");
        }
        else
        {
            LogDebug("   • Using existing grid container GameObject");
        }
        
        // Ensure container is properly positioned
        if (gridContainer.transform.parent != transform)
        {
            gridContainer.transform.SetParent(transform);
            LogDebug("   • Reparented grid container to BrickGrid");
        }
    }
    
    /// <summary>
    /// Calculate grid parameters based on configuration.
    /// </summary>
    private void CalculateGridParameters()
    {
        if (gridConfiguration == null)
        {
            LogWarning("⚠️ [BrickGrid] No GridData configuration - using default parameters");
            gridDimensions = new Vector2(10f, 6f);
            gridStartPosition = Vector3.zero;
            return;
        }
        
        // Calculate grid dimensions
        gridDimensions = gridConfiguration.CalculateGridSize();
        
        // Calculate start position
        gridStartPosition = gridConfiguration.CalculateCenteredOffset();
        
        LogDebug($"   • Grid dimensions: {gridDimensions.x:F1}x{gridDimensions.y:F1}");
        LogDebug($"   • Grid start position: {gridStartPosition}");
    }
    
    #endregion
    
    #region Configuration Validation
    
    /// <summary>
    /// Validates the grid configuration and dependencies.
    /// </summary>
    /// <returns>True if configuration is valid</returns>
    public bool ValidateConfiguration()
    {
        bool isValid = true;
        
        // Check GridData configuration
        if (gridConfiguration == null)
        {
            LogWarning("⚠️ [BrickGrid] No GridData configuration assigned");
            isValid = false;
        }
        else if (!gridConfiguration.ValidateConfiguration())
        {
            LogError("❌ [BrickGrid] GridData configuration validation failed");
            isValid = false;
        }
        
        // Validate grid container
        if (gridContainer != null && gridContainer.transform.parent != transform)
        {
            LogWarning("⚠️ [BrickGrid] Grid container is not properly parented");
        }
        
        return isValid;
    }
    
    /// <summary>
    /// Validates that the grid can be generated with current configuration.
    /// </summary>
    /// <returns>True if grid generation is possible</returns>
    public bool ValidateGridGeneration()
    {
        if (!isInitialized)
        {
            LogError("❌ [BrickGrid] Cannot validate generation - manager not initialized");
            return false;
        }
        
        if (gridConfiguration == null)
        {
            LogError("❌ [BrickGrid] Cannot generate grid - no configuration assigned");
            return false;
        }
        
        if (!gridConfiguration.FitsInPlayArea())
        {
            LogError("❌ [BrickGrid] Grid does not fit in configured play area");
            return false;
        }
        
        return true;
    }
    
    #endregion
    
    #region Framework Methods (To be implemented in future tasks)
    
    /// <summary>
    /// Generates the brick grid based on current configuration.
    /// This method will be implemented in future tasks with full grid generation logic.
    /// </summary>
    public void GenerateGrid()
    {
        LogDebug("🏗️ [BrickGrid] GenerateGrid() called - implementation pending");
        
        if (!ValidateGridGeneration())
        {
            LogError("❌ [BrickGrid] Grid generation validation failed");
            return;
        }
        
        // TODO: Implement grid generation logic in future tasks
        // - Load brick prefab
        // - Instantiate bricks based on GridData configuration
        // - Apply layout pattern (Standard, Pyramid, Diamond, Random, Custom)
        // - Configure individual brick components
        // - Update grid state and brick count
        
        LogDebug("📋 [BrickGrid] Grid generation framework ready - awaiting implementation");
    }
    
    /// <summary>
    /// Clears all bricks from the grid and resets state.
    /// This method will be implemented in future tasks with full cleanup logic.
    /// </summary>
    public void ClearGrid()
    {
        LogDebug("🧹 [BrickGrid] ClearGrid() called - implementation pending");
        
        // TODO: Implement grid clearing logic in future tasks
        // - Destroy all brick GameObjects
        // - Clear activeBricks collection
        // - Reset grid state flags
        // - Clean up grid container
        
        LogDebug("📋 [BrickGrid] Grid clearing framework ready - awaiting implementation");
    }
    
    /// <summary>
    /// Validates the current grid state and integrity.
    /// This method will be implemented in future tasks with comprehensive validation.
    /// </summary>
    /// <returns>True if grid state is valid</returns>
    public bool ValidateGrid()
    {
        LogDebug("🧪 [BrickGrid] ValidateGrid() called - implementation pending");
        
        // TODO: Implement grid validation logic in future tasks
        // - Check brick count consistency
        // - Validate brick positions and spacing
        // - Verify brick component states
        // - Check completion status accuracy
        
        LogDebug("📋 [BrickGrid] Grid validation framework ready - awaiting implementation");
        return true; // Placeholder return
    }
    
    #endregion
    
    #region State Management
    
    /// <summary>
    /// Resets the grid state to initial values.
    /// </summary>
    public void ResetGridState()
    {
        gridGenerated = false;
        brickCount = 0;
        completionStatus = false;
        activeBricks.Clear();
        
        LogDebug("🔄 [BrickGrid] Grid state reset to initial values");
    }
    
    /// <summary>
    /// Updates the brick count and checks for completion status.
    /// Called when bricks are destroyed or added.
    /// </summary>
    /// <param name="newBrickCount">Updated brick count</param>
    public void UpdateBrickCount(int newBrickCount)
    {
        int previousCount = brickCount;
        brickCount = newBrickCount;
        
        // Check for completion
        bool wasComplete = completionStatus;
        completionStatus = (brickCount <= 0) && gridGenerated;
        
        if (enableDebugLogging)
        {
            LogDebug($"🔢 [BrickGrid] Brick count updated: {previousCount} → {brickCount}");
            
            if (completionStatus && !wasComplete)
            {
                LogDebug("🎉 [BrickGrid] Grid completion detected!");
            }
        }
    }
    
    /// <summary>
    /// Marks the grid as generated and updates state.
    /// </summary>
    /// <param name="initialBrickCount">Number of bricks in the generated grid</param>
    public void SetGridGenerated(int initialBrickCount)
    {
        gridGenerated = true;
        brickCount = initialBrickCount;
        completionStatus = false;
        
        LogDebug($"✅ [BrickGrid] Grid marked as generated with {initialBrickCount} bricks");
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Gets debug information about the current grid state.
    /// </summary>
    /// <returns>Formatted debug information string</returns>
    public string GetGridDebugInfo()
    {
        return $"BrickGrid Debug Info:\n" +
               $"  Initialized: {isInitialized}\n" +
               $"  Configuration: {(gridConfiguration != null ? gridConfiguration.name : "None")}\n" +
               $"  Generated: {gridGenerated}\n" +
               $"  Brick Count: {brickCount}\n" +
               $"  Complete: {completionStatus}\n" +
               $"  Container: {(gridContainer != null ? gridContainer.name : "None")}\n" +
               $"  Grid Size: {gridDimensions.x:F1}x{gridDimensions.y:F1}\n" +
               $"  Start Position: {gridStartPosition}\n" +
               $"  Active Bricks: {activeBricks.Count}";
    }
    
    /// <summary>
    /// Logs configuration summary for debugging.
    /// </summary>
    private void LogConfigurationSummary()
    {
        if (!enableDebugLogging || gridConfiguration == null) return;
        
        LogDebug("📋 [BrickGrid] Configuration Summary:");
        LogDebug($"   • Pattern: {gridConfiguration.pattern}");
        LogDebug($"   • Dimensions: {gridConfiguration.rows}x{gridConfiguration.columns}");
        LogDebug($"   • Spacing: {gridConfiguration.horizontalSpacing}x{gridConfiguration.verticalSpacing}");
        LogDebug($"   • Grid Size: {gridDimensions.x:F1}x{gridDimensions.y:F1}");
        LogDebug($"   • Start Position: {gridStartPosition}");
        LogDebug($"   • Fits in Play Area: {gridConfiguration.FitsInPlayArea()}");
    }
    
    #endregion
    
    #region Debug Logging
    
    /// <summary>
    /// Logs debug message if debug logging is enabled.
    /// </summary>
    /// <param name="message">Debug message to log</param>
    private void LogDebug(string message)
    {
        if (enableDebugLogging)
        {
            Debug.Log(message);
        }
    }
    
    /// <summary>
    /// Logs warning message.
    /// </summary>
    /// <param name="message">Warning message to log</param>
    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }
    
    /// <summary>
    /// Logs error message.
    /// </summary>
    /// <param name="message">Error message to log</param>
    private void LogError(string message)
    {
        Debug.LogError(message);
    }
    
    #endregion
    
    #region Gizmos
    
    /// <summary>
    /// Draws grid visualization gizmos in Scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showGridGizmos || gridConfiguration == null) return;
        
        // Draw grid bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(gridStartPosition + new Vector3(gridDimensions.x * 0.5f, gridDimensions.y * 0.5f, 0f), 
                           new Vector3(gridDimensions.x, gridDimensions.y, 0.1f));
        
        // Draw play area bounds
        if (gridConfiguration.playAreaBounds.size != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(gridConfiguration.playAreaBounds.center, gridConfiguration.playAreaBounds.size);
        }
        
        // Draw grid start position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gridStartPosition, 0.2f);
    }
    
    /// <summary>
    /// Draws selected gizmos when GameObject is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!showGridGizmos || gridConfiguration == null) return;
        
        // Draw detailed grid layout
        Gizmos.color = Color.white;
        
        for (int row = 0; row < gridConfiguration.rows; row++)
        {
            for (int col = 0; col < gridConfiguration.columns; col++)
            {
                Vector3 brickPosition = gridStartPosition + new Vector3(
                    col * gridConfiguration.horizontalSpacing,
                    row * gridConfiguration.verticalSpacing,
                    0f
                );
                
                Gizmos.DrawWireCube(brickPosition, new Vector3(1f, 0.5f, 0.1f));
            }
        }
    }
    
    #endregion
}