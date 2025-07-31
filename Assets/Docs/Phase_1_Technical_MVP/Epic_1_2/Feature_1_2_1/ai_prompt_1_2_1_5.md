# **Unity C# Implementation Task: Visual Effects System** *(45 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.2.1.5
**Category:** Feature
**Tags:** Visual Effects, Particle Systems, Player Feedback
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** Particle effects system for brick destruction visual feedback
**Game Context:** Breakout arcade game requiring satisfying visual feedback when bricks are destroyed

**Purpose:** Adds immediate and satisfying particle effects when bricks are destroyed, enhancing player experience with visual feedback that matches brick colors and provides arcade-quality destruction effects.
**Complexity:** Medium - Unity ParticleSystem integration with dynamic configuration

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// Extend existing Brick MonoBehaviour class
public class Brick : MonoBehaviour
{
    // ... existing properties from previous tasks ...
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem destructionParticles;
    [SerializeField] private int particleCount = 10;
    [SerializeField] private float particleLifetime = 1.0f;
    [SerializeField] private float particleSpeed = 5.0f;
    
    // Visual effects methods
    private void TriggerDestructionEffects()
    {
        if (destructionParticles != null)
        {
            ConfigureParticleSystem();
            EmitDestructionParticles();
        }
    }
    
    private void ConfigureParticleSystem() { }
    private void EmitDestructionParticles() { }
    private void CleanupParticleEffects() { }
}
```

### **Core Logic:**

- ParticleSystem integration for destruction effects with dynamic color matching
- Particle burst configuration based on brick type and color for visual consistency
- Immediate effect triggering synchronized with destruction events
- Color matching system using brickData.brickColor for particle start color
- Particle system cleanup and memory management on GameObject destruction

### **Dependencies:**

- Destruction mechanics from Task 1.2.1.4 (required)
- Existing brick state management and BrickData configuration
- Unity ParticleSystem component on brick GameObjects
- If ParticleSystem missing: Create basic visual feedback with object scaling/fading
- If destruction mechanics missing: Create manual effect triggering

### **Performance Constraints:**

- Efficient particle emission with minimal draw calls and optimized settings
- No continuous particle effects - burst emission only on destruction
- Particle system cleanup prevents memory leaks and performance degradation

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - visual effects for destruction only
- Keep particle effects focused on immediate destruction feedback
- Only implement visual effects explicitly required by specification
- Avoid adding complex animations or continuous effects

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Adds ParticleSystem components to brick GameObjects
**Scene Hierarchy:** Particle effects positioned relative to brick GameObject transform
**Inspector Config:** Particle count, lifetime, speed, and effect settings as serialized fields
**System Connections:** Triggered by destruction events, coordinates with existing feedback systems

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering ParticleSystem setup, color matching, burst configuration, effect timing, and cleanup)
2. **Code Files** (Extended Brick.cs with ParticleSystem integration and visual effects)
3. **Editor Setup Script** (adds and configures ParticleSystem components on brick GameObjects)
4. **Integration Notes** (explanation of how visual effects enhance destruction feedback and coordinate with game systems)

**File Structure:** `Assets/Scripts/Gameplay/Brick.cs` (extend existing file)
**Code Standards:** Unity ParticleSystem conventions, efficient effect triggering, clear configuration

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1215CreateVisualEffectsSetup.cs`

**Menu Structure:** `"Breakout/Setup/Configure Brick Visual Effects"`

**Class Pattern:** `CreateVisualEffectsSetup` (static class)

**Core Functionality:**

- Find existing Brick components and add ParticleSystem components
- Configure particle system settings for destruction effects
- Set up particle emission parameters (count, lifetime, speed)
- Configure particle appearance settings (color, size, material)
- Test particle effects with sample destruction scenarios
- Validate effect cleanup and performance impact

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output showing visual effects system configuration
- List particle system settings and their effects on visual feedback
- Provide testing instructions for validating particle effects and cleanup
- Include performance impact assessment and optimization recommendations

### **Documentation:**

- Create brief .md file capturing:
  - Particle system configuration and effect parameters
  - Color matching system for brick-specific visual feedback
  - Performance considerations and optimization settings
  - Effect timing and synchronization with destruction events

### **Custom Instructions:**

- Add particle effect preview tools for development and tuning
- Include particle system performance monitoring and optimization
- Create visual effect testing utilities for different brick types

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Particle effects trigger immediately on brick destruction
- [ ] Particle colors match destroyed brick colors for visual consistency
- [ ] Visual effects enhance destruction satisfaction without cluttering screen
- [ ] Particle system performs efficiently without frame rate impact
- [ ] Effect cleanup prevents particle system memory leaks

### **Integration Tests:**

- [ ] Destruction events trigger particle effects correctly
- [ ] Particle colors match brickData.brickColor values
- [ ] Effects play at proper timing synchronized with destruction
- [ ] Particle system cleanup completes without errors

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity ParticleSystem best practices
- [ ] Visual effects enhance gameplay experience appropriately
- [ ] Performance impact is minimal and within acceptable limits

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** Basic - create simple visual feedback if ParticleSystem unavailable
**ValidationLevel:** Basic - validate particle system exists and effects trigger correctly
**Reusability:** Reusable - visual effects should work with different brick types and configurations

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Use ParticleSystem.Emit() for controlled burst emission
- Configure particle systems for efficient rendering (minimal overdraw)
- Use ParticleSystem.main.startColor for dynamic color assignment
- Implement proper ParticleSystem.Stop() and cleanup on destruction
- Cache ParticleSystem references during initialization

### **Performance Requirements:**

- Particle effects render at 60fps without frame drops
- Burst emission completes within single frame
- No continuous particle effects that accumulate performance cost

### **Architecture Pattern:**

- Component-based particle effects with event-driven triggering and dynamic configuration

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If destruction mechanics not implemented:** Create manual effect triggering method with clear integration instructions
- **If ParticleSystem component missing:** Create basic visual feedback using Transform scaling or material color changes
- **If BrickData color information missing:** Use default particle colors with warnings

**Fallback Behaviors:**

- Continue destruction process even if particle effects fail
- Log informative warnings for particle system configuration issues
- Provide alternative visual feedback if ParticleSystem unavailable