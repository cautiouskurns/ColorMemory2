# **Unity C# Implementation Task: Scene Canvas Hierarchy Setup** *(45 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.1.1.1
**Category:** System
**Tags:** UI, Foundation, WebGL
**Priority:** Critical

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** Unity Canvas UI foundation system
**Game Context:** Color Memory - Memory puzzle game requiring responsive WebGL UI foundation

**Purpose:** Establishes the fundamental UI rendering foundation that all game interface elements will build upon, ensuring proper WebGL compatibility and responsive design across different screen sizes.
**Complexity:** Low complexity foundational setup with 45-minute implementation estimate

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// No custom classes required - this task focuses on Unity component configuration
// Primary deliverable is Editor Setup Script for Canvas creation and configuration
```

### **Core Logic:**

Canvas hierarchy establishment with proper component configuration:
- Screen Space - Overlay rendering for WebGL compatibility
- Canvas Scaler with Scale With Screen Size for responsive design
- GraphicRaycaster for UI interaction detection
- Standard WebGL canvas dimensions and aspect ratio handling

### **Dependencies:**

- Clean Unity scene (no additional systems required)
- Unity UI package (included by default)
- No fallback strategies needed for this foundational task

### **Performance Constraints:**

- Target 60fps WebGL performance
- Minimal overhead Canvas configuration
- Efficient UI rendering setup for browser deployment

### **Architecture Guidelines:**

- Follow Unity UI best practices for WebGL deployment
- Establish clean hierarchy structure matching TDS specification
- Configure components for optimal WebGL performance
- Use standard Unity UI patterns for Canvas setup

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Single Canvas GameObject as root UI container with Canvas, Canvas Scaler, and GraphicRaycaster components
**Scene Hierarchy:** Canvas as root-level UI container, positioned to match TDS hierarchy structure
**Inspector Config:** Canvas render mode, Canvas Scaler settings, GraphicRaycaster configuration
**System Connections:** Foundation for GameGrid system and future UI elements (ScoreText, LevelText, RestartButton)

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (Editor Setup Script only - no custom gameplay scripts needed)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** `Assets/Editor/Setup/111Create[Clean Task Name]Setup.cs`
**Code Standards:** Unity conventions, clear documentation, proper error handling

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/111CreateCanvasHierarchySetup.cs`

**Menu Structure:** `"Color Memory/Setup/Create Canvas Hierarchy"`

**Class Pattern:** `CreateCanvasHierarchySetup` (static class)

**Core Functionality:**

- Create Canvas GameObject with proper naming
- Add Canvas component with Screen Space - Overlay render mode
- Add Canvas Scaler component with Scale With Screen Size configuration
- Add GraphicRaycaster component for UI interaction
- Configure Canvas Scaler reference resolution for WebGL
- Set proper sorting order and layer configuration
- Position Canvas in scene hierarchy as root UI container
- Validate component configuration matches TDS specifications

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class CreateCanvasHierarchySetup
{
    [MenuItem("Color Memory/Setup/Create Canvas Hierarchy")]
    public static void CreateCanvasHierarchy()
    {
        // Validation and Canvas GameObject creation
        // Canvas component configuration for WebGL
        // Canvas Scaler setup for responsive design
        // GraphicRaycaster attachment and configuration
        Debug.Log("✅ Canvas Hierarchy created successfully");
    }

    [MenuItem("Color Memory/Setup/Create Canvas Hierarchy", true)]
    public static bool ValidateCreateCanvasHierarchy()
    {
        // Return false if Canvas already exists
        return GameObject.Find("Canvas") == null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with Canvas configuration details
- Validate Canvas doesn't already exist to prevent duplicates
- Confirm all required components are properly attached and configured
- Provide actionable error messages for configuration failures

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing Canvas configuration
- Provide next steps for GameGrid integration and UI element attachment
- Include validation checklist for proper Canvas setup

### **Documentation:**

- Create brief .md file capturing Canvas setup process
- Document component configuration details and rationale
- Include usage instructions for future UI element integration
- Document Editor setup script usage and validation

### **Custom Instructions:**

- Include validation logging for each component configuration step
- Provide clear indication of Canvas Scaler settings for WebGL deployment
- Document proper hierarchy positioning for TDS compliance

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Canvas GameObject exists in scene hierarchy
- [ ] Canvas component configured for WebGL target platform
- [ ] Canvas Scaler properly configured for responsive scaling
- [ ] UI hierarchy matches TDS specification structure
- [ ] GraphicRaycaster component enabled for button click detection
- [ ] Canvas render mode set to Screen Space - Overlay
- [ ] Proper sorting order and layer configuration established

### **Integration Tests:**

- [ ] Canvas serves as proper foundation for future GameGrid integration
- [ ] Canvas Scaler responds appropriately to different WebGL canvas sizes
- [ ] GraphicRaycaster properly detects UI interaction events

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity UI best practices for WebGL deployment
- [ ] Canvas configuration optimized for 60fps performance target
- [ ] Editor Setup Script handles validation and prevents duplicate creation

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** None - no external dependencies for this foundational task
**ValidationLevel:** Basic - validate Canvas existence and component attachment
**Reusability:** OneOff - specific Canvas configuration for Color Memory game

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use Screen Space - Overlay for consistent WebGL rendering
- Configure Canvas Scaler for responsive design across devices
- Minimize Canvas component overhead for optimal performance
- Follow Unity UI hierarchy patterns for maintainable scene structure
- Use proper GameObject naming conventions matching TDS specification

### **Performance Requirements:**

- Target 60fps WebGL performance with minimal Canvas overhead
- Efficient UI rendering configuration for browser deployment
- Responsive scaling without performance degradation

### **Architecture Pattern:**

- Unity UI foundation pattern with Canvas as root container
- Standard Unity component configuration approach
- Clean hierarchy structure supporting future UI system integration

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If Unity UI package is missing:** Log clear error with package installation instructions
- **If scene is not clean:** Warn about existing Canvas and provide merge strategy
- **If WebGL build target not set:** Provide guidance for platform configuration

**Fallback Behaviors:**

- Use default Canvas settings if specific configurations fail
- Log informative warnings for suboptimal configuration
- Gracefully handle missing Unity UI components with clear error messages

---