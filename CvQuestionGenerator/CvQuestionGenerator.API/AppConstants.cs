namespace CvQuestionGenerator.API;

/// <summary>
/// Application constants including AI prompt templates.
/// </summary>
public static class AppConstants
{
    /// <summary>
    /// Default AI deployment name for Azure OpenAI.
    /// </summary>
    public const string DefaultDeploymentName = "gpt-4o-mini";

    /// <summary>
    /// Maximum allowed text length for CV or job description input.
    /// </summary>
    public const int MaxTextLength = 50000;

    /// <summary>
    /// Number of questions to generate per skill topic.
    /// </summary>
    public const int QuestionsPerTopic = 5;

    /// <summary>
    /// AI prompt templates for extraction and generation.
    /// </summary>
    public static class Prompts
    {
        /// <summary>
        /// System prompt for extracting structured data from CV text.
        /// </summary>
        public const string CvExtraction = """
            You are an expert CV/resume parser. Extract structured data from the provided CV text.
            
            PROFICIENCY LEVEL INFERENCE RULES:
            - 0-2 years or "familiar", "touched", "basic", "exposure" = Beginner
            - 2-4 years or "worked with", "used", "experience" = Intermediate  
            - 4-6 years or "proficient", "strong", "skilled" = Advanced
            - 6+ years or "expert", "senior", "lead", "architect" = Expert
            
            SKILL CATEGORY RULES:
            - Frontend: HTML, CSS, JavaScript, TypeScript, React, Angular, Vue, etc.
            - Backend: Java, C#, Python, Node.js, Go, Ruby, PHP, etc.
            - Database: SQL, PostgreSQL, MySQL, MongoDB, Redis, etc.
            - DevOps: Docker, Kubernetes, CI/CD, Jenkins, etc.
            - Cloud: AWS, Azure, GCP, etc.
            - Mobile: iOS, Android, React Native, Flutter, etc.
            - MachineLearning: TensorFlow, PyTorch, ML models, etc.
            - Testing: Unit testing, integration testing, QA, etc.
            - SoftSkills: Communication, leadership, teamwork, etc.
            - Other: Any skill that doesn't fit above categories
            
            IMPORTANT:
            - DO NOT include contact information (email, phone, address) in the output
            - Extract as many skills as you can identify with their proficiency levels
            - If years of experience are mentioned, include them
            - Extract all work experience entries with responsibilities
            - Extract all education entries
            
            Return a valid JSON object with this exact structure:
            {
              "personalInfo": {
                "name": "string or null",
                "title": "string or null", 
                "summary": "string or null"
              },
              "skills": [
                {
                  "name": "string",
                  "category": "Frontend|Backend|Database|DevOps|Cloud|Mobile|MachineLearning|Testing|SoftSkills|Other",
                  "proficiency": "Beginner|Intermediate|Advanced|Expert",
                  "yearsOfExperience": number or null,
                  "context": "original text snippet or null"
                }
              ],
              "experience": [
                {
                  "jobTitle": "string",
                  "company": "string",
                  "duration": "string or null",
                  "responsibilities": ["string"]
                }
              ],
              "education": [
                {
                  "degree": "string",
                  "institution": "string",
                  "year": "string or null"
                }
              ],
              "warnings": ["string"] // Include if no skills found or data seems incomplete
            }
            """;

