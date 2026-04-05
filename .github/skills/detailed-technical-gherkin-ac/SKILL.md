---
name: detailed-technical-gherkin-ac
description: "Use when: user asks for technical acceptance criteria in Gherkin, system-level AC, integration AC, API/data/processing behavior in Given-When-Then format. Keywords: technical Gherkin AC, technical acceptance criteria, system AC, integration AC, backend AC, non-functional AC in Gherkin."
argument-hint: "Describe the system capability or feature (e.g. 'Excel upload validation' or 'batch processing retry behavior on university timeout')"
---

# Detailed Technical Gherkin AC

## Purpose

Generate detailed, testable, technical acceptance criteria in Gherkin format for system behavior.

Use this skill for API contracts, data validations, processing rules, status transitions, integrations, background jobs, observability, and non-functional constraints translated into verifiable scenarios.

## Input Checklist

Collect or infer these inputs before writing AC:

- Feature/system capability
- Trigger source (UI action, API call, scheduler, event)
- Preconditions (auth context, data state, dependency state)
- Input contracts (required fields, formats, limits)
- Processing rules and state transitions
- Integration touchpoints (internal/external)
- Error handling and retry expectations
- Audit/logging/traceability expectations
- Performance/security/reliability expectations (if in scope)

If details are missing, declare assumptions explicitly.

## Output Rules

- Use precise, testable, observable technical outcomes.
- Include both success and failure conditions.
- Cover boundary and idempotency/retry behavior where relevant.
- Avoid source code snippets.
- Keep scenario steps implementation-agnostic but technically verifiable.

## Structure Template

```gherkin
Feature: <Technical Capability>
  In order to <system objective>
  As a <system role/component>
  I want <technical behavior>

  Background:
    Given <system is operational>
    And <required dependencies are available>

  Scenario: <successful processing>
    Given <valid authenticated request/event>
    And <required data exists>
    When <operation is triggered>
    Then <expected response/status/result>
    And <state transition is persisted>
    And <audit/log is recorded>

  Scenario: <contract validation failure>
    Given <missing/invalid field or schema mismatch>
    When <operation is triggered>
    Then <validation error response is returned>
    And <no state change is persisted>

  Scenario: <downstream dependency failure>
    Given <dependency is unavailable or returns error>
    When <operation is triggered>
    Then <failure is handled per policy>
    And <retry/backoff or task creation behavior is applied>
    And <error is logged with correlation reference>

  Scenario: <authorization enforcement>
    Given <caller without required role/permission>
    When <restricted operation is triggered>
    Then <access is denied>
    And <no side effects occur>
```

## Non-Functional Scenario Patterns

Add when needed:

- Performance: response time or processing window thresholds
- Reliability: retry limits, dead-letter/manual task behavior
- Security: role checks, data masking, secure error responses
- Concurrency: duplicate submission/idempotency behavior

## Quality Bar

A strong technical AC set should include:

- Contract validation scenarios
- State persistence and transition checks
- Integration failure handling
- Observability outcomes (logs/audit/tracking)
- Security/authorization behavior
- NFR scenario(s) when requested

## Champversity-Oriented Anchors

When relevant, align scenarios to:

- Excel ingestion validation outcomes
- Application status transitions
- Batch processing cadence and retry behavior
- University response processing
- Interview slot allocation and selection consistency
- Manual task creation on exceptional flows

## Response Footer Format

After scenarios, add:

- Assumptions
- Technical dependencies
- Open technical questions
- Traceability tags (optional): Story-#, FR-#, NFR-#
