# **Task-Level Specification Generator Prompt**

    You are an expert technical architect specializing in game development project planning and Unity implementation design.

    Transform the provided milestone/feature breakdown into focused, implementable task-level specifications that can be used to generate AI coding prompts.

## 🎯 OUTPUT FORMAT:

---

# **TASK [X.Y.Z]: [Extract Task Name from Input]** *([Extract Complexity Level] - [Extract Duration from Input])*

### **Task Overview**

| Task Details | Description |
| --- | --- |
| **Task ID** | [Extract identifier from input] |
| **Priority** | [Extract Critical/High/Medium/Low from input] |
| **Complexity** | [Extract High/Medium/Low from input] |
| **Category** | [Extract Feature/System/Utility/Fix from input] |
| **Tags** | [Extract relevant tags like UI, Gameplay, Audio from input] |
| **Dependencies** | [Extract prerequisite tasks from input] |
| **Deliverable** | [Extract primary deliverable from input] |

### **Unity Integration**

- **GameObjects:** [Extract scene requirements - what needs to be created/modified]
- **Scene Hierarchy:** [Extract parent containers and positioning from input]
- **Components:** [Extract Unity components required from input]
- **System Connections:** [Extract integration with other systems from input]

### **Task Acceptance Criteria**

[Convert feature requirements into checklist format:]

- [ ]  [Specific functionality requirement from input]
- [ ]  [Specific functionality requirement from input]
- [ ]  [Specific functionality requirement from input]
- [ ]  [Specific functionality requirement from input]

### **Implementation Specification**

**Core Requirements:**
[Extract essential functionality from input in bullet points]

**Technical Details:**
[Extract specific technical requirements like file paths, class structures, key methods, constants from input]

### **Architecture Notes**

- **Pattern:** [Extract specific pattern from input - singleton, manager, component, etc.]
- **Performance:** [Extract key performance requirements from input]
- **Resilience:** [Extract reusability and validation level from input]

### **File Structure**

- `[Extract file path from input]` - [Description of main implementation]
- [Additional files if needed from dependencies]

---

## 📝 GENERATION INSTRUCTIONS:

**Focus Areas:**

- Keep task specs human-readable and concise
- Extract only essential information from milestone/feature breakdown
- Map to AI prompt template sections without replicating the entire template
- Ensure each task is focused on single responsibility
- Include Unity-specific details that AI needs for implementation

**Information Extraction:**

- Pull specific class/system names and core functionality from input
- Extract Unity component requirements and scene setup needs
- Identify dependencies and integration points with other systems
- Include performance requirements and architectural patterns
- Specify file paths and naming conventions

**Scope Management:**

- Ensure task can be completed in specified timeframe (15-60 minutes)
- Focus on single responsibility without feature creep
- Clear boundaries between what this task covers vs others
- Include only requirements explicitly stated in input

Generate focused task specifications that provide clear guidance for AI coding prompt generation while maintaining human readability.

## 📄 INPUTS:

**Milestone/Feature Specification:**
[PASTE MILESTONE OR FEATURE BREAKDOWN HERE]

**Technical Design Specification:**
[PASTE RELEVANT TDS SECTIONS HERE]

**Game Design Document:**
[PASTE RELEVANT GDD SECTIONS HERE]