# **Unity C# Implementation Task: Physics Layer Configuration Setup** *(45 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.1.3.1
**Category:** System
**Tags:** Physics, Configuration, Foundation
**Priority:** Critical

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** Unity Physics Layer System Configuration
**Game Context:** Breakout arcade game requiring precise collision detection between ball, paddle, bricks, power-ups, and boundaries

**Purpose:** Establishes foundation for collision isolation that prevents unwanted physics interactions while ensuring proper collision detection between game objects that should interact.
**Complexity:** Medium - Unity physics settings configuration with collision matrix setup

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// No custom classes required - Unity Editor configuration only
// Editor setup script for automated configuration
public static class CreatePhysicsLayersSetup
{
    public static void CreatePhysicsLayers()
    {
        // Configure 5 named physics layers
        // Set up collision matrix interactions
        // Apply layers to existing GameObjects
    }
}
```

### **Core Logic:**

- Create 5 named physics layers: "Ball", "Paddle", "Bricks", "PowerUps", "Boundaries"
- Configure collision matrix to allow only intended interactions:
  - Ball interacts with: Paddle, Bricks, Boundaries
  - Paddle interacts with: Ball, PowerUps, Boundaries  
  - Bricks interact with: Ball only
  - PowerUps interact with: Paddle, Boundaries only
  - Boundaries interact with: Ball, Paddle, PowerUps
- Apply appropriate layer assignments to existing Ball and Paddle GameObjects

### **Dependencies:**

- Basic Unity scene with Ball and Paddle GameObjects existing
- If Ball GameObject missing: Log error with setup instructions
- If Paddle GameObject missing: Log error with setup instructions

### **Performance Constraints:**

- One-time configuration with no runtime performance impact
- No memory allocation during gameplay - configuration only

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - configuration setup only
- Keep setup script focused on physics layer configuration exclusively
- Only implement layer configuration explicitly required by specification
- Avoid adding extra layers or collision rules not specified

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Apply layers to existing Ball and Paddle GameObjects in scene
**Scene Hierarchy:** No hierarchy changes - layer assignment to existing objects only
**Inspector Config:** No serialized fields required - pure configuration task
**System Connections:** Establishes foundation for CollisionManager event routing system

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (Editor setup script only)
3. **Editor Setup Script** (creates physics layer configuration)
4. **Integration Notes** (brief explanation of how this supports collision system)

**File Structure:** `Assets/Editor/Setup/1131CreatePhysicsLayersSetup.cs`
**Code Standards:** Unity Editor conventions, clear logging, proper validation

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1131CreatePhysicsLayersSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Physics Layers"`

**Class Pattern:** `CreatePhysicsLayersSetup` (static class)

**Core Functionality:**

- Create 5 named physics layers in Unity's layer system
- Configure collision matrix with proper interaction rules
- Find and assign layers to existing Ball and Paddle GameObjects
- Validate layer assignments and collision matrix settings
- Handle missing GameObjects gracefully with clear error messages
- Prevent duplicate layer creation with validation
- Log detailed success/failure messages for each step

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreatePhysicsLayersSetup
{
    [MenuItem("Breakout/Setup/Create Physics Layers")]
    public static void CreatePhysicsLayers()
    {
        // Layer creation and naming
        // Collision matrix configuration
        // GameObject layer assignment
        // Validation and logging
        Debug.Log("✅ Physics Layers configured successfully");
    }

    [MenuItem("Breakout/Setup/Create Physics Layers", true)]
    public static bool ValidateCreatePhysicsLayers()
    {
        // Return false if layers already properly configured
        return true; // Allow reconfiguration
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages for each configuration step
- Handle missing Ball/Paddle GameObjects with informative errors
- Validate layer creation succeeded before matrix configuration
- Provide actionable instructions for manual fixes if automation fails

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output showing which layers were created/configured
- List which GameObjects had their layers assigned
- Provide validation steps to verify collision matrix is working correctly
- Include instructions for manually verifying layer assignments in Inspector

### **Documentation:**

- Create brief .md file capturing:
  - Layer names and their intended purposes
  - Collision matrix configuration details
  - GameObject layer assignments made
  - Editor setup script usage instructions

### **Custom Instructions:**

- Use TagManager.asset SerializedObject manipulation for automated layer creation
- Include Physics2D.GetIgnoreLayerCollision() validation to confirm matrix settings
- Log layer index assignments for reference by future collision code

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Five distinct physics layers created and properly named (Ball, Paddle, Bricks, PowerUps, Boundaries)
- [ ] Collision matrix configured to allow only intended interactions
- [ ] Ball and Paddle GameObjects assigned to correct layers  
- [ ] Layer system prevents PowerUps from colliding with Bricks
- [ ] Physics layer configuration validated through collision testing

### **Integration Tests:**

- [ ] Ball GameObject assigned to "Ball" layer successfully
- [ ] Paddle GameObject assigned to "Paddle" layer successfully
- [ ] Collision matrix prevents Ball-PowerUp interactions

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity Editor scripting best practices
- [ ] Configuration is reusable and doesn't break existing scene setup
- [ ] Clear logging provides feedback on all configuration steps

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** Basic - create minimal logging if Ball/Paddle GameObjects missing
**ValidationLevel:** Strict - validate all layer assignments and collision matrix settings
**Reusability:** Reusable - configuration should work across different Breakout scene setups

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use SerializedObject for TagManager.asset modification
- Cache layer indices in constants for future reference
- Use LayerMask.LayerToName() for validation
- No runtime allocations - editor-time configuration only

### **Performance Requirements:**

- Configuration completes in under 5 seconds
- No impact on scene loading or runtime performance
- Memory efficient editor scripting

### **Architecture Pattern:**

- Configuration setup pattern - pure editor utility with no runtime components

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If Ball GameObject is missing:** Log clear error: "Ball GameObject not found in scene. Please create Ball GameObject before configuring physics layers."
- **If Paddle GameObject is missing:** Log clear error: "Paddle GameObject not found in scene. Please create Paddle GameObject before configuring physics layers."
- **If Managers hierarchy missing:** Create Managers parent GameObject automatically

**Fallback Behaviors:**

- Continue configuration even if some GameObjects are missing
- Log warnings for partial configuration completion
- Provide manual assignment instructions for missing objects