        // Job extraction prompt will be added in T041
        public const string JobExtraction = """
            You are an expert job description analyzer. Extract structured requirements from the provided job description text.
            
            PROFICIENCY LEVEL INFERENCE RULES:
            - "basic", "familiarity", "nice to have" = Beginner
            - "experience with", "knowledge of", "comfortable with" = Intermediate
            - "strong", "proficient", "solid experience", "3+ years" = Advanced
            - "expert", "senior", "5+ years", "lead", "architect" = Expert
            
            SKILL CATEGORY RULES:
            - Frontend: HTML, CSS, JavaScript, TypeScript, React, Angular, Vue, etc.
            - Backend: Java, C#, Python, Node.js, Go, Ruby, PHP, etc.
            - Database: SQL, PostgreSQL, MySQL, MongoDB, Redis, etc.
            - DevOps: Docker, Kubernetes, CI/CD, Jenkins, etc.
            - Cloud: AWS, Azure, GCP, etc.
            - Mobile: iOS, Android, React Native, Flutter, etc.
            - MachineLearning: TensorFlow, PyTorch, ML models, etc.
            - Testing: Unit testing, integration testing, QA, etc.
            - SoftSkills: Communication, leadership, teamwork, etc.
            - Other: Any skill that doesn't fit above categories
            
            IMPORTANT:
            - Extract all required and preferred skills mentioned
            - Identify key competencies (soft skills, behaviors, attributes)
            - Extract experience level requirements for different areas
            - If years of experience are mentioned, include them
            
            Return a valid JSON object with this exact structure:
            {
              "requiredSkills": [
                {
                  "name": "string",
                  "category": "Frontend|Backend|Database|DevOps|Cloud|Mobile|MachineLearning|Testing|SoftSkills|Other",
                  "proficiency": "Beginner|Intermediate|Advanced|Expert",
                  "yearsOfExperience": number or null,
                  "context": "original text snippet or null"
                }
              ],
              "competencies": ["string"], // Soft skills, behaviors, attributes like "leadership", "problem-solving"
              "experienceRequirements": [
                {
                  "area": "string", // e.g., "Java Development", "Cloud Architecture"
                  "minimumLevel": "Beginner|Intermediate|Advanced|Expert",
                  "yearsRequired": number or null
                }
              ],
              "warnings": ["string"] // Include if no skills found or requirements seem incomplete
            }
            """;

        /// <summary>
        /// System prompt for generating interview questions based on CV and job requirements.
        /// </summary>
        public const string QuestionGeneration = """
            You are an expert interview question generator for HR professionals. Generate thoughtful interview questions based on the candidate's CV and job requirements.
            
            DIFFICULTY CALIBRATION RULES:
            - If candidate proficiency is BELOW job requirement → Foundational questions (test basic understanding)
            - If candidate proficiency MATCHES job requirement → Standard questions (test practical application)
            - If candidate proficiency EXCEEDS job requirement → Advanced questions (test depth and edge cases)
            
            IMPORTANT CONSTRAINTS:
            - DO NOT generate questions about protected characteristics (age, religion, marital status, health, pregnancy, national origin, disability, etc.)
            - Focus ONLY on professional skills, experience, and competencies
            - Questions must be relevant to the job requirements
            - Avoid any questions that could be considered discriminatory
            - Each topic should have exactly 5 questions
            
            ANSWER GUIDELINES FORMAT:
            - Write in plain English for non-technical recruiters
            - Explain what good vs bad answers look like
            - Focus on behaviors and outcomes, not technical jargon
            
            KEY TERMS FORMAT:
            - Include 2-4 technical terms per question
            - Provide simple, plain English explanations
            - Help recruiters understand what candidates are talking about
            
            Return a valid JSON object with this exact structure:
            {
              "questionBlocks": [
                {
                  "topicLabel": "Category - SkillName", // e.g., "Backend - Java"
                  "difficulty": "Foundational|Standard|Advanced",
                  "questions": [
                    {
                      "text": "The interview question",
                      "answerGuidelines": "Non-technical explanation of what to look for in answers",
                      "keyTerms": [
                        {
                          "term": "Technical term",
                          "explanation": "Plain English explanation"
                        }
                      ],
                      "followUpQuestions": ["Follow-up question 1", "Follow-up question 2"]
                    }
                  ] // Exactly 5 questions per topic
                }
              ]
            }
            
            Generate questions for the most relevant skill matches between the CV and job requirements.
            Prioritize skills that are explicitly required in the job description.
            """;
    }
}
