# Research: CV Question Generation API

**Feature**: 001-cv-question-api  
**Date**: 2025-12-11  
**Status**: Complete

## Research Tasks

### 1. .NET Aspire Azure OpenAI Integration

**Task**: Research best practices for integrating Azure OpenAI with .NET Aspire

**Decision**: Use `Aspire.Azure.AI.OpenAI` package with `AddAzureOpenAIClient` and `IChatClient` abstraction

**Rationale**:
- .NET Aspire 9.3+ provides first-class support for Azure OpenAI via `Aspire.Azure.AI.OpenAI` NuGet package
- The `AddAzureOpenAIClient` extension method registers `AzureOpenAIClient` with DI
- Chaining `.AddChatClient()` provides `IChatClient` abstraction from `Microsoft.Extensions.AI`
- This integrates seamlessly with Semantic Kernel and provides automatic DI resolution
- Connection strings follow Aspire patterns: `Endpoint=https://{account}.openai.azure.com/;Key={key}`

**Alternatives Considered**:
1. Direct `AzureOpenAIClient` SDK usage - Rejected: Doesn't follow Aspire patterns, no built-in resilience
2. `AddOpenAIClientFromConfiguration` - Considered: Auto-detects Azure vs OpenAI, good for flexibility
3. Raw `AddConnectionString` - Rejected: Less type-safe, requires manual client construction

**Implementation Pattern**:
```csharp
// AppHost.cs
var openAi = builder.AddConnectionString("open-ai");
var api = builder.AddProject<Projects.CvQuestionGenerator_API>("api")
    .WithReference(openAi);

// Program.cs (API)
builder.AddAzureOpenAIClient("open-ai")
    .AddChatClient("gpt-4o-mini"); // deployment name
```

---

### 2. AI Prompt Engineering for CV Extraction

**Task**: Research prompt patterns for extracting structured data from unformatted text

**Decision**: Use structured JSON output with system prompts defining schema and examples

**Rationale**:
- OpenAI models support JSON mode for structured outputs
- System prompts should define exact schema expected
- Few-shot examples improve extraction accuracy
- Separate prompts for CV vs Job Description extraction allows optimization

**Prompt Strategy**:
1. **CV Extraction Prompt**: Focus on skills with proficiency inference, experience, education
2. **Job Description Prompt**: Focus on required skills, experience levels, competencies
3. **Question Generation Prompt**: Match candidate proficiency to question difficulty

**Proficiency Inference Rules** (from spec):
- "X years [skill]" → Map to proficiency level (1-2y=Beginner, 3-5y=Intermediate, 5+=Advanced)
- "expert/senior" → Advanced
- "touched/familiar with" → Beginner
- "worked with/used" → Intermediate

---

### 3. Minimal API Best Practices

**Task**: Research Minimal API patterns for .NET 10

**Decision**: Use endpoint groups with `MapGroup` and extension methods for route definitions

**Rationale**:
- Constitution mandates Minimal APIs over Controllers
- .NET 10 Minimal APIs support `TypedResults` for OpenAPI integration
- Endpoint groups provide logical organization similar to Controllers
- Extension methods keep `Program.cs` clean

**Implementation Pattern**:
```csharp
// Program.cs
app.MapCvEndpoints();
app.MapJobEndpoints();
app.MapQuestionEndpoints();

// CvEndpoints.cs
public static class CvEndpoints
{
    public static void MapCvEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/cvs")
            .WithTags("CVs")
            .WithOpenApi();
            
        group.MapPost("/", UploadCv);
        group.MapGet("/", GetCv);
    }
}
```

---

### 4. RFC 7807 Problem Details Implementation

**Task**: Research Problem Details implementation for error handling

**Decision**: Use built-in `Results.Problem()` with custom extensions

**Rationale**:
- ASP.NET Core has built-in support for RFC 7807 via `ProblemDetails`
- `Results.Problem()` in Minimal APIs returns proper Problem Details JSON
- Custom extension methods can create domain-specific problem types
- Consistent error format improves API consumer experience

**Implementation Pattern**:
```csharp
// Return problem details for errors
return Results.Problem(
    title: "CV Not Found",
    detail: "No CV has been uploaded. Please upload a CV first.",
    statusCode: StatusCodes.Status404NotFound,
    type: "https://api.cvquestiongen.com/errors/cv-not-found"
);
```

