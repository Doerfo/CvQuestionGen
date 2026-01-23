# Implementation Tasks

**Change ID:** `add-cv-question-api`

## Task Checklist

### Phase 1: Project Setup and Configuration

- [x] **1.1 Configure Aspire Azure OpenAI Connection**
  - Add Azure OpenAI connection string to AppHost configuration
  - Configure AppHost with `.AddConnectionString("open-ai")` and project references
  - Add Swagger UI endpoint configuration in AppHost
  - **Validation:** AppHost builds and connection string is available

- [x] **1.2 Configure API Key Authentication**
  - Add Authentication configuration section to appsettings.json and appsettings.Development.json
  - Set up User Secrets for API key (development)
  - Document environment variables needed for production
  - **Validation:** Configuration loads successfully on application startup

- [x] **1.3 Install Required NuGet Packages**
  - Add Aspire.Azure.AI.OpenAI package to API project
  - Add Microsoft.AspNetCore.Authentication package
  - Add Swashbuckle.AspNetCore for Swagger with security definitions
  - **Validation:** Packages restore without conflicts

### Phase 2: Core Data Models

- [x] **2.1 Create Proficiency Level Enum**
  - Define ProficiencyLevel enum (Beginner, Intermediate, Advanced, Expert)
  - Place in Models namespace
  - **Validation:** Enum compiles and can be serialized to JSON

- [x] **2.2 Create CV Data Models**
  - Create CVData, CVExtractedData, PersonalInfo, Skill, WorkExperience, Education classes
  - Add data annotations for validation
  - **Validation:** Models serialize/deserialize correctly with System.Text.Json

- [x] **2.3 Create Job Description Data Models**
  - Create JobDescriptionData, JobDescExtractedData, RequiredSkill classes
  - Add data annotations for validation
  - **Validation:** Models serialize/deserialize correctly with System.Text.Json

- [x] **2.4 Create Question Generation Data Models**
  - Create QuestionSet, TopicGroup, Question, KeyTerm classes
  - Add data annotations for validation
  - **Validation:** Models serialize/deserialize correctly with System.Text.Json

- [x] **2.5 Create AppConstants Class**
  - Create AppConstants.cs in API project root
  - Add nested Prompts class with all AI prompt templates (CV extraction, job desc extraction, question generation)
  - Add any other constants needed
  - **Validation:** AppConstants class compiles and prompts are accessible

### Phase 3: AI Service Integration

- [x] **3.1 Create AI Service Interface**
  - Define IAIService interface with methods: ExtractCVData, ExtractJobDescriptionData, GenerateQuestions
  - Methods should accept chat client and use prompts from AppConstants
  - **Validation:** Interface compiles

- [x] **3.2 Implement AI Service with Aspire Chat Client**
  - Create AIService implementing IAIService
  - Inject ChatClient from Aspire (via DI)
  - Use prompts from AppConstants.Prompts
  - Implement retry logic with exponential backoff
  - **Validation:** Service can make successful test call to AI endpoint using chat client

- [x] **3.3 Implement AI Response Parsing**
  - Add JSON schema validation for AI responses
  - Implement error handling for malformed responses
  - Add fallback logic for parsing failures
  - **Validation:** Parser handles both valid and invalid AI responses gracefully

### Phase 4: Repository Layer (Singleton Storage)

- [x] **4.1 Create CV Repository Interface and Implementation**
  - Define ICVRepository interface (Set, Get, Exists methods)
  - Create SingletonCVRepository using a single nullable field
  - Set method replaces existing CV
  - **Validation:** Can store and retrieve one CV at a time; new CV replaces old one

- [x] **4.2 Create Job Description Repository Interface and Implementation**
  - Define IJobDescRepository interface (Set, Get, Exists methods)
  - Create SingletonJobDescRepository using a single nullable field
  - Set method replaces existing job description
  - **Validation:** Can store and retrieve one job description at a time; new one replaces old one

### Phase 5: Authentication

- [x] **5.1 Create API Key Authentication Handler**
  - Create ApiKeyAuthenticationHandler implementing AuthenticationHandler
  - Validate API key from X-API-Key header against configuration
  - Return 401 Unauthorized for missing/invalid keys
  - **Validation:** Handler correctly authenticates valid keys and rejects invalid ones

