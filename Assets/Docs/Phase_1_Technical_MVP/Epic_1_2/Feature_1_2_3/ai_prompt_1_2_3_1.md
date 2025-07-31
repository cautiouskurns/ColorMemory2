# **Unity C# Implementation Task: Collision Event System Foundation** *(45 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.2.3.1
**Category:** System
**Tags:** Events, Communication, Architecture, UnityEvents
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** BrickCollisionEvents system with UnityEvent integration
**Game Context:** Breakout-style game requiring event-driven communication between collision detection, brick destruction, scoring systems, and level completion tracking

**Purpose:** Creates the foundational event system that enables decoupled communication between all collision-related systems, providing the architecture foundation for scoring, tracking, and future power-up integration.
**Complexity:** Medium complexity - 45 minutes (event system architecture with data structures)

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
[System.Serializable]
public class CollisionEventData
{
    public Vector3 collisionPoint;
    public Vector3 impactForce;
    public float intensity;
    public GameObject collidingObject;
    public string destructionCause;
    public System.DateTime timestamp;
}

[System.Serializable]
public class BrickEventData : CollisionEventData
{
    public BrickType brickType;
    public Vector3 brickPosition;
    public int pointValue;
    public bool wasDestroyable;
}

public class BrickCollisionEvents : MonoBehaviour
{
    [System.Serializable]
    public class BrickDestroyedEvent : UnityEvent<BrickEventData> { }
    
    [System.Serializable]
    public class BrickDamagedEvent : UnityEvent<BrickEventData> { }
    
    [System.Serializable]
    public class BrickCollisionEvent : UnityEvent<BrickEventData> { }
    
    public static BrickCollisionEvents Instance { get; private set; }
    
    [Header("Collision Events")]
    public BrickDestroyedEvent OnBrickDestroyed;
    public BrickDamagedEvent OnBrickDamaged;
    public BrickCollisionEvent OnBrickCollision;
}
```

### **Core Logic:**

- Singleton pattern for global event access with proper initialization
- UnityEvent integration for Inspector subscription and MonoBehaviour compatibility
- Event data structures containing collision details, brick references, and impact information
- Event validation and error handling to prevent null reference exceptions
- Centralized event coordination with subscription management

### **Dependencies:**

- Unity's UnityEvent system for event architecture
- Basic Unity project setup with MonoBehaviour support
- Future integration with Brick system (if missing, create placeholder BrickType enum)

### **Performance Constraints:**

- Minimal memory allocation during event firing
- Efficient subscription management without memory leaks
- No garbage collection during event processing

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - pure event system without game logic
- Keep event data structures focused on essential collision information
- Use UnityEvent for Inspector integration and MonoBehaviour compatibility
- Implement singleton pattern for global event access

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** BrickCollisionEvents GameObject with singleton component
**Scene Hierarchy:** Event management container at root level for global access
**Inspector Config:** UnityEvent fields with organized headers for event subscription
**System Connections:** Foundation for collision detection, brick destruction, scoring systems, and level completion tracking

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering event system approach and singleton pattern)
2. **Code Files** (all event system files in dependency order)
3. **Editor Setup Script** (creates event system GameObject and demonstrates usage)
4. **Integration Notes** (explanation of event system benefits and subscription patterns)

**File Structure:** `Assets/Scripts/Events/BrickCollisionEvents.cs`, `Assets/Scripts/Events/CollisionEventData.cs`
**Code Standards:** Unity C# conventions, XML documentation for public events, organized Inspector sections

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/Task1231CreateCollisionEventSystemSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Collision Event System"`

**Class Pattern:** `CreateCollisionEventSystemSetup` (static class)

**Core Functionality:**

- Create BrickCollisionEvents GameObject with singleton component
- Configure event system with default settings and validation
- Demonstrate event subscription patterns for testing
- Validate singleton initialization and event firing
- Set up example event listeners for development testing

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateCollisionEventSystemSetup
{
    [MenuItem("Breakout/Setup/Create Collision Event System")]
    public static void CreateCollisionEventSystem()
    {
        // Validation and singleton setup
        // GameObject creation and component configuration
        // Event system testing and validation
        Debug.Log("✅ Collision Event System created successfully");
    }

    [MenuItem("Breakout/Setup/Create Collision Event System", true)]
    public static bool ValidateCreateCollisionEventSystem()
    {
        return Object.FindFirstObjectByType<BrickCollisionEvents>() == null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log event system creation success with singleton validation
- Handle duplicate singleton creation with clear warnings
- Validate event system initialization and provide setup guidance
- Report event subscription patterns and usage examples

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate console output showing event system architecture and available events
- Provide event subscription examples and integration patterns for future systems
- List event data structure contents and their purposes for scoring and tracking

### **Documentation:**

- Create brief .md file capturing:
    - Event system purpose and architecture benefits
    - Event subscription patterns and best practices
    - Event data structure documentation and usage examples
    - Integration guidance for future collision-related systems

### **Custom Instructions:**

- Include event firing examples and testing utilities
- Add event validation to ensure proper data population
- Provide clear debugging output for event system operations

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Event system supports brick destruction, damage, and collision event types with appropriate data structures
- [ ] Events can be subscribed to and fired reliably without memory leaks or performance impact
- [ ] Event data structures provide sufficient information for scoring, tracking, and future power-up systems
- [ ] Event system integrates seamlessly with Unity's component architecture and MonoBehaviour lifecycle

### **Integration Tests:**

- [ ] Event system creates and maintains singleton instance properly
- [ ] UnityEvents are properly configured for Inspector subscription
- [ ] Event data structures populate correctly with collision information

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] Event system is focused on communication architecture only
- [ ] All public events have XML documentation
- [ ] Singleton pattern implemented correctly with proper initialization

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** True - Create placeholder BrickType enum if brick system unavailable
**ValidationLevel:** Basic - Include event validation and error handling for null references
**Reusability:** Reusable - Design event system for use across multiple game systems and future features

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use singleton pattern with proper Unity lifecycle management
- Cache event data to avoid allocation during event firing
- Apply [Header] attributes for organized Inspector event sections
- Use [System.Serializable] for event data structures
- Implement proper null checking for event subscribers

### **Performance Requirements:**

- Minimal memory allocation during event firing operations
- Efficient event subscription management without memory leaks
- No garbage collection during collision event processing

### **Architecture Pattern:**

Observer pattern with UnityEvent system for decoupled communication and Inspector integration

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If BrickType enum missing:** Create placeholder enum with Basic, Strong, Bonus types
- **If MonoBehaviour framework unavailable:** Log error and provide Unity setup instructions 
- **If UnityEvent system missing:** Use standard C# events with explanation note

**Fallback Behaviors:**

- Use default event data values when collision information unavailable
- Log informative warnings for event system initialization issues
- Provide event system functionality even with minimal collision data available