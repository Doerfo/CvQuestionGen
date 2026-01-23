var builder = DistributedApplication.CreateBuilder(args);

// Add Azure OpenAI connection string
var openAi = builder.AddConnectionString("open-ai");

var api = builder.AddProject<Projects.CvQuestionGenerator_API>("api")
    .WithExternalHttpEndpoints()
    .WithReference(openAi)
    .WaitFor(openAi)
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Swagger UI";
        url.Url = "/swagger";
    });

builder.Build().Run();