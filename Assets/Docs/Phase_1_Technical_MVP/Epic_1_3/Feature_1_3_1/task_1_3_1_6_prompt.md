# **Unity C# Implementation Task: Resolution Scaling and Aspect Ratio Management** *(50 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.3.1.6
**Category:** System
**Tags:** Resolution, Scaling, Aspect Ratio, Adaptation
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** ResolutionScalingManager with aspect ratio maintenance
**Game Context:** Breakout game requiring consistent 16:10 aspect ratio gameplay across different screen resolutions while maintaining proper boundary positioning and game balance

**Purpose:** Implements resolution adaptation system that maintains 16:10 aspect ratio gameplay across different screen sizes, detecting resolution changes and updating boundary positioning accordingly.
**Complexity:** Medium - requires resolution detection, scaling calculations, aspect ratio enforcement, and dynamic boundary updates

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
[System.Serializable]
public class ResolutionScalingManager : MonoBehaviour
{
    [Header("Target Aspect Ratio")]
    public float targetAspectRatio = 1.6f; // 16:10 aspect ratio
    public Vector2 referenceResolution = new Vector2(1920f, 1200f);
    
    [Header("Scaling Configuration")]
    public bool maintainAspectRatio = true;
    public bool updateOnResolutionChange = true;
    
    [Header("System References")]
    public Camera gameCamera;
    public CameraBoundaryIntegration boundaryIntegration;
    
    [Header("Runtime Data")]
    public Vector2 currentResolution;
    public float currentAspectRatio;
    public float scaleFactor;
    
    // Resolution detection and scaling calculations
    // Boundary scaling logic with position adjustment
    // Aspect ratio enforcement system
    // Resolution change handling with updates
}
```

### **Core Logic:**

- Create ResolutionScalingManager that detects screen resolution and calculates scaling factors for 16:10 maintenance
- Implement boundary scaling logic that adjusts wall positions and dimensions based on resolution scaling
- Add aspect ratio enforcement system that ensures gameplay area maintains 16:10 regardless of screen dimensions
- Include resolution change handling that updates boundary system when screen size changes during gameplay

### **Dependencies:**

- CameraBoundaryIntegration from Task 1.3.1.4 (required)
- BoundaryWall system from Task 1.3.1.2 for boundary updates
- Main Camera for scaling calculations
- **Fallback Strategy:** Use default scaling if camera integration missing, maintain basic 16:10 enforcement

### **Performance Constraints:**

- Efficient scaling calculations with minimal computational overhead
- Update only when resolution changes, not every frame
- Preserve gameplay balance and ball physics consistency across resolutions

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - focus only on resolution and scaling management
- Keep scaling system separate from boundary creation logic
- Only implement resolution detection and scaling calculation functionality
- Use Manager pattern for centralized resolution and scaling management

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** ResolutionScalingManager GameObject with scaling management component
**Scene Hierarchy:** Scaling manager in root level for global resolution handling
**Inspector Config:** ResolutionScalingManager MonoBehaviour with [Header] attributes for scaling settings
**System Connections:** Updates boundary system when resolution changes, maintains 16:10 aspect ratio, integrates with camera bounds

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** 
- `Assets/Scripts/Resolution/ResolutionScalingManager.cs` - Resolution and scaling management system
- Integration with camera bounds and boundary positioning from Task 1.3.1.4

**Code Standards:** Unity C# naming conventions, XML documentation for public methods, [Header] attributes for Inspector organization

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1316CreateResolutionScalingSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Resolution Scaling"`

**Class Pattern:** `CreateResolutionScalingSetup` (static class)

**Core Functionality:**

- Create "Resolution Manager" GameObject in root scene hierarchy
- Add ResolutionScalingManager component with default 16:10 configuration
- Connect references to camera and boundary integration system
- Configure target resolution and aspect ratio for Breakout gameplay
- Set up resolution change detection and boundary update system
- Initialize scaling calculations for current screen resolution
- Configure camera scaling to maintain gameplay area proportions

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateResolutionScalingSetup
{
    [MenuItem("Breakout/Setup/Create Resolution Scaling")]
    public static void CreateResolutionScaling()
    {
        // Check for prerequisite camera integration system
        // Create resolution manager GameObject
        // Add ResolutionScalingManager component
        // Configure 16:10 aspect ratio settings
        // Connect camera and boundary system references
        // Initialize scaling for current resolution
        Debug.Log("✅ Resolution Scaling created successfully");
    }

    [MenuItem("Breakout/Setup/Create Resolution Scaling", true)]
    public static bool ValidateCreateResolutionScaling()
    {
        // Return false if resolution manager already exists
        // Validate camera integration prerequisite exists
        return GameObject.Find("Resolution Manager") == null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with specific details
- Handle missing camera integration with clear error and setup instructions
- Validate scaling calculations and boundary updates completed successfully
- Provide troubleshooting steps if aspect ratio enforcement fails

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing resolution scaling setup and current calculations
- Provide instructions on how to test resolution scaling with different screen sizes
- Explain aspect ratio maintenance system and its impact on gameplay

### **Documentation:**

- Create brief .md file capturing resolution scaling methodology and aspect ratio enforcement
- Document scaling calculations and boundary update procedures
- Include testing procedures for different screen resolutions

### **Custom Instructions:**

- Include resolution change detection system that responds to screen size changes
- Add validation methods to verify aspect ratio maintenance accuracy
- Implement camera orthographic size adjustment for consistent gameplay area

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Gameplay area maintains consistent 16:10 aspect ratio across different screen resolutions and sizes
- [ ] Boundary walls scale and position correctly to maintain proper game area dimensions
- [ ] Resolution changes during gameplay update boundary system without disrupting ball physics
- [ ] Scaling system preserves gameplay balance and ball physics consistency across all supported resolutions

### **Integration Tests:**

- [ ] Screen resolution changes trigger appropriate scaling calculations and boundary updates
- [ ] Different aspect ratio screens maintain 16:10 gameplay area with appropriate letterboxing or scaling

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] Resolution scaling system is focused on aspect ratio and scaling management only
- [ ] Proper integration with camera and boundary systems

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** true - create basic scaling system if camera integration missing
**ValidationLevel:** Strict - include comprehensive aspect ratio and scaling validation
**Reusability:** Reusable - scaling system works with any camera and boundary configuration

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use Camera.orthographicSize for 2D scaling adjustments
- Cache Screen.width and Screen.height for resolution change detection
- Update scaling only when resolution actually changes to avoid performance impact
- Use Time.deltaTime-independent calculations for consistent scaling

### **Performance Requirements:**

- Efficient scaling calculations without frame rate impact
- Resolution change detection without continuous polling
- Minimal computational overhead for aspect ratio maintenance

### **Architecture Pattern:**

Manager pattern for centralized resolution and scaling management with event-driven updates

## **DEPENDENCY HANDLING**

**Missing Dependencies:**
- **If CameraBoundaryIntegration missing:** Log clear error and provide instructions to run Task 1.3.1.4 setup first
- **If Main Camera missing:** Create default orthographic camera with appropriate 16:10 setup
- **If BoundaryWall system missing:** Create scaling system that can be connected when boundaries are available

**Fallback Behaviors:**

- Use default 16:10 scaling if camera integration configuration is incomplete
- Log informative warnings for scaling issues with suggested resolution
- Provide basic aspect ratio enforcement even if advanced boundary updates unavailable

---