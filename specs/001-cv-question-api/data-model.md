# Data Model: CV Question Generation API

**Feature**: 001-cv-question-api  
**Date**: 2025-12-11  
**Status**: Complete

## Entity Overview

```
┌─────────────────┐     ┌─────────────────────┐
│       CV        │     │   JobDescription    │
├─────────────────┤     ├─────────────────────┤
│ Id              │     │ Id                  │
│ OriginalText    │     │ OriginalText        │
│ PersonalInfo    │     │ RequiredSkills[]    │
│ Skills[]        │     │ Competencies[]      │
│ Experience[]    │     │ ExperienceLevels    │
│ Education[]     │     │ ExtractedAt         │
│ ExtractedAt     │     └─────────────────────┘
└─────────────────┘              │
        │                        │
        │    ┌───────────────────┘
        │    │
        ▼    ▼
┌─────────────────────────────────────┐
│          QuestionBlock              │
├─────────────────────────────────────┤
│ TopicLabel (e.g., "Frontend - Angular")
│ DifficultyLevel                     │
│ Questions[]                         │
└─────────────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────┐
│        InterviewQuestion            │
├─────────────────────────────────────┤
│ Text                                │
│ AnswerGuidelines                    │
│ KeyTerms[] (with explanations)      │
│ FollowUpQuestions[]                 │
└─────────────────────────────────────┘
```

## Domain Entities

### CV

Represents a candidate's curriculum vitae with AI-extracted structured data.

```csharp
public sealed record Cv
{
    public required Guid Id { get; init; }
    public required string OriginalText { get; init; }
    public required PersonalInfo PersonalInfo { get; init; }
    public required IReadOnlyList<Skill> Skills { get; init; }
    public required IReadOnlyList<WorkExperience> Experience { get; init; }
    public required IReadOnlyList<Education> Education { get; init; }
    public required DateTimeOffset ExtractedAt { get; init; }
}

public sealed record PersonalInfo
{
    public string? Name { get; init; }
    public string? Title { get; init; }
    public string? Summary { get; init; }
    // Note: Email, phone, address are NOT stored per FR-019
}

public sealed record WorkExperience
{
    public required string JobTitle { get; init; }
    public required string Company { get; init; }
    public string? Duration { get; init; }
    public IReadOnlyList<string> Responsibilities { get; init; } = [];
}

public sealed record Education
{
    public required string Degree { get; init; }
    public required string Institution { get; init; }
    public string? Year { get; init; }
}
```

**Validation Rules**:
- `OriginalText`: Required, 1-50,000 characters
- `Skills`: May be empty array (with warning)
- `ExtractedAt`: Set automatically on AI extraction

**State Transitions**:
- Initial → Stored (on POST /api/cvs)
- Stored → Replaced (on subsequent POST /api/cvs)

---

### JobDescription

Represents a job posting with AI-extracted requirements.

```csharp
public sealed record JobDescription
{
    public required Guid Id { get; init; }
    public required string OriginalText { get; init; }
    public required IReadOnlyList<Skill> RequiredSkills { get; init; }
    public required IReadOnlyList<string> Competencies { get; init; }
    public required IReadOnlyList<ExperienceRequirement> ExperienceRequirements { get; init; }
    public required DateTimeOffset ExtractedAt { get; init; }
}

public sealed record ExperienceRequirement
{
    public required string Area { get; init; }
    public required ProficiencyLevel MinimumLevel { get; init; }
    public int? YearsRequired { get; init; }
}
```

**Validation Rules**:
- `OriginalText`: Required, 1-50,000 characters
- `RequiredSkills`: May be empty (generates behavioral questions only)
- `ExtractedAt`: Set automatically on AI extraction

---

### Skill

Represents a technical or professional capability with proficiency level.

```csharp
public sealed record Skill
{
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required ProficiencyLevel Proficiency { get; init; }
    public int? YearsOfExperience { get; init; }
    public string? Context { get; init; }  // Original text snippet for reference
}

public enum ProficiencyLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Expert = 4
}

public enum SkillCategory
{
    Frontend,
    Backend,
    Database,
    DevOps,
    Cloud,
    Mobile,
    MachineLearning,
    Testing,
    SoftSkills,
    Other
}
```

**Proficiency Mapping Rules**:
| Years/Signal | Level |
|--------------|-------|
| 0-2 years, "familiar", "touched" | Beginner |
| 2-4 years, "worked with", "used" | Intermediate |
| 4-6 years, "proficient", "strong" | Advanced |
| 6+ years, "expert", "lead" | Expert |

---

### QuestionBlock

Represents a group of related interview questions for a skill topic.

