# **Unity C# Implementation Task: Bounce Angle Calculation System** *(55 minutes)*

## **1. TASK METADATA** *(Project Context)*

**Task ID:** 1.1.3.3
**Category:** Gameplay
**Tags:** Physics, Gameplay, Player Control
**Priority:** High

## **2. IMPLEMENTATION OBJECTIVE** *(What & Why)*

**Target System:** Bounce angle calculation system within CollisionManager
**Game Context:** Breakout arcade game where player controls ball direction by hitting different parts of the paddle

**Purpose:** Gives players agency over ball direction by calculating bounce angles based on paddle hit position, creating satisfying arcade physics that reward skillful paddle positioning and timing.
**Complexity:** Medium - mathematical bounce calculations with physics integration and player control mechanics

## **3. TECHNICAL SPECIFICATION** *(How to Build)*

### **Class Structure:**

```csharp
// Extend existing CollisionManager class
public class CollisionManager : MonoBehaviour
{
    [Header("Bounce Calculation")]
    [SerializeField] private float minBounceAngle = 15f;  // Minimum bounce angle in degrees
    [SerializeField] private float maxBounceAngle = 165f; // Maximum bounce angle in degrees
    [SerializeField] private float paddleWidth = 2.0f;    // Paddle width for hit position calculation
    
    // Core bounce calculation method
    private Vector2 CalculateBounceAngle(Collision2D collision, GameObject paddle)
    {
        // Calculate relative hit position (-1.0 to 1.0)
        // Map hit position to bounce angle
        // Maintain ball speed while changing direction
        // Return new velocity vector
    }
    
    // Enhanced paddle collision handling
    private void HandlePaddleCollision(Collision2D collision)
    {
        // Apply bounce angle calculation
        // Update ball velocity with new direction
    }
}
```

### **Core Logic:**

- Calculate relative hit position: (collision.contacts[0].point.x - paddle.transform.position.x) / (paddleWidth * 0.5f)
- Map hit position to bounce angle: Mathf.Lerp(minAngle, maxAngle, (hitPosition + 1.0f) * 0.5f)
- Preserve ball speed: Vector2 newVelocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * ballSpeed
- Clamp bounce angles between 15-165 degrees to prevent horizontal bounces
- Apply velocity to ball Rigidbody2D: ballRigidbody.velocity = newVelocity

### **Dependencies:**

- CollisionManager base structure with collision event handling (Task 1.1.3.2)
- Ball GameObject with Rigidbody2D component for velocity modification
- Paddle GameObject with proper collider and positioning
- If Ball Rigidbody2D missing: Log error and skip velocity modification
- If Paddle transform missing: Use collision point for fallback calculation

### **Performance Constraints:**

- Mathematical calculations with minimal allocation and processing overhead
- Bounce calculation completes within single collision frame
- No garbage collection during bounce angle computation

### **Architecture Guidelines:**

- Follow Single Responsibility Principle - focus on bounce calculation only
- Keep bounce calculation methods focused and mathematically precise
- Only implement bounce angle functionality explicitly required
- Avoid adding extra physics effects not specified in requirements

## **4. UNITY INTEGRATION** *(Where It Fits)*

**GameObject Setup:** Modifies existing Ball GameObject physics behavior through CollisionManager
**Scene Hierarchy:** No hierarchy changes - extends CollisionManager functionality only
**Inspector Config:** Bounce angle parameters (min/max angles, paddle width) as serialized fields
**System Connections:** Integrates with Ball Rigidbody2D for velocity modification and Paddle transform for position data

## **5. RESPONSE CONTRACT** *(How AI Should Reply)*

**Required Output Format:**

1. **Implementation Plan** (4-6 bullets covering hit position calculation, angle mapping, speed preservation, and velocity application)
2. **Code Files** (Extended CollisionManager.cs with bounce calculation methods)
3. **Editor Setup Script** (configures bounce calculation parameters on existing CollisionManager)
4. **Integration Notes** (explanation of how bounce calculations provide player control and integrate with physics)

**File Structure:** `Assets/Scripts/Managers/CollisionManager.cs` (extend existing file)
**Code Standards:** Unity physics conventions, efficient mathematical calculations, clear parameter naming

### **Editor Setup Script Requirements:**

**File Location:** `Assets/Editor/Setup/1133CreateBounceCalculationSetup.cs`

**Menu Structure:** `"Breakout/Setup/Configure Bounce Calculation"`

**Class Pattern:** `CreateBounceCalculationSetup` (static class)

