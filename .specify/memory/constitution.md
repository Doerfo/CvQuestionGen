<!--
================================================================================
CONSTITUTION SYNC IMPACT REPORT
================================================================================
Version Change: Initial → 1.0.0
Change Type: MAJOR (Initial constitution establishment)
Date: 2025-12-11

Principles Defined:
  - I. ASPIRE-First Architecture (new)
  - II. Privacy-First Development (new - NON-NEGOTIABLE)
  - III. AI Quality Assurance (new)

Sections Added:
  - Technology Stack Standards
  - API Standards

Templates Review:
  ✅ plan-template.md - Reviewed, consistent
  ✅ spec-template.md - Reviewed, consistent
  ✅ tasks-template.md - Reviewed, consistent

Follow-up Actions:
  - None - all templates aligned with constitution principles

Rationale for v1.0.0:
  - Initial constitution establishment for CvQuestionGen project
  - Defines core architectural, privacy, and AI quality principles
  - Sets foundation for .NET Aspire distributed application development
================================================================================
-->

# CvQuestionGen Constitution

## Core Principles

### I. ASPIRE-First Architecture

Follow .NET Aspire distributed application patterns strictly. This principle MUST govern all architectural decisions:

- **AppHost Project**: MUST be used for orchestration and service discovery. All service configuration and dependencies are declared here.
- **ServiceDefaults Project**: MUST implement shared telemetry, health checks, and resilience patterns. All distributed application concerns are centralized here.
- **Azure Service Integration**: All Azure services MUST be integrated using Aspire components. Direct SDK usage without Aspire abstraction is forbidden.
- **Project Structure**: The repository MUST maintain exactly three projects: API (main service), AppHost (orchestration), and ServiceDefaults (shared configuration). Additional projects require constitutional amendment justification.
- **Distributed Application Patterns**: Services communicate through well-defined contracts with built-in observability and resilience.

**Rationale**: .NET Aspire provides opinionated, cloud-ready patterns for distributed applications. Deviating from these patterns undermines observability, testability, and operational excellence. The three-project structure ensures separation of concerns while maintaining cohesion.

### II. Privacy-First Development (NON-NEGOTIABLE)

CVs contain sensitive Personally Identifiable Information (PII) and MUST be handled with GDPR compliance as an absolute requirement:

- **Input Validation**: All user inputs MUST be validated and sanitized before processing. No raw user data may reach AI services or storage without validation.
- **Data Retention**: Data retention policies are MANDATORY. Default retention period must be defined and enforced programmatically.
- **Deletion Capabilities**: Users MUST have the ability to delete their data on demand. Deletion must be verifiable and complete across all systems.
- **Audit Trails**: All operations involving sensitive data MUST be logged with user ID, timestamp, operation type, and data scope. Audit logs are immutable.
- **Encryption**: Data MUST be encrypted at rest and in transit. Use Azure Key Vault for key management. No hardcoded secrets.
- **Consent-Based Storage**: CV data MUST NOT be permanently stored without explicit user consent. Temporary processing is permitted with automatic expiration.
- **PII Minimization**: Only collect and process the minimum PII necessary for question generation. Strip unnecessary metadata.

**Rationale**: Legal compliance is non-negotiable. GDPR violations carry severe penalties and reputational damage. Privacy-by-design prevents compliance debt and builds user trust. This principle supersedes all performance and feature considerations.

### III. AI Quality Assurance

AI-generated interview questions MUST be validated before delivery to ensure fairness, legality, and relevance:

