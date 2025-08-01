# **Unity C# Implementation Task: Boundary Configuration Data Structures** *(30 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.3.1.1
**Category:** System
**Tags:** Data Structures, Configuration, Boundaries, Physics
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** BoundaryConfig data structures and configuration system
**Game Context:** Breakout game requiring 16:10 aspect ratio playfield containment with physics-based boundary walls

**Purpose:** Creates foundational data structures and configuration system for boundary setup and management, enabling consistent boundary wall creation across different resolutions and gameplay scenarios.
**Complexity:** Low - requires data structure design, serialization, and configuration management

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// Enum for boundary types
public enum BoundaryType
{
    Top,
    Left, 
    Right,
    Bottom
}

// Main configuration structure
[System.Serializable]
public class BoundaryConfig
{
    // Boundary dimensions and positioning
    // Physics properties and material settings
    // Resolution scaling parameters
    // Validation parameters
}
```

### **Core Logic:**

- BoundaryType enum defines all boundary wall types (Top, Left, Right, Bottom)
- BoundaryConfig class/struct contains dimensions, positioning, physics properties for boundary setup
- Physics configuration includes material properties, bounce coefficients, collision settings
- Resolution scaling parameters maintain 16:10 aspect ratio across screen sizes
- Validation parameters for edge case detection and collision accuracy verification

### **Dependencies:**

- Basic Unity project setup only
- No external system dependencies for this foundational task
- **Fallback Strategy:** Use default values if configuration is missing

### **Performance Constraints:**

- Lightweight data structures with minimal memory footprint
- Serializable for Inspector configuration and runtime adjustment
- No garbage collection during gameplay - all data pre-configured

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - data structures only handle configuration
- Keep BoundaryConfig focused on boundary parameters without logic implementation
- Only implement fields and properties explicitly required by specification
- Use Data Transfer Object (DTO) pattern for clean configuration management

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** No direct GameObject creation - data structures only
**Scene Hierarchy:** N/A for this task
**Inspector Config:** Serializable data structures with [Header] attributes for organization
**System Connections:** Foundation for boundary wall creation, physics configuration, and scaling systems

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** 
- `Assets/Scripts/Boundaries/BoundaryConfig.cs` - Main configuration data structures and enumerations

**Code Standards:** Unity C# naming conventions, XML documentation for public methods, [Header] attributes for Inspector organization

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1311CreateBoundaryConfigSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Boundary Configuration"`

**Class Pattern:** `CreateBoundaryConfigSetup` (static class)

**Core Functionality:**

- Create BoundaryConfig ScriptableObject asset in Resources folder for easy loading
- Set up default configuration values for 16:10 aspect ratio gameplay
- Configure default physics material properties for arcade-style bouncing
- Create folder structure for boundary system organization
- Validate configuration completeness and provide usage instructions

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateBoundaryConfigSetup
{
    [MenuItem("Breakout/Setup/Create Boundary Configuration")]
    public static void CreateBoundaryConfiguration()
    {
        // Create Resources folder if needed
        // Create default BoundaryConfig ScriptableObject
        // Set default values for 16:10 aspect ratio
        // Save asset and log success message
        Debug.Log("✅ Boundary Configuration created successfully");
    }

    [MenuItem("Breakout/Setup/Create Boundary Configuration", true)]
    public static bool ValidateCreateBoundaryConfiguration()
    {
        // Return false if configuration already exists
        return !AssetDatabase.FindAssets("t:BoundaryConfig").Length > 0;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with specific details
- Handle missing Resources folder by creating it
- Validate configuration asset creation completed successfully
- Provide actionable instructions for using the created configuration

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing the data structures created
- Provide instructions on how to modify configuration values in Inspector
- Explain how other boundary system components will use these structures

### **Documentation:**

- Create brief .md file capturing data structure design decisions
- Document configuration parameters and their gameplay impact
- Include usage instructions for future boundary system development

### **Custom Instructions:**

- Create as ScriptableObject for easy Inspector configuration and runtime loading
- Include validation methods for configuration completeness
- Add default value initialization for immediate usability

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] BoundaryConfig structure supports all required boundary parameters for collision and physics setup
- [ ] Configuration system enables easy boundary modification and testing during development
- [ ] Data structures are serializable for Inspector configuration and runtime adjustment
- [ ] Boundary configuration supports resolution scaling while maintaining 16:10 aspect ratio gameplay

### **Integration Tests:**

- [ ] BoundaryConfig can be created and modified through Inspector interface
- [ ] Configuration values can be loaded and accessed by other systems at runtime

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] Classes are focused on configuration data only
- [ ] All properties are properly serialized for Inspector access

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** false - no external dependencies for data structures
**ValidationLevel:** Basic - simple validation for configuration completeness
**Reusability:** Reusable - configuration system for all boundary wall types

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use [System.Serializable] for Inspector visibility
- Include [Header] attributes for Inspector organization
- No hard-coded magic numbers (use named constants)
- Implement ISerializationCallbackReceiver if complex validation needed

### **Performance Requirements:**

- Lightweight data structures with minimal memory allocation
- Pre-configured values to avoid runtime calculations
- Efficient serialization for fast loading

### **Architecture Pattern:**

Data Transfer Object (DTO) pattern for clean separation between configuration and implementation

## **DEPENDENCY HANDLING**

**Missing Dependencies:**
- **If Unity project setup incomplete:** Log clear error with Unity version requirements
- **If Scripts folder missing:** Create folder structure automatically in setup script

**Fallback Behaviors:**

- Use sensible default values if configuration is incomplete
- Log informative warnings for missing configuration sections
- Provide self-validating configuration with error reporting

---