**Core Functionality:**

- Find existing CollisionManager GameObject and component
- Configure bounce angle parameters (min/max angles, paddle width)
- Validate Ball and Paddle GameObjects exist for bounce calculation
- Set up proper parameter values for arcade-style bouncing
- Test bounce calculation with sample collision data
- Handle missing CollisionManager gracefully with setup instructions

**Template Structure:**

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateBounceCalculationSetup
{
    [MenuItem("Breakout/Setup/Configure Bounce Calculation")]
    public static void ConfigureBounceCalculation()
    {
        // Find existing CollisionManager
        // Configure bounce angle parameters
        // Validate Ball and Paddle references
        // Test calculation setup
        Debug.Log("✅ Bounce Calculation configured successfully");
    }

    [MenuItem("Breakout/Setup/Configure Bounce Calculation", true)]
    public static bool ValidateConfigureBounceCalculation()
    {
        return GameObject.FindObjectOfType<CollisionManager>() != null;
    }
}
#endif
```

**Error Handling Requirements:**

- Log clear success/failure messages for bounce calculation setup
- Handle missing CollisionManager with setup instructions
- Validate Ball Rigidbody2D exists for velocity modification
- Provide parameter tuning guidance for different bounce feels

## **6. ADDITIONAL DELIVERABLES** *(Task-Specific Instructions)*

### **Terminal Summary:**

- Generate detailed console output showing bounce calculation configuration
- List bounce angle parameters and their effects on gameplay
- Provide testing instructions for validating bounce angle variety
- Include parameter tuning guide for different bounce behaviors

### **Documentation:**

- Create brief .md file capturing:
  - Bounce angle calculation algorithm explanation
  - Parameter effects on gameplay (min/max angles, paddle width)
  - Player control mechanics through paddle positioning
  - Integration with Unity physics system

### **Custom Instructions:**

- Add bounce angle visualization in Scene view using OnDrawGizmos() for development
- Include hit position calculation logging for debugging bounce behavior
- Implement bounce angle clamping to prevent impossible physics scenarios

## **7. SUCCESS CRITERIA** *(When You're Done)*

### **Core Functionality:**

- [ ] Ball bounces at different angles based on paddle hit position
- [ ] Center paddle hits produce near-vertical bounces
- [ ] Edge paddle hits produce more horizontal bounces
- [ ] Ball speed remains consistent through bounce calculations
- [ ] Bounce angles feel predictable and controllable by player

### **Integration Tests:**

- [ ] Ball-paddle collisions trigger bounce angle calculation
- [ ] Hit position correctly maps from paddle center to edges (-1.0 to 1.0 range)
- [ ] Bounce angles stay within 15-165 degree constraints

### **Quality Gates:**

- [ ] No compilation errors
- [ ] Follows Unity physics best practices
- [ ] Mathematical calculations are efficient and garbage-free
- [ ] Bounce behavior feels satisfying and arcade-appropriate

## **8. RESILIENCE FLAGS** *(Implementation Modifiers)*

**StubMissingDeps:** Basic - create fallback bounce calculation if Ball/Paddle references missing
**ValidationLevel:** Strict - validate all mathematical calculations and angle constraints
**Reusability:** Reusable - bounce calculation should work with different paddle and ball configurations

## **CONSTRAINTS & BEST PRACTICES**

### **Unity Best Practices:**

- Cache Ball Rigidbody2D reference during collision manager initialization
- Use Mathf functions for efficient trigonometric calculations
- Minimize Vector2 allocations during bounce calculations
- Apply velocity changes directly to Rigidbody2D for immediate physics response
- Use [Range] attributes for bounce angle parameters in Inspector

### **Performance Requirements:**

- Bounce calculation completes within 0.1ms per collision
- No garbage collection during angle computation
- Trigonometric calculations optimized for 60fps performance

### **Architecture Pattern:**

- Strategy pattern for bounce calculation within collision event handling framework

## **DEPENDENCY HANDLING**

**Missing Dependencies:**

- **If CollisionManager doesn't exist:** Log error with setup instructions from Task 1.1.3.2
- **If Ball Rigidbody2D missing:** Create reference field with null checking and clear error logging
- **If Paddle GameObject missing:** Use collision contact point for fallback hit position calculation

**Fallback Behaviors:**

- Use default bounce angle if hit position calculation fails
- Log warnings for invalid collision data with graceful degradation
- Continue collision processing even if bounce angle calculation encounters errors