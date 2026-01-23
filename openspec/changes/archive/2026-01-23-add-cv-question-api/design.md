# Design Document: CV Question API

**Change ID:** `add-cv-question-api`  
**Created:** 2026-01-22

## Architecture Overview

The CV Question API follows a layered architecture within the existing ASP.NET Core Web API project:

```
┌─────────────────────────────────────────┐
│         API Controllers Layer           │
│  (CVController, JobDescController,      │
│   QuestionController)                   │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         Service Layer                   │
│  (CVService, JobDescService,            │
│   QuestionGenerationService)            │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         AI Integration Layer            │
│  (IAIService implementation)            │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         External AI Service             │
│  (Azure OpenAI / OpenAI)                │
└─────────────────────────────────────────┘
```

## Technology Stack

- **Framework:** .NET 10 Web API
- **.NET Aspire:** Service orchestration, configuration, and dependency management
- **AI Provider:** Azure OpenAI Service with Aspire chat client integration
- **Authentication:** API key authentication
- **Serialization:** System.Text.Json
- **Storage:** In-memory (Dictionary-based) for MVP - future database integration
- **HTTP Client:** Aspire-managed HTTP client for AI service calls

## Data Models

**Note:** The system stores only one CV and one job description at a time (singleton pattern). Uploading a new CV or job description replaces the previous one.

### CV Data Model
```
CVData
├── RawText: string (original unformatted CV)
├── ExtractedData: CVExtractedData
│   ├── PersonalInfo: PersonalInfo
│   │   ├── Name: string?
│   │   ├── Email: string?
│   │   └── Phone: string?
│   ├── Skills: List<Skill>
│   │   ├── Name: string
│   │   ├── ProficiencyLevel: enum (Beginner, Intermediate, Advanced, Expert)
│   │   └── ExperienceContext: string? (e.g., "3 years", "touched briefly")
│   ├── WorkExperience: List<WorkExperience>
│   │   ├── Company: string
│   │   ├── Position: string
│   │   ├── Duration: string?
│   │   └── Description: string?
│   └── Education: List<Education>
│       ├── Institution: string
│       ├── Degree: string?
│       └── Year: string?
└── CreatedAt: DateTime
```

### Job Description Data Model
```
JobDescriptionData
├── RawText: string (original unformatted job description)
├── ExtractedData: JobDescExtractedData
│   ├── RequiredSkills: List<RequiredSkill>
│   │   ├── Name: string
│   │   └── RequiredLevel: enum (Beginner, Intermediate, Advanced, Expert)
│   ├── ExperienceLevel: string? (e.g., "3-5 years")
│   └── KeyCompetencies: List<string>
└── CreatedAt: DateTime
```

### Question Generation Data Model
```
QuestionSet
├── GeneratedAt: DateTime

├── TopicGroups: List<TopicGroup>
    └── TopicGroup
        ├── Topic: string (e.g., "Frontend - Angular")
        ├── DifficultyLevel: enum (Beginner, Intermediate, Advanced, Expert)
        └── Questions: List<Question>
            └── Question
                ├── QuestionText: string
                ├── AnswerGuidelines: string (natural language for recruiters)
                ├── KeyTerms: List<KeyTerm>
                │   ├── Term: string
                │   └── Explanation: string (plain English)
                └── SuggestedFollowUps: List<string>
```

## AI Integration Strategy

### Aspire Chat Client Configuration

Use .NET Aspire's Azure OpenAI integration:

```csharp
// In AppHost (CvQuestionGenerator.AppHost/AppHost.cs)
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

```csharp
// In API Program.cs
builder.AddAzureOpenAIClient("open-ai").AddChatClient();
```

### Prompt Templates

All AI prompts stored in `AppConstants.cs` in the API project:

```csharp
public static class AppConstants
{
    public static class Prompts
    {
        public const string CvExtractionSystem = "You are an expert CV parser...";
        public const string CvExtractionUser = "Parse this CV and score skill proficiency...";
        public const string JobDescExtractionSystem = "You are an expert job requirement analyzer...";
        public const string JobDescExtractionUser = "Extract required skills with proficiency levels...";
        public const string QuestionGenerationSystem = "You are an expert technical interviewer...";
        public const string QuestionGenerationUser = "Generate interview questions based on...";
    }
}
```

### Approach
Use structured prompts with JSON schema responses to ensure consistent, parseable outputs from the AI service.

### CV Extraction Prompt Pattern
```
System: You are an expert CV parser. Extract structured data from the CV text.
Output must be valid JSON matching this schema: {schema}

