# **Unity C# Implementation Task: Ball Loss Audio-Visual Feedback System** *(45 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.3.2.5
**Category:** Feature
**Tags:** Audio Effects, Visual Effects, Feedback, Player Experience
**Priority:** Medium

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** DeathZoneFeedback MonoBehaviour with audio-visual effects system
**Game Context:** Breakout game requiring immediate and satisfying feedback when ball is lost to enhance arcade-style player experience

**Purpose:** Implements immediate audio-visual feedback system for ball loss events to provide satisfying player awareness, creating dramatic "ball lost" experience with appropriate timing and effects.
**Complexity:** Medium - requires audio system integration, visual effects coordination, timing management, and effect customization

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
[System.Serializable]
public class DeathZoneFeedback : MonoBehaviour
{
    [Header("Configuration")]
    public DeathZoneConfig config;
    
    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip ballLossClip;
    public float audioVolume = 0.8f;
    public float pitchVariation = 0.1f;
    
    [Header("Visual Feedback")]
    public ParticleSystem lossParticles;
    public float screenFlashDuration = 0.2f;
    public Color flashColor = Color.red;
    public CanvasGroup screenFlash;
    
    [Header("Timing")]
    public float feedbackDuration = 1.0f;
    public float dramaticPauseDuration = 0.5f;
    
    [Header("Effect Intensity")]
    public bool enableScreenFlash = true;
    public bool enableParticles = true;
    public bool enableAudio = true;
    
    // Audio-visual feedback coordination
    // Effect timing and sequencing
    // Particle system management
    // Screen flash effects
}
```

### **Core Logic:**

- Implement audio feedback system with appropriate sound effects for ball loss events
- Add visual feedback using particle effects, screen flash, or UI animations for immediate player awareness
- Create feedback timing system that coordinates audio-visual effects for maximum impact
- Include feedback customization options for intensity, duration, and effect selection

### **Dependencies:**

- DeathZoneTrigger events from Task 1.3.2.3 (required)
- Unity Audio system for AudioSource and AudioClip functionality
- Unity Particle System for visual effects (optional)
- **Fallback Strategy:** Create feedback system with basic effects if advanced systems missing

### **Performance Constraints:**

- Efficient effect triggering with pooled particle systems and audio optimization
- Minimal memory allocation during effect playback
- Coordinated timing without frame rate impact

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - focus only on audio-visual feedback
- Keep feedback system separate from life management and scoring logic
- Only implement feedback coordination and effect triggering
- Use Observer pattern with coordinated audio-visual feedback system

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Feedback system GameObject with AudioSource and particle system components
**Scene Hierarchy:** Feedback effects organized under death zone or effects container
**Inspector Config:** DeathZoneFeedback MonoBehaviour with [Header] attributes for effect settings
**System Connections:** Subscribes to death zone trigger events and coordinates audio-visual feedback timing

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering approach and key steps)
2. **Code Files** (all files in dependency order)
3. **Editor Setup Script** (always required - creates GameObjects and scene setup)
4. **Integration Notes** (brief explanation of how this integrates with other systems)

**File Structure:** 
- `Assets/Scripts/DeathZone/DeathZoneFeedback.cs` - Audio-visual feedback coordination system
- Integration with DeathZoneTrigger events from Task 1.3.2.3

**Code Standards:** Unity C# naming conventions, XML documentation for public methods, [Header] attributes for Inspector organization

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1325CreateDeathZoneFeedbackSetup.cs`

**Menu Structure:** `"Breakout/Setup/Create Death Zone Feedback"`

**Class Pattern:** `CreateDeathZoneFeedbackSetup` (static class)

**Core Functionality:**

- Find existing Death Zone System GameObject
- Create "Death Zone Feedback" child GameObject
- Add AudioSource component for sound effects
- Add ParticleSystem component for visual effects
- Add DeathZoneFeedback MonoBehaviour component
- Configure audio and visual effect settings
- Connect to DeathZoneTrigger events for feedback triggering
- Set up screen flash UI overlay if Canvas exists

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateDeathZoneFeedbackSetup
{
    [MenuItem("Breakout/Setup/Create Death Zone Feedback")]
    public static void CreateDeathZoneFeedback()
    {
        // Check for prerequisite death zone trigger
        // Create feedback GameObject
        // Add and configure AudioSource
        // Add and configure ParticleSystem
        // Add DeathZoneFeedback component
        // Configure feedback settings
        // Connect trigger events
        Debug.Log("✅ Death Zone Feedback created successfully");
    }

    [MenuItem("Breakout/Setup/Create Death Zone Feedback", true)]
    public static bool ValidateCreateDeathZoneFeedback()
    {
        // Return false if feedback already exists
        // Validate death zone trigger prerequisite exists
        return GameObject.Find("Death Zone System") != null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages with specific details
- Handle missing death zone trigger with clear error and setup instructions
- Validate audio and visual component creation completed successfully
- Provide troubleshooting steps if effect coordination fails

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output summarizing feedback system setup and effect configuration
- Provide instructions on how to test audio-visual feedback and customize effects
- Explain feedback timing coordination and dramatic pause behavior

### **Documentation:**

- Create brief .md file capturing feedback system architecture and effect coordination
- Document audio and visual effect configuration options
- Include testing procedures for feedback timing and intensity

### **Custom Instructions:**

- Include Coroutine-based timing system for coordinated effects
- Add particle effect configuration with appropriate ball loss theme
- Implement screen flash effect using UI Canvas overlay with fade animation

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Audio-visual feedback provides immediate and satisfying "ball lost" experience matching arcade expectations
- [ ] Feedback timing creates appropriate dramatic pause without disrupting game flow
- [ ] Audio effects enhance gameplay tension and provide clear event confirmation
- [ ] Visual feedback draws appropriate player attention without being overwhelming or distracting

### **Integration Tests:**

- [ ] Death zone trigger events properly activate audio-visual feedback
- [ ] Multiple feedback effects coordinate properly with appropriate timing

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity best practices and Single Responsibility Principle
- [ ] DeathZoneFeedback class is focused on effect coordination only
- [ ] Proper effect timing and coordination without performance impact

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** true - create feedback system with basic effects if particle systems or audio missing
**ValidationLevel:** Basic - validate effect triggering and coordination
**Reusability:** Reusable - feedback system works with any death zone trigger configuration

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use AudioSource.PlayOneShot() for sound effect playback
- Cache ParticleSystem component references during initialization
- Use Coroutines for timed effect coordination
- Implement proper particle system Play() and Stop() management

### **Performance Requirements:**

- Efficient effect triggering with pooled particle systems and audio optimization
- Minimal memory allocation during effect playback
- Coordinated timing without frame rate impact

### **Architecture Pattern:**

Observer pattern with coordinated audio-visual feedback system, Coroutine-based timing coordination

## **DEPENDENCY HANDLING**

**Missing Dependencies:**
- **If DeathZoneTrigger missing:** Log clear error and provide instructions to run Task 1.3.2.3 setup first
- **If Audio system missing:** Create feedback system with visual effects only and log audio setup instructions
- **If Particle System missing:** Create feedback system with audio and screen flash effects

**Fallback Behaviors:**

- Use default effect settings if configuration is incomplete
- Log informative warnings for missing audio or visual components with setup instructions
- Create functional feedback system even if some effect types unavailable

---