# **TASK 1.1.3.3: BOUNCE ANGLE CALCULATION SYSTEM** *(Medium - 55 minutes)*

### **Task Overview**

| Task Details | Description |
| --- | --- |
| **Task ID** | 1.1.3.3 |
| **Priority** | High |
| **Complexity** | Medium |
| **Category** | Gameplay |
| **Tags** | Physics, Gameplay, Player Control |
| **Dependencies** | CollisionManager base structure and Ball/Paddle physics working |
| **Deliverable** | Dynamic bounce angle system responding to paddle hit position |

### **Unity Integration**

- **GameObjects:** Modifies existing Ball GameObject physics behavior
- **Scene Hierarchy:** No hierarchy changes - extends CollisionManager functionality
- **Components** | Utilizes existing Rigidbody2D on Ball for velocity modification
- **System Connections:** Integrates with BallController for velocity management and PaddleController for position data

### **Task Acceptance Criteria**

- [ ] Ball bounces at different angles based on paddle hit position
- [ ] Center paddle hits produce near-vertical bounces
- [ ] Edge paddle hits produce more horizontal bounces  
- [ ] Ball speed remains consistent through bounce calculations
- [ ] Bounce angles feel predictable and controllable by player

### **Implementation Specification**

**Core Requirements:**
- Calculate relative hit position on paddle surface (-1.0 to 1.0 range)
- Map hit position to bounce angle with steeper angles toward paddle edges
- Maintain consistent ball speed while adjusting direction vector
- Ensure bounce angles remain within playable range constraints
- Integrate smoothly with Ball Rigidbody2D velocity system

**Technical Details:**
- Hit Position Calculation: (collision.contacts[0].point.x - paddle.transform.position.x) / (paddleWidth * 0.5f)
- Angle Mapping: Mathf.Lerp(minAngle, maxAngle, (hitPosition + 1.0f) * 0.5f)
- Speed Preservation: Vector2 newVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ballSpeed
- Angle Constraints: Clamp bounce angles between 15-165 degrees to prevent horizontal bounces
- Velocity Application: ballRigidbody.velocity = newVelocity

### **Architecture Notes**

- **Pattern:** Strategy pattern for bounce calculation within CollisionManager
- **Performance:** Mathematical calculations with minimal allocation and processing overhead
- **Resilience:** Robust angle clamping prevents edge case physics anomalies

### **File Structure**

- `Assets/Scripts/Managers/CollisionManager.cs` - Extend with CalculateBounceAngle() method
- Integration with existing Ball and Paddle systems through collision event handling