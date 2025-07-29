# **Unity C# Implementation Task: Ball Launch Mechanics** *(90 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.1.1.5
**Category:** System
**Tags:** Physics, Launch, Input, Gameplay
**Priority:** Critical

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** BallController launch mechanics enhancement with state management and directional control
**Game Context:** Breakout arcade game requiring intuitive ball launch system for player control and game flow

**Purpose:** Provide players with meaningful control over ball launch direction and timing, creating engaging arcade gameplay with predictable trajectory control and seamless integration with paddle mechanics
**Complexity:** Medium complexity state machine system with input integration and physics coordination (90 minute implementation)

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
public enum BallLaunchState 
{ 
    Ready, 
    Launching, 
    InPlay 
}

public class BallController : MonoBehaviour
{
    [Header("Launch Mechanics")]
    [SerializeField] private BallLaunchState currentState = BallLaunchState.Ready;
    [SerializeField] private float launchAngleRange = 60f;
    [SerializeField] private Vector2 defaultLaunchDirection = Vector2.up;
    [SerializeField] private Transform paddleTransform;
    [SerializeField] private float paddleOffset = 0.5f;
    
    private Vector2 launchDirection;
    private bool isReadyToLaunch = true;
    
    // Core launch methods
    private void Update() // Input polling for launch trigger
    private void HandleLaunchInput() // Spacebar detection and launch triggering
    private void CalculateLaunchDirection() // Direction calculation based on paddle position
    private void ExecuteLaunch() // Launch execution with velocity integration
    private void TransitionToState(BallLaunchState newState) // State machine transitions
    private void PositionOnPaddle() // Ball positioning relative to paddle during ready state
    private Vector2 GetLaunchVector() // Launch vector calculation with angle range
}
```

### **Core Logic:**

- **Launch State Machine:** State transitions between Ready (positioned on paddle) → Launching (direction calculation) → InPlay (normal physics) with proper validation
- **Directional Control:** Launch angle calculation based on paddle position, input timing, and configurable angle range providing meaningful player control
- **Input Integration:** Update() method polling for Input.GetKeyDown(KeyCode.Space) trigger as specified in GDD controls
- **Paddle Integration:** Transform positioning system keeping ball relative to paddle during ready state with configurable offset
- **Launch Vector Calculation:** Computed launch direction with angle range constraints and default fallback direction
- **Velocity Coordination:** Integration with velocity management system ensuring consistent launch speed and physics behavior

### **Dependencies:**

- **Velocity management system:** Requires Task 1.1.1.4 implementation for consistent launch speed application
- **Paddle positioning:** Requires paddle Transform reference for launch positioning and angle calculation
- **BallController foundation:** Enhanced version of existing BallController with state management integration
- **Input system:** Unity's legacy Input system for spacebar detection (Input.GetKeyDown)

### **Performance Constraints:**

- **60fps WebGL target:** Input polling and state transitions must not impact frame rate
- **Minimal allocations:** Avoid Vector2 allocations during launch calculations, cache direction vectors
- **Response time:** <50ms from spacebar press to launch execution for responsive gameplay
- **State efficiency:** Lightweight state machine with minimal overhead during InPlay state

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - launch mechanics focused solely on state management and directional control
- Keep launch state machine methods focused and appropriately sized
- Only implement launch functionality as specified - avoid trajectory prediction or advanced launch features
- Separate launch state enumeration into dedicated file for clear organization

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Enhance existing Ball GameObject with launch state management, requires paddle Transform reference assignment
**Scene Hierarchy:** Launch mechanics integrated into BallController under GameArea/Ball, requires connection to GameArea/Paddle
**Inspector Config:** Launch parameters exposed with [Header("Launch Mechanics")] organization, launchAngleRange (60f), defaultLaunchDirection (Vector2.up), paddleOffset (0.5f), and paddleTransform reference slot
**System Connections:** Integration with paddle positioning for launch point calculation, input handling for spacebar trigger, velocity management for launch speed application, and state management for game flow control

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** 
- `Assets/Scripts/Ball/BallLaunchState.cs` - Launch state enumeration and transition logic
- `Assets/Scripts/Ball/BallController.cs` - Enhanced with launch state machine and directional control methods

**Code Standards:** Unity C# naming conventions, [Header] attributes for Inspector organization, XML documentation for public methods, proper state machine implementation

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1115CreateBallLaunchMechanicsSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Ball Launch Mechanics"`

