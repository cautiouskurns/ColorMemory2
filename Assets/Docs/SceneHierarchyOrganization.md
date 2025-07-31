# Scene Hierarchy Organization

## Overview

The Scene Hierarchy Organization system provides clean, structured GameObject management for the BrickGrid system. It creates organized parent-child relationships with consistent naming conventions, enabling easy scene navigation, debugging, and efficient cleanup during grid regeneration.

## Core Features

### Hierarchical Structure

```
BrickGrid_Manager
└── BrickGrid_Organized (Main Container)
    ├── Row_00 (Row Container)
    │   ├── Brick_R00C00 (Normal)
    │   ├── Brick_R00C01 (Normal)
    │   └── Brick_R00C02 (Reinforced)
    ├── Row_01 (Row Container)
    │   ├── Brick_R01C00 (Normal)
    │   └── Brick_R01C01 (PowerUp)
    └── Row_02 (Row Container)
        └── Brick_R02C00 (Indestructible)
```

### Container Management

#### CreateGridContainer()
**Purpose**: Creates the main parent container for the entire grid

**Features**:
- Automatic creation with configurable naming
- Proper parenting under BrickGrid_Manager
- Reuses existing container if available
- Proper Transform initialization (position, rotation, scale)

**Usage**:
```csharp
GameObject gridContainer = brickGrid.CreateGridContainer();
// Returns existing or creates new "BrickGrid_Organized" container
```

#### CreateRowContainer(int rowIndex)
**Purpose**: Creates row-specific containers for organized brick grouping

**Features**:
- Consistent naming with zero-padded indices (Row_00, Row_01, etc.)
- Automatic parenting under grid container
- Configurable naming prefix via Inspector
- Supports disabling row organization for flat structure

**Usage**:
```csharp
GameObject rowContainer = brickGrid.CreateRowContainer(2);
// Creates "Row_02" container under grid container
```

## Brick Organization System

### Automatic Organization

The system automatically organizes bricks during instantiation:

```csharp
// Hierarchy-aware instantiation
GameObject brick = brickGrid.InstantiateBrick(position, BrickType.Normal, row, column);
// Automatically places brick in appropriate row container with proper naming
```

### Manual Organization

For reorganizing existing bricks:

```csharp
brickGrid.OrganizeBricksInHierarchy();
// Moves all instantiated bricks to appropriate row containers
// Creates missing row containers as needed
// Validates hierarchy integrity
```

### Organization Modes

**Row Organization Enabled** (Default):
- Bricks organized under row containers
- Clear separation by grid rows
- Easy inspection and debugging
- Scalable for large grids

**Row Organization Disabled**:
- All bricks directly under grid container
- Flatter hierarchy structure
- Better for small grids or simple layouts

## Naming Conventions

### Container Naming

- **Grid Container**: Configurable name (default: "BrickGrid_Organized")
- **Row Containers**: Prefix + zero-padded index (default: "Row_00", "Row_01")
- **Consistent Formatting**: Maintains alphabetical sorting in hierarchy

### Brick Naming

- **Format**: `Brick_R##C##` (e.g., "Brick_R02C05")
- **Row Index**: Zero-padded 2-digit row number
- **Column Index**: Zero-padded 2-digit column number
- **Fallback**: Type-based naming for bricks without position info

## Hierarchy Cleanup System

### ClearHierarchy()

**Purpose**: Efficient cleanup of entire hierarchy structure

**Process**:
1. Destroys all row containers and their children
2. Cleans up direct children of grid container
3. Clears internal tracking collections
4. Provides detailed cleanup statistics
5. Handles Editor vs. Runtime destruction properly

**Performance Benefits**:
- Batch destruction of entire containers
- Avoids individual brick cleanup overhead
- Prevents orphaned GameObject references
- Maintains clean scene state

### Cleanup Integration

The hierarchy cleanup is integrated with the existing grid management:

```csharp
brickGrid.ClearGrid();
// Now uses ClearHierarchy() for efficient cleanup
// Maintains same public interface
// Enhanced performance and reliability
```

## Inspector Configuration

### Hierarchy Organization Settings

```csharp
[Header("Hierarchy Organization")]
[SerializeField] private string gridContainerName = "BrickGrid";
[SerializeField] private string rowContainerPrefix = "Row_";
[SerializeField] private bool useRowOrganization = true;
```

**Grid Container Name**: Customizable main container name
**Row Container Prefix**: Prefix for row container naming
**Use Row Organization**: Toggle between row-based and flat organization

### Runtime Configuration

All settings can be modified at runtime:

```csharp
// Change naming conventions
brickGrid.gridContainerName = "CustomGridName";
brickGrid.rowContainerPrefix = "Level_";

// Toggle organization mode
brickGrid.useRowOrganization = false;
brickGrid.OrganizeBricksInHierarchy(); // Apply changes
```

## Benefits for Development

### Scene Navigation

- **Clear Structure**: Easy to locate specific rows and bricks
- **Hierarchical Folding**: Collapse/expand rows for better visibility
- **Search-Friendly**: Consistent naming supports quick GameObject finding
- **Inspector Integration**: Properties organized with Header attributes

### Debugging Support

- **Visual Organization**: Grid structure visible in hierarchy
- **Row Isolation**: Easy to disable/enable specific rows
- **Brick Identification**: Names indicate exact grid position
- **Validation Tools**: Built-in integrity checking

