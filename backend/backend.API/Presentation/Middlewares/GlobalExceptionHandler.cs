using Microsoft.AspNetCore.Diagnostics;

namespace backend.API.Presentation.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "İşlenmeyen hata: {Message}", exception.Message);

        var (status, message) = exception switch
        {
            InvalidOperationException => (400, exception.Message),
            UnauthorizedAccessException => (401, exception.Message),
            ArgumentException => (422, exception.Message),
            KeyNotFoundException => (404, exception.Message),
            _ => (500, "Sunucu hatası oluştu.")
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            statusCode = status,
            message,
            timestamp = DateTime.UtcNow
        }, cancellationToken);

        return true;
    }
}
