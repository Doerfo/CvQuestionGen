var builder = DistributedApplication.CreateBuilder(args);

// Configure Azure OpenAI connection
var openAi = builder.AddConnectionString("open-ai");

var api = builder.AddProject<Projects.CvQuestionGenerator_API>("api")
    .WithReference(openAi)
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Swagger UI";
        url.Url = "/swagger";
    });

builder.Build().Run();