```csharp
public sealed record QuestionBlock
{
    public required string TopicLabel { get; init; }  // e.g., "Frontend - Angular"
    public required DifficultyLevel Difficulty { get; init; }
    public required IReadOnlyList<InterviewQuestion> Questions { get; init; }
}

public enum DifficultyLevel
{
    Foundational,  // For skill gaps
    Standard,      // Candidate meets requirements
    Advanced       // Candidate exceeds requirements
}
```

**Business Rules**:
- Exactly 5 questions per block (FR-022)
- TopicLabel format: "{Category} - {SkillName}"

---

### InterviewQuestion

Represents a single interview question with supporting information.

```csharp
public sealed record InterviewQuestion
{
    public required string Text { get; init; }
    public required string AnswerGuidelines { get; init; }  // Non-technical friendly
    public required IReadOnlyList<KeyTerm> KeyTerms { get; init; }
    public required IReadOnlyList<string> FollowUpQuestions { get; init; }
}

public sealed record KeyTerm
{
    public required string Term { get; init; }
    public required string Explanation { get; init; }  // Plain English
}
```

**Content Requirements** (per Constitution III):
- Questions must NOT ask about protected characteristics
- AnswerGuidelines in non-technical language for recruiters
- KeyTerms explain technical concepts in plain English

---

## Request/Response DTOs

### CV Endpoints

```csharp
// POST /api/cvs - Request
public sealed record UploadCvRequest
{
    [Required]
    [StringLength(50000, MinimumLength = 1)]
    public required string Text { get; init; }
}

// POST /api/cvs - Response (201 Created)
public sealed record CvUploadResponse
{
    public required Guid Id { get; init; }
    public required string Message { get; init; }
    public required CvExtractionResult Extraction { get; init; }
}

// GET /api/cvs - Response (200 OK)
public sealed record CvResponse
{
    public required Guid Id { get; init; }
    public required string OriginalText { get; init; }
    public required CvExtractionResult Extraction { get; init; }
    public required DateTimeOffset ExtractedAt { get; init; }
}

public sealed record CvExtractionResult
{
    public required PersonalInfo PersonalInfo { get; init; }
    public required IReadOnlyList<Skill> Skills { get; init; }
    public required IReadOnlyList<WorkExperience> Experience { get; init; }
    public required IReadOnlyList<Education> Education { get; init; }
    public IReadOnlyList<string>? Warnings { get; init; }
}
```

### Job Description Endpoints

```csharp
// POST /api/jobs - Request
public sealed record UploadJobRequest
{
    [Required]
    [StringLength(50000, MinimumLength = 1)]
    public required string Text { get; init; }
}

// POST /api/jobs - Response (201 Created)
public sealed record JobUploadResponse
{
    public required Guid Id { get; init; }
    public required string Message { get; init; }
    public required JobExtractionResult Extraction { get; init; }
}

// GET /api/jobs - Response (200 OK)
public sealed record JobResponse
{
    public required Guid Id { get; init; }
    public required string OriginalText { get; init; }
    public required JobExtractionResult Extraction { get; init; }
    public required DateTimeOffset ExtractedAt { get; init; }
}

public sealed record JobExtractionResult
{
    public required IReadOnlyList<Skill> RequiredSkills { get; init; }
    public required IReadOnlyList<string> Competencies { get; init; }
    public required IReadOnlyList<ExperienceRequirement> ExperienceRequirements { get; init; }
    public IReadOnlyList<string>? Warnings { get; init; }
}
```

### Question Generation Endpoints

```csharp
// GET /api/questions - Response (200 OK)
public sealed record QuestionsResponse
{
    public required Guid CvId { get; init; }
    public required Guid JobId { get; init; }
    public required IReadOnlyList<QuestionBlock> QuestionBlocks { get; init; }
    public required DateTimeOffset GeneratedAt { get; init; }
}
```

---

## Error Response DTOs

Following RFC 7807 Problem Details:

```csharp
// Standard Problem Details (built-in)
{
    "type": "https://api.cvquestiongen.com/errors/cv-not-found",
    "title": "CV Not Found",
    "status": 404,
    "detail": "No CV has been uploaded. Please upload a CV first.",
    "instance": "/api/questions"
}

// Validation Error (400)
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "Validation Error",
    "status": 400,
    "detail": "One or more validation errors occurred.",
    "errors": {
        "Text": ["The Text field is required."]
    }
}

// AI Service Unavailable (503)
{
    "type": "https://api.cvquestiongen.com/errors/ai-unavailable",
    "title": "AI Service Unavailable",
    "status": 503,
    "detail": "The AI service is temporarily unavailable. Please retry in 30 seconds.",
    "retryAfter": 30
}
```

---

## Storage Interface

```csharp
public interface IDataStore
{
    void StoreCv(Cv cv);
    Cv? GetCv();
    void StoreJob(JobDescription job);
    JobDescription? GetJob();
    void Clear();
}
```

**Implementation Notes**:
- Singleton lifetime for in-memory persistence
- Thread-safe with simple locking
- `Clear()` for testing support
