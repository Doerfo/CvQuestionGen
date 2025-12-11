using CvQuestionGenerator.API.Endpoints;
using CvQuestionGenerator.API.Services;
using CvQuestionGenerator.API.Storage;
using CvQuestionGenerator.ServiceDefaults;
using Microsoft.Extensions.AI;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service discovery
builder.AddServiceDefaults();

// Configure Azure OpenAI client
builder.AddAzureOpenAIClient("open-ai")
    .AddChatClient();

// Register storage
builder.Services.AddSingleton<IDataStore, InMemoryDataStore>();

// Register services
builder.Services.AddScoped<IAiExtractionService, AiExtractionService>();
builder.Services.AddScoped<ICvService, CvService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

// Configure RFC 7807 Problem Details
builder.Services.AddProblemDetails();

// Configure OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Enable exception handler for Problem Details
app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

// Map endpoints
app.MapCvEndpoints();
app.MapJobEndpoints();
app.MapQuestionEndpoints();

app.Run();