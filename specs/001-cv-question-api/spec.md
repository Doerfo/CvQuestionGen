# Feature Specification: CV-Powered Interview Question Generator API

**Feature Branch**: `001-cv-question-api`  
**Created**: 11 December 2025  
**Status**: Draft  
**Input**: User description: "Build a REST API for HR professionals to generate AI-powered interview questions based on candidate CVs and job descriptions"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Upload and Process CV (Priority: P1)

As an HR professional, I want to upload a candidate's CV text so that the system can extract and structure the candidate's information for question generation.

**Why this priority**: This is the foundational data input - without structured CV data, no interview questions can be generated. CVs are the primary source for understanding candidate capabilities.

**Independent Test**: Can be fully tested by uploading CV text via API and verifying the structured response contains extracted skills, experience, and education. Delivers immediate value by organizing unformatted CV data.

**Acceptance Scenarios**:

1. **Given** a user has unformatted CV text, **When** they submit it to the CV endpoint, **Then** the system confirms successful storage and returns a structured extraction containing skills with proficiency levels, work experience, and education.

2. **Given** a CV has been previously uploaded, **When** the user requests the current CV, **Then** the system returns both the original text and the AI-extracted structured data.

3. **Given** a CV text contains skill mentions like "3 years Java" or "touched TypeScript", **When** processed, **Then** skills are categorized with proficiency levels (e.g., "3 years Java" = Intermediate, "touched TypeScript" = Beginner).

4. **Given** a CV text is empty or contains no parseable content, **When** submitted, **Then** the system returns a clear error message indicating the CV could not be processed.

5. **Given** a CV is already stored, **When** a new CV is uploaded, **Then** the new CV overwrites the previous one and the system confirms successful storage.

---

### User Story 2 - Upload and Process Job Description (Priority: P1)

As an HR professional, I want to upload a job description so that the system can extract required skills and competencies to match against candidates.

**Why this priority**: Job descriptions define what skills and experience levels are required. Together with CVs, this forms the complete input needed for meaningful question generation.

**Independent Test**: Can be fully tested by uploading job description text and verifying the structured response contains required skills, experience levels, and key competencies.

**Acceptance Scenarios**:

1. **Given** a user has unformatted job description text, **When** they submit it to the job description endpoint, **Then** the system confirms successful storage and returns a structured extraction containing required skills, experience levels, and key competencies to assess.

2. **Given** a job description has been previously uploaded, **When** the user requests the current job description, **Then** the system returns both the original text and the AI-extracted structured data.

3. **Given** a job description text is empty or contains no parseable content, **When** submitted, **Then** the system returns a clear error message indicating the job description could not be processed.

4. **Given** a job description is already stored, **When** a new job description is uploaded, **Then** the new job description overwrites the previous one and the system confirms successful storage.

---

### User Story 3 - Generate Interview Questions (Priority: P1)

As an HR professional, I want to generate interview questions based on a candidate's CV matched against a job description, so that I can conduct relevant and appropriately-leveled technical interviews.

**Why this priority**: This is the core value proposition - generating tailored interview questions that match candidate skills to job requirements at appropriate difficulty levels.

**Independent Test**: Can be fully tested by specifying a CV and job description identifier, then verifying the response contains topic-grouped question blocks with all required elements (questions, answer guidelines, key terms, follow-ups).

**Acceptance Scenarios**:

1. **Given** a CV and job description are both stored, **When** the user requests question generation, **Then** the system returns topic-grouped question blocks organized by skill area (e.g., "Frontend - Angular", "Backend - TypeScript").

2. **Given** generated questions, **When** reviewed, **Then** each question includes: the question text, natural language answer guidelines for non-technical recruiters, key technical terms with plain English explanations, and suggested follow-up questions.

3. **Given** a candidate has "3 years Angular experience" and the job requires Angular, **When** questions are generated, **Then** Angular questions are at intermediate difficulty level matching the candidate's proficiency.

