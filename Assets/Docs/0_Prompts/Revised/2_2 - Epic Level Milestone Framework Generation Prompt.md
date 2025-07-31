# Epic Level **Milestone Framework Generation Prompt**

You are an expert technical architect specializing in Unity game development project planning.

Analyze the provided GDD and TDS documents to create a focused epic specification that identifies features and their scope without over-engineering implementation details.

## 🎯 TARGET EPIC:

**Epic to Detail:** [SPECIFY EPIC NAME AND NUMBER HERE - e.g., "Epic 1.2: Grid System"]

## 📐 OUTPUT FORMAT:

---

# **EPIC [X.Y]: [EPIC_NAME]** *([Total Duration])*

## **Epic Overview**

| Epic Details | Description |
| --- | --- |
| **Priority** | [Critical/High/Medium/Low] |
| **Risk Level** | [High/Medium/Low] |
| **Purpose** | [What this epic accomplishes and why it's essential for the game] |
| **Dependencies** | [What must be complete before this epic can begin] |

**Playable State After Epic:** [What players can experience after this epic is complete]

## **Feature Breakdown**

| Feature | Duration | Prerequisites | Core Purpose |
| --- | --- | --- | --- |
| **[X.Y.1]: [Feature Name]** | [X hours] | [Dependencies] | [What this feature enables] |
| **[X.Y.2]: [Feature Name]** | [X hours] | [Dependencies] | [What this feature enables] |
| **[X.Y.3]: [Feature Name]** | [X hours] | [Dependencies] | [What this feature enables] |

---

## **FEATURE SPECIFICATIONS**

### **FEATURE [X.Y.1]: [FEATURE_NAME]** *([Duration])*

**Purpose:** [What this feature accomplishes and why it matters for the game]

**Technical Approach:** [High-level technical strategy - key systems, patterns, or Unity features used]

**Core Deliverables:**

- [Primary system/class this feature creates]
- [Key Unity components or scene setup required]
- [Integration points with other systems]

**Success Criteria:**

- [ ]  [Specific player-facing functionality that works]
- [ ]  [Technical milestone that can be validated]
- [ ]  [Integration checkpoint with other systems]

### **FEATURE [X.Y.2]: [FEATURE_NAME]** *([Duration])*

[Same format for each feature]

### **FEATURE [X.Y.3]: [FEATURE_NAME]** *([Duration])*

[Same format for each feature]

---

## **Epic Integration**

**System Architecture:** [How the features in this epic work together]

**Dependencies on Other Epics:** [What this epic requires from other parts of the game]

**Provides to Other Epics:** [What this epic enables for future development]

---

## 📝 GENERATION INSTRUCTIONS:

**Focus Areas:**

- Keep epic specs strategic and architectural rather than implementation-detailed
- Identify features and their scope without duplicating feature-to-task breakdown work
- Focus on what needs to be built and why, not how to build it
- Ensure features are properly scoped for the estimated durations
- Include enough context for feature breakdown but avoid over-specification

**Epic-Level Concerns:**

- Overall system architecture and how features fit together
- Dependencies between features and with other epics
- Player-facing outcomes and playable states
- Technical risk identification and mitigation approach
- Resource and time estimation for planning

**Avoid:**

- Detailed class specifications (leave for feature-to-task breakdown)
- Specific method signatures or implementation details
- Detailed Unity component configurations
- Asset specifications beyond high-level requirements
- Implementation sequences (handle at task level)

Generate focused epic specifications that enable effective feature planning without over-engineering implementation details.

## 📄 INPUTS:

**Target Epic:** [SPECIFY EPIC TO DETAIL]

**Game Design Document:** [PASTE RELEVANT GDD SECTIONS]

**Technical Design Specification:** [PASTE RELEVANT TDS SECTIONS]