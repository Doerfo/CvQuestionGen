# Capability: Question Generation

**Status:** New capability  
**Change:** `add-cv-question-api`

## Overview

Provides API endpoint for generating AI-powered interview questions by matching candidate CV skills to job description requirements, organized by topic with appropriate difficulty levels and recruiter-friendly explanations.

## ADDED Requirements

### Requirement: The system MUST generate interview questions

The system MUST generate topic-grouped interview questions based on the currently stored CV and job description, with appropriate difficulty levels matching candidate proficiency to job requirements.

#### Scenario: Generate questions when CV and job description exist

**Given** a CV has been stored containing skills like "Angular (Intermediate)" and "TypeScript (Beginner)"  
**And** a job description has been stored requiring "Angular (Intermediate)" and "TypeScript (Advanced)"  
**When** an HR user GETs `/api/questions`  
**Then** the system:
- Matches candidate skills to job requirements
- Generates questions grouped by topic (e.g., "Frontend - Angular", "Backend - TypeScript")
- Sets difficulty levels appropriate to candidate proficiency
- Returns HTTP 200 OK with the question set

#### Scenario: Generate questions without CV

**Given** no CV has been uploaded  
**When** an HR user GETs `/api/questions`  
**Then** the system returns HTTP 400 Bad Request with error "CV must be uploaded before generating questions"

#### Scenario: Generate questions without job description

**Given** a CV has been uploaded  
**And** no job description has been uploaded  
**When** an HR user GETs `/api/questions`  
**Then** the system returns HTTP 400 Bad Request with error "Job description must be uploaded before generating questions"

### Requirement: Questions MUST include structured content

Each generated question MUST include the question text, answer guidelines for recruiters, key technical terms with plain English explanations, and suggested follow-up questions.

#### Scenario: Question includes answer guidelines

**Given** a question is generated about Angular components  
**When** the response is returned  
**Then** the question includes natural language answer guidelines that describe what a good answer should cover (e.g., "Candidate should explain components, templates, modules, and services")

#### Scenario: Question includes key technical terms

**Given** a question is generated about Angular  
**When** the response is returned  
**Then** the question includes key technical terms with plain English explanations  
**Examples:**
- "Components: TypeScript class combined with HTML template and CSS styling"
- "Templates: HTML view with Angular-specific directives"
- "Services: Injectable business logic classes shared across components"

#### Scenario: Question includes follow-up questions

**Given** a question is generated  
**When** the response is returned  
**Then** the question includes 2-3 suggested follow-up questions to deepen the assessment

### Requirement: Questions MUST be grouped by topic

The system MUST organize generated questions into topic groups that represent specific technical areas, with each group having an associated difficulty level.

#### Scenario: Questions grouped by technology topic

**Given** a candidate has skills in both Angular and TypeScript  
**When** questions are generated  
**Then** questions are grouped by topics like:
- "Frontend - Angular"
- "Backend - TypeScript"
- "Backend - Node.js"

#### Scenario: Each topic has difficulty level

**Given** questions are generated for a topic  
**When** the response is returned  
**Then** each topic group includes a difficulty level (Beginner, Intermediate, Advanced, Expert) based on candidate proficiency vs job requirements

### Requirement: The system MUST determine difficulty level

The system MUST determine question difficulty by comparing candidate skill proficiency levels from the CV against required skill levels from the job description.

#### Scenario: Candidate skill matches job requirement

**Given** a candidate has "Angular (Intermediate)" in their CV  
**And** the job requires "Angular (Intermediate)"  
**When** questions are generated for Angular  
**Then** the topic difficulty level is set to "Intermediate"

#### Scenario: Candidate skill below job requirement

**Given** a candidate has "TypeScript (Beginner)" in their CV  
**And** the job requires "TypeScript (Advanced)"  
**When** questions are generated for TypeScript  
**Then** the topic difficulty level is set to "Intermediate" or "Advanced" to assess growth potential

