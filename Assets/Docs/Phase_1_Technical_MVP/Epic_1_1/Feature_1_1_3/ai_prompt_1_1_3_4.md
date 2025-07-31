# **Unity C# Implementation Task: Collision Feedback Integration** *(40 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.1.3.4
**Category:** Feature
**Tags:** Audio, Visual, Feedback, Polish
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** Collision feedback system within CollisionManager with audio-visual effects
**Game Context:** Breakout arcade game requiring immediate, satisfying feedback for ball collisions to enhance player experience

**Purpose:** Provides immediate audio-visual feedback for all collision types, creating arcade-quality game feel that communicates collision events clearly and enhances player engagement through sensory feedback.
**Complexity:** Low - audio-visual effect triggers with collision event integration

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// Extend existing CollisionManager class
public class CollisionManager : MonoBehaviour
{
    [Header("Collision Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private Camera gameCamera;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip paddleBounceClip;
    [SerializeField] private AudioClip wallBounceClip;
    [SerializeField] private AudioClip brickHitClip;
    
    [Header("Feedback Settings")]
    [SerializeField] private float screenShakeIntensity = 0.1f;
    [SerializeField] private float screenShakeDuration = 0.15f;
    [SerializeField] private int particleBurstCount = 5;
    
    // Feedback trigger method
    private void TriggerCollisionFeedback(CollisionType type, Vector2 position, float intensity)
    {
        // Play appropriate audio clip
        // Trigger particle effect at collision point
        // Apply screen shake based on collision force
    }
    
    // Screen shake coroutine
    private IEnumerator ScreenShakeCoroutine(float intensity, float duration) { }
}
```

### **Core Logic:**

- Audio feedback: AudioSource.PlayOneShot() for collision sound effects based on collision type
- Particle effects: ParticleSystem.Emit() for collision impact bursts at collision contact point
- Screen shake: Camera position offset using Mathf.Sin() with dampening over 0.1-0.3 seconds
- Intensity scaling: collision.relativeVelocity.magnitude determines feedback strength
- Collision type routing: Different audio clips and particle colors for paddle/wall/brick collisions

### **Dependencies:**

- Working bounce angle calculations and collision detection (Task 1.1.3.3)
- AudioSource component for sound effect playback
- ParticleSystem component for visual impact effects
- Camera reference for screen shake effects
- If AudioSource missing: Log warning and skip audio feedback
- If ParticleSystem missing: Log warning and skip particle effects

### **Performance Constraints:**

- Efficient one-shot audio and particle effects with minimal overhead
- Feedback effects complete within 0.3 seconds maximum
- No continuous effects that accumulate memory usage

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - collision feedback only
- Keep feedback methods focused on audio-visual effects exclusively
- Only implement feedback types explicitly required by specification
- Avoid adding complex effect chains or animations not specified

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Extend existing CollisionManager with AudioSource and ParticleSystem components
**Scene Hierarchy:** Add audio and particle effect components to CollisionManager GameObject
**Inspector Config:** Audio clips, particle settings, screen shake parameters as serialized fields
**System Connections:** Triggered by CollisionManager collision events, coordinates with existing audio and visual systems

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering audio integration, particle effects, screen shake, intensity scaling, and collision type routing)
2. **Code Files** (Extended CollisionManager.cs with feedback methods and coroutines)
3. **Editor Setup Script** (adds feedback components and configures collision feedback system)
4. **Integration Notes** (explanation of how feedback enhances collision system and arcade game feel)

**File Structure:** `Assets/Scripts/Managers/CollisionManager.cs` (extend existing file)
**Code Standards:** Unity audio/visual effect conventions, efficient coroutine usage, clear parameter organization

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1134CreateCollisionFeedbackSetup.cs`

**Menu Structure:** `"Breakout/Setup/Configure Collision Feedback"`

**Class Pattern:** `CreateCollisionFeedbackSetup` (static class)

**Core Functionality:**

- Find existing CollisionManager GameObject and component
- Add AudioSource component with proper configuration
- Add ParticleSystem component with collision burst settings
- Assign main Camera reference for screen shake effects
- Configure default feedback parameters for arcade-style response
- Create placeholder audio clips (AudioClip references for assignment)
- Validate all feedback components are properly connected

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateCollisionFeedbackSetup
{
    [MenuItem("Breakout/Setup/Configure Collision Feedback")]
    public static void ConfigureCollisionFeedback()
    {
        // Find existing CollisionManager
        // Add AudioSource and ParticleSystem components
        // Configure feedback parameters
        // Assign Camera reference and validate setup
        Debug.Log("✅ Collision Feedback configured successfully");
    }

    [MenuItem("Breakout/Setup/Configure Collision Feedback", true)]
    public static bool ValidateConfigureCollisionFeedback()
    {
        CollisionManager cm = GameObject.FindObjectOfType<CollisionManager>();
        return cm != null && cm.GetComponent<AudioSource>() == null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages for feedback component setup
- Handle missing CollisionManager with setup instructions
- Validate AudioSource and ParticleSystem components added successfully
- Provide instructions for assigning audio clips manually in Inspector

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output showing feedback system configuration status
- List which collision types will trigger which feedback effects
- Provide testing instructions for validating audio-visual feedback triggers
- Include parameter tuning guide for different feedback intensities

### **Documentation:**

- Create brief .md file capturing:
  - Collision feedback system integration details
  - Audio clip assignment requirements for different collision types
  - Particle effect configuration for collision impacts
  - Screen shake implementation and intensity scaling

### **Custom Instructions:**

- Implement collision intensity scaling using collision.relativeVelocity.magnitude
- Add particle color variation based on collision type (blue for paddle, white for walls, colored for bricks)
- Create screen shake coroutine with proper cleanup and multiple collision handling

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] All collision types trigger appropriate audio and visual feedback
- [ ] Feedback timing is immediate and synchronized with physics events
- [ ] Audio cues are distinct and recognizable for each collision type
- [ ] Visual effects enhance collision impact without cluttering screen
- [ ] Feedback system performs efficiently without frame rate impact

### **Integration Tests:**

- [ ] Paddle collisions trigger paddle bounce audio and particle effects
- [ ] Wall collisions trigger different audio cues and appropriate visual feedback
- [ ] Screen shake intensity scales appropriately with collision force

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity audio and particle system best practices
- [ ] Feedback effects are polished and enhance gameplay experience
- [ ] System gracefully handles missing audio clips or effect components

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** Basic - continue operation with logging if audio/visual components missing
**ValidationLevel:** Basic - validate component existence but allow graceful degradation
**Reusability:** Reusable - feedback system should work with different collision scenarios

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache AudioSource and ParticleSystem references during collision manager initialization
- Use AudioSource.PlayOneShot() for collision sound effects to avoid cutting off sounds
- Configure ParticleSystem for efficient burst emission without continuous particles
- Implement screen shake coroutine with proper StopCoroutine() cleanup
- Use [Header] attributes for Inspector organization of feedback parameters

### **Performance Requirements:**

- Audio playback completes without frame rate impact
- Particle effects use minimal draw calls and efficient emission
- Screen shake effects run smoothly at 60fps

### **Architecture Pattern:**

- Observer pattern - CollisionManager triggers feedback responses based on collision events

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If CollisionManager doesn't exist:** Log error with setup instructions from Task 1.1.3.2
- **If AudioSource component missing:** Create AudioSource with default settings and log configuration needed
- **If ParticleSystem missing:** Create basic ParticleSystem with collision burst configuration
- **If Camera reference missing:** Find main Camera automatically with fallback logging

**Fallback Behaviors:**

- Continue collision processing even if feedback components are missing
- Log informative warnings for missing audio clips with assignment instructions
- Gracefully skip effects that can't be triggered due to missing components