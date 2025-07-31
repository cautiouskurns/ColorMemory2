# Grid Configuration Data Structures

## Overview

The Grid Configuration Data Structures provide a comprehensive system for defining and managing brick grid layouts in the Breakout game. This system enables configurable level designs with various layout patterns and supports procedural generation.

## Core Components

### LayoutPattern Enum

Defines the available arrangement strategies for brick placement:

- **Standard**: Classic brick wall with uniform rows and columns
- **Pyramid**: Triangular formation narrowing downward
- **Diamond**: Symmetric diamond/rhombus arrangement
- **Random**: Procedural placement with configurable density
- **Custom**: User-defined patterns for specialized layouts

### GridData Class

ScriptableObject-based configuration system containing:

#### Grid Dimensions
- `rows`: Number of rows (1-20 range)
- `columns`: Number of columns (1-30 range)
- `horizontalSpacing`: Distance between brick centers horizontally
- `verticalSpacing`: Distance between brick centers vertically

#### Layout Configuration
- `pattern`: Selected LayoutPattern type
- `gridOffset`: Global position offset for the entire grid
- `density`: Fill density for random patterns (0.1-1.0)

#### Brick Distribution
- `rowBrickTypes[]`: Per-row brick type configuration
- `GetBrickTypeForRow()`: Intelligent brick type selection

#### Boundary Management
- `playAreaBounds`: Constraints for grid placement
- `centerInPlayArea`: Automatic centering option
- `edgeMargin`: Minimum distance from boundaries

#### Advanced Options
- `enableStaggering`: Alternating row offsets (brick wall pattern)
- `staggerOffset`: Offset amount for staggered rows

## Key Methods

### Validation
- `ValidateConfiguration()`: Comprehensive data integrity checks
- `FitsInPlayArea()`: Boundary constraint validation

### Calculations
- `CalculateGridSize()`: Dynamic grid size computation
- `CalculateCenteredOffset()`: Automatic positioning calculation

### Factory Methods
- `CreateDefault()`: Standard Breakout configuration
- `CreatePyramid()`: Triangular formation preset
- `CreateDiamond()`: Diamond pattern preset
- `CreateRandom()`: Random layout preset

## Usage Examples

### Creating Custom GridData

```csharp
// Create via ScriptableObject menu
// Create > Breakout > Grid Configuration

// Or programmatically
GridData customGrid = GridData.CreateDefault();
customGrid.rows = 6;
customGrid.columns = 12;
customGrid.pattern = LayoutPattern.Pyramid;
customGrid.density = 0.7f;
```

### Integration with BrickGrid Manager

```csharp
public class BrickGridManager : MonoBehaviour
{
    [SerializeField] private GridData gridConfiguration;
    
    void Start()
    {
        if (gridConfiguration.ValidateConfiguration())
        {
            GenerateGrid(gridConfiguration);
        }
    }
    
    void GenerateGrid(GridData config)
    {
        Vector3 startPos = config.CalculateCenteredOffset();
        Vector2 gridSize = config.CalculateGridSize();
        
        for (int row = 0; row < config.rows; row++)
        {
            BrickType brickType = config.GetBrickTypeForRow(row);
            // Generate row with specified brick type
        }
    }
}
```

### Inspector Configuration

GridData assets provide organized Inspector sections:

1. **Grid Dimensions**: Rows, columns with validation
2. **Spacing Configuration**: Horizontal/vertical spacing controls
3. **Layout Pattern**: Pattern selection dropdown
4. **Brick Distribution**: Row-based type arrays
5. **Boundary Configuration**: Play area bounds and centering
6. **Advanced Options**: Staggering and margin controls

## Sample Configurations

### Standard Breakout (8×10)
```
Rows: 8, Columns: 10
Spacing: 1.1×0.6
Pattern: Standard
Density: 1.0 (full)
```

### Pyramid Challenge (6×12)
```
Rows: 6, Columns: 12
Spacing: 1.1×0.6
Pattern: Pyramid
Density: 0.8
```

### Diamond Formation (7×9)
```
Rows: 7, Columns: 9
Spacing: 1.1×0.6
Pattern: Diamond
Density: 0.9
```

### Random Layout (10×12)
```
Rows: 10, Columns: 12
Spacing: 1.0×0.5
Pattern: Random
Density: 0.6
```

## Integration Points

### BrickData System
- Full compatibility with `BrickType` enum
- Row-based brick type distribution
- Consistent with existing brick component system

### Level Design Workflow
- ScriptableObject assets for different levels
- Inspector-based configuration
- Runtime validation and debugging

### Grid Generation System
- Foundation for BrickGrid Manager implementation
- Helper methods for positioning and sizing
- Boundary constraint validation

## Next Steps

1. **Implement BrickGrid Manager** using these data structures
2. **Create Level Progression System** with different GridData configurations
3. **Test Pattern Generation** with all layout types
4. **Integrate with Brick Prefab System** from Task 1.2.1.7

## Error Handling

The system includes comprehensive validation:
- Dimension constraints (positive values)
- Spacing validation (non-zero)
- Density range checking (0.1-1.0)
- Play area bounds validation
- Row brick type array consistency checking

All validation provides clear error messages and actionable feedback for configuration issues.