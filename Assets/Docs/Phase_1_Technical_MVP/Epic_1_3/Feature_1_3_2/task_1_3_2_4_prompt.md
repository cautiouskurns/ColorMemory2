# **Unity C# Implementation Task: Life Management Integration** *(50 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.3.2.4
**Category:** System
**Tags:** Life Management, Game State, Game Over Detection, Event Integration
**Priority:** Critical

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** LifeManager MonoBehaviour with life tracking and game over detection
**Game Context:** Breakout game requiring life management system that responds to ball loss events and manages game over conditions

**Purpose:** Implements life reduction system that responds to death zone triggers and manages game over conditions, tracking player lives and handling life-related game state transitions.
**Complexity:** Medium - requires life tracking, event subscription, game over detection, and state management integration

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
[System.Serializable]
public class LifeManager : MonoBehaviour
{
    [Header("Life Configuration")]
    public DeathZoneConfig config;
    public int currentLives = 3;
    public int maxLives = 5;
    
    [Header("Game State Integration")]
    public bool gameOverTriggered = false;
    public bool autoRespawn = true;
    
    [Header("Events")]
    public UnityEvent OnLifeLost;
    public UnityEvent OnGameOver;
    public UnityEvent<int> OnLivesChanged;
    public System.Action<int> LifeChangedEvent;
    public System.Action GameOverEvent;
    
    [Header("Debug")]
    public bool enableLogging = true;
    
    // Life tracking and management methods
    // Death zone event subscription system
    // Game over detection and handling
    // UI integration points
}
```

### **Core Logic:**

- Implement life tracking system with configurable starting lives and life reduction logic
- Add death zone event subscription system that responds to ball loss triggers
- Create game over detection that triggers when lives reach zero with appropriate state management
- Include life state persistence and UI integration points for life display updates

### **Dependencies:**

- DeathZoneTrigger events from Task 1.3.2.3 (required)
- DeathZoneConfig from Task 1.3.2.1 for life settings
- Game state management system (can be stubbed)
- **Fallback Strategy:** Create life management with basic game state if advanced state management missing

### **Performance Constraints:**

- Efficient event handling with minimal computational overhead
- Life tracking without garbage collection during gameplay
- State management with minimal performance impact

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - focus only on life management
- Keep life tracking separate from game state implementation
- Only implement life counting, reduction, and game over detection
- Use Manager pattern with event-driven integration for loose coupling

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** LifeManager GameObject for centralized life management
**Scene Hierarchy:** Life manager in root level or game management container
**Inspector Config:** LifeManager MonoBehaviour with [Header] attributes for life tracking settings
**System Connections:** Subscribes to death zone trigger events, integrates with game state management and UI systems

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** 
- `Assets/Scripts/GameManagement/LifeManager.cs` - Main life management system
- Integration with DeathZoneTrigger events from Task 1.3.2.3

**Code Standards:** Unity C# naming conventions, XML documentation for public methods, [Header] attributes for Inspector organization

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1324CreateLifeManagerSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Life Manager"`

**Class Pattern:** `CreateLifeManagerSetup` (static class)

**Core Functionality:**

- Create "Game Manager" parent GameObject if it doesn't exist
- Add LifeManager MonoBehaviour component
- Configure starting lives and life management settings
- Connect to DeathZoneTrigger events for ball loss detection
- Set up UI integration points for life display
- Configure game over detection and state management

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateLifeManagerSetup
{
    [MenuItem("Breakout/Setup/Create Life Manager")]
    public static void CreateLifeManager()
    {
        // Check for prerequisite death zone trigger
        // Create or find Game Manager GameObject
        // Add LifeManager component
        // Configure life management settings
        // Connect death zone trigger events
        // Set up UI integration points
        Debug.Log("✅ Life Manager created successfully");
    }

    [MenuItem("Breakout/Setup/Create Life Manager", true)]
    public static bool ValidateCreateLifeManager()
    {
        // Return false if life manager already exists
        // Validate death zone trigger prerequisite exists
        return FindObjectsOfType<LifeManager>().Length == 0;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with specific details
- Handle missing death zone trigger with clear error and setup instructions
- Validate life manager creation and event subscription completed successfully
- Provide troubleshooting steps if event integration fails

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing life management setup and configuration
- Provide instructions on how to test life reduction and game over detection
- Explain event system integration and UI connection points

### **Documentation:**

- Create brief .md file capturing life management methodology and event integration
- Document life tracking behavior and game over detection logic
- Include testing procedures for life reduction and game over scenarios

### **Custom Instructions:**

- Include event subscription to DeathZoneTrigger.OnBallEnterDeathZone
- Add life reduction methods with validation and boundary checking
- Implement game over detection with configurable behavior

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Life reduction occurs immediately upon ball entering death zone with accurate life counting
- [ ] Game over detection triggers correctly when lives reach zero with proper state management
- [ ] Life management integrates cleanly with game state systems without tight coupling
- [ ] Life tracking provides consistent behavior across game sessions and respawn cycles

### **Integration Tests:**

- [ ] Death zone trigger events properly reduce lives in life manager
- [ ] Game over state triggers correctly when lives reach zero

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] LifeManager class is focused on life tracking and game over detection only
- [ ] Proper event-driven integration with loose coupling

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** true - create life manager with basic state management if advanced systems missing
**ValidationLevel:** Strict - include comprehensive life tracking validation and boundary checking
**Reusability:** Reusable - life management system works with any death zone configuration

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache component references during initialization
- Use UnityEvent for Inspector-assignable event connections
- Implement proper event subscription and unsubscription in OnEnable/OnDisable
- Use System.Action for code-based event subscriptions

### **Performance Requirements:**

- Efficient event handling with minimal computational overhead
- Life tracking without garbage collection during gameplay
- State change notifications without performance bottlenecks

### **Architecture Pattern:**

Manager pattern with event-driven integration for centralized life management and loose coupling

## **DEPENDENCY HANDLING**

**Missing Dependencies:**
- **If DeathZoneTrigger missing:** Log clear error and provide instructions to run Task 1.3.2.3 setup first
- **If DeathZoneConfig missing:** Create life manager with default life settings
- **If Game state management missing:** Create basic state management stub for game over handling

**Fallback Behaviors:**

- Use default life settings if configuration is incomplete
- Log informative warnings for missing death zone trigger with integration instructions
- Create functional life tracking even if advanced state management unavailable

---