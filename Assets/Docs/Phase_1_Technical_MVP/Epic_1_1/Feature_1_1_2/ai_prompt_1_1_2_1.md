# **Unity C# Implementation Task: Paddle Data Structure Definition** *(45 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.1.2.1
**Category:** System
**Tags:** Data Structure, Configuration, Physics
**Priority:** Critical

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** PaddleData serializable class for configuration management
**Game Context:** Breakout arcade game requiring responsive paddle control with configurable physics properties

**Purpose:** Creates foundational data structure that defines paddle physics properties, input configuration, and movement constraints for Inspector-driven configuration and runtime behavior management
**Complexity:** Low - 45 minutes for pure data structure implementation

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
[System.Serializable]
public class PaddleData
{
    [Header("Movement Properties")]
    public float movementSpeed = 8.0f;
    public float acceleration = 15.0f;
    public Vector2 paddleDimensions = new Vector2(2.0f, 0.3f);
    
    [Header("Input Configuration")]
    public float inputSensitivity = 1.0f;
    public bool enableKeyboardInput = true;
    public bool enableMouseInput = true;
    
    [Header("Boundary Constraints")]
    public float leftBoundary = -8.0f;
    public float rightBoundary = 8.0f;
    
    [Header("Runtime State")]
    public Vector2 currentPosition;
    public float currentVelocity;
    public InputMethod activeInputMethod = InputMethod.None;
}

public enum InputMethod { None, Keyboard, Mouse }
```

### **Core Logic:**

- Serializable data container pattern with [System.Serializable] attribute
- Logical grouping of properties using [Header] attributes for Inspector clarity
- Default values appropriate for arcade-style Breakout paddle control
- Input sensitivity range validation (0.5f to 3.0f) with default 1.0f
- Boundary constraint parameters for playfield containment

### **Dependencies:**

- No external dependencies - pure data structure
- Uses Unity's Vector2 for position and dimension data
- Requires Unity's serialization system for Inspector integration

### **Performance Constraints:**

- Lightweight structure with minimal memory footprint
- No runtime allocation or garbage collection pressure
- Value types where appropriate for optimal performance

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - pure data container without behavior
- Keep class focused on configuration and state management only
- Use appropriate default values for immediate usability
- Include validation ranges for Inspector slider integration

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** No GameObject creation required - pure data structure
**Scene Hierarchy:** N/A - script-only implementation
**Inspector Config:** Serializable fields with [Header] organization and appropriate default values
**System Connections:** Foundation for PaddleController integration and Inspector configuration

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (creates folder structure for paddle system)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** `Assets/Scripts/Paddle/PaddleData.cs`
**Code Standards:** Unity C# naming conventions, XML documentation for public properties, comprehensive [Header] organization

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/Task1121CreatePaddleDataSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Paddle Data Structure"`

**Class Pattern:** `CreatePaddleDataSetup` (static class)

**Core Functionality:**

- Create Assets/Scripts/Paddle/ folder structure
- Generate PaddleData.cs with proper template
- No GameObject creation required for this task
- Validate folder structure exists
- Handle directory creation gracefully

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public static class CreatePaddleDataSetup
{
    [MenuItem("Breakout/Setup/Create Paddle Data Structure")]
    public static void CreatePaddleDataStructure()
    {
        // Create folder structure
        // Generate PaddleData.cs file
        // Validate implementation
        Debug.Log("✅ Paddle Data Structure created successfully");
    }

    [MenuItem("Breakout/Setup/Create Paddle Data Structure", true)]
    public static bool ValidateCreatePaddleDataStructure()
    {
        return !File.Exists("Assets/Scripts/Paddle/PaddleData.cs");
    }
}
#endif
```

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output showing PaddleData class structure and default values
- Provide instructions for using the data structure in future PaddleController implementation
- Include guidance on Inspector configuration and property customization

### **Documentation:**

- Create `Assets/Scripts/Paddle/README.md` capturing implementation details and usage instructions
- Document default value rationale and configuration guidelines
- Include examples of how properties affect paddle behavior

### **Custom Instructions:**

- Include property validation methods for runtime safety
- Add helpful tooltips using [Tooltip] attributes for Inspector usability
- Implement property range validation where appropriate

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] PaddleData class properly serializes in Unity Inspector
- [ ] All movement properties have appropriate default values and constraints
- [ ] Input configuration supports multiple input methods
- [ ] Boundary constraint parameters enable proper playfield containment

### **Integration Tests:**

- [ ] Class appears correctly in Inspector when used as serialized field
- [ ] Default values provide immediately functional paddle behavior
- [ ] Property organization with [Header] attributes creates clear Inspector sections

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] Class is focused and appropriately sized for data container role
- [ ] XML documentation provided for all public properties

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** Not applicable - no external dependencies
**ValidationLevel:** Basic - include property range validation and null checks
**Reusability:** Reusable - generic data structure for any paddle-based game

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use [System.Serializable] for Inspector integration
- Organize properties with [Header] attributes for clarity
- Provide meaningful default values for immediate usability
- Use appropriate Unity types (Vector2, float) for consistency
- Include [Tooltip] attributes for Inspector documentation

### **Performance Requirements:**

- Lightweight data structure with minimal memory footprint
- No runtime allocations or garbage collection pressure
- Value types for simple properties where appropriate

### **Architecture Pattern:**

- Data container pattern with validation and default value management
- Separation of data from behavior (pure configuration structure)

## **DEPENDENCY HANDLING**

**Missing Dependencies:**
- No external dependencies required for this pure data structure

**Fallback Behaviors:**

- Provide safe default values for all properties
- Include property validation for range constraints
- Graceful handling of invalid configuration values