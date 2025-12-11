var builder = DistributedApplication.CreateBuilder(args);


var api = builder.AddProject<Projects.CvQuestionGenerator_API>("api")
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Swagger UI";
        url.Url = "/swagger";
    });

builder.Build().Run();