#### Scenario: Candidate skill exceeds job requirement

**Given** a candidate has "React (Expert)" in their CV  
**And** the job requires "React (Intermediate)"  
**When** questions are generated for React  
**Then** the topic difficulty level is set to "Intermediate" or "Advanced" to verify claimed expertise

### Requirement: The system MUST use AI to generate contextual questions

The system MUST use AI to generate contextually relevant questions based on the specific skills, experience, and requirements extracted from the CV and job description.

#### Scenario: AI generates contextual questions

**Given** a CV mentions "3 years Angular experience building e-commerce applications"  
**And** a job description requires "Angular for customer-facing web applications"  
**When** questions are generated  
**Then** the AI generates questions relevant to both Angular and e-commerce/customer-facing contexts

### Requirement: The system MUST authenticate API requests

The system MUST require API key authentication for the question generation endpoint and support authentication testing in Swagger UI.

#### Scenario: Request without API key

**Given** an HR user makes a request without an API key  
**When** they GET `/api/questions`  
**Then** the system returns HTTP 401 Unauthorized

#### Scenario: Request with invalid API key

**Given** an HR user provides an invalid API key  
**When** they GET `/api/questions`  
**Then** the system returns HTTP 401 Unauthorized

#### Scenario: Request with valid API key

**Given** an HR user provides a valid API key  
**When** they GET `/api/questions` with valid cvId and jobDescriptionId  
**Then** the system processes the request and generates questions

#### Scenario: API key authentication in Swagger

**Given** Swagger UI is configured  
**When** an HR user accesses the Swagger documentation  
**Then** they can enter an API key and test the question generation endpoint

## Data Model

```
QuestionSet
├── GeneratedAt: DateTime
└── TopicGroups: List<TopicGroup>
    └── TopicGroup
        ├── Topic: string (e.g., "Frontend - Angular")
        ├── DifficultyLevel: enum (Beginner, Intermediate, Advanced, Expert)
        └── Questions: List<Question>
            └── Question
                ├── QuestionText: string (required)
                ├── AnswerGuidelines: string (required, natural language for recruiters)
                ├── KeyTerms: List<KeyTerm>
                │   ├── Term: string (required)
                │   └── Explanation: string (required, plain English)
                └── SuggestedFollowUps: List<string> (2-3 follow-up questions)
```

## API Contracts

### GET /api/questions

**No query parameters required.** Uses currently stored CV and job description.

**Success Response (200 OK):**
```json
{
  "generatedAt": "ISO 8601 datetime",
  "topicGroups": [
    {
      "topic": "Frontend - Angular",
      "difficultyLevel": "Intermediate",
      "questions": [
        {
          "questionText": "What are the building blocks of an Angular application?",
          "answerGuidelines": "Candidate should explain components, templates, modules, and services",
          "keyTerms": [
            {
              "term": "Components",
              "explanation": "TypeScript class combined with HTML template and CSS styling"
            },
            {
              "term": "Templates",
              "explanation": "HTML view with Angular-specific directives"
            },
            {
              "term": "Services",
              "explanation": "Injectable business logic classes shared across components"
            }
          ],
          "suggestedFollowUps": [
            "Can you explain component lifecycle hooks?",
            "How do you share data between components?",
            "What is dependency injection in Angular?"
          ]
        }
      ]
    }
  ]
}
```

**Error Response (400 Bad Request - Missing CV):**
```json
{
  "error": "CV must be uploaded before generating questions"
}
```

**Error Response (400 Bad Request - Missing Job Description):**
```json
{
  "error": "Job description must be uploaded before generating questions"
}
```

## Dependencies

- `cv-management` capability (must check if CV exists)
- `job-description-management` capability (must check if job description exists)
- AI service integration (Azure OpenAI or OpenAI) for question generation
- HTTP client for AI API calls
- JSON serialization/deserialization
