# **Unity C# Implementation Task: Death Zone Configuration System** *(30 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.3.2.1
**Category:** System
**Tags:** Data Structures, Configuration, Death Zone, Life Management
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** DeathZoneConfig data structures and configuration system
**Game Context:** Breakout game requiring death zone detection system that triggers life loss when ball falls below paddle area

**Purpose:** Creates foundational data structures and configuration system for death zone setup and management, enabling consistent death zone behavior across different gameplay scenarios.
**Complexity:** Low - requires data structure design, serialization, and configuration management for death zone parameters

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// Main configuration structure
[System.Serializable]
public class DeathZoneConfig
{
    [Header("Trigger Dimensions")]
    public Vector2 triggerSize;
    public float detectionSensitivity;
    
    [Header("Positioning Parameters")]
    public float paddleOffset;
    public Vector2 positioningOffsets;
    
    [Header("Life Management")]
    public int startingLives;
    public int livesReduction;
    public bool enableGameOverDetection;
    
    [Header("Feedback Configuration")]
    public float audioVolume;
    public float feedbackDuration;
    public float effectIntensity;
}
```

### **Core Logic:**

- DeathZoneConfig structure contains trigger dimensions, positioning offsets, and detection sensitivity
- Life management configuration with starting lives, reduction amounts, and game over detection settings
- Positioning parameters for paddle-relative placement and screen resolution adaptation
- Feedback configuration for audio-visual effects timing and intensity settings

### **Dependencies:**

- Basic Unity project setup only
- Boundary system from Feature 1.3.1 (optional for positioning reference)
- **Fallback Strategy:** Use default values if boundary system configuration missing

### **Performance Constraints:**

- Lightweight data structures with minimal memory footprint
- Serializable for Inspector configuration and runtime adjustment
- No garbage collection during gameplay - all data pre-configured

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - data structures only handle configuration
- Keep DeathZoneConfig focused on death zone parameters without logic implementation
- Only implement fields and properties explicitly required by specification
- Use Data Transfer Object (DTO) pattern for clean configuration management

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** No direct GameObject creation - data structures only
**Scene Hierarchy:** N/A for this task
**Inspector Config:** Serializable data structures with [Header] attributes for organization
**System Connections:** Foundation for death zone positioning, trigger detection, and life management systems

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** 
- `Assets/Scripts/DeathZone/DeathZoneConfig.cs` - Main configuration data structures and enumerations

**Code Standards:** Unity C# naming conventions, XML documentation for public methods, [Header] attributes for Inspector organization

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1321CreateDeathZoneConfigSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Death Zone Configuration"`

**Class Pattern:** `CreateDeathZoneConfigSetup` (static class)

**Core Functionality:**

- Create DeathZoneConfig ScriptableObject asset in Resources folder for easy loading
- Set up default configuration values for Breakout gameplay balance
- Configure default life management settings with appropriate starting lives
- Create folder structure for death zone system organization
- Validate configuration completeness and provide usage instructions

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateDeathZoneConfigSetup
{
    [MenuItem("Breakout/Setup/Create Death Zone Configuration")]
    public static void CreateDeathZoneConfiguration()
    {
        // Create Resources folder if needed
        // Create default DeathZoneConfig ScriptableObject
        // Set default values for Breakout gameplay
        // Save asset and log success message
        Debug.Log("✅ Death Zone Configuration created successfully");
    }

    [MenuItem("Breakout/Setup/Create Death Zone Configuration", true)]
    public static bool ValidateCreateDeathZoneConfiguration()
    {
        // Return false if configuration already exists
        return !AssetDatabase.FindAssets("t:DeathZoneConfig").Length > 0;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with specific details
- Handle missing Resources folder by creating it
- Validate configuration asset creation completed successfully
- Provide actionable instructions for using the created configuration

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing the configuration structure created
- Provide instructions on how to modify configuration values in Inspector
- Explain how other death zone components will use these structures

### **Documentation:**

- Create brief .md file capturing data structure design decisions
- Document configuration parameters and their gameplay impact
- Include usage instructions for future death zone system development

### **Custom Instructions:**

- Create as ScriptableObject for easy Inspector configuration and runtime loading
- Include validation methods for configuration completeness
- Add default value initialization for immediate usability

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] DeathZoneConfig structure supports all required parameters for trigger detection and life management
- [ ] Configuration system enables easy death zone modification and testing during development
- [ ] Data structures are serializable for Inspector configuration and runtime adjustment
- [ ] Death zone configuration supports adaptive positioning relative to paddle and screen scaling

### **Integration Tests:**

- [ ] DeathZoneConfig can be created and modified through Inspector interface
- [ ] Configuration values can be loaded and accessed by other systems at runtime

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] Classes are focused on configuration data only
- [ ] All properties are properly serialized for Inspector access

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** false - no external dependencies for data structures
**ValidationLevel:** Basic - simple validation for configuration completeness
**Reusability:** Reusable - configuration system for all death zone functionality

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use [System.Serializable] for Inspector visibility
- Include [Header] attributes for Inspector organization
- No hard-coded magic numbers (use named constants)
- Implement ISerializationCallbackReceiver if complex validation needed

### **Performance Requirements:**

- Lightweight data structures with minimal memory allocation
- Pre-configured values to avoid runtime calculations
- Efficient serialization for fast loading

### **Architecture Pattern:**

Data Transfer Object (DTO) pattern for clean separation between configuration and implementation

## **DEPENDENCY HANDLING**

**Missing Dependencies:**
- **If Unity project setup incomplete:** Log clear error with Unity version requirements
- **If Scripts folder missing:** Create folder structure automatically in setup script

**Fallback Behaviors:**

- Use sensible default values if configuration is incomplete
- Log informative warnings for missing configuration sections
- Provide self-validating configuration with error reporting

---