using System.Security.Claims;
using System.Text.Encodings.Web;
using CvQuestionGenerator.API;
using CvQuestionGenerator.API.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace CvQuestionGenerator.Tests.Authentication;

public class ApiKeyAuthenticationHandlerTests
{
    private readonly IConfiguration _configuration;
    private readonly IOptionsMonitor<ApiKeyAuthenticationOptions> _options;
    private readonly ILoggerFactory _loggerFactory;
    private readonly UrlEncoder _urlEncoder;

    private const string ValidApiKey = "test-valid-api-key";

    public ApiKeyAuthenticationHandlerTests()
    {
        var configData = new Dictionary<string, string?>
        {
            { "Authentication:ApiKey", ValidApiKey }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _options = Substitute.For<IOptionsMonitor<ApiKeyAuthenticationOptions>>();
        _options.Get(Arg.Any<string>()).Returns(new ApiKeyAuthenticationOptions());
        _options.CurrentValue.Returns(new ApiKeyAuthenticationOptions());

        _loggerFactory = Substitute.For<ILoggerFactory>();
        _loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

        _urlEncoder = UrlEncoder.Default;
    }

    private async Task<ApiKeyAuthenticationHandler> CreateHandler(HttpContext httpContext)
    {
        var handler = new ApiKeyAuthenticationHandler(_options, _loggerFactory, _urlEncoder, _configuration);
        
        var scheme = new AuthenticationScheme(
            AppConstants.Authentication.SchemeName, 
            AppConstants.Authentication.SchemeName, 
            typeof(ApiKeyAuthenticationHandler));
        
        await handler.InitializeAsync(scheme, httpContext);
        return handler;
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WithValidApiKey_ReturnsSuccess()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[AppConstants.Authentication.ApiKeyHeaderName] = ValidApiKey;

        var handler = await CreateHandler(httpContext);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Principal);
        Assert.Contains(result.Principal.Claims, c => c.Type == ClaimTypes.Name && c.Value == "ApiKeyUser");
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WithInvalidApiKey_ReturnsFailure()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[AppConstants.Authentication.ApiKeyHeaderName] = "invalid-api-key";

        var handler = await CreateHandler(httpContext);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Invalid API key", result.Failure?.Message);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WithMissingApiKey_ReturnsFailure()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        // No API key header added

        var handler = await CreateHandler(httpContext);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("API key is missing", result.Failure?.Message);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WithEmptyApiKeyHeader_ReturnsFailure()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[AppConstants.Authentication.ApiKeyHeaderName] = "";

        var handler = await CreateHandler(httpContext);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
    }
}
