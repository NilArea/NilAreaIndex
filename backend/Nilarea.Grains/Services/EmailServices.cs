using Microsoft.Extensions.Logging;

namespace NilArea.Grains.Services;

public interface IEmailServices
{
    internal ValueTask InitializeAsync();
    ValueTask SendEmailAsync(string targetEmail, string title, string message);
}

public class EmailServices(ILogger<EmailServices> logger) : IEmailServices
{
    public async ValueTask InitializeAsync()
    {
        logger.LogInformation("Initializing email services...");
    }


    public async ValueTask SendEmailAsync(string targetEmail, string title, string message)
    {
    }
}
