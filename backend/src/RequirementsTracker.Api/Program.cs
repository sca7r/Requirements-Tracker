using Microsoft.EntityFrameworkCore;
using RequirementsTracker.Api.Data;
using RequirementsTracker.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Services -----------------------------------------------------------

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Provider switch: set "Database:Provider" in appsettings to "Sqlite" or "Postgres".
var provider = builder.Configuration.GetValue<string>("Database:Provider") ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString(provider)
    ?? throw new InvalidOperationException($"No connection string configured for provider '{provider}'.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    switch (provider.ToLowerInvariant())
    {
        case "postgres":
            options.UseNpgsql(connectionString);
            break;
        case "sqlite":
            options.UseSqlite(connectionString);
            break;
        default:
            throw new InvalidOperationException($"Unsupported database provider '{provider}'.");
    }
});

// Allow CORS for the Vite dev server
const string DevCorsPolicy = "DevCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(DevCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- Pipeline -----------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(DevCorsPolicy);
}

app.UseHttpsRedirection();
app.MapControllers();

// Create schema and seed demo data on startup.
// Dev (SQLite): uses EnsureCreated — no migration tooling required.
// Prod (Postgres): uses EF Core migrations — see README for how to generate them.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
    {
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }

    if (!db.Projects.Any())
    {
        var demo = new Project
        {
            Name = "Website Relaunch",
            CustomerName = "Acme Corp",
            Description = "Rebuild the existing customer website on .NET 8 and React.",
            Requirements =
            {
                new Requirement
                {
                    Title = "Responsive Layout",
                    Description = "The site must work correctly on mobile, tablet, and desktop.",
                    Type = RequirementType.NonFunctional,
                    Priority = Priority.Must,
                    EstimatedHours = 16
                },
                new Requirement
                {
                    Title = "Contact Form with Email Delivery",
                    Description = "The submission form must send emails to info@acme.com.",
                    Type = RequirementType.Functional,
                    Priority = Priority.Must,
                    EstimatedHours = 8
                },
                new Requirement
                {
                    Title = "Dark Mode",
                    Type = RequirementType.NonFunctional,
                    Priority = Priority.Could,
                    EstimatedHours = 4
                }
            }
        };
        db.Projects.Add(demo);
        db.SaveChanges();
    }
}

app.Run();