- [x] **5.2 Configure Swagger with API Key Support**
  - Add OpenApiSecurityScheme for ApiKey in header
  - Add OpenApiSecurityRequirement to all endpoints
  - Configure Swagger UI to display API key input field
  - **Validation:** Swagger UI shows API key input and applies it to requests

### Phase 6: Service Layer

- [x] **6.1 Create CV Service**
  - Define ICVService interface
  - Implement CVService with methods: SubmitCV (replaces existing), GetCV
  - Integrate with IAIService for extraction (inject ChatClient)
  - Integrate with ICVRepository for storage (Set/Get pattern)
  - Add input validation
  - **Validation:** Unit tests pass for CV submission (replacing) and retrieval with mocked dependencies

- [x] **6.2 Create Job Description Service**
  - Define IJobDescService interface
  - Implement JobDescService with methods: SubmitJobDescription (replaces existing), GetJobDescription
  - Integrate with IAIService for extraction (inject ChatClient)
  - Integrate with IJobDescRepository for storage (Set/Get pattern)
  - Add input validation
  - **Validation:** Unit tests pass for job description submission (replacing) and retrieval with mocked dependencies

- [x] **6.3 Create Question Generation Service**
  - Define IQuestionGenerationService interface
  - Implement QuestionGenerationService with GenerateQuestions method (no parameters)
  - Check if CV and job description exist using repository Exists methods
  - Retrieve current CV and job description from repositories
  - Call IAIService to generate questions (inject ChatClient)
  - Implement skill-to-requirement matching logic
  - Implement difficulty calibration algorithm
  - Return 400 error if CV or job description missing
  - **Validation:** Unit tests pass for question generation with mocked dependencies

### Phase 7: API Controllers

- [x] **7.1 Create CV Controller**
  - Implement POST /api/cvs endpoint (submit CV, returns 204 No Content)
  - Implement GET /api/cvs endpoint (retrieve current CV, no ID parameter)
  - Add [Authorize] attribute for API key authentication
  - Add request/response DTOs if needed
  - Add proper HTTP status codes (204, 200, 400, 401, 404)
  - **Validation:** Controller methods compile and can be invoked

- [x] **7.2 Create Job Description Controller**
  - Implement POST /api/jobs endpoint (submit job description, returns 204 No Content)
  - Implement GET /api/jobs endpoint (retrieve current job description, no ID parameter)
  - Add [Authorize] attribute for API key authentication
  - Add request/response DTOs if needed
  - Add proper HTTP status codes (204, 200, 400, 401, 404)
  - **Validation:** Controller methods compile and can be invoked

- [x] **7.3 Create Question Controller**
  - Implement GET /api/questions endpoint (no query parameters)
  - Add [Authorize] attribute for API key authentication
  - Add validation error handling (CV/job description missing)
  - Add proper HTTP status codes (200, 400, 401)
  - **Validation:** Controller methods compile and can be invoked

### Phase 8: Dependency Injection and Registration

- [x] **7.1 Register Services in Program.cs**
  - Register IAIService and implementation
  - Register repositories as singletons
  - Register services as scoped
  - Configure IHttpClientFactory for AI service
  - Bind configuration options
  - **Validation:** Application starts without DI errors

### Phase 9: Testing

- [x] **9.1 Write Unit Tests for CV Service**
  - Test SubmitCV with valid input
  - Test SubmitCV with invalid input (null/empty)
  - Test GetCV with existing ID
  - Test GetCV with non-existent ID
  - Mock ChatClient and ICVRepository
  - **Validation:** All unit tests pass

- [x] **9.2 Write Unit Tests for Job Description Service**
  - Test SubmitJobDescription with valid input
  - Test SubmitJobDescription with invalid input
  - Test GetJobDescription with existing ID
  - Test GetJobDescription with non-existent ID
  - Mock ChatClient and IJobDescRepository
  - **Validation:** All unit tests pass

