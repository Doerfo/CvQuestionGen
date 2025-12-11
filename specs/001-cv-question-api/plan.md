\`\`\`\`markdown
# Implementation Plan: CV Question Generation API

**Branch**: \`001-cv-question-api\` | **Date**: 2025-12-11 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from \`/specs/001-cv-question-api/spec.md\`

**Note**: This template is filled in by the \`/speckit.plan\` command. See \`.specify/templates/commands/plan.md\` for the execution workflow.

## Summary

Build a REST API for HR professionals to generate AI-powered interview questions based on candidate CVs and job descriptions. The API will use Azure OpenAI via .NET Aspire integration to extract structured data from unformatted CV and job description text, then generate topic-grouped interview questions with difficulty calibrated to candidate proficiency levels.

## Technical Context

**Language/Version**: C# 13 with .NET 10  
**Primary Dependencies**: .NET Aspire 13.x, Aspire.Azure.AI.OpenAI, Microsoft.Extensions.AI, Microsoft.AspNetCore.OpenApi  
**Storage**: In-memory only (single CV + single job description per instance)  
**Testing**: xUnit (existing CvQuestionGenerator.API.Tests project)  
**Target Platform**: Linux/Windows server (containerized via Aspire)  
**Project Type**: Web API (.NET Aspire distributed application)  
**Performance Goals**: CV processing <30s, Job processing <30s, Question generation <60s  
**Constraints**: 50,000 character limit for CV/Job text, English language only  
**Scale/Scope**: Single-user in-memory storage, stateless API design

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. ASPIRE-First Architecture
| Requirement | Status | Notes |
|-------------|--------|-------|
| AppHost for orchestration | ✅ PASS | Existing AppHost project will be extended with OpenAI connection |
| ServiceDefaults for shared concerns | ✅ PASS | Existing ServiceDefaults project with telemetry/health checks |
| Azure services via Aspire components | ✅ PASS | Will use \`AddAzureOpenAIClient\` or \`AddConnectionString\` pattern |
| 3-project structure maintained | ✅ PASS | API, AppHost, ServiceDefaults + Tests (tests don't count) |

### II. Privacy-First Development (NON-NEGOTIABLE)
| Requirement | Status | Notes |
|-------------|--------|-------|
| Input validation before processing | ✅ PASS | Will implement FluentValidation/DataAnnotations |
| PII minimization | ✅ PASS | FR-019: Discard contact details after extraction |
| Data retention policies | ✅ PASS | In-memory only, lost on restart per A-004 |
| Consent-based storage | ✅ PASS | Temporary processing only, no persistence |

### III. AI Quality Assurance
| Requirement | Status | Notes |
|-------------|--------|-------|
| Content validation | ⚠️ NEEDS DESIGN | Must ensure questions are unbiased, non-discriminatory |
| Prompt management in source control | ✅ PASS | Prompts stored in AppConstants.cs |
| Rate limiting/resilience | ✅ PASS | ServiceDefaults includes resilience handlers |
| Cost management via caching | ⚠️ DEFERRED | Out of scope for initial version per A-006 |

### IV. Technology Stack Standards
| Requirement | Status | Notes |
|-------------|--------|-------|
| C# 13 / .NET 10 | ✅ PASS | Project already configured for net10.0 |
| Minimal APIs | ⚠️ NEEDS MIGRATION | Current setup uses Controllers - will migrate to Minimal APIs |
| Modern language features | ✅ PASS | Will use primary constructors, collection expressions, required properties |

### V. API Standards
| Requirement | Status | Notes |
|-------------|--------|-------|
| RESTful conventions | ✅ PASS | Spec defines proper resource naming |
| RFC 7807 Problem Details | ✅ PASS | Will implement for all errors |
| DTOs for all requests/responses | ✅ PASS | Will create dedicated DTOs |
| OpenAPI documentation | ✅ PASS | Microsoft.AspNetCore.OpenApi already referenced |

**Pre-Phase 0 Gate Status**: ✅ PASS (with noted design requirements)

## Project Structure

### Documentation (this feature)

\`\`\`text
specs/001-cv-question-api/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
\`\`\`

### Source Code (repository root)

\`\`\`text
CvQuestionGenerator/
├── CvQuestionGenerator.slnx              # Solution file
├── CvQuestionGenerator.API/              # Main API project
│   ├── Program.cs                        # Entry point with Minimal API routes
│   ├── AppConstants.cs                   # AI prompts and constants (NEW)
│   ├── Models/                           # Domain models and DTOs (NEW)
│   │   ├── Cv.cs
│   │   ├── JobDescription.cs
│   │   ├── Skill.cs
│   │   ├── QuestionBlock.cs
│   │   ├── InterviewQuestion.cs
│   │   └── Requests/                     # Request DTOs
│   │       ├── UploadCvRequest.cs
│   │       └── UploadJobRequest.cs
│   │   └── Responses/                    # Response DTOs
│   │       ├── CvResponse.cs
│   │       ├── JobResponse.cs
│   │       └── QuestionsResponse.cs
│   ├── Services/                         # Business logic (NEW)
│   │   ├── ICvService.cs
│   │   ├── CvService.cs
│   │   ├── IJobService.cs
│   │   ├── JobService.cs
│   │   ├── IQuestionService.cs
│   │   ├── QuestionService.cs
│   │   ├── IAiExtractionService.cs
│   │   └── AiExtractionService.cs
│   ├── Endpoints/                        # Minimal API endpoint definitions (NEW)
│   │   ├── CvEndpoints.cs
│   │   ├── JobEndpoints.cs
│   │   └── QuestionEndpoints.cs
│   └── Storage/                          # In-memory storage (NEW)
│       ├── IDataStore.cs
│       └── InMemoryDataStore.cs
├── CvQuestionGenerator.AppHost/          # Aspire orchestration
│   └── AppHost.cs                        # Updated with OpenAI connection
├── CvQuestionGenerator.ServiceDefaults/  # Shared configuration
│   └── Extensions.cs                     # Existing telemetry/resilience
└── CvQuestionGenerator.API.Tests/        # Unit tests
    └── Services/                         # Service tests (NEW)
        ├── CvServiceTests.cs
        ├── JobServiceTests.cs
        └── QuestionServiceTests.cs
\`\`\`

**Structure Decision**: Using existing .NET Aspire 3-project structure with API, AppHost, and ServiceDefaults. New folders (Models, Services, Endpoints, Storage) will be added to the API project following Minimal API patterns.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Minimal API migration | Constitution requires Minimal APIs | Controllers were scaffolded by template - will remove/replace |

\`\`\`\`

## Constitution Check: Post-Phase 1 Re-evaluation

*Re-evaluated after Phase 1 design completion*

### I. ASPIRE-First Architecture
| Requirement | Status | Notes |
|-------------|--------|-------|
| AppHost for orchestration | ✅ PASS | AppHost.cs updated pattern in research.md |
| ServiceDefaults for shared concerns | ✅ PASS | Existing setup sufficient |
| Azure services via Aspire components | ✅ PASS | Using `Aspire.Azure.AI.OpenAI` package |
| 3-project structure maintained | ✅ PASS | No additional projects needed |

### II. Privacy-First Development (NON-NEGOTIABLE)
| Requirement | Status | Notes |
|-------------|--------|-------|
| Input validation before processing | ✅ PASS | DataAnnotations on DTOs defined in data-model.md |
| PII minimization | ✅ PASS | PersonalInfo record excludes contact details |
| Data retention policies | ✅ PASS | InMemoryDataStore clears on restart |
| Consent-based storage | ✅ PASS | No persistent storage designed |

### III. AI Quality Assurance
| Requirement | Status | Notes |
|-------------|--------|-------|
| Content validation | ✅ PASS | Prompt constraints defined in research.md |
| Prompt management in source control | ✅ PASS | AppConstants.cs location confirmed |
| Rate limiting/resilience | ✅ PASS | Aspire ServiceDefaults resilience handlers |
| Cost management via caching | ⚠️ DEFERRED | Explicitly out of scope |

### IV. Technology Stack Standards
| Requirement | Status | Notes |
|-------------|--------|-------|
| C# 13 / .NET 10 | ✅ PASS | Using records, required properties, collection expressions |
| Minimal APIs | ✅ PASS | Endpoint pattern defined in research.md |
| Modern language features | ✅ PASS | data-model.md uses sealed records with required |

### V. API Standards
| Requirement | Status | Notes |
|-------------|--------|-------|
| RESTful conventions | ✅ PASS | OpenAPI contract follows REST patterns |
| RFC 7807 Problem Details | ✅ PASS | Error responses defined in openapi.yaml |
| DTOs for all requests/responses | ✅ PASS | All DTOs defined in data-model.md |
| OpenAPI documentation | ✅ PASS | Full OpenAPI 3.1 spec in contracts/ |

**Post-Phase 1 Gate Status**: ✅ PASS - Ready for Phase 2 task generation

