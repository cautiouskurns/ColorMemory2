# **Unity C# Implementation Task: Brick Prefab Assembly** *(40 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.2.1.7
**Category:** System
**Tags:** Prefab Creation, Configuration, Deployment
**Priority:** Critical

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** Complete brick prefab with all components configured for grid deployment
**Game Context:** Breakout arcade game requiring standardized brick prefabs for grid generation system

**Purpose:** Packages the complete brick system into a configured prefab that can be instantiated by the grid generation system, ensuring all components are properly set up with appropriate default values for immediate deployment.
**Complexity:** Low - prefab assembly and configuration with existing components

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// No new classes - prefab assembly and configuration only
// Prefab should include all components from previous tasks:
// - Brick (MonoBehaviour script)
// - SpriteRenderer (visual representation)
// - BoxCollider2D (collision detection) 
// - ParticleSystem (destruction effects)
// - AudioSource (destruction audio)
```

### **Core Logic:**

- Prefab creation with complete component configuration and default values
- Physics layer assignment matching collision system requirements ("Bricks" layer)
- Default BrickData configuration for Normal brick type with standard values
- Component reference assignment ensuring all systems are properly connected
- Prefab validation ensuring immediate usability without additional setup

### **Dependencies:**

- All previous brick system tasks completed (Tasks 1.2.1.1-1.2.1.6)
- Complete Brick MonoBehaviour with all functionality integrated
- Unity prefab system and component configuration tools
- If any components missing: Create minimal working configuration with clear upgrade path

### **Performance Constraints:**

- Optimized prefab settings for efficient instantiation and runtime performance
- Minimal memory footprint per brick instance
- Fast instantiation suitable for grid generation of multiple bricks

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - prefab template for brick deployment
- Keep prefab configuration complete and self-contained
- Only include components and settings explicitly required by specification
- Avoid adding extra components or configurations not needed for basic functionality

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Creates configured brick prefab with all required components
**Scene Hierarchy:** Prefab ready for instantiation under BrickGrid parent containers
**Inspector Config:** All components properly configured with appropriate default settings
**System Connections:** Integrated with physics layer system and ready for CollisionManager coordination

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering prefab creation, component assembly, configuration setup, validation, and deployment preparation)
2. **Code Files** (No new code files - prefab configuration only)
3. **Editor Setup Script** (creates complete brick prefab with all components configured)
4. **Integration Notes** (explanation of how prefab supports grid generation and brick deployment)

**File Structure:** `Assets/Prefabs/Gameplay/Brick.prefab` (new prefab asset)
**Code Standards:** Unity prefab conventions, complete component configuration, organized Inspector layout

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1217CreateBrickPrefabSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Brick Prefab"`

**Class Pattern:** `CreateBrickPrefabSetup` (static class)

**Core Functionality:**

- Create new GameObject with all required components for brick functionality
- Add and configure Brick MonoBehaviour with default BrickData settings
- Add SpriteRenderer with default sprite and material configuration
- Add BoxCollider2D with proper size and physics layer assignment
- Add ParticleSystem with destruction effect configuration
- Add AudioSource with destruction audio settings
- Save as prefab asset in proper location with validation
- Test prefab instantiation and component functionality

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output showing prefab creation and component configuration
- List all components added to prefab and their settings
- Provide usage instructions for instantiating and using the brick prefab
- Include validation steps for verifying prefab functionality

### **Documentation:**

- Create brief .md file capturing:
  - Brick prefab component architecture and configuration
  - Default settings and their purposes for each component
  - Usage instructions for grid generation system integration
  - Customization options for different brick types and behaviors

### **Custom Instructions:**

- Add prefab validation tools to verify all components are properly configured
- Include prefab testing utilities for instantiation and functionality verification
- Create prefab upgrade tools for future component additions

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Brick prefab includes all required components properly configured
- [ ] Physics layer assignment matches collision system requirements
- [ ] Visual components are configured with appropriate default settings
- [ ] Prefab can be instantiated and works immediately without additional setup
- [ ] All component references and configurations are properly serialized

### **Integration Tests:**

- [ ] Prefab instantiation creates fully functional brick GameObject
- [ ] All components are properly connected and configured
- [ ] Physics layer and collision detection work correctly
- [ ] Visual and audio effects trigger properly on destruction

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity prefab best practices and organization
- [ ] Prefab is optimized for performance and memory usage
- [ ] Complete component configuration enables immediate deployment

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** Basic - create minimal working prefab if some components unavailable
**ValidationLevel:** Strict - validate all component configurations and prefab functionality
**Reusability:** Reusable - prefab should work across different scenes and grid configurations

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use consistent naming conventions for prefab and components
- Configure all components with appropriate default values
- Ensure prefab is self-contained with no external dependencies
- Use proper layer assignments for physics and rendering
- Organize Inspector with [Header] attributes for clear component sections

### **Performance Requirements:**

- Prefab instantiation completes efficiently for grid generation
- Optimized component settings for runtime performance
- Minimal memory usage per brick instance

### **Architecture Pattern:**

- Prefab template pattern with complete component configuration for immediate deployment

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If Brick MonoBehaviour not complete:** Create prefab with placeholder script and clear upgrade instructions
- **If visual assets missing:** Use Unity primitive sprites as placeholders with asset assignment guidance
- **If audio clips missing:** Configure AudioSource with placeholder clips and assignment instructions

**Fallback Behaviors:**

- Create functional prefab even with missing optional components
- Log clear instructions for completing prefab configuration
- Ensure prefab works for basic functionality with upgrade path for full features