- [x] **9.3 Write Unit Tests for Question Generation Service**
  - Test GenerateQuestions with valid CV and job description
  - Test GenerateQuestions with non-existent CV
  - Test GenerateQuestions with non-existent job description
  - Test skill matching logic
  - Test difficulty calibration algorithm
  - Mock ChatClient and repositories
  - **Validation:** All unit tests pass

- [x] **9.4 Write Unit Tests for API Key Authentication**
  - Test authentication handler with valid API key
  - Test authentication handler with invalid API key
  - Test authentication handler with missing API key
  - **Validation:** All unit tests pass

### Phase 10: Manual Testing and Documentation

- [x] **10.1 Create Sample Test Data**
  - Create 2-3 sample CV texts covering different skill levels
  - Create 2-3 sample job description texts
  - **Validation:** Test data files exist and are ready for use

- [ ] **10.2 Manual End-to-End Testing via Swagger**
  - Test API key authentication in Swagger UI
  - Submit sample CVs via API and verify extraction quality (verify replacement behavior)
  - Submit sample job descriptions via API and verify extraction quality (verify replacement behavior)
  - Generate questions without uploading CV/job description first (verify 400 errors)
  - Generate questions for various CV/job description combinations
  - Verify question quality, difficulty levels, and recruiter-friendly explanations
  - Test error scenarios (missing data, missing/invalid API key)
  - **Validation:** All manual test scenarios produce expected results

- [x] **10.3 Create API Documentation**
  - Document all endpoints with request/response examples (note singleton pattern)
  - Add configuration setup instructions (Aspire, API keys)
  - Add troubleshooting guide
  - Create README with setup and usage instructions
  - **Validation:** Documentation is clear and complete

- [x] **10.4 Update OpenAPI/Swagger Documentation**
  - Add XML comments to controllers for OpenAPI generation
  - Verify Swagger UI displays all endpoints correctly
  - Add example request/response payloads
  - **Validation:** Swagger UI is functional and accurate

### Phase 11: Final Validation

- [x] **11.1 Code Review Checklist**
  - Verify all error handling is in place
  - Verify all validation is implemented
  - Verify logging is added where appropriate
  - Verify no sensitive data (API keys) in source control
  - Verify code follows project conventions
  - **Validation:** Code review passes

- [x] **11.2 Final Testing Pass**
  - Run all unit tests
  - Perform final manual testing via Swagger with API key
  - Test with real Azure OpenAI chat client (not mocked)
  - **Validation:** All tests pass; system works end-to-end

## Task Dependencies

**Sequential (must be completed in order):**
- Phase 1 → Phase 2 (need configuration before creating models)
- Phase 2 → Phase 3 (need models and AppConstants before implementing AI service)
- Phase 3 → Phase 4 (need AI service interface before repositories)
- Phase 4 → Phase 5 (need repositories before authentication)
- Phase 5 → Phase 6 (need authentication before services)
- Phase 6 → Phase 7 (need services before controllers)
- Phase 7 → Phase 8 (need controllers before registering)
- Phase 8 → Phase 9 (need working DI before testing)
- Phase 9 → Phase 10 (need unit tests passing before manual testing)
- Phase 10 → Phase 11 (need manual testing before final validation)

**Parallelizable:**
- Within Phase 2: All model creation tasks can be done in parallel
- Within Phase 4: CV and job description repositories can be implemented in parallel
- Within Phase 6: CV and job description services can be implemented in parallel (question service depends on both)
- Within Phase 7: CV and job description controllers can be implemented in parallel (question controller depends on both)
- Within Phase 9: All unit test tasks can be written in parallel

## Estimated Effort

- **Total Tasks:** 42
- **Estimated Time:** 3-5 days for experienced developer
- **Critical Path:** Phases 1-8 (setup through DI registration)
- **Testing:** ~30% of total effort

## Notes

- Each task should produce a small, verifiable increment of progress
- Run `dotnet build` frequently to catch compilation issues early
- Commit after completing each phase
- API keys must never be committed to source control (use User Secrets/Key Vault)
- Use Aspire dashboard to monitor Azure OpenAI connection
- Test API key authentication thoroughly in Swagger UI
- All prompts must be in AppConstants.cs, not hardcoded in services
- Consider using a development AI endpoint with lower costs for testing
