using UnityEngine;

/// <summary>
/// Defines the available layout patterns for brick grid generation.
/// Each pattern determines the arrangement strategy for placing bricks in the game area.
/// </summary>
[System.Serializable]
public enum LayoutPattern
{
    /// <summary>
    /// Classic brick wall arrangement with uniform rows and columns.
    /// Traditional Breakout pattern with consistent brick placement.
    /// </summary>
    Standard,
    
    /// <summary>
    /// Triangular formation starting wide at the top and narrowing downward.
    /// Creates challenging gameplay with focused destruction patterns.
    /// </summary>
    Pyramid,
    
    /// <summary>
    /// Diamond or rhombus shape with bricks arranged in a diamond pattern.
    /// Symmetric layout providing balanced destruction opportunities.
    /// </summary>
    Diamond,
    
    /// <summary>
    /// Random placement with configurable density.
    /// Procedural generation for varied gameplay experiences.
    /// </summary>
    Random,
    
    /// <summary>
    /// User-defined pattern for custom level designs.
    /// Allows for complex layouts and specialized level configurations.
    /// </summary>
    Custom
}

/// <summary>
/// Configuration data structure defining all parameters needed for brick grid generation.
/// Provides comprehensive control over grid layout, spacing, brick distribution, and boundaries.
/// Used by BrickGrid manager for procedural level generation.
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "GridData", menuName = "Breakout/Grid Configuration", order = 1)]
public class GridData : ScriptableObject
{
    [Header("Grid Dimensions")]
    [Tooltip("Number of rows in the grid. Standard Breakout uses 6-10 rows.")]
    [Range(1, 20)]
    public int rows = 8;
    
    [Tooltip("Number of columns in the grid. Standard Breakout uses 8-14 columns.")]
    [Range(1, 30)]
    public int columns = 10;
    
    [Header("Spacing Configuration")]
    [Tooltip("Horizontal spacing between brick centers. Affects grid width and brick overlap.")]
    [Range(0.1f, 5.0f)]
    public float horizontalSpacing = 1.1f;
    
    [Tooltip("Vertical spacing between brick centers. Affects grid height and row separation.")]
    [Range(0.1f, 5.0f)]
    public float verticalSpacing = 0.6f;
    
    [Header("Layout Pattern")]
    [Tooltip("Pattern type determining brick arrangement strategy.")]
    public LayoutPattern pattern = LayoutPattern.Standard;
    
    [Tooltip("Global offset applied to the entire grid. Used for positioning the grid in the play area.")]
    public Vector3 gridOffset = Vector3.zero;
    
    [Header("Brick Distribution")]
    [Tooltip("Brick types per row. Array size should match row count. Empty array uses Normal bricks.")]
    public BrickType[] rowBrickTypes = new BrickType[0];
    
    [Tooltip("Density factor for random patterns. 1.0 = full density, 0.5 = half density.")]
    [Range(0.1f, 1.0f)]
    public float density = 0.7f;
    
    [Header("Boundary Configuration")]
    [Tooltip("Play area boundaries that constrain grid placement. Used for automatic sizing and centering.")]
    public Bounds playAreaBounds = new Bounds(Vector3.zero, new Vector3(16f, 10f, 1f));
    
    [Tooltip("Automatically center the grid within the play area bounds.")]
    public bool centerInPlayArea = true;
    
    [Header("Advanced Options")]
    [Tooltip("Enable brick staggering for alternating row offsets (classic brick wall pattern).")]
    public bool enableStaggering = false;
    
    [Tooltip("Stagger offset for alternating rows. Only applies when staggering is enabled.")]
    [Range(0f, 1f)]
    public float staggerOffset = 0.5f;
    
    [Tooltip("Minimum distance from play area edges. Prevents bricks from spawning too close to boundaries.")]
    [Range(0f, 3f)]
    public float edgeMargin = 1f;
    
