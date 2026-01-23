# Project Context

## Purpose
CV Question Generator - A REST API for HR professionals to generate AI-powered interview questions based on candidate CVs and job descriptions.

## Tech Stack
- .NET 10 (net10.0)
- ASP.NET Core Web API
- .NET Aspire for service orchestration
- Azure OpenAI (GPT-4o-mini) via Aspire.Azure.AI.OpenAI
- Swashbuckle.AspNetCore for OpenAPI/Swagger
- xUnit for unit testing
- NSubstitute for mocking

## Project Conventions

### Code Style
- Follow C# coding conventions
- Use async/await for I/O operations
- Use dependency injection for all services
- Interface-based design for testability

### Architecture Patterns
- Singleton repository pattern for in-memory storage
- Service layer for business logic
- Controller layer for HTTP handling
- Custom authentication handler for API key validation

### Testing Strategy
- Unit tests using xUnit and NSubstitute
- Test services in isolation with mocked dependencies
- Tests located in CvQuestionGenerator.Tests project

### Git Workflow
- Feature branches for development
- PR-based workflow

## Domain Context
HR professionals use this API to:
- Upload candidate CVs (one at a time)
- Upload job descriptions (one at a time)
- Generate interview questions based on skill overlap between CV and job requirements
- Questions include recruiter-friendly explanations of technical terms

## Important Constraints
- Only one CV and one job description stored at a time (singleton pattern)
- API key authentication required (X-API-Key header)
- AI-generated content requires Azure OpenAI connection

## External Dependencies
- Azure OpenAI Service for AI-powered extraction and question generation
