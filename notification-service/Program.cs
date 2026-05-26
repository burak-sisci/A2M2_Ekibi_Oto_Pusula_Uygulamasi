using DotNetEnv;
using NotificationService.Services;
using NotificationService.Workers;

Env.Load();

// FIREBASE_CREDENTIALS_B64 set edildiyse (HF Spaces / production), dosyaya yaz.
var b64 = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_B64");
if (!string.IsNullOrEmpty(b64))
{
    var path = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_PATH") ?? "firebase-credentials.json";
    await File.WriteAllBytesAsync(path, Convert.FromBase64String(b64));
}

// HF Spaces / Render gibi platformlar bir HTTP port bekler — bu yüzden Worker'ı
// minimal WebApplication içinde host ediyoruz, /health endpoint döner.
var builder = WebApplication.CreateBuilder(args);

// RabbitMQ tek connection
builder.Services.AddSingleton<RabbitMqConnection>();

// Servisler
builder.Services.AddSingleton<FcmSender>();
builder.Services.AddSingleton<SmtpSender>();
builder.Services.AddSingleton<MongoLookup>();

// Workers (BackgroundService olarak ASP.NET host'unda çalışır)
builder.Services.AddHostedService<CommentNotificationWorker>();
builder.Services.AddHostedService<EmailWorker>();

// Port — HF Spaces 7860, Render PORT env, yerel 5000 default
var port = Environment.GetEnvironmentVariable("PORT") ?? "7860";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "otopusula-notification", status = "ok" }));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// RabbitMQ connection'ı host başlamadan ayağa kaldır
var rabbit = app.Services.GetRequiredService<RabbitMqConnection>();
await rabbit.InitAsync();

await app.RunAsync();