    /// <summary>
    /// Validates the grid configuration and ensures all parameters are within acceptable ranges.
    /// </summary>
    /// <returns>True if configuration is valid, false otherwise</returns>
    public bool ValidateConfiguration()
    {
        // Check basic dimension constraints
        if (rows <= 0 || columns <= 0)
        {
            Debug.LogError($"[GridData] Invalid dimensions: rows={rows}, columns={columns}. Both must be greater than 0.");
            return false;
        }
        
        // Check spacing constraints
        if (horizontalSpacing <= 0f || verticalSpacing <= 0f)
        {
            Debug.LogError($"[GridData] Invalid spacing: horizontal={horizontalSpacing}, vertical={verticalSpacing}. Both must be greater than 0.");
            return false;
        }
        
        // Check density for random patterns
        if (pattern == LayoutPattern.Random && (density <= 0f || density > 1f))
        {
            Debug.LogError($"[GridData] Invalid density for random pattern: {density}. Must be between 0 and 1.");
            return false;
        }
        
        // Check play area bounds
        if (playAreaBounds.size.x <= 0f || playAreaBounds.size.y <= 0f)
        {
            Debug.LogError($"[GridData] Invalid play area bounds: {playAreaBounds.size}. Width and height must be greater than 0.");
            return false;
        }
        
        // Validate row brick types array
        if (rowBrickTypes.Length > 0 && rowBrickTypes.Length != rows)
        {
            Debug.LogWarning($"[GridData] Row brick types array length ({rowBrickTypes.Length}) doesn't match row count ({rows}). Consider adjusting array size.");
        }
        
        return true;
    }
    
    /// <summary>
    /// Calculates the total grid size based on dimensions and spacing.
    /// </summary>
    /// <returns>Vector2 representing the grid's width and height in world units</returns>
    public Vector2 CalculateGridSize()
    {
        float width = (columns - 1) * horizontalSpacing;
        float height = (rows - 1) * verticalSpacing;
        return new Vector2(width, height);
    }
    
    /// <summary>
    /// Determines if the grid fits within the configured play area bounds.
    /// </summary>
    /// <returns>True if grid fits within bounds, false otherwise</returns>
    public bool FitsInPlayArea()
    {
        Vector2 gridSize = CalculateGridSize();
        Vector2 availableSize = new Vector2(
            playAreaBounds.size.x - (2f * edgeMargin),
            playAreaBounds.size.y - (2f * edgeMargin)
        );
        
        return gridSize.x <= availableSize.x && gridSize.y <= availableSize.y;
    }
    
    /// <summary>
    /// Calculates the optimal grid offset to center the grid in the play area.
    /// </summary>
    /// <returns>Vector3 offset for centering the grid</returns>
    public Vector3 CalculateCenteredOffset()
    {
        if (!centerInPlayArea)
            return gridOffset;
        
        Vector2 gridSize = CalculateGridSize();
        Vector3 centeredOffset = new Vector3(
            playAreaBounds.center.x - (gridSize.x * 0.5f),
            playAreaBounds.center.y + (playAreaBounds.size.y * 0.25f), // Position in upper portion
            gridOffset.z
        );
        
        return centeredOffset;
    }
    
    /// <summary>
    /// Gets the appropriate brick type for a specific row.
    /// </summary>
    /// <param name="rowIndex">Zero-based row index</param>
    /// <returns>BrickType for the specified row</returns>
    public BrickType GetBrickTypeForRow(int rowIndex)
    {
        // Use array if available and valid
        if (rowBrickTypes != null && rowBrickTypes.Length > 0)
        {
            if (rowIndex >= 0 && rowIndex < rowBrickTypes.Length)
                return rowBrickTypes[rowIndex];
        }
        
        // Default pattern: top rows are stronger
        if (rowIndex < 2)
            return BrickType.Reinforced;
        else if (rowIndex == rows - 1) // Bottom row
            return BrickType.PowerUp;
        else
            return BrickType.Normal;
    }
    
