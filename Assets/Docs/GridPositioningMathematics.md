# Grid Positioning Mathematics

## Overview

The Grid Positioning Mathematics system provides precise mathematical calculations for brick positioning in the Breakout game. It handles complex layout patterns, boundary validation, and ensures accurate placement within play area constraints.

## Core Mathematical Methods

### Position Calculation

#### CalculateGridPosition(int row, int column)

**Purpose**: Calculates the exact world position for a brick at grid coordinates

**Formula**:
```csharp
// Base position calculation
float xPosition = startPosition.x + (column * horizontalSpacing);
float yPosition = startPosition.y + (row * verticalSpacing);

// Staggering adjustment (for brick wall patterns)
if (enableStaggering && row % 2 == 1) {
    xPosition += horizontalSpacing * staggerOffset;
}

// Pattern-specific offsets
Vector3 finalPosition = basePosition + GetPatternOffset(row, column);
```

**Features**:
- Zero-based indexing (row 0 = bottom, column 0 = left)
- Configurable horizontal and vertical spacing
- Staggering support for alternating row offsets
- Pattern-specific positioning for Pyramid/Diamond layouts
- Input validation and error handling

### Grid Centering

#### CalculateGridCenter()

**Purpose**: Determines the center point of the entire grid formation

**Formula**:
```csharp
Vector3 center = startPosition + new Vector3(
    totalWidth * 0.5f, 
    totalHeight * 0.5f, 
    0f
);
```

**Applications**:
- Camera positioning and focus
- Visual effects center points
- UI overlay positioning
- Grid-relative calculations

### Boundary Validation

#### ValidateGridBounds()

**Purpose**: Ensures grid fits within play area with safety margins

**Algorithm**:
```csharp
// Calculate effective play area (with margins)
Vector3 marginOffset = new Vector3(edgeMargin, edgeMargin, 0f);
Bounds effectivePlayArea = playAreaBounds;
effectivePlayArea.size -= marginOffset * 2f;

// Check grid bounds fit within effective area
bool fitsHorizontally = gridBounds.min.x >= effectivePlayArea.min.x && 
                       gridBounds.max.x <= effectivePlayArea.max.x;
bool fitsVertically = gridBounds.min.y >= effectivePlayArea.min.y && 
                     gridBounds.max.y <= effectivePlayArea.max.y;
```

### Bounds Calculation

#### GetGridBounds()

**Purpose**: Calculates complete bounds encompassing all brick positions

**Considerations**:
- Includes standard brick size (1.0 × 0.5 × 0.1)
- Accounts for staggering extensions
- Handles pattern-specific dimensions
- Provides center and size for Unity Bounds

## Pattern-Specific Mathematics

### Standard Pattern
- Simple grid layout with uniform spacing
- No position offsets or hiding
- Direct row/column to world position mapping

### Pyramid Pattern

**Algorithm**:
```csharp
// Calculate visible columns for each row
int visibleColumns = totalColumns - (row * 2);

// Hide positions outside pyramid
if (column < row || column >= totalColumns - row) {
    return hidePosition; // Move far below grid
}

// Center remaining bricks
float horizontalOffset = row * 0.5f * horizontalSpacing;
return basePosition + new Vector3(horizontalOffset, 0f, 0f);
```

**Result**: Triangular formation with fewer bricks per row going upward

### Diamond Pattern

**Algorithm**:
```csharp
int midRow = totalRows / 2;
float rowDistance = Mathf.Abs(row - midRow);
float reductionFactor = rowDistance / midRow;
int columnsToRemove = Mathf.RoundToInt(reductionFactor * (totalColumns / 2));

// Hide edge positions
if (column < columnsToRemove || column >= totalColumns - columnsToRemove) {
    return hidePosition;
}

// Center remaining bricks
float horizontalOffset = columnsToRemove * horizontalSpacing * 0.5f;
```

**Result**: Rhombus formation expanding from center row

## Utility Methods

### Private Mathematical Helpers

#### GetStartingPosition()
- Combines transform position with grid offset
- Handles centering calculations
- Returns bottom-left corner position

#### GetTotalGridWidth()
```csharp
float width = (columns - 1) * horizontalSpacing;
if (enableStaggering) {
    width += horizontalSpacing * staggerOffset;
}
```

#### GetTotalGridHeight()
```csharp
float height = (rows - 1) * verticalSpacing;
```

