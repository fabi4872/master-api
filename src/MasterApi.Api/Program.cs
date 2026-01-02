using System.Text;
using MasterApi.Api.Middlewares;
using MasterApi.Application;
using MasterApi.Infrastructure;
using MasterApi.Infrastructure.Authentication;
using MasterApi.Application.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// 1. Add services from other layers
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

// 2. Add services for the API layer
builder.Services.AddControllers();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");


builder.Services.AddExceptionHandler<GlobalExceptionHandlingMiddleware>();
builder.Services.AddProblemDetails();

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings not found.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUser", policy => policy.RequireRole("User"));

    options.AddPolicy("RequireUsersReadPermission", policy => policy.RequireClaim("permission", Permissions.UsersRead));
    options.AddPolicy("RequireUsersCreatePermission", policy => policy.RequireClaim("permission", Permissions.UsersCreate));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MasterApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br /> 
                        Enter 'Bearer' [space] and then your token in the text input below. <br />
                        Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

app.UseExceptionHandler();


// This will make sure that the CorrelationId is added to the logs
app.UseSerilogRequestLogging();

// 3. Configure the HTTP request pipeline.
// Enable Swagger only for Development and Staging environments
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Staging"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuditMiddleware>();

app.MapControllers();

app.Run();
