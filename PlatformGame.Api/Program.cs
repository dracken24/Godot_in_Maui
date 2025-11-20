using PlatformGame.Api.Services;
using DotNetEnv;

// Charger le fichier .env s'il existe (cherche à la racine du projet)
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}
else
{
    // Essayer aussi à la racine du projet API
    var localEnvPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(localEnvPath))
    {
        Env.Load(localEnvPath);
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Enregistrer le service Supabase
builder.Services.AddSingleton<SupabaseService>();

// Configurer CORS pour autoriser MAUI et Godot
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Désactiver la redirection HTTPS en développement pour permettre HTTP (nécessaire pour Android)
// En développement, on accepte HTTP pour faciliter les connexions depuis Android
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Utiliser CORS
app.UseCors("AllowAll");

// Mapper les contrôleurs
app.MapControllers();

app.Run();
