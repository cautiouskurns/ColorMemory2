# **Unity C# Implementation Task: Death Zone Positioning System** *(45 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.3.2.2
**Category:** System
**Tags:** Positioning, Paddle Integration, Resolution Scaling, Adaptive
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** DeathZonePositioning MonoBehaviour with adaptive placement system
**Game Context:** Breakout game requiring death zone that adapts position relative to paddle location across different screen resolutions

**Purpose:** Implements adaptive positioning system that places death zone relative to paddle location with screen resolution support, maintaining consistent gameplay balance.
**Complexity:** Medium - requires paddle integration, resolution scaling calculations, and position update system

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
[System.Serializable]
public class DeathZonePositioning : MonoBehaviour
{
    [Header("Configuration")]
    public DeathZoneConfig config;
    
    [Header("Paddle Integration")]
    public Transform paddleTransform;
    public float paddleOffset = -2.0f;
    
    [Header("Resolution Adaptation")]
    public bool adaptToResolution = true;
    public Vector2 referenceResolution = new Vector2(1920f, 1200f);
    
    [Header("Runtime Data")]
    public Vector3 currentPosition;
    public Vector2 currentScreenSize;
    
    // Positioning calculation methods
    // Resolution adaptation system
    // Paddle-relative placement logic
    // Position update handling
}
```

### **Core Logic:**

- Paddle-relative positioning system that places death zone below paddle area consistently
- Screen resolution adaptation that maintains death zone placement across different aspect ratios
- Position update system that responds to paddle movement and screen resolution changes
- Positioning validation to ensure death zone covers appropriate area without gameplay interference

### **Dependencies:**

- DeathZoneConfig from Task 1.3.2.1 (required)
- Paddle positioning system (can be stubbed if missing)
- Unity Transform system for positioning
- **Fallback Strategy:** Create basic positioning if paddle system missing, use default screen dimensions

### **Performance Constraints:**

- Efficient position updates triggered by events, not continuous polling
- Minimal computational overhead during paddle movement
- Resolution change detection without continuous monitoring

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - focus only on positioning logic
- Keep positioning system separate from trigger detection
- Only implement position calculation and update functionality
- Use Component pattern with MonoBehaviour for Unity integration

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** DeathZonePositioning component attached to death zone management GameObject
**Scene Hierarchy:** Positioning system organized under death zone parent container
**Inspector Config:** DeathZonePositioning MonoBehaviour with [Header] attributes for positioning settings
**System Connections:** Integrates with paddle system for relative placement, uses DeathZoneConfig for parameters

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** 
- `Assets/Scripts/DeathZone/DeathZonePositioning.cs` - Main positioning system MonoBehaviour
- Dependencies on DeathZoneConfig from Task 1.3.2.1

**Code Standards:** Unity C# naming conventions, XML documentation for public methods, [Header] attributes for Inspector organization

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1322CreateDeathZonePositioningSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Death Zone Positioning"`

**Class Pattern:** `CreateDeathZonePositioningSetup` (static class)

**Core Functionality:**

- Create "Death Zone System" parent GameObject for organization
- Add DeathZonePositioning MonoBehaviour component
- Configure positioning parameters for Breakout gameplay
- Set up paddle reference connection (or stub if missing)
- Configure resolution adaptation settings
- Position death zone system appropriately in scene hierarchy

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateDeathZonePositioningSetup
{
    [MenuItem("Breakout/Setup/Create Death Zone Positioning")]
    public static void CreateDeathZonePositioning()
    {
        // Check for prerequisite DeathZoneConfig
        // Create death zone system GameObject
        // Add DeathZonePositioning component
        // Configure positioning parameters
        // Set up paddle reference or stub
        // Configure resolution adaptation
        Debug.Log("✅ Death Zone Positioning created successfully");
    }

    [MenuItem("Breakout/Setup/Create Death Zone Positioning", true)]
    public static bool ValidateCreateDeathZonePositioning()
    {
        // Return false if positioning system already exists
        // Validate DeathZoneConfig prerequisite exists
        return GameObject.Find("Death Zone System") == null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with specific details
- Handle missing DeathZoneConfig by creating default or showing clear error
- Validate positioning system creation and configuration completed successfully
- Provide actionable instructions for connecting paddle reference

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing positioning system setup
- Provide instructions on how to connect paddle reference and test positioning
- Explain resolution adaptation behavior and configuration options

### **Documentation:**

- Create brief .md file capturing positioning calculations and paddle integration
- Document resolution adaptation methodology and configuration parameters
- Include troubleshooting guide for positioning issues

### **Custom Instructions:**

- Include position update system that responds to paddle movement events
- Add resolution change detection with automatic position recalculation
- Implement position validation to ensure appropriate death zone coverage

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Death zone position adapts correctly to paddle location changes during gameplay
- [ ] Positioning system maintains consistent placement across different screen resolutions and aspect ratios
- [ ] Death zone area coverage provides appropriate gameplay balance without being too punishing or too forgiving
- [ ] Positioning updates efficiently without performance impact during paddle movement

### **Integration Tests:**

- [ ] Paddle movement updates death zone position correctly in real-time
- [ ] Screen resolution changes trigger appropriate position recalculation

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] DeathZonePositioning class is focused on positioning logic only
- [ ] Proper event-driven position updates without continuous polling

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** true - create positioning system with paddle stub if paddle missing
**ValidationLevel:** Basic - validate positioning calculations and paddle integration
**Reusability:** Reusable - positioning system works with any paddle configuration

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache Transform component references during initialization
- Use event-driven updates instead of Update() method for position changes
- Calculate positions using Screen.width and Screen.height for resolution adaptation
- Position GameObjects using Transform.position for precise placement

### **Performance Requirements:**

- Efficient position updates triggered by events, not continuous polling
- Minimal computational overhead during positioning calculations
- Resolution change detection without frame-by-frame monitoring

### **Architecture Pattern:**

Component pattern with MonoBehaviour for Unity integration, event-driven position updates

## **DEPENDENCY HANDLING**

**Missing Dependencies:**
- **If DeathZoneConfig is missing:** Log clear error and provide instructions to run Task 1.3.2.1 setup first
- **If Paddle GameObject missing:** Create positioning stub that can be connected when paddle is implemented
- **If Resolution scaling system missing:** Use basic screen dimension calculations

**Fallback Behaviors:**

- Use default positioning parameters if configuration is incomplete
- Log informative warnings for missing paddle reference with connection instructions
- Create functional positioning system even if paddle integration unavailable

---