# **Unity C# Implementation Task: Scoring System Integration** *(40 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.3.2.6
**Category:** System
**Tags:** Scoring, Penalties, Bonuses, System Integration
**Priority:** Medium

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** DeathZoneScoring MonoBehaviour with scoring integration and penalty system
**Game Context:** Breakout game requiring death zone integration with scoring mechanics for penalties, bonuses, and score calculations to enhance gameplay balance

**Purpose:** Integrates death zone system with scoring mechanics for potential penalties, bonuses, or score calculations, responding to death zone events with appropriate score modifications.
**Complexity:** Medium - requires scoring system integration, penalty calculations, bonus opportunities, and event-driven score updates

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
[System.Serializable]
public class DeathZoneScoring : MonoBehaviour
{
    [Header("Configuration")]
    public DeathZoneConfig config;
    
    [Header("Penalty Settings")]
    public int ballLossPenalty = 100;
    public bool enablePenalties = true;
    public float penaltyMultiplier = 1.0f;
    
    [Header("Bonus Opportunities")]
    public int nearMissBonus = 50;
    public int consecutiveSaveBonus = 25;
    public float nearMissDistance = 1.0f;
    public bool enableBonuses = true;
    
    [Header("Score Tracking")]
    public int consecutiveSaves = 0;
    public int totalBallLosses = 0;
    public float lastNearMissTime = 0f;
    
    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;
    public System.Action<int> ScoreChangeEvent;
    
    [Header("Debug")]
    public bool enableLogging = true;
    
    // Scoring integration methods
    // Penalty calculation system
    // Bonus scoring opportunities
    // Score event communication
}
```

### **Core Logic:**

- Implement scoring integration system that responds to death zone trigger events
- Add score penalty calculations for ball loss events with configurable penalty amounts
- Create bonus scoring opportunities based on consecutive saves or near-miss scenarios
- Include scoring event system that communicates with main scoring system for score updates

### **Dependencies:**

- DeathZoneTrigger events from Task 1.3.2.3 (required)
- Scoring system (can be stubbed if missing)
- DeathZoneConfig from Task 1.3.2.1 for penalty settings
- **Fallback Strategy:** Create scoring integration with stub scoring system if main scoring unavailable

### **Performance Constraints:**

- Efficient scoring calculations with minimal computational overhead
- Event-driven score updates without continuous polling
- Penalty and bonus calculations without frame rate impact

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - focus only on death zone scoring integration
- Keep scoring calculations separate from core scoring system implementation
- Only implement death zone-related score modifications and event communication
- Use Integration pattern with event-driven scoring system communication

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Scoring integration component attached to death zone or scoring management GameObject
**Scene Hierarchy:** Scoring integration organized under game management container
**Inspector Config:** DeathZoneScoring MonoBehaviour with [Header] attributes for scoring settings
**System Connections:** Subscribes to death zone trigger events and communicates with main scoring system

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** 
- `Assets/Scripts/DeathZone/DeathZoneScoring.cs` - Scoring integration and penalty system
- Integration with DeathZoneTrigger events from Task 1.3.2.3

**Code Standards:** Unity C# naming conventions, XML documentation for public methods, [Header] attributes for Inspector organization

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1326CreateDeathZoneScoringSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Death Zone Scoring"`

**Class Pattern:** `CreateDeathZoneScoringSetup` (static class)

**Core Functionality:**

- Find existing Death Zone System or Game Manager GameObject
- Add DeathZoneScoring MonoBehaviour component
- Configure penalty and bonus scoring settings
- Connect to DeathZoneTrigger events for score calculations
- Set up score communication with main scoring system
- Configure penalty amounts and bonus opportunities for balanced gameplay

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateDeathZoneScoringSetup
{
    [MenuItem("Breakout/Setup/Create Death Zone Scoring")]
    public static void CreateDeathZoneScoring()
    {
        // Check for prerequisite death zone trigger
        // Find or create scoring management GameObject
        // Add DeathZoneScoring component
        // Configure penalty and bonus settings
        // Connect death zone trigger events
        // Set up scoring system communication
        Debug.Log("✅ Death Zone Scoring created successfully");
    }

    [MenuItem("Breakout/Setup/Create Death Zone Scoring", true)]
    public static bool ValidateCreateDeathZoneScoring()
    {
        // Return false if scoring integration already exists
        // Validate death zone trigger prerequisite exists
        return FindObjectsOfType<DeathZoneScoring>().Length == 0;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with specific details
- Handle missing death zone trigger with clear error and setup instructions
- Validate scoring integration setup and event connections completed successfully
- Provide troubleshooting steps if score communication fails

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing scoring integration setup and penalty/bonus configuration
- Provide instructions on how to test penalty calculations and bonus opportunities
- Explain scoring system communication and integration points

### **Documentation:**

- Create brief .md file capturing scoring integration methodology and penalty system
- Document penalty calculation logic and bonus opportunity conditions
- Include testing procedures for score modifications and system communication

### **Custom Instructions:**

- Include near-miss detection system for bonus scoring opportunities
- Add consecutive save tracking for progressive bonus rewards
- Implement score modification methods with validation and boundary checking

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Scoring integration responds appropriately to death zone events with accurate penalty calculations
- [ ] Score penalties provide appropriate gameplay balance without being overly punishing
- [ ] Bonus scoring opportunities add positive reinforcement for skilled play near death zone
- [ ] Integration with scoring system maintains clean separation of concerns and loose coupling

### **Integration Tests:**

- [ ] Death zone trigger events properly calculate and apply score penalties
- [ ] Bonus scoring opportunities trigger correctly for near-miss and consecutive save scenarios

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] DeathZoneScoring class is focused on score integration only
- [ ] Proper event-driven integration with scoring system

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** true - create scoring integration with stub scoring system if main scoring unavailable
**ValidationLevel:** Basic - validate score calculations and penalty/bonus logic
**Reusability:** Reusable - scoring integration works with any death zone and scoring system configuration

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache component references during initialization
- Use UnityEvent for Inspector-assignable score event connections
- Implement proper event subscription and unsubscription in OnEnable/OnDisable
- Use System.Action for code-based score update events

### **Performance Requirements:**

- Efficient scoring calculations with minimal computational overhead
- Event-driven score updates without continuous polling
- Penalty and bonus calculations without frame rate impact

### **Architecture Pattern:**

Integration pattern with event-driven scoring system communication, loose coupling with main scoring system

## **DEPENDENCY HANDLING**

**Missing Dependencies:**
- **If DeathZoneTrigger missing:** Log clear error and provide instructions to run Task 1.3.2.3 setup first
- **If Main scoring system missing:** Create scoring integration with stub scoring system and log integration instructions
- **If DeathZoneConfig missing:** Create scoring integration with default penalty and bonus settings

**Fallback Behaviors:**

- Use default penalty and bonus settings if configuration is incomplete
- Log informative warnings for missing scoring system with integration instructions
- Create functional scoring integration even if main scoring system unavailable

---