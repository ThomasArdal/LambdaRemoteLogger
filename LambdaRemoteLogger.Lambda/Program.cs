using Amazon.Lambda.Core;
using AspNetCore.Authentication.ApiKey;
using LambdaRemoteLogger.Lambda.Authentication;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Options;
using LambdaRemoteLogger.Lambda.Logging;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Endpoints
builder.Services.AddFastEndpoints();

// Making it lambda
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Auth
builder.Services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
    .AddApiKeyInHeader<ApiKeyProvider>(opt =>
    {
        opt.Realm = "Lambda logger";
        opt.KeyName = "api_key";
        opt.IgnoreAuthenticationIfAllowAnonymous = true;
    });

builder.Services.AddLogging(logging =>
{
    logging.AddRemoteLogger();
});

var app = builder.Build();

// Https redirection
app.UseHttpsRedirection();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Fast endpoints
app.UseFastEndpoints();

// Swagger
app.UseOpenApi();
app.UseSwaggerUi3(s =>
{
    s.ConfigureDefaults();
});


app.Run();