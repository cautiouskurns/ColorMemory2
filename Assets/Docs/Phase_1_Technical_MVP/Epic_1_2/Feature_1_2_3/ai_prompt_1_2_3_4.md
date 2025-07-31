# **Unity C# Implementation Task: Brick Tracking and Level Completion System** *(45 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.2.3.4
**Category:** System
**Tags:** Tracking, Level Completion, Game Progression, Events
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** BrickTracker system with level completion detection
**Game Context:** Breakout-style game requiring accurate brick counting and level completion detection for game progression and player feedback

**Purpose:** Implements system to track remaining bricks through destruction event subscription, detects level completion when all destroyable bricks are eliminated, and provides foundation for game progression and level management systems.
**Complexity:** Medium complexity - 45 minutes (event subscription with tracking state management)

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
public class BrickTracker : MonoBehaviour
{
    [Header("Tracking Configuration")]
    [SerializeField] private bool trackAllBricks = true;
    [SerializeField] private bool enableCompletionLogging = true;
    [SerializeField] private float completionCheckDelay = 0.1f;
    
    [Header("Current Tracking State")]
    [SerializeField] private int totalBrickCount = 0;
    [SerializeField] private int remainingBrickCount = 0;
    [SerializeField] private int destroyedBrickCount = 0;
    [SerializeField] private bool levelCompleted = false;
    
    // Events for level completion communication
    [System.Serializable]
    public class LevelCompletedEvent : UnityEvent<int, int> { } // remaining, destroyed
    
    public LevelCompletedEvent OnLevelCompleted;
    public static System.Action<int, int> GlobalLevelCompleted;
    
    private BrickCollisionEvents eventSystem;
    private HashSet<GameObject> trackedBricks;
    private Coroutine completionCheckCoroutine;
    
    // Core tracking methods
    public void InitializeBrickTracking()
    public void OnBrickDestroyed(BrickEventData eventData)
    private bool ValidateLevelCompletion()
    private void FireLevelCompletionEvent()
    
    // Validation and error recovery
    private void ValidateBrickCount()
    private void RecoverFromCountingErrors()
}
```

### **Core Logic:**

- Event subscription to brick destruction events for accurate counting
- Level completion detection when all destroyable bricks are eliminated
- Brick counting validation and error recovery for missed or duplicate events
- Level completion event firing for communication with game state systems
- Delayed completion checking to handle rapid multiple destructions

### **Dependencies:**

- Brick destruction event integration from Task 1.2.3.3 for destruction notifications
- BrickCollisionEvents system for event subscription
- Existing Brick system for initial brick counting

### **Performance Constraints:**

- Efficient brick counting without frame-by-frame scene traversal
- Minimal memory allocation during tracking operations
- No performance impact during rapid brick destruction scenarios

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - focus purely on tracking and completion detection
- Keep tracking methods focused on counting without game state management
- Use event-driven architecture for loose coupling with other systems
- Implement validation and error recovery for robust tracking

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** BrickTracker GameObject with tracking component
**Scene Hierarchy:** Level management container for tracking system organization
**Inspector Config:** Tracking configuration and current state display with organized sections
**System Connections:** Subscribes to destruction events, communicates with future game state management systems

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering tracking approach and completion detection strategy)
2. **Code Files** (complete BrickTracker system with level completion detection)
3. **Editor Setup Script** (creates tracking system and configures with existing bricks)
4. **Integration Notes** (explanation of tracking accuracy and completion event benefits)

**File Structure:** `Assets/Scripts/Level/BrickTracker.cs`
**Code Standards:** Unity C# conventions, comprehensive tracking documentation, organized Inspector display

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/Task1234CreateBrickTrackingSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Brick Tracking System"`

**Class Pattern:** `CreateBrickTrackingSetup` (static class)

**Core Functionality:**

- Validate brick destruction event integration exists (call Task 1.2.3.3 setup if needed)
- Create BrickTracker GameObject with tracking component
- Initialize brick counting with existing bricks in scene
- Set up event subscription to destruction events
- Test tracking accuracy with sample destruction scenarios

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateBrickTrackingSetup
{
    [MenuItem("Breakout/Setup/Create Brick Tracking System")]
    public static void CreateBrickTrackingSystem()
    {
        // Validate prerequisites and call setup if needed
        // Create BrickTracker GameObject and configure component
        // Initialize brick counting and event subscription
        // Test tracking accuracy and completion detection
        Debug.Log("✅ Brick Tracking System created successfully");
    }

    [MenuItem("Breakout/Setup/Create Brick Tracking System", true)]
    public static bool ValidateCreateBrickTrackingSystem()
    {
        return Object.FindFirstObjectByType<BrickTracker>() == null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log tracking system creation success with initial brick count and configuration
- Handle missing event system with clear setup instructions
- Validate brick counting accuracy and provide debugging guidance
- Report level completion detection testing results

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate console output showing brick tracking system capabilities and initial brick count
- Provide level completion detection explanation and event communication benefits
- List tracking validation results and accuracy confirmation

### **Documentation:**

- Create brief .md file capturing:
    - Brick tracking system purpose and game progression benefits
    - Level completion detection methodology and accuracy validation
    - Event subscription patterns and error recovery mechanisms
    - Integration guidance for future game state management systems

### **Custom Instructions:**

- Include brick counting validation with scene traversal verification
- Add level completion testing with various brick destruction scenarios
- Provide clear debugging output for tracking accuracy and completion detection

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] System accurately tracks remaining brick count through destruction event subscription
- [ ] Level completion is detected correctly when all destroyable bricks are eliminated
- [ ] Level completion events provide necessary data for future game state and progression systems
- [ ] Brick counting remains accurate even with rapid multiple destructions or edge case scenarios

### **Integration Tests:**

- [ ] Tracking system subscribes to destruction events from Task 1.2.3.3 correctly
- [ ] Level completion detection works with various brick arrangements and destruction patterns
- [ ] Event firing integrates properly with UnityEvent system for Inspector subscription

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] Tracking system focused on counting and completion detection only
- [ ] All tracking methods have XML documentation
- [ ] Robust error recovery ensures tracking accuracy in edge cases

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** True - Create minimal tracking functionality if event system unavailable
**ValidationLevel:** Strict - Include comprehensive tracking validation and error recovery
**Reusability:** Reusable - Design tracking system for different level configurations and brick arrangements

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache brick references during initialization to avoid repeated scene searches
- Use HashSet for efficient brick tracking without duplicate counting
- Apply coroutines for delayed completion checking to handle timing issues
- Implement proper event subscription cleanup in OnDestroy
- Use SerializeField for inspector visibility of tracking state

### **Performance Requirements:**

- Efficient brick counting without frame-by-frame scene traversal operations
- Minimal memory allocation during tracking and completion detection
- No performance impact during rapid multiple brick destruction scenarios

### **Architecture Pattern:**

Observer pattern for event subscription with tracking state management and validation

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If brick destruction events missing:** Create minimal tracking with scene-based brick counting and log setup instructions
- **If BrickCollisionEvents unavailable:** Use alternative tracking methods with manual brick monitoring
- **If existing brick system missing:** Create placeholder tracking functionality for testing

**Fallback Behaviors:**

- Use scene-based brick counting when event subscription unavailable
- Log informative warnings for tracking accuracy issues with clear resolution steps
- Provide basic completion detection even when advanced event communication unavailable