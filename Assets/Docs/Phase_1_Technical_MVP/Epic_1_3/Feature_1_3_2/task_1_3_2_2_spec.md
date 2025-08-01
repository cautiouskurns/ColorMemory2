# **TASK 1.3.2.2: Death Zone Positioning System** *(Medium Complexity - 45 minutes)*

### **Task Overview**

| Task Details | Description |
| --- | --- |
| **Task ID** | 1.3.2.2 |
| **Priority** | High |
| **Complexity** | Medium |
| **Category** | System |
| **Tags** | Positioning, Paddle Integration, Resolution Scaling, Adaptive |
| **Dependencies** | Death zone configuration (Task 1.3.2.1), paddle positioning system (can be stubbed) |
| **Deliverable** | DeathZonePositioning MonoBehaviour with adaptive placement system |

### **Unity Integration**

- **GameObjects:** DeathZonePositioning component attached to death zone management GameObject
- **Scene Hierarchy:** Positioning system organized under death zone parent container
- **Components:** DeathZonePositioning MonoBehaviour with Transform positioning logic
- **System Connections:** Integrates with paddle system for relative placement, uses DeathZoneConfig for parameters

### **Task Acceptance Criteria**

- [ ] Death zone position adapts correctly to paddle location changes during gameplay
- [ ] Positioning system maintains consistent placement across different screen resolutions and aspect ratios
- [ ] Death zone area coverage provides appropriate gameplay balance without being too punishing or too forgiving
- [ ] Positioning updates efficiently without performance impact during paddle movement

### **Implementation Specification**

**Core Requirements:**
- Implement paddle-relative positioning system that places death zone below paddle area consistently
- Add screen resolution adaptation that maintains death zone placement across different aspect ratios
- Create positioning update system that responds to paddle movement and screen resolution changes
- Include positioning validation to ensure death zone covers appropriate area without gameplay interference

**Technical Details:**
- File location: `Assets/Scripts/DeathZone/DeathZonePositioning.cs`
- MonoBehaviour with paddle reference and positioning calculation methods
- Screen resolution adaptation with aspect ratio maintenance
- Position update system responding to paddle movement and resolution changes
- Validation methods for appropriate death zone area coverage

### **Architecture Notes**

- **Pattern:** Component pattern with MonoBehaviour for Unity integration
- **Performance:** Efficient position updates triggered by events, not continuous polling
- **Resilience:** Robust positioning system with paddle integration and resolution adaptation

### **File Structure**

- `Assets/Scripts/DeathZone/DeathZonePositioning.cs` - Main positioning system MonoBehaviour
- Dependencies on DeathZoneConfig from Task 1.3.2.1