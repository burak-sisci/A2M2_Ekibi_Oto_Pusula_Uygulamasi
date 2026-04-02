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

// ── Exception Handler ─────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


// Shared configiruration
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<MongoTransactionManager>();

// ── Comments Module ───────────────────────────────────────────
builder.Services.AddScoped<ICommentRepository, MongoCommentRepository>();
builder.Services.AddScoped<GetCarCommentsQuery>();
builder.Services.AddScoped<AddCommentCommand>();

builder.Services.AddScoped<GetCommentQuery>();
builder.Services.AddScoped<UpdateCommentCommand>();
builder.Services.AddScoped<DeleteCommentCommand>();

// -- Prediction Module (ML modeli için)-------------------------
builder.Services.AddHttpClient<IPredictionService, PredictionService>(client =>
{
    var fastApiUrl = Environment.GetEnvironmentVariable("FASTAPI_BASE_URL")
        ?? builder.Configuration["FastApi:BaseUrl"]
        ?? "http://127.0.0.1:8000";
    client.BaseAddress = new Uri(fastApiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();