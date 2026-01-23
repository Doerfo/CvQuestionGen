namespace CvQuestionGenerator.API;

/// <summary>
/// Application-wide constants including AI prompt templates.
/// </summary>
public static class AppConstants
{
    /// <summary>
    /// AI prompt templates for data extraction and question generation.
    /// </summary>
    public static class Prompts
    {
        /// <summary>
        /// System prompt for CV extraction.
        /// </summary>
        public const string CvExtractionSystem = """
            You are an expert CV parser. Your task is to extract structured data from CV text.
            You must output valid JSON matching the specified schema exactly.
            Do not include any text outside the JSON object.
            If information is not available in the CV, use null for optional fields or empty arrays for lists.
            """;

        /// <summary>
        /// User prompt template for CV extraction. Use {cvText} placeholder.
        /// </summary>
        public const string CvExtractionUser = """
            Parse this CV and extract structured data. Score skill proficiency based on:
            - Beginner: mentioned/touched/familiar with the skill
            - Intermediate: 1-3 years or project experience
            - Advanced: 3-5 years or extensive experience
            - Expert: 5+ years or demonstrated mastery

            Output JSON matching this schema:
            {
              "personalInfo": {
                "name": "string or null",
                "email": "string or null",
                "phone": "string or null"
              },
              "skills": [
                {
                  "name": "string",
                  "proficiencyLevel": "Beginner|Intermediate|Advanced|Expert",
                  "experienceContext": "string or null"
                }
              ],
              "workExperience": [
                {
                  "company": "string",
                  "position": "string",
                  "duration": "string or null",
                  "description": "string or null"
                }
              ],
              "education": [
                {
                  "institution": "string",
                  "degree": "string or null",
                  "year": "string or null"
                }
              ]
            }

            CV Text:
            {cvText}
            """;

        /// <summary>
        /// System prompt for job description extraction.
        /// </summary>
        public const string JobDescExtractionSystem = """
            You are an expert job requirement analyzer. Your task is to extract structured data from job descriptions.
            You must output valid JSON matching the specified schema exactly.
            Do not include any text outside the JSON object.
            If information is not available, use null for optional fields or empty arrays for lists.
            """;

        /// <summary>
        /// User prompt template for job description extraction. Use {jobDescText} placeholder.
        /// </summary>
        public const string JobDescExtractionUser = """
            Extract required skills with proficiency levels, experience requirements, and key competencies from this job description.

            Score required proficiency levels based on:
            - Beginner: entry-level or basic skill requirements
            - Intermediate: 1-3 years experience requirements
            - Advanced: 3-5 years or senior-level requirements
            - Expert: 5+ years or principal/lead-level requirements

            Output JSON matching this schema:
            {
              "requiredSkills": [
                {
                  "name": "string",
                  "requiredLevel": "Beginner|Intermediate|Advanced|Expert"
                }
              ],
              "experienceLevel": "string or null",
              "keyCompetencies": ["string"]
            }

            Job Description:
            {jobDescText}
            """;

        /// <summary>
        /// System prompt for question generation.
        /// </summary>
        public const string QuestionGenerationSystem = """
            You are an expert technical interviewer helping HR professionals conduct effective interviews.
            Generate interview questions that match candidate skills to job requirements.
            Questions should be appropriate for the candidate's proficiency level.
            Include plain English explanations of technical terms for non-technical recruiters.
            You must output valid JSON matching the specified schema exactly.
            Do not include any text outside the JSON object.
            """;

        /// <summary>
        /// User prompt template for question generation. Use {cvSkills} and {jobRequirements} placeholders.
        /// </summary>
        public const string QuestionGenerationUser = """
            Generate interview questions based on the candidate's skills and job requirements.

            Candidate Skills:
            {cvSkills}

            Job Requirements:
            {jobRequirements}

            Instructions:
            1. Group questions by topic (e.g., "Frontend - Angular", "Backend - TypeScript")
            2. Set difficulty level based on comparing candidate proficiency to job requirements:
               - If candidate skill matches requirement: use that level
               - If candidate skill is below requirement: use Intermediate/Advanced to assess growth potential
               - If candidate skill exceeds requirement: use Intermediate/Advanced to verify claimed expertise
            3. For each question, include:
               - Clear question text
               - Answer guidelines in natural language for non-technical recruiters
               - Key technical terms with plain English explanations
               - 2-3 suggested follow-up questions
            4. Generate 2-3 questions per topic group

            Output JSON matching this schema:
            {
              "topicGroups": [
                {
                  "topic": "string",
                  "difficultyLevel": "Beginner|Intermediate|Advanced|Expert",
                  "questions": [
                    {
                      "questionText": "string",
                      "answerGuidelines": "string",
                      "keyTerms": [
                        {
                          "term": "string",
                          "explanation": "string"
                        }
                      ],
                      "suggestedFollowUps": ["string"]
                    }
                  ]
                }
              ]
            }
            """;
    }

    /// <summary>
    /// Authentication-related constants.
    /// </summary>
    public static class Authentication
    {
        /// <summary>
        /// Header name for API key authentication.
        /// </summary>
        public const string ApiKeyHeaderName = "X-API-Key";

        /// <summary>
        /// Authentication scheme name.
        /// </summary>
        public const string SchemeName = "ApiKey";
    }
}