4. **Given** a candidate has skills marked as "beginner", **When** questions are generated, **Then** questions for those skills are at foundational/beginner difficulty level.

5. **Given** no CV is stored, **When** question generation is requested, **Then** the system returns an error indicating a CV must be uploaded first.

6. **Given** no job description is stored, **When** question generation is requested, **Then** the system returns an error indicating a job description must be uploaded first.

---

### User Story 4 - Retrieve Stored CV (Priority: P2)

As an HR professional, I want to retrieve the currently stored CV so that I can review what data is being used for question generation.

**Why this priority**: Supports workflow transparency by allowing users to verify stored data, but not essential for the core question generation flow.

**Independent Test**: Can be tested by retrieving the current CV and verifying the complete data is returned.

**Acceptance Scenarios**:

1. **Given** a CV was previously uploaded, **When** the user requests the current CV, **Then** the system returns the original text and all extracted structured data.

2. **Given** no CV has been uploaded, **When** requested, **Then** the system returns a clear "not found" error.

---

### User Story 5 - Retrieve Stored Job Description (Priority: P2)

As an HR professional, I want to retrieve the currently stored job description so that I can review what requirements are being used for question generation.

**Why this priority**: Supports workflow transparency by allowing users to verify stored requirements, but not essential for core flow.

**Independent Test**: Can be tested by retrieving the current job description and verifying complete data is returned.

**Acceptance Scenarios**:

1. **Given** a job description was previously uploaded, **When** the user requests the current job description, **Then** the system returns the original text and all extracted structured data.

2. **Given** no job description has been uploaded, **When** requested, **Then** the system returns a clear "not found" error.

---

### Edge Cases

- What happens when CV text contains no recognizable skills? → System returns structured data with empty skills array and includes a warning.
- What happens when job description has no technical requirements? → System generates behavioral/soft-skill questions based on available competencies.
- How does the system handle CVs in non-English languages? → System processes English CVs; non-English content may result in degraded extraction quality.
- What happens when candidate skills have no overlap with job requirements? → System generates foundational questions for required skills and notes the skill gaps.
- How does the system handle very long CV or job description text? → System processes up to a reasonable limit (e.g., 50,000 characters) and returns an error for oversized content.
- What happens when the AI service is unavailable? → System returns HTTP 503 with a clear error message and suggested retry timing; no degraded/cached results are served.
- What happens when the application restarts? → All stored CVs and job descriptions are lost; users must re-upload to generate new questions.

## Requirements *(mandatory)*

### Functional Requirements

**CV Management**
- **FR-001**: System MUST accept unformatted CV text via POST request and confirm successful storage.
- **FR-002**: System MUST use AI to extract and structure from CV text: personal information, skills with proficiency scoring, work experience, and education.
- **FR-003**: System MUST infer skill proficiency levels from contextual clues (e.g., "3 years Java" = Intermediate, "touched TypeScript" = Beginner, "expert in Python" = Advanced).
- **FR-004**: System MUST allow retrieval of the currently stored CV via GET request.
- **FR-005**: System MUST return both original text and structured extraction when retrieving the CV.
- **FR-019**: System MUST discard personal contact details (email, phone, address) after AI extraction; only professional data (skills, experience, education, job titles) is persisted.
- **FR-021**: System MUST store exactly one CV and one job description in-memory; uploading new data overwrites the previous entry.

**Job Description Management**
- **FR-006**: System MUST accept unformatted job description text via POST request and confirm successful storage.
- **FR-007**: System MUST use AI to extract from job descriptions: required skills, expected experience levels, and key competencies to assess.
- **FR-008**: System MUST allow retrieval of the currently stored job description via GET request.
- **FR-009**: System MUST return both original text and structured extraction when retrieving the job description.

