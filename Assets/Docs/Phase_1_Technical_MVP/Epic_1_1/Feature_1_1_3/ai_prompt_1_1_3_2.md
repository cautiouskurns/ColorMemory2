# **Unity C# Implementation Task: CollisionManager Base Structure** *(50 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.1.3.2
**Category:** System
**Tags:** Physics, Manager, Foundation
**Priority:** Critical

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** CollisionManager MonoBehaviour class with singleton pattern
**Game Context:** Breakout arcade game requiring centralized collision coordination between ball, paddle, bricks, and boundaries

**Purpose:** Creates the central hub for all collision response coordination, providing a foundation framework that future collision logic can extend while maintaining single responsibility for collision detection and routing.
**Complexity:** Medium - singleton manager with event subscription system and collision categorization

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance { get; private set; }
    
    [Header("Collision Detection")]
    [SerializeField] private bool enableCollisionLogging = true;
    
    // Collision event handling methods
    public void OnCollisionEnter2D(Collision2D collision) { }
    public void OnCollisionExit2D(Collision2D collision) { }
    
    // Collision routing framework
    private void HandlePaddleCollision(Collision2D collision) { }
    private void HandleBrickCollision(Collision2D collision) { }
    private void HandleBoundaryCollision(Collision2D collision) { }
    
    // Collision type detection
    private CollisionType DetermineCollisionType(Collision2D collision) { }
}

public enum CollisionType
{
    Paddle, Brick, Boundary, PowerUp, Unknown
}
```

### **Core Logic:**

- Singleton pattern ensures single collision coordinator across game
- Event subscription system captures all Ball collision events
- Collision type detection uses GameObject layer to categorize collisions
- Routing framework provides stub methods for specific collision response implementations
- Logging system captures collision events for debugging and validation

### **Dependencies:**

- Physics layers configured and applied to game objects (Task 1.1.3.1)
- Ball GameObject with Rigidbody2D and Collider2D components
- If Ball GameObject missing: Create stub reference with clear logging
- If physics layers not configured: Log warning and use fallback layer detection

### **Performance Constraints:**

- Event-driven system with minimal processing overhead
- No memory allocation during collision events
- Collision detection completes within single frame

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - collision coordination only
- Keep CollisionManager focused on event handling and routing
- Only implement collision detection and logging explicitly required
- Avoid adding collision response logic - save for future tasks

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** New CollisionManager GameObject under Managers hierarchy with CollisionManager component
**Scene Hierarchy:** Create under existing Managers parent container, position at (0,0,0)
**Inspector Config:** Enable collision logging checkbox, clear component references for future extension
**System Connections:** Subscribes to Ball collision events, provides framework for future collision response routing

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering singleton setup, event handling, collision detection, and framework preparation)
2. **Code Files** (CollisionManager.cs with full implementation)
3. **Editor Setup Script** (creates CollisionManager GameObject and configures scene)
4. **Integration Notes** (explanation of how this coordinates with Ball physics and prepares for future collision responses)

**File Structure:** `Assets/Scripts/Managers/CollisionManager.cs`
**Code Standards:** Unity MonoBehaviour conventions, singleton pattern best practices, clear debug logging

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1132CreateCollisionManagerSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Collision Manager"`

**Class Pattern:** `CreateCollisionManagerSetup` (static class)

**Core Functionality:**

- Create CollisionManager GameObject under Managers hierarchy
- Add CollisionManager component with proper configuration
- Position GameObject at origin with zero rotation
- Configure collision logging enabled by default
- Validate Ball GameObject exists and log connection status
- Handle missing Managers parent by creating it
- Prevent duplicate creation with validation MenuItem

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateCollisionManagerSetup
{
    [MenuItem("Breakout/Setup/Create Collision Manager")]
    public static void CreateCollisionManager()
    {
        // Validate prerequisites and create Managers hierarchy if needed
        // Create CollisionManager GameObject with component
        // Configure component settings and validate Ball connection
        // Log setup completion and next steps
        Debug.Log("✅ CollisionManager created successfully");
    }

    [MenuItem("Breakout/Setup/Create Collision Manager", true)]
    public static bool ValidateCreateCollisionManager()
    {
        return GameObject.FindObjectOfType<CollisionManager>() == null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with collision manager status
- Handle missing Managers parent container by creating it
- Validate singleton instance creation completed successfully
- Provide instructions for connecting to Ball GameObject if missing

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output showing CollisionManager creation status
- List which collision event methods are ready for future implementation
- Provide validation steps to verify collision events are being captured
- Include instructions for testing collision detection with Ball movement

### **Documentation:**

- Create brief .md file capturing:
  - CollisionManager singleton usage patterns
  - Collision type detection implementation details
  - Framework methods available for future collision response logic
  - Integration points with Ball physics system

### **Custom Instructions:**

- Implement collision event logging with collision type, GameObject names, and timestamps
- Add collision type detection using layer comparison with clear fallback for unknown layers
- Create framework stub methods that log when called to verify routing works

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] CollisionManager properly detects and categorizes all collision types
- [ ] Collision events are captured and logged for all relevant GameObjects
- [ ] System maintains single responsibility for collision coordination
- [ ] Framework ready for specific collision response implementations
- [ ] Singleton pattern ensures single collision coordinator

### **Integration Tests:**

- [ ] CollisionManager singleton accessible from other scripts
- [ ] Ball collision events trigger CollisionManager detection methods
- [ ] Collision type detection correctly identifies paddle vs boundary collisions

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity MonoBehaviour and singleton best practices
- [ ] Classes are focused on collision coordination only
- [ ] Clear debug logging provides useful development information

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** Basic - create minimal logging for missing Ball GameObject reference
**ValidationLevel:** Strict - validate singleton instance and collision event subscription
**Reusability:** Reusable - CollisionManager should work across different Breakout scene configurations

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache Ball GameObject reference during Awake() for collision event handling
- Use singleton pattern with proper null checking and instance management
- Minimize garbage collection with efficient collision event processing
- No hard-coded layer indices - use LayerMask.NameToLayer() for layer detection
- Implement proper MonoBehaviour lifecycle (Awake, Start, OnDestroy)

### **Performance Requirements:**

- Collision event processing completes within 0.1ms
- No memory allocation during collision event handling
- Singleton access with O(1) performance

### **Architecture Pattern:**

- Singleton Manager pattern for centralized collision coordination with event-driven collision handling

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If Ball GameObject is missing:** Create ball reference field for future assignment with clear logging
- **If physics layers not configured:** Use default layer detection with warnings
- **If Managers hierarchy missing:** Create Managers parent GameObject automatically

**Fallback Behaviors:**

- Log informative warnings for missing Ball GameObject connection
- Use LayerMask.NameToLayer() with -1 checks for missing layers
- Gracefully handle collision events even with incomplete configuration