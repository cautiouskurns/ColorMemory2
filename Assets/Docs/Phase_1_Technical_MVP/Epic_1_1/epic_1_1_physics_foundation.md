# **EPIC 1.1: CORE PHYSICS FOUNDATION** *(1.5-2 weeks)*

## **Epic Overview**

| Epic Details | Description |
| --- | --- |
| **Priority** | Critical |
| **Risk Level** | High |
| **Purpose** | Establishes the fundamental physics systems that power all core gameplay - ball movement, paddle control, and collision detection. This epic validates that responsive, satisfying arcade physics can be achieved within Unity's physics system while maintaining 60fps WebGL performance. |
| **Dependencies** | Clean Unity scene with basic project setup |

**Playable State After Epic:** Player can control paddle smoothly with keyboard input, ball launches from paddle and bounces realistically off paddle and walls, ball movement feels responsive and predictable, collision detection works consistently without physics anomalies or tunneling.

## **Feature Breakdown**

| Feature | Duration | Prerequisites | Core Purpose |
| --- | --- | --- | --- |
| **1.1.1: Ball Physics System** | 32 hours | Unity 2D physics setup | Creates responsive ball movement with reliable collision detection and speed management |
| **1.1.2: Paddle Control System** | 24 hours | Input system configuration | Implements precise paddle movement with multiple input methods and boundary constraints |
| **1.1.3: Collision Response Integration** | 28 hours | Ball and Paddle systems complete | Integrates physics systems with proper bounce calculations and collision feedback |

---

## **FEATURE SPECIFICATIONS**

### **FEATURE 1.1.1: BALL PHYSICS SYSTEM** *(32 hours)*

**Purpose:** Creates the core ball physics that drive all gameplay interactions, ensuring consistent and satisfying movement with reliable collision detection at varying speeds.

**Technical Approach:** Unity Rigidbody2D system with continuous collision detection, custom velocity management scripts, and physics material configuration for consistent bouncing behavior.

**Core Deliverables:**
- BallController MonoBehaviour with physics integration and velocity management
- Ball GameObject with Rigidbody2D, CircleCollider2D, and physics materials configured
- Ball launch mechanics from paddle with directional control
- Speed normalization system to maintain consistent ball velocity
- Physics debugging tools for collision validation

**Success Criteria:**
- [ ] Ball maintains consistent speed throughout gameplay without acceleration/deceleration
- [ ] Ball launches from paddle in predictable directions based on input
- [ ] Collision detection works reliably at all ball speeds without tunneling
- [ ] Ball physics feel responsive and arcade-appropriate (not overly realistic)
- [ ] Physics system performs at 60fps with multiple simultaneous calculations

### **FEATURE 1.1.2: PADDLE CONTROL SYSTEM** *(24 hours)*

**Purpose:** Implements responsive paddle movement that feels precise and immediate, supporting multiple input methods while maintaining proper game boundaries.

**Technical Approach:** Input polling system with smooth movement interpolation, boundary constraint system, and support for both keyboard and mouse control schemes.

**Core Deliverables:**
- PaddleController MonoBehaviour with input handling and movement logic
- Paddle GameObject with BoxCollider2D and proper physics material
- Multi-input support (WASD, Arrow Keys, Mouse) with input method switching
- Boundary constraint system preventing paddle from leaving play area
- Input response system achieving <50ms latency requirement

**Success Criteria:**
- [ ] Paddle responds to input within 50ms for arcade-quality feel
- [ ] Multiple input methods (keyboard/mouse) work interchangeably
- [ ] Paddle movement is smooth and predictable with proper acceleration/deceleration
- [ ] Boundary constraints prevent paddle from leaving playfield
- [ ] Input system handles multiple simultaneous key presses correctly

### **FEATURE 1.1.3: COLLISION RESPONSE INTEGRATION** *(28 hours)*

**Purpose:** Integrates ball and paddle physics systems with sophisticated collision response that creates satisfying bounce behavior and proper physics feedback.

**Technical Approach:** Unity collision event system with custom bounce angle calculations, collision layer configuration, and physics material optimization for consistent response behavior.

**Core Deliverables:**
- CollisionManager system handling ball-paddle interaction physics
- Bounce angle calculation based on paddle hit position for player control
- Physics layer configuration isolating different collision types
- Collision feedback system providing immediate visual/audio cues
- Edge case handling for unusual collision scenarios

**Success Criteria:**
- [ ] Ball bounce angle changes predictably based on paddle hit position
- [ ] Collision response feels immediate and satisfying with proper physics feedback
- [ ] Physics layers correctly isolate different collision interactions
- [ ] System handles edge cases (corner hits, high-speed collisions) gracefully
- [ ] Collision detection maintains accuracy across all supported frame rates

---

## **Epic Integration**

**System Architecture:** The Core Physics Foundation creates three interconnected systems where BallController manages autonomous physics behavior, PaddleController handles player input and movement, and CollisionManager coordinates their interactions through Unity's collision event system.

**Dependencies on Other Epics:** Requires basic Unity scene setup but operates independently. Will provide physics foundation for Basic Brick System (Epic 1.2) collision integration and Game Boundaries (Epic 1.3) wall interaction.

**Provides to Other Epics:** Delivers reliable physics framework that Basic Brick System can extend for brick destruction, Game Boundaries can utilize for wall bouncing, and all future systems can depend on for consistent collision behavior and performance characteristics.