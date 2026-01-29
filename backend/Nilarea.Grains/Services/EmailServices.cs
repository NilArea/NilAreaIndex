using Microsoft.Extensions.Logging;

namespace NilArea.Grains.Services;

public interface IEmailServices : IAsyncLifetime
{
    ValueTask SendEmailAsync(string targetEmail, string title, string message);
}

public class EmailServices(ILogger<EmailServices> logger) : IEmailServices
{
    public Task InitializeAsync()
    {
        logger.LogInformation("Initializing email services...");
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        logger.LogInformation("Disposing email services...");
        return Task.CompletedTask;
    }


    public ValueTask SendEmailAsync(string targetEmail, string title, string message)
    {
        return ValueTask.CompletedTask;
    }
}
