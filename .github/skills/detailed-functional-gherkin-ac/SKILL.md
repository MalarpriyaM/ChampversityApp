---
name: detailed-functional-gherkin-ac
description: "Use when: user asks for functional acceptance criteria, business Gherkin AC, BA/PO AC, user-facing scenarios, happy path and alternate flows, or Given-When-Then functional behavior without technical internals. Keywords: functional AC, business AC, Gherkin AC, acceptance criteria, user story AC, BA AC, PO AC, Product AC."
argument-hint: "Describe the feature or user story (e.g. 'student interview slot reschedule' or 'admin batch processing approval')"
---

# Detailed Functional Gherkin AC

## Purpose

Generate detailed, business-focused acceptance criteria in Gherkin format for BA/PO/PM workflows.

Use this skill for user outcomes, business rules, validations, role permissions, and workflow paths.

Do not include implementation details such as API internals, DB tables, service names, class names, or framework-specific code behavior.

## Input Checklist

Collect or infer these inputs before writing AC:

- Feature name
- User story (As a / I want / So that)
- Actors and roles (Student, Admin, Staff, University)
- Preconditions and assumptions
- Business rules and constraints
- Primary workflow and alternate/error paths
- Data-level validations in business language
- Out-of-scope notes

If required information is missing, state assumptions explicitly before scenarios.

## Output Rules

- Output only Gherkin-friendly AC sections plus assumptions.
- Write clear role-based scenarios.
- Cover positive, negative, boundary, and exception paths.
- Use business terms from Champversity domain.
- Keep one behavioral intent per scenario.
- Prefer Scenario Outline where multiple combinations are needed.

## Structure Template

```gherkin
Feature: <Feature Name>
  As a <role>
  I want <capability>
  So that <business value>

  Background:
    Given <global precondition>
    And <role is authenticated/authorized if needed>

  # Happy path
  Scenario: <primary business flow>
    Given <initial business state>
    When <business action>
    Then <expected business outcome>
    And <status/notification/visibility outcome>

  # Validation/negative path
  Scenario: <validation failure>
    Given <invalid or missing business input>
    When <action>
    Then <business error message>
    And <no invalid state transition occurs>

  # Alternate path
  Scenario: <alternate flow>
    Given <alternate precondition>
    When <action>
    Then <alternate expected outcome>

  # Permission path
  Scenario: <access control behavior>
    Given <user role>
    When <restricted action>
    Then <access result>
```

## Quality Bar

A strong functional AC set should include:

- End-to-end happy path
- At least 2 validation/error scenarios
- At least 1 role/permission scenario (if role-based behavior exists)
- Clear business-state transitions
- Observable user-facing outcomes (UI/state/notification)

## Domain Anchors For Champversity

When relevant, align wording with existing flow:

- Template download
- Excel upload
- Validation pass/fail
- Admin review/manual task
- University response processing
- Interview slot selection
- Admission/rejection decision

## Response Footer Format

After scenarios, add:

- Assumptions
- Open questions for stakeholders
- Traceability tags (optional): FR-#, NFR-#, Story-#
