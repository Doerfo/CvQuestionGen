# Capability: Job Description Management

**Status:** Active

## Purpose

Provides API endpoints for storing and retrieving job description data with AI-powered extraction of required skills, experience levels, and key competencies to assess during interviews.

## Requirements

### Requirement: The system MUST store job description text

The system MUST allow HR users to submit unformatted job description text. Only one job description is stored at a time; submitting a new job description replaces the previous one.

#### Scenario: Submit new job description

**Given** an HR user has a job description as plain text  
**When** they POST the job description text to `/api/jobs`  
**Then** the system stores the job description and returns HTTP 204 No Content

#### Scenario: Submit job description replaces previous one

**Given** a job description has already been stored  
**When** an HR user POSTs a new job description to `/api/jobs`  
**Then** the system replaces the previous job description with the new one and returns HTTP 204 No Content

#### Scenario: Submit invalid job description

**Given** an HR user attempts to submit an empty or null job description  
**When** they POST to `/api/jobs`  
**Then** the system returns HTTP 400 Bad Request with validation error message

### Requirement: The system MUST extract structured data from job description

The system MUST use AI to extract and structure required skills with proficiency levels, experience requirements, and key competencies from unformatted job description text.

#### Scenario: Extract required skills with levels

**Given** a job description specifies skills like "Senior Angular developer" or "3+ years TypeScript experience"  
**When** the AI processes the job description  
**Then** each skill is extracted with an appropriate required proficiency level:
- "Beginner" for entry-level or basic skills
- "Intermediate" for 1-3 years experience requirements
- "Advanced" for 3-5 years or senior-level requirements
- "Expert" for 5+ years or principal/lead-level requirements

#### Scenario: Extract experience level

**Given** a job description mentions overall experience requirements (e.g., "3-5 years in software development")  
**When** the AI processes the job description  
**Then** the experience level is extracted as a string

#### Scenario: Extract key competencies

**Given** a job description lists competencies to assess (e.g., "problem-solving", "team collaboration", "code review")  
**When** the AI processes the job description  
**Then** key competencies are extracted as a list of strings

### Requirement: The system MUST retrieve job description with extracted data

The system MUST allow HR users to retrieve the currently stored job description along with its AI-extracted structured data.

#### Scenario: Retrieve existing job description

**Given** a job description has been stored  
**When** an HR user GETs `/api/jobs`  
**Then** the system returns:
- HTTP 200 OK
- JSON body containing both raw job description text and extracted structured data

#### Scenario: Retrieve when no job description exists

**Given** no job description has been stored  
**When** an HR user GETs `/api/jobs`  
**Then** the system returns HTTP 404 Not Found

### Requirement: The system MUST persist job description data

The system MUST keep the submitted job description and its extracted data available for the duration of the application session. Only one job description is stored at a time.

#### Scenario: Job description persists in memory

**Given** a job description has been submitted  
**When** the application is running  
**Then** the job description and its extracted data remain retrievable until application restart

**Note:** Database persistence is out of scope for MVP; in-memory storage is acceptable.

### Requirement: The system MUST authenticate API requests

The system MUST require API key authentication for all job description management endpoints and support authentication testing in Swagger UI.

#### Scenario: Request without API key

**Given** an HR user makes a request without an API key  
**When** they POST to `/api/jobs` or GET `/api/jobs`  
**Then** the system returns HTTP 401 Unauthorized

#### Scenario: Request with invalid API key

**Given** an HR user provides an invalid API key  
**When** they POST to `/api/jobs` or GET `/api/jobs`  
**Then** the system returns HTTP 401 Unauthorized

#### Scenario: Request with valid API key

**Given** an HR user provides a valid API key  
**When** they POST to `/api/jobs` or GET `/api/jobs`  
**Then** the system processes the request normally

#### Scenario: API key authentication in Swagger

**Given** Swagger UI is configured  
**When** an HR user accesses the Swagger documentation  
**Then** they can enter an API key and test authenticated endpoints

## Data Model

```
JobDescriptionData
├── RawText: string (original unformatted job description)
├── ExtractedData: JobDescExtractedData
│   ├── RequiredSkills: List<RequiredSkill>
│   │   ├── Name: string (required)
│   │   └── RequiredLevel: enum (Beginner, Intermediate, Advanced, Expert)
│   ├── ExperienceLevel: string (nullable, e.g., "3-5 years")
│   └── KeyCompetencies: List<string>
└── CreatedAt: DateTime
```

## API Contracts

### POST /api/jobs

**Request:**
```json
{
  "jobDescriptionText": "string (required, non-empty)"
}
```

**Success Response (204 No Content):**
No response body. Job description has been stored and processed.

**Error Response (400 Bad Request):**
```json
{
  "error": "Job description text is required and cannot be empty"
}
```

### GET /api/jobs

**Success Response (200 OK):**
```json
{
  "rawText": "string",
  "extractedData": {
    "requiredSkills": [
      {
        "name": "string",
        "requiredLevel": "Beginner|Intermediate|Advanced|Expert"
      }
    ],
    "experienceLevel": "string or null",
    "keyCompetencies": ["string"]
  },
  "createdAt": "ISO 8601 datetime"
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "No job description has been uploaded"
}
```

## Dependencies

- AI service integration (Azure OpenAI or OpenAI) for text extraction
- HTTP client for AI API calls
- JSON serialization/deserialization