    /// <summary>
    /// Creates a default GridData configuration suitable for standard Breakout gameplay.
    /// </summary>
    /// <returns>GridData instance with default values</returns>
    public static GridData CreateDefault()
    {
        GridData defaultGrid = CreateInstance<GridData>();
        defaultGrid.name = "DefaultGridData";
        
        // Standard Breakout configuration
        defaultGrid.rows = 8;
        defaultGrid.columns = 10;
        defaultGrid.horizontalSpacing = 1.1f;
        defaultGrid.verticalSpacing = 0.6f;
        defaultGrid.pattern = LayoutPattern.Standard;
        defaultGrid.gridOffset = Vector3.zero;
        defaultGrid.density = 1.0f; // Full density for standard pattern
        defaultGrid.playAreaBounds = new Bounds(Vector3.zero, new Vector3(16f, 10f, 1f));
        defaultGrid.centerInPlayArea = true;
        defaultGrid.enableStaggering = false;
        defaultGrid.staggerOffset = 0.5f;
        defaultGrid.edgeMargin = 1f;
        
        // Setup row brick types for varied gameplay
        defaultGrid.rowBrickTypes = new BrickType[8];
        for (int i = 0; i < 8; i++)
        {
            if (i < 2) defaultGrid.rowBrickTypes[i] = BrickType.Reinforced;
            else if (i == 7) defaultGrid.rowBrickTypes[i] = BrickType.PowerUp;
            else defaultGrid.rowBrickTypes[i] = BrickType.Normal;
        }
        
        return defaultGrid;
    }
    
    /// <summary>
    /// Creates a pyramid pattern GridData configuration.
    /// </summary>
    /// <returns>GridData instance configured for pyramid layout</returns>
    public static GridData CreatePyramid()
    {
        GridData pyramidGrid = CreateDefault();
        pyramidGrid.name = "PyramidGridData";
        pyramidGrid.pattern = LayoutPattern.Pyramid;
        pyramidGrid.rows = 6;
        pyramidGrid.columns = 12;
        pyramidGrid.density = 0.8f;
        
        return pyramidGrid;
    }
    
    /// <summary>
    /// Creates a diamond pattern GridData configuration.
    /// </summary>
    /// <returns>GridData instance configured for diamond layout</returns>
    public static GridData CreateDiamond()
    {
        GridData diamondGrid = CreateDefault();
        diamondGrid.name = "DiamondGridData";
        diamondGrid.pattern = LayoutPattern.Diamond;
        diamondGrid.rows = 7;
        diamondGrid.columns = 9;
        diamondGrid.density = 0.9f;
        
        return diamondGrid;
    }
    
    /// <summary>
    /// Creates a random pattern GridData configuration.
    /// </summary>
    /// <returns>GridData instance configured for random layout</returns>
    public static GridData CreateRandom()
    {
        GridData randomGrid = CreateDefault();
        randomGrid.name = "RandomGridData";
        randomGrid.pattern = LayoutPattern.Random;
        randomGrid.rows = 10;
        randomGrid.columns = 12;
        randomGrid.density = 0.6f;
        
        return randomGrid;
    }
    
    /// <summary>
    /// Provides debug information about the grid configuration.
    /// </summary>
    /// <returns>Formatted string with configuration details</returns>
    public string GetDebugInfo()
    {
        Vector2 gridSize = CalculateGridSize();
        Vector3 centeredOffset = CalculateCenteredOffset();
        bool fitsInBounds = FitsInPlayArea();
        
        return $"GridData Debug Info:\n" +
               $"  Pattern: {pattern}\n" +
               $"  Dimensions: {rows}x{columns}\n" +
               $"  Spacing: {horizontalSpacing}x{verticalSpacing}\n" +
               $"  Grid Size: {gridSize.x:F1}x{gridSize.y:F1}\n" +
               $"  Offset: {centeredOffset}\n" +
               $"  Density: {density:F1}\n" +
               $"  Fits in Bounds: {fitsInBounds}\n" +
               $"  Play Area: {playAreaBounds.size}\n" +
               $"  Edge Margin: {edgeMargin}\n" +
               $"  Row Types: {(rowBrickTypes?.Length ?? 0)} defined";
    }
}