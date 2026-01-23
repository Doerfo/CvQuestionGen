# CV Question Generator API

An AI-powered REST API for generating interview questions based on candidate CVs and job descriptions. Built with .NET 10, .NET Aspire, and Azure OpenAI.

## Features

- **CV Management**: Upload and store candidate CVs with AI-powered extraction of skills, experience, and education
- **Job Description Management**: Upload and store job descriptions with AI-powered extraction of requirements
- **Question Generation**: Generate contextual interview questions matching candidate skills to job requirements
- **Singleton Pattern**: Only one CV and one job description stored at a time (simplified workflow)
- **API Key Authentication**: Secure endpoints with X-API-Key header authentication
- **Swagger UI**: Interactive API documentation with authentication support

## Getting Started

### Prerequisites

- .NET 10 SDK
- Azure OpenAI Service or OpenAI API access
- Visual Studio 2024 or VS Code with C# Dev Kit

### Configuration

#### 1. Set up Azure OpenAI Connection String

Add the connection string to your Aspire AppHost secrets:

```bash
cd CvQuestionGenerator.AppHost
dotnet user-secrets set "ConnectionStrings:open-ai" "Endpoint=https://your-resource.openai.azure.com/;Key=your-api-key"
```

#### 2. Configure API Key for Authentication

Set the API key in user secrets (development):

```bash
cd CvQuestionGenerator.API
dotnet user-secrets set "Authentication:ApiKey" "your-secure-api-key"
```

Or set via environment variable (production):
```bash
export Authentication__ApiKey="your-secure-api-key"
```

### Running the Application

```bash
cd CvQuestionGenerator.AppHost
dotnet run
```

Access the Swagger UI at the URL shown in the Aspire dashboard (typically `http://localhost:5xxx/swagger`).

## API Endpoints

### Authentication

All endpoints require the `X-API-Key` header with a valid API key.

### CV Management

#### POST /api/cvs - Submit CV

Uploads and processes a CV. Replaces any previously stored CV.

**Request:**
```json
{
  "cvText": "John Doe\njohn@example.com\n5 years C# experience..."
}
```

**Response:** `204 No Content`

#### GET /api/cvs - Get Current CV

Retrieves the currently stored CV with extracted data.

**Response:** `200 OK`
```json
{
  "rawText": "John Doe\njohn@example.com...",
  "extractedData": {
    "personalInfo": {
      "name": "John Doe",
      "email": "john@example.com",
      "phone": null
    },
    "skills": [
      {
        "name": "C#",
        "proficiencyLevel": "Expert",
        "experienceContext": "5 years"
      }
    ],
    "workExperience": [...],
    "education": [...]
  },
  "createdAt": "2026-01-23T10:00:00Z"
}
```

**Error Response:** `404 Not Found` if no CV uploaded

### Job Description Management

#### POST /api/jobs - Submit Job Description

Uploads and processes a job description. Replaces any previously stored job description.

**Request:**
```json
{
  "jobDescriptionText": "Senior C# Developer\n5+ years experience required..."
}
```

**Response:** `204 No Content`

#### GET /api/jobs - Get Current Job Description

Retrieves the currently stored job description with extracted data.

**Response:** `200 OK`
```json
{
  "rawText": "Senior C# Developer...",
  "extractedData": {
    "requiredSkills": [
      {
        "name": "C#",
        "requiredLevel": "Expert"
      }
    ],
    "experienceLevel": "5+ years",
    "keyCompetencies": ["Problem solving", "Leadership"]
  },
  "createdAt": "2026-01-23T10:00:00Z"
}
```

**Error Response:** `404 Not Found` if no job description uploaded

### Question Generation

#### GET /api/questions - Generate Interview Questions

Generates interview questions based on the currently stored CV and job description.

**Prerequisites:** Both CV and job description must be uploaded first.

**Response:** `200 OK`
```json
{
  "generatedAt": "2026-01-23T10:30:00Z",
  "topicGroups": [
    {
      "topic": "Backend - C#",
      "difficultyLevel": "Intermediate",
      "questions": [
        {
          "questionText": "What are the building blocks of a C# application?",
          "answerGuidelines": "Candidate should explain classes, methods, namespaces, and assemblies",
          "keyTerms": [
            {
              "term": "Class",
              "explanation": "A blueprint for creating objects that groups related data and methods"
            }
          ],
          "suggestedFollowUps": [
            "Can you explain inheritance in C#?",
            "What is the difference between a class and a struct?"
          ]
        }
      ]
    }
  ]
}
```

**Error Responses:**
- `400 Bad Request`: CV or job description not uploaded
- `401 Unauthorized`: Invalid or missing API key

## Proficiency Levels

The system uses four proficiency levels for skills:

| Level | CV Indicators | Job Requirements |
|-------|--------------|------------------|
| Beginner | "mentioned", "touched", "familiar" | Entry-level, basic |
| Intermediate | 1-3 years, project experience | 1-3 years required |
| Advanced | 3-5 years, extensive experience | 3-5 years, senior-level |
| Expert | 5+ years, demonstrated mastery | 5+ years, principal/lead |

## Error Handling

All error responses follow this format:

```json
{
  "error": "Error message describing what went wrong"
}
```

Common HTTP status codes:
- `200`: Success
- `204`: Success (no content)
- `400`: Bad request (validation error)
- `401`: Unauthorized (missing/invalid API key)
- `404`: Not found (no CV or job description uploaded)
- `500`: Internal server error

## Sample Test Data

Sample CVs and job descriptions are provided in `CvQuestionGenerator.API/TestData/`:

- `sample-cv-junior.txt` - Entry-level developer
- `sample-cv-mid.txt` - Mid-level developer  
- `sample-cv-senior.txt` - Senior developer
- `sample-job-junior.txt` - Junior developer position
- `sample-job-mid.txt` - Mid-level position
- `sample-job-senior.txt` - Senior developer position

## Architecture

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
│  (AIService with ChatClient)            │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         Azure OpenAI Service            │
└─────────────────────────────────────────┘
```

## Troubleshooting

### "API key is missing" error
Ensure the `X-API-Key` header is included in your request. In Swagger UI, click the "Authorize" button and enter your API key.

### "CV must be uploaded before generating questions"
Upload a CV using POST /api/cvs before calling GET /api/questions.

### "Job description must be uploaded before generating questions"
Upload a job description using POST /api/jobs before calling GET /api/questions.

### AI service not responding
1. Verify the Azure OpenAI connection string is correct
2. Check the Aspire dashboard for connection status
3. Ensure your Azure OpenAI deployment is active

### Empty extracted data
The AI may have trouble parsing some CV formats. Try reformatting the CV text to be more structured.

## Development

### Running Tests

```bash
cd CvQuestionGenerator
dotnet test
```

### Building

```bash
cd CvQuestionGenerator
dotnet build
```

## Security Notes

- API keys should never be committed to source control
- Use User Secrets for development, Azure Key Vault for production
- CV data contains PII - future versions will add user authentication
- Consider rate limiting in production to control AI costs

## License

[Add your license here]
