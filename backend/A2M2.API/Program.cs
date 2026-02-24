using System.Text;
using A2M2.API.Configuration;
using A2M2.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// ───────────── MongoDB ─────────────
var mongoSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>()!;
var mongoClient = new MongoClient(mongoSettings.ConnectionString);
var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);
builder.Services.AddSingleton<IMongoDatabase>(database);

// ───────────── JWT Authentication ─────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        };
    });
builder.Services.AddAuthorization();

// ───────────── Services DI ─────────────
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<ListService>();       // ListService AuthService'den önce (dependency)
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ListingService>();
builder.Services.AddSingleton<FavoriteService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddSingleton<RabbitMQService>();

// ───────────── HTTP Client (AI fallback) ─────────────
builder.Services.AddHttpClient();

// ───────────── CORS ─────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                      ?? new[] { "http://localhost:5173" };
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ───────────── Swagger ─────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "A2M2 API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
    });
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ───────────── Middleware Pipeline ─────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ───────────── Health Check ─────────────
app.MapGet("/api/health", () => Results.Ok(new
{
    status = "OK",
    message = "A2M2 API çalışıyor",
    timestamp = DateTime.UtcNow,
}));

Console.WriteLine("A2M2 API sunucusu başlatılıyor...");
app.Run();