## Coordinate System

### Grid Coordinates
- **Origin**: Bottom-left corner (0, 0)
- **Row 0**: Bottom row of bricks
- **Column 0**: Leftmost column
- **Positive Y**: Upward (higher rows)
- **Positive X**: Rightward (higher columns)

### World Coordinates
- Unity world space positions
- Consistent with Unity's coordinate system
- Accounts for BrickGrid transform position
- Applies configured offsets and centering

## Precision and Validation

### Mathematical Precision
- Floating-point calculations with Unity's precision
- Error tolerance: < 0.001 units for spacing verification
- Consistent positioning across all grid sizes
- Handles edge cases and boundary conditions

### Input Validation
- Row/column bounds checking
- Configuration null checking
- Graceful handling of invalid inputs
- Warning messages for out-of-bounds access

### Boundary Constraints
- Play area bounds enforcement
- Edge margin calculations
- Overflow detection and warnings
- Safe positioning within game boundaries

## Debug Visualization

### Scene View Gizmos

**Color Coding**:
- **Yellow**: Calculated grid bounds
- **Magenta**: Grid center with crosshairs
- **Cyan**: Play area bounds
- **Semi-transparent Cyan**: Effective area (with margins)
- **Green**: Grid starting position
- **Red/Green**: Boundary validation status
- **White**: Individual brick positions (when selected)

**Pattern Visualization**:
- **Pyramid**: Triangular outline with hidden positions
- **Diamond**: Rhombus shape with center highlighting
- **Corner Markers**: Yellow spheres at grid corners

### Testing and Verification

#### Automated Tests
```csharp
// Spacing verification
float expectedWidth = (columns - 1) * horizontalSpacing;
float actualWidth = topRight.x - topLeft.x;
float error = Mathf.Abs(expectedWidth - actualWidth);
bool isAccurate = error < 0.001f; // Precision threshold
```

#### Manual Testing Commands
- `Breakout/Debug/Test Grid Position Math`: Random position testing
- `Breakout/Setup/Task1223 Create Grid Positioning Math`: Complete validation suite

## Usage Examples

### Basic Position Calculation
```csharp
BrickGrid grid = FindObjectOfType<BrickGrid>();
Vector3 position = grid.CalculateGridPosition(2, 3); // Row 2, Column 3
```

### Boundary Validation
```csharp
if (grid.ValidateGridBounds()) {
    // Safe to generate grid
    grid.GenerateGrid();
} else {
    // Adjust configuration or warn user
    Debug.LogWarning("Grid too large for play area");
}
```

### Center-based Operations
```csharp
Vector3 center = grid.CalculateGridCenter();
Camera.main.transform.LookAt(center);
```

### Pattern Testing
```csharp
// Test all positions in pyramid pattern
for (int row = 0; row < grid.GridConfiguration.rows; row++) {
    for (int col = 0; col < grid.GridConfiguration.columns; col++) {
        Vector3 pos = grid.CalculateGridPosition(row, col);
        bool isVisible = pos.y > -100f; // Check if position is hidden
    }
}
```

## Integration Points

### BrickGrid Manager
- Core positioning system for grid generation
- State management integration
- Configuration validation support

### GridData System
- Spacing parameter consumption
- Pattern type switching
- Boundary constraint enforcement

### Future Grid Generation
- Foundation for brick instantiation
- Position accuracy for collision detection
- Performance optimization for large grids

## Performance Considerations

### Computational Efficiency
- O(1) position calculations
- Cached dimension calculations
- Minimal garbage allocation
- Optimized mathematical operations

### Memory Usage
- No runtime position storage
- On-demand calculation approach
- Efficient bounds checking
- Minimal allocation during calculations

## Error Handling

### Common Issues
- **Null Configuration**: Returns safe default positions
- **Invalid Coordinates**: Logs warnings and returns Vector3.zero
- **Boundary Violations**: Clear diagnostic messages
- **Pattern Edge Cases**: Graceful degradation

### Diagnostic Output
```
⚠️ [BrickGrid] Invalid grid coordinates: row=10, column=5
⚠️ [BrickGrid] Grid exceeds horizontal bounds: Grid=[-2.0, 14.0], Play=[-8.0, 8.0]
```

The Grid Positioning Mathematics system provides a robust, precise foundation for all brick positioning needs in the Breakout game, with comprehensive testing, validation, and visualization support.