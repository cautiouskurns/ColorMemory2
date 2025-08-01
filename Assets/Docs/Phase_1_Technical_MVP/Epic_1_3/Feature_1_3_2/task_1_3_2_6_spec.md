# **TASK 1.3.2.6: Scoring System Integration** *(Medium Complexity - 40 minutes)*

### **Task Overview**

| Task Details | Description |
| --- | --- |
| **Task ID** | 1.3.2.6 |
| **Priority** | Medium |
| **Complexity** | Medium |
| **Category** | System |
| **Tags** | Scoring, Penalties, Bonuses, System Integration |
| **Dependencies** | Death zone trigger detection (Task 1.3.2.3), scoring system (can be stubbed) |
| **Deliverable** | DeathZoneScoring MonoBehaviour with scoring integration and penalty system |

### **Unity Integration**

- **GameObjects:** Scoring integration component attached to death zone or scoring management GameObject
- **Scene Hierarchy:** Scoring integration organized under game management container
- **Components:** DeathZoneScoring MonoBehaviour with scoring calculation and event integration
- **System Connections:** Subscribes to death zone trigger events and communicates with main scoring system

### **Task Acceptance Criteria**

- [ ] Scoring integration responds appropriately to death zone events with accurate penalty calculations
- [ ] Score penalties provide appropriate gameplay balance without being overly punishing
- [ ] Bonus scoring opportunities add positive reinforcement for skilled play near death zone
- [ ] Integration with scoring system maintains clean separation of concerns and loose coupling

### **Implementation Specification**

**Core Requirements:**
- Implement scoring integration system that responds to death zone trigger events
- Add score penalty calculations for ball loss events with configurable penalty amounts
- Create bonus scoring opportunities based on consecutive saves or near-miss scenarios
- Include scoring event system that communicates with main scoring system for score updates

**Technical Details:**
- File location: `Assets/Scripts/DeathZone/DeathZoneScoring.cs`
- MonoBehaviour with scoring calculation methods and event integration
- Death zone trigger event subscription with penalty calculation logic
- Bonus scoring system for near-miss scenarios and consecutive saves
- Scoring system communication with event-driven updates and score modifications

### **Architecture Notes**

- **Pattern:** Integration pattern with event-driven scoring system communication
- **Performance:** Efficient scoring calculations with minimal computational overhead
- **Resilience:** Loose coupling with scoring system and configurable penalty/bonus calculations

### **File Structure**

- `Assets/Scripts/DeathZone/DeathZoneScoring.cs` - Scoring integration and penalty system
- Integration with DeathZoneTrigger events from Task 1.3.2.3