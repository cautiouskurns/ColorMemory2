# **TASK 1.1.1.5: BALL LAUNCH MECHANICS** *(Medium - 90 minutes)*

### **Task Overview**

| Task Details | Description |
| --- | --- |
| **Task ID** | 1.1.1.5 |
| **Priority** | Critical |
| **Complexity** | Medium |
| **Category** | System |
| **Tags** | Physics, Launch, Input, Gameplay |
| **Dependencies** | Velocity management system from Task 1.1.1.4 |
| **Deliverable** | Complete ball launch system with directional control and paddle integration |

### **Unity Integration**

- **GameObjects:** Ball GameObject enhanced with launch state management and positioning
- **Scene Hierarchy:** Launch mechanics integrated into BallController under GameArea
- **Components:** Enhanced BallController with launch state machine and directional control
- **System Connections:** Integration with paddle positioning, input handling, and velocity management

### **Task Acceptance Criteria**

- [ ] Ball launches from paddle in predictable directions based on input
- [ ] Launch direction calculation provides meaningful player control
- [ ] Launch state management properly transitions between different ball states
- [ ] Launch mechanics integrate seamlessly with velocity management system
- [ ] Launch input handling responds to spacebar trigger as specified in GDD
- [ ] Ball positioning relative to paddle works correctly during launch preparation

### **Implementation Specification**

**Core Requirements:**
- Implement launch state management system (ready to launch, launching, in play)
- Create directional launch calculations providing player control over ball trajectory
- Integrate with paddle position for launch positioning and angle calculation
- Handle spacebar input for launch trigger as specified in GDD controls
- Coordinate with velocity management system for consistent launch behavior
- Provide visual and functional feedback during launch preparation phase

**Technical Details:**
- Launch states: Ready, Launching, InPlay enum with state transition logic
- Direction calculation: Launch angle based on paddle position and input timing
- Input handling: Input.GetKeyDown(KeyCode.Space) for launch trigger detection
- Paddle integration: Transform positioning relative to paddle during ready state
- Launch vector: Calculated launch direction with configurable angle range
- State transitions: Proper state machine with validation and error handling

### **Architecture Notes**

- **Pattern:** State machine pattern for launch phase management with input integration
- **Performance:** Efficient state transitions and input polling targeting 60fps WebGL performance
- **Resilience:** Robust state validation preventing invalid transitions and stuck states

### **File Structure**

- `Assets/Scripts/Ball/BallController.cs` - Enhanced with launch state machine and directional control methods
- `Assets/Scripts/Ball/BallLaunchState.cs` - Launch state enumeration and transition logic