### Performance Optimization

- **Batch Operations**: Operate on entire row containers efficiently
- **Memory Management**: Proper cleanup prevents memory leaks
- **Scene Complexity**: Organized structure reduces visual clutter
- **Scalability**: Maintains organization with 100+ bricks

## Validation and Integrity

### ValidateHierarchyIntegrity()

**Purpose**: Ensures hierarchy maintains proper organization

**Validation Checks**:
- Grid container existence and parenting
- Row container count and organization
- Brick distribution across containers
- Orphaned GameObject detection
- Naming convention compliance

**Error Reporting**:
```
Hierarchy validation: 64 total bricks, 8 containers, 0 orphaned bricks
⚠️ [BrickGrid] Found 2 orphaned bricks not properly organized
```

### Automatic Correction

The system automatically corrects common issues:
- Creates missing containers during organization
- Moves orphaned bricks to appropriate containers
- Applies proper naming to unnamed objects
- Reports and logs correction actions

## Integration with Existing Systems

### Brick Instantiation

- Seamless integration with existing InstantiateBrick() methods
- Backward compatibility with position-only instantiation
- Enhanced overloads support row/column organization
- Automatic parent assignment based on organization settings

### Grid Generation

- GenerateGridBricks() uses hierarchy-aware instantiation
- Row containers created during generation process
- Performance optimizations maintain generation speed
- Clean integration with pattern-based layouts

### State Management

- Hierarchy state tracked alongside grid state
- Container references maintained in serialized arrays
- Cleanup integrated with existing state reset
- Validation runs automatically during operations

## Usage Examples

### Basic Setup

```csharp
// Enable row organization
brickGrid.useRowOrganization = true;

// Generate organized grid
brickGrid.GenerateGrid();
// Result: Clean hierarchy with row containers
```

### Custom Organization

```csharp
// Configure custom naming
brickGrid.gridContainerName = "Level1_Bricks";
brickGrid.rowContainerPrefix = "Tier_";

// Create organized structure
brickGrid.CreateGridContainer();
for (int i = 0; i < 5; i++)
{
    brickGrid.CreateRowContainer(i);
}
```

### Manual Brick Placement

```csharp
// Place brick with hierarchy organization
Vector3 position = new Vector3(2f, 3f, 0f);
GameObject brick = brickGrid.InstantiateBrick(position, BrickType.Reinforced, 1, 2);
// Creates "Brick_R01C02" in "Row_01" container
```

### Reorganization

```csharp
// Reorganize existing messy hierarchy
brickGrid.OrganizeBricksInHierarchy();
// Moves all bricks to proper containers
// Creates missing row containers
// Validates final organization
```

### Cleanup Testing

```csharp
// Test cleanup efficiency
System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
brickGrid.ClearHierarchy();
timer.Stop();
Debug.Log($"Cleanup time: {timer.ElapsedMilliseconds}ms");
```

## Best Practices

### Container Management

1. **Consistent Naming**: Use descriptive, sortable names
2. **Proper Parenting**: Maintain clear parent-child relationships
3. **Container Reuse**: Check for existing containers before creating
4. **Transform Initialization**: Always reset local transforms

### Organization Strategy

1. **Row-Based Default**: Use row organization for most cases
2. **Flat for Small Grids**: Consider flat organization for < 20 bricks
3. **Custom Patterns**: Adapt organization to game-specific patterns
4. **Runtime Flexibility**: Support organization changes during gameplay

### Performance Considerations

1. **Batch Operations**: Operate on containers rather than individual bricks
2. **Cleanup Efficiency**: Use hierarchy cleanup for best performance
3. **Validation Frequency**: Balance integrity checking with performance
4. **Container Caching**: Cache container references to avoid searches

### Integration Guidelines

1. **Backward Compatibility**: Maintain existing API while adding features
2. **Optional Organization**: Make hierarchy organization configurable
3. **Error Handling**: Gracefully handle missing or corrupted hierarchy
4. **Logging Clarity**: Provide clear feedback about organization operations

## Troubleshooting

### Common Issues

**Orphaned Bricks**: Bricks not properly organized under containers
- **Solution**: Run `OrganizeBricksInHierarchy()` to reorganize
- **Prevention**: Use hierarchy-aware instantiation methods

**Missing Containers**: Row containers not created during generation
- **Solution**: Check `useRowOrganization` setting and grid configuration
- **Debug**: Verify row count and container creation logic

**Naming Conflicts**: Duplicate or malformed GameObject names
- **Solution**: Use `GetBrickName()` method for consistent naming
- **Prevention**: Always specify row/column when possible

**Cleanup Issues**: Objects remaining after ClearHierarchy()
- **Solution**: Check for external references holding GameObjects
- **Debug**: Validate hierarchy integrity before and after cleanup

### Performance Issues

**Slow Organization**: OrganizeBricksInHierarchy() taking too long
- **Solution**: Reduce validation frequency, batch operations
- **Optimization**: Cache container references, minimize searches

**Memory Leaks**: GameObjects not properly destroyed
- **Solution**: Ensure ClearHierarchy() is called before regeneration
- **Prevention**: Use proper destruction methods (Destroy vs. DestroyImmediate)

The Scene Hierarchy Organization system provides a robust foundation for managing complex brick grid structures while maintaining clean, navigable scene hierarchies that scale efficiently with grid size and complexity.