**Question Generation**
- **FR-010**: System MUST generate interview questions when both a CV and job description are currently stored in memory.
- **FR-011**: System MUST fail with a clear error if no CV is currently stored when question generation is requested.
- **FR-012**: System MUST fail with a clear error if no job description is currently stored when question generation is requested.
- **FR-013**: System MUST group generated questions by topic area (e.g., "Frontend - Angular", "Backend - TypeScript").
- **FR-014**: System MUST assign appropriate difficulty levels to questions based on candidate's demonstrated proficiency in each skill area.
- **FR-015**: Each generated question MUST include: the question text, natural language answer guidelines suitable for non-technical recruiters, key technical terms with plain English explanations, and suggested follow-up questions.
- **FR-022**: System MUST generate exactly 5 questions per skill topic area.

**API Design**
- **FR-016**: System MUST expose a RESTful JSON API.
- **FR-017**: System MUST return appropriate HTTP status codes (200 for success, 400 for bad requests, 404 for not found, 500 for server errors, 503 for AI service unavailability).
- **FR-018**: System MUST return meaningful error messages in JSON format.
- **FR-020**: When the AI service is unavailable or errors, system MUST return HTTP 503 with a clear error message and suggested retry timing (e.g., "Retry-After" header).

### Key Entities

- **CV**: Represents a candidate's curriculum vitae. Contains original unformatted text and AI-extracted structured data including skills (with proficiency levels), work experience, and education. Personal contact details (email, phone, address) are discarded after extraction. Only one CV is stored in memory at a time; uploading a new CV overwrites the previous one.

- **Job Description**: Represents a job posting's requirements. Contains original unformatted text and AI-extracted structured data including required skills, experience levels, and key competencies. Only one job description is stored in memory at a time; uploading a new job description overwrites the previous one.

- **Skill**: Represents a technical or professional capability. Has a name, category (e.g., "Frontend", "Backend", "Soft Skills"), and proficiency level (Beginner, Intermediate, Advanced, Expert).

- **Question Block**: Represents a group of related interview questions. Contains a topic label (e.g., "Frontend - Angular"), difficulty level, and a collection of questions.

- **Interview Question**: Represents a single question to ask a candidate. Contains question text, answer guidelines (in recruiter-friendly language), key technical terms with explanations, and suggested follow-up questions.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: HR professionals can upload a CV and receive structured skill extraction within 30 seconds.
- **SC-002**: HR professionals can upload a job description and receive structured requirements extraction within 30 seconds.
- **SC-003**: HR professionals can generate a complete set of interview questions within 60 seconds.
- **SC-004**: 90% of generated questions are rated as "relevant to the role" by HR professionals in user testing.
- **SC-005**: 85% of non-technical recruiters can understand the answer guidelines without additional explanation.
- **SC-006**: Skill proficiency levels are correctly inferred (matching human assessment) for 80% of skills with contextual clues.
- **SC-007**: Generated questions appropriately match difficulty to candidate proficiency level in 85% of cases.
- **SC-008**: System successfully processes CVs and job descriptions up to 50,000 characters in length.

## Assumptions

- **A-001**: CVs and job descriptions are provided in English text format.
- **A-002**: The AI service for text analysis and question generation is available and accessible.
- **A-003**: Proficiency levels follow a four-tier model: Beginner, Intermediate, Advanced, Expert.
- **A-004**: Data is stored in-memory only and persists until application restart; no persistent database required for this initial version.
- **A-005**: Authentication and authorization are out of scope for this feature and will be handled separately.
- **A-006**: Rate limiting and abuse prevention are out of scope for this initial version.

## Clarifications

### Session 2025-12-11

- Q: How should the system handle CV data containing PII (names, addresses, phone numbers, emails)? → A: Extract and store professional data only (skills, experience, education); discard contact details after extraction.
- Q: What should happen when the AI service is unavailable or returns an error? → A: Return a clear error message indicating AI service unavailability with suggested retry timing.
- Q: How long should stored CVs and job descriptions be retained in the system? → A: In-memory storage only, data persists until application restart.
- Q: How many interview questions should be generated per skill area or topic? → A: 5
- Q: What should the system do when a CV is uploaded but no job description exists yet (or vice versa)? → A: Allow independent uploads; store only one CV and one job description in memory (uploading new data overwrites previous).
