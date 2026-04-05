# Copilot Instructions — Champversity BA/PO/PM Workflows

## About This Product

**Champversity** is a student masters consultation and admission services platform.

User Role : What they Do
**Students** : Download Excel application template → Fill it out → Upload to system → Track application status → Select interview slots 
**Universities** : Receive batched student applications → Send back interview slot offers → Respond with admission decisions 
**Admin/Staff** : Validate applications → Batch process to universities → Review responses → Manage interview scheduling 

**Current Status:** Legacy application, ASP.NET Core 10.0 MVC with SQLite (local dev) / SQL Server (production)

---

## Your Workflows & Resources

### 1. **Requirements Gathering & Documentation**

**When:** Clarifying features, writing Functional Requirements (FR) and Non-Functional Requirements (NFR), creating PRDs

**Key Resources:**
- **[COMPLETE_WORKFLOW_TESTING_GUIDE.md](../COMPLETE_WORKFLOW_TESTING_GUIDE.md)** — Step-by-step user workflows (student, admin, university flows) — use for understanding current user journeys
- **Confluence spaces** — Link PRDs and requirement docs here for team alignment
- **Jira backlog** — Requirements already tracked as Epics/Stories

**Copilot Tips:**
- Ask: *"Generate To be FR and NFR for [feature] based on the student workflow"*
- Use: `requirements-gen` agent to consolidate feedback from Confluence + Jira + user interviews

---

### 2. **Gap Analysis & Feature Planning**

**When:** Prioritizing features, assessing scope, finding missing coverage

**Key Questions to Ask Copilot:**
- *"What gaps exist in the current application flow vs. our requirements?"*
- *"Which admin features are missing to handle [scenario]?"*
- *"Compare the current interview slot system against [new requirement]"*

**Copilot Agent:**
- Use: `gap-analysis` agent to identify inconsistencies, missing coverage, and blind spots

---

### 3. **Backlog Management**

**When:** Writing Jira stories, creating epics, sprint planning

**Current System:**
- **2 codebases:** `Champversity.Web` (UI, controllers, views) and `Champversity.DataAccess` (database, services)
- **User types:** Student, Admin/Staff, University (mocked)
- **Key workflows:** Application submission → Validation → University batching → Interview scheduling → Admission

**Copilot Agent:**
- Use: `backlog-creator` to generate Jira Epics, Features, and Stories with acceptance criteria
- Example: *"Create Jira stories for adding a new application status 'Waitlisted' across the system"*

---

### 4. **User Research & Personas**

**When:** Understanding user needs, documenting personas, analyzing feedback

**Key Artifacts:**
- **User workflows** — See COMPLETE_WORKFLOW_TESTING_GUIDE.md for current flows
- **Design files** — Figma prototypes and wireframes
- **User feedback docs** — Support tickets, interviews, surveys

**Copilot Tips:**
- Ask: *"Generate personas for [student type/admin role] based on Figma prototypes"*
- Ask: *"What pain points exist in the current [interview slot] workflow?"*

---

## Using Copilot for BA/PO/PM Tasks

**Copilot Agents Available:**
- `reverse-engg` — Understand about the current system architecture, data models, and workflows from code and documentation
- `requirements-gen` — Consolidate and synthesize requirements from Confluence, Jira, user feedback, As-is context
- `gap-analysis` — Identify missing features, inconsistencies, coverage gaps, gaps between As-is and To-be states
- `backlog-creator` — Generate Jira epics, features, user stories with detailed acceptance criteria
- `doc-publisher` — Write and publish documentation to Confluence

**Example Prompts:**
- *"Write a status report summarizing application processing metrics"*
- *"Generate a PRD for [feature] using the current system architecture"*
- *"Create user stories for the interview slot selection redesign"*
- *"Identify gaps between our Confluence requirements and the current Jira backlog"*
- *"Write a status report summarizing application processing metrics"*

---

## Common Workflows

### Writing a User Story
1. Ask Copilot: *"Create a Jira user story for [feature] with acceptance criteria"*
2. Copilot generates story with:
   - User perspective
   - Acceptance criteria (testable)
   - Business value
   - Design considerations

### Analyzing Feature Gaps
1. Ask Copilot: *"What gaps exist for [requirement] vs. current system?"*
2. Copilot identifies:
   - Missing workflows
   - Incomplete coverage
   - Conflicting requirements

### Creating a PRD
1. Ask Copilot: *"Generate a PRD for [feature] including user flows, success metrics, and open questions"*
2. Copilot creates structured PRD ready for stakeholder review

---

## Tips & Best Practices

**Do this:**
- Reference Figma prototypes, Confluence docs, and Jira stories when briefing Copilot
- Never skip the questions/human checkpoints
- Store final outputs in Confluence/Jira based on user's preferences
- Alwyays make sure traceability exists between requirements, user stories, and documentation

**Avoid:**
- Asking for code or technical implementation details — that's for developers
- Treating Copilot outputs as final without stakeholder review
- Creating siloed documentation — link everything
