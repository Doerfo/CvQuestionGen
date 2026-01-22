# Change Proposal: Add CV Question API

**Change ID:** `add-cv-question-api`  
**Status:** Draft  
**Created:** 2026-01-22

## Why

HR professionals and recruiters need an automated way to generate relevant interview questions based on candidate qualifications and job requirements. Currently, this process is manual, time-consuming, and often fails to properly align candidate experience with job requirements. 

Without AI assistance, recruiters may:
- Struggle to assess technical skills they're not familiar with
- Generate questions that are too easy or too difficult for the candidate's level
- Miss important technical areas that should be evaluated
- Lack understanding of technical terms and concepts

This change enables recruiters to conduct more effective technical interviews by automatically generating context-appropriate questions with plain-language explanations, improving hiring quality and reducing time-to-hire.

## Overview

Build a REST API for HR professionals to generate AI-powered interview questions based on candidate CVs and job descriptions. The API will analyze unstructured text inputs, extract structured data, and generate context-appropriate interview questions with recruiter-friendly explanations.

## Motivation

HR professionals and recruiters need an automated way to:
- Assess candidate qualifications against job requirements
- Generate relevant, level-appropriate interview questions
- Understand technical concepts in plain language
- Conduct more effective technical interviews without deep domain expertise

Manual question generation is time-consuming and may not properly align candidate experience with job requirements.

## Proposed Solution

Implement three core capabilities using a singleton storage pattern:

1. **CV Management** - Store and retrieve candidate CVs with AI-powered extraction of:
   - Personal information
   - Skills with proficiency scoring (based on experience duration and context)
   - Work experience
   - Education
   - **Note:** Only one CV is stored at a time; uploading a new CV replaces the previous one

2. **Job Description Management** - Store and retrieve job descriptions with AI extraction of:
   - Required skills
   - Experience levels
   - Key competencies to assess
   - **Note:** Only one job description is stored at a time; uploading a new one replaces the previous one

3. **Question Generation** - Generate topic-grouped interview questions by:
   - Using the currently stored CV and job description (no ID parameters needed)
   - Validating that both CV and job description have been uploaded
   - Matching candidate skills to job requirements
   - Determining appropriate difficulty levels
   - Providing answer guidelines, key technical terms with explanations, and follow-up questions

## Scope

### In Scope
- RESTful JSON API endpoints
- AI integration for text analysis and question generation
- Data models for CVs, job descriptions, and generated questions
- Level-appropriate question generation matching candidate skills to job requirements
- Recruiter-friendly explanations of technical concepts
- Validation that CV and job description exist before question generation

### Out of Scope
- User authentication and authorization (future enhancement)
- CV/job description persistence in database (in-memory storage acceptable for MVP)
- CV file upload/parsing (accepts plain text only)
- Multi-language support
- Question customization or templates
- Candidate response evaluation

## Dependencies

- .NET 10 Web API framework
- .NET Aspire for service orchestration and dependency management
- Azure OpenAI Service with Aspire chat client integration
- API key authentication system
- Swagger/OpenAPI with security definitions

## Technical Requirements

**API Endpoints:**
- CV Management: `POST /api/cvs` (returns 204), `GET /api/cvs` (no ID parameter)
- Job Management: `POST /api/jobs` (returns 204), `GET /api/jobs` (no ID parameter)
- Questions: `GET /api/questions` (no parameters, uses current CV and job description)

**Singleton Pattern:**
- Only one CV and one job description stored at a time
- Uploading new data replaces previous data
- Question generation validates both are present before proceeding

**Security:**
- API key authentication for all endpoints (X-API-Key header)
- Swagger UI with API key input support

**AI Integration:**
- Aspire Azure OpenAI client: `builder.AddAzureOpenAIClient("open-ai").AddChatClient()`
- AI prompts stored in `AppConstants.cs`

**Aspire Configuration:**
- AppHost manages Azure OpenAI connection string
- API project references OpenAI connection via Aspire

**Testing:**
- Unit tests for all core functionalities
- Manual testing via Swagger UI
- No integration tests required for MVP

## Risks and Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| AI service costs | High usage could be expensive | Implement request throttling and caching strategies |
| AI response quality | Generated questions may be irrelevant | Include prompt engineering best practices and validation |
| AI service availability | API downtime if AI service fails | Provide clear error messages and retry logic |
| Data privacy | CV data contains personal information | Document data handling requirements; future auth required |

## Success Criteria

- [ ] API successfully stores and retrieves CV text
- [ ] API successfully stores and retrieves job description text
- [ ] API extracts structured data from unstructured CV text with AI
- [ ] API extracts structured data from unstructured job description text with AI
- [ ] API generates topic-grouped questions with appropriate difficulty levels
- [ ] Questions include answer guidelines, key terms with explanations, and follow-ups
- [ ] Question generation fails gracefully when CV or job description is missing
- [ ] API returns JSON responses with proper HTTP status codes
- [ ] All endpoints validated with integration tests

## Related Changes

None - this is the initial implementation.

## Affected Capabilities

This change adds three new capabilities:
- `cv-management`
- `job-description-management`
- `question-generation`
