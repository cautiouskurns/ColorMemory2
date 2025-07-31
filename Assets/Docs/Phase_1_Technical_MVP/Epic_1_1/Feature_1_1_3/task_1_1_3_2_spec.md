# **TASK 1.1.3.2: COLLISIONMANAGER BASE STRUCTURE** *(Medium - 50 minutes)*

### **Task Overview**

| Task Details | Description |
| --- | --- |
| **Task ID** | 1.1.3.2 |
| **Priority** | Critical |
| **Complexity** | Medium |
| **Category** | System |
| **Tags** | Physics, Manager, Foundation |
| **Dependencies** | Physics layers configured and applied to game objects |
| **Deliverable** | CollisionManager MonoBehaviour with collision event handling foundation |

### **Unity Integration**

- **GameObjects:** New CollisionManager GameObject under Managers hierarchy
- **Scene Hierarchy:** Create under existing Managers parent container
- **Components:** MonoBehaviour component with collision event subscription system
- **System Connections:** Subscribes to Ball collision events, provides framework for future collision response routing

### **Task Acceptance Criteria**

- [ ] CollisionManager properly detects and categorizes all collision types
- [ ] Collision events are captured and logged for all relevant GameObjects
- [ ] System maintains single responsibility for collision coordination
- [ ] Framework ready for specific collision response implementations
- [ ] Singleton pattern ensures single collision coordinator

### **Implementation Specification**

**Core Requirements:**
- Create CollisionManager MonoBehaviour class with singleton pattern
- Implement collision event subscription system for Ball GameObject
- Detect and categorize collision types (paddle vs brick vs boundary)
- Establish routing framework for collision response handling
- Add collision logging system for debugging and validation

**Technical Details:**
- Class: CollisionManager : MonoBehaviour with singleton Instance property
- Event Methods: OnCollisionEnter2D, OnCollisionExit2D event handling
- Collision Detection: Use Collision2D.gameObject.layer to determine collision type
- Logging System: Debug.Log collision events with object names and collision types
- Framework Methods: HandlePaddleCollision(), HandleBrickCollision(), HandleBoundaryCollision() stubs

### **Architecture Notes**

- **Pattern:** Singleton Manager pattern for centralized collision coordination
- **Performance:** Event-driven system with minimal processing overhead
- **Resilience:** Foundation system that must handle all collision types reliably

### **File Structure**

- `Assets/Scripts/Managers/CollisionManager.cs` - Main collision coordination system
- Scene: Add CollisionManager GameObject under Managers hierarchy