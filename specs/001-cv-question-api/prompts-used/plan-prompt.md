# Implementation Plan: CV Question Generation API

## API Endpoints

### CV Management
- `POST /api/cvs` - Store unformatted CV text, return CV ID
- `GET /api/cvs` - Retrieve structured CV data
- AI extraction: personal info, skills with proficiency levels, work experience, education

### Job Description Management  
- `POST /api/jobs` - Store unformatted job description, return job ID
- `GET /api/jobs` - Retrieve structured job requirements
- AI extraction: required skills, experience levels, key competencies

### Question Generation
- `GET /api/questions` - Generate interview questions
- Returns topic-grouped question blocks with difficulty matching candidate-to-role fit
- Each question includes: text, answer guidelines, key terms with explanations, follow-ups
- Validation: fail if CV or job description missing

### Security
- API key authentication for all endpoints
- usable in swagger

## Technical Stack
- .NET 10 Web API (existing project structure)
- AI integration for text analysis and question generation
- JSON responses
- RESTful design
- Dependency injection for all services

## Dependencies
- all dependencies should be handled in AppHost like:
```csharp
// Chat client connection string
var openAi = builder.AddConnectionString("open-ai"); 
var api = builder.AddProject<Projects.CvQuestionGenerator_API>("api")
    .WithExternalHttpEndpoints()
    .WithReference(openAi)
    .WaitFor(openAi)
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Swagger UI";
        url.Url = "/swagger";
    });
```

## Ai client
- use Aspire options if possible e.g.:
```csharp
builder.AddAzureOpenAIClient("open-ai").AddChatClient();
```
- AI prompts should be located in AppConstants

## App constants
- Create `AppConstants.cs` in API project for prompt templates and other constants

## Testing
- Unit tests for all core functionalities
- No integration tests needed at this time

## Key Implementation Areas
1. AI service integration for CV/job parsing and question generation
2. Data models for CV, job description, and question structures
3. Controllers for each endpoint group
4. Validation logic for question generation prerequisites
5. Skill proficiency scoring algorithm
6. Question difficulty calibration based on skill gaps
