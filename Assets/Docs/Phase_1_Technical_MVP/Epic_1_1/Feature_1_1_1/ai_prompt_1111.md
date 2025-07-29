# **Unity C# Implementation Task: Ball Data Structure Definition** *(45 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.1.1.1
**Category:** System
**Tags:** Physics, Data Structure, Foundation
**Priority:** Critical

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** BallData serializable class for ball physics configuration and state management
**Game Context:** Breakout - Arcade action game requiring consistent ball physics with configurable parameters

**Purpose:** Creates the foundational data structure that defines ball physics properties, state tracking, and configuration parameters for all ball physics systems. This enables Inspector-based tuning and provides the data foundation for BallController, launch mechanics, and physics debugging.
**Complexity:** Low complexity data structure implementation with 45-minute time estimate

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
[System.Serializable]
public class BallData
{
    [Header("Speed Configuration")]
    public float baseSpeed;
    public float minSpeed;
    public float maxSpeed;
    
    [Header("Launch Settings")]
    public Vector2 launchDirection;
    public float launchAngleRange;
    
    [Header("Physics State")]
    public Vector2 currentVelocity;
    public int collisionCount;
    public Vector3 launchPosition;
    
    [Header("Arcade Physics Tuning")]
    public float bounceDamping;
    public bool maintainConstantSpeed;
}
```

### **Core Logic:**

Data structure pattern with serializable properties for Unity Inspector integration:
- Speed constraints system with min/max velocity values for arcade-style physics
- Launch mechanics configuration with directional control and angle parameters
- Runtime state tracking for velocity, collision count, and position monitoring
- Arcade physics tuning parameters for bounce damping and speed consistency
- Default value initialization appropriate for immediate arcade gameplay testing

### **Dependencies:**

- Unity Engine (Vector2, Vector3 for position/velocity tracking)
- System.Serializable attribute for Inspector visibility
- No external dependencies - pure data structure implementation

### **Performance Constraints:**

- Lightweight data container with minimal memory overhead for real-time physics
- Value types for physics properties to avoid garbage collection pressure
- Efficient serialization for Inspector updates without performance impact

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - focus solely on data storage and configuration
- Keep class size minimal and focused on ball physics data only
- Only implement fields explicitly required by ball physics system specifications
- Avoid methods or complex logic - pure data structure pattern

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** No GameObject creation required - pure data structure implementation
**Scene Hierarchy:** N/A - data structure only
**Inspector Config:** Serializable fields with [Header] attributes for organized Inspector display
**System Connections:** Foundation for BallController, physics debugging, and launch mechanics

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (BallData.cs with complete implementation)
3. **Editor Setup Script** (validation script ensuring proper class configuration)
4. **Integration Notes** (brief explanation of how this integrates with future ball physics systems)

**File Structure:** `Assets/Scripts/Ball/BallData.cs`
**Code Standards:** Unity C# conventions, clear documentation, proper serialization attributes

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1111CreateBallDataSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Ball Data Structure"`

**Class Pattern:** `CreateBallDataSetup` (static class)

**Core Functionality:**

- Validate BallData class exists and compiles correctly
- Create test GameObject with BallData serialization validation
- Verify Inspector serialization displays properly with organized headers
- Test default value assignment and constraint validation
- Provide debugging information about data structure configuration
- Validate that all required fields are properly serialized

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateBallDataSetup
{
    [MenuItem("Breakout/Setup/Create Ball Data Structure")]
    public static void CreateBallDataStructure()
    {
        // Validation of BallData class compilation
        // Test GameObject creation for serialization validation
        // Inspector serialization verification
        // Default value validation
        Debug.Log("✅ Ball Data Structure validated successfully");
    }

    [MenuItem("Breakout/Setup/Create Ball Data Structure", true)]
    public static bool ValidateCreateBallDataStructure()
    {
        // Return true - always available for validation
        return true;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages for data structure validation
- Verify Inspector serialization works correctly with organized field display
- Validate default values are appropriate for arcade gameplay
- Provide actionable feedback for any serialization or compilation issues

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing BallData class structure and configuration
- Provide next steps for BallController integration and physics system usage
- Include validation results for Inspector serialization and default values

### **Documentation:**

- Create brief .md file capturing BallData structure design decisions
- Document field purposes and default value rationale
- Include usage instructions for future ball physics system integration
- Document Inspector organization and serialization approach

### **Custom Instructions:**

- Include comprehensive field documentation with XML comments
- Provide clear default values suitable for arcade-style Breakout gameplay
- Ensure Inspector organization with logical [Header] groupings for ease of use

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] BallData class properly serializes in Unity Inspector
- [ ] All physics properties have appropriate default values and constraints
- [ ] State tracking variables accurately reflect ball status during gameplay
- [ ] Configuration parameters enable arcade-style physics tuning
- [ ] Class follows Unity serialization best practices
- [ ] Data structure supports all planned ball physics features

### **Integration Tests:**

- [ ] Inspector displays organized field groups with clear headers
- [ ] Default values provide immediate arcade-appropriate physics behavior
- [ ] Data structure compiles without errors and integrates with Unity serialization

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity C# naming conventions and serialization best practices
- [ ] Class is focused and appropriately sized for single responsibility
- [ ] Proper XML documentation for all public fields

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** None - no external dependencies for pure data structure
**ValidationLevel:** Basic - validate serialization and default values
**Reusability:** Reusable - data structure will be used by multiple ball physics systems

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use [System.Serializable] attribute for Inspector visibility
- Organize fields with [Header] attributes for logical Inspector grouping
- Use appropriate data types (float for speeds, Vector2 for directions)
- Provide meaningful default values for immediate testing capability
- Follow Unity C# naming conventions (camelCase for fields)

### **Performance Requirements:**

- Lightweight data container with minimal memory overhead
- Value types for physics properties to avoid GC pressure
- Efficient serialization without runtime performance impact

### **Architecture Pattern:**

- Data structure pattern with serializable properties for Unity integration
- Single responsibility focused solely on ball physics data storage
- Foundation pattern supporting multiple dependent systems

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If Unity Engine unavailable:** Log clear error - Unity environment required
- **If serialization fails:** Provide troubleshooting steps for Inspector integration
- **If compilation errors occur:** Clear error messages with resolution steps

**Fallback Behaviors:**

- Use sensible default values if configuration not provided
- Log informative warnings for missing or invalid field values
- Ensure graceful degradation if Inspector serialization encounters issues

---