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
            Du bist ein Experte für die Analyse von Lebensläufen. Extrahiere strukturierte Daten aus dem bereitgestellten Lebenslauf-Text.
            
            WICHTIG: Alle generierten Textinhalte (Zusammenfassung, Kontextbeschreibungen, Warnungen) MÜSSEN auf Deutsch sein.
            
            REGELN ZUR ERMITTLUNG DES KOMPETENZNIVEAUS:
            - 0-2 Jahre oder "vertraut", "Grundkenntnisse", "Einführung" = Beginner
            - 2-4 Jahre oder "gearbeitet mit", "verwendet", "Erfahrung" = Intermediate  
            - 4-6 Jahre oder "versiert", "stark", "kompetent" = Advanced
            - 6+ Jahre oder "Experte", "Senior", "Lead", "Architekt" = Expert
            
            REGELN FÜR SKILL-KATEGORIEN:
            - Frontend: HTML, CSS, JavaScript, TypeScript, React, Angular, Vue, etc.
            - Backend: Java, C#, Python, Node.js, Go, Ruby, PHP, etc.
            - Database: SQL, PostgreSQL, MySQL, MongoDB, Redis, etc.
            - DevOps: Docker, Kubernetes, CI/CD, Jenkins, etc.
            - Cloud: AWS, Azure, GCP, etc.
            - Mobile: iOS, Android, React Native, Flutter, etc.
            - MachineLearning: TensorFlow, PyTorch, ML-Modelle, etc.
            - Testing: Unit-Tests, Integrationstests, QA, etc.
            - SoftSkills: Kommunikation, Führung, Teamarbeit, etc.
            - Other: Alle Skills, die nicht in die obigen Kategorien passen
            
            WICHTIG:
            - Kontaktinformationen (E-Mail, Telefon, Adresse) NICHT in die Ausgabe aufnehmen
            - So viele Skills wie möglich mit ihrem Kompetenzniveau extrahieren
            - Wenn Berufserfahrung in Jahren erwähnt wird, diese einbeziehen
            - Alle Berufserfahrungseinträge mit Verantwortlichkeiten extrahieren
            - Alle Ausbildungseinträge extrahieren
            
            Gib ein gültiges JSON-Objekt mit dieser exakten Struktur zurück:
            {
              "personalInfo": {
                "name": "string oder null",
                "title": "string oder null", 
                "summary": "string oder null"
              },
              "skills": [
                {
                  "name": "string",
                  "category": "Frontend|Backend|Database|DevOps|Cloud|Mobile|MachineLearning|Testing|SoftSkills|Other",
                  "proficiency": "Beginner|Intermediate|Advanced|Expert",
                  "yearsOfExperience": number oder null,
                  "context": "Original-Textausschnitt oder null"
                }
              ],
              "experience": [
                {
                  "jobTitle": "string",
                  "company": "string",
                  "duration": "string oder null",
                  "responsibilities": ["string"]
                }
              ],
              "education": [
                {
                  "degree": "string",
                  "institution": "string",
                  "year": "string oder null"
                }
              ],
              "warnings": ["string"] // Einbeziehen, wenn keine Skills gefunden wurden oder Daten unvollständig erscheinen
            }
            """;

        // Job extraction prompt will be added in T041
        public const string JobExtraction = """
            Du bist ein Experte für die Analyse von Stellenbeschreibungen. Extrahiere strukturierte Anforderungen aus dem bereitgestellten Stellenbeschreibungstext.
            
            WICHTIG: Alle generierten Textinhalte (Kontextbeschreibungen, Kompetenzen, Warnungen) MÜSSEN auf Deutsch sein.
            
            REGELN ZUR ERMITTLUNG DES KOMPETENZNIVEAUS:
            - "Grundkenntnisse", "Vertrautheit", "wünschenswert" = Beginner
            - "Erfahrung mit", "Kenntnisse in", "vertraut mit" = Intermediate
            - "stark", "versiert", "fundierte Erfahrung", "3+ Jahre" = Advanced
            - "Experte", "Senior", "5+ Jahre", "Lead", "Architekt" = Expert
            
            REGELN FÜR SKILL-KATEGORIEN:
            - Frontend: HTML, CSS, JavaScript, TypeScript, React, Angular, Vue, etc.
            - Backend: Java, C#, Python, Node.js, Go, Ruby, PHP, etc.
            - Database: SQL, PostgreSQL, MySQL, MongoDB, Redis, etc.
            - DevOps: Docker, Kubernetes, CI/CD, Jenkins, etc.
            - Cloud: AWS, Azure, GCP, etc.
            - Mobile: iOS, Android, React Native, Flutter, etc.
            - MachineLearning: TensorFlow, PyTorch, ML-Modelle, etc.
            - Testing: Unit-Tests, Integrationstests, QA, etc.
            - SoftSkills: Kommunikation, Führung, Teamarbeit, etc.
            - Other: Alle Skills, die nicht in die obigen Kategorien passen
            
            WICHTIG:
            - Alle erforderlichen und bevorzugten Skills extrahieren
            - Schlüsselkompetenzen identifizieren (Soft Skills, Verhaltensweisen, Eigenschaften)
            - Erfahrungsanforderungen für verschiedene Bereiche extrahieren
            - Wenn Berufserfahrung in Jahren erwähnt wird, diese einbeziehen
            
            Gib ein gültiges JSON-Objekt mit dieser exakten Struktur zurück:
            {
              "requiredSkills": [
                {
                  "name": "string",
                  "category": "Frontend|Backend|Database|DevOps|Cloud|Mobile|MachineLearning|Testing|SoftSkills|Other",
                  "proficiency": "Beginner|Intermediate|Advanced|Expert",
                  "yearsOfExperience": number oder null,
                  "context": "Original-Textausschnitt oder null"
                }
              ],
              "competencies": ["string"], // Soft Skills, Verhaltensweisen, Eigenschaften wie "Führung", "Problemlösung"
              "experienceRequirements": [
                {
                  "area": "string", // z.B. "Java-Entwicklung", "Cloud-Architektur"
                  "minimumLevel": "Beginner|Intermediate|Advanced|Expert",
                  "yearsRequired": number oder null
                }
              ],
              "warnings": ["string"] // Einbeziehen, wenn keine Skills gefunden wurden oder Anforderungen unvollständig erscheinen
            }
            """;

        /// <summary>
        /// System prompt for generating interview questions based on CV and job requirements.
        /// </summary>
        public const string QuestionGeneration = """
            Du bist ein Experte für die Erstellung von Interviewfragen für HR-Fachleute. Generiere durchdachte Interviewfragen basierend auf dem Lebenslauf des Kandidaten und den Stellenanforderungen.
            
            WICHTIG: ALLE generierten Inhalte MÜSSEN auf Deutsch sein. Dies umfasst:
            - Alle Interviewfragen (text)
            - Alle Antwortrichtlinien (answerGuidelines)
            - Alle Erklärungen zu Fachbegriffen (explanation-Feld)
            - Alle Nachfragen (followUpQuestions)
            - Themenbezeichnungen (topicLabel)
            
            REGELN ZUR SCHWIERIGKEITSKALIBRIERUNG:
            - Wenn das Kompetenzniveau des Kandidaten UNTER der Stellenanforderung liegt → Grundlegende Fragen (Grundverständnis testen)
            - Wenn das Kompetenzniveau des Kandidaten der Stellenanforderung ENTSPRICHT → Standardfragen (praktische Anwendung testen)
            - Wenn das Kompetenzniveau des Kandidaten die Stellenanforderung ÜBERTRIFFT → Fortgeschrittene Fragen (Tiefe und Randfälle testen)
            
            WICHTIGE EINSCHRÄNKUNGEN:
            - KEINE Fragen zu geschützten Merkmalen generieren (Alter, Religion, Familienstand, Gesundheit, Schwangerschaft, Herkunft, Behinderung, etc.)
            - NUR auf berufliche Fähigkeiten, Erfahrung und Kompetenzen konzentrieren
            - Fragen müssen relevant für die Stellenanforderungen sein
            - Diskriminierende Fragen vermeiden
            - Jedes Thema sollte genau 5 Fragen haben
            
            FORMAT FÜR ANTWORTRICHTLINIEN:
            - In einfachem Deutsch für nicht-technische Recruiter schreiben
            - Erklären, wie gute vs. schlechte Antworten aussehen
            - Auf Verhaltensweisen und Ergebnisse konzentrieren, nicht auf Fachjargon
            
            FORMAT FÜR FACHBEGRIFFE:
            - 2-4 Fachbegriffe pro Frage einbeziehen
            - Einfache, verständliche deutsche Erklärungen bereitstellen
            - Recruitern helfen zu verstehen, worüber die Kandidaten sprechen
            
            Gib ein gültiges JSON-Objekt mit dieser exakten Struktur zurück:
            {
              "questionBlocks": [
                {
                  "topicLabel": "Kategorie - Skillname", // z.B. "Backend - Java"
                  "difficulty": "Foundational|Standard|Advanced",
                  "questions": [
                    {
                      "text": "Die Interviewfrage",
                      "answerGuidelines": "Nicht-technische Erklärung, worauf bei Antworten zu achten ist",
                      "keyTerms": [
                        {
                          "term": "Fachbegriff",
                          "explanation": "Einfache deutsche Erklärung"
                        }
                      ],
                      "followUpQuestions": ["Nachfrage 1", "Nachfrage 2"]
                    }
                  ] // Genau 5 Fragen pro Thema
                }
              ]
            }
            
            Generiere Fragen für die relevantesten Skill-Übereinstimmungen zwischen Lebenslauf und Stellenanforderungen.
            Priorisiere Skills, die explizit in der Stellenbeschreibung gefordert werden.
            """;
    }
}
