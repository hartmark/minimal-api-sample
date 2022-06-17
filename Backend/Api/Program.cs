using System.Text.Json;
using System.Text.Json.Serialization;
using Api;
using Api.Routes;
using Api.Validation;
using FluentValidation.AspNetCore;
using JWT.Extensions.AspNetCore;
using Microsoft.AspNetCore.Http.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddApiVersioning(setup =>
    {
        setup.ReportApiVersions = true;
    })
    .AddApiExplorer(setup =>
    {
        setup.GroupNameFormat = "'v'VVV";
        setup.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddDependencyInjection();

builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddLogging(logging => { logging.SetMinimumLevel(LogLevel.Debug); });
builder.Services.AddFileLogging(builder.Configuration);

builder.Services.AddFluentValidation(v => v.RegisterValidatorsFromAssemblyContaining<WeatherEntryValidator>());

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    Formatting = Formatting.None,
    NullValueHandling = NullValueHandling.Ignore
};

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

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

app.MapWeatherForecastRoutes();
app.MapAuthRoutes();

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

app.Run();