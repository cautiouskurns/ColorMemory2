# BrickGrid Manager Testing Guide

## Understanding the Initialization Errors

The errors you encountered:
```
❌ [BrickGrid] Cannot validate generation - manager not initialized
```

These occur because:

1. **Editor vs Play Mode**: The test scripts run in Editor mode where Unity's lifecycle methods (Awake/Start) are not called automatically
2. **Initialization Required**: BrickGrid requires initialization through Awake() to set up internal state, validate configuration, and create the grid container
3. **Validation Checks**: Methods like `ValidateGridGeneration()` and `GenerateGrid()` check `IsInitialized` to ensure proper setup

## Testing Approaches

### 1. Editor Mode Testing (Limited)

Use the updated test scripts that check for Play mode:
```
Breakout/Debug/Test BrickGrid Manager
```

Editor mode tests can verify:
- Component existence
- Configuration validation
- Debug info generation
- Basic state management

### 2. Play Mode Testing (Full)

Enter Play mode to test all functionality:

1. **Create BrickGrid Manager**: `Breakout/Setup/Task1222 Create BrickGrid Manager`
2. **Assign GridData**: In Inspector, assign a GridData asset to Grid Configuration
3. **Enter Play Mode**: Press Play button in Unity
4. **Use Inspector Testing**: The custom Inspector provides runtime test buttons

### 3. Custom Inspector Testing

The `BrickGridRuntimeTester` provides an enhanced Inspector with:

- **Initialization Status**: Shows if BrickGrid is properly initialized
- **Framework Method Buttons**: Test GenerateGrid(), ClearGrid(), ValidateGrid()
- **State Management Buttons**: Test state updates and brick counting
- **Debug Information**: Show comprehensive debug info
- **Live State Display**: Shows current grid state in real-time

## Testing Workflow

### Step 1: Setup
```
1. Create BrickGrid Manager (if not exists)
2. Assign GridData asset in Inspector
3. Enable debug logging and grid gizmos
```

### Step 2: Editor Testing
```
1. Run "Test BrickGrid Manager" menu item
2. Verify component setup and configuration
3. Check debug info generation
```

### Step 3: Play Mode Testing
```
1. Enter Play mode
2. Check console for initialization logs
3. Use Inspector buttons to test methods
4. Observe Scene view gizmos
```

## Expected Behavior

### In Editor Mode
- `IsInitialized`: false
- Framework methods: Skip validation or show warning
- State management: Basic operations work
- Debug info: Available but limited

### In Play Mode
- `IsInitialized`: true (after Awake)
- Framework methods: Execute with proper validation
- State management: Full functionality
- Debug info: Complete with all parameters

## Common Issues and Solutions

### Issue: "Cannot validate generation - manager not initialized"
**Solution**: This is expected in Editor mode. Test in Play mode for full functionality.

### Issue: "No GridData configuration assigned"
**Solution**: Assign a GridData asset to the Grid Configuration field in Inspector.

### Issue: Framework methods don't do anything
**Solution**: These are stubs prepared for future implementation. They log their execution but don't generate actual bricks yet.

## Debug Commands

### Console Testing
```csharp
// In Play mode console or script:
BrickGrid brickGrid = FindObjectOfType<BrickGrid>();

// Check state
Debug.Log(brickGrid.GetGridDebugInfo());

// Test operations
brickGrid.ValidateConfiguration();
brickGrid.UpdateBrickCount(10);
brickGrid.SetGridGenerated(10);
```

### Scene Visualization
Enable "Show Grid Gizmos" to see:
- Yellow: Grid bounds
- Cyan: Play area bounds
- Green: Grid start position
- White: Individual brick positions (when selected)

## Integration Testing

When integrating with other systems:

1. **GameManager Integration**:
   ```csharp
   if (brickGrid.IsInitialized && brickGrid.IsComplete)
   {
       // Handle level completion
   }
   ```

2. **Level Loading**:
   ```csharp
   // Ensure BrickGrid is initialized before operations
   if (brickGrid.IsInitialized)
   {
       brickGrid.ClearGrid();
       // Assign new GridData
       brickGrid.GenerateGrid();
   }
   ```

3. **State Monitoring**:
   ```csharp
   // Safe state access
   int brickCount = brickGrid.IsInitialized ? brickGrid.ActiveBrickCount : 0;
   ```

## Summary

The BrickGrid Manager is designed with proper initialization checks to ensure safe operation. The "not initialized" errors are expected in Editor mode and demonstrate the system is working correctly by preventing operations before proper setup. Use Play mode for full testing of all framework features.