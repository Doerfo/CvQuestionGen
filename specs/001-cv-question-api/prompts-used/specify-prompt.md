/speckit.specify

Build a REST API for HR professionals to generate AI-powered interview questions based on candidate CVs and job descriptions.

**Core Features:**

1. **CV Management** - POST/GET endpoints to store and retrieve unformatted CV text. AI should extract and structure: personal info, skills with proficiency scoring (e.g., "3 years Java" = intermediate, "touched TypeScript" = beginner), work experience, and education.

2. **Job Description Management** - POST/GET endpoints for unformatted job descriptions. AI extracts required skills, experience levels, and key competencies to assess.

3. **Question Generation** - GET endpoint that generates topic-grouped question blocks (e.g., "Frontend - Angular", "Backend - TypeScript") at appropriate difficulty levels. Each question includes:
   - The question text
   - Natural language answer guidelines for non-technical recruiters
   - Key technical terms with plain English explanations
   - Suggested follow-up questions
   - should fail if no CV or job description is set

**Example Output:**
```
Topic: Frontend - Angular (Intermediate)
Q: "What are the building blocks of an Angular application?"
Expected: Candidate explains components, templates, modules, services
Key Terms: 
  - Components: TypeScript class + HTML template + CSS styling
  - Templates: HTML view with Angular directives
  - Services: Injectable business logic classes
```

**Requirements:**
- RESTful JSON API
- AI integration for text analysis and question generation
- Level-appropriate questions matching candidate skills to job requirements
- Recruiter-friendly explanations of technical concepts
