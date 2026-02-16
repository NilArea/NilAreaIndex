using Microsoft.Extensions.Logging;
using NilArea.Common.Services;
using NilArea.Interfaces.IGrains;

namespace NilArea.Grains.Services;

public interface IEmailServices : IAsyncLifetime
{
    ValueTask SendEmailAsync(string targetEmail, string title, string message);
}

public static class Helpers
{
    extension(IEmailServices emailServices)
    {
        public async ValueTask SendConfirmKeyAsync(string targetEmail, string confirmKey, ConfirmKey keyType)
        {
            //TODO 发送邮件
        }
    }
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
