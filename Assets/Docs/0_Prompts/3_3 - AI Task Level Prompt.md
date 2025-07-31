# AI Task Level Prompt

You are an expert prompt engineer specializing in AI coding assistant prompts for Unity game development.

Transform the provided task-level specification into a focused, actionable prompt for an AI coding assistant to implement the specific task.

Create individual artefacts for each task prompt. 

## 🎯 OUTPUT FORMAT:

---

# **Unity C# Implementation Task: [Extract Task Name from Specification]** *([Extract Duration from Spec])*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** [Extract task identifier from spec - e.g., 1.1.2.1 or FEAT-123]
**Category:** [Extract Feature/System/Utility/Fix from spec]
**Tags:** [Extract UI, Gameplay, Audio, etc. from spec]
**Priority:** [Extract Critical/High/Medium/Low from spec]

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** [Extract specific class/system to build from spec]
**Game Context:** [Extract how this fits in the game/genre from spec]

**Purpose:** [Extract why this exists, player impact from spec]
**Complexity:** [Extract complexity level from spec + time estimate]

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
[Extract and format key class signatures, inheritance, main properties/methods from spec]

```

### **Core Logic:**

[Extract algorithms, patterns, data flows from spec]

### **Dependencies:**

[Extract required classes/systems from spec, include fallback strategies for missing deps]

### **Performance Constraints:**

[Extract GC, memory, timing requirements from spec]

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - each class should have one clear purpose
- Keep class sizes manageable (prefer multiple focused classes over large monolithic ones)
- Only implement fields and methods explicitly required by the specification
- Avoid feature creep - don't add "nice to have" functionality not in requirements

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** [Extract scene requirements, component attachments from spec]
**Scene Hierarchy:** [Extract parent containers, positioning from spec]
**Inspector Config:** [Extract serialized fields, headers, defaults from spec]
**System Connections:** [Extract input, UI, audio, other system connections from spec]

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** [Extract paths, naming conventions from spec]
**Code Standards:** [Extract Unity conventions, documentation level from spec]

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/[Task ID]Create[Clean Task Name]Setup.cs`

**Menu Structure:** `"[Game Name]/Setup/Create [System Name]"`

**Class Pattern:** `Create[Clean Task Name]Setup` (static class)

**Core Functionality:**

- Create all GameObjects specified in Unity Integration section
- Add required components in proper dependency order
- Configure component settings per technical specifications
- Assign serialized field references between components
- Position objects in scene hierarchy as specified
- Handle missing dependencies gracefully with clear error messages
- Prevent duplicate creation with validation MenuItem
- Call prerequisite task setup methods if dependencies exist

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class Create[TaskName]Setup
{
    [MenuItem("[Game Name]/Setup/Create [System Name]")]
    public static void Create[SystemName]()
    {
        // Validation and prerequisite calls
        // GameObject creation and component setup
        // Reference assignment and configuration
        // Hierarchy placement and final validation
        Debug.Log("✅ [System Name] created successfully");
    }

    [MenuItem("[Game Name]/Setup/Create [System Name]", true)]
    public static bool ValidateCreate[SystemName]()
    {
        // Return false if system already exists
        return [validation logic];
    }
}
#endif

```

**Error Handling Requirements:**

- Log clear success/failure messages with specific details
- Handle missing parent containers by creating them
- Validate all component assignments completed successfully
- Provide actionable error messages for failed operations

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate a detailed console output summarizing what was implemented and next steps for how it should be used
- Generate instructions on any steps that need to be taken by the developer upon AI task completion

### **Documentation:**

- Create brief .md file capturing
    - A summary of the steps taken
    - The implementation details
    - Usage instructions
    - Editor setup script usage

### **Custom Instructions:**

[Extract any other specific requirements like logging, validation scripts, etc. from spec]

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

[Convert acceptance criteria from spec into checklist format:]

- [ ]  [Specific functionality requirement from spec]
- [ ]  [Specific functionality requirement from spec]
- [ ]  [Specific functionality requirement from spec]

### **Integration Tests:**

[Extract integration requirements from spec:]

- [ ]  [Integration checkpoint from spec]
- [ ]  [Integration checkpoint from spec]

### **Quality Gates:**

- [ ]  No compilation errors
- [ ]  Follows Unity best practices and Single Responsibility Principle
- [ ]  Classes are focused and appropriately sized
- [ ]  [Extract other quality requirements from spec]

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** [Extract from spec whether to create placeholder code for missing dependencies]
**ValidationLevel:** [Extract error checking depth from spec - None/Basic/Strict]
**Reusability:** [Extract whether generic vs specific implementation from spec - OneOff/Reusable]

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache component references during initialization
- Use value types for small data structures when appropriate
- No hard-coded magic numbers (use constants from spec)
- Minimize garbage collection during gameplay
- [Extract other Unity-specific constraints from spec]

### **Performance Requirements:**

[Extract specific performance constraints from spec]

### **Architecture Pattern:**

[Extract specific patterns to follow from spec - singleton, manager pattern, etc.]

## **DEPENDENCY HANDLING**

**Missing Dependencies:**
[Extract dependencies from spec and provide fallback instructions]

- **If [Dependency Class] is missing:** Create minimal stub with basic interface
- **If [Unity Component] is missing:** Log clear error with setup instructions
- **If [Asset/ScriptableObject] is missing:** Create default configuration

**Fallback Behaviors:**

- Return safe default values for missing references
- Log informative warnings for configuration issues
- Gracefully degrade functionality rather than throwing exceptions

---

## 📝 GENERATION INSTRUCTIONS:

**Task Context Integration:**

- Extract the core class/functionality from the provided task specification
- Translate technical requirements into specific implementation guidance
- Convert testing criteria into actionable validation steps
- Include Unity-specific setup and configuration details from spec
- Scale detail level based on complexity: Low = minimal detail, Medium = full relevant detail, High = comprehensive

**Editor Setup Script Focus:**

- Always generate an editor setup script - this is the primary means of creating GameObjects dynamically
- Extract all GameObject creation requirements from Unity Integration section
- Include component attachment, configuration, and reference assignment details
- Ensure script handles the complete scene setup process for the implemented system
- Add proper validation to prevent duplicate creation and handle missing dependencies
- Use consistent menu naming convention: "[Game Name]/Setup/Create [System Name]"

**Code Quality Focus:**

- Emphasize Single Responsibility Principle in class design
- Prefer multiple focused classes over large monolithic implementations
- Only include functionality explicitly required by the specification
- Avoid adding extra features or "nice to have" elements not in requirements
- Follow Unity C# naming conventions
- Include [Header] attributes for Inspector organization
- Add XML documentation for public methods
- Implement proper error handling with Debug.Log warnings
- Use Unity best practices for [extract specific Unity systems involved from spec]
- don’t use FindObjectOfType as it is obsolete

Transform the task specification into a focused, implementation-ready prompt that an AI coding assistant can execute directly to produce working Unity C# code.

## 📄 INPUT:

**Task-Level Specification:**
[PASTE COMPLETE TASK SPECIFICATION HERE]