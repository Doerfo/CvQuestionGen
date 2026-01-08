# Quickstart: CV Question Generation API

**Feature**: 001-cv-question-api  
**Date**: 2025-12-11

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for Aspire Dashboard)
- Azure OpenAI resource with deployed model (e.g., `gpt-4o-mini`)

## Configuration

### 1. Set up Azure OpenAI Connection

Add your Azure OpenAI connection string to user secrets:

```bash
cd CvQuestionGenerator/CvQuestionGenerator.AppHost
dotnet user-secrets set "ConnectionStrings:open-ai" "Endpoint=https://YOUR-RESOURCE.openai.azure.com/;Key=YOUR-API-KEY"
```

Or configure via environment variables in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "open-ai": "Endpoint=https://YOUR-RESOURCE.openai.azure.com/;Key=YOUR-API-KEY"
  }
}
```

### 2. Configure Deployment Name

In `appsettings.json` of the API project:

```json
{
  "OpenAI": {
    "DeploymentName": "gpt-4o-mini"
  }
}
```

## Running the Application

### Using .NET Aspire

```bash
cd CvQuestionGenerator/CvQuestionGenerator.AppHost
dotnet run
```

This will:
1. Start the Aspire Dashboard at `https://localhost:17200`
2. Start the API at `https://localhost:5001` (or configured port)
3. Open Swagger UI at `/swagger`

### Direct API Run (Development)

```bash
cd CvQuestionGenerator/CvQuestionGenerator.API
dotnet run
```

## API Usage

### 1. Upload a CV

```bash
curl -X POST http://localhost:5000/api/cvs \
  -H "Content-Type: application/json" \
  -d '{
    "text": "John Doe\nSenior Software Engineer\n\nSkills: 5 years Java, 3 years Python, touched TypeScript\n\nExperience:\n- Senior Developer at TechCorp (2020-Present)\n  Built microservices using Spring Boot\n- Developer at StartupXYZ (2018-2020)\n  Full-stack development with React and Node.js\n\nEducation:\n- BSc Computer Science, State University, 2018"
  }'
```

**Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "message": "CV successfully processed",
  "extraction": {
    "personalInfo": {
      "name": "John Doe",
      "title": "Senior Software Engineer"
    },
    "skills": [
      { "name": "Java", "category": "Backend", "proficiency": "Advanced", "yearsOfExperience": 5 },
      { "name": "Python", "category": "Backend", "proficiency": "Intermediate", "yearsOfExperience": 3 },
      { "name": "TypeScript", "category": "Frontend", "proficiency": "Beginner" }
    ],
    "experience": [...],
    "education": [...]
  }
}
```

### 2. Upload a Job Description

```bash
curl -X POST http://localhost:5000/api/jobs \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Senior Backend Developer\n\nRequirements:\n- 5+ years Java experience\n- Strong understanding of Spring Boot\n- Experience with microservices architecture\n- Familiarity with Python is a plus"
  }'
```

### 3. Generate Interview Questions

```bash
curl http://localhost:5000/api/questions
```

**Response (200 OK):**
```json
{
  "cvId": "550e8400-e29b-41d4-a716-446655440000",
  "jobId": "660e8400-e29b-41d4-a716-446655440001",
  "questionBlocks": [
    {
      "topicLabel": "Backend - Java",
      "difficulty": "Standard",
      "questions": [
        {
          "text": "Can you describe your experience building microservices with Spring Boot?",
          "answerGuidelines": "Look for specific examples of services built, understanding of service boundaries, and awareness of challenges like service discovery and inter-service communication.",
          "keyTerms": [
            { "term": "Microservices", "explanation": "Small, independent services that work together" },
            { "term": "Spring Boot", "explanation": "A Java framework for building web applications" }
          ],
          "followUpQuestions": [
            "How did you handle communication between services?",
            "What challenges did you face with deployment?"
          ]
        }
        // ... 4 more questions
      ]
    }
  ],
  "generatedAt": "2025-12-11T10:30:00Z"
}
```

### 4. Retrieve Stored CV

```bash
curl http://localhost:5000/api/cvs
```

### 5. Retrieve Stored Job Description

```bash
curl http://localhost:5000/api/jobs
```

## Error Handling

### CV Not Found (404)
```json
{
  "type": "https://api.cvquestiongen.com/errors/cv-not-found",
  "title": "CV Not Found",
  "status": 404,
  "detail": "No CV has been uploaded yet."
}
```

### Validation Error (400)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "errors": {
    "text": ["The Text field is required."]
  }
}
```

### AI Service Unavailable (503)
```json
{
  "type": "https://api.cvquestiongen.com/errors/ai-unavailable",
  "title": "AI Service Unavailable",
  "status": 503,
  "detail": "The AI service is temporarily unavailable.",
  "retryAfter": 30
}
```

## Running Tests

```bash
cd CvQuestionGenerator/CvQuestionGenerator.API.Tests
dotnet test
```

## Project Structure

```
CvQuestionGenerator/
├── CvQuestionGenerator.API/          # Main API (Minimal APIs)
│   ├── Models/                       # Domain models & DTOs
│   ├── Services/                     # Business logic
│   ├── Endpoints/                    # API route definitions
│   ├── Storage/                      # In-memory data store
│   ├── AppConstants.cs               # AI prompts
│   └── Program.cs                    # Entry point
├── CvQuestionGenerator.AppHost/      # Aspire orchestration
├── CvQuestionGenerator.ServiceDefaults/  # Shared config
└── CvQuestionGenerator.API.Tests/    # Unit tests
```

## Development Notes

- **Data Persistence**: All data is stored in-memory and lost on application restart
- **Single Instance**: Only one CV and one Job Description can be stored at a time
- **AI Processing**: CV and Job extraction typically complete within 30 seconds
- **Question Generation**: Generates 5 questions per skill topic
