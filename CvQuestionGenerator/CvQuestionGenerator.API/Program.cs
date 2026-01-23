using CvQuestionGenerator.API;
using CvQuestionGenerator.API.Authentication;
using CvQuestionGenerator.API.Repositories;
using CvQuestionGenerator.API.Services;
using CvQuestionGenerator.ServiceDefaults;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add Aspire Azure OpenAI chat client
builder.AddAzureOpenAIClient("open-ai").AddChatClient("gpt-4o-mini");

// Register singleton repositories (one CV, one job description at a time)
builder.Services.AddSingleton<ICVRepository, SingletonCVRepository>();
builder.Services.AddSingleton<IJobDescRepository, SingletonJobDescRepository>();

// Register AI service
builder.Services.AddScoped<IAIService, AIService>();

// Register application services
builder.Services.AddScoped<ICVService, CVService>();
builder.Services.AddScoped<IJobDescService, JobDescService>();
builder.Services.AddScoped<IQuestionGenerationService, QuestionGenerationService>();

// Add API key authentication
builder.Services.AddAuthentication(AppConstants.Authentication.SchemeName)
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
        AppConstants.Authentication.SchemeName, 
        options => { });
builder.Services.AddAuthorization();

// Add controllers
builder.Services.AddControllers();

// Configure Swagger with API key support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CV Question Generator API",
        Version = "v1",
        Description = "API for generating AI-powered interview questions based on CVs and job descriptions"
    });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = AppConstants.Authentication.ApiKeyHeaderName,
        Description = "API Key Authentication. Enter your API key in the field below."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("ApiKey", document),
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CV Question Generator API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();