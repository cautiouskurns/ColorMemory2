# **Unity C# Implementation Task: CollisionManager Brick Extension** *(55 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.2.3.2
**Category:** System
**Tags:** Collision, Manager, Integration, Routing
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** Enhanced CollisionManager with brick collision handling methods
**Game Context:** Breakout-style game requiring centralized collision management for ball-brick interactions with event system integration

**Purpose:** Extends existing CollisionManager to handle brick-specific collision events, coordinate destruction processing, and provide centralized collision management that integrates with the new event system while maintaining Epic 1.1 compatibility.
**Complexity:** Medium complexity - 55 minutes (extending existing system with brick-specific collision handling)

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// Extension to existing CollisionManager class
public partial class CollisionManager : MonoBehaviour
{
    [Header("Brick Collision Handling")]
    [SerializeField] private BrickCollisionEvents eventSystem;
    [SerializeField] private float impactForceMultiplier = 1.0f;
    [SerializeField] private bool enableBrickCollisionLogging = true;
    
    // Brick collision handling methods
    public void HandleBrickCollision(Collision2D collision, Brick brick)
    {
        // Process brick collision with intensity calculation and event firing
    }
    
    public void RegisterBrickCollision(Vector3 contactPoint, Vector3 impactForce, Brick brick)
    {
        // Register collision for centralized tracking and feedback systems
    }
    
    // Collision routing and validation
    private bool ValidateBrickCollision(Collision2D collision, Brick brick)
    private float CalculateCollisionIntensity(Collision2D collision)
    private Vector3 CalculateImpactForce(Collision2D collision)
    private void RouteCollisionToSystems(BrickEventData eventData)
}
```

### **Core Logic:**

- Extension of existing CollisionManager with brick-specific collision handling
- Collision intensity calculation based on relative velocity and impact force
- Collision routing that directs ball-brick collisions to destruction and feedback systems
- Event system integration for centralized collision coordination
- Collision validation and filtering to prevent duplicate processing

### **Dependencies:**

- Existing CollisionManager from Epic 1.1 (if missing, create minimal stub)
- BrickCollisionEvents system from Task 1.2.3.1
- Brick system from Feature 1.2.1 (if missing, create placeholder)

### **Performance Constraints:**

- Efficient collision routing without performance degradation
- Minimal computational overhead for collision intensity calculations
- No frame rate impact during collision processing

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - extend existing manager without breaking compatibility
- Keep brick collision handling focused and separate from other collision types
- Maintain compatibility with existing Epic 1.1 collision framework
- Use partial class approach if original CollisionManager cannot be modified directly

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Extends existing CollisionManager GameObject functionality
**Scene Hierarchy:** Works with existing CollisionManager in scene hierarchy
**Inspector Config:** Additional brick collision settings with organized headers
**System Connections:** Integrates with Epic 1.1 collision framework and new event system

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering extension approach and collision routing strategy)
2. **Code Files** (enhanced CollisionManager with brick handling methods)
3. **Editor Setup Script** (configures existing CollisionManager with brick extension)
4. **Integration Notes** (explanation of Epic 1.1 compatibility and event system integration)

**File Structure:** `Assets/Scripts/Collision/CollisionManager.cs` - enhance existing class
**Code Standards:** Unity C# conventions, maintain existing code style, comprehensive collision handling documentation

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/Task1232CreateCollisionManagerExtensionSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create CollisionManager Extension"`

**Class Pattern:** `CreateCollisionManagerExtensionSetup` (static class)

**Core Functionality:**

- Validate existing CollisionManager exists (call Epic 1.1 setup if needed)
- Configure BrickCollisionEvents reference on CollisionManager
- Set up brick collision handling parameters and settings
- Test collision routing with sample collision events
- Validate compatibility with existing Epic 1.1 collision framework

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateCollisionManagerExtensionSetup
{
    [MenuItem("Breakout/Setup/Create CollisionManager Extension")]
    public static void CreateCollisionManagerExtension()
    {
        // Validate CollisionManager exists and call prerequisites
        // Configure brick collision handling on existing manager
        // Set up event system integration and references
        // Test collision routing and validate compatibility
        Debug.Log("✅ CollisionManager Extension created successfully");
    }

    [MenuItem("Breakout/Setup/Create CollisionManager Extension", true)]
    public static bool ValidateCreateCollisionManagerExtension()
    {
        return Object.FindFirstObjectByType<CollisionManager>() != null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log extension setup success with collision handling validation
- Handle missing CollisionManager with clear setup instructions for Epic 1.1
- Validate event system integration and provide configuration guidance
- Report compatibility status with existing collision framework

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate console output showing CollisionManager extension capabilities and brick handling features
- Provide collision routing explanation and integration with event system
- List collision intensity calculation details and impact force determination

### **Documentation:**

- Create brief .md file capturing:
    - CollisionManager extension approach and Epic 1.1 compatibility
    - Brick collision handling workflow and event system integration
    - Collision intensity calculation methodology and performance considerations
    - Integration testing recommendations for collision routing validation

### **Custom Instructions:**

- Include collision intensity testing with various impact scenarios
- Add collision routing validation to ensure proper event firing
- Provide clear debugging output for collision processing verification

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] CollisionManager properly receives and processes brick collision events from existing Brick collision detection
- [ ] Collision routing directs ball-brick impacts to destruction system while maintaining performance
- [ ] Collision intensity and impact calculations provide consistent feedback for audio-visual systems
- [ ] Enhanced CollisionManager maintains compatibility with existing Epic 1.1 collision framework

### **Integration Tests:**

- [ ] Extension integrates seamlessly with existing CollisionManager without breaking functionality
- [ ] Event system integration works correctly with brick collision handling
- [ ] Collision routing properly fires events with accurate collision data

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] Extension focused on brick collision handling without feature creep
- [ ] All new methods have XML documentation
- [ ] Maintains compatibility with existing Epic 1.1 collision system

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** True - Create minimal CollisionManager stub if Epic 1.1 unavailable
**ValidationLevel:** Strict - Include comprehensive collision validation and compatibility checking
**Reusability:** Reusable - Design extension to work with various collision scenarios and brick types

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache collision calculations to avoid repeated computation
- Use Unity's Collision2D system for consistent collision data
- Apply proper component reference validation during initialization
- Implement collision intensity calculations with appropriate physics scaling
- Use SerializeField for inspector configuration without public exposure

### **Performance Requirements:**

- Efficient collision routing without frame rate impact during collision-heavy scenarios
- Minimal computational overhead for collision intensity and force calculations
- No garbage collection during collision processing operations

### **Architecture Pattern:**

Extension of existing Manager pattern with specialized brick collision handling while maintaining system compatibility

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If CollisionManager missing:** Create minimal stub with basic collision handling interface and setup guidance
- **If BrickCollisionEvents unavailable:** Log error and provide setup instructions for Task 1.2.3.1
- **If Brick system missing:** Create placeholder Brick class for compilation and testing

**Fallback Behaviors:**

- Use default collision intensity values when physics data unavailable
- Log informative warnings for missing event system integration
- Provide basic collision handling even when advanced routing unavailable