**Class Pattern:** `CreateBallLaunchMechanicsSetup` (static class)

**Core Functionality:**

- Validate existing Ball GameObject with BallController component exists
- Enhance BallController with launch mechanics functionality
- Create and assign BallLaunchState enumeration
- Configure launch parameters per technical specifications
- Locate and assign paddle Transform reference automatically
- Set default values for launchAngleRange (60f), paddleOffset (0.5f), defaultLaunchDirection (Vector2.up)
- Handle missing Ball GameObject or Paddle gracefully with clear error messages
- Prevent duplicate setup with validation MenuItem
- Call prerequisite velocity management setup if missing

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateBallLaunchMechanicsSetup
{
    [MenuItem("Breakout/Setup/Create Ball Launch Mechanics")]
    public static void CreateBallLaunchMechanics()
    {
        // Validation and prerequisite calls
        // Ball GameObject validation and enhancement
        // Paddle reference assignment
        // Launch mechanics configuration
        // State machine initialization
        Debug.Log("✅ Ball Launch Mechanics created successfully");
    }

    [MenuItem("Breakout/Setup/Create Ball Launch Mechanics", true)]
    public static bool ValidateCreateBallLaunchMechanics()
    {
        // Return false if launch mechanics already configured
        return [validation logic];
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with specific launch mechanics details
- Handle missing Ball GameObject by providing setup instructions
- Handle missing Paddle GameObject by creating placeholder or providing instructions
- Validate BallController component exists and has velocity management before enhancement
- Provide actionable error messages for failed paddle reference assignment

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing launch mechanics implementation
- Provide instructions on testing launch direction control in play mode
- Include next steps for integrating with game state management and level progression
- Document spacebar input testing procedures

### **Documentation:**

- Create brief .md file capturing launch mechanics implementation details
- Document launch state machine transitions and directional control algorithm
- Include usage instructions for launch parameters and paddle integration
- Provide editor setup script usage guidelines and testing procedures

### **Custom Instructions:**

- Implement launch debugging tools for monitoring state transitions
- Add console logging for launch direction calculations and input detection
- Create validation methods for paddle reference and launch readiness

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Ball launches from paddle in predictable directions based on input
- [ ] Launch direction calculation provides meaningful player control
- [ ] Launch state management properly transitions between different ball states
- [ ] Launch mechanics integrate seamlessly with velocity management system
- [ ] Launch input handling responds to spacebar trigger as specified in GDD
- [ ] Ball positioning relative to paddle works correctly during launch preparation

### **Integration Tests:**

- [ ] Integration with paddle positioning for launch point calculation
- [ ] Integration with input handling for spacebar trigger detection
- [ ] Integration with velocity management for consistent launch behavior
- [ ] Integration with state management for game flow control

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] Launch state machine is focused and appropriately sized
- [ ] Minimal garbage collection during launch operations
- [ ] State transitions work consistently across WebGL builds

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** Create paddle Transform stub and velocity management interface if missing dependencies
**ValidationLevel:** Strict - comprehensive state validation and input handling edge cases
**Reusability:** Reusable - launch mechanics should work for any ball-paddle physics game

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache Transform and component references during initialization
- Use enum-based state machine for clear state management
- No hard-coded magic numbers (use serialized fields for launch parameters)
- Minimize garbage collection during Update() and input polling
- Use Input.GetKeyDown for single-frame input detection
- Implement proper null checks for paddle Transform reference

### **Performance Requirements:**

- 60fps WebGL target with launch mechanics active
- <5ms execution time for launch direction calculations
- Minimal Vector2 allocations during launch operations
- Efficient state machine transitions without performance spikes

### **Architecture Pattern:**

- State machine pattern for launch phase management with clear state transitions
- Component enhancement pattern for extending existing BallController functionality

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If Velocity Management System is missing:** Create basic velocity application interface with launch speed setting
- **If Paddle GameObject is missing:** Create placeholder paddle or provide clear setup instructions
- **If BallController is missing:** Create minimal stub with Rigidbody2D integration and basic state management
- **If Input system is missing:** Log clear error with input configuration instructions

**Fallback Behaviors:**

- Return safe default launch directions if paddle reference is missing
- Log informative warnings for missing paddle positioning and provide fallback launch behavior
- Gracefully handle state transitions rather than throwing exceptions during invalid states

---