---

### 5. In-Memory Storage Pattern

**Task**: Research thread-safe in-memory storage for single CV/Job

**Decision**: Use singleton service with `lock` for thread safety

**Rationale**:
- Spec requires single CV and single Job stored in memory
- Singleton lifetime ensures single instance across requests
- Simple `lock` sufficient for this scale (single-user focus)
- No need for `ConcurrentDictionary` as only one item per type

**Implementation Pattern**:
```csharp
public sealed class InMemoryDataStore : IDataStore
{
    private readonly object _lock = new();
    private Cv? _cv;
    private JobDescription? _job;
    
    public void StoreCv(Cv cv) { lock (_lock) { _cv = cv; } }
    public Cv? GetCv() { lock (_lock) { return _cv; } }
}
```

---

### 6. AI Content Validation for Interview Questions

**Task**: Research approaches for ensuring generated questions are unbiased and legal

**Decision**: Include explicit constraints in system prompt + post-generation validation

**Rationale**:
- Constitution requires AI outputs be validated for bias and discrimination
- System prompts should explicitly prohibit protected characteristic questions
- Post-generation check can flag questions mentioning age, religion, marital status, etc.
- Azure AI Content Safety integration deferred to future iteration

**Prompt Constraints**:
```text
IMPORTANT CONSTRAINTS:
- DO NOT generate questions about protected characteristics (age, religion, 
  marital status, health, pregnancy, national origin, etc.)
- Focus ONLY on professional skills, experience, and competencies
- Questions must be relevant to the job requirements
- Avoid any questions that could be considered discriminatory
```

---

### 7. Skill Proficiency Scoring Algorithm

**Task**: Research algorithm for mapping skill mentions to proficiency levels

**Decision**: Pattern matching with contextual analysis

**Rationale**:
- Spec defines four tiers: Beginner, Intermediate, Advanced, Expert
- Years of experience is primary indicator
- Qualifying words ("expert", "touched", "familiar") provide additional signal
- AI extraction should output structured proficiency data

**Proficiency Mapping**:
| Signal | Proficiency Level |
|--------|-------------------|
| "touched", "familiar with", "exposure to", "basic" | Beginner |
| "worked with", "used", "experience with", 1-3 years | Intermediate |
| "strong", "proficient", "skilled", 3-5 years | Advanced |
| "expert", "senior", "lead", "architect", 5+ years | Expert |

---

### 8. Question Difficulty Calibration

**Task**: Research matching question difficulty to candidate proficiency

**Decision**: Generate difficulty-appropriate questions based on skill gap analysis

**Rationale**:
- If candidate proficiency matches job requirement → mid-level questions
- If candidate proficiency exceeds job requirement → probe depth of knowledge
- If candidate proficiency below job requirement → foundational questions
- 5 questions per topic area (per FR-022)

**Difficulty Mapping**:
| Candidate vs Job Requirement | Question Difficulty |
|------------------------------|---------------------|
| Candidate < Required | Foundational (test basic understanding) |
| Candidate = Required | Standard (test practical application) |
| Candidate > Required | Advanced (test depth and edge cases) |

---

## Resolved Clarifications

All NEEDS CLARIFICATION items from Technical Context have been resolved:

| Item | Resolution |
|------|------------|
| Minimal API migration | Will remove Controllers, implement Minimal API endpoints |
| AI Content validation | System prompt constraints + future Content Safety integration |
| Cost management caching | Deferred per A-006 (out of scope for initial version) |

## Dependencies Identified

| Package | Purpose | Version |
|---------|---------|---------|
| Aspire.Azure.AI.OpenAI | Azure OpenAI Aspire integration | 9.x+ |
| Microsoft.Extensions.AI | IChatClient abstraction | Latest |
| Microsoft.AspNetCore.OpenApi | OpenAPI documentation | 10.0.0 |
| System.Text.Json | JSON serialization | Built-in |

## Next Steps

1. Proceed to Phase 1: Data Model Design
2. Generate API contracts (OpenAPI spec)
3. Create quickstart guide
