using backend.API.Modules.Auth.Application;
using backend.API.Modules.Auth.Infrastructure;
using backend.API.Modules.Cars.Infrastructure;
using backend.API.Modules.Cars.Application;
using backend.API.Modules.Cars.Infrastructure;
using backend.API.Modules.Comments.Application;
using backend.API.Modules.Comments.Infrastructure;
using backend.API.Modules.Lists.Application;
using backend.API.Modules.Lists.Infrastructure;
using backend.API.Modules.Prediction.Application;
using backend.API.Modules.Prediction.Infrastructure;
using backend.API.Presentation.Middlewares;
using backend.API.Shared.Database;
using backend.API.Shared.Messaging;
using backend.API.Shared.Security;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// ── Port ──────────────────────────────────────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ── MongoDB ───────────────────────────────────────────────────
var mongoConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

var mongoSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);
// Bağlantı kurulamadığında uygulamanın hemen çökmemesi için timeout'ları kıs
mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
mongoSettings.ConnectTimeout         = TimeSpan.FromSeconds(10);
mongoSettings.SocketTimeout          = TimeSpan.FromSeconds(30);

builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoSettings));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<MongoTransactionManager>();

// Index oluşturma — uygulama başlarken tek seferlik, async olarak çalışır
builder.Services.AddHostedService<backend.API.Shared.Database.MongoIndexInitializer>();

// ── Redis ─────────────────────────────────────────────────────
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
var redisAvailable = false;
if (!string.IsNullOrEmpty(redisConnectionString))
{
    try
    {
        var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
        redisOptions.AbortOnConnectFail = false;
        redisOptions.ConnectTimeout     = 5000;
        redisOptions.SyncTimeout        = 5000;

        var mux = ConnectionMultiplexer.Connect(redisOptions);
        builder.Services.AddSingleton<IConnectionMultiplexer>(mux);
        redisAvailable = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Redis] Bağlantı kurulamadı, fallback servisler kullanılacak: {ex.Message}");
    }
}

// ── RabbitMQ ──────────────────────────────────────────────────
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

// ── JWT ───────────────────────────────────────────────────────
var jwtSecret   = Environment.GetEnvironmentVariable("JWT_SECRET")   ?? builder.Configuration["JWT_SECRET"]!;
var jwtIssuer   = Environment.GetEnvironmentVariable("ISSUER")       ?? builder.Configuration["ISSUER"]!;
var jwtAudience = Environment.GetEnvironmentVariable("AUDIENCE")     ?? builder.Configuration["AUDIENCE"]!;

builder.Services.AddSingleton<JwtTokenGenerator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime         = true,
            ValidIssuer              = jwtIssuer,
            ValidAudience            = jwtAudience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew                = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ── Controllers + JSON ────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy         = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive  = true;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddHttpClient();

// ── Swagger ───────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OtoPusula API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization: 'Bearer <token>'",
        Name        = "Authorization",
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.ApiKey,
        Scheme      = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ── Exception Handler ─────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ── CORS ──────────────────────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ── MediatR ───────────────────────────────────────────────────
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// ═══════════════════════════════════════════════════════════════
// MODÜL A — Auth & Kullanıcı (Burak Şişci)
// ═══════════════════════════════════════════════════════════════
// Singleton: IMongoCollection<T> thread-safe, her istekte yeni nesne yaratmaya gerek yok
builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
if (redisAvailable)
    builder.Services.AddScoped<ITokenBlacklist, RedisTokenBlacklist>();
else
    builder.Services.AddScoped<ITokenBlacklist, NoOpTokenBlacklist>();
builder.Services.AddScoped<RegisterUserCommand>();
builder.Services.AddScoped<LoginUserCommand>();
builder.Services.AddScoped<LogoutUserCommand>();

// ═══════════════════════════════════════════════════════════════
// MODÜL B — Araç İlanları & Yapay Zeka (Anıl Elmaz)
// ═══════════════════════════════════════════════════════════════
builder.Services.AddSingleton<ICarRepository, MongoCarRepository>();
if (redisAvailable)
    builder.Services.AddScoped<ICarCacheService, RedisCarCacheService>();
else
    builder.Services.AddScoped<ICarCacheService, NoOpCarCacheService>();
builder.Services.AddScoped<GetCarsQuery>();
builder.Services.AddScoped<GetCarByIdQuery>();
builder.Services.AddScoped<AddCarCommand>();
builder.Services.AddScoped<UpdateCarCommand>();

builder.Services.AddHttpClient<IPredictionService, PredictionService>(client =>
{
    var fastApiUrl = Environment.GetEnvironmentVariable("FASTAPI_BASE_URL")
        ?? builder.Configuration["FastApi:BaseUrl"]
        ?? "http://127.0.0.1:8000";
    client.BaseAddress = new Uri(fastApiUrl);
    client.Timeout     = TimeSpan.FromSeconds(30);
});

// ═══════════════════════════════════════════════════════════════
// MODÜL C — Özel Listeler & Favoriler (Mehmet Öz)
// ═══════════════════════════════════════════════════════════════
builder.Services.AddSingleton<IListRepository, MongoListRepository>();
if (redisAvailable)
    builder.Services.AddScoped<IListCacheService, RedisListCacheService>();
else
    builder.Services.AddScoped<IListCacheService, NoOpListCacheService>();
builder.Services.AddScoped<CreateListCommand>();
builder.Services.AddScoped<AddCarToListCommand>();
builder.Services.AddScoped<RemoveCarFromListCommand>();
builder.Services.AddScoped<UpdateListNameCommand>();
builder.Services.AddScoped<DeleteListCommand>();
builder.Services.AddScoped<CreateDefaultListCommand>();
builder.Services.AddScoped<AddItemToListCommand>();

// ═══════════════════════════════════════════════════════════════
// MODÜL D — Yorumlar & Paylaşım (Mehmet Uludağ)
// ═══════════════════════════════════════════════════════════════
builder.Services.AddSingleton<ICommentRepository, MongoCommentRepository>();
if (redisAvailable)
    builder.Services.AddScoped<IShareLinkService, RedisShareLinkService>();
else
    builder.Services.AddScoped<IShareLinkService, LocalShareLinkService>();
builder.Services.AddScoped<GetCarCommentsQuery>();
builder.Services.AddScoped<GetCommentQuery>();
builder.Services.AddScoped<AddCommentCommand>();
builder.Services.AddScoped<UpdateCommentCommand>();
builder.Services.AddScoped<DeleteCommentCommand>();
builder.Services.AddScoped<LikeCommentCommand>();
builder.Services.AddScoped<UnlikeCommentCommand>();

// ═══════════════════════════════════════════════════════════════
var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseCors();
app.UseStaticFiles();
app.UseMiddleware<backend.API.Presentation.Middlewares.JwtAuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { durum = "saglikli" }));

app.Run();
