````markdown
# Tasks: CV Question Generation API

**Input**: Design documents from `/specs/001-cv-question-api/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/openapi.yaml

**Tests**: Tests are NOT included in this implementation (per spec - unit tests only, no integration tests needed at this time)

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 Remove WeatherForecast sample code from CvQuestionGenerator.API/Controllers/WeatherForecastController.cs and CvQuestionGenerator.API/WeatherForecast.cs
- [x] T002 Add Aspire.Azure.AI.OpenAI NuGet package to CvQuestionGenerator.API/CvQuestionGenerator.API.csproj
- [x] T003 [P] Add Microsoft.Extensions.AI NuGet package to CvQuestionGenerator.API/CvQuestionGenerator.API.csproj
- [x] T004 [P] Create AppConstants.cs in CvQuestionGenerator.API/ for AI prompt templates
- [x] T005 [P] Create directory structure: Models/, Services/, Endpoints/, Storage/ in CvQuestionGenerator.API/

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T006 Configure Azure OpenAI connection in CvQuestionGenerator.AppHost/AppHost.cs using AddConnectionString pattern
- [x] T007 Update CvQuestionGenerator.API/Program.cs to call AddAzureOpenAIClient and AddChatClient
- [x] T008 [P] Create IDataStore interface in CvQuestionGenerator.API/Storage/IDataStore.cs
- [x] T009 [P] Implement InMemoryDataStore in CvQuestionGenerator.API/Storage/InMemoryDataStore.cs with thread-safe singleton pattern
- [x] T010 [P] Create ProficiencyLevel enum in CvQuestionGenerator.API/Models/Skill.cs
- [x] T011 [P] Create SkillCategory enum in CvQuestionGenerator.API/Models/Skill.cs
- [x] T012 [P] Create DifficultyLevel enum in CvQuestionGenerator.API/Models/QuestionBlock.cs
- [x] T013 Register IDataStore as singleton in CvQuestionGenerator.API/Program.cs
- [x] T014 Remove Controllers configuration from CvQuestionGenerator.API/Program.cs (migrate to Minimal APIs)
- [x] T015 Configure RFC 7807 Problem Details support in CvQuestionGenerator.API/Program.cs

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Upload and Process CV (Priority: P1) 🎯 MVP

**Goal**: Allow HR professionals to upload CV text and receive AI-extracted structured data including skills with proficiency levels, work experience, and education

**Independent Test**: Upload CV via POST /api/cvs and verify structured extraction contains skills, experience, education. Retrieve via GET /api/cvs and verify complete data returned.

### Domain Models for User Story 1

- [x] T016 [P] [US1] Create PersonalInfo record in CvQuestionGenerator.API/Models/PersonalInfo.cs
- [x] T017 [P] [US1] Create Skill record in CvQuestionGenerator.API/Models/Skill.cs
- [x] T018 [P] [US1] Create WorkExperience record in CvQuestionGenerator.API/Models/WorkExperience.cs
- [x] T019 [P] [US1] Create Education record in CvQuestionGenerator.API/Models/Education.cs
- [x] T020 [US1] Create Cv record in CvQuestionGenerator.API/Models/Cv.cs (depends on T016-T019)

### Request/Response DTOs for User Story 1

- [x] T021 [P] [US1] Create UploadCvRequest record in CvQuestionGenerator.API/Models/Requests/UploadCvRequest.cs with validation attributes
- [x] T022 [P] [US1] Create CvExtractionResult record in CvQuestionGenerator.API/Models/Responses/CvExtractionResult.cs
- [x] T023 [P] [US1] Create CvUploadResponse record in CvQuestionGenerator.API/Models/Responses/CvUploadResponse.cs
- [x] T024 [P] [US1] Create CvResponse record in CvQuestionGenerator.API/Models/Responses/CvResponse.cs

### AI Prompts for User Story 1

- [x] T025 [US1] Add CV extraction prompt to CvQuestionGenerator.API/AppConstants.cs with schema definition and proficiency inference rules

### Services for User Story 1

- [x] T026 [P] [US1] Create IAiExtractionService interface in CvQuestionGenerator.API/Services/IAiExtractionService.cs
- [x] T027 [US1] Implement AiExtractionService.ExtractCvDataAsync in CvQuestionGenerator.API/Services/AiExtractionService.cs using IChatClient
- [x] T028 [P] [US1] Create ICvService interface in CvQuestionGenerator.API/Services/ICvService.cs
- [x] T029 [US1] Implement CvService in CvQuestionGenerator.API/Services/CvService.cs (upload, retrieve, uses IDataStore and IAiExtractionService)
- [x] T030 Register ICvService and IAiExtractionService in CvQuestionGenerator.API/Program.cs

### Endpoints for User Story 1

- [x] T031 [US1] Create CvEndpoints.cs in CvQuestionGenerator.API/Endpoints/ with MapCvEndpoints extension method
- [x] T032 [US1] Implement POST /api/cvs endpoint in CvEndpoints.cs with validation and error handling
- [x] T033 [US1] Implement GET /api/cvs endpoint in CvEndpoints.cs with 404 handling
- [x] T034 [US1] Call MapCvEndpoints in CvQuestionGenerator.API/Program.cs

**Checkpoint**: At this point, User Story 1 should be fully functional - can upload CVs, extract data, and retrieve stored CV

---

## Phase 4: User Story 2 - Upload and Process Job Description (Priority: P1)

**Goal**: Allow HR professionals to upload job description text and receive AI-extracted required skills, experience levels, and competencies

**Independent Test**: Upload job description via POST /api/jobs and verify structured extraction. Retrieve via GET /api/jobs and verify complete data returned.

### Domain Models for User Story 2

- [x] T035 [P] [US2] Create ExperienceRequirement record in CvQuestionGenerator.API/Models/ExperienceRequirement.cs
- [x] T036 [US2] Create JobDescription record in CvQuestionGenerator.API/Models/JobDescription.cs

### Request/Response DTOs for User Story 2

- [x] T037 [P] [US2] Create UploadJobRequest record in CvQuestionGenerator.API/Models/Requests/UploadJobRequest.cs with validation attributes
- [x] T038 [P] [US2] Create JobExtractionResult record in CvQuestionGenerator.API/Models/Responses/JobExtractionResult.cs
- [x] T039 [P] [US2] Create JobUploadResponse record in CvQuestionGenerator.API/Models/Responses/JobUploadResponse.cs
- [x] T040 [P] [US2] Create JobResponse record in CvQuestionGenerator.API/Models/Responses/JobResponse.cs

### AI Prompts for User Story 2

- [x] T041 [US2] Add Job extraction prompt to CvQuestionGenerator.API/AppConstants.cs with schema definition and requirement extraction rules

### Services for User Story 2

- [x] T042 [US2] Implement AiExtractionService.ExtractJobDataAsync in CvQuestionGenerator.API/Services/AiExtractionService.cs
- [x] T043 [P] [US2] Create IJobService interface in CvQuestionGenerator.API/Services/IJobService.cs
- [x] T044 [US2] Implement JobService in CvQuestionGenerator.API/Services/JobService.cs (upload, retrieve, uses IDataStore and IAiExtractionService)
- [x] T045 Register IJobService in CvQuestionGenerator.API/Program.cs

### Endpoints for User Story 2

- [x] T046 [US2] Create JobEndpoints.cs in CvQuestionGenerator.API/Endpoints/ with MapJobEndpoints extension method
- [x] T047 [US2] Implement POST /api/jobs endpoint in JobEndpoints.cs with validation and error handling
- [x] T048 [US2] Implement GET /api/jobs endpoint in JobEndpoints.cs with 404 handling
- [x] T049 [US2] Call MapJobEndpoints in CvQuestionGenerator.API/Program.cs

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently - can upload/retrieve CVs and Job Descriptions

---

## Phase 5: User Story 3 - Generate Interview Questions (Priority: P1)

**Goal**: Generate AI-powered interview questions matched to candidate skills vs job requirements with appropriate difficulty calibration

**Independent Test**: With CV and Job stored, call GET /api/questions and verify response contains topic-grouped questions with all required elements (question text, answer guidelines, key terms, follow-ups)

### Domain Models for User Story 3

- [x] T050 [P] [US3] Create KeyTerm record in CvQuestionGenerator.API/Models/KeyTerm.cs
- [x] T051 [P] [US3] Create InterviewQuestion record in CvQuestionGenerator.API/Models/InterviewQuestion.cs
- [x] T052 [US3] Create QuestionBlock record in CvQuestionGenerator.API/Models/QuestionBlock.cs

### Request/Response DTOs for User Story 3

- [x] T053 [US3] Create QuestionsResponse record in CvQuestionGenerator.API/Models/Responses/QuestionsResponse.cs

### AI Prompts for User Story 3

- [x] T054 [US3] Add question generation prompt to CvQuestionGenerator.API/AppConstants.cs with difficulty calibration rules and content validation constraints

### Services for User Story 3

- [x] T055 [P] [US3] Create IQuestionService interface in CvQuestionGenerator.API/Services/IQuestionService.cs
- [x] T056 [US3] Implement QuestionService.GenerateQuestionsAsync in CvQuestionGenerator.API/Services/QuestionService.cs (skill gap analysis, difficulty mapping, AI generation)
- [x] T057 Register IQuestionService in CvQuestionGenerator.API/Program.cs

### Endpoints for User Story 3

- [x] T058 [US3] Create QuestionEndpoints.cs in CvQuestionGenerator.API/Endpoints/ with MapQuestionEndpoints extension method
- [x] T059 [US3] Implement GET /api/questions endpoint in QuestionEndpoints.cs with CV/Job existence validation
- [x] T060 [US3] Call MapQuestionEndpoints in CvQuestionGenerator.API/Program.cs

**Checkpoint**: All P1 user stories complete - full MVP functionality available (upload CV, upload Job, generate questions)

---

## Phase 6: User Story 4 - Retrieve Stored CV (Priority: P2)

**Goal**: Already implemented in Phase 3 (T033)

**Note**: User Story 4 GET /api/cvs endpoint was implemented as part of User Story 1. No additional tasks needed.

---

## Phase 7: User Story 5 - Retrieve Stored Job Description (Priority: P2)

**Goal**: Already implemented in Phase 4 (T048)

**Note**: User Story 5 GET /api/jobs endpoint was implemented as part of User Story 2. No additional tasks needed.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [x] T061 [P] Update CvQuestionGenerator.API/appsettings.json with OpenAI deployment name configuration
- [x] T062 [P] Add XML documentation comments to all public APIs for OpenAPI generation
- [x] T063 [P] Update quickstart.md with actual curl examples after endpoints implemented
- [x] T064 [P] Write unit tests for CvService in CvQuestionGenerator.API.Tests/Services/CvServiceTests.cs
- [x] T065 [P] Write unit tests for JobService in CvQuestionGenerator.API.Tests/Services/JobServiceTests.cs
- [x] T066 [P] Write unit tests for QuestionService in CvQuestionGenerator.API.Tests/Services/QuestionServiceTests.cs
- [x] T067 [P] Write unit tests for InMemoryDataStore in CvQuestionGenerator.API.Tests/Storage/InMemoryDataStoreTests.cs
- [x] T068 Validate all error responses follow RFC 7807 Problem Details format
- [x] T069 Test AI service unavailability handling and 503 responses
- [ ] T070 Run quickstart.md validation with real Azure OpenAI connection

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on Foundational completion - Can run parallel with US1
- **User Story 3 (Phase 5)**: Depends on US1 and US2 completion (needs CV and Job models)
- **User Story 4 (Phase 6)**: Already completed in US1
- **User Story 5 (Phase 7)**: Already completed in US2
- **Polish (Phase 8)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Independent - Can start after Foundational
- **User Story 2 (P1)**: Independent - Can start after Foundational, parallel with US1
- **User Story 3 (P1)**: Requires US1 and US2 models (Cv, JobDescription, Skill)
- **User Story 4 (P2)**: Implemented in US1
- **User Story 5 (P2)**: Implemented in US2

### Within Each User Story

- Models before services
- Services before endpoints
- Endpoint registration in Program.cs after endpoint implementation
- Story complete before moving to next priority

### Parallel Opportunities Per Phase

**Phase 1 - Setup**: T002, T003, T004, T005 can run in parallel

**Phase 2 - Foundational**: T008-T012 can run in parallel

**Phase 3 - User Story 1**:
- Models: T016, T017, T018, T019 can run in parallel
- DTOs: T021, T022, T023, T024 can run in parallel
- Services: T026 and T028 (interfaces) can run in parallel

**Phase 4 - User Story 2**:
- Models: T035 (if started after T017 completes for Skill dependency)
- DTOs: T037, T038, T039, T040 can run in parallel
- Services: T043 (interface) can start early

**Phase 5 - User Story 3**:
- Models: T050, T051 can run in parallel
- Services: T055 (interface) can start early

**Phase 8 - Polish**: T061, T062, T063, T064, T065, T066, T067 can all run in parallel

---

## Parallel Example: User Story 1 Implementation

```bash
# Launch all model creation in parallel:
Task T016: "Create PersonalInfo record in CvQuestionGenerator.API/Models/PersonalInfo.cs"
Task T017: "Create Skill record in CvQuestionGenerator.API/Models/Skill.cs"
Task T018: "Create WorkExperience record in CvQuestionGenerator.API/Models/WorkExperience.cs"
Task T019: "Create Education record in CvQuestionGenerator.API/Models/Education.cs"

