# Capability: CV Management

**Status:** Active

## Purpose

Provides API endpoints for storing and retrieving candidate CV data with AI-powered extraction of structured information including personal details, skills with proficiency scoring, work experience, and education.

## Requirements

### Requirement: The system MUST store CV text

The system MUST allow HR users to submit unformatted CV text. Only one CV is stored at a time; submitting a new CV replaces the previous one.

#### Scenario: Submit new CV

**Given** an HR user has a candidate's CV as plain text  
**When** they POST the CV text to `/api/cvs`  
**Then** the system stores the CV and returns HTTP 204 No Content

#### Scenario: Submit CV replaces previous CV

**Given** a CV has already been stored  
**When** an HR user POSTs a new CV to `/api/cvs`  
**Then** the system replaces the previous CV with the new one and returns HTTP 204 No Content

#### Scenario: Submit invalid CV

**Given** an HR user attempts to submit an empty or null CV  
**When** they POST to `/api/cvs`  
**Then** the system returns HTTP 400 Bad Request with validation error message

### Requirement: Extract Structured Data from CV

The system MUST use AI to extract and structure personal information, skills, work experience, and education from unformatted CV text.

#### Scenario: Extract personal information

**Given** a CV text contains name, email, and phone number  
**When** the AI processes the CV  
**Then** personal information is extracted and included in the response

#### Scenario: Extract skills with proficiency

**Given** a CV text mentions skills with varying experience levels (e.g., "3 years Java", "touched TypeScript")  
**When** the AI processes the CV  
**Then** each skill is extracted with an appropriate proficiency level:
- "Beginner" for mentioned/touched/familiar skills
- "Intermediate" for 1-3 years or project experience
- "Advanced" for 3-5 years or extensive experience  
- "Expert" for 5+ years or demonstrated mastery

#### Scenario: Extract work experience

**Given** a CV text contains employment history  
**When** the AI processes the CV  
**Then** work experience is extracted with company, position, duration, and description

#### Scenario: Extract education

**Given** a CV text contains educational background  
**When** the AI processes the CV  
**Then** education is extracted with institution, degree, and year

### Requirement: The system MUST retrieve CV with extracted data

The system MUST allow HR users to retrieve the currently stored CV along with its AI-extracted structured data.

#### Scenario: Retrieve existing CV

**Given** a CV has been stored  
**When** an HR user GETs `/api/cvs`  
**Then** the system returns:
- HTTP 200 OK
- JSON body containing both raw CV text and extracted structured data

#### Scenario: Retrieve when no CV exists

**Given** no CV has been stored  
**When** an HR user GETs `/api/cvs`  
**Then** the system returns HTTP 404 Not Found

### Requirement: The system MUST persist CV data

The system MUST keep the submitted CV and its extracted data available for the duration of the application session. Only one CV is stored at a time.

#### Scenario: CV persists in memory

**Given** a CV has been submitted  
**When** the application is running  
**Then** the CV and its extracted data remain retrievable until application restart

**Note:** Database persistence is out of scope for MVP; in-memory storage is acceptable.

### Requirement: The system MUST authenticate API requests

The system MUST require API key authentication for all CV management endpoints and support authentication testing in Swagger UI.

#### Scenario: Request without API key

**Given** an HR user makes a request without an API key  
**When** they POST to `/api/cvs` or GET `/api/cvs`  
**Then** the system returns HTTP 401 Unauthorized

#### Scenario: Request with invalid API key

**Given** an HR user provides an invalid API key  
**When** they POST to `/api/cvs` or GET `/api/cvs`  
**Then** the system returns HTTP 401 Unauthorized

#### Scenario: Request with valid API key

**Given** an HR user provides a valid API key  
**When** they POST to `/api/cvs` or GET `/api/cvs`  
**Then** the system processes the request normally

#### Scenario: API key authentication in Swagger

**Given** Swagger UI is configured  
**When** an HR user accesses the Swagger documentation  
**Then** they can enter an API key and test authenticated endpoints

## Data Model

```
CVData
├── RawText: string (original unformatted CV)
├── ExtractedData: CVExtractedData
│   ├── PersonalInfo: PersonalInfo
│   │   ├── Name: string (nullable)
│   │   ├── Email: string (nullable)
│   │   └── Phone: string (nullable)
│   ├── Skills: List<Skill>
│   │   ├── Name: string (required)
│   │   ├── ProficiencyLevel: enum (Beginner, Intermediate, Advanced, Expert)
│   │   └── ExperienceContext: string (nullable, e.g., "3 years")
│   ├── WorkExperience: List<WorkExperience>
│   │   ├── Company: string (required)
│   │   ├── Position: string (required)
│   │   ├── Duration: string (nullable)
│   │   └── Description: string (nullable)
│   └── Education: List<Education>
│       ├── Institution: string (required)
│       ├── Degree: string (nullable)
│       └── Year: string (nullable)
└── CreatedAt: DateTime
```

## API Contracts

### POST /api/cvs

**Request:**
```json
{
  "cvText": "string (required, non-empty)"
}
```

**Success Response (204 No Content):**
No response body. CV has been stored and processed.

**Error Response (400 Bad Request):**
```json
{
  "error": "CV text is required and cannot be empty"
}
```

### GET /api/cvs

**Success Response (200 OK):**
```json
{
  "rawText": "string",
  "extractedData": {
    "personalInfo": {
      "name": "string or null",
      "email": "string or null",
      "phone": "string or null"
    },
    "skills": [
      {
        "name": "string",
        "proficiencyLevel": "Beginner|Intermediate|Advanced|Expert",
        "experienceContext": "string or null"
      }
    ],
    "workExperience": [
      {
        "company": "string",
        "position": "string",
        "duration": "string or null",
        "description": "string or null"
      }
    ],
    "education": [
      {
        "institution": "string",
        "degree": "string or null",
        "year": "string or null"
      }
    ]
  },
  "createdAt": "ISO 8601 datetime"
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "No CV has been uploaded"
}
```

## Dependencies

- AI service integration (Azure OpenAI or OpenAI) for text extraction
- HTTP client for AI API calls
- JSON serialization/deserialization
