# **High-Level Milestone Framework Generation Prompt**

You are an expert project manager specializing in solo game development planning with deep understanding of standard game development milestones.

Analyze the provided GDD and TDS to create a focused development roadmap using proven milestone phases: Technical MVP, Vertical Slice, Playable Beta, and Release Version.

## 🎯 OUTPUT FORMAT:

---

# **DEVELOPMENT ROADMAP: "[Game Title]"**

## **Project Summary**

**Game Type:** [Genre and core mechanics]

**Development Time:** [Total estimated duration across all phases]

**Platform:** [Primary target platform]

**Key Challenge:** [Main technical or design challenge to solve]

---

## **DEVELOPMENT PHASES**

### **PHASE 1: TECHNICAL MVP** *([Duration])*

**Goal:** Prove core systems work technically - validate that the game can be built
**Validation:** [What technical systems need to be proven to work]
**Playable State:** [Specific description of basic functionality that demonstrates core systems work - what can players interact with to prove the concept?]

**Key Epics:**

- **[Epic Name]:** [Essential system that proves technical feasibility]
- **[Epic Name]:** [Foundation system required for core gameplay]
- **[Epic Name]:** [Critical technical component that validates approach]

### **PHASE 2: VERTICAL SLICE** *([Duration])*

**Goal:** Complete one full gameplay loop - prove the game is fun and engaging
**Validation:** [What complete gameplay experience needs to work end-to-end]
**Playable State:** [Specific description of complete player experience - what can players do from start to finish of one gameplay cycle?]

**Key Epics:**

- **[Epic Name]:** [System that completes the core gameplay loop]
- **[Epic Name]:** [Feature that adds game mechanics and rules]
- **[Epic Name]:** [Component that provides player feedback and progression]

### **PHASE 3: PLAYABLE BETA** *([Duration])*

**Goal:** All systems integrated and working together - rough but feature complete
**Validation:** [What full game experience needs to be playable]
**Playable State:** [Specific description of complete game experience - what can players do across all features and how does it feel as a cohesive game?]

**Key Epics:**

- **[Epic Name]:** [System integration that brings everything together]
- **[Epic Name]:** [Content system that provides full game scope]
- **[Epic Name]:** [Polish system that improves player experience]

### **PHASE 4: RELEASE VERSION** *([Duration])*

**Goal:** Launch-ready product with polish, optimization, and final content
**Validation:** [What quality standards need to be met for release]
**Playable State:** [Specific description of final player experience - what does the polished, complete game offer to players?]

**Key Epics:**

- **[Epic Name]:** [Polish and optimization system]
- **[Epic Name]:** [Final content and balancing]
- **[Epic Name]:** [Release preparation and platform integration]

---

## **MILESTONE VALIDATION**

**Technical MVP Success:** [Specific criteria that prove technical feasibility]
**Vertical Slice Success:** [Specific criteria that prove fun gameplay loop]

**Playable Beta Success:** [Specific criteria that prove integrated game experience]
**Release Ready:** [Specific criteria that prove launch readiness]

**Critical Path:** [Essential systems that must be built first → systems that depend on them → final polish]

**Biggest Risk:** [Main thing that could derail development and mitigation strategy]

---

## 📝 GENERATION INSTRUCTIONS:

**Phase Structure Requirements:**

- Phase 1 (Technical MVP): Focus on proving core systems work, not polish
- Phase 2 (Vertical Slice): One complete, fun gameplay loop that validates game concept
- Phase 3 (Playable Beta): All features integrated, rough but complete game experience
- Phase 4 (Release Version): Polish, optimization, and launch preparation

**Validation Focus:**

- Each phase should have clear success criteria that can be tested
- Playable states should describe what players can actually DO and experience
- Build on previous phases - each adds to what came before
- Focus on player experience progression, not just technical milestones

**Solo Development Optimization:**

- Realistic time estimates for single developer
- Front-load technical risk in MVP phase
- Emphasize working systems over perfect implementation until Release phase
- Consider platform constraints and deployment requirements

**Keep It Focused:**

- Don't over-detail epic specifications (leave for epic breakdown)
- Emphasize clear milestone definitions over complex planning
- Focus on what players can experience at each phase
- Identify only critical path dependencies, not exhaustive lists
- Ensure thta you number the Epics accordng to their phase and number order

Generate a focused roadmap using proven game development milestone phases that enables clear development decisions and progress validation.

## 📄 INPUTS:

**Game Design Document:** [PASTE RELEVANT GDD SECTIONS]

**Technical Design Specification:** [PASTE RELEVANT TDS SECTIONS]