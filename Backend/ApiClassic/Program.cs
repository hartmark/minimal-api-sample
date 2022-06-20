using System.Text.Json;
using System.Text.Json.Serialization;
using ApiClassic.Extensions;
using ApiClassic.Validation;
using FluentValidation.AspNetCore;
using JWT.Extensions.AspNetCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencyInjection();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            $"JWT Authorization header using the Bearer scheme. {Environment.NewLine}" +
            "Enter \"Bearer 'JWT'\" in the text input below."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });    
});

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFile(builder.Configuration);

builder.Services.AddFluentValidation(v => v.RegisterValidatorsFromAssemblyContaining<WeatherEntryValidator>());

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    Formatting = Formatting.None,
    NullValueHandling = NullValueHandling.Ignore
};

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtAuthenticationDefaults.AuthenticationScheme;
    })
    .AddJwt(options =>
    {
        options.Keys = new[] { builder.Configuration.GetSecret() };
        //options.Keys = null;
        options.VerifySignature = true;
    });

var app = builder.Build();

app.UseAuthentication();
app.UseJwtAuthentication();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "WeatherService - Swagger";
        c.RoutePrefix = string.Empty;
        c.SwaggerEndpoint($"{builder.Configuration.GetSwaggerBase()}/swagger/v1/swagger.json", "v1");
    });
}

app.MapControllers();

app.Run();

/// <summary>
/// We need this to differentiate between the APis in the test
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
internal class ApiClassicProgram : Program {}