User: Parse this CV and score skill proficiency based on:
- Beginner: mentioned/touched/familiar
- Intermediate: 1-3 years or project experience
- Advanced: 3-5 years or extensive experience
- Expert: 5+ years or demonstrated mastery

CV Text: {cvText}
```

### Job Description Extraction Prompt Pattern
```
System: You are an expert job requirement analyzer. Extract structured data.
Output must be valid JSON matching this schema: {schema}

User: Extract required skills with proficiency levels, experience requirements, 
and key competencies from this job description.

Job Description: {jobDescText}
```

### Question Generation Prompt Pattern
```
System: You are an expert technical interviewer. Generate questions that match 
candidate skills to job requirements.

User: Generate interview questions based on:
- Candidate Skills: {cvSkills}
- Job Requirements: {jobRequirements}

Group questions by topic (e.g., "Frontend - Angular", "Backend - TypeScript").
Set difficulty based on candidate proficiency vs job requirements.
Include:
1. Question text
2. Answer guidelines for non-technical recruiters
3. Key technical terms with plain English explanations
4. 2-3 follow-up questions

Output as JSON matching this schema: {schema}
```

### Error Handling
- Retry logic with exponential backoff for transient failures
- Validation of AI responses against expected JSON schema
- Fallback to simplified extraction if structured parsing fails
- Clear error messages for users

## API Design

### Security

All endpoints require API key authentication:

- API key passed via `X-API-Key` header
- Configured in appsettings.json (development) or Azure Key Vault (production)
- Swagger UI configured with API key input field
- Returns 401 Unauthorized for missing/invalid API keys

### RESTful Endpoints

**CV Management**
- `POST /api/cvs` - Submit CV text, replaces any existing CV, returns 204 No Content
- `GET /api/cvs` - Retrieve currently stored CV with extracted data
- Response includes both raw text and structured extracted data

**Job Description Management**
- `POST /api/jobs` - Submit job description text, replaces any existing job description, returns 204 No Content
- `GET /api/jobs` - Retrieve currently stored job description with extracted data
- Response includes both raw text and structured extracted data

**Question Generation**
- `GET /api/questions` - Generate interview questions (no parameters)
- Uses currently stored CV and job description
- Returns 400 Bad Request if CV or job description missing
- Returns 200 OK with question set organized by topics

### Response Format Example
```json
{
  "generatedAt": "2026-01-22T10:30:00Z",
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

## Configuration

Use .NET configuration system with Aspire integration:

```json
{
  "ConnectionStrings": {
    "open-ai": "Endpoint=https://...;Key=***"
  },
  "Authentication": {
    "ApiKey": "your-api-key-here"
  }
}
```

Store secrets in User Secrets (development) or Azure Key Vault (production).

AppHost configuration handles AI service connection and dependency injection.

## Service Registration

In `Program.cs`:
```csharp
// Add Aspire service defaults
builder.AddServiceDefaults();

// Add Aspire Azure OpenAI chat client
builder.AddAzureOpenAIClient("open-ai").AddChatClient();

// Register singleton repositories (one CV, one job description at a time)
builder.Services.AddSingleton<ICVRepository, SingletonCVRepository>();
builder.Services.AddSingleton<IJobDescRepository, SingletonJobDescRepository>();

// Register services
builder.Services.AddScoped<ICVService, CVService>();
builder.Services.AddScoped<IJobDescService, JobDescService>();
builder.Services.AddScoped<IQuestionGenerationService, QuestionGenerationService>();

// Add API key authentication
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", options => { });
builder.Services.AddAuthorization();

// Configure Swagger with API key support
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-API-Key",
        Description = "API Key Authentication"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

## Testing Strategy

1. **Unit Tests**
   - Service layer logic (mocked AI chat client)
   - Data model validation
   - Proficiency scoring logic
   - API key authentication handler
   - Skill matching and difficulty calibration algorithms

2. **Manual Testing**
   - Real AI service integration via Swagger UI
   - API key authentication flow
   - Sample CVs and job descriptions
   - Question quality evaluation

## Security Considerations

- **API Keys:** Never commit to source control; use User Secrets/Key Vault
- **Input Validation:** Sanitize CV and job description text to prevent prompt injection
- **Rate Limiting:** Implement to prevent abuse and control AI costs
- **Data Privacy:** Document that CV data contains PII; future auth required
- **HTTPS:** Enforce for all endpoints

## Future Enhancements

- Database persistence (Entity Framework Core)
- User authentication and multi-tenancy
- Question templates and customization
- CV file upload and parsing (PDF, DOCX)
- Question history and caching
- Candidate response evaluation
- Analytics and reporting