# Then create Cv record (depends on above):
Task T020: "Create Cv record in CvQuestionGenerator.API/Models/Cv.cs"

# Launch all DTO creation in parallel:
Task T021: "Create UploadCvRequest record"
Task T022: "Create CvExtractionResult record"
Task T023: "Create CvUploadResponse record"
Task T024: "Create CvResponse record"
```

---

## Implementation Strategy

### MVP First (User Stories 1, 2, 3 Only - All P1)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1 (Upload CV)
4. **VALIDATE**: Test CV upload/retrieval independently
5. Complete Phase 4: User Story 2 (Upload Job)
6. **VALIDATE**: Test Job upload/retrieval independently
7. Complete Phase 5: User Story 3 (Generate Questions)
8. **VALIDATE**: Full end-to-end test (upload CV → upload Job → generate questions)
9. Complete Phase 8: Polish and tests

### Incremental Delivery Strategy

1. **Week 1**: Setup + Foundational → Infrastructure ready
2. **Week 2**: User Story 1 → Demo CV extraction
3. **Week 3**: User Story 2 → Demo Job extraction
4. **Week 4**: User Story 3 → Demo full question generation (MVP complete!)
5. **Week 5**: Polish, tests, documentation

### Parallel Team Strategy

With multiple developers after Foundational phase completes:

- **Developer A**: User Story 1 (CV management)
- **Developer B**: User Story 2 (Job management) - Can work parallel with Dev A
- **Developer C**: Prepare User Story 3 scaffolding, write tests

After US1 and US2 complete:
- All developers collaborate on User Story 3 (question generation)

---

## Notes

- **Tests**: Unit tests only, deferred to Polish phase per spec requirement
- **Path convention**: Using CvQuestionGenerator.API/ prefix for all source files
- **Minimal APIs**: All endpoints use Minimal API pattern per constitution
- **Records**: Using C# 13 record types with required properties
- **Thread safety**: InMemoryDataStore uses lock for thread-safe singleton
- **AI prompts**: All AI prompts centralized in AppConstants.cs
- **Error handling**: RFC 7807 Problem Details for all error responses
- **Data persistence**: In-memory only, data lost on restart

````