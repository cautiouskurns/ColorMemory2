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
    
    [Tooltip("Array of row container GameObjects for organizing bricks by row. Created automatically during generation.")]
    [SerializeField] private GameObject[] rowContainers = new GameObject[0];
    
    [Header("Hierarchy Organization")]
    [Tooltip("Name for the main grid container GameObject.")]
    [SerializeField] private string gridContainerName = "BrickGrid";
    
    [Tooltip("Prefix for row container names. Will be followed by row number (e.g., 'Row_01', 'Row_02').")]
    [SerializeField] private string rowContainerPrefix = "Row_";
    
    [Tooltip("Enable row-based organization. When disabled, all bricks are placed directly under grid container.")]
    [SerializeField] private bool useRowOrganization = true;
    
    [Header("Brick Prefabs")]
    [Tooltip("Prefab used for creating brick instances. Must have Brick component attached.")]
    [SerializeField] private GameObject brickPrefab;
    
    [Tooltip("Array of BrickData configurations for different brick types. Indexed by BrickType enum.")]
    [SerializeField] private BrickData[] brickDataConfigurations = new BrickData[0];
    
    [Header("Instantiation Control")]
    [Tooltip("Parent Transform for organizing instantiated bricks. Uses gridContainer if not assigned.")]
    [SerializeField] private Transform brickParent;
    
    [Tooltip("Enable batch instantiation optimizations for large grids.")]
    [SerializeField] private bool useBatchInstantiation = true;
    
    [Tooltip("Maximum bricks to instantiate per frame to avoid performance spikes.")]
    [Range(1, 100)]
    [SerializeField] private int maxBricksPerFrame = 50;
    
    [Header("Debug Settings")]
    [Tooltip("Enable detailed logging for grid operations and state changes.")]
    [SerializeField] private bool enableDebugLogging = true;
    
    [Tooltip("Show grid bounds and visualization gizmos in Scene view.")]
    [SerializeField] private bool showGridGizmos = true;
    
    [Header("Debug and Validation")]
    [Tooltip("Enable debug visualization for grid bounds, positioning, and validation status.")]
    [SerializeField] private bool enableDebugVisualization = true;
    
    [Tooltip("Run comprehensive validation checks during grid generation.")]
    [SerializeField] private bool runValidationOnGeneration = true;
    
    [Tooltip("Color for drawing grid bounds and positioning gizmos in Scene view.")]
    [SerializeField] private Color debugBoundsColor = Color.yellow;
    
    [Tooltip("Color for highlighting validation errors in gizmo visualization.")]
    [SerializeField] private Color debugErrorColor = Color.red;
    
    [Tooltip("Color for indicating successful validation in gizmo visualization.")]
    [SerializeField] private Color debugSuccessColor = Color.green;
    
    [Header("Auto Generation")]
    [Tooltip("Automatically generate the grid when the game starts (in Play mode).")]
    [SerializeField] private bool autoGenerateOnStart = true;
    
    [Header("Pattern Configuration")]
    [Tooltip("Current layout pattern for brick generation. Overrides GridData pattern if set.")]
    [SerializeField] private LayoutPattern currentPattern = LayoutPattern.Standard;
    
    [Tooltip("Use GridData pattern setting instead of override pattern.")]
    [SerializeField] private bool useGridDataPattern = true;
    
    [Tooltip("Density factor for random patterns. Controls percentage of positions filled with bricks.")]
    [Range(0.1f, 1.0f)]
    [SerializeField] private float patternDensity = 0.8f;
    
    [Tooltip("Create hollow center for diamond patterns. Reduces brick density in pattern center.")]
    [SerializeField] private bool hollowCenter = false;
    
    [Tooltip("Pattern-specific randomization seed for reproducible layouts.")]
    [SerializeField] private int patternSeed = 42;
    
    // Internal state management
    private List<Brick> activeBricks = new();
    private List<GameObject> instantiatedBricks = new();
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
    
    /// <summary>
    /// Gets the brick prefab used for instantiation.
    /// </summary>
    public GameObject BrickPrefab => brickPrefab;
    
    /// <summary>
    /// Gets the list of instantiated brick GameObjects.
    /// </summary>
    public List<GameObject> InstantiatedBricks => instantiatedBricks;
    
    /// <summary>
    /// Gets the parent Transform for brick organization.
    /// </summary>
    public Transform BrickParent => brickParent != null ? brickParent : (gridContainer != null ? gridContainer.transform : transform);
    
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
        
        // Auto-generate grid if enabled and in Play mode
        if (autoGenerateOnStart && Application.isPlaying)
        {
            LogDebug("🏗️ [BrickGrid] Auto-generating grid on start...");
            GenerateGrid();
        }
        else
        {
            LogDebug("✅ [BrickGrid] BrickGrid manager ready for grid generation");
        }
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
        instantiatedBricks = new List<GameObject>();
        
        // Reset state flags
        gridGenerated = false;
        brickCount = 0;
        completionStatus = false;
        
        // Validate instantiation system
        ValidatePrefabReferences();
        
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
    
    #region Hierarchy Organization Management
    
    /// <summary>
    /// Creates the main grid container GameObject with proper naming and hierarchy setup.
    /// </summary>
    /// <returns>Created or existing grid container GameObject</returns>
    public GameObject CreateGridContainer()
    {
        if (gridContainer != null)
        {
            LogDebug($"   • Using existing grid container: {gridContainer.name}");
            return gridContainer;
        }
        
        // Create new grid container
        gridContainer = new GameObject(gridContainerName);
        gridContainer.transform.SetParent(transform);
        gridContainer.transform.localPosition = Vector3.zero;
        gridContainer.transform.localRotation = Quaternion.identity;
        gridContainer.transform.localScale = Vector3.one;
        
        LogDebug($"   • Created grid container: {gridContainer.name}");
        return gridContainer;
    }
    
    /// <summary>
    /// Creates a row container GameObject for organizing bricks in a specific row.
    /// </summary>
    /// <param name="rowIndex">Zero-based row index</param>
    /// <returns>Created row container GameObject</returns>
    public GameObject CreateRowContainer(int rowIndex)
    {
        if (!useRowOrganization)
        {
            LogDebug("   • Row organization disabled - returning grid container");
            return CreateGridContainer();
        }
        
        // Ensure grid container exists
        GameObject parentContainer = CreateGridContainer();
        
        // Create row container with proper naming
        string rowName = $"{rowContainerPrefix}{rowIndex:D2}";
        GameObject rowContainer = new GameObject(rowName);
        rowContainer.transform.SetParent(parentContainer.transform);
        rowContainer.transform.localPosition = Vector3.zero;
        rowContainer.transform.localRotation = Quaternion.identity;
        rowContainer.transform.localScale = Vector3.one;
        
        LogDebug($"   • Created row container: {rowName} under {parentContainer.name}");
        return rowContainer;
    }
    
    /// <summary>
    /// Organizes all instantiated bricks into proper hierarchy containers.
    /// Creates row containers as needed and moves existing bricks to appropriate parents.
    /// </summary>
    public void OrganizeBricksInHierarchy()
    {
        if (!useRowOrganization)
        {
            OrganizeBricksUnderGridContainer();
            return;
        }
        
        LogDebug("🗂️ [BrickGrid] Organizing bricks in row-based hierarchy...");
        
        // Initialize row containers array if needed
        if (gridConfiguration != null && (rowContainers == null || rowContainers.Length != gridConfiguration.rows))
        {
            rowContainers = new GameObject[gridConfiguration.rows];
        }
        
        int organizedCount = 0;
        
        // Process each instantiated brick
        foreach (GameObject brick in instantiatedBricks)
        {
            if (brick == null) continue;
            
            // Extract row information from brick name or position
            int rowIndex = GetBrickRowFromGameObject(brick);
            if (rowIndex >= 0 && rowIndex < rowContainers.Length)
            {
                // Ensure row container exists
                if (rowContainers[rowIndex] == null)
                {
                    rowContainers[rowIndex] = CreateRowContainer(rowIndex);
                }
                
                // Move brick to appropriate row container
                if (brick.transform.parent != rowContainers[rowIndex].transform)
                {
                    brick.transform.SetParent(rowContainers[rowIndex].transform);
                    organizedCount++;
                }
            }
        }
        
        LogDebug($"   • Organized {organizedCount} bricks into row containers");
        ValidateHierarchyIntegrity();
    }
    
    /// <summary>
    /// Organizes all bricks directly under the grid container (no row organization).
    /// </summary>
    private void OrganizeBricksUnderGridContainer()
    {
        LogDebug("🗂️ [BrickGrid] Organizing bricks under grid container...");
        
        GameObject container = CreateGridContainer();
        int organizedCount = 0;
        
        foreach (GameObject brick in instantiatedBricks)
        {
            if (brick != null && brick.transform.parent != container.transform)
            {
                brick.transform.SetParent(container.transform);
                organizedCount++;
            }
        }
        
        LogDebug($"   • Organized {organizedCount} bricks under grid container");
    }
    
    /// <summary>
    /// Clears the entire hierarchy including all containers and bricks.
    /// Provides efficient cleanup for grid regeneration.
    /// </summary>
    public void ClearHierarchy()
    {
        LogDebug("🧹 [BrickGrid] Clearing hierarchy...");
        
        int clearedContainers = 0;
        int clearedBricks = 0;
        
        // Clear row containers
        if (rowContainers != null)
        {
            for (int i = 0; i < rowContainers.Length; i++)
            {
                if (rowContainers[i] != null)
                {
                    // Count bricks in this container
                    clearedBricks += rowContainers[i].transform.childCount;
                    
                    // Destroy container and all children
                    if (Application.isPlaying)
                        Destroy(rowContainers[i]);
                    else
                        DestroyImmediate(rowContainers[i]);
                    
                    clearedContainers++;
                }
                rowContainers[i] = null;
            }
        }
        
        // Clear any remaining bricks directly under grid container
        if (gridContainer != null)
        {
            // Count and destroy any direct children
            int directChildren = gridContainer.transform.childCount;
            for (int i = directChildren - 1; i >= 0; i--)
            {
                GameObject child = gridContainer.transform.GetChild(i).gameObject;
                if (Application.isPlaying)
                    Destroy(child);
                else
                    DestroyImmediate(child);
                clearedBricks++;
            }
        }
        
        // Clear tracking collections
        instantiatedBricks.Clear();
        activeBricks.Clear();
        
        LogDebug($"   • Cleared {clearedContainers} row containers");
        LogDebug($"   • Cleared {clearedBricks} brick GameObjects");
        LogDebug("✅ [BrickGrid] Hierarchy cleanup complete");
    }
    
    /// <summary>
    /// Generates a consistent brick name based on row and column indices.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Formatted brick name</returns>
    private string GetBrickName(int row, int column)
    {
        return $"Brick_R{row:D2}C{column:D2}";
    }
    
    /// <summary>
    /// Extracts row index from a brick GameObject's name or position.
    /// </summary>
    /// <param name="brickObject">Brick GameObject to analyze</param>
    /// <returns>Row index, or -1 if unable to determine</returns>
    private int GetBrickRowFromGameObject(GameObject brickObject)
    {
        if (brickObject == null) return -1;
        
        // Try to extract from name pattern "Brick_R##C##"
        string name = brickObject.name;
        if (name.Contains("_R") && name.Contains("C"))
        {
            try
            {
                int rIndex = name.IndexOf("_R") + 2;
                int cIndex = name.IndexOf("C");
                if (rIndex > 1 && cIndex > rIndex)
                {
                    string rowStr = name.Substring(rIndex, cIndex - rIndex);
                    if (int.TryParse(rowStr, out int rowIndex))
                    {
                        return rowIndex;
                    }
                }
            }
            catch (System.Exception e)
            {
                LogWarning($"⚠️ [BrickGrid] Failed to parse row from brick name '{name}': {e.Message}");
            }
        }
        
        // Fallback: try to determine from position relative to grid
        if (gridConfiguration != null)
        {
            Vector3 localPos = transform.InverseTransformPoint(brickObject.transform.position);
            Vector3 startPos = gridStartPosition;
            
            // Calculate approximate row based on vertical position
            float verticalOffset = localPos.y - startPos.y;
            int estimatedRow = Mathf.RoundToInt(verticalOffset / gridConfiguration.verticalSpacing);
            
            if (estimatedRow >= 0 && estimatedRow < gridConfiguration.rows)
            {
                return estimatedRow;
            }
        }
        
        LogWarning($"⚠️ [BrickGrid] Unable to determine row for brick: {name}");
        return -1;
    }
    
    /// <summary>
    /// Validates the hierarchy integrity and reports any issues.
    /// </summary>
    private void ValidateHierarchyIntegrity()
    {
        if (!enableDebugLogging) return;
        
        int totalBricks = 0;
        int orphanedBricks = 0;
        int validContainers = 0;
        
        // Validate grid container
        if (gridContainer == null)
        {
            LogWarning("⚠️ [BrickGrid] Grid container is null");
            return;
        }
        
        // Count bricks in row containers
        if (useRowOrganization && rowContainers != null)
        {
            for (int i = 0; i < rowContainers.Length; i++)
            {
                if (rowContainers[i] != null)
                {
                    validContainers++;
                    totalBricks += rowContainers[i].transform.childCount;
                }
            }
        }
        
        // Count direct children of grid container
        int directChildren = gridContainer.transform.childCount;
        if (useRowOrganization)
        {
            // In row organization mode, direct children should only be row containers
            totalBricks += directChildren - validContainers; // Non-container direct children are considered orphaned
            orphanedBricks = directChildren - validContainers;
        }
        else
        {
            // In non-row mode, direct children are the bricks
            totalBricks += directChildren;
        }
        
        LogDebug($"   • Hierarchy validation: {totalBricks} total bricks, {validContainers} containers, {orphanedBricks} orphaned bricks");
        
        if (orphanedBricks > 0)
        {
            LogWarning($"⚠️ [BrickGrid] Found {orphanedBricks} orphaned bricks not properly organized");
        }
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
    /// Creates brick GameObjects using pattern-based generation with proper positioning and configuration.
    /// </summary>
    public void GenerateGrid()
    {
        LogDebug("🏗️ [BrickGrid] Starting grid generation...");
        
        if (!ValidateGridGeneration())
        {
            LogError("❌ [BrickGrid] Grid generation validation failed");
            return;
        }
        
        // Determine which pattern to use
        LayoutPattern patternToUse = useGridDataPattern && gridConfiguration != null ? 
            gridConfiguration.pattern : currentPattern;
        
        // Generate grid using pattern-based system
        GeneratePattern(patternToUse);
        
        LogDebug("✅ [BrickGrid] Grid generation completed");
    }
    
    /// <summary>
    /// Clears all bricks from the grid and resets state.
    /// Destroys all instantiated brick GameObjects and resets collections.
    /// </summary>
    public void ClearGrid()
    {
        LogDebug("🧹 [BrickGrid] Clearing existing grid...");
        
        // Use the hierarchy cleanup system for efficient clearing
        ClearHierarchy();
        
        // Reset grid state
        ResetGridState();
        
        LogDebug("🧹 [BrickGrid] Grid clearing complete");
    }
    
    /// <summary>
    /// Validates the current grid state and integrity with comprehensive checks.
    /// </summary>
    /// <returns>True if grid state is valid</returns>
    public bool ValidateGrid()
    {
        LogDebug("🧪 [BrickGrid] Starting comprehensive grid validation...");
        
        bool isValid = true;
        
        // Run all validation checks
        isValid &= ValidateGridConfiguration();
        isValid &= ValidateGeneratedGrid();
        isValid &= ValidateBrickCount();
        isValid &= ValidatePositionAccuracy();
        
        LogValidationResults("Grid Validation", isValid);
        return isValid;
    }
    
    /// <summary>
    /// Validates GridData configuration for errors before generation attempts.
    /// Checks for null references, invalid dimensions, and configuration conflicts.
    /// </summary>
    /// <returns>True if grid configuration is valid</returns>
    public bool ValidateGridConfiguration()
    {
        LogDebug("🔍 [Validation] Checking grid configuration...");
        
        bool isValid = true;
        List<string> errors = new List<string>();
        
        // Check GridData reference
        if (gridConfiguration == null)
        {
            errors.Add("GridData configuration is null");
            isValid = false;
        }
        else
        {
            // Check grid dimensions
            if (gridConfiguration.rows <= 0 || gridConfiguration.columns <= 0)
            {
                errors.Add($"Invalid grid dimensions: {gridConfiguration.rows}x{gridConfiguration.columns}");
                isValid = false;
            }
            
            // Check spacing values
            if (gridConfiguration.horizontalSpacing <= 0 || gridConfiguration.verticalSpacing <= 0)
            {
                errors.Add($"Invalid spacing values: {gridConfiguration.horizontalSpacing}x{gridConfiguration.verticalSpacing}");
                isValid = false;
            }
            
            // Check pattern density for random patterns
            if (gridConfiguration.pattern == LayoutPattern.Random)
            {
                float density = useGridDataPattern ? gridConfiguration.density : patternDensity;
                if (density <= 0f || density > 1f)
                {
                    errors.Add($"Invalid pattern density: {density} (must be 0.1-1.0)");
                    isValid = false;
                }
            }
        }
        
        // Check brick prefab reference
        if (brickPrefab == null)
        {
            errors.Add("Brick prefab is not assigned");
            isValid = false;
        }
        else
        {
            // Validate brick prefab has required components
            if (brickPrefab.GetComponent<Brick>() == null)
            {
                errors.Add("Brick prefab missing Brick component");
                isValid = false;
            }
        }
        
        // Check brick data configurations
        if (brickDataConfigurations == null || brickDataConfigurations.Length == 0)
        {
            errors.Add("No brick data configurations found");
            isValid = false;
        }
        
        // Log results
        if (errors.Count > 0)
        {
            LogWarning($"⚠️ [Validation] Configuration validation failed:");
            foreach (string error in errors)
            {
                LogWarning($"   • {error}");
            }
        }
        
        LogValidationResults("Configuration Validation", isValid);
        return isValid;
    }
    
    /// <summary>
    /// Validates generated grid accuracy and completeness.
    /// Verifies brick placement, counts, and hierarchy organization.
    /// </summary>
    /// <returns>True if generated grid is valid</returns>
    public bool ValidateGeneratedGrid()
    {
        LogDebug("🔍 [Validation] Checking generated grid...");
        
        if (!gridGenerated)
        {
            LogDebug("📋 [Validation] No grid generated - skipping generated grid validation");
            return true; // Valid state for non-generated grid
        }
        
        bool isValid = true;
        List<string> errors = new List<string>();
        
        // Check grid container exists
        if (gridContainer == null)
        {
            errors.Add("Grid container is missing");
            isValid = false;
        }
        
        // Validate brick counts match expected
        int expectedBricks = CalculateExpectedBrickCount();
        int actualBricks = instantiatedBricks.Count;
        
        if (actualBricks != expectedBricks)
        {
            errors.Add($"Brick count mismatch: expected {expectedBricks}, found {actualBricks}");
            isValid = false;
        }
        
        // Validate active brick components
        int activeBricksFound = 0;
        int nullBricksFound = 0;
        
        foreach (GameObject brickObj in instantiatedBricks)
        {
            if (brickObj == null)
            {
                nullBricksFound++;
                continue;
            }
            
            Brick brickComponent = brickObj.GetComponent<Brick>();
            if (brickComponent != null)
            {
                activeBricksFound++;
            }
        }
        
        if (nullBricksFound > 0)
        {
            errors.Add($"Found {nullBricksFound} null brick references");
            isValid = false;
        }
        
        if (activeBricksFound != actualBricks - nullBricksFound)
        {
            errors.Add($"Brick component mismatch: {activeBricksFound} components for {actualBricks - nullBricksFound} objects");
            isValid = false;
        }
        
        // Validate hierarchy organization
        if (useRowOrganization)
        {
            if (rowContainers.Length != gridConfiguration.rows)
            {
                errors.Add($"Row container count mismatch: expected {gridConfiguration.rows}, found {rowContainers.Length}");
                isValid = false;
            }
        }
        
        // Log results
        if (errors.Count > 0)
        {
            LogWarning($"⚠️ [Validation] Generated grid validation failed:");
            foreach (string error in errors)
            {
                LogWarning($"   • {error}");
            }
        }
        
        LogValidationResults("Generated Grid Validation", isValid);
        return isValid;
    }
    
    /// <summary>
    /// Test generation of all pattern types for validation and performance testing.
    /// </summary>
    public void TestAllPatterns()
    {
        LogDebug("🎭 [Validation] Testing all pattern types...");
        
        if (gridConfiguration == null)
        {
            LogWarning("⚠️ [Validation] Cannot test patterns - no grid configuration");
            return;
        }
        
        LayoutPattern originalPattern = gridConfiguration.pattern;
        bool wasGenerated = gridGenerated;
        
        LayoutPattern[] patterns = { LayoutPattern.Standard, LayoutPattern.Pyramid, LayoutPattern.Diamond, LayoutPattern.Random };
        
        foreach (LayoutPattern pattern in patterns)
        {
            LogDebug($"🧪 [Validation] Testing {pattern} pattern...");
            
            try
            {
                // Set pattern and generate
                gridConfiguration.pattern = pattern;
                if (wasGenerated) ClearGrid();
                
                System.DateTime startTime = System.DateTime.Now;
                GenerateGrid();
                System.TimeSpan generationTime = System.DateTime.Now - startTime;
                
                // Validate results
                bool isValid = ValidateGeneratedGrid();
                int brickCount = instantiatedBricks.Count;
                
                LogValidationResults($"{pattern} Pattern Test", isValid);
                LogDebug($"   • Generated {brickCount} bricks in {generationTime.TotalMilliseconds:F1}ms");
                
                if (!isValid)
                {
                    LogWarning($"⚠️ [Validation] {pattern} pattern validation failed");
                }
            }
            catch (System.Exception e)
            {
                LogWarning($"⚠️ [Validation] {pattern} pattern test failed: {e.Message}");
            }
        }
        
        // Restore original state
        gridConfiguration.pattern = originalPattern;
        if (wasGenerated)
        {
            ClearGrid();
            GenerateGrid();
        }
        
        LogDebug("✅ [Validation] Pattern testing complete");
    }
    
    /// <summary>
    /// Run performance testing for large grid configurations.
    /// </summary>
    public void RunPerformanceTest()
    {
        LogDebug("⚡ [Performance] Starting performance testing...");
        
        if (gridConfiguration == null)
        {
            LogWarning("⚠️ [Performance] Cannot test performance - no grid configuration");
            return;
        }
        
        // Store original configuration
        int originalRows = gridConfiguration.rows;
        int originalColumns = gridConfiguration.columns;
        LayoutPattern originalPattern = gridConfiguration.pattern;
        bool wasGenerated = gridGenerated;
        
        // Test configurations: small, medium, large
        var testConfigs = new[]
        {
            new { rows = 5, columns = 8, name = "Small Grid (5x8)" },
            new { rows = 10, columns = 15, name = "Medium Grid (10x15)" },
            new { rows = 15, columns = 20, name = "Large Grid (15x20)" }
        };
        
        foreach (var config in testConfigs)
        {
            LogDebug($"📊 [Performance] Testing {config.name}...");
            
            gridConfiguration.rows = config.rows;
            gridConfiguration.columns = config.columns;
            
            if (wasGenerated) ClearGrid();
            
            // Measure generation time
            System.DateTime startTime = System.DateTime.Now;
            GenerateGrid();
            System.TimeSpan generationTime = System.DateTime.Now - startTime;
            
            // Measure validation time
            System.DateTime validationStart = System.DateTime.Now;
            bool isValid = ValidateGeneratedGrid();
            System.TimeSpan validationTime = System.DateTime.Now - validationStart;
            
            int brickCount = instantiatedBricks.Count;
            
            LogDebug($"   • {config.name}: {brickCount} bricks");
            LogDebug($"   • Generation: {generationTime.TotalMilliseconds:F1}ms");
            LogDebug($"   • Validation: {validationTime.TotalMilliseconds:F1}ms");
            LogDebug($"   • Valid: {isValid}");
            
            // Performance recommendations
            if (generationTime.TotalMilliseconds > 100)
            {
                LogWarning($"⚠️ [Performance] Slow generation for {config.name}: {generationTime.TotalMilliseconds:F1}ms");
                LogDebug("   💡 Consider enabling batch instantiation or reducing grid size");
            }
        }
        
        // Restore original configuration
        gridConfiguration.rows = originalRows;
        gridConfiguration.columns = originalColumns;
        gridConfiguration.pattern = originalPattern;
        
        if (wasGenerated)
        {
            ClearGrid();
            GenerateGrid();
        }
        
        LogDebug("✅ [Performance] Performance testing complete");
    }
    
    /// <summary>
    /// Utility method to validate brick count consistency.
    /// </summary>
    /// <returns>True if brick counts are consistent</returns>
    private bool ValidateBrickCount()
    {
        LogDebug("🔢 [Validation] Checking brick count consistency...");
        
        if (!gridGenerated)
        {
            return true; // No grid to validate
        }
        
        bool isValid = true;
        
        // Check instantiated bricks count
        int instantiatedCount = instantiatedBricks.Count;
        int activeBricksCount = activeBricks.Count;
        int serializedBrickCount = brickCount;
        
        // Compare counts
        if (instantiatedCount != activeBricksCount)
        {
            LogWarning($"⚠️ [Validation] Brick count mismatch: instantiated={instantiatedCount}, active={activeBricksCount}");
            isValid = false;
        }
        
        if (serializedBrickCount != activeBricksCount)
        {
            LogWarning($"⚠️ [Validation] Serialized count mismatch: serialized={serializedBrickCount}, active={activeBricksCount}");
            isValid = false;
        }
        
        LogValidationResults("Brick Count Validation", isValid);
        return isValid;
    }
    
    /// <summary>
    /// Utility method to validate position accuracy of placed bricks.
    /// </summary>
    /// <returns>True if brick positions are accurate</returns>
    private bool ValidatePositionAccuracy()
    {
        LogDebug("📐 [Validation] Checking position accuracy...");
        
        if (!gridGenerated || gridConfiguration == null)
        {
            return true; // No grid to validate
        }
        
        bool isValid = true;
        int positionErrors = 0;
        float tolerance = 0.1f; // Position tolerance
        
        // Check each instantiated brick position
        foreach (GameObject brickObj in instantiatedBricks)
        {
            if (brickObj == null) continue;
            
            // Try to determine expected position from brick name or position
            Vector3 actualPosition = brickObj.transform.position;
            
            // Find closest expected grid position
            float minDistance = float.MaxValue;
            for (int row = 0; row < gridConfiguration.rows; row++)
            {
                for (int col = 0; col < gridConfiguration.columns; col++)
                {
                    Vector3 expectedPosition = CalculateGridPosition(row, col);
                    float distance = Vector3.Distance(actualPosition, expectedPosition);
                    minDistance = Mathf.Min(minDistance, distance);
                }
            }
            
            if (minDistance > tolerance)
            {
                positionErrors++;
                if (positionErrors <= 5) // Limit error spam
                {
                    LogWarning($"⚠️ [Validation] Position error: {brickObj.name} at {actualPosition}, closest expected distance: {minDistance:F3}");
                }
            }
        }
        
        if (positionErrors > 0)
        {
            LogWarning($"⚠️ [Validation] Found {positionErrors} position errors (tolerance: {tolerance})");
            isValid = false;
        }
        
        LogValidationResults("Position Accuracy Validation", isValid);
        return isValid;
    }
    
    /// <summary>
    /// Calculate expected brick count for current configuration and pattern.
    /// </summary>
    /// <returns>Expected number of bricks</returns>
    private int CalculateExpectedBrickCount()
    {
        if (gridConfiguration == null) return 0;
        
        int totalPositions = gridConfiguration.rows * gridConfiguration.columns;
        
        // Pattern-specific calculations
        switch (gridConfiguration.pattern)
        {
            case LayoutPattern.Standard:
                return totalPositions;
                
            case LayoutPattern.Pyramid:
                // Triangular pattern has fewer bricks
                int pyramidCount = 0;
                for (int row = 0; row < gridConfiguration.rows; row++)
                {
                    pyramidCount += CalculatePyramidBricksInRow(row, gridConfiguration.rows, gridConfiguration.columns);
                }
                return pyramidCount;
                
            case LayoutPattern.Diamond:
                // Diamond pattern calculation
                int diamondCount = 0;
                for (int row = 0; row < gridConfiguration.rows; row++)
                {
                    diamondCount += CalculateDiamondBricksInRow(row, gridConfiguration.rows, gridConfiguration.columns);
                }
                return diamondCount;
                
            case LayoutPattern.Random:
                // Approximate based on density
                float density = useGridDataPattern ? gridConfiguration.density : patternDensity;
                return Mathf.RoundToInt(totalPositions * density);
                
            default:
                return totalPositions;
        }
    }
    
    /// <summary>
    /// Logs validation results in a consistent format.
    /// </summary>
    /// <param name="testName">Name of the validation test</param>
    /// <param name="passed">Whether the test passed</param>
    private void LogValidationResults(string testName, bool passed)
    {
        if (passed)
        {
            LogDebug($"✅ [Validation] {testName}: PASSED");
        }
        else
        {
            LogWarning($"❌ [Validation] {testName}: FAILED");
        }
    }
    
    #endregion
    
    #region Brick Instantiation System
    
    /// <summary>
    /// Creates a single brick GameObject at the specified position with proper configuration.
    /// Handles prefab instantiation, positioning, and component configuration.
    /// </summary>
    /// <param name="position">World position for brick placement</param>
    /// <param name="brickType">Type of brick to create</param>
    /// <returns>Instantiated brick GameObject, or null if instantiation failed</returns>
    public GameObject InstantiateBrick(Vector3 position, BrickType brickType)
    {
        return InstantiateBrick(position, brickType, -1, -1);
    }
    
    /// <summary>
    /// Creates a single brick GameObject with proper hierarchy organization and naming.
    /// </summary>
    /// <param name="position">World position for brick placement</param>
    /// <param name="brickType">Type of brick to create</param>
    /// <param name="row">Row index for hierarchy organization (-1 to disable)</param>
    /// <param name="column">Column index for naming (-1 to use default naming)</param>
    /// <returns>Instantiated brick GameObject, or null if instantiation failed</returns>
    public GameObject InstantiateBrick(Vector3 position, BrickType brickType, int row, int column)
    {
        if (brickPrefab == null)
        {
            LogError("❌ [BrickGrid] Cannot instantiate brick: No brick prefab assigned");
            return null;
        }
        
        try
        {
            // Determine appropriate parent for hierarchy organization
            Transform parentTransform = GetBrickParentTransform(row);
            
            // Instantiate brick GameObject
            GameObject brickInstance = Instantiate(brickPrefab, position, Quaternion.identity, parentTransform);
            
            // Configure brick instance
            if (!ConfigureBrickInstance(brickInstance, brickType))
            {
                LogError($"❌ [BrickGrid] Failed to configure brick instance at {position}");
                if (Application.isPlaying)
                    Destroy(brickInstance);
                else
                    DestroyImmediate(brickInstance);
                return null;
            }
            
            // Add to tracking collections
            instantiatedBricks.Add(brickInstance);
            
            Brick brickComponent = brickInstance.GetComponent<Brick>();
            if (brickComponent != null)
            {
                activeBricks.Add(brickComponent);
            }
            
            // Generate proper name based on row/column information
            if (row >= 0 && column >= 0)
            {
                brickInstance.name = GetBrickName(row, column);
            }
            else
            {
                brickInstance.name = $"Brick_{brickType}_{activeBricks.Count:D3}";
            }
            
            LogDebug($"🧱 [BrickGrid] Instantiated {brickType} brick '{brickInstance.name}' at {position}");
            return brickInstance;
        }
        catch (System.Exception e)
        {
            LogError($"❌ [BrickGrid] Exception during brick instantiation: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Gets the appropriate parent Transform for a brick based on row index and organization settings.
    /// </summary>
    /// <param name="row">Row index (-1 for default parent)</param>
    /// <returns>Transform to use as parent</returns>
    private Transform GetBrickParentTransform(int row)
    {
        if (row < 0 || !useRowOrganization)
        {
            // Use default brick parent (grid container or configured parent)
            return BrickParent;
        }
        
        // Ensure row containers array is initialized
        if (gridConfiguration != null && (rowContainers == null || rowContainers.Length != gridConfiguration.rows))
        {
            rowContainers = new GameObject[gridConfiguration.rows];
        }
        
        // Create row container if it doesn't exist
        if (rowContainers != null && row < rowContainers.Length)
        {
            if (rowContainers[row] == null)
            {
                rowContainers[row] = CreateRowContainer(row);
            }
            return rowContainers[row].transform;
        }
        
        // Fallback to default parent
        LogWarning($"⚠️ [BrickGrid] Unable to create row container for row {row}, using default parent");
        return BrickParent;
    }
    
    /// <summary>
    /// Generates the complete grid of bricks using batch instantiation for efficiency.
    /// Creates bricks based on GridData configuration and layout patterns.
    /// </summary>
    public void GenerateGridBricks()
    {
        if (gridConfiguration == null)
        {
            LogError("❌ [BrickGrid] Cannot generate grid: No GridData configuration");
            return;
        }
        
        LogDebug($"🏗️ [BrickGrid] Generating {gridConfiguration.rows}×{gridConfiguration.columns} grid with {gridConfiguration.pattern} pattern");
        
        int totalBricks = 0;
        int successfulBricks = 0;
        System.DateTime startTime = System.DateTime.Now;
        
        // Generate bricks for each grid position
        for (int row = 0; row < gridConfiguration.rows; row++)
        {
            for (int column = 0; column < gridConfiguration.columns; column++)
            {
                totalBricks++;
                
                // Calculate position using positioning mathematics
                Vector3 brickPosition = CalculateGridPosition(row, column);
                
                // Skip hidden positions (for pattern layouts)
                if (brickPosition.y < -100f)
                {
                    LogDebug($"   • Skipping hidden position [{row},{column}] for {gridConfiguration.pattern} pattern");
                    continue;
                }
                
                // Determine brick type for this position
                BrickType brickType = gridConfiguration.GetBrickTypeForRow(row);
                
                // Handle random pattern density
                if (gridConfiguration.pattern == LayoutPattern.Random)
                {
                    if (Random.value > gridConfiguration.density)
                    {
                        LogDebug($"   • Skipping position [{row},{column}] due to random density ({gridConfiguration.density:F1})");
                        continue;
                    }
                }
                
                // Instantiate brick with proper hierarchy organization
                GameObject brickInstance = InstantiateBrick(brickPosition, brickType, row, column);
                if (brickInstance != null)
                {
                    successfulBricks++;
                }
                
                // Performance throttling for large grids
                if (!useBatchInstantiation && successfulBricks % maxBricksPerFrame == 0)
                {
                    // In a real implementation, this could yield control back to Unity
                    // For now, we'll just log progress
                    LogDebug($"   • Progress: {successfulBricks}/{totalBricks} bricks instantiated");
                }
            }
        }
        
        // Update grid state
        SetGridGenerated(successfulBricks);
        
        // Calculate generation time
        System.TimeSpan generationTime = System.DateTime.Now - startTime;
        
        LogDebug($"✅ [BrickGrid] Grid generation complete:");
        LogDebug($"   • Total positions: {totalBricks}");
        LogDebug($"   • Successful bricks: {successfulBricks}");
        LogDebug($"   • Generation time: {generationTime.TotalMilliseconds:F1}ms");
        LogDebug($"   • Average per brick: {(generationTime.TotalMilliseconds / successfulBricks):F2}ms");
    }
    
    #endregion
    
    #region Layout Pattern Implementation
    
    /// <summary>
    /// Generates brick layout using specified pattern algorithm.
    /// Uses strategy pattern to switch between different formation types.
    /// </summary>
    /// <param name="pattern">Layout pattern to generate</param>
    public void GeneratePattern(LayoutPattern pattern)
    {
        if (gridConfiguration == null)
        {
            LogError("❌ [BrickGrid] Cannot generate pattern: No GridData configuration");
            return;
        }
        
        LogDebug($"🎨 [BrickGrid] Generating {pattern} pattern...");
        System.DateTime startTime = System.DateTime.Now;
        
        // Set random seed for reproducible patterns
        Random.InitState(patternSeed);
        
        // Clear existing grid before generating new pattern
        ClearGrid();
        
        int generatedBricks = 0;
        
        // Execute pattern-specific generation algorithm
        switch (pattern)
        {
            case LayoutPattern.Standard:
                generatedBricks = GenerateStandardPattern();
                break;
                
            case LayoutPattern.Pyramid:
                generatedBricks = GeneratePyramidPattern();
                break;
                
            case LayoutPattern.Diamond:
                generatedBricks = GenerateDiamondPattern();
                break;
                
            case LayoutPattern.Random:
                generatedBricks = GenerateRandomPattern();
                break;
                
            case LayoutPattern.Custom:
                generatedBricks = GenerateCustomPattern();
                break;
                
            default:
                LogWarning($"⚠️ [BrickGrid] Unknown pattern {pattern}, using Standard");
                generatedBricks = GenerateStandardPattern();
                break;
        }
        
        // Update grid state and log results
        SetGridGenerated(generatedBricks);
        System.TimeSpan generationTime = System.DateTime.Now - startTime;
        
        LogDebug($"✅ [BrickGrid] {pattern} pattern complete:");
        LogDebug($"   • Generated bricks: {generatedBricks}");
        LogDebug($"   • Generation time: {generationTime.TotalMilliseconds:F1}ms");
    }
    
    /// <summary>
    /// Generates classic Breakout brick wall formation with uniform rows and columns.
    /// Creates traditional pattern with configurable brick type distribution per row.
    /// </summary>
    /// <returns>Number of bricks successfully generated</returns>
    private int GenerateStandardPattern()
    {
        LogDebug("   • Executing Standard pattern algorithm...");
        
        int successfulBricks = 0;
        
        // Generate uniform grid with all positions filled
        for (int row = 0; row < gridConfiguration.rows; row++)
        {
            for (int column = 0; column < gridConfiguration.columns; column++)
            {
                // Calculate position using existing positioning system
                Vector3 brickPosition = CalculateGridPosition(row, column);
                
                // Determine brick type for this position
                BrickType brickType = GetBrickTypeForPosition(row, column, LayoutPattern.Standard);
                
                // Instantiate brick with hierarchy organization
                GameObject brickInstance = InstantiateBrick(brickPosition, brickType, row, column);
                if (brickInstance != null)
                {
                    successfulBricks++;
                }
            }
        }
        
        LogDebug($"   • Standard pattern: {successfulBricks} bricks in {gridConfiguration.rows}×{gridConfiguration.columns} grid");
        return successfulBricks;
    }
    
    /// <summary>
    /// Generates triangular formation starting wide at bottom and narrowing upward.
    /// Creates challenging gameplay with focused destruction patterns.
    /// </summary>
    /// <returns>Number of bricks successfully generated</returns>
    private int GeneratePyramidPattern()
    {
        LogDebug("   • Executing Pyramid pattern algorithm...");
        
        int successfulBricks = 0;
        int totalRows = gridConfiguration.rows;
        
        for (int row = 0; row < totalRows; row++)
        {
            // Calculate pyramid reduction: fewer bricks per row as we go up
            int bricksInRow = CalculatePyramidBricksInRow(row, totalRows, gridConfiguration.columns);
            int startColumn = (gridConfiguration.columns - bricksInRow) / 2;
            
            for (int column = startColumn; column < startColumn + bricksInRow; column++)
            {
                if (ShouldPlaceBrickAtPosition(row, column, LayoutPattern.Pyramid))
                {
                    Vector3 brickPosition = CalculateGridPosition(row, column);
                    BrickType brickType = GetBrickTypeForPosition(row, column, LayoutPattern.Pyramid);
                    
                    GameObject brickInstance = InstantiateBrick(brickPosition, brickType, row, column);
                    if (brickInstance != null)
                    {
                        successfulBricks++;
                    }
                }
            }
        }
        
        LogDebug($"   • Pyramid pattern: {successfulBricks} bricks in triangular formation");
        return successfulBricks;
    }
    
    /// <summary>
    /// Generates diamond/rhombus shape with symmetric layout.
    /// Provides balanced destruction opportunities with optional hollow center.
    /// </summary>
    /// <returns>Number of bricks successfully generated</returns>
    private int GenerateDiamondPattern()
    {
        LogDebug("   • Executing Diamond pattern algorithm...");
        
        int successfulBricks = 0;
        int totalRows = gridConfiguration.rows;
        int totalColumns = gridConfiguration.columns;
        int midRow = totalRows / 2;
        
        for (int row = 0; row < totalRows; row++)
        {
            // Calculate diamond shape: expand from center, then contract
            int bricksInRow = CalculateDiamondBricksInRow(row, totalRows, totalColumns);
            int startColumn = (totalColumns - bricksInRow) / 2;
            
            for (int column = startColumn; column < startColumn + bricksInRow; column++)
            {
                if (ShouldPlaceBrickAtPosition(row, column, LayoutPattern.Diamond))
                {
                    // Skip hollow center if enabled
                    if (hollowCenter && IsInDiamondCenter(row, column, totalRows, totalColumns))
                    {
                        continue;
                    }
                    
                    Vector3 brickPosition = CalculateGridPosition(row, column);
                    BrickType brickType = GetBrickTypeForPosition(row, column, LayoutPattern.Diamond);
                    
                    GameObject brickInstance = InstantiateBrick(brickPosition, brickType, row, column);
                    if (brickInstance != null)
                    {
                        successfulBricks++;
                    }
                }
            }
        }
        
        LogDebug($"   • Diamond pattern: {successfulBricks} bricks in {(hollowCenter ? "hollow " : "")}diamond formation");
        return successfulBricks;
    }
    
    /// <summary>
    /// Generates random placement with configurable density.
    /// Includes playability validation to ensure balanced gameplay.
    /// </summary>
    /// <returns>Number of bricks successfully generated</returns>
    private int GenerateRandomPattern()
    {
        LogDebug("   • Executing Random pattern algorithm...");
        
        int successfulBricks = 0;
        float effectiveDensity = useGridDataPattern ? gridConfiguration.density : patternDensity;
        
        // Generate random placement with density control
        for (int row = 0; row < gridConfiguration.rows; row++)
        {
            for (int column = 0; column < gridConfiguration.columns; column++)
            {
                // Use density to determine if brick should be placed
                if (Random.value <= effectiveDensity)
                {
                    if (ShouldPlaceBrickAtPosition(row, column, LayoutPattern.Random))
                    {
                        Vector3 brickPosition = CalculateGridPosition(row, column);
                        BrickType brickType = GetBrickTypeForPosition(row, column, LayoutPattern.Random);
                        
                        GameObject brickInstance = InstantiateBrick(brickPosition, brickType, row, column);
                        if (brickInstance != null)
                        {
                            successfulBricks++;
                        }
                    }
                }
            }
        }
        
        // Validate minimum brick count for playability
        int minBricks = Mathf.Max(5, (gridConfiguration.rows * gridConfiguration.columns) / 10);
        if (successfulBricks < minBricks)
        {
            LogWarning($"⚠️ [BrickGrid] Random pattern generated {successfulBricks} bricks (minimum: {minBricks})");
            LogWarning("   • Consider increasing density or adjusting grid size for better gameplay");
        }
        
        LogDebug($"   • Random pattern: {successfulBricks} bricks with {effectiveDensity:F1} density");
        return successfulBricks;
    }
    
    /// <summary>
    /// Generates custom pattern based on GridData configuration.
    /// Placeholder for user-defined patterns and specialized layouts.
    /// </summary>
    /// <returns>Number of bricks successfully generated</returns>
    private int GenerateCustomPattern()
    {
        LogDebug("   • Executing Custom pattern algorithm...");
        
        // For now, fall back to standard pattern
        LogWarning("⚠️ [BrickGrid] Custom pattern not implemented, using Standard pattern");
        return GenerateStandardPattern();
    }
    
    #endregion
    
    #region Pattern Utility Methods
    
    /// <summary>
    /// Determines if a brick should be placed at the specified position for the given pattern.
    /// Provides pattern-specific validation and placement logic.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <param name="pattern">Layout pattern being generated</param>
    /// <returns>True if brick should be placed at position</returns>
    private bool ShouldPlaceBrickAtPosition(int row, int column, LayoutPattern pattern)
    {
        // Basic bounds checking
        if (row < 0 || row >= gridConfiguration.rows || column < 0 || column >= gridConfiguration.columns)
        {
            return false;
        }
        
        // Pattern-specific placement rules
        switch (pattern)
        {
            case LayoutPattern.Standard:
                return true; // All positions valid for standard pattern
                
            case LayoutPattern.Pyramid:
                return IsValidPyramidPosition(row, column);
                
            case LayoutPattern.Diamond:
                return IsValidDiamondPosition(row, column);
                
            case LayoutPattern.Random:
                return ValidateRandomPlacement(row, column);
                
            case LayoutPattern.Custom:
                return true; // Custom logic would go here
                
            default:
                return true;
        }
    }
    
    /// <summary>
    /// Determines appropriate brick type for position based on pattern and configuration.
    /// Provides pattern-specific brick type distribution logic.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <param name="pattern">Layout pattern being generated</param>
    /// <returns>Brick type for this position</returns>
    private BrickType GetBrickTypeForPosition(int row, int column, LayoutPattern pattern)
    {
        // Use GridData row-based type distribution if available
        if (gridConfiguration.rowBrickTypes != null && gridConfiguration.rowBrickTypes.Length > row)
        {
            return gridConfiguration.rowBrickTypes[row];
        }
        
        // Pattern-specific type distribution
        switch (pattern)
        {
            case LayoutPattern.Standard:
                return GetStandardPatternBrickType(row, column);
                
            case LayoutPattern.Pyramid:
                return GetPyramidPatternBrickType(row, column);
                
            case LayoutPattern.Diamond:
                return GetDiamondPatternBrickType(row, column);
                
            case LayoutPattern.Random:
                return GetRandomPatternBrickType(row, column);
                
            default:
                return BrickType.Normal;
        }
    }
    
    /// <summary>
    /// Calculates number of bricks in a row for pyramid pattern.
    /// </summary>
    /// <param name="row">Current row index</param>
    /// <param name="totalRows">Total number of rows</param>
    /// <param name="maxColumns">Maximum columns available</param>
    /// <returns>Number of bricks for this row</returns>
    private int CalculatePyramidBricksInRow(int row, int totalRows, int maxColumns)
    {
        // Pyramid narrows from bottom to top
        float reductionFactor = (float)row / totalRows;
        int reduction = Mathf.FloorToInt(reductionFactor * maxColumns * 0.5f);
        return Mathf.Max(1, maxColumns - reduction * 2);
    }
    
    /// <summary>
    /// Calculates number of bricks in a row for diamond pattern.
    /// </summary>
    /// <param name="row">Current row index</param>
    /// <param name="totalRows">Total number of rows</param>
    /// <param name="maxColumns">Maximum columns available</param>
    /// <returns>Number of bricks for this row</returns>
    private int CalculateDiamondBricksInRow(int row, int totalRows, int maxColumns)
    {
        int midRow = totalRows / 2;
        int distanceFromCenter = Mathf.Abs(row - midRow);
        float expansionFactor = 1.0f - ((float)distanceFromCenter / midRow);
        return Mathf.Max(1, Mathf.FloorToInt(maxColumns * expansionFactor));
    }
    
    /// <summary>
    /// Determines if position is in the hollow center of a diamond pattern.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <param name="totalRows">Total rows in grid</param>
    /// <param name="totalColumns">Total columns in grid</param>
    /// <returns>True if position is in hollow center</returns>
    private bool IsInDiamondCenter(int row, int column, int totalRows, int totalColumns)
    {
        int midRow = totalRows / 2;
        int midColumn = totalColumns / 2;
        
        // Define center area as 25% of diamond size
        int centerRadiusRow = Mathf.Max(1, totalRows / 4);
        int centerRadiusColumn = Mathf.Max(1, totalColumns / 4);
        
        return Mathf.Abs(row - midRow) <= centerRadiusRow && 
               Mathf.Abs(column - midColumn) <= centerRadiusColumn;
    }
    
    /// <summary>
    /// Validates if position is valid for pyramid pattern.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>True if position is valid for pyramid</returns>
    private bool IsValidPyramidPosition(int row, int column)
    {
        int bricksInRow = CalculatePyramidBricksInRow(row, gridConfiguration.rows, gridConfiguration.columns);
        int startColumn = (gridConfiguration.columns - bricksInRow) / 2;
        return column >= startColumn && column < startColumn + bricksInRow;
    }
    
    /// <summary>
    /// Validates if position is valid for diamond pattern.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>True if position is valid for diamond</returns>
    private bool IsValidDiamondPosition(int row, int column)
    {
        int bricksInRow = CalculateDiamondBricksInRow(row, gridConfiguration.rows, gridConfiguration.columns);
        int startColumn = (gridConfiguration.columns - bricksInRow) / 2;
        return column >= startColumn && column < startColumn + bricksInRow;
    }
    
    /// <summary>
    /// Validates random placement to ensure balanced distribution.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>True if position passes validation</returns>
    private bool ValidateRandomPlacement(int row, int column)
    {
        // Basic validation - could be enhanced with cluster prevention
        return true;
    }
    
    /// <summary>
    /// Gets brick type for standard pattern positions.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Appropriate brick type</returns>
    private BrickType GetStandardPatternBrickType(int row, int column)
    {
        // Standard pattern: varied types based on row position
        if (row < gridConfiguration.rows * 0.3f) return BrickType.Normal;
        if (row < gridConfiguration.rows * 0.7f) return BrickType.Reinforced;
        return BrickType.PowerUp;
    }
    
    /// <summary>
    /// Gets brick type for pyramid pattern positions.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Appropriate brick type</returns>
    private BrickType GetPyramidPatternBrickType(int row, int column)
    {
        // Pyramid: stronger bricks at the top (harder to reach)
        float rowRatio = (float)row / gridConfiguration.rows;
        if (rowRatio > 0.8f) return BrickType.PowerUp;
        if (rowRatio > 0.5f) return BrickType.Reinforced;
        return BrickType.Normal;
    }
    
    /// <summary>
    /// Gets brick type for diamond pattern positions.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Appropriate brick type</returns>
    private BrickType GetDiamondPatternBrickType(int row, int column)
    {
        // Diamond: special bricks at edges, normal in center
        int midRow = gridConfiguration.rows / 2;
        int midColumn = gridConfiguration.columns / 2;
        float distanceFromCenter = Vector2.Distance(new Vector2(row, column), new Vector2(midRow, midColumn));
        float maxDistance = Vector2.Distance(Vector2.zero, new Vector2(midRow, midColumn));
        float edgeRatio = distanceFromCenter / maxDistance;
        
        if (edgeRatio > 0.8f) return BrickType.PowerUp;
        if (edgeRatio > 0.5f) return BrickType.Reinforced;
        return BrickType.Normal;
    }
    
    /// <summary>
    /// Gets brick type for random pattern positions.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Appropriate brick type</returns>
    private BrickType GetRandomPatternBrickType(int row, int column)
    {
        // Random distribution with weighted probabilities
        float rand = Random.value;
        if (rand < 0.1f) return BrickType.PowerUp;
        if (rand < 0.3f) return BrickType.Reinforced;
        if (rand < 0.05f) return BrickType.Indestructible;
        return BrickType.Normal;
    }
    
    #endregion
    
    #region Brick Configuration and Validation
    
    /// <summary>
    /// Configures a brick instance with proper BrickData and component setup.
    /// </summary>
    /// <param name="brickInstance">Brick GameObject to configure</param>
    /// <param name="brickType">Type of brick to configure</param>
    /// <returns>True if configuration successful, false otherwise</returns>
    private bool ConfigureBrickInstance(GameObject brickInstance, BrickType brickType)
    {
        if (brickInstance == null)
        {
            LogError("❌ [BrickGrid] Cannot configure null brick instance");
            return false;
        }
        
        // Get or add Brick component
        Brick brickComponent = brickInstance.GetComponent<Brick>();
        if (brickComponent == null)
        {
            LogWarning($"⚠️ [BrickGrid] Adding missing Brick component to {brickInstance.name}");
            brickComponent = brickInstance.AddComponent<Brick>();
        }
        
        // Get BrickData for this type
        BrickData brickData = GetBrickDataForType(brickType);
        if (brickData == null)
        {
            LogWarning($"⚠️ [BrickGrid] No BrickData found for {brickType}, using default");
            brickData = BrickData.CreateNormal();
        }
        
        // Initialize brick with configuration
        try
        {
            brickComponent.Initialize(brickData);
            LogDebug($"   • Configured {brickType} brick with {brickData.hitPoints}HP, {brickData.scoreValue} points");
            return true;
        }
        catch (System.Exception e)
        {
            LogError($"❌ [BrickGrid] Failed to initialize brick component: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Validates prefab references and configuration arrays.
    /// </summary>
    /// <returns>True if validation passes, false otherwise</returns>
    private bool ValidatePrefabReferences()
    {
        bool isValid = true;
        
        // Check brick prefab
        if (brickPrefab == null)
        {
            LogWarning("⚠️ [BrickGrid] No brick prefab assigned - instantiation will fail");
            isValid = false;
        }
        else
        {
            // Validate prefab has Brick component
            Brick prefabBrick = brickPrefab.GetComponent<Brick>();
            if (prefabBrick == null)
            {
                LogWarning("⚠️ [BrickGrid] Brick prefab missing Brick component - will be added during instantiation");
            }
            else
            {
                LogDebug("   • Brick prefab validation: ✅");
            }
        }
        
        // Check BrickData configurations
        if (brickDataConfigurations == null || brickDataConfigurations.Length == 0)
        {
            LogWarning("⚠️ [BrickGrid] No BrickData configurations - will use defaults");
        }
        else
        {
            int validConfigs = 0;
            foreach (BrickData config in brickDataConfigurations)
            {
                if (config != null && config.IsValidConfiguration())
                {
                    validConfigs++;
                }
            }
            LogDebug($"   • BrickData configurations: {validConfigs}/{brickDataConfigurations.Length} valid");
        }
        
        // Check parent transform setup
        Transform parent = BrickParent;
        if (parent != null)
        {
            LogDebug($"   • Brick parent: {parent.name} ✅");
        }
        
        return isValid;
    }
    
    /// <summary>
    /// Gets BrickData configuration for the specified brick type.
    /// </summary>
    /// <param name="brickType">Type of brick data to retrieve</param>
    /// <returns>BrickData instance, or null if not found</returns>
    private BrickData GetBrickDataForType(BrickType brickType)
    {
        // Try to find in configuration array first
        if (brickDataConfigurations != null && brickDataConfigurations.Length > 0)
        {
            foreach (BrickData config in brickDataConfigurations)
            {
                if (config != null && config.brickType == brickType)
                {
                    return config;
                }
            }
        }
        
        // Fallback to factory methods
        try
        {
            switch (brickType)
            {
                case BrickType.Normal:
                    return BrickData.CreateNormal();
                case BrickType.Reinforced:
                    return BrickData.CreateReinforced();
                case BrickType.PowerUp:
                    return BrickData.CreatePowerUp();
                case BrickType.Indestructible:
                    return BrickData.CreateIndestructible();
                default:
                    LogWarning($"⚠️ [BrickGrid] Unknown brick type: {brickType}");
                    return BrickData.CreateNormal();
            }
        }
        catch (System.Exception e)
        {
            LogError($"❌ [BrickGrid] Failed to create BrickData for {brickType}: {e.Message}");
            return null;
        }
    }
    
    #endregion
    
    #region Mathematical Positioning Methods
    
    /// <summary>
    /// Calculates the world position for a brick at the specified grid coordinates.
    /// Takes into account spacing, staggering, and grid offset configuration.
    /// </summary>
    /// <param name="row">Zero-based row index (0 = bottom row)</param>
    /// <param name="column">Zero-based column index (0 = leftmost column)</param>
    /// <returns>World position for brick placement</returns>
    public Vector3 CalculateGridPosition(int row, int column)
    {
        if (gridConfiguration == null)
        {
            LogWarning("⚠️ [BrickGrid] CalculateGridPosition: No GridData configuration");
            return Vector3.zero;
        }
        
        // Validate grid coordinates
        if (row < 0 || row >= gridConfiguration.rows || column < 0 || column >= gridConfiguration.columns)
        {
            LogWarning($"⚠️ [BrickGrid] Invalid grid coordinates: row={row}, column={column}");
            return Vector3.zero;
        }
        
        // Get base starting position
        Vector3 startPosition = GetStartingPosition();
        
        // Calculate base position
        float xPosition = startPosition.x + (column * gridConfiguration.horizontalSpacing);
        float yPosition = startPosition.y + (row * gridConfiguration.verticalSpacing);
        
        // Apply staggering if enabled (alternating rows offset)
        if (gridConfiguration.enableStaggering && row % 2 == 1)
        {
            xPosition += gridConfiguration.horizontalSpacing * gridConfiguration.staggerOffset;
        }
        
        // Apply any custom pattern offsets
        Vector3 patternOffset = GetPatternOffset(row, column);
        
        return new Vector3(xPosition, yPosition, startPosition.z) + patternOffset;
    }
    
    /// <summary>
    /// Calculates the center point of the entire grid formation.
    /// Useful for camera positioning and visual effects.
    /// </summary>
    /// <returns>World position of grid center</returns>
    public Vector3 CalculateGridCenter()
    {
        if (gridConfiguration == null)
        {
            LogWarning("⚠️ [BrickGrid] CalculateGridCenter: No GridData configuration");
            return transform.position;
        }
        
        // Get grid dimensions
        float totalWidth = GetTotalGridWidth();
        float totalHeight = GetTotalGridHeight();
        
        // Calculate center from starting position
        Vector3 startPosition = GetStartingPosition();
        Vector3 center = startPosition + new Vector3(totalWidth * 0.5f, totalHeight * 0.5f, 0f);
        
        return center;
    }
    
    /// <summary>
    /// Validates that the grid fits within the configured play area boundaries.
    /// Checks both position and size constraints with edge margins.
    /// </summary>
    /// <returns>True if grid fits within boundaries, false otherwise</returns>
    public bool ValidateGridBounds()
    {
        if (gridConfiguration == null)
        {
            LogWarning("⚠️ [BrickGrid] ValidateGridBounds: No GridData configuration");
            return false;
        }
        
        // Get grid bounds
        Bounds gridBounds = GetGridBounds();
        Bounds playBounds = gridConfiguration.playAreaBounds;
        
        // Apply edge margins
        Vector3 marginOffset = new(gridConfiguration.edgeMargin, gridConfiguration.edgeMargin, 0f);
        playBounds.size -= marginOffset * 2f;
        
        // Check if grid bounds fit within play area bounds
        bool fitsHorizontally = gridBounds.min.x >= playBounds.min.x && gridBounds.max.x <= playBounds.max.x;
        bool fitsVertically = gridBounds.min.y >= playBounds.min.y && gridBounds.max.y <= playBounds.max.y;
        
        if (!fitsHorizontally)
        {
            LogWarning($"⚠️ [BrickGrid] Grid exceeds horizontal bounds: Grid=[{gridBounds.min.x:F1}, {gridBounds.max.x:F1}], Play=[{playBounds.min.x:F1}, {playBounds.max.x:F1}]");
        }
        
        if (!fitsVertically)
        {
            LogWarning($"⚠️ [BrickGrid] Grid exceeds vertical bounds: Grid=[{gridBounds.min.y:F1}, {gridBounds.max.y:F1}], Play=[{playBounds.min.y:F1}, {playBounds.max.y:F1}]");
        }
        
        return fitsHorizontally && fitsVertically;
    }
    
    /// <summary>
    /// Calculates the total bounds of the generated grid.
    /// Includes all brick positions and accounts for brick size.
    /// </summary>
    /// <returns>Bounds encapsulating entire grid</returns>
    public Bounds GetGridBounds()
    {
        if (gridConfiguration == null)
        {
            LogWarning("⚠️ [BrickGrid] GetGridBounds: No GridData configuration");
            return new Bounds(transform.position, Vector3.one);
        }
        
        // Calculate grid dimensions
        float totalWidth = GetTotalGridWidth();
        float totalHeight = GetTotalGridHeight();
        
        // Account for brick size (standard brick dimensions)
        Vector3 brickSize = new(1.0f, 0.5f, 0.1f); // Standard brick size
        
        // Get starting position and calculate bounds
        Vector3 startPosition = GetStartingPosition();
        Vector3 center = startPosition + new Vector3(totalWidth * 0.5f, totalHeight * 0.5f, 0f);
        Vector3 size = new(totalWidth + brickSize.x, totalHeight + brickSize.y, brickSize.z);
        
        // Account for staggering if enabled
        if (gridConfiguration.enableStaggering)
        {
            size.x += gridConfiguration.horizontalSpacing * gridConfiguration.staggerOffset;
        }
        
        return new Bounds(center, size);
    }
    
    #endregion
    
    #region Private Mathematical Utility Methods
    
    /// <summary>
    /// Calculates the starting position for grid generation.
    /// Handles centering and offset configuration.
    /// </summary>
    /// <returns>World position where grid generation begins (bottom-left corner)</returns>
    private Vector3 GetStartingPosition()
    {
        if (gridConfiguration == null)
        {
            return transform.position;
        }
        
        // Use calculated or configured offset
        Vector3 baseOffset = gridConfiguration.centerInPlayArea ? 
            gridConfiguration.CalculateCenteredOffset() : 
            gridConfiguration.gridOffset;
        
        // Apply transform position as additional offset
        return transform.position + baseOffset;
    }
    
    /// <summary>
    /// Calculates the total width of the grid formation.
    /// </summary>
    /// <returns>Total width in world units</returns>
    private float GetTotalGridWidth()
    {
        if (gridConfiguration == null) return 0f;
        
        // Base width calculation
        float width = (gridConfiguration.columns - 1) * gridConfiguration.horizontalSpacing;
        
        // Add staggering width if enabled
        if (gridConfiguration.enableStaggering)
        {
            width += gridConfiguration.horizontalSpacing * gridConfiguration.staggerOffset;
        }
        
        return width;
    }
    
    /// <summary>
    /// Calculates the total height of the grid formation.
    /// </summary>
    /// <returns>Total height in world units</returns>
    private float GetTotalGridHeight()
    {
        if (gridConfiguration == null) return 0f;
        
        return (gridConfiguration.rows - 1) * gridConfiguration.verticalSpacing;
    }
    
    /// <summary>
    /// Calculates pattern-specific position offset for special layouts.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Additional offset based on layout pattern</returns>
    private Vector3 GetPatternOffset(int row, int column)
    {
        if (gridConfiguration == null) return Vector3.zero;
        
        switch (gridConfiguration.pattern)
        {
            case LayoutPattern.Pyramid:
                return CalculatePyramidOffset(row, column);
                
            case LayoutPattern.Diamond:
                return CalculateDiamondOffset(row, column);
                
            case LayoutPattern.Random:
                // Random pattern doesn't use position offset, uses density instead
                return Vector3.zero;
                
            case LayoutPattern.Custom:
                // Custom patterns would implement their own offset logic
                return Vector3.zero;
                
            case LayoutPattern.Standard:
            default:
                return Vector3.zero;
        }
    }
    
    /// <summary>
    /// Calculates position offset for pyramid pattern layout.
    /// Creates triangular formation with fewer bricks per row going up.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Horizontal offset for pyramid formation</returns>
    private Vector3 CalculatePyramidOffset(int row, int column)
    {
        // Calculate how many bricks should be removed from each side
        float rowReduction = row * 0.5f; // Remove half brick per side per row
        
        // Only offset if column would be visible in pyramid
        if (column < row || column >= gridConfiguration.columns - row)
        {
            // This position should be empty in pyramid pattern
            return new Vector3(0f, -1000f, 0f); // Move far below to hide
        }
        
        // Center the remaining bricks
        float horizontalOffset = rowReduction * gridConfiguration.horizontalSpacing;
        return new Vector3(horizontalOffset, 0f, 0f);
    }
    
    /// <summary>
    /// Calculates position offset for diamond pattern layout.
    /// Creates rhombus formation with expanding then contracting rows.
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Horizontal offset for diamond formation</returns>
    private Vector3 CalculateDiamondOffset(int row, int column)
    {
        int midRow = gridConfiguration.rows / 2;
        float rowDistance = Mathf.Abs(row - midRow);
        float maxDistance = midRow;
        
        // Calculate reduction based on distance from middle
        float reductionFactor = rowDistance / maxDistance;
        int columnsToRemove = Mathf.RoundToInt(reductionFactor * (gridConfiguration.columns / 2));
        
        // Check if this position should be visible
        if (column < columnsToRemove || column >= gridConfiguration.columns - columnsToRemove)
        {
            // This position should be empty in diamond pattern
            return new Vector3(0f, -1000f, 0f); // Move far below to hide
        }
        
        // Center the remaining bricks
        float horizontalOffset = columnsToRemove * gridConfiguration.horizontalSpacing * 0.5f;
        return new Vector3(horizontalOffset, 0f, 0f);
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
    /// Draws grid visualization gizmos in Scene view with comprehensive validation visualization.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showGridGizmos || gridConfiguration == null) return;
        
        // Only show debug visualization if enabled
        if (!enableDebugVisualization) return;
        
        // Calculate current bounds and positions
        Bounds calculatedBounds = GetGridBounds();
        Vector3 calculatedCenter = CalculateGridCenter();
        Vector3 startPos = GetStartingPosition();
        
        // Run validation checks for visualization
        bool configValid = ValidateGridConfiguration();
        bool boundsValid = ValidateGridBounds();
        bool gridValid = gridGenerated ? ValidateGeneratedGrid() : true;
        
        // Choose color based on overall validation status
        Color validationColor = (configValid && boundsValid && gridValid) ? debugSuccessColor : debugErrorColor;
        
        // Draw calculated grid bounds with validation color
        Gizmos.color = validationColor;
        Gizmos.DrawWireCube(calculatedBounds.center, calculatedBounds.size);
        
        // Draw grid center
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(calculatedCenter, 0.3f);
        Gizmos.DrawLine(calculatedCenter + Vector3.up * 0.5f, calculatedCenter - Vector3.up * 0.5f);
        Gizmos.DrawLine(calculatedCenter + Vector3.right * 0.5f, calculatedCenter - Vector3.right * 0.5f);
        
        // Draw play area bounds
        if (gridConfiguration.playAreaBounds.size != Vector3.zero)
        {
            Gizmos.color = debugBoundsColor;
            Gizmos.DrawWireCube(gridConfiguration.playAreaBounds.center, gridConfiguration.playAreaBounds.size);
            
            // Draw edge margins
            Vector3 marginSize = gridConfiguration.playAreaBounds.size - new Vector3(gridConfiguration.edgeMargin * 2f, gridConfiguration.edgeMargin * 2f, 0f);
            Gizmos.color = new Color(debugBoundsColor.r, debugBoundsColor.g, debugBoundsColor.b, 0.3f);
            Gizmos.DrawWireCube(gridConfiguration.playAreaBounds.center, marginSize);
        }
        
        // Draw grid start position
        Gizmos.color = configValid ? debugSuccessColor : debugErrorColor;
        Gizmos.DrawWireSphere(startPos, 0.2f);
        
        // Draw validation status indicators
        Vector3 statusPos = calculatedBounds.center + Vector3.up * (calculatedBounds.size.y * 0.5f + 0.5f);
        
        // Configuration validation indicator
        Gizmos.color = configValid ? debugSuccessColor : debugErrorColor;
        Gizmos.DrawWireCube(statusPos + Vector3.left * 0.8f, new Vector3(0.3f, 0.3f, 0.1f));
        
        // Bounds validation indicator
        Gizmos.color = boundsValid ? debugSuccessColor : debugErrorColor;
        Gizmos.DrawWireCube(statusPos, new Vector3(0.3f, 0.3f, 0.1f));
        
        // Grid generation validation indicator (only if grid is generated)
        if (gridGenerated)
        {
            Gizmos.color = gridValid ? debugSuccessColor : debugErrorColor;
            Gizmos.DrawWireCube(statusPos + Vector3.right * 0.8f, new Vector3(0.3f, 0.3f, 0.1f));
        }
        
        // Draw pattern-specific validation visualization
        DrawPatternValidationGizmos();
    }
    
    /// <summary>
    /// Draws selected gizmos when GameObject is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!showGridGizmos || gridConfiguration == null) return;
        
        // Draw detailed grid layout with calculated positions
        for (int row = 0; row < gridConfiguration.rows; row++)
        {
            for (int col = 0; col < gridConfiguration.columns; col++)
            {
                // Use the actual calculation method
                Vector3 brickPosition = CalculateGridPosition(row, col);
                
                // Skip positions that are hidden (for pattern layouts)
                if (brickPosition.y < -100f) continue;
                
                // Color based on pattern
                switch (gridConfiguration.pattern)
                {
                    case LayoutPattern.Pyramid:
                        Gizmos.color = (col >= row && col < gridConfiguration.columns - row) ? 
                            Color.white : new Color(1f, 1f, 1f, 0.1f);
                        break;
                    case LayoutPattern.Diamond:
                        int midRow = gridConfiguration.rows / 2;
                        float distance = Mathf.Abs(row - midRow) / (float)midRow;
                        Gizmos.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0.3f), distance);
                        break;
                    default:
                        Gizmos.color = Color.white;
                        break;
                }
                
                // Draw brick position
                Gizmos.DrawWireCube(brickPosition, new Vector3(1f, 0.5f, 0.1f));
                
                // Draw row/column labels for corner bricks
                if ((row == 0 && col == 0) || 
                    (row == 0 && col == gridConfiguration.columns - 1) ||
                    (row == gridConfiguration.rows - 1 && col == 0) ||
                    (row == gridConfiguration.rows - 1 && col == gridConfiguration.columns - 1))
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(brickPosition, 0.1f);
                }
            }
        }
        
        // Draw pattern-specific visualization
        if (gridConfiguration.pattern == LayoutPattern.Pyramid || gridConfiguration.pattern == LayoutPattern.Diamond)
        {
            Gizmos.color = new Color(1f, 0f, 1f, 0.5f);
            Vector3 patternCenter = CalculateGridCenter();
            Gizmos.DrawWireSphere(patternCenter, 0.5f);
        }
    }
    
    /// <summary>
    /// Draws pattern-specific validation visualization gizmos.
    /// </summary>
    private void DrawPatternValidationGizmos()
    {
        if (gridConfiguration == null) return;
        
        // Get expected brick count for pattern validation
        int expectedBricks = CalculateExpectedBrickCount();
        int actualBricks = gridGenerated ? instantiatedBricks.Count : 0;
        
        // Draw pattern validation indicator
        bool patternValid = !gridGenerated || (expectedBricks == actualBricks);
        Vector3 patternCenter = CalculateGridCenter();
        
        Gizmos.color = patternValid ? debugSuccessColor : debugErrorColor;
        
        // Draw pattern-specific indicators
        switch (gridConfiguration.pattern)
        {
            case LayoutPattern.Standard:
                // Draw rectangular outline for standard pattern
                Gizmos.DrawWireCube(patternCenter, new Vector3(0.5f, 0.5f, 0.1f));
                break;
                
            case LayoutPattern.Pyramid:
                // Draw triangular indicator for pyramid pattern
                Gizmos.DrawWireSphere(patternCenter + Vector3.up * 0.3f, 0.2f);
                Gizmos.DrawLine(patternCenter + Vector3.up * 0.3f, patternCenter + Vector3.left * 0.3f + Vector3.down * 0.2f);
                Gizmos.DrawLine(patternCenter + Vector3.up * 0.3f, patternCenter + Vector3.right * 0.3f + Vector3.down * 0.2f);
                Gizmos.DrawLine(patternCenter + Vector3.left * 0.3f + Vector3.down * 0.2f, patternCenter + Vector3.right * 0.3f + Vector3.down * 0.2f);
                break;
                
            case LayoutPattern.Diamond:
                // Draw diamond indicator
                Gizmos.DrawLine(patternCenter + Vector3.up * 0.3f, patternCenter + Vector3.right * 0.3f);
                Gizmos.DrawLine(patternCenter + Vector3.right * 0.3f, patternCenter + Vector3.down * 0.3f);
                Gizmos.DrawLine(patternCenter + Vector3.down * 0.3f, patternCenter + Vector3.left * 0.3f);
                Gizmos.DrawLine(patternCenter + Vector3.left * 0.3f, patternCenter + Vector3.up * 0.3f);
                break;
                
            case LayoutPattern.Random:
                // Draw scattered dots for random pattern
                Random.InitState(patternSeed);
                for (int i = 0; i < 8; i++)
                {
                    Vector3 randomOffset = new Vector3(
                        Random.Range(-0.4f, 0.4f),
                        Random.Range(-0.4f, 0.4f),
                        0f
                    );
                    Gizmos.DrawWireSphere(patternCenter + randomOffset, 0.05f);
                }
                break;
        }
        
        // Draw brick count validation if grid is generated
        if (gridGenerated)
        {
            Vector3 countPos = patternCenter + Vector3.down * 0.8f;
            Gizmos.color = patternValid ? debugSuccessColor : debugErrorColor;
            Gizmos.DrawWireCube(countPos, new Vector3(0.2f, 0.2f, 0.1f));
            
            // Draw expected vs actual count indicators
            float countRatio = expectedBricks > 0 ? (float)actualBricks / expectedBricks : 1f;
            Color countColor = Mathf.Approximately(countRatio, 1f) ? debugSuccessColor : debugErrorColor;
            
            Gizmos.color = countColor;
            Gizmos.DrawWireSphere(countPos + Vector3.up * 0.3f, 0.1f * countRatio);
        }
    }
    
    #endregion
}