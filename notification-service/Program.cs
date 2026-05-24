using DotNetEnv;
using NotificationService.Services;
using NotificationService.Workers;

Env.Load();

// FIREBASE_CREDENTIALS_B64 set edildiyse (Fly.io / production), dosyaya yaz.
var b64 = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_B64");
if (!string.IsNullOrEmpty(b64))
{
    var path = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_PATH") ?? "firebase-credentials.json";
    await File.WriteAllBytesAsync(path, Convert.FromBase64String(b64));
}

var builder = Host.CreateApplicationBuilder(args);

// RabbitMQ tek connection
builder.Services.AddSingleton<RabbitMqConnection>();

// Servisler
builder.Services.AddSingleton<FcmSender>();
builder.Services.AddSingleton<SmtpSender>();
builder.Services.AddSingleton<MongoLookup>();

// Workers
builder.Services.AddHostedService<CommentNotificationWorker>();
builder.Services.AddHostedService<EmailWorker>();

var host = builder.Build();

// RabbitMQ connection'ı host başlamadan ayağa kaldır
var rabbit = host.Services.GetRequiredService<RabbitMqConnection>();
await rabbit.InitAsync();

await host.RunAsync();