- **Content Validation**: Every generated question MUST be unbiased, non-discriminatory, role-appropriate, and evidence-based on actual CV content.
- **Content Filtering**: MUST implement Azure AI Content Safety to detect and block inappropriate, discriminatory, or legally problematic outputs.
- **Prompt Management**: All prompts MUST be versioned in source control. Track performance metrics per prompt version. Implement A/B testing for prompt improvements.
- **Security Monitoring**: MUST monitor for prompt injection attempts and adversarial inputs. Log and alert on suspicious patterns.
- **Legal Compliance**: Generated questions MUST comply with employment law (e.g., avoid questions about protected characteristics: age, religion, marital status, health).
- **Cost Management**: MUST implement response caching to reduce API costs. Use semantic similarity to detect duplicate requests.
- **Resilience**: MUST implement rate limiting to prevent quota exhaustion. Use retry policies with exponential backoff for transient failures.

**Rationale**: Unvalidated AI outputs pose legal, ethical, and reputational risks. HR contexts demand high quality standards. Poor questions damage hiring outcomes and expose organizations to discrimination claims. Cost controls prevent budget overruns in production.

## Technology Stack Standards

The following technology standards MUST be followed to ensure modern, maintainable, and performant code:

- **Language Version**: C# 13 with .NET 10. MUST use modern language features:
  - Primary constructors for concise initialization
  - Collection expressions for readable data structures
  - Required properties for compile-time null safety
- **API Style**: MUST use Minimal APIs for endpoints. Controllers are forbidden unless constitutional justification provided.
- **Dependency Injection**: MUST use .NET built-in DI container configured in ServiceDefaults.

**Rationale**: Modern C# features reduce boilerplate and improve safety. Minimal APIs align with .NET Aspire's lightweight approach. Consistency across the codebase reduces cognitive load and onboarding time.

## API Standards

RESTful API design MUST follow these conventions to ensure consistency, discoverability, and maintainability:

- **RESTful Conventions**: Resources are nouns, actions are HTTP verbs. MUST use appropriate HTTP verbs:
  - GET for reads (idempotent)
  - POST for creation
  - PUT/PATCH for updates
  - DELETE for deletion
- **HTTP Status Codes**: MUST use semantically correct status codes:
  - 200 OK for successful GET
  - 201 Created for successful POST
  - 204 No Content for successful DELETE
  - 400 Bad Request for validation failures
  - 401 Unauthorized for missing/invalid auth
  - 403 Forbidden for insufficient permissions
  - 404 Not Found for missing resources
  - 500 Internal Server Error for unhandled exceptions
- **Error Responses**: MUST use RFC 7807 Problem Details for all error responses. Include `type`, `title`, `status`, `detail`, and `instance`.
- **Data Transfer Objects (DTOs)**: MUST use DTOs for all request and response models. Never expose domain entities directly.
- **Request Validation**: MUST validate all requests using FluentValidation or DataAnnotations before processing.
- **API Documentation**: OpenAPI/Swagger documentation is MANDATORY. All endpoints, parameters, and response schemas must be documented.

**Rationale**: Consistent API design reduces integration friction for consumers. Problem Details provides machine-readable error information. DTOs decouple API contracts from domain model evolution. Validation at the API boundary prevents invalid state.

## Governance

This constitution supersedes all other development practices, coding standards, and architectural decisions. It serves as the source of truth for technical decision-making.

### Amendment Process

- **Proposal**: Any team member may propose amendments with justification and impact analysis.
- **Review**: Amendments require documentation of rationale, affected systems, and migration plan.
- **Approval**: Constitutional changes require explicit approval (define approval authority based on team structure).
- **Migration**: Breaking changes require migration guide and backward compatibility period where feasible.

### Compliance

- **Code Reviews**: All pull requests MUST verify constitutional compliance before merge.
- **Complexity Justification**: Any deviation from constitutional principles MUST be explicitly justified in planning documents.
- **Monitoring**: Periodic reviews ensure ongoing compliance and identify opportunities for constitutional refinement.

### Runtime Development Guidance

For day-to-day development decisions and implementation patterns not covered by constitutional principles, refer to project-specific guidance documents as they are created (e.g., coding standards, architecture decision records, runbooks).

**Version**: 1.0.0 | **Ratified**: 2025-12-11 | **Last Amended**: 2025-12-11
