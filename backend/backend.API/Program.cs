using backend.API.Shared.Database;
using backend.API.Shared.Security;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using System.Text;
using Microsoft.OpenApi;
using backend.API.Modules.Comments.Application;
using backend.API.Modules.Comments.Infrastructure;
using backend.API.Presentation.Middlewares;
using backend.API.Modules.Auth.Application;
using backend.API.Modules.Auth.Infrastructure;
using backend.API.Modules.Cars.Application;
using backend.API.Modules.Cars.Infrastructure;
using backend.API.Modules.Lists.Application;
using backend.API.Modules.Lists.Infrastructure;
using MongoDB.Driver;
using StackExchange.Redis;
using backend.API.Modules.Prediction.Application;
using backend.API.Modules.Prediction.Infrastructure;
Env.Load();
var builder = WebApplication.CreateBuilder(args);


var mongoConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));




// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddHttpClient();


// ── Swagger / OpenAPI ─────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Car Market API", Version = "v1" });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Lütfen 'Bearer' boşluk 'Token_Değeriniz' formatında giriş yapın. \r\n\r\nÖrnek: \"Bearer eyJhbGci...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
         Array.Empty<string>()
     }
 });


});


// JWT Authentication
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration["JWT_SECRET"]!;
var jwtIssuer = Environment.GetEnvironmentVariable("ISSUER") ?? builder.Configuration["ISSUER"]!;
var jwtAudience = Environment.GetEnvironmentVariable("AUDIENCE") ?? builder.Configuration["AUDIENCE"]!;




builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorization();




// ── Redis Yapılandırması ─────────────────────────────────────────────────────
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");


if (!string.IsNullOrEmpty(redisConnectionString))
{
    // ConfigurationOptions kullanarak daha güvenli bir başlatma yapıyoruz
    var options = ConfigurationOptions.Parse(redisConnectionString);
    
    // Uygulama başlarken Redis'e ulaşamazsa uygulamanın çökmesini engeller
    options.AbortOnConnectFail = false; 
    
    // Bağlantı zaman aşımı sürelerini biraz artırmak Cloud bağlantılarında faydalıdır
    options.ConnectTimeout = 10000; // 10 saniye
    options.SyncTimeout = 10000;


    builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
    {
        return ConnectionMultiplexer.Connect(options);
    });
}




// ── Exception Handler ─────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();




// Shared configiruration
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<MongoTransactionManager>();
builder.Services.AddSingleton<JwtTokenGenerator>();


// ── Comments Module ───────────────────────────────────────────
builder.Services.AddScoped<ICommentRepository, MongoCommentRepository>();
builder.Services.AddScoped<GetCarCommentsQuery>();
builder.Services.AddScoped<AddCommentCommand>();


builder.Services.AddScoped<GetCommentQuery>();
builder.Services.AddScoped<UpdateCommentCommand>();
builder.Services.AddScoped<DeleteCommentCommand>();


// ── Auth Module ───────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, MongoUserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<ITokenBlacklist, RedisTokenBlacklist>();
builder.Services.AddScoped<RegisterUserCommand>();
builder.Services.AddScoped<LoginUserCommand>();
builder.Services.AddScoped<LogoutUserCommand>();


// ── Cars Module ───────────────────────────────────────────────
builder.Services.AddScoped<ICarRepository, MongoCarRepository>();
builder.Services.AddScoped<GetCarsQuery>();
builder.Services.AddScoped<AddCarCommand>();


// -- Prediction Module (ML modeli için)-------------------------
builder.Services.AddHttpClient<IPredictionService, PredictionService>(client =>
{
    var fastApiUrl = Environment.GetEnvironmentVariable("FASTAPI_BASE_URL")
        ?? builder.Configuration["FastApi:BaseUrl"]
        ?? "http://127.0.0.1:8000";
    client.BaseAddress = new Uri(fastApiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});


// ── Lists Module ──────────────────────────────────────────────
builder.Services.AddScoped<IListRepository, MongoListRepository>();
builder.Services.AddScoped<CreateDefaultListCommand>();
builder.Services.AddScoped<AddItemToListCommand>();


// ── CORS ──────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


// MediatR Entegrasyonu 
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));




var app = builder.Build();


app.UseExceptionHandler();


app.UseSwagger();
app.UseSwaggerUI();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseCors();


app.UseStaticFiles();


app.UseAuthentication();


app.UseAuthorization();


app.MapControllers();


app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));


app.Run();







