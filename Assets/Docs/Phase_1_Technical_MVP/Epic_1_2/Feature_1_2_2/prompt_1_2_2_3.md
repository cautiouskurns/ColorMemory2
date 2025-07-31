# **Unity C# Implementation Task: Grid Positioning Mathematics** *(50 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.2.2.3
**Category:** System
**Tags:** Mathematics, Positioning, Grid System, Algorithms
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** Mathematical positioning calculation system within BrickGrid manager
**Game Context:** Breakout-style game requiring precise brick positioning in grid formations

**Purpose:** Implements accurate mathematical calculations for brick positioning, ensures proper grid centering within game boundaries, and provides the foundation for reliable brick placement in any grid configuration.
**Complexity:** Medium complexity - 50 minutes (mathematical algorithms with boundary validation)

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// Add these methods to existing BrickGrid class
public class BrickGrid : MonoBehaviour
{
    // Mathematical positioning methods
    public Vector3 CalculateGridPosition(int row, int column)
    {
        // Calculate world position for brick at grid coordinates
    }
    
    public Vector3 CalculateGridCenter()
    {
        // Calculate center point of entire grid formation
    }
    
    public bool ValidateGridBounds()
    {
        // Validate grid fits within play area boundaries
    }
    
    public Bounds GetGridBounds()
    {
        // Calculate total bounds of generated grid
    }
    
    // Private utility methods
    private Vector3 GetStartingPosition()
    private float GetTotalGridWidth()
    private float GetTotalGridHeight()
}
```

### **Core Logic:**

- Grid position calculation using row/column indices with configurable spacing
- Centering algorithm that positions entire grid formation within game boundaries  
- Spacing calculation system with horizontal and vertical gap configuration
- Boundary validation ensuring grid fits within playable area constraints
- Mathematical precision maintaining proper alignment across all grid sizes

### **Dependencies:**

- BrickGrid manager foundation from Task 1.2.2.2
- GridData configuration for spacing and dimension parameters
- Unity Vector3 and Bounds for position calculations

### **Performance Constraints:**

- Optimized calculations with minimal computational overhead
- No frame-by-frame recalculation - compute once and cache results
- Efficient bounds checking without excessive math operations

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - focus purely on mathematical calculations
- Keep methods focused on specific calculation types
- Use private utility methods for complex calculation breakdowns
- Avoid side effects - pure calculation methods that don't modify state

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** No direct GameObject creation - mathematical methods only
**Scene Hierarchy:** N/A for this task
**Inspector Config:** No additional Inspector fields - uses existing GridData configuration
**System Connections:** Provides positioning foundation for brick instantiation in Task 1.2.2.4

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering mathematical approach and calculation strategy)
2. **Code Files** (updated BrickGrid.cs with positioning methods added)
3. **Editor Setup Script** (adds test visualization for grid bounds and positioning)
4. **Integration Notes** (explanation of calculation precision and boundary validation)

**File Structure:** `Assets/Scripts/Grid/BrickGrid.cs` - add methods to existing class
**Code Standards:** Unity C# conventions, XML documentation for calculation methods, clear variable naming

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/Task1223CreateGridPositioningMathSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Grid Positioning Math"`

**Class Pattern:** `CreateGridPositioningMathSetup` (static class)

**Core Functionality:**

- Validate BrickGrid manager exists in scene (call Task 1.2.2.2 setup if not)
- Add debug visualization for grid bounds and center point
- Create test GridData configuration for mathematical validation
- Set up Gizmos visualization for position calculation verification
- Test all positioning methods with sample configurations

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateGridPositioningMathSetup
{
    [MenuItem("Breakout/Setup/Create Grid Positioning Math")]
    public static void CreateGridPositioningMath()
    {
        // Validate BrickGrid exists and call prerequisite if needed
        // Set up test configuration for mathematical validation
        // Enable debug visualization for positioning verification
        // Test calculation methods with sample data
        Debug.Log("✅ Grid Positioning Mathematics implemented successfully");
    }

    [MenuItem("Breakout/Setup/Create Grid Positioning Math", true)]
    public static bool ValidateCreateGridPositioningMath()
    {
        // Return false if positioning methods already implemented
        var grid = Object.FindObjectOfType<BrickGrid>();
        return grid != null; // Must have BrickGrid to add math methods
    }
}
#endif
```

**Error Handling Requirements:**

- Log calculation test results with position accuracy verification
- Handle invalid grid configurations gracefully with clear error messages
- Validate boundary calculations and warn about out-of-bounds configurations
- Provide mathematical precision verification in debug output

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate console output showing mathematical calculation test results
- Provide verification that positioning algorithms work correctly for different grid sizes
- List boundary validation results and any constraint warnings

### **Documentation:**

- Create brief .md file capturing:
    - Mathematical approach and calculation formulas
    - Boundary validation logic and play area constraints
    - Usage examples for different grid configurations
    - Debug visualization explanation and usage

### **Custom Instructions:**

- Include Gizmos visualization for grid bounds and center point calculation
- Add mathematical precision testing with known configurations
- Provide clear debugging output for position calculation verification

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Position calculations produce accurate brick coordinates for any grid configuration
- [ ] Centering algorithm properly positions grid formations within game boundaries
- [ ] Spacing calculations create consistent gaps between bricks
- [ ] Boundary validation prevents grid generation outside playable area
- [ ] Mathematical precision maintains proper alignment across all grid sizes

### **Integration Tests:**

- [ ] Mathematical methods integrate with existing BrickGrid manager framework
- [ ] GridData configuration properly drives all calculation parameters
- [ ] Boundary validation works correctly with different play area sizes

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] Methods are focused on specific calculation responsibilities
- [ ] All public methods have XML documentation with parameter explanations
- [ ] Mathematical precision verified through testing

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** False - Requires BrickGrid foundation from Task 1.2.2.2
**ValidationLevel:** Strict - Include comprehensive boundary validation and mathematical precision checking
**Reusability:** Reusable - Design calculations to work with any grid configuration

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache calculated values to avoid repeated computation
- Use Vector3 and Bounds for consistent Unity coordinate system integration
- Apply mathematical precision appropriate for Unity's floating-point system
- Include comprehensive parameter validation for edge cases
- Use clear variable names that explain mathematical purpose

### **Performance Requirements:**

- Efficient calculation algorithms with minimal computational overhead
- No runtime allocation during position calculation operations
- Optimized boundary checking without excessive mathematical operations

### **Architecture Pattern:**

Utility methods pattern with mathematical calculation focus and pure function design

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If BrickGrid manager missing:** Log error and provide setup instructions for Task 1.2.2.2
- **If GridData unavailable:** Create minimal placeholder for compilation
- **If play area bounds undefined:** Use sensible default boundary values

**Fallback Behaviors:**

- Return safe default positions for invalid grid coordinates
- Log mathematical warnings for boundary constraint violations
- Gracefully handle edge cases like zero-sized grids or negative spacing