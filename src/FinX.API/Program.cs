using System.Text;
using FinX.Application.DTOs.Appointment;
using FinX.Application.DTOs.Patient;
using FinX.Application.Interfaces;
using FinX.Application.Services;
using FinX.Application.Validators.Appointment;
using FinX.Application.Validators.Patient;
using FinX.Domain.Interfaces;
using FinX.Infrastructure.Data;
using FinX.Infrastructure.Data.Repositories;
using FinX.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinX API", Version = "v1" });
    
    // Add JWT Authentication to Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header usando o esquema Bearer. \r\n\r\n Digite 'Bearer' [espaço] e seu token.\r\n\r\nExemplo: \"Bearer eyJhbGciOiJIUzI1NiIsIn\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(PatientDto).Assembly);

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePatientDtoValidator>();

// Register validators manually
builder.Services.AddScoped<IValidator<CreateAppointmentDto>, CreateAppointmentDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateAppointmentDto>, UpdateAppointmentDtoValidator>();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IMedicalHistoryRepository, MedicalHistoryRepository>();
builder.Services.AddScoped<IExternalExamRepository, ExternalExamRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// Add Services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();

// Usando o mock service para ExternalExam em vez da implementação real
builder.Services.AddScoped<IExternalExamService, MockExternalExamService>();

// Add Appointment Service
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Apply migrations at startup with retry mechanism
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // Create a retry policy
    var retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetry(
            retryCount: 5, 
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
            onRetry: (exception, sleepDuration, attemptNumber, context) =>
            {
                Log.Warning(
                    exception,
                    "Retry {RetryCount} encountered an error while connecting to the database. Waiting {TimeSpan} before next retry.", 
                    attemptNumber, sleepDuration);
            });

    // Execute with retry policy
    retryPolicy.Execute(() =>
    {
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            Log.Information("Checking database connection...");
            
            // Test database connection (synchronous version)
            if (context.Database.CanConnect())
            {
                Log.Information("Database connection successful.");

                if (context.Database.GetPendingMigrations().Any())
                {
                    Log.Information("Applying pending database migrations...");
                    context.Database.Migrate();
                    Log.Information("Database migrations applied successfully.");
                }
                else
                {
                    Log.Information("No pending migrations to apply.");
                }
            }
            else
            {
                Log.Error("Could not connect to the database.");
                throw new Exception("Database connection failed");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while migrating the database.");
            throw; // Rethrow to activate the retry policy
        }
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Remove HTTPS redirection as we're only using HTTP
// app.UseHttpsRedirection();

// Add authentication middleware - must be before authorization
app.UseAuthentication();
app.UseAuthorization();

// Map health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
