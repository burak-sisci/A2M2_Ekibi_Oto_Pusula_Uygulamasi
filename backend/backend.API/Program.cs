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

// ── Cars Module ───────────────────────────────────────────────
builder.Services.AddScoped<ICarRepository, MongoCarRepository>();
builder.Services.AddScoped<GetCarsQuery>();
builder.Services.AddScoped<AddCarCommand>();
// ── Lists Module ──────────────────────────────────────────────
builder.Services.AddScoped<IListRepository, MongoListRepository>();
builder.Services.AddScoped<CreateDefaultListCommand>();
builder.Services.AddScoped<AddItemToListCommand>();
